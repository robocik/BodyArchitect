using BodyArchitect.Model;
using BodyArchitect.Shared;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings
{
    public class SuplementEntryMapping : JoinedSubclassMapping<SuplementsEntry>
    {
        public SuplementEntryMapping()
        {
            Key(x => x.Column("EntryObject_id"));

            Set(x => x.Items, v =>
            {
                v.Cascade(Cascade.All | Cascade.DeleteOrphans);
                v.Inverse(true);
                v.Key(c => c.Column("SuplementsEntry_id"));
                v.Fetch(CollectionFetchMode.Subselect);
                v.Lazy(CollectionLazy.NoLazy);
                v.OrderBy(x => x.Position);
            }, h => h.OneToMany());

            //HasMany(x => x.Items).Cascade.AllDeleteOrphan().Inverse().Not.LazyLoad();
        }
    }
}
