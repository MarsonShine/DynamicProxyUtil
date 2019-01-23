using System;

namespace DynamicProxyUtil
{
    public class Program
    {
        static void Main(string[] args)
        {
            //var proxy = new DynamicProxy();
            var appservice = DynamicProxy.CreateProxyOfRealize<IAppService, AppService>();
            //IAppService appService = new BppService("marson shine");
            Console.WriteLine("Hello World!" + appservice.Name);
            Console.ReadLine();
        }
    }
}
