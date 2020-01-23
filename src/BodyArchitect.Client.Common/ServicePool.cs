using System;
using System.Collections.Generic;

using System.Threading;
using System.Threading.Tasks;
using BodyArchitect.Logger;

namespace BodyArchitect.Client.Common
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
            var task = Task.Factory.StartNew(delegate(object cancellationToken)
                                                 {
                                                     IServiceCommand command = null;
                                                     while (true)
                                                     {
                                                         manualEvent.WaitOne();
                                                         command = null;
                                                         lock(syncObject)
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
                                                         catch (Exception ex)
                                                         {
                                                             ExceptionHandler.Default.Process(ex);
                                                         }
                                                         
                                                     }
                                                 }, null,TaskCreationOptions.LongRunning);
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
