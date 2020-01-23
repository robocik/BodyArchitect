using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor.Controls
{
    public class ActivitiesFilteredComboBox: CachedFilteredComboBox<ActivityDTO>
    {
        public ActivitiesFilteredComboBox()
        {
            //TextSearch.TextPath="Name"
            TextSearch.SetTextPath(this, "Name");
            SelectedValuePath = "GlobalId";
            ItemTemplate = (DataTemplate)Application.Current. Resources["ActivitiesComboBoxItem"];
            ItemContainerStyle = (Style)Application.Current.Resources["StretchedComboBoxItemStyle"];
        }

        protected override ObjectsCacheBase<ActivityDTO> GetCache()
        {
            return ActivitiesReposidory.Instance;
        }

        protected override bool FilterImplementation(object value)
        {
            string text = this.Text.ToLower();
            var group = (ActivityDTO)value;
            return group.Name.SafeStartsWith(text);
        }

    
    }
}
