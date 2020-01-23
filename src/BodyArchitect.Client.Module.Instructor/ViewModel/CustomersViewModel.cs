using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.Instructor.Controls;
using BodyArchitect.Client.Module.Instructor.Controls.Customers;
using BodyArchitect.Client.Module.Instructor.Controls.Finances;
using BodyArchitect.Client.Module.Suplements;
using BodyArchitect.Client.Module.Suplements.Controls;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Cache;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.SchedulerEngine;
using BodyArchitect.Client.UI.Views;
using BodyArchitect.Client.UI.Views.Calendar;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor.ViewModel
{
    public class CustomersViewModel : ViewModelBase, IWeakEventListener
    {
        private IBAWindow parentView;
        
        private bool canEdit;
        private bool canDelete;
        private bool canNew;
        private CustomerDTO _selectedCustomer;
        private bool canShowCalendar;
        private bool isInProgress;
        ObservableCollection<CustomerDTO> customers = new ObservableCollection<CustomerDTO>();

        public CustomersViewModel(IBAWindow parentView)
        {
            this.parentView = parentView;
            CollectionChangedEventManager.AddListener(CustomersReposidory.Instance, this);
            
        }

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

        public bool CanShowCalendar
        {
            get { return canShowCalendar; }
            set
            {
                canShowCalendar = value;
                NotifyOfPropertyChange(() => CanShowCalendar);
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

        public IBAWindow ParentWindow
        {
            get { return parentView; }
        }

        public ICollection<CustomerDTO> Items
        {
            //get { return customers; }
            //private set
            //{
            //    customers = value;
            //    NotifyOfPropertyChange(() => Items);
            //}
            get { return customers; }
        }

        public CustomerDTO SelectedItem
        {
            get { return _selectedCustomer; }
            set
            {
                _selectedCustomer = value;
                updateButtons();
                NotifyOfPropertyChange(() => SelectedItem);
            }
        }

        public void Fill()
        {
            IsInProgress = true;
            updateButtons();
            ParentWindow.RunAsynchronousOperation(delegate(OperationContext context)
            {
                CustomersReposidory.Instance.EnsureLoaded();
                ParentWindow.SynchronizationContext.Send(delegate
                {
                    Items.Clear();
                    foreach (var activityDto in CustomersReposidory.Instance.Items.Values)
                    {
                        Items.Add(activityDto);
                    }
                    IsInProgress = false;
                }, null);
                IsInProgress = false;
                //var pagedResult = ServiceManager.GetCustomers(new CustomerSearchCriteria());
                //ParentWindow.SynchronizationContext.Send(delegate
                //{
                //    Items = new ObservableCollection<CustomerDTO>(pagedResult.Items);
                //}, null);
                
            });
        }

        void updateButtons()
        {
            CanNew = true;
            CanEdit = SelectedItem != null;
            CanDelete = SelectedItem != null;
            CanShowCalendar = SelectedItem != null;
        }

        public void NewCustomer()
        {
            CreateNewCustomer();
            NotifyOfPropertyChange(() => Items);
        }

        public static CustomerDTO CreateNewCustomer(UserSearchDTO connectedAccount=null)
        {
            var dlg = new EditDomainObjectWindow();

            var ctrl = new usrCustomerDetails();

            dlg.SetControl(ctrl);
            var customer = new CustomerDTO();
            customer.Settings= new CustomerSettingsDTO();
            customer.ConnectedAccount = connectedAccount;
            ctrl.SelectedCustomer=customer;
            if (dlg.ShowDialog() == true)
            {
                //FillActivities();//TODO:maybe we should change this
                CustomersReposidory.Instance.Add(ctrl.Customer);
                //refresh reminders only if this customer has it
                if (ctrl.Customer.RemindBefore!=null)
                {
                    ReminderItemsReposidory.Instance.ClearCache();
                }
                return ctrl.Customer;
            }
            return null;
        }

        public void DeleteSelectedActivity()
        {
            if (!UIHelper.EnsureInstructorLicence())
            {
                return;
            }
            if (SelectedItem != null && BAMessageBox.AskYesNo("CustomersViewModel_DeleteSelectedActivity_DeleteCustomerQuestion".TranslateInstructor(), SelectedItem.FullName) == MessageBoxResult.Yes)
            {
                PleaseWait.Run(delegate(MethodParameters param)
                {
                    try
                    {
                        ServiceManager.DeleteCustomer(SelectedItem);
                        CustomersReposidory.Instance.Remove(SelectedItem.GlobalId);
                        //refresh reminders only if this customer had it
                        if (SelectedItem.RemindBefore!=null)
                        {
                            ReminderItemsReposidory.Instance.ClearCache();
                        }
                        parentView.SynchronizationContext.Send((x) => NotifyOfPropertyChange(() => Items), null);
                    }
                    catch (Exception ex)
                    {
                        param.CloseProgressWindow();
                        parentView.SynchronizationContext.Send((x) => ExceptionHandler.Default.Process(ex, "ErrDeleteCustomer".TranslateInstructor(), ErrorWindow.MessageBox), null);

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

            var ctrl = new usrCustomerDetails();

            dlg.SetControl(ctrl);
            ctrl.SelectedCustomer = SelectedItem.Clone();
            if (dlg.ShowDialog() == true)
            {
                replace(SelectedItem, ctrl.Customer);
                //refresh reminders
                ReminderItemsReposidory.Instance.ClearCache();
            }
        }

        private void replace(CustomerDTO selected, CustomerDTO saved)
        {
            CustomersReposidory.Instance.Replace(selected,saved);
            SelectedItem = saved;
            NotifyOfPropertyChange(() => Items);
        }

        public void ShowCalendar()
        {
            if(!UIHelper.EnsureInstructorLicence())
            {
                return;
            }
            MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.UI;component/Views/Calendar/NewCalendarView.xaml?CustomerId=" + SelectedItem.GlobalId), () => new CalendarPageContext(null, SelectedItem));
        }

        public void ShowReports()
        {
            if (!UIHelper.EnsureInstructorLicence())
            {
                return;
            }
            MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.UI;component/Views/NewReportingView.xaml?CustomerId=" + SelectedItem.GlobalId), () => new PageContext(null, SelectedItem));
        }

        public void StartSupplementsCycle()
        {
            if (!UIHelper.EnsureInstructorLicence())
            {
                return;
            }
            var dlg = new StartSupplementsCycleWindow();
            dlg.AllowChangePlan = true;
            dlg.Customer = SelectedItem;
            if (dlg.ShowDialog() == true)
            {
                BAMessageBox.ShowInfo("Info_usrSupplementsCyclesView_rbtnStartCycle_Click_StartedCycle".TranslateSupple());

            }
        }

        public void ShowProducts()
        {
            if (!UIHelper.EnsureInstructorLicence())
            {
                return;
            }

            MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Controls/Finances/ProductsView.xaml?CustomerId=" + SelectedItem.GlobalId), () => new PageContext(null, SelectedItem));
        }

        
        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            parentView.SynchronizationContext.Send(state => Fill(), null);

            return true;
        }

        public void ShowMyTrainings()
        {
            if (!UIHelper.EnsureInstructorLicence())
            {
                return;
            }

            MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.UI;component/Views/MyTrainingsView.xaml?CustomerId=" + SelectedItem.GlobalId), () => new PageContext(null, SelectedItem));
        }
    }
}
