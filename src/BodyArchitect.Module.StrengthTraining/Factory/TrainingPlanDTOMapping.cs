using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using FM.Model;
using FM.StrengthTraining.Model.TrainingPlans;

namespace FM.StrengthTraining.Factory
{
    public class TrainingPlanDTOMapping : ClassMap<TrainingPlanDTO>
    {
        public TrainingPlanDTOMapping()
        {
            this.Not.LazyLoad();
            Id(x => x.GlobalId).GeneratedBy.Assigned();
            Map(x => x.PlanContent).CustomSqlType("image").Not.Nullable();
            Map(x => x.Name).Length(Constants.NameColumnLength).Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.Version).Not.Nullable();
            Map(x => x.Author).Length(Constants.NameColumnLength).Nullable();
            Map(x => x.TrainingType).CustomType<TrainingType>().Not.Nullable();
            Map(x => x.Difficult).CustomType<TrainingPlanDifficult>().Not.Nullable();
        }
    }
}
