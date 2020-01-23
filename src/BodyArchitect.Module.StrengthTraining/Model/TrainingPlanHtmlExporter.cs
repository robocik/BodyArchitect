using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Module.StrengthTraining.Localization;
using BodyArchitect.Service.Model;
using BodyArchitect.Service.Model.TrainingPlans;
using BodyArchitect.Shared;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Controls.UserControls;
using BodyArchitect.Logger;
using BodyArchitect.Module.StrengthTraining.Controls;
using BodyArchitect.Module.StrengthTraining.Controls.SourceGridExtension;
using Microsoft.Win32;

namespace BodyArchitect.Module.StrengthTraining.Model.TrainingPlans
{
    public class TrainingPlanHtmlExporter:IHtmlProvider,IDisposable
    {
        private bool printInfo = true;
        private bool printDescription = true;
        private bool printRequiredSetsOnly = true;
        private int setsNumberToPrint = 2;
        private string printDays;
        private TrainingPlan trainingPlan;
        private bool printSetsPlaceholders = true;
        private bool printHeaders = true;
        private bool useAlternateRows = true;
        private bool initialPrintBackground;
        private bool printExerciseType = true;
        private bool printExerciseShortcut = false;
        private bool printSetsComment = false;
        private bool printLegend = true;
        private bool printEntriesComment = true;

        public const char DaysSeparator = ';';

        public TrainingPlanHtmlExporter(TrainingPlan trainingPlan)
        {
            this.trainingPlan = trainingPlan;
            prepareTrainingPlan();
            initialPrintBackground = PrintBackground;

            setAllTrainingDaysToPrint(trainingPlan);
        }

        private void setAllTrainingDaysToPrint(TrainingPlan trainingPlan)
        {
            List<Guid> allDays = new List<Guid>();
            foreach (var day in trainingPlan.Days)
            {
                allDays.Add(day.GlobalId);
            }
            var days = allDays.ToArray();
            StringBuilder builder = new StringBuilder();
            foreach (var dayId in days)
            {
                builder.AppendFormat("{0}{1}", dayId, TrainingPlanHtmlExporter.DaysSeparator);
            }
            PrintDays= builder.ToString().TrimEnd(TrainingPlanHtmlExporter.DaysSeparator);
        }

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

        [SRDescription("TrainingPlanHtmlExporter.PrintEntriesComment.Description")]
        [SRDisplayName("TrainingPlanHtmlExporter.PrintEntriesComment.DisplayName")]
        [SRCategory("PrintOptions")]
        [DefaultValue(true)]
        public bool PrintEntriesComment
        {
            get { return printEntriesComment; }
            set { printEntriesComment = value; }
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

        [SRDescription("TrainingPlanHtmlExporter.PrintInfo.Description")]
        [SRDisplayName("TrainingPlanHtmlExporter.PrintInfo.DisplayName")]
        [SRCategory("PrintOptions")]
        [DefaultValue(true)]
        public bool PrintInfo
        {
            get { return printInfo; }
            set { printInfo = value; }
        }

        [SRDescription("TrainingPlanHtmlExporter.PrintBackground.Description")]
        [SRDisplayName("TrainingPlanHtmlExporter.PrintBackground.DisplayName")]
        [SRCategory("PrintOptions")]
        [DefaultValue(false)]
        public bool PrintBackground
        {
            get
            {
                try
                {
                    RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("Microsoft").OpenSubKey("Internet Explorer").OpenSubKey("Main");

                    //Get the current setting so that we can revert it after printjob
                    object defaultPrintBackground = regKey.GetValue("Print_Background");
                    return object.Equals((string)defaultPrintBackground,"yes");
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Default.Process(ex);
                }
                return false;
            }
            set
            {
                try
                {
                    RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("Microsoft").OpenSubKey("Internet Explorer").OpenSubKey("Main", true);
                    regKey.SetValue("Print_Background", value ? "yes" : "no");
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Default.Process(ex);
                }
                
            }
        }

        [SRDescription("TrainingPlanHtmlExporter.PrintDescription.Description")]
        [SRDisplayName("TrainingPlanHtmlExporter.PrintDescription.DisplayName")]
        [SRCategory("PrintOptions")]
        [DefaultValue(true)]
        public bool PrintDescription
        {
            get { return printDescription; }
            set { printDescription = value; }
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

        [EditorAttribute(typeof(TrainingPlanDaysPropertyEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(TrainingPlanDayIdsTypeConverter))]
        [SRDescription("TrainingPlanHtmlExporter.PrintDays.Description")]
        [SRDisplayName("TrainingPlanHtmlExporter.PrintDays.DisplayName")]
        [SRCategory("PrintOptions")]
        public string PrintDays
        {
            get { return printDays; }
            set { printDays = value; }
        }

        [SRDescription("TrainingPlanHtmlExporter.PrintSetsPlaceholders.Description")]
        [SRDisplayName("TrainingPlanHtmlExporter.PrintSetsPlaceholders.DisplayName")]
        [SRCategory("PrintOptions")]
        [DefaultValue(true)]
        public bool PrintSetsPlaceholders
        {
            get { return printSetsPlaceholders; }
            set { printSetsPlaceholders = value; }
        }

        [SRDescription("TrainingPlanHtmlExporter.PrintHeaders.Description")]
        [SRDisplayName("TrainingPlanHtmlExporter.PrintHeaders.DisplayName")]
        [SRCategory("PrintOptions")]
        [DefaultValue(true)]
        public bool PrintHeaders
        {
            get { return printHeaders; }
            set { printHeaders = value; }
        }

        [SRDescription("TrainingPlanHtmlExporter.UseAlternateRows.Description")]
        [SRDisplayName("TrainingPlanHtmlExporter.UseAlternateRows.DisplayName")]
        [SRCategory("PrintOptions")]
        [DefaultValue(true)]
        public bool UseAlternateRows
        {
            get { return useAlternateRows; }
            set { useAlternateRows = value; }
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

        void addHeader(string title,StringBuilder builder)
        {
            if(PrintHeaders)
            {
                builder.AppendFormat("<tr><th class='label' colspan='2'>{0}</th></tr>",title);
            }
        }

        void addGroupTable(StringBuilder builder,string header,HtmlProcessMethod method)
        {
            builder.AppendLine("<tr><td><table cellspacing='0' cellpadding='0' class='group'>");
            addHeader(header, builder);
            method(builder);
            builder.AppendLine("</table></td></tr>");
        }

        delegate void HtmlProcessMethod(StringBuilder builder);

        public Guid[] GetPrintDaysIds()
        {
            List<Guid> ids = new List<Guid>();
            string[] days=new string[0];
            if(PrintDays!=null)
            {
                days = PrintDays.Split(DaysSeparator);
            }
            
            foreach (var day in days)
            {
                if (!string.IsNullOrEmpty(day))
                {
                    ids.Add(new Guid(day));
                }
            }
            return ids.ToArray();
        }

        public string GetHtml()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<html>");
            buildHtmlHead(builder);
            builder.AppendLine("<body>");
            builder.AppendFormat("<h1>{0}</h1>", TrainingPlan.Name);
            builder.AppendLine("<table class='main'>");
            buildInfo(builder);
            
            buildDescription(builder);
            buildDetails(builder);
            buildLegend(builder);
            
            builder.AppendLine("</table>");
            builder.AppendLine("</body>");
            builder.AppendLine("</html>");

            return builder.ToString();
        }

        void prepareTrainingPlan()
        {
            TrainingPlan.Comment = TrainingPlan.Comment.Replace("\n","<br/>");
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
        bool shouldPrintDay(TrainingPlanDay day)
        {
            return Array.IndexOf(GetPrintDaysIds(), day.GlobalId) > -1;
        }

        private void buildDetails(StringBuilder builder)
        {
            if(GetPrintDaysIds().Length==0)
            {//user doesn't want to print any days so we can skip whole code here
                return;
            }
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
                                                          if (!shouldPrintDay(day))
                                                          {
                                                              continue;
                                                          }
                                                          int colSpan = maxSets + 2;
                                                          if (PrintExerciseType)
                                                          {
                                                              colSpan++;
                                                          }
                                                          if(PrintEntriesComment)
                                                          {
                                                              colSpan++;
                                                          }
                                                          builder.AppendFormat("<tr><td height='8' colspan='{0}'></td></tr>",colSpan);
                                                          /*+++*/
                                                          builder.AppendFormat("<tr><td class='dayHeader' colspan='{0}'>{1}</td></tr>", colSpan, string.IsNullOrEmpty(day.Name) ? StrengthTrainingEntryStrings.ExercisesTrainingPlanListView_EmptyDayName : day.Name);
                                                              
                                                          bool alt = false;
                                                          foreach (var planEntry in day.Entries)
                                                          {
                                                              ExerciseDTO exercise = ObjectsReposidory.GetExercise(planEntry.ExerciseId);
                                                              string superSetBgColor = "";

                                                              //get special color for superset entry
                                                              var superSet=day.GetSuperSet(planEntry);
                                                              if(superSet!=null)
                                                              {
                                                                  var color = superSetManager.GetSuperSetColor(superSet.SuperSetId.ToString());
                                                                  superSetBgColor = string.Format("bgcolor='{0}'",color.ToKnownColor().ToString());
                                                              }
                                                              string exerciseName = exercise.GetLocalizedName();
                                                              if(PrintExerciseShortcut && exercise.GlobalId!=Constants.UnsavedGlobalId)
                                                              {//put shortcut only when this exercise is not deleted
                                                                  exerciseName += string.Format("-({0})",exercise.Shortcut);
                                                              }
                                                              string exerciseTypeColumn = "";
                                                              if(PrintExerciseType)
                                                              {
                                                                  exerciseTypeColumn = string.Format("<td>{0}</td>", EnumLocalizer.Default.Translate(exercise.ExerciseType));
                                                              }
                                                              string entriesComment = "";
                                                              if(PrintEntriesComment)
                                                              {
                                                                  entriesComment = string.Format("<td class='commentColumn'>{0}</td>", planEntry.Comment);
                                                              }
                                                              int restTime = TrainingPlan.RestSeconds;
                                                              //if rest time for this exercise is set then we should show this time (not global rest time)
                                                              if(planEntry.RestSeconds>0)
                                                              {
                                                                  restTime = planEntry.RestSeconds;
                                                              }
                                                              //string exerciseName = exercise.GlobalId !=Constants.UnsavedGlobalId? exercise.Name: "(Deleted)";
                                                              builder.AppendFormat("<tr class='{2}'><td {3}>{0}</td>{1}<td>{4}</td>{5}",
                                                                                   exerciseName, exerciseTypeColumn,
                                                                                   alt && UseAlternateRows ? "exercisesAlt" : "exercises", superSetBgColor, string.Format(StrengthTrainingEntryStrings.TrainingPlanHtml_RestTimeValue, restTime), entriesComment);

                                                              for (int j = 0; j < maxSets; j++)
                                                              {
                                                                  if (planEntry.Sets.Count > j || !PrintRequiredSetsOnly)
                                                                  {
                                                                      var serie =planEntry.Sets.Count > j ? planEntry.Sets[j] : null;

                                                                      buildSet(builder, serie);
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

        void buildSet(StringBuilder builder,TrainingPlanSerie set)
        {
            string setTable = set != null ? set.GetDisplayText() : "&nbsp;";
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
        private void buildDescription(StringBuilder builder)
        {
            if (PrintDescription)
            {
                addGroupTable(builder, StrengthTrainingEntryStrings.TrainingPlanHtml_Description, delegate(StringBuilder stringBuilder)
                         {
                             stringBuilder.AppendFormat(
                                 "<tr><td  colspan='2'>{0}</td></tr>",
                                 TrainingPlan.Comment);
                         });
            }
        }

        private void buildInfo(StringBuilder builder)
        {
            if (PrintInfo)
            {
                addGroupTable(builder, StrengthTrainingEntryStrings.TrainingPlanHtml_Information, delegate(StringBuilder stringBuilder)
                        {
                            stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", StrengthTrainingEntryStrings.TrainingPlanHtml_InfoAuthor, TrainingPlan.Author);
                            stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", StrengthTrainingEntryStrings.TrainingPlanHtml_InfoCreationDate,TrainingPlan.CreationDate.ToLocalTime().ToShortDateString());
                            string difficultText = new EnumLocalizer(StrengthTrainingEntryStrings.ResourceManager).Translate(TrainingPlan.Difficult);
                            stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", StrengthTrainingEntryStrings.TrainingPlanHtml_InfoDifficult, difficultText);
                            stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", StrengthTrainingEntryStrings.TrainingPlanHtml_InfoTrainingType, TrainingPlan.TrainingType);
                            stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", StrengthTrainingEntryStrings.TrainingPlanHtml_InfoRestTime,string.Format(StrengthTrainingEntryStrings.TrainingPlanHtml_RestTimeValue,TrainingPlan.RestSeconds));
                            stringBuilder.AppendFormat("<tr><td>{0}</td><td><a href='{1}'>{1}</a></td></tr>", StrengthTrainingEntryStrings.TrainingPlanHtml_InfoUrl, TrainingPlan.Url);
                            var language = Language.GetLanguage(TrainingPlan.Language);
                            if(language!=null)
                            {
                                stringBuilder.AppendFormat("<tr><td>{0}</td><td><a href='{1}'>{1}</a></td></tr>",StrengthTrainingEntryStrings.TrainingPlanHtml_Language, language.DisplayName);
                            }
                        });
                 
            }
        }

        private void buildHtmlHead(StringBuilder builder)
        {
            builder.AppendFormat("<link rel='stylesheet' href='{0}' type='text/css' >",Path.Combine(Application.StartupPath,"Css\\TrainingPlan.css"));
            builder.AppendFormat("<title>{0}</title>", TrainingPlan.Name);
        }

        public void Dispose()
        {
            PrintBackground = initialPrintBackground;
        }
    }

    
}