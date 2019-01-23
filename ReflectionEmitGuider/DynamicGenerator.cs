using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace ReflectionEmitGuider
{
    public class DynamicGenerator
    {
        private const string AssemblyName = "ReflectionEmitGuider.DynamicAssembly";
        private const string ModuleName = "ReflectionEmitGuider.DynamicModule";
        private const string TypeName = "ReflectionEmitGuider.DynamicType";
        private readonly MethodAttributes MethodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig;
        public AssemblyBuilder GetAssemblyBuilder()
        {
            var assemblyName = new AssemblyName(AssemblyName);
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
            return assemblyBuilder;
        }

        public ModuleBuilder GetModuleBuilder(AssemblyBuilder assemblyBuilder)
        {
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(ModuleName);
            return moduleBuilder;
        }

        public TypeBuilder GetTypeBuilder(ModuleBuilder moduleBuilder, string className)
        {
            TypeBuilder typeBuilder = moduleBuilder.DefineType(TypeName + "_" + className, TypeAttributes.Public);
            return typeBuilder;
        }

        public MethodBuilder GetMethod(TypeBuilder typeBuilder, string methodName)
        {
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(methodName, MethodAttributes);
            return methodBuilder;
        }

        public MethodBuilder GetMethod(TypeBuilder typBuilder, string methodName, Type returnType, params Type[] parameterTypes)
        {
            MethodBuilder builder = typBuilder.DefineMethod(methodName, MethodAttributes, CallingConventions.HasThis, returnType, parameterTypes);
            return builder;
        }

        public MethodBuilder GetMethod(TypeBuilder typeBuilder, string methodName, Type returnType, string[] genericParameters, params Type[] parameterTypes)
        {
            var methodBuilder = typeBuilder.DefineMethod(methodName, MethodAttributes, CallingConventions.HasThis, returnType, parameterTypes);
            var genBuilders = methodBuilder.DefineGenericParameters(genericParameters);
            foreach (var genBuilder in genBuilders)
            {
                genBuilder.SetGenericParameterAttributes(GenericParameterAttributes.ReferenceTypeConstraint | GenericParameterAttributes.DefaultConstructorConstraint);
            }
            return methodBuilder;
        }

        private TypeBuilder GetTypeBuilder(ModuleBuilder moduleBuilder, string className, params string[] genericParameters)
        {
            TypeBuilder builder = moduleBuilder.DefineType(className, TypeAttributes.Public);
            var genericParametersBuilder = builder.DefineGenericParameters(genericParameters);
            foreach (var genBuilder in genericParametersBuilder)
            {
                genBuilder.SetGenericParameterAttributes(GenericParameterAttributes.ReferenceTypeConstraint | GenericParameterAttributes.DefaultConstructorConstraint);
            }
            return builder;
        }
    }
}
