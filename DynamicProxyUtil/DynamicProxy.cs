using DynamicProxyUtil.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace DynamicProxyUtil
{
    public class DynamicProxy
    {
        private static readonly string[] _ignoreMethodName = new[] { "GetType", "ToString", "GetHashCode", "Equals" };

        public static TInterface CreateProxyOfRealize<TInterface, TImp>() where TImp : class, new() where TInterface : class
        {
            return Invoke<TInterface, TImp>();
        }

        public static TProxyClass CreateProxyOfInherit<TProxyClass>() where TProxyClass : class, new()
        {
            return Invoke<TProxyClass, TProxyClass>(true);
        }

        private static TInterface Invoke<TInterface, TImp>(bool inheritMode = false, Type interceptorType = null) where TImp : class where TInterface : class
        {
            if (inheritMode)
                return CreateProxyOfInherit(typeof(TImp), interceptorType) as TInterface;
            else
                return CreateProxyOfRealize(typeof(TInterface), typeof(TImp), interceptorType) as TInterface;
        }

        public static object CreateProxyOfRealize(Type interfaceType, Type impType, Type interceptorType = null)
        {
            string nameOfAssembly = impType.Name + "ProxyAssembly";
            string nameOfModule = impType.Name + "ProxyModule";
            string nameOfType = impType.Name + "Proxy";

            AssemblyBuilder assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(nameOfAssembly), AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assembly.DefineDynamicModule(nameOfModule);
            TypeBuilder typeBuilder = moduleBuilder.DefineType(nameOfType, TypeAttributes.Public, null, new[] { interfaceType });
            MethodAttributes methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final;

            return Invoke(impType, typeBuilder, methodAttributes, interceptorType);
        }

        public static object CreateProxyOfInherit(Type impType, Type interceptorType = null)
        {
            string nameOfAssembly = impType.Name + "ProxyAssembly";
            string nameOfModule = impType.Name + "ProxyModule";
            string nameOfType = impType.Name + "Proxy";

            AssemblyBuilder assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(nameOfAssembly), AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assembly.DefineDynamicModule(nameOfModule);
            TypeBuilder typeBuilder = moduleBuilder.DefineType(nameOfType, TypeAttributes.Public, impType);
            MethodAttributes methodAttributes = MethodAttributes.Public | MethodAttributes.Virtual;

            return Invoke(impType, typeBuilder, methodAttributes, interceptorType);
        }

        private static object Invoke(Type impType, TypeBuilder typeBuilder, MethodAttributes methodAttributes, Type interceptorType = null)
        {
            Type interceptorAttributeType = impType.GetCustomAttribute(typeof(InterceptorBaseAttribute))?.GetType() ?? interceptorType;

            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, null);
            var ilOfCtor = constructorBuilder.GetILGenerator();
            // ---- define fields ----
            //FieldBuilder fieldInterceptor = null;
            //if (interceptorAttributeType != null)
            //{
            //    fieldInterceptor = typeBuilder.DefineField("_interceptor", interceptorAttributeType, FieldAttributes.Private);
            //    ilOfCtor.Emit(OpCodes.Ldarg_0);
            //    ilOfCtor.Emit(OpCodes.Newobj, interceptorAttributeType.GetConstructor(new Type[0]));
            //    ilOfCtor.Emit(OpCodes.Stfld, fieldInterceptor);
            //}

            //initial field of impObj
            FieldBuilder _serviceImpObj = typeBuilder.DefineField("_serviceImpObj", impType, FieldAttributes.Private);
            ilOfCtor.Emit(OpCodes.Ldarg_0);
            ilOfCtor.Emit(OpCodes.Newobj, impType.GetConstructor(new Type[0]));
            ilOfCtor.Emit(OpCodes.Stfld, _serviceImpObj);

            ilOfCtor.Emit(OpCodes.Ret);

            // ---- define methods ----

            var methodsOfType = impType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

            foreach (var method in methodsOfType)
            {
                //ignore method
                if (_ignoreMethodName.Contains(method.Name))
                    continue;

                var methodParameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();

                var methodBuilder = typeBuilder.DefineMethod(method.Name, methodAttributes, CallingConventions.Standard, method.ReturnType, methodParameterTypes);

                var ilMethod = methodBuilder.GetILGenerator();

                //if (method.ReturnType == typeof(void)) {
                //    ilMethod.Emit(OpCodes.Ldnull);
                //}
                ilMethod.Emit(OpCodes.Ldarg_0);//this
                ilMethod.Emit(OpCodes.Ldfld, _serviceImpObj);
                for (var j = 0; j < methodParameterTypes.Length; j++)
                {
                    ilMethod.Emit(OpCodes.Ldarg, j + 1);
                }
                ilMethod.Emit(OpCodes.Callvirt, impType.GetMethod(method.Name));
                //if (method.ReturnType == typeof(void)) {

                //}
                // complete
                ilMethod.Emit(OpCodes.Ret);
            }

            var typeInfo = typeBuilder.CreateTypeInfo();

            return Activator.CreateInstance(typeInfo);
        }
    }
}
