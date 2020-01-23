using BodyArchitect.Model.Old;
using FluentNHibernate.Mapping;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings.Old
{
    public class A6WEntryMapping:SubclassMap<A6WEntry>
    {
        public A6WEntryMapping()
        {
            this.Not.LazyLoad();
            Map(x => x.Completed).Not.Nullable();
            Map(x => x.DayNumber).Not.Nullable();
            Map(x => x.Set1);
            Map(x => x.Set2);
            Map(x => x.Set3);
        }
    }
}
