using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Controls;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Logger;
using BodyArchitect.Module.StrengthTraining.Localization;
using BodyArchitect.Module.StrengthTraining.Model.TrainingPlans;
using BodyArchitect.Service.Model;
using BodyArchitect.Service.Model.TrainingPlans;
using BodyArchitect.Settings.Model;
using BodyArchitect.Shared;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    public static class WorkoutPlanOperationHelper
    {
        public static void LoadContent(this WorkoutPlanDTO plan,bool force=false)
        {
            if(plan.PlanContent==null || force)
            {
                var newPlan = ServiceManager.GetTrainingPlan(plan.GlobalId);
                plan.Author = newPlan.Author;
                plan.DaysCount = newPlan.DaysCount;
                plan.Difficult = newPlan.Difficult;
                plan.Language = newPlan.Language;
                plan.PublishDate = newPlan.PublishDate;
                plan.Status = newPlan.Status;
                plan.Rating = newPlan.Rating;
                plan.UserRating = newPlan.UserRating;
                plan.UserShortComment = newPlan.UserShortComment;
                plan.Purpose = newPlan.Purpose;
                plan.PlanContent = newPlan.PlanContent;
                plan.TrainingType = newPlan.TrainingType;
                plan.Version = newPlan.Version;
            }
        }

        public static string GetDisplayText(this TrainingPlanSerie serie)
        {
            string dropSetString = "";
            string repType = "";
            if (serie.DropSet != DropSetType.None)
            {
                dropSetString = "^" + (int)serie.DropSet;
            }
            if (serie.RepetitionsType != TrainingPlanSerieRepetitions.Normalna)
            {
                var localier = new EnumLocalizer(LocalizedPropertyGridStrings.ResourceManager);
                repType = localier.Translate(serie.RepetitionsType);
            }
            string format = serie.ToStringRepetitionsRange();

            return string.Format("{0} {1}{2}", format, repType, dropSetString);
        }

        public static void SetFromString(this SerieDTO set,string serie)
        {
            TimeSpan span;

            if (TimeSpan.TryParse(serie, out span))
            {//for cardio
                set.RepetitionNumber = (int)span.TotalSeconds;
                return;
            }
            int index = serie.IndexOf(SerieDTO.SerieSeparator);
            int tempRepetition;
            if (int.TryParse(serie.Substring(0, index), out tempRepetition))
            {
                set.RepetitionNumber = tempRepetition;
            }
            else
            {
                set.RepetitionNumber = null;
            }
            string weightString = serie.Substring(index + 1, serie.Length - index - 1);
            float tempWeight;
            if (float.TryParse(weightString, out tempWeight))
            {
                if (Settings.Settings1.Default.WeightType == (int)WeightType.Pounds)
                {
                    tempWeight = tempWeight * 0.454f;
                }
                set.Weight = tempWeight;
            }
            else
            {
                set.Weight = null;
            }
        }

        public static string GetDisplayText(this SerieDTO serie,bool extendedVersion)
        {
            string dropSetString = "";
            string repType = "";
            string kgText = string.Empty;
            if (extendedVersion)
            {
                if (serie.Weight.HasValue)
                {
                    kgText = Settings.Settings1.Default.WeightType == (int) WeightType.Kg
                                 ? ApplicationStrings.WeightType_Kg
                                 : ApplicationStrings.WeightType_Pound;
                }
                if (serie.DropSet != DropSetType.None)
                {
                    dropSetString = "^" + (int) serie.DropSet;
                }
                if (serie.SetType != SetType.Normalna)
                {
                    var localier = new EnumLocalizer(LocalizedPropertyGridStrings.ResourceManager);
                    repType = localier.Translate((TrainingPlanSerieRepetitions) serie.SetType);
                }

            }
            double? weightDisp = null;
            if(serie.Weight.HasValue)
            {
                weightDisp = serie.Weight.Value.ToDisplayWeight();
            }
            string format = string.Format("{0}x{1:#.##}", serie.RepetitionNumber, weightDisp);

            return string.Format("{0}{3} {1}{2}", format, repType, dropSetString, kgText);
        }

        public static TrainingPlan ToTrainingPlan(this WorkoutPlanDTO plan)
        {
            plan.LoadContent();
            XmlSerializationTrainingPlanFormatter formatter = new XmlSerializationTrainingPlanFormatter();
            var newPlan = formatter.FromXml(plan.PlanContent);
            return newPlan;
        }

        public static void Open(this WorkoutPlanDTO plan)
        {
            if (plan == null)
            {
                return;
            }
            HtmlPreviewWindow wnd = new HtmlPreviewWindow();
            using (TrainingPlanHtmlExporter htmlExporter = new TrainingPlanHtmlExporter(plan.ToTrainingPlan()))
            {
                wnd.Fill(htmlExporter);
                wnd.ShowDialog();
            }
            //var plan = ServiceManager.GetTrainingPlan(TrainingPlan);
            
        }

        public static bool RemoveFromFavorites(this WorkoutPlanDTO plan)
        {
            if (plan == null || !plan.IsFavorite() || plan.IsMine())
            {
                return false;
            }
            ServiceManager.WorkoutPlanFavoritesOperation(plan, FavoriteOperation.Remove);
            
            
            ObjectsReposidory.ClearWorkoutPlansCache();
            return true;
        }

        public static bool AddToFavorites(this WorkoutPlanDTO plan)
        {
            if (plan == null || plan.IsFavorite() || plan.IsMine())
            {
                return false;
            }
            PublishWorkoutPlanWindow dlg = new PublishWorkoutPlanWindow();
            dlg.Fill(plan, false);
            if (dlg.ShowDialog() == DialogResult.Cancel)
            {
                return false;
            }
            ObjectsReposidory.ClearWorkoutPlansCache();
            return true;
        }
    }
}
