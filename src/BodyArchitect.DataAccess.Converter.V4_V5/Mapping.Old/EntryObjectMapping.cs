using BodyArchitect.Model.Old;
using BodyArchitect.Shared;
using FluentNHibernate.Mapping;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings.Old
{
    public class EntryObjectMapping : ClassMap<EntryObject>
    {
        public EntryObjectMapping()
        {
            this.Not.LazyLoad();
            Id(x => x.Id);
            Map(x => x.Comment).CustomType("StringClob").Nullable();
            Map(x => x.Name).Length(Constants.NameColumnLength).Nullable();
            Map(x => x.ReportStatus).CustomType<ReportStatus>().Not.Nullable();
            References(x => x.TrainingDay).Not.LazyLoad();
            References(x => x.MyTraining).LazyLoad().Cascade.SaveUpdate();
        }
    }
}
