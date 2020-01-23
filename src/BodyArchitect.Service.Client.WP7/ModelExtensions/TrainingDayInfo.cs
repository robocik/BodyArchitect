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
using BodyArchitect.Client.Common;
using BodyArchitect.Portable;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Service.Client.WP7.ModelExtensions
{
    public class TrainingDayInfo
    {
        private Dictionary<LocalObjectKey, GPSPointsBag> _gpsCoordinates;

        public TrainingDayInfo()
        {
            //GPSCoordinates = new Dictionary<Guid, GPSPointsBag>();
            GPSCoordinates = new Dictionary<LocalObjectKey, GPSPointsBag>();
        }
        public TrainingDayInfo(TrainingDayDTO day):this()
        {
            DateTime = System.DateTime.UtcNow;
            TrainingDay = day;
            //OriginalVersion = day.Version;
        }
        public TrainingDayDTO TrainingDay { get; set; }

        public DateTime DateTime { get; set; }

        public bool IsModified { get; set; }


        public bool IsConflict { get; set; }

        //key is GPSTrackerEntryId
        public Dictionary<LocalObjectKey, GPSPointsBag> GPSCoordinates
        {
            get { return _gpsCoordinates; }
            set
            {
                _gpsCoordinates = value;
            }
        }

        public void CleanUpGpsCoordinates()
        {
            var gpsEntries=TrainingDay.Objects.OfType<GPSTrackerEntryDTO>();
            //find all gps coordintaes without GPSTrackerEntry and delete them
            List<LocalObjectKey> keysToRemove = new List<LocalObjectKey>();
            foreach (var gpsCoordinate in GPSCoordinates.Keys)
            {
                var existingEntry=gpsEntries.Where(x => x.GlobalId == gpsCoordinate.Id || x.InstanceId==gpsCoordinate.Id).SingleOrDefault();
                if (existingEntry == null)
                {
                    keysToRemove.Add(gpsCoordinate);
                }
            }
            foreach (var localObjectKey in keysToRemove)
            {
                GPSCoordinates.Remove(localObjectKey);
            }

            //now check if we can use GlobalId instead of InstanceId
            foreach (var key in GPSCoordinates.Keys)
            {
                if (key.KeyType == KeyType.InstanceId)
                {
                    var entry=gpsEntries.Where(x => x.InstanceId == key.Id).SingleOrDefault();
                    if (entry!=null && !entry.IsNew)
                    {
                        key.Id = entry.GlobalId;
                        key.KeyType = KeyType.GlobalId;
                    }
                }
            }
        }

        public void SetGpsCoordinates(GPSTrackerEntryDTO gpsEntry, IEnumerable<GPSPoint> points, bool isSaved = true)
        {
            if (gpsEntry.IsNew)
            {
                GPSCoordinates[new LocalObjectKey(gpsEntry.InstanceId, KeyType.InstanceId)] = new GPSPointsBag(points, isSaved);
            }
            else
            {
                //first check if there is old element which using InstanceId for this entry. if yes then remove it
                if (GPSCoordinates.ContainsKey(new LocalObjectKey(gpsEntry.InstanceId, KeyType.InstanceId)))
                {
                    GPSCoordinates.Remove(new LocalObjectKey(gpsEntry.InstanceId, KeyType.InstanceId));
                }
                GPSCoordinates[new LocalObjectKey(gpsEntry.GlobalId, KeyType.GlobalId)] = new GPSPointsBag(points, isSaved);
            }
        }

        public GPSPointsBag GetGpsCoordinates(GPSTrackerEntryDTO gpsEntry)
        {
            return GPSCoordinates.Where(x => x.Key.Id == gpsEntry.GlobalId || x.Key.Id == gpsEntry.InstanceId).Select(x => x.Value).SingleOrDefault();
        }
        //key is GPSTrackerEntryId
        //public Dictionary<Guid, GPSPointsBag> GPSCoordinates { get; set; }

        public void Update(EntryObjectDTO entryObject)
        {
            var entry = this.TrainingDay.Objects.Where(x => x.GlobalId == entryObject.GlobalId).SingleOrDefault();
            if (entry != null)
            {
                TrainingDay.Objects.Remove(entry);
                TrainingDay.Objects.Add(entryObject);
                entryObject.TrainingDay = TrainingDay;
            }

        }
    }
}
