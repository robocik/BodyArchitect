using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model.Old;
using BodyArchitect.Shared;
using FluentNHibernate.Mapping;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings.Old
{
    public class TrainingPlanMapping: ClassMap<TrainingPlan>
    {
        public TrainingPlanMapping()
        {
            this.LazyLoad();
            Id(x => x.GlobalId).GeneratedBy.Assigned();
            Map(x => x.Name).Length(Constants.NameColumnLength).Not.Nullable();
            Map(x => x.Author).Length(Constants.NameColumnLength).Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            References(x => x.Profile).LazyLoad().Not.Nullable().Cascade.None();
            Map(x => x.DaysCount).Not.Nullable();
            Map(x => x.Rating).Formula("(select avg(RatingUserValue.Rating) from RatingUserValue where RatingUserValue.RatedObjectId=GlobalId)");
            Map(x => x.Language).Not.Nullable();
            Map(x => x.PublishDate).Nullable();
            Map(x => x.TrainingType).CustomType<TrainingType>().Not.Nullable();
            Map(x => x.Purpose).CustomType<WorkoutPlanPurpose>().Not.Nullable();
            Map(x => x.Status).CustomType<PublishStatus>().Not.Nullable();
            Map(x => x.Difficult).CustomType<TrainingPlanDifficult>().Not.Nullable();
            Map(x => x.PlanContent).Not.Nullable().CustomType("StringClob");
            Version(x => x.Version);
        }
    }
}
