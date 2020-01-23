using System;
using System.Linq;
using BodyArchitect.Service.Client.WP7;

namespace BodyArchitect.Service.V2.Model
{
    public partial class TrainingPlan:IRatingable
    {
        public TrainingPlanDay GetDay(Guid id)
        {
            return (from i in Days where i.GlobalId == id select i).SingleOrDefault();
        }

        public void AddDay(TrainingPlanDay entry)
        {
            entry.TrainingPlan = this;
            Days.Add(entry);

        }

        public void RemoveDay(TrainingPlanDay entry)
        {
            Days.Remove(entry);
            entry.TrainingPlan = null;
        }

        public TrainingPlanEntry GetEntry(Guid globalId)
        {
            return Days.SelectMany(itemDto => itemDto.Entries).FirstOrDefault(serieDto => serieDto.GlobalId == globalId);
        }

        public virtual void RepositionEntry(int index1, int index2)
        {
            var item = Days[index1];
            Days.Remove(item);
            Days.Insert(index2, item);
        }

        public bool IsMine
        {
            get
            {
                if (Profile == null || ApplicationState.Current.SessionData.Profile.GlobalId != Profile.GlobalId)
                {
                    return false;
                }
                return true;
            }

        }

        public bool IsFavorite
        {
            get
            {
                bool contains = ApplicationState.Current.Cache.TrainingPlans.Items.ContainsKey(GlobalId);
                if (!contains)
                {
                    return false;
                }
                return Profile != null && Profile.GlobalId != ApplicationState.Current.SessionData.Profile.GlobalId;
            }
        }
    }

}
