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

        private Type CreateIBuilder(ModuleBuilder mbuilder)
        {

            TypeBuilder tbuilder = mbuilder.DefineType("IBuilder", TypeAttributes.Interface |
                TypeAttributes.Public |
                TypeAttributes.Abstract |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass);

            //TypeBuilder tBuilder = mbuilder.DefineType("Builder", TypeAttributes.Public |
            //    TypeAttributes.AutoClass |
            //    TypeAttributes.AnsiClass |
            //    TypeAttributes.BeforeFieldInit,
            //    typeof(object),
            //    new Type[] { });

            //Define Divide
            Type[] tparams = { typeof(System.Int32), typeof(System.Int32) };
            MethodBuilder metDivide = tbuilder.DefineMethod("Divide", MethodAttributes.Public |
                MethodAttributes.Abstract |
                MethodAttributes.Virtual |
                MethodAttributes.HideBySig |
                MethodAttributes.NewSlot,
                CallingConventions.HasThis,
                typeof(System.Single), tparams);
            metDivide.SetImplementationFlags(MethodImplAttributes.Managed);

            MethodBuilder metSum = tbuilder.DefineMethod("Sum", MethodAttributes.Public |
                MethodAttributes.Abstract |
                MethodAttributes.Virtual |
                MethodAttributes.HideBySig |
                MethodAttributes.NewSlot,
                CallingConventions.HasThis,
                typeof(System.Single), tparams);
            metSum.SetImplementationFlags(MethodImplAttributes.Managed);

            MethodBuilder metMultiply = tbuilder.DefineMethod("Multiply", MethodAttributes.Public |
                MethodAttributes.Abstract |
                MethodAttributes.Virtual |
                MethodAttributes.HideBySig |
                MethodAttributes.NewSlot,
                CallingConventions.HasThis,
                typeof(System.Single), tparams);
            metMultiply.SetImplementationFlags(MethodImplAttributes.Managed);

            MethodBuilder metSubstract = tbuilder.DefineMethod("Substract", MethodAttributes.Public |
                MethodAttributes.Abstract |
                MethodAttributes.Virtual |
                MethodAttributes.HideBySig |
                MethodAttributes.NewSlot,
                CallingConventions.HasThis,
                typeof(System.Single), tparams);
            metSubstract.SetImplementationFlags(MethodImplAttributes.Managed);

            Type tIBuilder = tbuilder.CreateType();


            return tIBuilder;
        }

        private void HandleFieldAndProperties(TypeBuilder tbuilder) {
            FieldBuilder fFirst = tbuilder.DefineField("firstNum", typeof(System.Int32), FieldAttributes.Private);
            PropertyBuilder pFirst = tbuilder.DefineProperty("FirstNum", PropertyAttributes.HasDefault, typeof(System.Int32), null);
            //Getter
            MethodBuilder mFirstGet = tbuilder.DefineMethod("get_FirstNum", MethodAttributes.Public |
                MethodAttributes.SpecialName |
                MethodAttributes.HideBySig, typeof(System.Int32), Type.EmptyTypes);
            ILGenerator firstGetIL = mFirstGet.GetILGenerator();

            firstGetIL.Emit(OpCodes.Ldarg_0);
            firstGetIL.Emit(OpCodes.Ldfld, fFirst);
            firstGetIL.Emit(OpCodes.Ret);

            //Setter
            MethodBuilder mFirstSet = tbuilder.DefineMethod("set_FirstNum", MethodAttributes.Public |
                MethodAttributes.SpecialName |
                MethodAttributes.HideBySig, null, new Type[] { typeof(System.Int32) });

            ILGenerator firstSetIL = mFirstSet.GetILGenerator();

            firstSetIL.Emit(OpCodes.Ldarg_0);
            firstSetIL.Emit(OpCodes.Ldarg_1);
            firstSetIL.Emit(OpCodes.Stfld, fFirst);
            firstSetIL.Emit(OpCodes.Ret);

            pFirst.SetGetMethod(mFirstGet);
            pFirst.SetSetMethod(mFirstSet);
        }

        private void HandleMethodWithTryCatch(TypeBuilder tbuilder) {
            MethodBuilder mDivide = tbuilder.DefineMethod("Divide", MethodAttributes.Public |
                MethodAttributes.HideBySig |
                MethodAttributes.NewSlot |
                MethodAttributes.Virtual |
                MethodAttributes.Final,
                CallingConventions.Standard, typeof(Single), new Type[] { typeof(int), typeof(int) });
            mDivide.SetImplementationFlags(MethodImplAttributes.Managed);   //cli managed
            var dil = mDivide.GetILGenerator();
            dil.Emit(OpCodes.Nop);
            var lblTry = dil.BeginExceptionBlock();
            dil.Emit(OpCodes.Ldarg_0);
            dil.Emit(OpCodes.Ldarg_1);
            dil.Emit(OpCodes.Div);
            dil.Emit(OpCodes.Conv_R4);
            dil.Emit(OpCodes.Stloc_0);
            dil.Emit(OpCodes.Leave, lblTry);

            dil.BeginCatchBlock(typeof(DivideByZeroException));
            dil.Emit(OpCodes.Stloc_1);
            dil.Emit(OpCodes.Nop);
            dil.Emit(OpCodes.Ldstr, "ZeroDivide exception : {0}");
            dil.Emit(OpCodes.Stloc_1);
            var dbzExceptonMethod = typeof(DivideByZeroException).GetMethod("get_Message");
            dil.Emit(OpCodes.Callvirt, dbzExceptonMethod);
            dil.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string), typeof(object) }));
            dil.Emit(OpCodes.Nop);
            dil.Emit(OpCodes.Ldc_R4,0.0);
            dil.Emit(OpCodes.Stloc_0);
            dil.Emit(OpCodes.Leave_S, lblTry);

            dil.EndExceptionBlock();
            dil.Emit(OpCodes.Ldloc_0);
            dil.Emit(OpCodes.Ret);
        }
        private void HandleSumMethod(TypeBuilder tbuilder) {
            MethodBuilder mDivide = tbuilder.DefineMethod("Divide", MethodAttributes.Public |
                MethodAttributes.HideBySig |
                MethodAttributes.NewSlot |
                MethodAttributes.Virtual |
                MethodAttributes.Final,
                CallingConventions.Standard, typeof(Single), new Type[] { typeof(int), typeof(int) });
            mDivide.SetImplementationFlags(MethodImplAttributes.Managed);   //cli managed
            var dil = mDivide.GetILGenerator();
            dil.Emit(OpCodes.Nop);
            dil.Emit(OpCodes.Ldarg_0);
            dil.Emit(OpCodes.Ldarg_1);
            dil.Emit(OpCodes.Add);
            dil.Emit(OpCodes.Conv_R4);
            dil.Emit(OpCodes.Stloc_0);
            var endofmethodSum = dil.DefineLabel();

            dil.Emit(OpCodes.Br_S, endofmethodSum);
            dil.Emit(OpCodes.Ldloc_0);
            dil.MarkLabel(endofmethodSum);  //标记label所在的地方
            dil.Emit(OpCodes.Ret);
        }
    }
}
