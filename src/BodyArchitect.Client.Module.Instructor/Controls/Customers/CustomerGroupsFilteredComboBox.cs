using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor.Controls.Customers
{
    public class CustomerGroupsFilteredComboBox : CachedFilteredComboBox<CustomerGroupDTO>
    {
        public CustomerGroupsFilteredComboBox()
        {
            TextSearch.SetTextPath(this, "Name");
            SelectedValuePath = "GlobalId";
            ItemTemplate = (DataTemplate)Application.Current.Resources["GroupsComboBoxItem"];
            ItemContainerStyle = (Style)Application.Current.Resources["StretchedComboBoxItemStyle"];
        }

        protected override ObjectsCacheBase<CustomerGroupDTO> GetCache()
        {
            return CustomerGroupsReposidory.Instance;
        }

        protected override bool FilterImplementation(object value)
        {
            string text = this.Text.ToLower();
            var group = (CustomerGroupDTO)value;
            return group.Name.SafeStartsWith(text);
        }

    }
}
