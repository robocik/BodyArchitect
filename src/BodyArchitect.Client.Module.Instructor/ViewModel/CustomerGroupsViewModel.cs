using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Windows;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.Instructor.Controls;
using BodyArchitect.Client.Module.Instructor.Controls.Groups;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Cache;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Shared;

namespace BodyArchitect.Client.Module.Instructor.ViewModel
{
    class CustomerGroupsViewModel : ViewModelBase, IWeakEventListener
    {
        private IBAWindow parentView;

        private bool canEdit;
        private bool canDelete;
        private bool canNew;
        private CustomerGroupDTO _selectedItem;
        private IEnumerable<CustomerGroupDTO> groups;
        ObservableCollection<CustomerDTO> selectedGroupCustomers = new ObservableCollection<CustomerDTO>();
        private CustomerDTO _selectedCustomer;
        private bool isInProgress;

        public CustomerGroupsViewModel(IBAWindow parentView)
        {
            this.parentView = parentView;
            updateButtons();
            CollectionChangedEventManager.AddListener(CustomerGroupsReposidory.Instance, this);
        }

        #region Properties

        public bool IsInProgress
        {
            get { return isInProgress; }
            set
            {
                isInProgress = value;
                NotifyOfPropertyChange(() => IsInProgress);
            }
        }

        public bool CanNew
        {
            get { return canNew; }
            set
            {
                canNew = value;
                NotifyOfPropertyChange(() => CanNew);
            }
        }

        public bool CanEdit
        {
            get { return canEdit; }
            set
            {
                canEdit = value;
                NotifyOfPropertyChange(() => CanEdit);
            }
        }

        public bool CanDelete
        {
            get { return canDelete; }
            set
            {
                canDelete = value;
                NotifyOfPropertyChange(() => CanDelete);
            }
        }

        public ObservableCollection<CustomerDTO> SelectedGroupCustomers
        {
            get { return selectedGroupCustomers; }
        }

        public IBAWindow ParentWindow
        {
            get { return parentView; }
        }

        public IEnumerable<CustomerGroupDTO> Items
        {
            get { return groups; }
            set
            {
                groups = value;
                NotifyOfPropertyChange(()=>Items);
            }
        }

        public CustomerGroupDTO SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                fillGroupCustomers();
                updateButtons();
                NotifyOfPropertyChange(() => SelectedItem);
            }
        }

        public CustomerDTO SelectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {
                _selectedCustomer = value;
                NotifyOfPropertyChange(() => SelectedCustomer);
            }
        }
        #endregion

        void fillGroupCustomers()
        {
            selectedGroupCustomers.Clear();
            if(SelectedItem==null)
            {
                return;
            }
            foreach (var customer in SelectedItem.Customers)
            {
                //the base customer list is in customersreposidory. customers from group are inly for information purposes (as a list of customer id and nothing more)
                var customerFromRep=CustomersReposidory.Instance.GetItem(customer.GlobalId);
                if (customerFromRep != null)
                {
                    selectedGroupCustomers.Add(customerFromRep);
                }
#if DEBUG
                else
                {
                    //TODO:Needs to be implemented
                    throw new NotImplementedException("Co zrobić?");
                }
#endif
            }
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            parentView.SynchronizationContext.Send(state => Fill(), null);

            return true;
        }

        public void Fill()
        {
            IsInProgress = true;
            ParentWindow.RunAsynchronousOperation(delegate
            {
                CustomerGroupsReposidory.Instance.EnsureLoaded();
                ParentWindow.SynchronizationContext.Send(delegate
                                                             {
                                                                 Items = CustomerGroupsReposidory.Instance.Items.Values;
                                                                 IsInProgress = false;
                                                             }, null);
            });
        }

        void updateButtons()
        {
            CanNew = true;
            CanEdit = SelectedItem != null;
            CanDelete = SelectedItem != null;
        }

        public void NewGroup()
        {
            if(!UIHelper.EnsureInstructorLicence())
            {
                return;
            }
            var dlg = new EditDomainObjectWindow();

            var ctrl = new usrCustomerGroupDetails();

            dlg.SetControl(ctrl);
            var customer = new CustomerGroupDTO();
            customer.Color = Color.LightGoldenrodYellow.ToColorString();
            ctrl.Fill(customer);
            if (dlg.ShowDialog() == true)
            {
                CustomerGroupsReposidory.Instance.Add(ctrl.CustomerGroup);
                NotifyOfPropertyChange(() => Items);
            }
            
        }



        public void DeleteSelectedGroup()
        {
            if (SelectedItem != null && UIHelper.EnsureInstructorLicence() && BAMessageBox.AskYesNo("CustomerGroupsViewModel_DeleteSelectedGroup, SelectedItem.Name".TranslateInstructor()) == MessageBoxResult.Yes)
            {
                PleaseWait.Run(delegate(MethodParameters param)
                {
                    try
                    {
                        ServiceManager.DeleteCustomerGroup(SelectedItem);
                        CustomerGroupsReposidory.Instance.Remove(SelectedItem.GlobalId);
                        parentView.SynchronizationContext.Send((x) => NotifyOfPropertyChange(() => Items), null);
                    }
                    catch (Exception ex)
                    {
                        param.CloseProgressWindow();
                        parentView.SynchronizationContext.Send((x) => ExceptionHandler.Default.Process(ex, "Exception_CustomerGroupsViewModel_DeleteSelectedGroup_CannotRemove".TranslateInstructor(), ErrorWindow.EMailReport), null);

                    }
                });
            }
        }

        public void EditSelectedItem()
        {
            if (!UIHelper.EnsureInstructorLicence())
            {
                return;
            }
            var dlg = new EditDomainObjectWindow();

            var ctrl = new usrCustomerGroupDetails();

            dlg.SetControl(ctrl);
            ctrl.Fill(SelectedItem.Clone());
            if (dlg.ShowDialog() == true)
            {
                replace(SelectedItem, ctrl.CustomerGroup);
            }
        }

        private void replace(CustomerGroupDTO selected, CustomerGroupDTO saved)
        {
            CustomerGroupsReposidory.Instance.Replace(selected, saved);
            SelectedItem = saved;
            NotifyOfPropertyChange(() => Items);
        }
    }
}
