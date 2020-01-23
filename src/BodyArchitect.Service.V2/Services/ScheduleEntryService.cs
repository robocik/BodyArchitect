using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using AutoMapper;
using AutoMapper.Mappers;
using BodyArchitect.Client.Common;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Transform;
using Profile = BodyArchitect.Model.Profile;
using ReminderType = BodyArchitect.Model.ReminderType;

namespace BodyArchitect.Service.V2.Services
{
    public class ScheduleEntryService : ServiceBase
    {
        public ScheduleEntryService(ISession session, SecurityInfo securityInfo, ServiceConfiguration configuration) : base(session, securityInfo, configuration)
        {
        }

        public ScheduleEntryDTO SaveScheduleEntry( ScheduleEntryDTO entry)
        {
            Log.WriteWarning("SaveScheduleEntry:Username={0},entryId={1}", SecurityInfo.SessionData.Profile.UserName, entry.GlobalId);
            ScheduleEntry db = null;
            if (!SecurityInfo.Licence.IsInstructor)
            {
                throw new LicenceException("This feature is allowed for Instructor account");
            }

            using (var trans = Session.BeginSaveTransaction())
            {
                var dbProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
                int count = Session.QueryOver<ScheduleEntry>().Where(
                        x => x.Profile == dbProfile && x.GlobalId != entry.GlobalId &&
                             (x.StartTime <= entry.StartTime && x.EndTime > entry.StartTime ||
                              x.StartTime < entry.EndTime && x.EndTime >= entry.EndTime)).RowCount();
                if (count > 0)
                {
                    throw new AlreadyOccupiedException("This user is busy at the selected time");
                }

                if (entry.GlobalId!=Constants.UnsavedGlobalId)
                {
                    db = Session.Get<ScheduleEntry>(entry.GlobalId);
                    Mapper.Map(entry, db);
                }
                else
                {
                    db = entry.Map<ScheduleEntry>();
                    db.Profile = dbProfile;
                }

                if(entry.MyPlaceId==null)
                {
                    db.MyPlace = Session.QueryOver<MyPlace>().Where(x=>x.Profile==dbProfile && x.IsDefault).SingleOrDefault();
                }
                if(db.MyPlace.Profile!=dbProfile)
                {
                    throw new CrossProfileOperationException("MyPlace not belong to this user");
                }

                db.Activity = Session.Get<Activity>(entry.ActivityId);
                if (entry.CustomerGroupId.HasValue)
                {
                    db.CustomerGroup = Session.Get<CustomerGroup>(entry.CustomerGroupId);
                    if (db.CustomerGroup.Profile != dbProfile)
                    {
                        throw new CrossProfileOperationException("Group not belong to this user");
                    }
                }
                if (db.Activity.Profile!=dbProfile)
                {
                    throw new CrossProfileOperationException("Activity not belong to this user");
                }
                dbProfile.DataInfo.ScheduleEntryHash = Guid.NewGuid();
                db = Session.Merge(db);
                trans.Commit();
            }

            return Mapper.Map<ScheduleEntry, ScheduleEntryDTO>(db);
        }

        public void DeleteScheduleEntry(ScheduleEntryDTO entry)
        {
            Log.WriteWarning("DeleteScheduleEntry:Username={0},entryId={1}", SecurityInfo.SessionData.Profile.UserName, entry.GlobalId);
            if (!SecurityInfo.Licence.IsInstructor)
            {
                throw new LicenceException("This feature is allowed for Instructor account");
            }

            using (var trans = Session.BeginSaveTransaction())
            {
                var dbProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
                var db = Session.Load<ScheduleEntry>(entry.GlobalId);
                if (db.Profile != dbProfile)
                {
                    throw new CrossProfileOperationException();
                }
                Session.Delete(db);
                dbProfile.DataInfo.ScheduleEntryHash = Guid.NewGuid();
                trans.Commit();
            }
        }

        public IList<ScheduleEntryBaseDTO> SaveScheduleEntryRange(SaveScheduleEntryRangeParam saveScheduleEntryRangeParam)
        {
            DateTime startDate = saveScheduleEntryRangeParam.StartDay;
            DateTime endDate = saveScheduleEntryRangeParam.EndDay.AddDays(1);

            Log.WriteWarning("SaveScheduleEntriesRange:Username={0},Start={1},End={2}", SecurityInfo.SessionData.Profile.UserName, saveScheduleEntryRangeParam.StartDay,saveScheduleEntryRangeParam.EndDay);

            if (!SecurityInfo.Licence.IsInstructor)
            {
                throw new LicenceException("This feature is allowed for Instructor account");
            }

            if(saveScheduleEntryRangeParam.Entries.Where(x=>x.StartTime<startDate).Count()>0 || saveScheduleEntryRangeParam.Entries.Where(x=>x.EndTime>endDate).Count()>0)
            {
                throw new ArgumentOutOfRangeException("Entries are out of range StartDay and EndDay");
            }

            

            //determine if we should copy instead of save
            bool copy = saveScheduleEntryRangeParam.CopyStart != null && saveScheduleEntryRangeParam.CopyEnd != null;

            var newDb = saveScheduleEntryRangeParam.Entries.Map<IList<ScheduleEntryBase>>();
            using (var trans = Session.BeginSaveTransaction())
            {

                var dbProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
                IList<ScheduleEntryBase> itemsToSave;
                if(copy)
                {
                    //if((saveScheduleEntryRangeParam.CopyStart.Value-saveScheduleEntryRangeParam.StartDay).TotalDays<7 ||
                    //    (saveScheduleEntryRangeParam.CopyEnd.Value - saveScheduleEntryRangeParam.EndDay).TotalDays < 7)
                    //{
                    //    throw new ArgumentException("Copy start and end range must be at least one week after source range");
                    //}
                    //var count =Session.QueryOver<ScheduleEntry>().Where(x =>x.StartTime >= saveScheduleEntryRangeParam.CopyStart.Value && x.EndTime <= saveScheduleEntryRangeParam.CopyEnd.Value && x.Profile == dbProfile).RowCount();
                    //if (count>0)
                    //{
                    //    throw new AlreadyOccupiedException("Destination days are occupied already");
                    //}
                    var count = Session.QueryOver<ScheduleEntry>().Where(x => x.StartTime >= saveScheduleEntryRangeParam.CopyStart.Value && x.EndTime <= saveScheduleEntryRangeParam.CopyEnd.Value && x.Profile == dbProfile).List();
                    if (count.Count>0)
                    {
                        throw new AlreadyOccupiedException("Destination days are occupied already");
                    }
                    itemsToSave = newDb;
                }
                else
                {
                    var db = Session.QueryOver<ScheduleEntryBase>().Where(x => x.StartTime >= startDate && x.EndTime < endDate && x.Profile == dbProfile).List();
                    var comparer = new CollectionComparer<ScheduleEntryBase>(newDb, db);
                    foreach (var entry in comparer.RemovedItems)
                    {
                        if (entry.IsLocked)
                        {
                            throw new DeleteConstraintException("Cannot delete schedule entry");
                        }
                        Session.Delete(entry);
                    }
                    itemsToSave = comparer.GetForSaveItems();
                }

                for (int index = 0; index < itemsToSave.Count; index++)
                {
                    var entry = itemsToSave[index];
                    ScheduleEntryBase dbOldEntry = null;
                    if (entry.GlobalId != Guid.Empty)
                    {
                        dbOldEntry = Session.QueryOver<ScheduleEntryBase>().Fetch(x => x.Reminder).Eager.Where(x => x.GlobalId == entry.GlobalId).SingleOrDefault();
                        entry.Reservations = dbOldEntry.Reservations;
                        entry.Reminder = dbOldEntry.Reminder;
                        if(!copy && entry.IsLocked)
                        {//if we are in saving mode and this entry is locked then skip to the next one
                            itemsToSave[index] = dbOldEntry;
                            continue;
                        }
                    }
                    if (dbOldEntry == null)
                    {
                        entry.Profile = dbProfile;
                        dbOldEntry = entry;
                    }
                    

                    var scheduleEntry = entry as ScheduleEntry;

                    if (dbOldEntry.Profile != dbProfile)
                    {
                        throw new CrossProfileOperationException();
                    }

                    if (scheduleEntry != null && scheduleEntry.CustomerGroup != null && scheduleEntry.CustomerGroup.Profile != dbProfile)
                    {
                        throw new CrossProfileOperationException("Group not belong to this user");
                    }
                    if (scheduleEntry != null && scheduleEntry.Activity.Profile != dbProfile)
                    {
                        throw new CrossProfileOperationException("Activity is not belong to this user");
                    }

                    if (itemsToSave.Where(x => x != entry && (
                                                                 x.StartTime <= entry.StartTime &&
                                                                 x.EndTime > entry.StartTime ||
                                                                 x.StartTime < entry.EndTime &&
                                                                 x.EndTime >= entry.EndTime))
                            .Count() > 0)
                    {
                        throw new AlreadyOccupiedException();
                    }

                    var dto = saveScheduleEntryRangeParam.Entries[newDb.IndexOf(entry)];
                    //if (dto.RemindBefore!=null)
                    //{
                    //    if(entry.Reminder==null)
                    //    {
                    //        entry.Reminder=new ReminderItem();
                    //    }
                    //    entry.Reminder.Profile = dbProfile;
                    //    entry.Reminder.DateTime = entry.StartTime;
                    //    entry.Reminder.RemindBefore = dto.RemindBefore==TimeSpan.Zero?(TimeSpan?) null:dto.RemindBefore.Value;
                    //    entry.Reminder.Name = string.Format("{0}: {1}", entry.Activity.Name,entry.StartTime);
                    //    entry.Reminder.Type = ReminderType.ScheduleEntry;
                    //}
                    if (dto.RemindBefore != null)
                    {
                        ReminderService.CreateReminder<ScheduleEntryDTO>(Session, entry, dto.RemindBefore, dbProfile,
                                                                                       entry.StartTime,ReminderType.ScheduleEntry);
                    }
                    entry.Profile = dbProfile;
                    if(dto.MyPlaceId==null)
                    {
                        entry.MyPlace = Session.QueryOver<MyPlace>().Where(x=>x.Profile==dbProfile && x.IsDefault).SingleOrDefault();
                    }
                    if (entry.MyPlace.Profile != dbProfile)
                    {
                        throw new CrossProfileOperationException("MyPlace not belong to this user");
                    }
                    if (!copy )
                    {
                        //save only when we are in saving mode. in copy mode this loop is only for checking and preparing
                        itemsToSave[index]=Session.Merge(entry);

                        if(dto.RemindBefore!=null)
                        {
                            itemsToSave[index].Reminder.ConnectedObject = string.Format("ScheduleEntryDTO:{0}", itemsToSave[index].GlobalId);
                            Session.Update(itemsToSave[index].Reminder);
                        }
                        else if (dbOldEntry.Reminder!=null)
                        {
                            Session.Delete(dbOldEntry.Reminder);
                            dbOldEntry.Reminder = null;
                            dbProfile.DataInfo.ReminderHash = Guid.NewGuid();
                        }
                    }
                }
                if(copy)
                {
                    itemsToSave=copyScheduleEntries(startDate, endDate, saveScheduleEntryRangeParam.CopyStart.Value, saveScheduleEntryRangeParam.CopyEnd.Value, itemsToSave,saveScheduleEntryRangeParam.Mode);
                }
                dbProfile.DataInfo.ScheduleEntryHash = Guid.NewGuid();
                trans.Commit();
                return itemsToSave.Map<IList<ScheduleEntryBaseDTO>>();
            }
        }

        private IList<ScheduleEntryBase> copyScheduleEntries(DateTime startDay, DateTime endDay, DateTime copyFrom, DateTime copyTo, IList<ScheduleEntryBase> daysToCopy,SaveScheduleEntryRangeCopyMode mode)
        {
            daysToCopy = daysToCopy.OrderBy(x => x.StartTime).ToList();
            var days = (copyTo - copyFrom).TotalDays;
            var sourceDays = (endDay - startDay).TotalDays;

            if (sourceDays < 6 && startDay.DayOfWeek - endDay.DayOfWeek != 1)
            {
                throw new ArgumentException("Source time must be at least one week");
            }

            if (days < 6 && copyFrom.DayOfWeek - copyTo.DayOfWeek != 1)
            {
                throw new ArgumentException("Destination time must be at least one week");
            }

            List<ScheduleEntryBase> entries = new List<ScheduleEntryBase>();
            int sourceIndex = 0;
            Dictionary<int, ScheduleDayInfo> daysInfo = new Dictionary<int, ScheduleDayInfo>();
            for (int i = 0; i < sourceDays; i++)
            {
                var res = daysToCopy.Where(x => x.StartTime.Date == startDay.AddDays(i).Date).ToList();
                if(mode==SaveScheduleEntryRangeCopyMode.OnlyScheduleEntries)
                {
                    res = res.OfType<ScheduleEntry>().ToList<ScheduleEntryBase>();
                }
                ScheduleDayInfo info = new ScheduleDayInfo(startDay.AddDays(i).Date);
                info.Entries.AddRange(res);
                daysInfo.Add(i, info);
            }
            for (int i = 0; i < days; i++)
            {
                if (sourceIndex >= daysInfo.Count)
                {
                    sourceIndex = 0;
                }
                var newDate = copyFrom.AddDays(i).Date;
                ScheduleDayInfo dayInfo;
                while ((dayInfo = daysInfo[sourceIndex]).Date.DayOfWeek != newDate.DayOfWeek)
                {
                    sourceIndex++;
                }

                foreach (var entry in dayInfo.Entries)
                {
                    var copy = entry.StandardClone();
                    
                    copy.Reservations = new List<ScheduleEntryReservation>();
                    var startTime = copy.StartTime.TimeOfDay;
                    var endTime = copy.EndTime.TimeOfDay;
                    copy.Version = 0;
                    copy.GlobalId = Guid.Empty;
                    copy.StartTime = newDate + startTime;
                    copy.EndTime = newDate + endTime;
                    Session.SaveOrUpdate(copy);
                    if(copy.Reminder!=null)
                    {
                        copy.Reminder.DateTime = copy.StartTime;
                        copy.Reminder.ConnectedObject = string.Format("ScheduleEntryDTO:{0}", copy.GlobalId);
                    }
                    Session.Flush();
                    entries.Add(copy);
                }

                sourceIndex++;

            }
            return entries;
        }

        public PagedResult<ScheduleEntryBaseDTO> GetScheduleEntries(GetScheduleEntriesParam getScheduleEntriesParam, PartialRetrievingInfo retrievingInfo)
        {
            Log.WriteWarning("GetScheduleEntries:Username={0}", SecurityInfo.SessionData.Profile.UserName);

            if (!SecurityInfo.Licence.IsInstructor)
            {
                throw new LicenceException("This feature is allowed for Instructor account");
            }

            using (var trans = Session.BeginGetTransaction())
            {
                Profile dbProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
                var query = Session.QueryOver<ScheduleEntryBase>()
                    .Fetch(x=>x.Reservations).Eager
                    .Fetch(x=>x.Reminder).Eager
                    .Fetch(x => ((Championship)x).Categories).Eager
                    .Where(x => x.Profile == dbProfile);


                if (getScheduleEntriesParam.EntryId.HasValue)
                {
                    query = query.Where(x => x.GlobalId == getScheduleEntriesParam.EntryId.Value);
                }
                if (getScheduleEntriesParam.ActivityId.HasValue)
                {
                    //query = query.JoinAlias(x =>((ScheduleEntry) x).Usluga, () => usluga);
                    query = query.Where(x => ((ScheduleEntry)x).Activity.GlobalId == getScheduleEntriesParam.ActivityId.Value);
                }
                if (getScheduleEntriesParam.StartTime.HasValue)
                {
                    query = query.Where(x => x.StartTime >= getScheduleEntriesParam.StartTime.Value);
                }
                if (getScheduleEntriesParam.EndTime.HasValue)
                {
                    query = query.Where(x => x.EndTime <= getScheduleEntriesParam.EndTime.Value);
                }
                query.TransformUsing(new DistinctRootEntityResultTransformer());
                trans.Commit();


                var res= query.ToPagedResults<ScheduleEntryBaseDTO, ScheduleEntryBase>(retrievingInfo);
                return res;
            }
        }

    }

    class ScheduleDayInfo
    {
        public DateTime Date { get; private set; }

        public ScheduleDayInfo(DateTime date)
        {
            Date = date;
            Entries = new List<ScheduleEntryBase>();
        }
        public List<ScheduleEntryBase> Entries { get; private set; }
    }

}
