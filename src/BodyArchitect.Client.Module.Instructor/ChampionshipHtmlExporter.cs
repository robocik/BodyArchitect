using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Module.Instructor.ViewModel;
using BodyArchitect.Client.Module.StrengthTraining;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.PreviewGenerator;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.Instructor
{
    [DefaultPropertyAttribute("Title")]
    [Serializable]
    public class ChampionshipHtmlExporter : HtmlCycleExporterBase
    {
        private ChampionshipDTO championship;

        public ChampionshipHtmlExporter(ChampionshipDTO championship)
            : base(championship.Name)
        {
            this.championship = championship;
        }

        [Browsable(false)]
        [CategoryAttribute("Name")]
        public override string Title
        {
            get { return championship.Name; }
        }

        [Browsable(false)]
        public override Guid GlobalId
        {
            get { return championship.GlobalId; }
        }

        [Browsable(false)]
        public override string Language
        {
            get { return null; }
        }

        protected override void BuildDetails(StringBuilder builder)
        {
            
            foreach (IGrouping<ChampionshipCategoryDTO, ChampionshipResultItemDTO> dto in championship.Results.GroupBy(x => x.Category).OrderBy(x => x.Key.Category))
            {
                var mainItem = new ChampionshipResultsViewModel(dto);
                
                addGroupTable(builder, mainItem.Category, delegate(StringBuilder stringBuilder)
                {
                    if (dto.Key.Category != ChampionshipWinningCategories.Druzynowa)
                    {
                        stringBuilder.AppendFormat("<TR><TD><table width='100%' height='100%' style=' solid black;border-collapse:collapse;' Border='1' RULES='COLS' FRAME='VOID'><TR><TH class='dayHeader'></TH><TH class='dayHeader'>{0}</TH>", InstructorStrings.ChampionshipDataGrid_Grid_Customer);
                        stringBuilder.AppendFormat("<TH class='dayHeader'  >{0}</TH>",InstructorStrings.ChampionshipView_Grid_Year);
                        stringBuilder.AppendFormat("<TH class='dayHeader'  >{0}</TH>", InstructorStrings.ChampionshipDataGrid_Grid_Team);
                        stringBuilder.AppendFormat("<TH class='dayHeader' >{0}</TH>", InstructorStrings.ChampionshipView_Grid_Weight);

                        if (this.PrintEntriesComment)
                        {
                            stringBuilder.AppendFormat("<TH class='commentColumn' >{0}</TH>", InstructorStrings.ChampionshipDataGrid_Grid_Comment);
                        }

                        stringBuilder.AppendFormat("<TH class='dayHeader'  colspan='3'>{0}</TH>", ExercisesReposidory.Instance.BenchPress.Name);
                        if (championship.ChampionshipType == ChampionshipType.Trojboj)
                        {
                            stringBuilder.AppendFormat("<TH class='dayHeader'  colspan='3'>{0}</TH>", ExercisesReposidory.Instance.Deadlift.Name);
                            stringBuilder.AppendFormat("<TH class='dayHeader'  colspan='3'>{0}</TH>", ExercisesReposidory.Instance.Sqad.Name);
                        }
                        stringBuilder.AppendFormat("<TH class='dayHeader' >{0}</TH>", InstructorStrings.ChampionshipView_Grid_Total);
                        stringBuilder.AppendFormat("<TH class='dayHeader' >{0}</TH></TR>", InstructorStrings.ChampionshipView_Grid_Wilks);

                        stringBuilder.Append("<TR><TH/><TH/><TH/><TH/><TH/>");
                        if (this.PrintEntriesComment)
                        {
                            stringBuilder.Append("<TH class='commentColumn' ></TH>");
                        }
                        stringBuilder.Append("<TH>1</TH><TH>2</TH><TH>3</TH>");
                        if (championship.ChampionshipType == ChampionshipType.Trojboj)
                        {
                            stringBuilder.Append("<TH >1</TH><TH>2</TH><TH>3</TH>");
                            stringBuilder.Append("<TH >1</TH><TH>2</TH><TH>3</TH>");
                        }
                        stringBuilder.Append("<TH/><TH/></TR>");
                        if (dto.Key.Type==ChampionshipCategoryType.Weight)
                        {
                            
                            var items=mainItem.Items.OrderBy(x=>x.ResultItem.Value).GroupBy(x => x.WeightCategory);
                            foreach (IGrouping<string, ChampionshipResultItemViewModel> grouping in items)
                            {
                                bool alt = false;
                                stringBuilder.AppendFormat("<TR><TD/><TD class='dayHeader'><b class='doNotTranslate'>{0}</b></TD><TD class='dayHeader'/><TD class='dayHeader'/><TD class='dayHeader'/><TD class='dayHeader'/><TD class='dayHeader'/><TD class='dayHeader'/><TD class='dayHeader'/><TD class='dayHeader'/>", grouping.Key);
                                if (this.PrintEntriesComment)
                                {
                                    stringBuilder.Append("<TD class='dayHeader'/>");
                                }
                                if (championship.ChampionshipType == ChampionshipType.Trojboj)
                                {
                                    stringBuilder.Append("<TD class='dayHeader'/><TD class='dayHeader'/><TD class='dayHeader'/><TD class='dayHeader'/><TD class='dayHeader'/><TD class='dayHeader'/>");
                                }
                                foreach (var itemViewModel in grouping)
                                {
                                    buildCustomerItem(stringBuilder, itemViewModel, alt);
                                    alt = !alt;
                                }
                            }
                        }
                        else
                        {
                            bool alt = false;
                            foreach (var itemViewModel in mainItem.Items)
                            {
                                buildCustomerItem(stringBuilder, itemViewModel,alt);
                                alt = !alt;
                            }
                        }
                        stringBuilder.AppendLine("</TABLE></TD></TR>");
                    }
                    else
                    {
                        stringBuilder.AppendFormat("<TR><TD><table width='100%' height='100%' style=' solid black;border-collapse:collapse;' Border='1' RULES='COLS' FRAME='VOID'><TR><TH class='dayHeader'></TH><TH class='dayHeader'>{0}</TH>", InstructorStrings.ChampionshipDataGrid_Grid_Team);
                        stringBuilder.AppendFormat("<TH class='dayHeader' >{0}</TH></TR>", InstructorStrings.ChampionshipView_Grid_TeamPoints);
                        bool alt = false;
                        foreach (var itemViewModel in mainItem.Items)
                        {
                            stringBuilder.AppendFormat("<TR class='{1}'><TD width='20' class='doNotTranslate'>{0}</TD>", itemViewModel.Position, alt && UseAlternateRows ? "exercisesAlt" : "exercises");
                            stringBuilder.AppendFormat("<TD class='doNotTranslate' align='center'>{0}</TD>", itemViewModel.FullName);
                            stringBuilder.AppendFormat("<TD class='doNotTranslate' align='center'>{0}</TD></TR>", itemViewModel.Wilks);
                            alt = !alt;
                        }
                        stringBuilder.AppendLine("</TABLE></TD></TR>");
                    }
                });
            }

            
        }

        private void buildCustomerItem(StringBuilder stringBuilder, ChampionshipResultItemViewModel itemViewModel,bool alternateRow)
        {
            stringBuilder.AppendFormat("<TR class='{1}'><TD class='doNotTranslate'  align='center'>{0}</TD>", itemViewModel.Position, alternateRow && UseAlternateRows ? "exercisesAlt" : "exercises");
            stringBuilder.AppendFormat("<TD><span class='doNotTranslate'  align='center'>{0}</span></TD>", itemViewModel.FullName);
            stringBuilder.AppendFormat("<TD class='doNotTranslate'  align='center'>{0}</TD>", itemViewModel.Customer.Birthday.Value.Year);
            stringBuilder.AppendFormat("<TD class='doNotTranslate'  align='center'>{0}</TD>",
                                       itemViewModel.ResultItem.Customer.Group != null
                                           ? itemViewModel.ResultItem.Customer.Group.Name
                                           : "");
            stringBuilder.AppendFormat("<TD class='doNotTranslate' >{0}</TD>", itemViewModel.Weight);

            if(this.PrintEntriesComment)
            {
                stringBuilder.AppendFormat("<TD class='commentColumn'>{0}</TD>", itemViewModel.ResultItem.Customer.Comment);
            }
            ChampionshipEntryViewModel entryViewModel =new ChampionshipEntryViewModel(itemViewModel.Customer, championship, null);
            stringBuilder.AppendFormat("<TD class='doNotTranslate'  align='center'>{0}</TD>", getTry(entryViewModel.Exercise1.Entry.Try1));
            stringBuilder.AppendFormat("<TD class='doNotTranslate'  align='center'>{0}</TD>", getTry(entryViewModel.Exercise1.Entry.Try2));
            stringBuilder.AppendFormat("<TD class='doNotTranslate'  align='center'>{0}</TD>", getTry(entryViewModel.Exercise1.Entry.Try3));

            if (championship.ChampionshipType == ChampionshipType.Trojboj)
            {
                stringBuilder.AppendFormat("<TD class='doNotTranslate' align='center'>{0}</TD>",getTry(entryViewModel.Exercise2.Entry.Try1));
                stringBuilder.AppendFormat("<TD class='doNotTranslate' align='center'>{0}</TD>",getTry(entryViewModel.Exercise2.Entry.Try2));
                stringBuilder.AppendFormat("<TD class='doNotTranslate' align='center'>{0}</TD>",getTry(entryViewModel.Exercise2.Entry.Try3));

                stringBuilder.AppendFormat("<TD class='doNotTranslate' align='center'>{0}</TD>",getTry(entryViewModel.Exercise3.Entry.Try1));
                stringBuilder.AppendFormat("<TD class='doNotTranslate' align='center'>{0}</TD>",getTry(entryViewModel.Exercise3.Entry.Try2));
                stringBuilder.AppendFormat("<TD class='doNotTranslate' align='center'>{0}</TD>",getTry(entryViewModel.Exercise3.Entry.Try3));
            }
            stringBuilder.AppendFormat("<TD class='doNotTranslate' align='center'>{0}</TD>", itemViewModel.Total);
            stringBuilder.AppendFormat("<TD class='doNotTranslate' align='center'>{0}</TD></TR>", itemViewModel.Wilks);
        }

        private string getTry(ChampionshipTryDTO try1)
        {
            if (try1.Result==ChampionshipTryResult.NotDone)
            {
                return "-";
            }
            var temp = ChampionshipExerciseViewModel.GetDisplayWeight(try1.Weight, Gender.NotSet);
            if(try1.Result==ChampionshipTryResult.Fail)
            {
                temp = "-" + temp;
            }
            return temp;
        }


        protected override void BuildInfo(StringBuilder builder)
        {

        }

        protected override void BuildDescription(StringBuilder builder)
        {
            addGroupTable(builder, "Comment", delegate(StringBuilder stringBuilder)
            {
                stringBuilder.AppendFormat(
                    "<tr><td  colspan='2'>{0}</td></tr>",
                    championship.Comment);
            });
        }
    }
}
