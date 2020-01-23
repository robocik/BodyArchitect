using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;


namespace BodyArchitect.WP7.Controls.Cache
{
    public interface IServiceCommand
    {
        void Execute();
    }

    public static class ServicePool
    {
        static Queue<IServiceCommand> queue = new Queue<IServiceCommand>();
        static object syncObject = new object();
        private static ManualResetEvent manualEvent = new ManualResetEvent(false);

        static ServicePool()
        {
            ThreadPool.QueueUserWorkItem(delegate
               {
                   IServiceCommand command = null;
                   while (true)
                   {
                       manualEvent.WaitOne();
                       command = null;
                       lock (syncObject)
                       {
                           if (queue.Count > 0)
                           {
                               command = queue.Dequeue();
                           }
                       }
                       if (command == null)
                       {
                           manualEvent.Reset();
                           continue;
                       }
                       try
                       {
                           command.Execute();
                       }
                       catch
                       {

                       }

                   }    
               });

        }

        public static void Add(IServiceCommand command)
        {
            lock (syncObject)
            {
                queue.Enqueue(command);
                manualEvent.Set();
            }

        }
    }
}
