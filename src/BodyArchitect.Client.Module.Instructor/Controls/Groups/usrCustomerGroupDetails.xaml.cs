using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.Instructor.ViewModel;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Shared;

namespace BodyArchitect.Client.Module.Instructor.Controls.Groups
{
    /// <summary>
    /// Interaction logic for usrCustomerGroupDetails.xaml
    /// </summary>
    public partial class usrCustomerGroupDetails : IEditableControl
    {
        private CustomerGroupDetailsViewModel group;

        public usrCustomerGroupDetails()
        {
            InitializeComponent();
        }

        public void Fill(CustomerGroupDTO customer)
        {
            Object =customer;
            
        }

        public CustomerGroupDTO CustomerGroup
        {
            get { return group.Group; }
        }
        #region Implementation of IEditableControl

        public object Object
        {
            get { return group; }
            set
            {
                group =new CustomerGroupDetailsViewModel(this.ParentWindow,(CustomerGroupDTO)value);
                DataContext = group;
            }
        }

        public bool ReadOnly
        {
            get;
            set;
        }

        public object Save()
        {
            try
            {
                group.Apply();
                return ServiceManager.SaveCustomerGroup(CustomerGroup);
            }
            catch (AlreadyOccupiedException ex)
            {
                Dispatcher.Invoke(new Action(() => ExceptionHandler.Default.Process(ex, "AlreadyOccupiedException_usrCustomerGroupDetails_Save".TranslateInstructor(), ErrorWindow.MessageBox)));
            }
            return null;
        }
        #endregion


    }

    
}
