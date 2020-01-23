using BodyArchitect.Model;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings
{
    public class A6WEntryMapping : JoinedSubclassMapping<A6WEntry>
    {
        public A6WEntryMapping()
        {
            Key(x => x.Column("EntryObject_id"));
            Property(b => b.Completed, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.DayNumber, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.Set1, y =>
            {
                y.NotNullable(false);
            });
            Property(b => b.Set2, y =>
            {
                y.NotNullable(false);
            });
            Property(b => b.Set3, y =>
            {
                y.NotNullable(false);
            });
            //Map(x => x.Completed).Not.Nullable();
            //Map(x => x.DayNumber).Not.Nullable();
            //Map(x => x.Set1);
            //Map(x => x.Set2);
            //Map(x => x.Set3);
        }
    }
}
