using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;
using Microsoft.Phone.Controls;

namespace BodyArchitect.WP7.UserControls
{
    public partial class UpgradeAccountControl
    {
        public UpgradeAccountControl()
        {
            InitializeComponent();
        }

        public static bool EnsureAccountType(string featureName, DependencyObject page, AccountType accountType = AccountType.PremiumUser)
        {
            if(ApplicationState.Current.ProfileInfo.Licence.CurrentAccountType<accountType)
            {
                ShowInfo(featureName,  page, accountType);
                return false;
            }
            return true;
        }

        public static void ShowInfo(string featureName, DependencyObject page,AccountType accountType=AccountType.PremiumUser)
        {

            showProImplementation(featureName, page,accountType);
        }



        static void showProImplementation(string title,  DependencyObject page, AccountType accountType)
        {

            var ctrl = new UpgradeAccountControl();
            var popup = ctrl.ShowPopup(t =>
            {
                ctrl.lblFeatureName.Text = title;
                ctrl.lblDescription.MakeFormattedTextBlock(accountType == AccountType.PremiumUser ?
                    ApplicationStrings.UpgradeAccountControl_PremiumAccountMessage : 
                    ApplicationStrings.UpgradeAccountControl_InstructorAccountMessage);
                if (accountType==AccountType.PremiumUser)
                {
                    ctrl.btnPremiumInfo.Visibility = Visibility.Visible;
                }
                else
                {
                    ctrl.btnInstructorInfo.Visibility = Visibility.Visible;
                }
                t.ActionPopUpButtons.Clear();
                var btn = new Button();
                btn.Click += (s, e) =>
                                 {
                                     page.GetParent<PhoneApplicationPage>().Navigate("/Pages/AccountTypePage.xaml");
                                 };

                btn.Content = ApplicationStrings.UpgradeAccountControl_ChangeButton; 
                t.ActionPopUpButtons.Add(btn);
                btn = new Button();
                btn.Content = ApplicationStrings.ButtonClose;
                btn.Click += delegate
                {
                    t.Hide();
                };
                t.ActionPopUpButtons.Add(btn);
            }, page);
        }
    }
}
