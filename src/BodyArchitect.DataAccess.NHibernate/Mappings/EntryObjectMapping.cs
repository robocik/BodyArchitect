using BodyArchitect.Model;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.NHibernate.Mappings
{
    public class EntryObjectMapping : ClassMapping<EntryObject>
    {
        public EntryObjectMapping()
        {
            Id(x => x.GlobalId, map =>
            {
                map.Generator(Generators.GuidComb);
            });
            Property(b => b.Comment, y =>
            {
                y.Type(NHibernateUtil.StringClob);
                y.NotNullable(false);
            });
            Property(b => b.Status, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.Name, y =>
            {
                y.Length(Constants.NameColumnLength);
                y.NotNullable(false);
            });
            Property(b => b.ReportStatus, y =>
            {
                y.NotNullable(true);
            });

            ManyToOne(x => x.Reminder, g =>
            {
                g.NotNullable(false);
                g.Column("Reminder_id");
                g.Cascade(Cascade.All | Cascade.DeleteOrphans);
            });

            ManyToOne(x => x.LoginData, g =>
            {
                g.NotNullable(false);
                g.Column("LoginData_id");
                g.Cascade(Cascade.None);
            });
            ManyToOne(x => x.Reservation, g =>
            {
                g.NotNullable(false);
                g.Column("Reservation_id");
                g.Cascade(Cascade.None);
            });

            ManyToOne(x => x.LoginData, g =>
            {
                g.NotNullable(false);
                g.Column("LoginData_id");
                g.Cascade(Cascade.None);
            });

            ManyToOne(x => x.TrainingDay, g =>
            {
                g.NotNullable(true);
                g.Column("TrainingDay_id");
                
            });
            ManyToOne(x => x.MyTraining, g =>
            {
                g.NotNullable(false);
                g.Column("MyTraining_id");
                g.Cascade(Cascade.Merge | Cascade.Persist);

            });

            Version(x => x.Version, map => map.Generated(VersionGeneration.Never));
            //References(x => x.MyTraining).LazyLoad().Cascade.SaveUpdate();
        }
    }
}
