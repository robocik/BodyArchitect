using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Module.StrengthTraining.Model.TrainingPlans;
using BodyArchitect.Service.Model.TrainingPlans;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using BodyArchitect.Controls.UserControls;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    public class TrainingPlanDaysPropertyEditor : PropertyEditorBase
    {
        private CheckedListBoxControl withEventsField_myListBox = new CheckedListBoxControl();

        private CheckedListBoxControl myListBox
        {
            get { return withEventsField_myListBox; }
        }

        protected override Control GetEditControl(string PropertyName, object CurrentValue, object instance)
        {
            TrainingPlanHtmlExporter exporter = (TrainingPlanHtmlExporter)instance;

            string[] days = new string[0];
            if (CurrentValue != null)
            {
                days = CurrentValue.ToString().Split(TrainingPlanHtmlExporter.DaysSeparator);
            }
            myListBox.BorderStyle = BorderStyles.NoBorder;
            myListBox.HotTrackItems = true;
            myListBox.HotTrackSelectMode = HotTrackSelectMode.SelectItemOnClick;
            //Creating ListBox items... 
            //Note that as this is executed in design mode, performance is not important and there is no need to cache these items if they can change each time.
            myListBox.Items.Clear();

            foreach (var trainingPlanDay in exporter.TrainingPlan.Days)
            {
                CheckedListBoxItem item = new CheckedListBoxItem(trainingPlanDay);
                item.CheckState = Array.IndexOf(days, trainingPlanDay.GlobalId.ToString()) > -1 ? CheckState.Checked : CheckState.Unchecked;
                item.Description = trainingPlanDay.Name;

                myListBox.Items.Add(item);
            }

            //myListBox.SelectedIndex = myListBox.FindString(CurrentValue.ToString());
            //Select current item based on CurrentValue of the property
            //myListBox.Height = myListBox.PreferredHeight;
            return myListBox;
        }

        protected override object GetEditedValue(Control EditControl, string PropertyName, object OldValue)
        {
            StringBuilder builder = new StringBuilder();

            foreach (CheckedListBoxItem item in myListBox.Items)
            {
                if (item.CheckState == CheckState.Checked)
                {
                    builder.AppendFormat("{0}{1}", ((TrainingPlanDay)item.Value).GlobalId, TrainingPlanHtmlExporter.DaysSeparator);
                }
            }
            return builder.ToString().TrimEnd(TrainingPlanHtmlExporter.DaysSeparator);//.Substring(0, 2);
            //return new value for property
        }
    }
}
