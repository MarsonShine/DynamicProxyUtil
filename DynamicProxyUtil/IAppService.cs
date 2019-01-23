using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicProxyUtil
{
    public interface IAppService
    {
        string Name { get; }
        void Execute();
        void ChangedName(string name);

        string GetFirstName();
    }
}
