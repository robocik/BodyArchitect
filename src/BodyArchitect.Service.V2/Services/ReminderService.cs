using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using BodyArchitect.Service.V2.Localization;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Criterion;
using Profile = BodyArchitect.Model.Profile;
using ReminderRepetitions = BodyArchitect.Model.ReminderRepetitions;
using ReminderType = BodyArchitect.Model.ReminderType;

namespace BodyArchitect.Service.V2.Services
{
    public class ReminderService: ServiceBase
    {
        //private static readonly TimeSpan MaxRemindBefore = TimeSpan.FromDays(7);

        public ReminderService(ISession session, SecurityInfo securityInfo, ServiceConfiguration configuration)
            : base(session, securityInfo, configuration)
        {
        }

        public ReminderItemDTO SaveReminder(ReminderItemDTO reminder)
        {
            Log.WriteWarning("SaveReminder:Username={0},GlobalId={1}", SecurityInfo.SessionData.Profile.UserName,reminder.GlobalId);

            if (!SecurityInfo.Licence.IsPremium)
            {
                throw new LicenceException("This feature is allowed for Premium account");
            }

            var dbReminder = reminder.Map<ReminderItem>();
            using (var trans = Session.BeginSaveTransaction())
            {
                Profile dbProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);

                ReminderItem db = Session.Get<ReminderItem>(reminder.GlobalId);
                if(db!=null)
                {
                    if (SecurityInfo.SessionData.Profile.GlobalId != db.Profile.GlobalId)
                    {
                        throw new CrossProfileOperationException("Cannot modify Reminder for another user");
                    }
                }
                dbReminder.Profile = dbProfile;

                if (reminder.RemindBefore.HasValue && reminder.RemindBefore.Value.TotalDays>7)
                {
                    throw new ArgumentOutOfRangeException("RemindBefore can be maximum 7 days");
                }
                int res = Session.QueryOver<ReminderItem>().Where(x => x.Name == dbReminder.Name && x.GlobalId != dbReminder.GlobalId && x.Profile == dbProfile).RowCount();
                if (res > 0)
                {
                    throw new UniqueException("Reminder with the same name is already exist");
                }

                dbReminder = Session.Merge(dbReminder);
                dbProfile.DataInfo.ReminderHash = Guid.NewGuid();
                trans.Commit();
                return dbReminder.Map<ReminderItemDTO>();
            }
        }

        static string getReminderName(DateTime dateTime, ReminderType type, IRemindable dtoEntry, IHasReminder entry)
        {
            if(type==ReminderType.Birthday)
            {
                return string.Format(LocalizedStrings.ReminderBirthday, ((CustomerDTO)dtoEntry).FullName, dateTime.Date.ToShortDateString());
            }
            else if(type==ReminderType.ScheduleEntry)
            {
                return string.Format(LocalizedStrings.ReminderScheduleEntry, dateTime.Date,((ScheduleEntry)entry).Activity.Name);
            }
            else if (type == ReminderType.EntryObject)
            {
                if (entry is SuplementsEntry)
                {
                    return string.Format(LocalizedStrings.ReminderSupplements,dateTime.Date.ToShortDateString());    
                }
                if (entry is StrengthTrainingEntry)
                {
                    return string.Format(LocalizedStrings.ReminderStrengthTraining, dateTime.ToShortDateString());
                }
                if (entry is SizeEntry)
                {
                    return string.Format(LocalizedStrings.ReminderMeasurements, dateTime.Date.ToShortDateString());
                }
            }
            return "TODO:Name not set";
        }

        

        public void PrepareReminder(Profile dbProfile, IRemindable dtoEntry, IHasReminder entry, IHasReminder origEntry, DateTime dateTime,ReminderType type,ReminderRepetitions repetitions)
        {
            if (dtoEntry.RemindBefore.HasValue)
            {

                if (origEntry==null ||origEntry.Reminder == null)
                {
                    entry.Reminder = new ReminderItem();
                }
                else
                {
                    entry.Reminder = origEntry.Reminder;
                }
                //entry.Reminder.ConnectedObject = entry.ToString();
                entry.Reminder.DateTime = dateTime;
                entry.Reminder.Profile = dbProfile;
                entry.Reminder.Type = type;
                entry.Reminder.RemindBefore = dtoEntry.RemindBefore != TimeSpan.Zero
                                                  ? dtoEntry.RemindBefore.Value
                                                  : (TimeSpan?)null;
                entry.Reminder.Repetitions = repetitions;
                entry.Reminder.Name = getReminderName(dateTime, type, dtoEntry, entry);
                Session.SaveOrUpdate(entry.Reminder);
                dbProfile.DataInfo.ReminderHash = Guid.NewGuid();
            }
            else if (origEntry!=null && origEntry.Reminder != null)
            {
                Session.Delete(origEntry.Reminder);
                entry.Reminder = null;
                dbProfile.DataInfo.ReminderHash = Guid.NewGuid();
            }
        }

        public PagedResult<ReminderItemDTO> GetReminders(GetRemindersParam remindersParam, PartialRetrievingInfo pageInfo)
        {
            using (var tx = Session.BeginGetTransaction())
            {
                //DateTime now = Configuration.TimerService.UtcNow;
                Profile dbProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);

                //var query = Session.CreateQuery("SELECT ri FROM ReminderItem ri WHERE (ri.DateTime-ri.RemindBefore)>=?");
                //query.SetDateTime(0, now);
                //var query = Session.CreateQuery("SELECT ri.DateTime-ri.RemindBefore FROM ReminderItem ri ");
                //var test=query.List<object>();
                //take all reminders from one week since current time

                var query = Session.QueryOver<ReminderItem>().Where(x => x.Profile == dbProfile);

                //if(remindersParam.ValidForTime.HasValue)
                //{
                //    var langOr = Restrictions.Disjunction();
                //    langOr.Add(Restrictions.Between("DateTime", now, now + remindersParam.ValidForTime.Value + MaxRemindBefore));
                //    langOr.Add(Expression.IsNotNull("RepeatPattern"));
                //    query = query.And(langOr);
                //}

                if (remindersParam.Types!=null && remindersParam.Types.Count > 0)
                {
                    var langOr = Restrictions.Disjunction();
                    foreach (var type in remindersParam.Types)
                    {
                        langOr.Add<ReminderItem>(x => x.Type == (ReminderType) type);
                    }
                    query = query.And(langOr);

                }

                //var res = query.List();
                
                ////now for pattern reminders
                //if (remindersParam.ValidForTime.HasValue)
                //{
                //    List<ReminderItem> reminders = new List<ReminderItem>();
                //    reminders.AddRange(evaluatePattern(res, now,remindersParam.ValidForTime.Value));
                //    reminders.AddRange(res.Where(x => string.IsNullOrWhiteSpace(x.RepeatPattern)).ToList());
                //    return reminders.Map<IList<ReminderItemDTO>>();
                //}
                //return res.Map<IList<ReminderItemDTO>>();
                var listPack = query.ToPagedResults<ReminderItemDTO, ReminderItem>(pageInfo);
                tx.Commit();
                return listPack;
            }
            
        }

        public ReminderItemDTO ReminderOperation(ReminderOperationParam remindersParam)
        {
            ReminderItemDTO res = null;
            using (var tx = Session.BeginSaveTransaction())
            {
                //DateTime now = Configuration.TimerService.UtcNow;
                Profile dbProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
                var dbReminder = Session.Get<ReminderItem>(remindersParam.ReminderItemId);

                if(dbReminder.Profile!=dbProfile)
                {
                    throw new CrossProfileOperationException("Cannot change reminder from another user");
                }

                if(remindersParam.Operation==ReminderOperationType.Delete)
                {
                    deleteReminder(dbReminder,true);
                }
                else if (remindersParam.Operation == ReminderOperationType.CloseAfterShow)
                {
                    if (dbReminder.Repetitions==ReminderRepetitions.Once)
                    {
                        deleteReminder(dbReminder,true);
                    }
                    else
                    {
                        dbReminder.LastShown = Configuration.TimerService.UtcNow;
                        Session.Update(dbReminder);
                        res= dbReminder.Map<ReminderItemDTO>();
                    }
                }
                tx.Commit();
                return res;
                
            }
        }

        private void deleteReminder(ReminderItem dbReminder,bool markAsModified)
        {
            var connectedObject = getConnectedObject(Session, dbReminder);
            if (connectedObject != null)
            {
                connectedObject.Reminder = null;
            }
            if (markAsModified)
            {
                dbReminder.Profile.DataInfo.ReminderHash = Guid.NewGuid();
            }
            Session.Delete(dbReminder);
        }

        IHasReminder getConnectedObject(ISession session,ReminderItem item)
        {
            if(!string.IsNullOrEmpty(item.ConnectedObject))
            {
                var arr=item.ConnectedObject.Split(':');
                return (IHasReminder)session.Get(Type.GetType("BodyArchitect.Model." + arr[0].Replace("DTO", "") + ",BodyArchitect.Model"), new Guid(arr[1]));
            }
            return null;
        }

        //public static ReminderItem CreateReminder<T>(ISession session,TimeSpan? remindBefore,Profile dbProfile,string name,DateTime time)
        //{
        //    ReminderItem reminder = null;
        //    if (remindBefore.HasValue)
        //    {
        //        reminder = new ReminderItem();

        //        reminder.DateTime = time;
        //        reminder.Profile = dbProfile;
        //        reminder.Type = ReminderType.EntryObject;
        //        reminder.RemindBefore = remindBefore!= TimeSpan.Zero
        //                                          ? remindBefore.Value
        //                                          : (TimeSpan?)null;
        //        reminder.Repetitions = ReminderRepetitions.Once;
        //        reminder.Name = name;
        //        reminder.ConnectedObject = typeof(T).Name+":{0}";
        //        session.Save(reminder);
        //        //dbProfile.DataInfo.LastReminderModification=...
        //    }
        //    return reminder;
        //}

        public static void CreateReminder<T>(ISession session,IHasReminder hasReminder, TimeSpan? remindBefore, Profile dbProfile, DateTime time,ReminderType type)
        {
            if (hasReminder.Reminder == null)
            {
                hasReminder.Reminder = new ReminderItem();
            }
            hasReminder.Reminder.DateTime = time;
            hasReminder.Reminder.Profile = dbProfile;
            hasReminder.Reminder.Type = type;
            hasReminder.Reminder.RemindBefore = remindBefore != TimeSpan.Zero
                                              ? remindBefore.Value
                                              : (TimeSpan?)null;
            hasReminder.Reminder.Repetitions = ReminderRepetitions.Once;
            hasReminder.Reminder.Name = getReminderName(time,type,null,hasReminder);
            hasReminder.Reminder.ConnectedObject = typeof(T).Name + ":{0}";

            dbProfile.DataInfo.ReminderHash = Guid.NewGuid();
            //dbProfile.DataInfo.LastReminderModification=...
        }

        internal void RemoveOldReminders(Guid profileId)
        {
            var oldReminders = Session.QueryOver<ReminderItem>().Where(x => x.Profile.GlobalId == profileId && x.DateTime < DateTime.UtcNow.Date.AddDays(-1) && x.Repetitions == ReminderRepetitions.Once).List();
            foreach (var reminderItem in oldReminders)
            {
                deleteReminder(reminderItem,false);
            }
            //TODO: Should we mark reminders as changed?
            //dbProfile.DataInfo.ReminderHash = Guid.NewGuid();
        }
    }

}
