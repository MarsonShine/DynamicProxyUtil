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

        public TypeBuilder GetType(ModuleBuilder moduleBuilder, string className)
        {
            TypeBuilder typeBuilder = moduleBuilder.DefineType(className, TypeAttributes.Public);
            return typeBuilder;
        }

        private TypeBuilder GetType(ModuleBuilder moduleBuilder, string className, params string[] genericParameters) {
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
