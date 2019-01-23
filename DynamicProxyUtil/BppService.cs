using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicProxyUtil
{
    public class BppService
    {
        private readonly string _name;
        public string Name => _name;
        public BppService(string name)
        {
            _name = name;
        }
        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
