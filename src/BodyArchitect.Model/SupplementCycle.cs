using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model
{
    [Serializable]
    public class SupplementCycleDefinition : FMGlobalObject, ISortable
    {
        public SupplementCycleDefinition()
        {
            Weeks = new HashSet<SupplementCycleWeek>();
        }

        public virtual DateTime? PublishDate
        {
            get;
            set;
        }

        public virtual PublishStatus Status { get; set; }

        public virtual string Name { get; set; }

        public virtual string Url { get; set; }

        public virtual string Author { get; set; }

        public virtual bool CanBeIllegal { get; set; }

        public virtual Profile Profile { get; set; }

        public virtual int Version { get; set; }

        public virtual TrainingPlanDifficult Difficult { get; set; }

        public virtual WorkoutPlanPurpose Purpose { get; set; }

        public virtual string Language { get; set; }

        public virtual DateTime CreationDate { get; set; }

        public virtual float Rating
        {
            get;
            set;
        }

        public virtual ICollection<SupplementCycleWeek> Weeks { get; protected set; }

        public virtual SupplementCycleDefinition BasedOn { get; set; }

        public virtual string Comment
        {
            get; set;
        }

        public virtual int GetTotalWeeks()
        {
            int maxWeek = 0;
            foreach (var day in Weeks)
            {
                maxWeek = Math.Max(maxWeek, day.CycleWeekEnd);
            }
            return maxWeek;
        }

        public virtual int GetTotalDays(int totalWeeks)
        {
            var result = GetTotalWeeks();
            if (totalWeeks>0)
            {
                result = totalWeeks;
            }
            return result * 7;
        }
    }

    public class SupplementCycle : MyTraining
    {
        public virtual string TrainingDays { get; set; }

        public virtual SupplementCycleDefinition SupplementsCycleDefinition { get; set; }

        public virtual decimal Weight { get; set; }

        public virtual int TotalWeeks { get; set; }
    }
}
