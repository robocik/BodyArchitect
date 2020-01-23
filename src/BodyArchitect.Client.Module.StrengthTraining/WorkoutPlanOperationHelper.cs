using System;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.StrengthTraining.Controls.TrainingPlans;
using BodyArchitect.Client.Module.StrengthTraining.Model;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Views;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.WCF;
using BodyArchitect.Client.Module.StrengthTraining.Localization;
using BodyArchitect.Client.Module.StrengthTraining.Model.TrainingPlans;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using BodyArchitect.Settings.Model;
using BodyArchitect.Shared;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls
{
    public static class WorkoutPlanOperationHelper
    {
        public static string TranslateStrength(this string key)
        {
            return EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.StrengthTraining:StrengthTrainingEntryStrings:" + key);
        }

        //public static void LoadContent(this TrainingPlanInfo plan, bool force = false)
        //{
        //    if(plan.Tag==null || force)
        //    {
        //        var newPlan = ServiceManager.GetTrainingPlan(plan.GlobalId);
        //        plan.Author = newPlan.Author;
        //        //plan.DaysCount = newPlan.DaysCount;
        //        plan.Difficult = newPlan.Difficult;
        //        plan.Language = newPlan.Language;
        //        plan.PublishDate = newPlan.PublishDate;
        //        plan.Status = newPlan.Status;
        //        plan.Rating = newPlan.Rating;
        //        plan.UserRating = newPlan.UserRating;
        //        plan.UserShortComment = newPlan.UserShortComment;
        //        plan.Purpose = newPlan.Purpose;
        //        plan.Tag = newPlan;
        //        plan.TrainingType = newPlan.TrainingType;
        //        plan.Version = newPlan.Version;
        //    }
        //}

        public static string GetDisplayText(this TrainingPlanSerie serie,bool isCardio)
        {
            string dropSetString = "";
            string repType = "";
            string superSlow = "";
            string restPause = "";

            if (isCardio)
            {
                decimal seconds = serie.RepetitionNumberMin.HasValue ? serie.RepetitionNumberMin.Value : 0;
                var time = TimeSpan.FromSeconds((double)seconds);
                return time.ToString();
            }
            if (serie.DropSet != DropSetType.None)
            {
                dropSetString = "^" + (int)serie.DropSet;
            }
            if (serie.RepetitionsType != SetType.Normalna)
            {
                var localier = new EnumLocalizer(StrengthTrainingEntryStrings.ResourceManager);
                repType = localier.Translate(serie.RepetitionsType);
            }
            if (serie.IsSuperSlow)
            {
                superSlow = " SS";
            }
            if (serie.IsRestPause)
            {
                restPause = " RP";
            }
            string format = serie.ToStringRepetitionsRange();

            return string.Format("{0} {1}{2}{3}{4}", format, repType, dropSetString,superSlow,restPause).Trim();
        }

        public static void SetFromString(this SerieDTO set,string serie)
        {
            if (string.IsNullOrWhiteSpace(serie))
            {
                set.RepetitionNumber = null;
                set.Weight = null;
                return;
            }
            TimeSpan span;

            if (set.StrengthTrainingItem.Exercise.ExerciseType==ExerciseType.Cardio )
            {//for cardio
                if (TimeSpan.TryParse(serie, out span))
                {
                    set.Weight = (int) span.TotalSeconds;
                }
                return;
            }
            int index = serie.IndexOf(SerieDTO.SerieSeparator);
            int tempRepetition;
            if (index>0 && int.TryParse(serie.Substring(0, index), out tempRepetition))
            {
                set.RepetitionNumber = tempRepetition;
            }
            else
            {
                set.RepetitionNumber = null;
            }
            string weightString = serie.Substring(index + 1, serie.Length - index - 1);
            decimal tempWeight;
            if (decimal.TryParse(weightString, out tempWeight))
            {
                if (UserContext.Current.ProfileInformation.Settings.WeightType == WeightType.Pounds)
                {
                    tempWeight = tempWeight * 0.454M;
                }
                set.Weight = tempWeight;
            }
            else
            {
                set.Weight = null;
            }
        }

        public enum SetDisplayMode
        {
            Short,
            Medium,
            Extended
        }

        public static string GetDisplayText(this SerieDTO serie, SetDisplayMode displayMode)
        {
            string dropSetString = "";
            string repType = "";
            string kgText = string.Empty;
            string superSlow = "";

            if (serie.IsCardio())
            {
                decimal seconds = serie.Weight.HasValue ? serie.Weight.Value : 0;
                var time = TimeSpan.FromSeconds((double) Math.Round(seconds,0));
                return time.ToString();
            }
            if (serie.Weight.HasValue && (displayMode == SetDisplayMode.Medium || displayMode == SetDisplayMode.Extended))
            {
                kgText = UserContext.Current.ProfileInformation.Settings.WeightType == WeightType.Kg
                                 ? Strings.WeightType_Kg
                                 : Strings.WeightType_Pound;
            }
            
            if (displayMode == SetDisplayMode.Extended)
            {
                if (serie.IsSuperSlow)
                {
                    superSlow = " SS";
                }
                if (serie.DropSet != DropSetType.None)
                {
                    dropSetString = "^" + (int) serie.DropSet;
                }
                if (serie.SetType != SetType.Normalna)
                {
                    repType = EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.StrengthTraining:StrengthTrainingEntryStrings:SetType_" + serie.SetType.ToString());
                }

            }
            decimal? weightDisp = null;
            if(serie.Weight.HasValue)
            {
                weightDisp = serie.Weight.Value.ToDisplayWeight();
            }
            string format = string.Format("{0:D}x{1:#.##}", (int?)serie.RepetitionNumber, weightDisp);

            string text = string.Format("{0}{3} {1}{2}{4}", format, repType, dropSetString, kgText, superSlow);
            return text.Trim();
        }

        //public static TrainingPlan ToTrainingPlan(this TrainingPlanInfo plan)
        //{
        //    plan.LoadContent();
        //    return (TrainingPlan) plan.Tag;
        //}

        public static void Open(this TrainingPlan plan)
        {
            MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.UI;component/Views/HtmlPreviewView.xaml"), () =>
            {
                return new HtmlPreviewPageContext(new TrainingPlanHtmlExporter(plan));
            },true);
        }

        public static bool RemoveFromFavorites(this ExerciseDTO exercise)
        {
            if (!CanRemoveFromFavorites(exercise))
            {
                return false;
            }
            ServiceCommand command = new ServiceCommand(()=>
                                                            {
                                                                try
                                                                {
                                                                    ExerciseOperationParam param = new ExerciseOperationParam();
                                                                    param.ExerciseId = exercise.GlobalId;
                                                                    param.Operation = FavoriteOperation.Remove;
                                                                    ServiceManager.ExerciseOperation(param);
                                                                }
                                                                catch (ObjectIsNotFavoriteException)
                                                                {
                                                                    
                                                                }
                                                                catch
                                                                {
                                                                    ExercisesReposidory.Instance.Add(exercise);
                                                                    throw;
                                                                }
                                                                
                                                            });
            ServicePool.Add(command);
            ExercisesReposidory.Instance.Remove(exercise.GlobalId);
            return true;
        }

        public static bool CanAddToFavorites(this ExerciseDTO exercise)
        {
            return !(exercise == null || exercise.Profile == null || exercise.IsFavorite() || exercise.IsMine());
        }

        public static bool CanRemoveFromFavorites(this ExerciseDTO exercise)
        {
            return !(exercise == null || exercise.Profile == null || !exercise.IsFavorite() || exercise.IsMine());
        }
        public static bool AddToFavorites(this ExerciseDTO exercise)
        {
            if (!CanAddToFavorites(exercise))
            {
                return false;
            }
            ServiceCommand command = new ServiceCommand(() =>
            {
                try
                {
                    ExerciseOperationParam param = new ExerciseOperationParam();
                    param.ExerciseId = exercise.GlobalId;
                    param.Operation = FavoriteOperation.Add;
                    ServiceManager.ExerciseOperation(param);
                }
                catch (ObjectIsFavoriteException)
                {
                }
                catch
                {
                    ExercisesReposidory.Instance.Remove(exercise.GlobalId);
                    throw;
                }
            });
            ServicePool.Add(command);
            ExercisesReposidory.Instance.Add(exercise);
            return true;
        }

        public static bool RemoveFromFavorites(this TrainingPlan plan)
        {
            if (plan == null || !plan.IsFavorite() || plan.IsMine())
            {
                return false;
            }
            var param = new WorkoutPlanOperationParam();
            param.WorkoutPlanId = plan.GlobalId;
            param.Operation = SupplementsCycleDefinitionOperation.RemoveFromFavorites;
            ServiceManager.WorkoutPlanOperation(param);
            
            
            WorkoutPlansReposidory.Instance.Remove(plan.GlobalId);
            return true;
        }

        public static bool AddToFavorites(this TrainingPlan plan)
        {
            if (plan == null || plan.IsFavorite() || plan.IsMine())
            {
                return false;
            }
            PublishWorkoutPlanWindow dlg = new PublishWorkoutPlanWindow();
            dlg.Fill(plan, false);
            if (dlg.ShowDialog() ==false)
            {
                return false;
            }
            WorkoutPlansReposidory.Instance.Add(plan);
            //WorkoutPlansReposidory.Instance.ClearCache();
            return true;
        }

        public static void SetRestTime(this StrengthTrainingItemDTO item,TimeSpan restTime)
        {
            DateTime previousEnd = DateTime.Today;
            for (int i = 0; i < item.Series.Count; i++)
            {
                TimeSpan setTime = TimeSpan.Zero;
                var set = item.Series[i];
                if(set.StartTime.HasValue && set.EndTime.HasValue)
                {
                    setTime = set.EndTime.Value - set.StartTime.Value;
                }
                set.StartTime = previousEnd + restTime;
                set.EndTime = set.StartTime + setTime;
                previousEnd = set.EndTime.Value;
            }
        }
    }
}
