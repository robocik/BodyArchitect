using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.PreviewGenerator;
using BodyArchitect.Logger;
using BodyArchitect.Client.Module.StrengthTraining.Controls;
using BodyArchitect.Client.Module.StrengthTraining.Localization;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using BodyArchitect.Shared;
using Microsoft.Win32;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Primitives;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace BodyArchitect.Client.Module.StrengthTraining.Model.TrainingPlans
{
    public class MyIntegerUpDownEditor : TypeEditor<IntegerUpDown>
    {
        protected override void SetValueDependencyProperty()
        {
            ValueProperty = IntegerUpDown.ValueProperty;
        }

        protected override void SetControlProperties()
        {
            base.SetControlProperties();
            Editor.BorderThickness = new Thickness(0);
        }
    }

    [DefaultPropertyAttribute("Title")]
    [Serializable]
    public class TrainingPlanHtmlExporter : HtmlCycleExporterBase
    {
        private bool printRequiredSetsOnly = true;
        private int setsNumberToPrint = 2;
        private TrainingPlan trainingPlan;
        private bool printSetsPlaceholders = true;
        
        private bool printExerciseType = true;
        private bool printExerciseShortcut = false;
        private bool printSetsComment = false;
        private bool printLegend = true;
        

        public const char DaysSeparator = ';';

        public TrainingPlanHtmlExporter(TrainingPlan trainingPlan):base(trainingPlan.Name)
        {
            this.trainingPlan = trainingPlan;
            prepareTrainingPlan();
            

            //setAllTrainingDaysToPrint(trainingPlan);
        }

        //private void setAllTrainingDaysToPrint(TrainingPlan trainingPlan)
        //{
        //    List<Guid> allDays = new List<Guid>();
        //    foreach (var day in trainingPlan.Weeks)
        //    {
        //        allDays.Add(day.GlobalId);
        //    }
        //    var days = allDays.ToArray();
        //    StringBuilder builder = new StringBuilder();
        //    foreach (var dayId in days)
        //    {
        //        builder.AppendFormat("{0}{1}", dayId, TrainingPlanHtmlExporter.DaysSeparator);
        //    }
        //    PrintDays= builder.ToString().TrimEnd(TrainingPlanHtmlExporter.DaysSeparator);
        //}

        #region Properties

        [SRDescription("TrainingPlanHtmlExporter.PrintSetsComment.Description")]
        [SRDisplayName("TrainingPlanHtmlExporter.PrintSetsComment.DisplayName")]
        [SRCategory("PrintOptions")]
        [DefaultValue(false)]
        public bool PrintSetsComment
        {
            get { return printSetsComment; }
            set { printSetsComment = value; }
        }


        

        [SRDescription("TrainingPlanHtmlExporter.PrintExerciseType.Description")]
        [SRDisplayName("TrainingPlanHtmlExporter.PrintExerciseType.DisplayName")]
        [SRCategory("PrintOptions")]
        [DefaultValue(true)]
        public bool PrintExerciseType
        {
            get { return printExerciseType; }
            set { printExerciseType = value; }
        }

        [SRDescription("TrainingPlanHtmlExporter.PrintExerciseShortcut.Description")]
        [SRDisplayName("TrainingPlanHtmlExporter.PrintExerciseShortcut.DisplayName")]
        [SRCategory("PrintOptions")]
        [DefaultValue(false)]
        public bool PrintExerciseShortcut
        {
            get { return printExerciseShortcut; }
            set { printExerciseShortcut = value; }
        }

        

        

        

        [SRDescription("TrainingPlanHtmlExporter.PrintRequiredSetsOnly.Description")]
        [SRDisplayName("TrainingPlanHtmlExporter.PrintRequiredSetsOnly.DisplayName")]
        [SRCategory("PrintOptions")]
        [DefaultValue(true)]
        public bool PrintRequiredSetsOnly
        {
            get { return printRequiredSetsOnly; }
            set { printRequiredSetsOnly = value; }
        }

        [SRDescription("TrainingPlanHtmlExporter.SetsNumberToPrint.Description")]
        [SRDisplayName("TrainingPlanHtmlExporter.SetsNumberToPrint.DisplayName")]
        [SRCategory("PrintOptions")]
        [Editor(typeof(MyIntegerUpDownEditor), typeof(MyIntegerUpDownEditor))]
        public int SetsNumberToPrint
        {
            get
            {
                int max = 0;
                if (TrainingPlan != null)
                {
                    max=TrainingPlan.GetMaximumSeriesCount();
                   
                }
                return Math.Max(setsNumberToPrint,max);
            }
            set
            {
                if (TrainingPlan != null && TrainingPlan.GetMaximumSeriesCount()>value)
                {
                    throw new ArgumentException("Set number must be at least the sets count from training plan");
                }
                setsNumberToPrint = value;
            }
        }

        //TODO:Finish
        //[EditorAttribute(typeof(TrainingPlanDaysPropertyEditor), typeof(System.Drawing.Design.UITypeEditor))]
        //[TypeConverter(typeof(TrainingPlanDayIdsTypeConverter))]
        //[SRDescription("TrainingPlanHtmlExporter.PrintDays.Description")]
        //[SRDisplayName("TrainingPlanHtmlExporter.PrintDays.DisplayName")]
        //[SRCategory("PrintOptions")]
        //public string PrintDays
        //{
        //    get { return printDays; }
        //    set { printDays = value; }
        //}

        [SRDescription("TrainingPlanHtmlExporter.PrintSetsPlaceholders.Description")]
        [SRDisplayName("TrainingPlanHtmlExporter.PrintSetsPlaceholders.DisplayName")]
        [SRCategory("PrintOptions")]
        [DefaultValue(true)]
        public bool PrintSetsPlaceholders
        {
            get { return printSetsPlaceholders; }
            set { printSetsPlaceholders = value; }
        }

        

        

        [Browsable(false)]
        public TrainingPlan TrainingPlan
        {
            get { return trainingPlan; }
        }

        [SRDescription("TrainingPlanHtmlExporter.PrintLegend.Description")]
        [SRDisplayName("TrainingPlanHtmlExporter.PrintLegend.DisplayName")]
        [SRCategory("PrintOptions")]
        [DefaultValue(true)]
        public bool PrintLegend
        {
            get { return printLegend; }
            set { printLegend = value; }
        }

        #endregion

        

        //public Guid[] GetPrintDaysIds()
        //{
        //    List<Guid> ids = new List<Guid>();
        //    string[] days=new string[0];
        //    if(PrintDays!=null)
        //    {
        //        days = PrintDays.Split(DaysSeparator);
        //    }
            
        //    foreach (var day in days)
        //    {
        //        if (!string.IsNullOrEmpty(day))
        //        {
        //            ids.Add(new Guid(day));
        //        }
        //    }
        //    return ids.ToArray();
        //}

        [Browsable(false)]
        [CategoryAttribute("Name")]
        public override string Title
        {
            get { return trainingPlan.Name; }
        }

        [Browsable(false)]
        public override Guid GlobalId
        {
            get { return trainingPlan.GlobalId; }
        }

        [Browsable(false)]
        public override string Language
        {
            get { return trainingPlan.Language; }
        }

        protected override void Build(StringBuilder builder)
        {
  
            base.Build(builder);
            buildLegend(builder);
        }


        void prepareTrainingPlan()
        {
            if (TrainingPlan.Comment != null)
            {
                TrainingPlan.Comment = TrainingPlan.Comment.Replace("\n", "<br/>");
            }
            foreach (var day in TrainingPlan.Days)
            {
                foreach (var entry in day.Entries)
                {
                    if (!string.IsNullOrWhiteSpace(entry.Comment))
                    {
                        entry.Comment = entry.Comment.Replace("\n", "<br/>");
                    }
                    foreach (var series in entry.Sets)
                    {
                        if (!string.IsNullOrEmpty(series.Comment))
                        {
                            series.Comment = series.Comment.Replace("\n", "<br/>");
                        }
                    }
                }
            }
        }

        private void buildLegend(StringBuilder builder)
        {
            if (PrintLegend)
            {
                addGroupTable(builder, StrengthTrainingEntryStrings.TrainingPlanHtml_Legend, delegate(StringBuilder stringBuilder)
                            {
                                stringBuilder.AppendLine(StrengthTrainingEntryStrings.TrainingPlanHtml_LegendContent);

                            });

            }
        }
        //bool shouldPrintDay(TrainingPlanDay day)
        //{
        //    return Array.IndexOf(GetPrintDaysIds(), day.GlobalId) > -1;
        //}

        protected override void BuildDetails(StringBuilder builder)
        {
            //TODO:Finish
            //if(GetPrintDaysIds().Length==0)
            //{//user doesn't want to print any days so we can skip whole code here
            //    return;
            //}
            SuperSetViewManager superSetManager = new SuperSetViewManager();
            addGroupTable(builder, StrengthTrainingEntryStrings.TrainingPlanHtml_Details, delegate(StringBuilder stringBuilder)
            {
                string exerciseTypeHeader = "";
                if (PrintExerciseType)
                {
                    exerciseTypeHeader = string.Format("<th>{0}</th>", StrengthTrainingEntryStrings.TrainingPlanHtml_MuscleColumn);
                }
                string entryComment = "";
                if (PrintEntriesComment)
                {
                    entryComment = string.Format("<th class='commentColumn'>{0}</th>", StrengthTrainingEntryStrings.TrainingPlanHtml_CommentColumn);
                }
                builder.AppendLine("<tr><td colspan='2'><table class='exercises'>");
                builder.AppendFormat("<tr><th>{0}</th>{1}<th>{2}</th>{3}", StrengthTrainingEntryStrings.TrainingPlanHtml_ExerciseColumn, exerciseTypeHeader, StrengthTrainingEntryStrings.TrainingPlanHtml_RestTimeColumn, entryComment);

                int maxSets = TrainingPlan.GetMaximumSeriesCount();
                if (!PrintRequiredSetsOnly)
                {
                    maxSets = Math.Max(maxSets, SetsNumberToPrint);
                }
                for (int i = 0; i < maxSets; i++)
                {
                    int setNumber = i + 1;
                    builder.AppendFormat("<th>{0} {1}</th>", StrengthTrainingEntryStrings.TrainingPlanHtml_SetColumn, setNumber);
                }

                for (int i = 0; i < TrainingPlan.Days.Count; i++)
                {
                    TrainingPlanDay day = TrainingPlan.Days[i];
                    //if (!shouldPrintDay(day))
                    //{
                    //    continue;
                    //}
                    int colSpan = maxSets + 2;
                    if (PrintExerciseType)
                    {
                        colSpan++;
                    }
                    if (PrintEntriesComment)
                    {
                        colSpan++;
                    }
                    builder.AppendFormat("<tr><td height='8' colspan='{0}'></td></tr>", colSpan);
                    /*+++*/
                    builder.AppendFormat("<tr><td class='dayHeader' colspan='{0}'>{1}</td></tr>", colSpan, string.IsNullOrEmpty(day.Name) ? StrengthTrainingEntryStrings.ExercisesTrainingPlanListView_EmptyDayName : day.Name);

                    bool alt = false;
                    foreach (var planEntry in day.Entries)
                    {
                        ExerciseLightDTO exercise = planEntry.Exercise;
                        string superSetBgColor = "";

                        //get special color for superset entry
                        if (!string.IsNullOrEmpty(planEntry.GroupName))
                        {
                            var color = superSetManager.GetSuperSetColor(planEntry.GroupName);
                            superSetBgColor = string.Format("bgcolor='{0}'", color.GetKnownColorName());
                        }
                        string exerciseName = exercise.GetLocalizedName();
                        if (PrintExerciseShortcut && exercise.GlobalId != Constants.UnsavedGlobalId)
                        {//put shortcut only when this exercise is not deleted
                            exerciseName += string.Format("-({0})", exercise.Shortcut);
                        }
                        string exerciseTypeColumn = "";
                        if (PrintExerciseType)
                        {
                            exerciseTypeColumn = string.Format("<td>{0}</td>", EnumLocalizer.Default.Translate(exercise.ExerciseType));
                        }
                        string entriesComment = "";
                        if (PrintEntriesComment)
                        {
                            entriesComment = string.Format("<td class='commentColumn'>{0}</td>", planEntry.Comment);
                        }
                        int restTime = TrainingPlan.RestSeconds;
                        //if rest time for this exercise is set then we should show this time (not global rest time)
                        if (planEntry.RestSeconds.HasValue)
                        {
                            restTime = planEntry.RestSeconds.Value;
                        }
                        //string exerciseName = exercise.GlobalId !=Constants.UnsavedGlobalId? exercise.Name: "(Deleted)";
                        builder.AppendFormat("<tr class='{2}'><td {3}>{0}</td>{1}<td>{4}</td>{5}",
                                             exerciseName, exerciseTypeColumn,
                                             alt && UseAlternateRows ? "exercisesAlt" : "exercises", superSetBgColor, string.Format(StrengthTrainingEntryStrings.TrainingPlanHtml_RestTimeValue, restTime), entriesComment);

                        for (int j = 0; j < maxSets; j++)
                        {
                            if (planEntry.Sets.Count > j || !PrintRequiredSetsOnly)
                            {
                                var serie = planEntry.Sets.Count > j ? planEntry.Sets[j] : null;

                                bool isCardio = planEntry.Exercise.ExerciseType == ExerciseType.Cardio;
                                buildSet(builder, serie, isCardio);
                            }


                        }
                        alt = !alt;
                    }
                    /*+++*/
                    builder.AppendLine("</tr>");

                }
                builder.Append("</tr>");
                builder.AppendLine("</table></td></tr>");
            });
        }

        void buildSet(StringBuilder builder, TrainingPlanSerie set, bool isCardio)
        {
            string setTable = set != null ? set.GetDisplayText(isCardio) : "&nbsp;";
            if (PrintSetsPlaceholders)
            {
                setTable = string.Format("<table border='0'><tr><td>{0}</td></tr><tr><td>__x__</td></tr></table>", setTable);
            }
            if (PrintSetsComment)
            {
                setTable = string.Format("<table border='0' width='200'><tr><td>{0}</td></tr><tr><td>{1}</td></tr></table>", setTable, set!=null?set.Comment:"&nbsp;");
            }
            
            builder.AppendFormat("<td>{0}</td>", setTable);
        }

        protected override void BuildDescription(StringBuilder builder)
        {
            addGroupTable(builder, StrengthTrainingEntryStrings.TrainingPlanHtml_Description, delegate(StringBuilder stringBuilder)
            {
                stringBuilder.AppendFormat(
                    "<tr><td  colspan='2'>{0}</td></tr>",
                    TrainingPlan.Comment);
            });
        }

        protected override void BuildInfo(StringBuilder builder)
        {
            addGroupTable(builder, StrengthTrainingEntryStrings.TrainingPlanHtml_Information, delegate(StringBuilder stringBuilder)
            {
                stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", StrengthTrainingEntryStrings.TrainingPlanHtml_InfoAuthor, TrainingPlan.Author);
                stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", StrengthTrainingEntryStrings.TrainingPlanHtml_InfoCreationDate, TrainingPlan.CreationDate.ToLocalTime().ToShortDateString());
                string difficultText = new EnumLocalizer(StrengthTrainingEntryStrings.ResourceManager).Translate(TrainingPlan.Difficult);
                stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", StrengthTrainingEntryStrings.TrainingPlanHtml_InfoDifficult, difficultText);
                stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", StrengthTrainingEntryStrings.TrainingPlanHtml_InfoTrainingType, TrainingPlan.TrainingType);
                stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", StrengthTrainingEntryStrings.TrainingPlanHtml_InfoRestTime, string.Format(StrengthTrainingEntryStrings.TrainingPlanHtml_RestTimeValue, TrainingPlan.RestSeconds));
                stringBuilder.AppendFormat("<tr><td>{0}</td><td><a href='{1}'>{1}</a></td></tr>", StrengthTrainingEntryStrings.TrainingPlanHtml_InfoUrl, TrainingPlan.Url);
                var language = Service.V2.Model.Language.GetLanguage(TrainingPlan.Language);
                if (language != null)
                {
                    stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", StrengthTrainingEntryStrings.TrainingPlanHtml_Language, language.DisplayName);
                }
            });
        }
       
    }

    
}