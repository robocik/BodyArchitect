using System;
using System.Collections.Generic;
using System.Linq;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.Client.WP7.ModelExtensions;

namespace BodyArchitect.Service.V2.Model
{

    public partial class TrainingDayDTO
    {
        public TrainingDayDTO()
        {
            Objects=new List<EntryObjectDTO>();
            AllowComments = true;
            if (ApplicationState.Current != null && ApplicationState.Current.ProfileInfo!=null)
            {
                AllowComments = ApplicationState.Current.ProfileInfo.Settings.AllowTrainingDayComments;
            }
        }

        
        public ICollection<EntryObjectDTO> GetEntries(Type entryObjectType)
        {
            return Objects.Where(x => x.GetType() == entryObjectType).ToList();
        }

        public ICollection<T> GetEntries<T>() where T : EntryObjectDTO
        {
            return Objects.OfType<T>().ToList();
        }

        public T GetEntry<T>(LocalObjectKey key) where T:EntryObjectDTO
        {
            return (T) GetEntry(key);
        }
        
        public EntryObjectDTO GetEntry(LocalObjectKey key)
        {
            if (key.KeyType == KeyType.InstanceId)
            {
                return Objects.Where(x => x.InstanceId == key.Id).SingleOrDefault();
            }
            else
            {
                return Objects.Where(x => x.GlobalId == key.Id).SingleOrDefault();
            }
        }
        //public EntryObjectDTO GetEntry(Type type)
        //{
        //    return (EntryObjectDTO)Objects.Where(x => x.GetType() == type).FirstOrDefault();
        //}

        //public BlogEntryDTO Blog
        //{
        //    get { return GetEntry<BlogEntryDTO>(); }
        //}

        //public StrengthTrainingEntryDTO StrengthWorkout
        //{
        //    get { return GetEntry<StrengthTrainingEntryDTO>(); }
        //}

        //public GPSTrackerEntryDTO GPS
        //{
        //    get { return GetEntry<GPSTrackerEntryDTO>(); }
        //}

        //public SizeEntryDTO Size
        //{
        //    get { return GetEntry<SizeEntryDTO>(); }
        //}

        //public SuplementsEntryDTO Supplements
        //{
        //    get { return GetEntry<SuplementsEntryDTO>(); }
        //}

        //public T GetEntry<T>() where T : EntryObjectDTO
        //{
        //    return (T)Objects.Where(x => x is T).FirstOrDefault();
        //}
    }
}
