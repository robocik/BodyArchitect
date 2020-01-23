using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using BodyArchitect.Client.Common;

namespace BodyArchitect.Client.UI.SchedulerEngine
{
    public class PopupNotificationTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ReminderTemplate { get; set; }
        public DataTemplate RecordTemplate { get; set; }

        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            if(item is NotifyObject)
            {
                return ReminderTemplate;
            }
            return RecordTemplate;
        }
    }
}
