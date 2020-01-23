using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model.Old;
using BodyArchitect.Shared;
using FluentNHibernate.Mapping;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings.Old
{
    public class MyTrainingMapping : ClassMap<MyTraining>
    {
        public MyTrainingMapping()
        {
            this.LazyLoad();
            Id(x => x.Id);
            Map(x => x.StartDate).Not.Nullable();
            Map(x => x.EndDate).Nullable();
            Map(x => x.TypeId).Not.Nullable();
            References(x => x.Profile).Not.Nullable();
            Map(x => x.TrainingEnd).CustomType<TrainingEnd>().Nullable();
            Map(x => x.PercentageCompleted).Nullable();
            Map(x => x.Name).Length(Constants.NameColumnLength).Not.Nullable();
            HasMany(x => x.EntryObjects).LazyLoad().Inverse().Fetch.Subselect();
        }
    }
}
