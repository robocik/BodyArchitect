using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Windows.Threading;
using BodyArchitect.Client.Common;
using BodyArchitect.Service.V2.Model;
using Quartz;
using Quartz.Impl;
using System.Linq;
using Quartz.Impl.Triggers;

namespace BodyArchitect.Client.UI.SchedulerEngine
{
    public static class Scheduler
    {
        private static IScheduler sched;
        private static ISchedulerFactory schedFact;


        public static void Init()
        {
            // construct a scheduler factory
            schedFact = new StdSchedulerFactory();

            NameValueCollection properties = new NameValueCollection();
            properties["quartz.scheduler.instanceName"] = "BodyArchitect";
                        properties["quartz.scheduler.instanceId"] = "BodyArchitect";
            properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
            properties["quartz.threadPool.threadCount"] = "10";
            properties["quartz.threadPool.threadPriority"] = "Normal";
            properties["quartz.scheduler.instanceName"] = "TestScheduler";
            properties["quartz.scheduler.instanceId"] = "instance_one";


            schedFact = new StdSchedulerFactory(properties);

            // get a scheduler
            sched = schedFact.GetScheduler();
            
            
        }

        public static void Start()
        {
            sched.Start();
        }

        public static void Pause()
        {
            sched.Standby();
        }

        public static void Add(IJobDetail job,ITrigger trigger)
        {
            sched.ScheduleJob(job, trigger);
        }


        public static IJobDetail GetJob(string jobName)
        {
            return sched.GetJobDetail(new JobKey(jobName));
        }

        public static void Close()
        {
            if (sched != null)
            {
                sched.Shutdown();
                sched = null;
            }
        }

        public static void Remove(params string[] name)
        {
            sched.DeleteJobs(name.Select(x=>new JobKey(x)).ToList());
        }

        public static void RemoveAll()
        {
            sched.Clear();
        }

        public static void Ensure()
        {
            if (UserContext.IsPremium)
            {
                Start();
            }
            else
            {
                Pause();
            }
        }
    }
}
