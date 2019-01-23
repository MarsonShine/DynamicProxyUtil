using System;
using System.Reflection.Emit;

namespace ReflectionEmitGuider
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            IBuilder builder = new Builder(1, 2);
            DynamicProxy proxy = new DynamicProxy();
            //var imp = proxy.CreateInstance<IBuilder, Builder>();
            var impArg = proxy.CreateInstance<IBuilder, Builder>(4, 5);
            //CreateMethod();
        }

        private static void CreateMethod()
        {
            DynamicGenerator proxy = new DynamicGenerator();
            var assembly = proxy.GetAssemblyBuilder();
            var module = proxy.GetModuleBuilder(assembly);
            var typeBuilder = proxy.GetTypeBuilder(module, "DynamicProxyClass");
            Type[] tparams = { typeof(int), typeof(int) };
            MethodBuilder methodSum = proxy.GetMethod(typeBuilder, "Sum", typeof(int), tparams);
            ILGenerator generator = methodSum.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Add_Ovf);
            generator.Emit(OpCodes.Stloc_0);
            generator.Emit(OpCodes.Br_S);
            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Ret);
            Type type = typeBuilder.CreateType();

            var obj = Activator.CreateInstance(type);
            var ret = obj.GetType().GetMethod("Sum").Invoke(obj, new object[] { 2, 3 });
            Console.WriteLine(ret);
        }
    }
}
