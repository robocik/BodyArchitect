using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using BodyArchitect.Service.Client.WP7.ModelExtensions;


namespace BodyArchitect.Service.V2.Model.TrainingPlans
{

    public class TrainingPlan : TrainingPlanBase
    {
        private TrainingType trainingType;
        
        public TrainingPlan()
        {
            Days=new List<TrainingPlanDay>();
            CreationDate = DateTime.UtcNow;
        }

        public string Language { get; set; }

        public List<TrainingPlanDay> Days { get; set; }

        public WorkoutPlanPurpose Purpose { get; set; }

        public object Tag { get; set; }

        public Guid? BasedOnId { get; set; }


        public string Name { get; set; }

        
        public string Author { get; set; }

        public string Url { get; set; }

        public DateTime CreationDate { get; set; }

        public TrainingPlanDifficult Difficult { get; set; }

        public string Comment
        {
            get;
            set;
        }


        public int RestSeconds { get; set; }

        
        public TrainingType TrainingType
        {
            get { return trainingType; }
            set { trainingType = value; }
        }

        public TrainingPlanDay GetDay(Guid id)
        {
            return (from i in Days where i.GlobalId == id select i).SingleOrDefault();
        }

        public void AddDay(TrainingPlanDay entry)
        {
            entry.TrainingPlan = this;
            Days.Add(entry);

        }

        public void RemoveDay(TrainingPlanDay entry)
        {
            Days.Remove(entry);
            entry.TrainingPlan = null;
        }

        public TrainingPlanEntry GetEntry(Guid globalId)
        {
            return Days.SelectMany(itemDto => itemDto.Entries).FirstOrDefault(serieDto => serieDto.GlobalId == globalId);
        }

        public virtual void RepositionEntry(int index1, int index2)
        {
            var item = Days[index1];
            Days.Remove(item);
            Days.Insert(index2, item);
        }


    }

}
