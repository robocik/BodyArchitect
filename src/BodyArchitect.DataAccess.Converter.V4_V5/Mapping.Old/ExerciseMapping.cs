using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model.Old;
using BodyArchitect.Shared;
using FluentNHibernate.Mapping;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings.Old
{
    public class ExerciseMapping : ClassMap<Exercise>
    {
        public ExerciseMapping()
        {
            this.Not.LazyLoad();
            Id(x => x.GlobalId).GeneratedBy.Assigned();
            Version(x => x.Version);
            Map(x => x.Name).Length(Constants.NameColumnLength).Not.Nullable();
            Map(x => x.Description).CustomType("StringClob").Nullable();
            Map(x => x.Url).Length(Constants.UrlLength).Nullable();
            Map(x => x.Shortcut).Length(20).Not.Nullable();
            Map(x => x.Rating).Formula("(select avg(RatingUserValue.Rating) from RatingUserValue where RatingUserValue.RatedObjectId=GlobalId)");
            Map(x => x.PublishDate).Nullable();
            Map(x => x.Status).CustomType<PublishStatus>().Not.Nullable();
            References(x => x.Profile).Nullable().LazyLoad().Cascade.None();;
            Map(x => x.MechanicsType).CustomType<MechanicsType>().Not.Nullable();
            Map(x => x.ExerciseForceType).CustomType<ExerciseForceType>().Not.Nullable();
            Map(x => x.ExerciseType).CustomType<ExerciseType>().Not.Nullable();
            Map(x => x.Difficult).CustomType<ExerciseDifficult>().Not.Nullable();

        }
    }
}
