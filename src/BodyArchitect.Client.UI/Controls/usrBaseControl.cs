using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.UI.Controls
{
    [Serializable]
    public class PageContext
    {
        private UserDTO user;
        private CustomerDTO _customer;

        public PageContext(UserDTO user, CustomerDTO customer)
        {
            this.user = user;
            _customer = customer;
        }

        public PageContext()
        {
        }

        public int DisplayMode { get; set; }

        public UserDTO User
        {
            get { return user; }
            set { user = value; }
        }

        public CustomerDTO Customer
        {
            get { return _customer; }
            set { _customer = value; }
        }

        public Guid? SelectedItem { get; set; }

    }
    public abstract class BasePage:usrBaseControl,IControlView
    {

        public static readonly DependencyProperty PageContextProperty =
            DependencyProperty.Register("PageContext", typeof(PageContext), typeof(BasePage), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Journal, OnPageContextChanged));

        public PageContext PageContext
        {
            get { return (PageContext)GetValue(PageContextProperty); }
            set
            {
                SetValue(PageContextProperty, value);
            }
        }


        private static void OnPageContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (BasePage)d;

            ctrl.NotifyOfPropertyChange(()=>ctrl.Header);
            ctrl.NotifyOfPropertyChange(() => ctrl.HeaderIcon);
        }


        public abstract void Fill();
        public abstract void RefreshView();

        public UserDTO User
        {
            get
            {
                UserDTO user = UserContext.Current.CurrentProfile;
                if (PageContext != null && PageContext.User!=null)
                {
                    user = PageContext.User;
                }
                return user;
            }
        }

        public CustomerDTO Customer
        {
            get
            {
                if (PageContext != null)
                {
                    return PageContext.Customer;
                }
                return null;
            }
        }


        public abstract Uri HeaderIcon { get; }

        public virtual string HeaderToolTip
        {
            get { return null; }
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(BasePage), new PropertyMetadata(null));

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set
            {
                SetValue(HeaderProperty, value);
            }
        }

        public static readonly DependencyProperty IsModifiedProperty =
            DependencyProperty.Register("IsModified", typeof(bool), typeof(BasePage), new PropertyMetadata(false));

        public bool IsModified
        {
            get { return (bool)GetValue(IsModifiedProperty); }
            set
            {
                SetValue(IsModifiedProperty, value);
            }
        }

        public static readonly DependencyProperty IsInProgressProperty =
            DependencyProperty.Register("IsInProgress", typeof(bool), typeof(BasePage), new PropertyMetadata(false));

        public bool IsInProgress
        {
            get { return (bool)GetValue(IsInProgressProperty); }
            set
            {
                SetValue(IsInProgressProperty, value);
            }
        }

        public virtual AccountType AccountType
        {
            get { return AccountType.User; }
        }

        public virtual void ActiveThisItem()
        {
            
        }
    }

    public class usrBaseControl : UserControl, INotifyPropertyChanged
    {
        public event EventHandler<ControlValidatedEventArgs> ControlValidated;

        public usrBaseControl()
        {
            this.Unloaded += new System.Windows.RoutedEventHandler(usrBaseControl_Unloaded);
            UserContext.Current.LoginStatusChanged += new EventHandler<LoginStatusEventArgs>(UserContext_LoginStatusChanged);
            this.AddHandler(Validation.ErrorEvent, new RoutedEventHandler(OnErrorEvent));
        }

        #region Validation

        private int errorCount;
        private void OnErrorEvent(object sender, RoutedEventArgs e)
        {
            var validationEventArgs = e as ValidationErrorEventArgs;
            if (validationEventArgs == null)
                throw new Exception("Unexpected event args");
            switch (validationEventArgs.Action)
            {
                case ValidationErrorEventAction.Added:
                    {
                        errorCount++; break;
                    }
                case ValidationErrorEventAction.Removed:
                    {
                        errorCount--; break;
                    }
                default:
                    {
                        throw new Exception("Unknown action");
                    }
            }
            OnControlValidated(validateInternal());
        }

        protected void OnControlValidated(bool isValid)
        {
            if (ControlValidated != null)
            {
                ControlValidated(this, new ControlValidatedEventArgs(isValid));
            }
        }

        bool validateInternal()
        {
            return errorCount == 0 && ValidateData();
        }


        public virtual bool ValidateData()
        {
            return true;
        }

        #endregion

        public virtual void NotifyOfPropertyChange<TProperty>(Expression<Func<TProperty>> property)
        {
            NotifyOfPropertyChange(property.GetMemberInfo().Name);
        }

        public virtual void NotifyOfPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        void usrBaseControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            UserContext.Current.LoginStatusChanged -= new EventHandler<LoginStatusEventArgs>(UserContext_LoginStatusChanged);
        }

        

        [Browsable(false)]
        public IBAWindow ParentWindow
        {
            get
            {
                var mainWnd = UIHelper.FindVisualParent<IBAWindow>(this);
                if(mainWnd==null)
                {
                    mainWnd=MainWindow.Instance;
                }
                return mainWnd;
            }
        }

        protected virtual void LoginStatusChanged(LoginStatus newStatus)
        {

        }

        void UserContext_LoginStatusChanged(object sender, LoginStatusEventArgs e)
        {
            //if (ParentWindow == null)
            //{
            //    return;
            //}
            if (!CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action<LoginStatus>(LoginStatusChanged), e.Status);
            }
            else
            {
                LoginStatusChanged(e.Status);
            }
        }
    }
}
