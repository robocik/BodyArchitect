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
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.SchedulerEngine;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Settings;
using BodyArchitect.Shared;

namespace BodyArchitect.Client.UI.Views
{
    /// <summary>
    /// Interaction logic for ChangeAccountTypeView.xaml
    /// </summary>
    public partial class ChangeAccountTypeView:IWeakEventListener
    {
        private bool _canFreeUser;
        private bool _canPremiumUser;
        private bool _canInstructor;
        private List<ListItem<string>> freeFeatures;
        private List<ListItem<string>> premiumFeatures;
        private List<ListItem<string>> instructorFeatures;

        public ChangeAccountTypeView()
        {
            InitializeComponent();

            fillFreeAccountFeatures();
            fillPremiumAccountFeatures();
            fillInstructorAccountFeatures();

            DataContext = this;
            PropertyChangedEventManager.AddListener(UserContext.Current,this,string.Empty);
        }

        void fillFreeAccountFeatures()
        {
            
            freeFeatures = new List<ListItem<string>>();
            FreeFeatures.Add(new ListItem<string>(EnumLocalizer.Default.GetGUIString("Feature_FreeAccount_UnlimitedCalendar"), null));
            FreeFeatures.Add(new ListItem<string>(EnumLocalizer.Default.GetGUIString("Feature_FreeAccount_LargeExercisesDb"), null));
            FreeFeatures.Add(new ListItem<string>(EnumLocalizer.Default.GetGUIString("Feature_FreeAccount_AccessToWorkoutPlans"), null));
            FreeFeatures.Add(new ListItem<string>(EnumLocalizer.Default.GetGUIString("Feature_FreeAccount_Measurements"), null));
            FreeFeatures.Add(new ListItem<string>(EnumLocalizer.Default.GetGUIString("Feature_FreeAccount_A6W"), null));
            FreeFeatures.Add(new ListItem<string>(EnumLocalizer.Default.GetGUIString("Feature_FreeAccount_Supplements"), null));
            FreeFeatures.Add(new ListItem<string>(EnumLocalizer.Default.GetGUIString("Feature_FreeAccount_AccessToCalendardOfOtherUsers"), null));
            FreeFeatures.Add(new ListItem<string>(EnumLocalizer.Default.GetGUIString("Feature_FreeAccount_Blog"), null));
            FreeFeatures.Add(new ListItem<string>(EnumLocalizer.Default.GetGUIString("Feature_FreeAccount_Community"), null));
            FreeFeatures.Add(new ListItem<string>(EnumLocalizer.Default.GetGUIString("Feature_FreeAccount_Records"), null));
        }

        void fillPremiumAccountFeatures()
        {
            premiumFeatures = new List<ListItem<string>>();
            PremiumFeatures.Add(new ListItem<string>(EnumLocalizer.Default.GetGUIString("Feature_PremiumAccount_CreateExercises"), null));
            PremiumFeatures.Add(new ListItem<string>(EnumLocalizer.Default.GetGUIString("Feature_PremiumAccount_CreateWorkoutPlans"), null));
            PremiumFeatures.Add(new ListItem<string>(EnumLocalizer.Default.GetGUIString("Feature_PremiumAccount_AccessSupplementsCycleDefinitions"), null));
            PremiumFeatures.Add(new ListItem<string>(EnumLocalizer.Default.GetGUIString("Feature_PremiumAccount_CreateSupplementsDefinitions"), null));
            PremiumFeatures.Add(new ListItem<string>(EnumLocalizer.Default.GetGUIString("Feature_PremiumAccount_MyTrainings"), null));
            PremiumFeatures.Add(new ListItem<string>(EnumLocalizer.Default.GetGUIString("Feature_PremiumAccount_EntriesInFuture"), null));
            PremiumFeatures.Add(new ListItem<string>(EnumLocalizer.Default.GetGUIString("Feature_PremiumAccount_Reminders"), null));
            PremiumFeatures.Add(new ListItem<string>(EnumLocalizer.Default.GetGUIString("Feature_PremiumAccount_Privacy"), null));
            PremiumFeatures.Add(new ListItem<string>(EnumLocalizer.Default.GetGUIString("Feature_PremiumAccount_Printing"), null));
            PremiumFeatures.Add(new ListItem<string>(EnumLocalizer.Default.GetGUIString("Feature_PremiumAccount_MyPlaces"), null));
            PremiumFeatures.Add(new ListItem<string>(EnumLocalizer.Default.GetGUIString("Feature_PremiumAccount_DoneWay"), null));
            PremiumFeatures.Add(new ListItem<string>(EnumLocalizer.Default.GetGUIString("Feature_PremiumAccount_Timer"), null));
            PremiumFeatures.Add(new ListItem<string>(EnumLocalizer.Default.GetGUIString("Feature_PremiumAccount_Reports"), null));
            PremiumFeatures.Add(new ListItem<string>(EnumLocalizer.Default.GetGUIString("Feature_PremiumAccount_AdvancedStrengthTraining"), null));
            

            
        }

        void fillInstructorAccountFeatures()
        {
            instructorFeatures = new List<ListItem<string>>();
            instructorFeatures.Add(new ListItem<string>(EnumLocalizer.Default.GetGUIString("Feature_InstructorAccount_Customers"), null));
            instructorFeatures.Add(new ListItem<string>(EnumLocalizer.Default.GetGUIString("Feature_InstructorAccount_CustomersCalendar"), null));
            instructorFeatures.Add(new ListItem<string>(EnumLocalizer.Default.GetGUIString("Feature_InstructorAccount_SchedulePlan"), null));
            instructorFeatures.Add(new ListItem<string>(EnumLocalizer.Default.GetGUIString("Feature_InstructorAccount_Championships"), null));
        }

        public int MyPoints
        {
            get { return UserContext.Current.ProfileInformation.Licence.BAPoints; }
        }

        public string CurrentAccountType
        {
            get
            {
                return EnumLocalizer.Default.Translate(UserContext.Current.ProfileInformation.Licence.CurrentAccountType);
            }
        }

        public string BaseAccountType
        {
            get
            {
                return EnumLocalizer.Default.Translate(UserContext.Current.ProfileInformation.Licence.AccountType);
            }
        }

        public string FreeUserPointsText
        {
            get
            {
                var test = EnumLocalizer.Default.GetGUIString("ChangeAccountTypeView_PointsPerDay");
                return string.Format(test, UserContext.Current.ProfileInformation.Licence.Payments.GetPoints(AccountType.User).GetCurrentPoints(DateTime.UtcNow));
            }
        }

        public string PremiumUserPointsText
        {
            get { return string.Format(EnumLocalizer.Default.GetGUIString("ChangeAccountTypeView_PointsPerDay"), UserContext.Current.ProfileInformation.Licence.Payments.GetPoints(AccountType.PremiumUser).GetCurrentPoints(DateTime.UtcNow)); }
        }

        public string InstructorPointsText
        {
            get { return string.Format(EnumLocalizer.Default.GetGUIString("ChangeAccountTypeView_PointsPerDay"), UserContext.Current.ProfileInformation.Licence.Payments.GetPoints(AccountType.Instructor).GetCurrentPoints(DateTime.UtcNow)); }
        }

        public bool CanFreeUser
        {
            get { return _canFreeUser; }
            set
            {
                _canFreeUser = value;
                NotifyOfPropertyChange(() => CanFreeUser);
            }
        }
        public bool CanPremiumUser
        {
            get { return _canPremiumUser; }
            set
            {
                _canPremiumUser = value;
                NotifyOfPropertyChange(() => CanPremiumUser);
            }
        }

        public bool IsAccountTypeIncreased
        {
            get
            {
                return UserContext.Current.ProfileInformation.Licence.CurrentAccountType !=
                       UserContext.Current.ProfileInformation.Licence.AccountType;
            }
        }
        public bool CanInstructor
        {
            get { return _canInstructor; }
            set
            {
                _canInstructor = value;
                NotifyOfPropertyChange(() => CanInstructor);
            }
        }
        
        public override void Fill()
        {
            Header = EnumLocalizer.Default.GetGUIString("ChangeAccountTypeView_Header");
            CanFreeUser = UserContext.Current.ProfileInformation.Licence.AccountType != AccountType.User;
            CanPremiumUser = UserContext.Current.ProfileInformation.Licence.AccountType != AccountType.PremiumUser;
            CanInstructor = UserContext.Current.ProfileInformation.Licence.AccountType != AccountType.Instructor;
            NotifyOfPropertyChange(null);
        }

        public override void RefreshView()
        {
            ParentWindow.RunAsynchronousOperation(delegate
            {
                UserContext.Current.RefreshUserData();
                //we don't need to invoke fill method because there is an event and fill will be called automatically
            });
        }

        public override Uri HeaderIcon
        {
            get { return "AccountType16.png".ToResourceUrl(); }
        }

        public List<ListItem<string>> PremiumFeatures
        {
            get { return premiumFeatures; }
        }


        public List<ListItem<string>> InstructorFeatures
        {
            get { return instructorFeatures; }
        }

        public List<ListItem<string>> FreeFeatures
        {
            get { return freeFeatures; }
        }

        private void btnImportSerial_Click(object sender, RoutedEventArgs e)
        {
            ImportLicenceKeyWindow dlg = new ImportLicenceKeyWindow();
            if(dlg.ShowDialog()==true)
            {
                RefreshView();
            }
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            UIHelper.BeginInvoke(Fill, Dispatcher);
            return true;
        }

        void changeAccountType(AccountType accountType)
        {
            if(UserContext.Current.ProfileInformation.Licence.AccountType >accountType)
            {
                int accountDiff = accountType - UserContext.Current.ProfileInformation.Licence.AccountType;
                int kara = Math.Abs(accountDiff) * UserContext.Current.ProfileInformation.Licence.Payments.Kara;
                if (BAMessageBox.AskWarningYesNo(EnumLocalizer.Default.GetGUIString("ChangeAccountTypeView_QLowerAccountType"), kara) == MessageBoxResult.No)
                {
                    return;
                }
            }
            else
            {
                if (BAMessageBox.AskYesNo(EnumLocalizer.Default.GetGUIString("ChangeAccountTypeView_QUpAccountType")) == MessageBoxResult.No)
                {
                    return;
                }
            }
            PleaseWait.Run((x)=>
            {
                try
                {
                    var param = new ProfileOperationParam();
                    param.Operation = ProfileOperation.AccountType;
                    param.ProfileId = UserContext.Current.CurrentProfile.GlobalId;
                    param.AccountType = accountType;
                    ServiceManager.ProfileOperation(param);
                    UserContext.Current.RefreshUserData();
                    Scheduler.Ensure();
                }
                catch(ConsistencyException ex)
                {
                    x.CloseProgressWindow();
                    Dispatcher.Invoke(new Action(() => ExceptionHandler.Default.Process(ex, EnumLocalizer.Default.GetGUIString("ChangeAccountTypeView_ErrEnoughtBAPoints"), ErrorWindow.MessageBox)));
                }
                catch (Exception ex)
                {
                    x.CloseProgressWindow();
                    Dispatcher.Invoke(new Action(() => ExceptionHandler.Default.Process(ex, EnumLocalizer.Default.GetGUIString("ChangeAccountTypeView_ErrDuringChangeAccountType"), ErrorWindow.EMailReport)));
                }
                
            },false,MainWindow.Instance);
        }

        private void btnFreeUser_Click(object sender, RoutedEventArgs e)
        {
            changeAccountType(AccountType.User);
        }

        private void btnPremiumUser_Click(object sender, RoutedEventArgs e)
        {
            changeAccountType(AccountType.PremiumUser);
        }

        private void btnInstructor_Click(object sender, RoutedEventArgs e)
        {
            changeAccountType(AccountType.Instructor);
        }

        private void btnBuy_Click(object sender, RoutedEventArgs e)
        {
            Helper.OpenUrl(ApplicationSettings.ServerUrl + "V2/Payments.aspx?Token=" + UserContext.Current.Token.SessionId);
        }
    }
}
