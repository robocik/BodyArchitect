using FluentNHibernate.Mapping;
using FM.StrengthTraining.Model;

namespace FM.StrengthTraining
{
    public class StrengthTrainingItemMapping : ClassMap<StrengthTrainingItem>
    {
        public StrengthTrainingItemMapping()
        {
            this.Not.LazyLoad();
            Id(x => x.Id, "ID");
            Map(x => x.ExerciseId,"ExerciseID").Not.Nullable();
            Map(x => x.StrengthEntryId, "StrengthTrainingEntryID").Not.Nullable();
            Map(x => x.Comment).Nullable();
        }
    }
}
