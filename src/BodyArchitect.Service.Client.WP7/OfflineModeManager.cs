using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7.Cache;
using BodyArchitect.Service.Client.WP7.ModelExtensions;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Service.Client.WP7
{
    public enum ModificationType
    {
        EntryModified,
        EntryOnServerButNotOnClient
    }

    public class OfflineModeManager
    {
        private Dictionary<CacheKey, TrainingDaysHolder> state;
        private Guid myProfileId;

        public OfflineModeManager(Dictionary<CacheKey, TrainingDaysHolder> state,Guid myProfileId)
        {
            this.state = state;
            this.myProfileId = myProfileId;
        }

        public bool RetrievedDays(DateTime startMonth, int months, IEnumerable<TrainingDayDTO> days,TrainingDaysHolder current)
        {
            bool withoutProblems = true;
            foreach (TrainingDayDTO dayDto in days)
            {
                if (current.TrainingDays.ContainsKey(dayDto.TrainingDate))
                {
                    var dayInfo = current.TrainingDays[dayDto.TrainingDate];
                    if (!dayInfo.IsModified)
                    {
                        current.TrainingDays[dayDto.TrainingDate] = new TrainingDayInfo(dayDto);
                    }
                    //else if (dayInfo.OriginalVersion != dayDto.Version)
                    //{
                    //    int version = dayDto.Version;
                    //    dayDto.Version = dayInfo.OriginalVersion;
                    //    dayInfo.TrainingDay.GlobalId = dayDto.GlobalId;
                    //    //version is changed but we must check if the content is different. If not we can merge
                    //    if (dayDto.IsModified(dayInfo.TrainingDay))
                    //    {//still problem with InstanceId in child objects
                    //        dayInfo.IsConflict = true;
                    //        withoutProblems = false;
                    //    }
                    //    else
                    //    {
                    //        state.TrainingDays[dayDto.TrainingDate].TrainingDay.Version = version;
                    //        state.TrainingDays[dayDto.TrainingDate].TrainingDay.GlobalId = dayDto.GlobalId;
                    //    }
                        
                    //}
                    else 
                    {
                        //int version = dayDto.Version;
                        //dayDto.Version = dayInfo.OriginalVersion;
                        dayInfo.TrainingDay.GlobalId = dayDto.GlobalId;
                        //version is changed but we must check if the content is different. If not we can merge
                        if (dayDto.IsModified(dayInfo.TrainingDay))
                        {//still problem with InstanceId in child objects
                            dayInfo.IsConflict = true;
                            withoutProblems = false;
                        }
                        else
                        {
                            setVersion(current.TrainingDays[dayDto.TrainingDate].TrainingDay, dayDto);

                            current.TrainingDays[dayDto.TrainingDate].TrainingDay.GlobalId = dayDto.GlobalId;
                        }

                    }
                }
                else
                {
                    TrainingDayInfo info = new TrainingDayInfo(dayDto);
                    current.TrainingDays.Add(dayDto.TrainingDate, info);
                }

            }
            for (int i = 0; i < months; i++)
            {
                DateTime tempDate = startMonth.AddMonths(i);
                if (!current.RetrievedMonths.Contains(tempDate))
                {
                    current.RetrievedMonths.Add(tempDate);
                }
            }
            return withoutProblems;
        }

        public void ClearTrainingDays()
        {
            foreach (var daysHolder in state.Values)
            {
                daysHolder.RetrievedMonths.Clear();
                List<DateTime> datesToRemove = new List<DateTime>();
                foreach (var dayInfo in daysHolder.TrainingDays)
                {
                    if (!dayInfo.Value.IsModified)
                    {
                        datesToRemove.Add(dayInfo.Key);
                    }
                }

                foreach (var dateTime in datesToRemove)
                {
                    daysHolder.TrainingDays.Remove(dateTime);
                }
            }
            //state.RetrievedMonths.Clear();
            //List<DateTime> datesToRemove = new List<DateTime>();
            //foreach (var dayInfo in state.TrainingDays)
            //{
            //    if(!dayInfo.Value.IsModified)
            //    {
            //        datesToRemove.Add(dayInfo.Key);
            //    }
            //}

            //foreach (var dateTime in datesToRemove)
            //{
            //    state.TrainingDays.Remove(dateTime);
            //}
        }


        //void intelligentCopy(TrainingDayDTO higherPriority, TrainingDayDTO lowerPriority)
        //{
        //    foreach (var objectDto in lowerPriority.Objects)
        //    {
        //        if (higherPriority.GetEntry(objectDto.GetType()) == null)
        //        {
        //            higherPriority.Objects.Add(objectDto);
        //        }
        //    }
        //}

        void setVersion(TrainingDayDTO target,TrainingDayDTO source)
        {
            if (source==null)
            {
                foreach (var dto in target.Objects)
                {
                    dto.Version = 0;
                    dto.GlobalId = Guid.Empty;
                }
                return;
            }
            for (int i = 0; i < source.Objects.Count; i++)
            {
                var entry = target.Objects.Where(x => x.GlobalId == source.Objects[i].GlobalId).SingleOrDefault();
                if (entry != null)
                {
                    entry.Version = source.Objects[i].Version;
                }
            }
        }
        public void MergeNew(TrainingDayDTO fromServer, ApplicationState appState, bool updateLocalCache, Func<ModificationType,bool> useServerQuestion)
        {
            Guid? customerId = appState.TrainingDay.TrainingDay.CustomerId;
            var holder = state[new CacheKey() { CustomerId = customerId, ProfileId = myProfileId }];

            if (fromServer != null)
            {
                if (fromServer.IsModified(appState.TrainingDay.TrainingDay))
                {
                    foreach (var serverEntry in fromServer.Objects)
                    {
                        var localEntry = appState.TrainingDay.TrainingDay.Objects.Where(x => x.GlobalId == serverEntry.GlobalId).SingleOrDefault();
                        if(localEntry==null)
                        {
                            //if (useServerQuestion(ModificationType.EntryOnServerButNotOnClient))
                            //{
                            //    appState.TrainingDay.TrainingDay.Objects.Add(serverEntry.Copy());
                            //}
                            appState.TrainingDay.TrainingDay.Objects.Add(serverEntry.Copy());
                            appState.TrainingDay.TrainingDay.GlobalId = fromServer.GlobalId;
                            //todo:here we "create" two entries so mabye we should inform the user about this
                        }
                        else if(serverEntry.IsModified(localEntry))
                        {
                            //the same entry has been modified
                            if (useServerQuestion(ModificationType.EntryModified))
                            {
                                //user wants to server version so replace it
                                appState.TrainingDay.TrainingDay.Objects.Remove(localEntry);
                                appState.TrainingDay.TrainingDay.Objects.Add(serverEntry);
                            }
                            else
                            {
                                localEntry.Version = serverEntry.Version;
                            }
                        }
                        else
                        {
                            localEntry.Version = serverEntry.Version;
                        }
                    }

                    //now we check what entries we remove on the client
                    for (int index = appState.TrainingDay.TrainingDay.Objects.Count - 1; index >= 0; index--)
                    {
                        var localEntry = appState.TrainingDay.TrainingDay.Objects[index];
                        if (localEntry.GlobalId != Guid.Empty)
                        {
//this is saved entry (not newly added) so we must check if it exists still on the server
                            var serverEntry =
                                fromServer.Objects.Where(x => x.GlobalId == localEntry.GlobalId).SingleOrDefault();
                            if (serverEntry == null)
                            {
//this entry doesn't exist on the server. we must remove it from the client
                                appState.TrainingDay.TrainingDay.Objects.Remove(localEntry);
                            }
                        }
                    }
                    //if (useServerQuestion())
                    //{
                    //    var localTemp = appState.TrainingDay;
                    //    appState.TrainingDay = fromServer;
                    //    if (updateLocalCache)
                    //    {
                    //        state.TrainingDays[fromServer.TrainingDate].IsModified = true;
                    //    }
                    //    intelligentCopy(fromServer, localTemp);

                    //    if (updateLocalCache)
                    //    {
                    //        state.TrainingDays[fromServer.TrainingDate].TrainingDay = appState.TrainingDay.Copy();
                    //    }
                    //}
                    //else
                    //{

                    //    setVersion(appState.TrainingDay, fromServer);
                    //    intelligentCopy(appState.TrainingDay, fromServer);

                    //    if (updateLocalCache)
                    //    {
                    //        state.TrainingDays[fromServer.TrainingDate].IsModified = true;
                    //    }
                    //}
                }
                else
                {
                    //appState.TrainingDay.Version = fromServer.Version;
                    setVersion(appState.TrainingDay.TrainingDay, fromServer);
                    if (updateLocalCache)
                    {
                        holder.TrainingDays[fromServer.TrainingDate].IsModified = false;
                        //state.TrainingDays[fromServer.TrainingDate].TrainingDay.Version = fromServer.Version;
                        setVersion(holder.TrainingDays[fromServer.TrainingDate].TrainingDay, fromServer);
                    }
                }
                if (updateLocalCache)
                {
                    holder.TrainingDays[fromServer.TrainingDate].IsConflict = false;
                }
            }
            else
            {
                //if on the server entry is deleted then we must set Id to 0 (newly created object)
                //TODO: teraz gdy na serwerze wpis został usunięty po prostu robimy tak ze dalej user go ma. mozna też wyswietlic
                //pytanie że wpis został usunięty i niech user zadecyduje co z tym fantem zrobić
                appState.TrainingDay.TrainingDay.GlobalId = Guid.Empty;
                if (updateLocalCache)
                {
                    holder.TrainingDays.Remove(appState.TrainingDay.TrainingDay.TrainingDate);
                }
                setVersion(appState.TrainingDay.TrainingDay, null);
                //appState.TrainingDay.Version = 0;
            }
        }

        //public void Merge(TrainingDayDTO fromServer,ApplicationState appState,bool updateLocalCache,Func<bool> useServerQuestion )
        //{
        //    if(fromServer!=null)
        //    {
        //        if (fromServer.IsModified(appState.TrainingDay))
        //        {
        //            if (useServerQuestion())
        //            {
        //                var localTemp = appState.TrainingDay;
        //                appState.TrainingDay = fromServer;
        //                if (updateLocalCache)
        //                {
        //                    state.TrainingDays[fromServer.TrainingDate].IsModified = true;
        //                }
        //                intelligentCopy(fromServer, localTemp);

        //                if (updateLocalCache)
        //                {
        //                    state.TrainingDays[fromServer.TrainingDate].TrainingDay = appState.TrainingDay.Copy();
        //                }
        //            }
        //            else
        //            {

        //                setVersion(appState.TrainingDay, fromServer);
        //                //appState.TrainingDay.Version = fromServer.Version;
        //                intelligentCopy(appState.TrainingDay, fromServer);

        //                if (updateLocalCache)
        //                {
        //                    state.TrainingDays[fromServer.TrainingDate].IsModified = true;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            //appState.TrainingDay.Version = fromServer.Version;
        //            setVersion(appState.TrainingDay, fromServer);
        //            if (updateLocalCache)
        //            {
        //                state.TrainingDays[fromServer.TrainingDate].IsModified = false;
        //                //state.TrainingDays[fromServer.TrainingDate].TrainingDay.Version = fromServer.Version;
        //                setVersion(state.TrainingDays[fromServer.TrainingDate].TrainingDay, fromServer);
        //            }
        //        }
        //        if (updateLocalCache)
        //        {
        //            state.TrainingDays[fromServer.TrainingDate].IsConflict = false;
        //        }
        //    }
        //    else
        //    {//if on the server entry is deleted then we must set Id to 0 (newly created object)
        //        appState.TrainingDay.GlobalId = Guid.Empty;
        //        if (updateLocalCache)
        //        {
        //            state.TrainingDays.Remove(appState.TrainingDay.TrainingDate);
        //        }
        //        setVersion(appState.TrainingDay, null);
        //        //appState.TrainingDay.Version = 0;
        //    }
        //}
    }
}
