using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Remoting.Proxies;
using System.Collections.Generic;

namespace BodyArchitect.Common
{
    [DebuggerStepThrough]
    public static class ServicesManager
    {
        static Dictionary<Type, Type> kernel = new Dictionary<Type, Type>();

        public static void AddComponent(string key, Type serviceType, Type implementationType)
        {
            kernel.Add(serviceType, implementationType);
        }

        public static T GetService<T>()
        {
            return (T)Activator.CreateInstance(kernel[typeof(T)]);
        }

        public static bool IsDesignMode
        {
            get
            {
                return LicenseManager.UsageMode == LicenseUsageMode.Designtime;
            }
        }

        public static void RemoveAllServices()
        {
            kernel.Clear();
        }
    }
}
