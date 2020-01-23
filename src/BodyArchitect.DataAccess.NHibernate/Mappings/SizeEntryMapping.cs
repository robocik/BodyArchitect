using BodyArchitect.Model;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.NHibernate.Mappings
{
    public class SizeEntryMapping : JoinedSubclassMapping<SizeEntry>
    {
        public SizeEntryMapping()
        {
            //References(x => x.Wymiary).Not.LazyLoad().Cascade.All().Fetch.Join();
            Key(x => x.Column("EntryObject_id"));

            ManyToOne(x => x.Wymiary, g =>
            {
                g.NotNullable(false);
                g.Lazy(LazyRelation.NoLazy);
                g.Column("Wymiary_id");
                g.Cascade(Cascade.All);
                g.Fetch(FetchKind.Join);
            });
        }
    }
}
