using System;
using System.Collections.Generic;
using FM.Model;
using FM.StrengthTraining.Model;
using FM.StrengthTraining.Model.TrainingPlans;
using NHibernate.Linq;

namespace FM.StrengthTraining.Factory
{
    class TrainingPlanFactory
    {
        public IList<TrainingPlanDTO> GetTrainingPlans(SessionData sessionData)
        {
            //var criteria = sessionData.Session.CreateCriteria(typeof (TrainingPlanDTO));
            return TrainingPlanDTO.FindAll();
        }

        public void DeleteTrainingPlan(SessionData sessionData, TrainingPlanDTO trainingPlan)
        {
            trainingPlan.Delete();

        }
        public void SaveTrainingPlan(SessionData sessionData, TrainingPlanDTO trainingPlan)
        {
            if (string.IsNullOrEmpty(trainingPlan.Name))
            {
                throw new ArgumentNullException("Name cannot be null");
            }

            //var query1 = from m in sessionData.Session.Linq<TrainingPlanDTO>()
            //             where m.GlobalId == trainingPlan.GlobalId 
            //             select m;
            //int count = query1.Count();
            //if (count > 0)
            //{
            //    throw new UniqueException("Training plan is already imported");
            //}
            trainingPlan.Save();
        }
    }
}
