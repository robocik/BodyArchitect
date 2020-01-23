using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BodyArchitect.Logger;

namespace BodyArchitect.Client.WCF
{
    public class ServiceCommand : IServiceCommand
    {
        private Action action;

        public ServiceCommand(Action action)
        {
            this.action = action;
        }

        public void Execute()
        {
            try
            {
                action();
                
            }
            catch(Exception ex)
            {//todo:maybe we should notify about error in some way?
                ExceptionHandler.Default.Process(ex);
            }
        }
    }
    public interface IServiceCommand
    {
        void Execute();
    }

    public static class ServicePool
    {
        //TODO:Maybe change to concurencyqueue
        //private static ConcurrentQueue<QueueObject> bc = new ConcurrentQueue<QueueObject>();
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
