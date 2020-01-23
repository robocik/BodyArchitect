using System;
using BodyArchitect.Service.Model;
using BodyArchitect.Service.Model.TrainingPlans;

namespace BodyArchitect.Service.Model.TrainingPlans
{
    public class TrainingBuilder
    {
        public bool FillRepetitionNumber { get; set; }

        public void FillTrainingFromPlan(TrainingPlanDay planDay, StrengthTrainingEntryDTO strengthEntry)
        {
            strengthEntry.Entries.Clear();
            foreach (TrainingPlanEntry trainingPlanEntry in planDay.Entries)
            {
                var strengthItem = new StrengthTrainingItemDTO();
                strengthItem.ExerciseId = trainingPlanEntry.ExerciseId;
                strengthItem.TrainingPlanItemId = trainingPlanEntry.GlobalId;
                strengthEntry.AddEntry(strengthItem);
                foreach (var set in trainingPlanEntry.Sets)
                {
                    var serie = new SerieDTO();
                    if (FillRepetitionNumber && set.RepetitionNumberMax.HasValue && set.RepetitionNumberMin.HasValue && set.RepetitionNumberMax == set.RepetitionNumberMin)
                    {
                        serie.RepetitionNumber = set.RepetitionNumberMax.Value;
                    }
                    serie.DropSet = set.DropSet;
                    serie.SetType = (SetType) set.RepetitionsType;
                    serie.TrainingPlanItemId = set.GlobalId;
                    strengthItem.AddSerie(serie);
                }
            }
            strengthEntry.TrainingPlanItemId = planDay.GlobalId;
            strengthEntry.TrainingPlanId = planDay.TrainingPlan.GlobalId;

            fillSuperSets(planDay, strengthEntry);
        }

        void fillSuperSets(TrainingPlanDay planDay, StrengthTrainingEntryDTO strengthEntry)
        {
            foreach (SuperSet superSet in planDay.SuperSets)
            {
                foreach (var trainingPlanEntry in superSet.SuperSets)
                {
                    var item1 = findStrengthTrainingItemByTrainingPlanItemId(trainingPlanEntry.GlobalId, strengthEntry);
                    if(item1!=null)
                    {
                        item1.SuperSetGroup = superSet.SuperSetId.ToString();
                    }
                }
            }
        }

        StrengthTrainingItemDTO findStrengthTrainingItemByTrainingPlanItemId(Guid trainingPlanItemId, StrengthTrainingEntryDTO strengthEntry)
        {
            foreach (var strengthTrainingItem in strengthEntry.Entries)
            {
                if(strengthTrainingItem.TrainingPlanItemId==trainingPlanItemId)
                {
                    return strengthTrainingItem;
                }
            }
            return null;
        }

        //public void ImportFromFile(SessionData sessionData, string filename, MethodParameters progressParams)
        //{
        //    progressParams.SetMessage(StrengthTrainingEntryStrings.ImportTrainingPlan_OpeningFile);
        //    WorkoutPlanDTO dto = new WorkoutPlanDTO();
        //    var stream1 = File.OpenRead(filename);
        //    var content = new byte[stream1.Length];
        //    stream1.Read(content, 0, (int)stream1.Length);
        //    stream1.Close();

        //    XmlSerializationTrainingPlanFormatter planFormatter = new XmlSerializationTrainingPlanFormatter();
        //    MemoryStream stream = new MemoryStream(content);
        //    //
        //    TrainingPlanPack newPlanPack = planFormatter.Read(stream);

        //    if (progressParams.Cancel)
        //    {
        //        return;
        //    }
        //    progressParams.SetMessage(StrengthTrainingEntryStrings.ImportTrainingPlan_Verifying);

        //    var plan = newPlanPack.TrainingPlan;

        //    //dto.SetTrainingPlan(plan);
        //    //

        //    if (progressParams.Cancel)
        //    {
        //        return;
        //    }
        //    progressParams.SetMessage(StrengthTrainingEntryStrings.ImportTrainingPlan_Saving);

        //    MessageBox.Show("Must be implemented");
        //    //using (TransactionManager manager = new TransactionManager(sessionData, true))
        //    //{
        //    //    //using (var scope = new TransactionScope())
        //    //    //{
        //    //    dto.Save();
        //    //    //scope.VoteCommit();
        //    //    //}

        //    //    List<ExerciseDTO> exercisesForValidation = ObjectsReposidory.Exercises.ToList();
        //    //    var exercises = ObjectsReposidory.Exercises.ToDictionary(v => v.GlobalId);

        //    //    if (progressParams.Cancel)
        //    //    {
        //    //        return;
        //    //    }

        //    //    foreach (var ex in newPlanPack.Exercises.Values)
        //    //    {
        //    //        if (!exercises.ContainsKey(ex.GlobalId))
        //    //        {
        //    //            LocalizedExerciseType exerciseType = new LocalizedExerciseType(ex.Muscle);
        //    //            progressParams.SetMessage(string.Format(ApplicationStrings.InfoImportExercisesProgress,
        //    //                                                    exerciseType.LocalizedName));
        //    //            var existingExercise = ObjectsReposidory.GetExercise(ex.Shortcut);
        //    //            if (existingExercise == null)
        //    //            {
        //    //                //ServicesManager.GetService<IExerciseFactory>().Save(sessionData, ex);
        //    //                ex.CreateAndFlush();
        //    //                exercisesForValidation.Add(ex);
        //    //            }
        //    //            else
        //    //            {
        //    //                ChangeExerciseShortcut dlg = new ChangeExerciseShortcut();
        //    //                dlg.Fill(ex, exercisesForValidation);
        //    //                var res = dlg.ShowDialog();
        //    //                if (res == DialogResult.OK)
        //    //                {
        //    //                    exercisesForValidation.Add(dlg.NewExercise);
        //    //                }
        //    //            }
        //    //            ObjectsReposidory.ClearExerciseCache();
        //    //            if (progressParams.Cancel)
        //    //            {
        //    //                return;
        //    //            }
        //    //        }
        //    //    }
        //    //    manager.CommitTransaction();
        //    //}
        //}
    }
}
