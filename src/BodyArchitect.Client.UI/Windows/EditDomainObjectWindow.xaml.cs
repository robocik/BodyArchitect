using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.Resources.Localization;

namespace BodyArchitect.Client.UI.Windows
{
    /// <summary>
    /// Interaction logic for EditDomainObjectWindow.xaml
    /// </summary>
    public partial class EditDomainObjectWindow
    {
        private IEditableControl userControl;

        public EditDomainObjectWindow()
        {
            InitializeComponent();
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
                        throw new Exception("UnknownAction");
                    }
            }
            usrProgressIndicatorButtons1.OkButton.IsEnabled = errorCount == 0;
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

        public void SetControl(IEditableControl userControl)
        {
            this.userControl = userControl;
            //userControl.SetValue(Grid.RowProperty, 0);
            userControl.ReadOnly = false;
            //mainGrid.Children.Add(userControl);
            placeHolder.Children.Add((Control)userControl);
        }

        public void UpdateProgressIndicator(OperationContext context)
        {
            bool startLoginOperation = context.State == OperationState.Started;
            usrProgressIndicatorButtons1.UpdateProgressIndicator(context);
            placeHolder.IsEnabled = !startLoginOperation;
        }

        private void usrProgressIndicatorButtons_OkClick(object sender, Controls.CancellationSourceEventArgs e)
        {
            bool isValid = true;
            Dispatcher.Invoke(new Action(delegate
               {
                   if (!validateInternal())
                   {
                       isValid = false;
                       BAMessageBox.ShowError(Strings.Message_EditDomainObjectWindow_WrongFields);
                       return;
                   }
                   
               }));
            if(!isValid)
            {
                return;
            }
            var res=userControl.Save();

            UIHelper.BeginInvoke(new Action(delegate
                                             {
                                                 if(res!=null)
                                                 {
                                                     userControl.Object = res;
                                                     DialogResult = true;
                                                     Close();
                                                 }
                                             }), Dispatcher);
        }

        private void usrProgressIndicatorButtons1_TaskProgressChanged(object sender, TaskStateChangedEventArgs e)
        {
            UpdateProgressIndicator(e.Context);
        }
    }
}
