using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.NHibernate.Mappings
{
    public class GPSTrackerEntryMapping : JoinedSubclassMapping<GPSTrackerEntry>
    {
        public GPSTrackerEntryMapping()
        {
            //References(x => x.Wymiary).Not.LazyLoad().Cascade.All().Fetch.Join();
            Key(x => x.Column("EntryObject_id"));

            Property(x => x.Duration);
            Property(x=> x.StartDateTime);
            Property(x => x.EndDateTime);
            Property(x => x.AvgSpeed);
            Property(x => x.MaxSpeed);
            Property(x => x.TotalAscent);
            Property(x => x.TotalDescent);
            Property(x => x.Calories);
            Property(x => x.MaxAltitude);
            Property(x => x.AvgHeartRate);
            Property(x => x.MaxHeartRate);
            Property(x => x.MinAltitude);
            Property(x => x.Mood);
            Property(x => x.Distance);
            ManyToOne(b => b.Exercise, y =>
            {
                y.Lazy(LazyRelation.NoLazy);
                y.NotNullable(true);
                y.Cascade(Cascade.None);
                y.Column("ExerciseId");
            });
            ManyToOne(b => b.Coordinates, y =>
            {
                y.Lazy(LazyRelation.Proxy);
                y.NotNullable(false);
                y.Cascade(Cascade.All | Cascade.DeleteOrphans);
            });

            Component(x => x.Weather);
            //Property(x => x.GPSCoordinates, v =>
            //                                   {
            //                                       v.Lazy(true);
            //                                       v.NotNullable(false);
            //                                       v.Type(NHibernateUtil.BinaryBlob);
            //                                   });
        }
    }

    public class WeatherMapping : ComponentMapping<Weather>
    {
        public WeatherMapping()
        {
            Property(b => b.Condition, y =>
            {
                y.NotNullable(true);
                y.Column("WeatherCondition");

            });
            Property(b => b.Temperature, y =>
                                             {
                                                 y.Column("WeatherTemperature");
                                                 y.NotNullable(false);
                                             });
        }
    }
}
