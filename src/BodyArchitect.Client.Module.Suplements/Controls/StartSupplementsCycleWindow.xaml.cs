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
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Suplements.Controls
{
    public partial class StartSupplementsCycleWindow 
    {
        private StartSupplementsCycleWindowViewModel viewModel;

        public StartSupplementsCycleWindow()
        {
            InitializeComponent();
            usrProgressIndicatorButtons1.OkButton.Content = EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Suplements:SuplementsEntryStrings:StartSupplementsCycleWindow_OKButton_Start");
            viewModel=new StartSupplementsCycleWindowViewModel();
            DataContext = viewModel;
        }

        public bool AllowChangePlan
        {
            get
            {
                if (viewModel!=null)
                {
                    return viewModel.AllowChangePlan;    
                }
                return false;

            }
            set
            {
                viewModel.AllowChangePlan = value;
            }
        } 

        public SupplementCycleDefinitionDTO SelectedCycleDefinition
        {
            get {
                return viewModel.CycleDefinition;
            }
            set {
                viewModel.CycleDefinition = value;
            }
        }

        public CustomerDTO Customer
        {
            get { return viewModel.Customer; }
            set { viewModel.Customer=value; }
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
                this.SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex, EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Suplements:SuplementsEntryStrings:Exception_StartSupplementsCycleWindow_OkClick_CannotStartCycle"), ErrorWindow.EMailReport), null);
            }
            
        }

        private void btnTrainingPreview_Click(object sender, RoutedEventArgs e)
        {
            
            RunAsynchronousOperation(op =>
                                         {
                                             try
                                             {
                                                 var result = viewModel.SimulateCycle();
                                                 Dispatcher.BeginInvoke(new Action(() =>
                                                 {
                                                     myTrainingPreview.Fill(result.EntryObjects.ToList());
                                                     myTrainingPreview.Visibility = System.Windows.Visibility.Visible;
                                                     lblPreviewMessage.Visibility = Visibility.Collapsed;
                                                 }));
                                             }
                                             catch (TimeoutException ex)
                                             {
                                                 TasksManager.SetException(ex);
                                                 this.SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex, "Exception_StartSupplementsCycleWindow_ErrPreviewTimeout".TranslateSupple(), ErrorWindow.EMailReport), null);
                                             }
                                             catch (Exception ex)
                                             {
                                                 TasksManager.SetException(ex);
                                                 this.SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex, "Exception_StartSupplementsCycleWindow_ErrPreview".TranslateSupple(), ErrorWindow.EMailReport), null);
                                             }
                                         },op=>
                                               {
                                                   previewProgressIndicator.IsRunning = op.State == OperationState.Started;
                                                   btnPreview.IsEnabled = op.State != OperationState.Started;
                                               });
            
            
        }
    }
}
