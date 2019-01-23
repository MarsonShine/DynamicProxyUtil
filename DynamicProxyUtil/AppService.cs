using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicProxyUtil
{
    public class AppService : IAppService
    {
        private string _name;
        public string Name => _name;
        public AppService()
        {
            _name = "Marson Shine";
            Console.WriteLine("Constructor!!");
        }

        public void Execute()
        {
            Console.WriteLine(Name);
        }

        public void ChangedName(string name)
        {
            _name = name;
        }

        public string GetFirstName()
        {
            return _name.Split(' ')[0];
        }
    }
}
