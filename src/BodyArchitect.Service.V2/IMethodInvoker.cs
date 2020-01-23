using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace BodyArchitect.Service.V2
{
    public interface IMethodInvoker
    {
        void Invoke(WaitCallback method, object param);
    }

    public class ThreadPoolMethodInvoker : IMethodInvoker
    {
        public void Invoke(WaitCallback method, object param)
        {
            ThreadPool.QueueUserWorkItem(method,param);
        }
    }
}
