using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Cache;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.SchedulerEngine;
using BodyArchitect.Client.UI.UserControls;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor.Controls.Customers
{
    /// <summary>
    /// Interaction logic for usrCustomerDetails.xaml
    /// </summary>
    public partial class usrCustomerDetails : IEditableControl
    {
        private CustomerDTO customer;

        public usrCustomerDetails()
        {
            InitializeComponent();
        }

        private void Fill(CustomerDTO customer)
        {
            Object = customer;
            if (customer != null)
            {
                chkAutomaticUpdateMeasurements.IsChecked = customer.Settings.AutomaticUpdateMeasurements;
                usrWymiaryEditor1.Fill(customer.Wymiary);
                if(customer.Address==null)
                {
                    customer.Address=new AddressDTO();
                }
            }
            baPictureEdit1.Clear();
            fillImage();
            NotifyOfPropertyChange(null);
        }

        public bool BirthdayReminderEnabled
        {
            get { return !ReadOnly && Customer != null && Customer.Birthday.HasValue; }
        }

        public bool CanChangeVirtual
        {
            get { return !ReadOnly && customer!=null && customer.IsNew; }
        }

        public bool IsMale
        {
            get
            {
                if (customer != null)
                {
                    return customer.Gender == Gender.Male;
                }
                return false;
            }
            set
            {
                if (value)
                {
                    customer.Gender = Gender.Male;
                    NotifyOfPropertyChange(() => IsMale);
                }
            }
        }



        public bool IsFemale
        {
            get
            {
                if (customer != null)
                {
                    return customer.Gender == Gender.Female;
                }
                return false;
            }
            set
            {
                if (value)
                {
                    customer.Gender = Gender.Female;
                    NotifyOfPropertyChange(() => IsFemale);
                }
            }
        }

        public CustomerDTO SelectedCustomer
        {
            get { return (CustomerDTO)GetValue(SelectedCustomerProperty); }
            set
            {
                SetValue(SelectedCustomerProperty, value);
            }
        }


        public static readonly DependencyProperty SelectedCustomerProperty =
            DependencyProperty.Register("SelectedCustomer", typeof(CustomerDTO), typeof(usrCustomerDetails), new UIPropertyMetadata(null, OnSelectedCustomerChanged));

        private static void OnSelectedCustomerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (usrCustomerDetails)d;

            ctrl.Fill((CustomerDTO) e.NewValue);
        }

        public CustomerDTO Customer
        {
            get { return customer; }
        }
        #region Implementation of IEditableControl

        public object Object
        {
            get { return customer; }
            set
            {
                customer = (CustomerDTO)value;
                DataContext = null;
                DataContext = value;
            }
        }

        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set
            {
                SetValue(ReadOnlyProperty, value);
            }
        }


        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register("ReadOnly", typeof(bool), typeof(usrCustomerDetails), new UIPropertyMetadata(false));

        public object Save()
        {
            PictureDTO picture = null;
            Dispatcher.Invoke(new Action(delegate
                                             {
                                                 picture = baPictureEdit1.Save(customer);
                                                 customer.Settings.AutomaticUpdateMeasurements = chkAutomaticUpdateMeasurements.IsChecked.Value;
                                                 customer.Wymiary=usrWymiaryEditor1.SaveWymiary(customer.Wymiary);
                                             }));

            if (picture != null && (customer.Picture == null || ForceUploadImage || picture.Hash != customer.Picture.Hash))
            {
                var info = ServiceManager.UploadImage(picture);
                picture.PictureId = info.PictureId;
                customer.Picture = info;
                PicturesCache.Instance.AddToCache(picture.ToPictureCacheItem());
            }

            var newCustomer= ServiceManager.SaveCustomer(customer);
            //TODO:Add optimalization not to invoke clear cache if reminder hasn't changed
            ReminderItemsReposidory.Instance.ClearCache();
            return newCustomer;
        }

        #endregion

        private void dteDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Customer!=null && Customer.Birthday == null)
            {
                Customer.RemindBefore = null;
            }
            NotifyOfPropertyChange(() => BirthdayReminderEnabled);
            NotifyOfPropertyChange(() => Customer);
        }

        private void chkAutomaticUpdateMeasurements_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            usrWymiaryEditor1.ReadOnly = chkAutomaticUpdateMeasurements.IsChecked.Value;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            fillImage();
        }

        void fillImage()
        {
            if (tcMain.SelectedItem == pictureTab)
            {
                var editWindow = UIHelper.FindVisualParent<EditDomainObjectWindow>(this);
                ParentWindow.RunAsynchronousOperation(delegate
                {
                    baPictureEdit1.Fill(customer);

                }, editWindow != null ? editWindow.UpdateProgressIndicator : (Action<OperationContext>)null);
            }
        }

        public bool ForceUploadImage
        {
            get { return baPictureEdit1.ErrorOccured; }
        }
    }
}
