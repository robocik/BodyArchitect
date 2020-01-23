using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model.Old;
using FluentNHibernate.Mapping;


namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings.Old
{
    public class SerieMapping : ClassMap<Serie>
    {
        public SerieMapping()
        {
            this.Not.LazyLoad();
            Id(x => x.Id);
            Map(x => x.IsCiezarBezSztangi).Not.Nullable();
            Map(x => x.SetType).CustomType<SetType>().Not.Nullable();
            Map(x => x.RepetitionNumber).Nullable();
            Map(x => x.Weight).Nullable();
            Map(x => x.DropSet).CustomType<DropSetType>().Not.Nullable();
            Map(x => x.TrainingPlanItemId).Nullable();
            Map(x => x.StartTime).Nullable();
            Map(x => x.EndTime).Nullable();
            Map(x => x.Comment).CustomType("StringClob").Nullable();
            References(x => x.StrengthTrainingItem);

        }

    }
}
