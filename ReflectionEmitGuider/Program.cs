using System;
using System.Reflection.Emit;

namespace ReflectionEmitGuider
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            CreateMethod();
        }

        private static void CreateMethod()
        {
            DynamicGenerator proxy = new DynamicGenerator();
            var assembly = proxy.GetAssemblyBuilder();
            var module = proxy.GetModuleBuilder(assembly);
            var typeBuilder = proxy.GetType(module, "DynamicProxyClass");
            Type[] tparams = { typeof(int), typeof(int) };
            MethodBuilder methodSum = proxy.GetMethod(typeBuilder, "Sum", typeof(float), tparams);
            ILGenerator generator = methodSum.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Add_Ovf);
            generator.Emit(OpCodes.Stloc_0);
            generator.Emit(OpCodes.Br_S);
            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Ret);
        }
    }
}
