using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.PreviewGenerator;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.Suplements.PreviewGenerator
{
    [Serializable]
    public class SupplementsCycleDefinitionHtmlExporter : HtmlCycleExporterBase
    {
        private SupplementCycleDefinitionDTO definition;
        private SupplementCycleDefinitionDTO originalDefinition;
        private bool _printSupplementType;
        private int weeks;

        public SupplementsCycleDefinitionHtmlExporter(SupplementCycleDefinitionDTO definition):base(definition.Name)
        {
            this.originalDefinition = definition;
            weeks = definition.GetTotalWeeks();
            prepare();
        }

        [Browsable(false)]
        public override Guid GlobalId
        {
            get { return definition.GlobalId; }
        }

        [Browsable(false)]
        public override string Language
        {
            get { return definition.Language; }
        }

        [SRDescription("SupplementsCycleDefinitionHtmlExporter.PrintSupplementName.Description")]
        [SRDisplayName("SupplementsCycleDefinitionHtmlExporter.PrintSupplementName.DisplayName")]
        [SRCategory("PrintOptions")]
        [DefaultValue(true)]
        public bool PrintSupplementType
        {
            get { return _printSupplementType; }
            set { _printSupplementType = value; }
        }

        [SRCategory("PrintOptions")]
        [SRDescription("SupplementsCycleDefinitionHtmlExporter.PrintWeeks.Description")]
        [SRDisplayName("SupplementsCycleDefinitionHtmlExporter.PrintWeeks.DisplayName")]
        public int Weeks
        {
            get
            {
                
                return weeks;
            }
            set
            {
                if (!UIHelper.EnsurePremiumLicence())
                {
                    weeks= originalDefinition.GetTotalWeeks();
                }
                else
                {
                    weeks = value;    
                }
                
            }
        }

        [Browsable(false)]
        public SupplementCycleDefinitionDTO CycleDefinition
        {
            get { return originalDefinition; }
        }

        void prepare()
        {
            CycleDefinition.Comment = normalizeString(CycleDefinition.Comment);
            foreach (var week in CycleDefinition.Weeks)
            {
                week.Comment = normalizeString(week.Comment);
                foreach (var entry in week.Dosages)
                {
                    entry.Comment = normalizeString(entry.Comment);
                }
            }
        }

        protected override void PrepareData()
        {
            SupplementsCycleRepetiter repetiter = new SupplementsCycleRepetiter();

            definition=repetiter.Preapre(originalDefinition, Weeks);
        }

        protected override void BuildDescription(StringBuilder builder)
        {
            addGroupTable(builder, EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Suplements:SuplementsEntryStrings:SupplementsCycleDefinitionHtmlExporter_BuildDescription"), delegate(StringBuilder stringBuilder)
            {
                stringBuilder.AppendFormat("<tr><td  colspan='2'>{0}</td></tr>", definition.Comment);
            });
        }

        protected override void BuildInfo(StringBuilder builder)
        {
            addGroupTable(builder, EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Suplements:SuplementsEntryStrings:SupplementsCycleDefinitionHtmlExporter_BuildInfo_Info"), delegate(StringBuilder stringBuilder)
            {
                stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Suplements:SuplementsEntryStrings:SupplementsCycleDefinitionHtmlExporter_BuildInfo_Author"), definition.Author);
                stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Suplements:SuplementsEntryStrings:SupplementsCycleDefinitionHtmlExporter_BuildInfo_CreationDate"), definition.CreationDate.ToLocalTime().ToShortDateString());
                string difficultText = EnumLocalizer.Default.Translate(definition.Difficult);
                stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Suplements:SuplementsEntryStrings:SupplementsCycleDefinitionHtmlExporter_BuildInfo_Difficulty"), difficultText);
                stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Suplements:SuplementsEntryStrings:SupplementsCycleDefinitionHtmlExporter_BuildInfo_Weeks"), string.Format(EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Suplements:SuplementsEntryStrings:SupplementsCycleDefinitionHtmlExporter_BuildInfo_Weeks_Value"), Weeks));
                stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Suplements:SuplementsEntryStrings:SupplementsCycleDefinitionHtmlExporter_BuildInfo_Purpose"), EnumLocalizer.Default.Translate(definition.Purpose));
                stringBuilder.AppendFormat("<tr><td>{0}</td><td><a href='{1}'>{1}</a></td></tr>", "Url", definition.Url);
                var language = Service.V2.Model.Language.GetLanguage(definition.Language);
                if (language != null)
                {
                    stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Suplements:SuplementsEntryStrings:SupplementsCycleDefinitionHtmlExporter_BuildInfo_Language"), language.DisplayName);
                }
            });
        }

        protected override void BuildDetails(StringBuilder builder)
        {
            addGroupTable(builder, EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Suplements:SuplementsEntryStrings:SupplementsCycleDefinitionHtmlExporter_BuildInfo_Details"), delegate(StringBuilder stringBuilder)
                                                  {
                                                      //add empty header for weeks
                                                      stringBuilder.Append("<TR><TD><table><TR><TH class='dayHeader'></TH>");

                                                      if(this.PrintEntriesComment)
                                                      {
                                                          stringBuilder.AppendFormat("<TH class='commentColumn'>{0}<BR/></TH>", "SupplementsCycleDefinitionHtmlExporter_Column_Comment".TranslateSupple());
                                                      }
                                                      //now add headers for every supplement
                                                      List<IGrouping<Tuple<SuplementDTO, string>, SupplementCycleDosageDTO>> supplementsGroup = definition.Weeks.SelectMany(x => x.Dosages).OfType<SupplementCycleDosageDTO>().GroupBy(x => new Tuple<SuplementDTO, string>(x.Supplement, x.Name)).ToList();

                                                      //var supplements = CycleDefinition.GetSupplements();
                                                      int supplementsNumber = supplementsGroup.Count;
                                                      for (int i = 0; i < supplementsNumber; i++)
                                                      {
                                                          var name = supplementsGroup[i].Key.Item1.Name;
                                                          if(!string.IsNullOrEmpty(supplementsGroup[i].Key.Item2))
                                                          {
                                                              if (PrintSupplementType)
                                                              {
                                                                  name += " - " + supplementsGroup[i].Key.Item2;
                                                              }
                                                              else
                                                              {
                                                                  name = supplementsGroup[i].Key.Item2;
                                                              }
                                                              
                                                          }
                                                          stringBuilder.AppendFormat("<TH class='dayHeader'>{0}<BR/></TH>", name);
                                                      }
                                                      stringBuilder.AppendLine("</TR>");

                                                      bool alt = false;
                                                      for (int i = 1; i <= Weeks; i++)
                                                      {
                                                          buildWeekRow(i,stringBuilder,supplementsGroup, alt);
                                                          alt = !alt;
                                                      }
                                                      stringBuilder.AppendLine("</TABLE></TD></TR>");
            });
        }

        private void buildWeekRow(int weekNumber, StringBuilder stringBuilder, List<IGrouping<Tuple<SuplementDTO, string>, SupplementCycleDosageDTO>> supplementsGroup,bool alternateRow)
        {
            stringBuilder.AppendFormat(EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Suplements:SuplementsEntryStrings:SupplementsCycleDefinitionHtmlExporter_BuildInfo_Week"), weekNumber, alternateRow && UseAlternateRows ? "exercisesAlt" : "exercises");
            var currentWeeks = definition.GetWeek(weekNumber);

            if (this.PrintEntriesComment)
            {
                var comment = string.Join("<br/>", currentWeeks.Select(x => x.Comment));
                stringBuilder.AppendFormat("<TD class='commentColumn'>{0}</TD>", comment);
            }

            for (int k = 0; k < supplementsGroup.Count; k++)
            {
                var suppleGroup = supplementsGroup[k];
                var dosages = getDosage(suppleGroup.Key.Item1, suppleGroup.Key.Item2, weekNumber);
                stringBuilder.Append("<TD cellpadding='6'>");

                var groupedDosages=dosages.GroupBy(x => x.Repetitions);

                bool hasMany = false;
                foreach (var groupedDosage in groupedDosages)
                {
                    if (hasMany)
                    {
                        stringBuilder.Append("<hr/>");
                    }
                    var repetitions = EnumLocalizer.Default.Translate(groupedDosage.Key);
                    stringBuilder.AppendFormat("<i>{0}</i><BR/>", repetitions);

                    foreach (var dosage in groupedDosage.OrderBy(x=>x.TimeType))
                    {
                        //if(!string.IsNullOrWhiteSpace(dosage.Name))
                        //{
                        //    stringBuilder.AppendFormat("{0}<BR/>", dosage.Name);
                        //}
                        

                        string unit = "";
                        string timeType = "";
                        if (dosage.TimeType != TimeType.NotSet)
                        {
                            timeType = EnumLocalizer.Default.Translate(dosage.TimeType);
                        }
                        if (dosage.DosageUnit == DosageUnit.ON10KgWight)
                        {//todo: add lbs here
                            unit = EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Suplements:SuplementsEntryStrings:SupplementsCycleDefinitionHtmlExporter_BuildInfo_DosagePer10Kg");
                        }
                        var dosageType = EnumLocalizer.Default.Translate(dosage.DosageType);

                        stringBuilder.AppendFormat("{1}:{0} {2} {3}<br/>", dosage.Dosage.ToString("0.##"), dosageType, unit.ToLower(), timeType.ToLower());

                        //if (!string.IsNullOrEmpty(dosage.GroupName))
                        //{
                        //    stringBuilder.AppendFormat("(<superscript>{0}</superscript>)",dosage.GroupName);
                        //}
                        //stringBuilder.Append("<br/>");

                        if (PrintEntriesComment && !string.IsNullOrEmpty(dosage.Comment))
                        {
                            stringBuilder.AppendFormat("<BR/>{0}", dosage.Comment);
                        }
                        

                        
                    }
                    hasMany = true;
                }
                
                
                stringBuilder.Append("</TD>");
            }

            
            stringBuilder.AppendLine("</TR>");
        }

        private List<SupplementCycleDosageDTO> getDosage(SuplementDTO suplementDTO, string name, int weekNumber)
        {
            List<SupplementCycleDosageDTO> dosages = new List<SupplementCycleDosageDTO>();
            foreach (var week in definition.Weeks)
            {
                if (weekNumber >= week.CycleWeekStart && weekNumber <= week.CycleWeekEnd)
                {
                    foreach (var dosage in week.Dosages.OfType<SupplementCycleDosageDTO>())
                    {
                        if (dosage.Supplement.GlobalId == suplementDTO.GlobalId && dosage.Name==name)
                        {
                            dosages.Add(dosage);
                        }
                    }
                }
            }
            return dosages;
        }

        //List<SupplementCycleDosageDTO> getDosage(Guid supplementId, int weekNumber)
        //{
        //    List<SupplementCycleDosageDTO> dosages = new List<SupplementCycleDosageDTO>();
        //    foreach (var week in CycleDefinition.Weeks)
        //    {
        //        if (weekNumber >= week.CycleWeekStart && weekNumber <= week.CycleWeekEnd)
        //        {
        //            foreach (var dosage in week.Dosages.OfType<SupplementCycleDosageDTO>())
        //            {
        //                if(dosage.Supplement.GlobalId==supplementId)
        //                {
        //                    dosages.Add(dosage);
        //                }
        //            }
        //        }
        //    }
        //    return dosages;
        //}

        [Browsable(false)]
        public override string Title
        {
            get { return originalDefinition.Name; }
        }
    }
}
