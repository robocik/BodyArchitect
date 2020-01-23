using BodyArchitect.Model;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings
{
    public class DataInfoMapping: ClassMapping<DataInfo>
    {
        public DataInfoMapping()
        {
            Id(x => x.GlobalId, map => map.Generator(Generators.GuidComb));
            Property(x => x.ExerciseHash, map =>
            {
                map.NotNullable(true);
            });
            Property(x => x.WorkoutPlanHash, map =>
            {
                map.NotNullable(true);
            });
            Property(x => x.TrainingDayHash, map =>
            {
                map.NotNullable(true);
            });
            Property(x => x.ActivityHash, map =>
            {
                map.NotNullable(true);
            });
            Property(x => x.CustomerHash, map =>
            {
                map.NotNullable(true);
            });
            Property(x => x.ReminderHash, map =>
            {
                map.NotNullable(true);
            });
            Property(x => x.CustomerGroupHash, map =>
            {
                map.NotNullable(true);
            });
            Property(x => x.MyPlaceHash, map =>
            {
                map.NotNullable(true);
            });
            Property(x => x.ScheduleEntryHash, map =>
            {
                map.NotNullable(true);
            });
            Property(x => x.SupplementsCycleDefinitionHash, map =>
            {
                map.NotNullable(true);
            });
            Property(x => x.MessageHash, map =>
            {
                map.NotNullable(true);
            });
            //ManyToOne(x => x.Profile, map =>
            //{
            //    map.Column("Profile_id");
            //    map.NotNullable(true);
            //    map.Cascade(Cascade.None);
            //    map.Lazy(LazyRelation.Proxy);
            //});
        }
    }
}
