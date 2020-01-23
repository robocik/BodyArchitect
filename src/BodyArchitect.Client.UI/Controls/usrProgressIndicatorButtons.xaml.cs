using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
using BodyArchitect.Client.UI.Windows;

namespace BodyArchitect.Client.UI.Controls
{
    /// <summary>
    /// Interaction logic for usrProgressIndicatorButtons.xaml
    /// </summary>
    [DefaultEvent("OkClick")]
    public partial class usrProgressIndicatorButtons : usrBaseControl
    {
        public event EventHandler<CancellationSourceEventArgs> OkClick;
        public event EventHandler CancelClick;
        public event EventHandler<TaskStateChangedEventArgs> TaskProgressChanged;
        private CancellationTokenSource cancelToken;

        public usrProgressIndicatorButtons()
        {
            InitializeComponent();
            //progressIndicator1.IsRunning = true;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (OkClick != null)
            {
                if (System.Windows.Interop.ComponentDispatcher.IsThreadModal)
                {
                    ParentWindow.DialogResult = null;
                }
                cancelToken = ParentWindow.RunAsynchronousOperation(delegate(OperationContext context)
                {
                    OkClick(sender, new CancellationSourceEventArgs(context.CancellatioToken));

                }, context=>
                       {
                           UpdateProgressIndicator(context);
                           if (TaskProgressChanged != null)
                           {
                               TaskProgressChanged(this, new TaskStateChangedEventArgs(context));
                           }
                       });
            }
        }

        [DefaultValue(true)]
        public bool AllowCancel { get; set; }

        public Button OkButton
        {
            get { return btnOK; }
        }

        public Button CancelButton
        {
            get { return btnCancel; }
        }


        public void UpdateProgressIndicator(OperationContext context)
        {
            bool startLoginOperation = context.State == OperationState.Started;
            if (!AllowCancel)
            {
                BaseWindow baseWindow = ParentWindow as BaseWindow;
                if (baseWindow != null)
                {
                    if (startLoginOperation)
                    {
                        baseWindow.DisableCloseButton();
                    }
                    else
                    {
                        baseWindow.EnableCloseButton();
                    }
                }
            }
            
            //btnOK.IsEnabled = !startLoginOperation;
            btnOK.SetCurrentValue(Button.IsEnabledProperty, !startLoginOperation);
            btnCancel.SetCurrentValue(Button.IsEnabledProperty, !AllowCancel || !startLoginOperation);
            //btnCancel.IsEnabled = AllowCancel || !startLoginOperation;
            progressIndicator1.IsRunning = startLoginOperation;
        }

        //void TasksManager_TaskStateChanged(object sender, UserControls.TaskStateChangedEventArgs e)
        //{

        //    //if (cancelToken == null || cancelToken.Token != e.Context.CancellatioToken)
        //    //{
        //    //    return;
        //    //}
        //    ParentWindow.SynchronizationContext.Send(delegate
        //                       {
        //                           UpdateProgressIndicator(e.Context);
        //                       },e);

        //    if(e.Context.State!=OperationState.Started)
        //    {
        //        cancelToken =null;
        //    }

        //}


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (cancelToken != null)
            {
                cancelToken.Cancel();
            }
            if (CancelClick != null)
            {
                CancelClick(sender, e);
            }
        }
    }

    public class CancellationSourceEventArgs : EventArgs
    {
        public CancellationSourceEventArgs(CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
        }

        public CancellationToken CancellationToken { get; private set; }
    }
}
