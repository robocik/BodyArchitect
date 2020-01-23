using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Client.Module.A6W.Localization;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.A6W.Controls
{

    public partial class StartA6WCycleWindow
    {
        private StartA6WViewModel viewModel;

        public StartA6WCycleWindow(DateTime startDate)
        {
            InitializeComponent();
            viewModel = new StartA6WViewModel(startDate);
            DataContext = viewModel;
            usrProgressIndicatorButtons1.OkButton.Content = EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.A6W:A6WEntryStrings:StartA6WCycleWindow_OKButton_Start");
            var binding = new Binding("CanStart");
            binding.Mode = BindingMode.OneWay;
            usrProgressIndicatorButtons1.OkButton.SetBinding(Button.IsEnabledProperty, binding);
        }

        public A6WTrainingDTO A6WTraining { get; set; }

        public CustomerDTO Customer
        {
            get {
                return viewModel.Customer;
            }
            set {
                viewModel.Customer = value;
            }
        }

        private void btnTrainingPreview_Click(object sender, RoutedEventArgs e)
        {
            RunAsynchronousOperation(op =>
            {
                var result = viewModel.SimulateCycle();
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    myTrainingPreview.Fill(result.EntryObjects.ToTrainingDays());
                    myTrainingPreview.Visibility = System.Windows.Visibility.Visible;
                    lblPreviewMessage.Visibility = Visibility.Collapsed;
                }));
            }, op =>
            {
                previewProgressIndicator.IsRunning = op.State == OperationState.Started;
                btnPreview.IsEnabled = op.State != OperationState.Started;
            });
        }

        private void usrProgressIndicatorButtons_OkClick(object sender, UI.Controls.CancellationSourceEventArgs e)
        {
            try
            {
                viewModel.StartCycle(Dispatcher);
                ThreadSafeClose(true);
            }
            catch (Exception ex)
            {
                
                TasksManager.SetException(ex);
                this.SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex, EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.A6W:A6WEntryStrings:ExceptionCannotStartA6WTraining"), ErrorWindow.EMailReport), null);
            }
        }
    }
}
