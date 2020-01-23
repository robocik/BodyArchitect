using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Shared;

namespace BodyArchitect.Service.V2.Model
{
    public enum Mood
    {
        Normal,
        Bad,
        Good
    }

    [Serializable]
    [EntryObjectOperationsAttribute(EntryObjectOperation.Copy | EntryObjectOperation.Move)]
    [DataContract(Namespace = Const.ServiceNamespace)]
    [EntryObjectInstance(EntryObjectInstance.Multiple)]
    public class GPSTrackerEntryDTO : SpecificEntryObjectDTO, ICloneable
    {
        public GPSTrackerEntryDTO()
        {
            Weather = new WeatherDTO();
        }

        [DataMember]
        public short? AvgHeartRate { get; set; }

        [DataMember]
        public short? MaxHeartRate { get; set; }

        [DataMember]
        public bool HasCoordinates { get; set; }

        [DataMember]
        public decimal? Duration { get; set; }

        [DataMember]
        public DateTime? StartDateTime { get; set; }

        [DataMember]
        public DateTime? EndDateTime { get; set; }

        [DataMember]
        [Required]
        public ExerciseLightDTO Exercise { get; set; }

        [DataMember]
        public decimal? AvgSpeed { get; set; }

        [DataMember]
        public decimal? Calories { get; set; }

        [DataMember]
        public decimal? MaxSpeed { get; set; }

        [DataMember]
        public decimal? Distance { get; set; }

        [DataMember]
        public decimal? MinAltitude { get; set; }

        [DataMember]
        public decimal? MaxAltitude { get; set; }

        [DataMember]
        public decimal? TotalAscent { get; set; }

        [DataMember]
        public decimal? TotalDescent { get; set; }

        [DataMember]
        public Mood Mood { get; set; }

        [DataMember]
        [Required]
        public WeatherDTO Weather { get; set; }

        public object Clone()
        {
            var entry = new GPSTrackerEntryDTO();
            entry.Mood = Mood;
            entry.Comment = Comment;
            entry.Name = Name;
            entry.Exercise = Exercise;
            entry.StartDateTime = StartDateTime;
            entry.EndDateTime = EndDateTime;
            entry.Duration = Duration;
            entry.Distance = Distance;
            entry.AvgHeartRate = AvgHeartRate;
            entry.MaxSpeed = MaxSpeed;
            entry.AvgSpeed = AvgSpeed;
            entry.MaxAltitude = MaxAltitude;
            entry.MinAltitude = MinAltitude;
            entry.TotalAscent = TotalAscent;
            entry.TotalDescent = TotalDescent;
            entry.Calories = Calories;
            entry.ReportStatus = ReportStatus;
            entry.RemindBefore = RemindBefore;
            return entry;
        }
    }
}
