using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

//using Microsoft.Windows.Design;

namespace BodyArchitect.Client.UI.Controls.Calendar
{
    [TemplatePart(Name = CalendarDayHeader.ElementDayHeaderLabel, Type = typeof(TextBlock))]
    //[ToolboxBrowsable(false)]
    public sealed class CalendarDayHeader : Control
    {
        private const string ElementDayHeaderLabel = "PART_DayHeaderLabel";

        static CalendarDayHeader()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarDayHeader), new FrameworkPropertyMetadata(typeof(CalendarDayHeader)));
        }

        public Calendar Owner { get { return UIHelper.FindVisualParent<Calendar>(this); } }

        private BindingBase GetOwnerBinding(string propertyName)
        {
            Binding result = new Binding(propertyName);
            result.Source = this.Owner;
            return result;
        }

        //TextBlock _dayHeaderLabel;
        private DateTime currentDate;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //_dayHeaderLabel = GetTemplateChild(ElementDayHeaderLabel) as TextBlock;

            PopulateHeader();
        }

        public DateTime CurrentDate
        {
            get { return currentDate; }
            set
            {
                currentDate = value;
                PopulateHeader();
            }
        }
        void PopulateHeader()
        {

            var text = GetTemplateChild("PART_DayHeaderLabel") as TextBlock;
            var border = GetTemplateChild("headerBorder") as Border;
            if(text==null)
            {
                return;
            }
            text.Text = string.Format("{0:d}", CurrentDate);
            if(CurrentDate.Date==DateTime.Now.Date)
            {
                border.SetBinding(Calendar.BackgroundProperty, GetOwnerBinding("TodayHeaderBackground"));
            }
            else
            {
                border.SetBinding(Calendar.BackgroundProperty, GetOwnerBinding("HeaderBackground"));
            }
        }

    }
}
