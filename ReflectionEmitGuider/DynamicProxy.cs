using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace ReflectionEmitGuider
{
    public class DynamicProxy
    {
        private readonly DynamicGenerator _generator;
        private readonly MethodAttributes MethodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual;
        private static readonly string[] _ignoreMethods = new[] { "GetType", "ToString", "GetHashCode", "Equals" };
        public DynamicProxy()
        {
            _generator = new DynamicGenerator();
        }

        public TIService CreateInstance<TIService, TImplement>()
            where TImplement : class, new()
        {
            return (TIService)CreateInstanceInternal(typeof(TIService), typeof(TImplement));
        }

        public TInterface CreateInstance<TInterface, TImplement>(params object[] parameters)
        {
            return (TInterface)CreateInstanceInternal(typeof(TInterface), typeof(TImplement), parameters);
        }

        private object CreateInstanceInternal(Type serviceType, Type implementType)
        {
            var assemblyBuilder = _generator.GetAssemblyBuilder();
            var moduleBuilder = _generator.GetModuleBuilder(assemblyBuilder);
            var typeBuilder = moduleBuilder.DefineType("DynamicProxy_" + implementType.Name, TypeAttributes.Public, null, new Type[] { serviceType });
            //定义构造函数
            var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
            var constructorInfo = implementType.GetConstructor(Type.EmptyTypes);
            //FieldBuilder _serviceImpObj = typeBuilder.DefineField("_serviceImpObj", implementType, FieldAttributes.Private);
            //初始化
            var constructIL = constructor.GetILGenerator();
            constructIL.Emit(OpCodes.Ldarg_0);
            constructIL.Emit(OpCodes.Call, constructorInfo);
            //constructIL.Emit(OpCodes.Newobj, implementType.GetConstructor(Type.EmptyTypes));
            //constructIL.Emit(OpCodes.Stfld, _serviceImpObj);
            constructIL.Emit(OpCodes.Ret);
            //方法体
            var methodInfos = serviceType.GetTypeInfo().GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var method in methodInfos)
            {
                if (_ignoreMethods.Contains(method.Name)) continue;
                var parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
                var methodBuilder = typeBuilder.DefineMethod(method.Name, MethodAttributes, CallingConventions.HasThis, method.ReturnType, parameterTypes);
                var methodIL = methodBuilder.GetILGenerator();
                methodIL.Emit(OpCodes.Ldarg_0);
                //methodIL.Emit(OpCodes.Ldfld, constructorInfo);
                //加载参数
                for (int i = 0; i < parameterTypes.Length; i++)
                {
                    methodIL.Emit(OpCodes.Ldarg, i + 1);
                }
                methodIL.Emit(OpCodes.Callvirt, implementType.GetMethod(method.Name));
                methodIL.Emit(OpCodes.Ret);
            }



            var type = typeBuilder.CreateTypeInfo();

            return Activator.CreateInstance(type);
        }

        private object CreateInstanceInternal(Type serviceType, Type implementType, params object[] parameters)
        {
            var assemblyBuilder = _generator.GetAssemblyBuilder();
            var moduleBuilder = _generator.GetModuleBuilder(assemblyBuilder);
            var typeBuilder = moduleBuilder.DefineType("DynamicProxy_" + implementType.Name, TypeAttributes.Public, null, new Type[] { serviceType });
            var constructorParameters = parameters.Select(p => p.GetType()).ToArray();
            //定义构造函数
            var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, constructorParameters);
            var constructorInfo = implementType.GetConstructor(constructorParameters);
            //FieldBuilder _serviceImpObj = typeBuilder.DefineField("_serviceImpObj", implementType, FieldAttributes.Private);
            //初始化
            var constructIL = constructor.GetILGenerator();
            constructIL.Emit(OpCodes.Ldarg_0);
            //加载构造函数参数
            for (int i = 0; i < constructorParameters.Length; i++)
            {
                constructIL.Emit(OpCodes.Ldarg, i + 1);
            }
            constructIL.Emit(OpCodes.Call, constructorInfo);
            //constructIL.Emit(OpCodes.Newobj, implementType.GetConstructor(Type.EmptyTypes));
            //constructIL.Emit(OpCodes.Stfld, _serviceImpObj);
            constructIL.Emit(OpCodes.Ret);
            //方法体
            var methodInfos = serviceType.GetTypeInfo().GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var method in methodInfos)
            {
                if (_ignoreMethods.Contains(method.Name)) continue;
                var parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
                var methodBuilder = typeBuilder.DefineMethod(method.Name, MethodAttributes, CallingConventions.HasThis, method.ReturnType, parameterTypes);
                var methodIL = methodBuilder.GetILGenerator();
                methodIL.Emit(OpCodes.Ldarg_0);
                //methodIL.Emit(OpCodes.Ldfld, constructorInfo);
                //加载参数
                for (int i = 0; i < parameterTypes.Length; i++)
                {
                    methodIL.Emit(OpCodes.Ldarg, i + 1);
                }
                methodIL.Emit(OpCodes.Callvirt, implementType.GetMethod(method.Name));
                methodIL.Emit(OpCodes.Ret);
            }



            var type = typeBuilder.CreateTypeInfo();

            return Activator.CreateInstance(type, parameters);
        }
    }
}
