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
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.UI.WindowsSevenIntegration;

namespace BodyArchitect.Client.UI.Controls.ProgressIndicator
{
    /// <summary>
    /// Interaction logic for StatusBarItemProgressIndicator.xaml
    /// </summary>
    public partial class StatusBarItemProgressIndicator : UserControl
    {
        public StatusBarItemProgressIndicator()
        {
            InitializeComponent();
        }

        private TasksManager tasksManager;

		public void Connect(TasksManager tasksManager)
		{
			this.tasksManager = tasksManager;
            Unloaded += new RoutedEventHandler(StatusBarItemProgressIndicator_Unloaded);
			tasksManager.TaskListChanged += new EventHandler(tasksManager_TaskListChanged);
		}

        void StatusBarItemProgressIndicator_Unloaded(object sender, RoutedEventArgs e)
        {
            if (tasksManager != null)
            {
                tasksManager.TaskListChanged -= new EventHandler(tasksManager_TaskListChanged);
            }
        }

		void tasksManager_TaskListChanged(object sender, EventArgs e)
		{
			startProgress(tasksManager.StartedTasksCount > 0);

		}

		void startProgress(bool started)
		{

            if (!Dispatcher.CheckAccess())
			{
                Dispatcher.BeginInvoke(new Action<bool>(startProgress), started);
			}
			else
			{
                TaskbarProgressWrapper.UpdateProgressState(started
                                                             ? TaskbarProgressWrapper.State.Indeterminate
                                                             : TaskbarProgressWrapper.State.NoProgress);
			    ShowProgress = started;
				lblMessage.Text = started ? Strings.ProgressIndicator_Processing : string.Empty;
			}
		}


		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ShowProgress
		{
			get { return progressIndicator.IsRunning; }
			set { progressIndicator.IsRunning = value; }
		}

        public string Message
        {
            get { return lblMessage.Text; }
            set { lblMessage.Text=value; }
        }
    }
}
