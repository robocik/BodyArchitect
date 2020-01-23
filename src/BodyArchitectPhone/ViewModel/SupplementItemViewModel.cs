using System;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;

namespace BodyArchitect.WP7.ViewModel
{
    public class SupplementItemViewModel
    {
        private SuplementItemDTO item;

        public SupplementItemViewModel(SuplementItemDTO item)
        {
            this.item = item;
        }

        public bool EditMode
        {
            get { return ApplicationState.Current.TrainingDay.TrainingDay.IsMine; }
        }

        public SuplementItemDTO Item
        {
            get { return item; }
        }

        public string Time
        {
            get
            {
                return Item.Time.DateTime.ToShortTimeString();
            }
        }
        
        
        public string Name
        {
            get
            {
                string name = string.Empty;
                
                if (item != null && item.Suplement != null)
                {
                    name= item.Suplement.Name;
                }
                if(item != null && !string.IsNullOrEmpty(item.Name))
                {
                    name += ": " + item.Name;
                }
                return name;
            }
        }

        public string Dosage
        {
            get
            {
                
                return item.Dosage.ToString("0.##");
            }
        }



        public string DosageType
        {
            get { return EnumLocalizer.Default.Translate(item.DosageType); }
        }

        public string TimeType
        {
            get { return EnumLocalizer.Default.Translate(item.Time.TimeType); }
        }
    }
}
