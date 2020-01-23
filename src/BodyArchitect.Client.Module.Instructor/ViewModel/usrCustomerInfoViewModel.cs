using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI.Cache;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor.ViewModel
{
    public class usrCustomerInfoViewModel:ViewModelBase
    {
        private IBAWindow parentWindow;
        private CustomerDTO _selectedCustomer;
        private ProfileInformationDTO currentUser;
        private bool _editMode;
        private bool _canSave;
        private bool _canDisconnect;

        public usrCustomerInfoViewModel(IBAWindow parentWindow )
        {
            this.parentWindow = parentWindow;
            
            
        }

        public void Fill(ProfileInformationDTO currentUser,bool isActive)
        {
            EditMode = false;
            this.currentUser = currentUser;
            if (currentUser != null && isActive)
            {
                var customer = currentUser.User.GetConnectedCustomer();
                SelectedCustomer = customer;    
            }
            CanDisconnect = SelectedCustomer != null;
        }

        public CustomerDTO SelectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {
                _selectedCustomer = value;
                CanSave = value != null;
                NotifyOfPropertyChange(() => SelectedCustomer);
                NotifyOfPropertyChange(() => ShowNotConnected);
                NotifyOfPropertyChange(() => ShowConnected);
            }
        }

        public bool EditMode
        {
            get {
                return _editMode;
            }
            set
            {
                _editMode = value;
                if (!_editMode && currentUser!=null)
                {//mostly needed in Cancel operation
                    SelectedCustomer = currentUser.User.GetConnectedCustomer();
                }
                NotifyOfPropertyChange(()=>EditMode);
                NotifyOfPropertyChange(() => ShowNotConnected);
                NotifyOfPropertyChange(() => ShowConnected);
            }
        }

        public bool CanSave
        {
            get { return _canSave; }
            set 
            {
                _canSave = value;
                 NotifyOfPropertyChange(()=>CanSave);
            }
        }

        public bool CanDisconnect
        {
            get { return _canDisconnect; }
            set
            {
                _canDisconnect = value;
                NotifyOfPropertyChange(() => CanDisconnect);
            }
        }

        public bool ShowNotConnected
        {
            get { return !EditMode && _selectedCustomer == null; }
        }

        public bool ShowConnected
        {
            get { return !EditMode && _selectedCustomer != null; }
        }

        public ProfileInformationDTO CurrentUser
        {
            get { return currentUser; }
        }

        public void SaveCustomer()
        {
            if(SelectedCustomer==null)
            {
                return;
            }
            CanSave = false;
            var customerToSave = SelectedCustomer;
            try
            {
                customerToSave.ConnectedAccount = CurrentUser.User;
                customerToSave = ServiceManager.SaveCustomer(customerToSave);
                CustomersReposidory.Instance.Update(customerToSave);
                SelectedCustomer = customerToSave;
                //parentWindow.SynchronizationContext.Send(state => EditMode = false, null);
                EditMode = false;
            }
            finally
            {
                CanSave = true;
            }
        }

        public void NewCustomer()
        {
            var customer=CustomersViewModel.CreateNewCustomer(CurrentUser.User);
            if(customer!=null)
            {
                SelectedCustomer = customer;
            }
        }

        public void Disconnect()
        {
            CanDisconnect = false;
            var customerToSave = SelectedCustomer.StandardClone();
            parentWindow.RunAsynchronousOperation(delegate
                                                      {
                                                          try
                                                          {
                                                              customerToSave.ConnectedAccount = null;
                                                              customerToSave = ServiceManager.SaveCustomer(customerToSave);
                                                              CustomersReposidory.Instance.Update(customerToSave);
                                                              SelectedCustomer = customerToSave;
                                                              parentWindow.SynchronizationContext.Send(state => EditMode = false, null);
                                                          }
                                                          finally
                                                          {
                                                              CanDisconnect = true;
                                                          }
                
            });
        }
    }
}
