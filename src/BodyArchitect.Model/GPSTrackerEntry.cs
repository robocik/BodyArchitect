using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Shared;

namespace BodyArchitect.Model
{
    public enum Mood
    {
        Normal,
        Bad,
        Good
    }

    [Serializable]
    [EntryObjectOperationsAttribute(EntryObjectOperation.Copy | EntryObjectOperation.Move)]
    public class GPSTrackerEntry : EntryObject
    {
        public GPSTrackerEntry()
        {
            Weather = new Weather();
        }

        public static readonly Guid EntryTypeId = new Guid("87DD05C5-2C29-47E3-9E65-3CB8831ADF23");

        public virtual DateTime? StartDateTime { get; set; }

        public virtual DateTime? EndDateTime { get; set; }

        public virtual decimal? Duration { get; set; }

        public virtual Exercise Exercise { get; set; }

        public virtual decimal? AvgSpeed { get; set; }

        public virtual decimal? MaxSpeed { get; set; }

        public virtual short? AvgHeartRate { get; set; }

        public virtual short? MaxHeartRate { get; set; }

        public virtual decimal? Distance { get; set; }

        public virtual decimal? MinAltitude { get; set; }

        public virtual decimal? MaxAltitude { get; set; }

        public virtual decimal? TotalAscent { get; set; }

        public virtual decimal? TotalDescent { get; set; }

        public virtual decimal? Calories { get; set; }

        public virtual Mood Mood { get; set; }

        //public virtual byte[] GPSCoordinates { get; set; }

        public virtual GPSCoordinates Coordinates { get; set; }

        public virtual Weather Weather { get; set; }

        public override Guid TypeId
        {
            get { return EntryTypeId; }
        }

        public override bool IsEmpty
        {
            get { return false; }
        }
    }
}
