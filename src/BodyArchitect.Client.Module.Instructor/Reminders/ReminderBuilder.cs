using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Module.Instructor.Reminders;
using BodyArchitect.Client.UI.SchedulerEngine;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using Quartz;
using Quartz.Impl.Triggers;

namespace BodyArchitect.Client.Module.Instructor
{
    public class ReminderBuilder
    {
        private ITimerService timerService;

        public ReminderBuilder(ITimerService timerService)
        {
            this.timerService = timerService;
        }

        public ReminderBuilder():this(new TimerService())
        {
            
        }
        public IList<Tuple<IJobDetail,ITrigger>> Import(params ReminderItemDTO[] items)
        {
            var result = new List<Tuple<IJobDetail, ITrigger>>();

            foreach (var reminderItemDto in items)
            {
                string name = reminderItemDto.GlobalId.ToString();
                var builder = JobBuilder.Create(typeof (ReminderJob));
                var jobDetails = builder.WithIdentity(name).Build();
                //var jobDetail = new JobDetail(reminderItemDto.GlobalId.ToString(), typeof (ReminderJob));
                jobDetails.JobDataMap.Add("ReminderItemDTO", reminderItemDto);
                DateTime startDate = reminderItemDto.RemindBefore.HasValue ? reminderItemDto.DateTime -reminderItemDto.RemindBefore.Value:reminderItemDto.DateTime;
                ITrigger trigger = null;
                if (reminderItemDto.Repetitions==ReminderRepetitions.Once)
                {
                    //do not show old reminders (older than 12 hours)
                    if ((reminderItemDto.DateTime.TimeOfDay==TimeSpan.Zero && reminderItemDto.DateTime!=DateTime.UtcNow.Date) //birthday (date without time) but not today
                        || (reminderItemDto.DateTime.TimeOfDay != TimeSpan.Zero && (DateTime.UtcNow - reminderItemDto.DateTime).TotalHours > 12))
                    {
                        continue;
                    }
                    var onceTrigger = new SimpleTriggerImpl(name, startDate);
                    onceTrigger.MisfireInstruction = MisfireInstruction.SimpleTrigger.FireNow;
                    
                    trigger = onceTrigger;
                    
                }
                else
                {
                    var now = timerService.UtcNow;
                    CalendarIntervalTriggerImpl calendarTrigger = null;
                    bool fireOnce = true;
                    if (reminderItemDto.Repetitions == ReminderRepetitions.EveryYear)
                    {
                        calendarTrigger = new CalendarIntervalTriggerImpl(name, IntervalUnit.Year, 1);
                        fireOnce = reminderItemDto.DateTime.Day == now.Day && reminderItemDto.DateTime.Month == now.Month;
                    }
                    else if (reminderItemDto.Repetitions == ReminderRepetitions.EveryMonth)
                    {
                        calendarTrigger = new CalendarIntervalTriggerImpl(name, IntervalUnit.Month, 1);
                        fireOnce = reminderItemDto.DateTime.Day == now.Day;
                    }
                    else if (reminderItemDto.Repetitions == ReminderRepetitions.EveryWeek)
                    {
                        calendarTrigger = new CalendarIntervalTriggerImpl(name, IntervalUnit.Week, 1);
                        fireOnce = reminderItemDto.DateTime.DayOfWeek == now.DayOfWeek;
                    }
                    else
                    {
                        calendarTrigger = new CalendarIntervalTriggerImpl(name, IntervalUnit.Day, 1);
                    }

                    if (fireOnce && reminderItemDto.DateTime.TimeOfDay == TimeSpan.Zero && (reminderItemDto.LastShown == null || now.Date != reminderItemDto.LastShown.Value.Date))
                    {
                        calendarTrigger.MisfireInstruction = MisfireInstruction.CalendarIntervalTrigger.FireOnceNow;
                    }
                    else
                    {
                        calendarTrigger.MisfireInstruction = MisfireInstruction.CalendarIntervalTrigger.DoNothing;
                    }

                    calendarTrigger.StartTimeUtc = startDate;
                    if (reminderItemDto.DateTime.TimeOfDay == TimeSpan.Zero)
                    {//if there is no Time set then this means All day
                        calendarTrigger.EndTimeUtc = calendarTrigger.StartTimeUtc.AddDays(1).AddSeconds(-1);
                    }
                    

                    trigger = calendarTrigger;
                }
                result.Add(new Tuple<IJobDetail, ITrigger>(jobDetails, trigger));
                
            }
            return result;
        }

        public static void Fill(bool update, params ReminderItemDTO[] items)
        {
            Scheduler.Remove(items.Select(x => x.GlobalId.ToString()).ToArray());
            if (update)
            {//when we update than we should remove old instances from NotificationsReposidory
                foreach (var item in items)
                {
                    NotificationsReposidory.Instance.Remove(item.GlobalId);
                }
            }
            ReminderBuilder builder = new ReminderBuilder();
            var jobs = builder.Import(items);
            foreach (var reminderItemDto in jobs)
            {
                Scheduler.Add(reminderItemDto.Item1, reminderItemDto.Item2);
            }
        }
    }
}
