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
using Coding4Fun.Phone.Controls;

namespace BodyArchitect.WP7.UserControls
{
    public partial class ForgotPasswordControl : UserControl
    {
        public ForgotPasswordControl()
        {
            InitializeComponent();
        }

        public static void Show()
        {
            var ctrl = new ForgotPasswordControl();
            ctrl.ShowPopup(dlg=>
                               {
                                   dlg.IsCancelVisible = true;
                                   dlg.Completed += (a, s) =>
                                   {
                                       if (s.PopUpResult == PopUpResult.Ok && !string.IsNullOrEmpty(ctrl.UserNameOrEmail))
                                       {
                                           //var service = ApplicationState.CreateService();
                                           //service.AccountOperationAsync(ctrl.UserNameOrEmail, AccountOperationType.RestorePassword);
                                           //BAMessageBox.ShowInfo(ApplicationStrings.ForgotPasswordControl_PasswordHasBeenSent);

                                           var m = new ServiceManager<GetImageCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<GetImageCompletedEventArgs> operationCompleted)
                                           {
                                               client1.AccountOperationAsync(ctrl.UserNameOrEmail, AccountOperationType.RestorePassword);
                                               BAMessageBox.ShowInfo(ApplicationStrings.ForgotPasswordControl_PasswordHasBeenSent);
                                           });

                                           m.Run(true);
                                       }

                                   };
                               });
        }

        protected string UserNameOrEmail
        {
            get { return txtUserName.Text; }
        }
    }
}
