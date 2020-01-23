using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Cache;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor.Controls.Customers
{
    public class CustomersFilteredComboBox : CachedFilteredComboBox<CustomerDTO>
    {
        public CustomersFilteredComboBox()
        {
            SelectedValuePath = "GlobalId";
        }

        protected override ObjectsCacheBase<CustomerDTO> GetCache()
        {
            return CustomersReposidory.Instance;
        }

        protected override void EnsureSorting()
        {
            Items.SortDescriptions.Clear();
            SortDescription sd = new SortDescription("FullName", ListSortDirection.Ascending);
            Items.SortDescriptions.Add(sd);
        }

        protected override bool FilterImplementation(object value)
        {
            string text = this.Text.ToLower();
            var customer = (CustomerDTO)value;
            return customer.FirstName.SafeStartsWith(text) ||
            customer.LastName.SafeStartsWith(text) ||
            customer.FullName.SafeStartsWith(text) ||
            customer.Email.SafeStartsWith(text);
        }

    }
}
