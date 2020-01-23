using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Controls.ProgressIndicator;

namespace BodyArchitect.Controls.UserControls
{
	class ToolStripProgressIndicator: ToolStripControlHost
	{
		private TasksManager tasksManager;

		public ToolStripProgressIndicator()
			: base(new ProgressIndicatorStatus())
		{
		}

		public void Connect(TasksManager tasksManager)
		{
			this.tasksManager = tasksManager;
			tasksManager.TaskListChanged += new EventHandler(tasksManager_TaskListChanged);
		}

		void tasksManager_TaskListChanged(object sender, EventArgs e)
		{
			startProgress(tasksManager.StartedTasksCount > 0);

		}

		void startProgress(bool started)
		{
			if(this.Parent.InvokeRequired)
			{
				Parent.BeginInvoke(new Action<bool>(startProgress), started);
			}
			else
			{
				ShowProgress = started;
				ProgressIndicatorStatus.Message = started ? ApplicationStrings.ProgressIndicator_Processing : string.Empty;
			}
		}

		public ProgressIndicatorStatus ProgressIndicatorStatus
		{
			get
			{
				return Control as ProgressIndicatorStatus;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ShowProgress
		{
			get
			{
				if (ProgressIndicatorStatus != null)
				{
					return ProgressIndicatorStatus.ShowProgress;
				}
				return false;
			}
			set
			{
				if (ProgressIndicatorStatus != null)
				{
					ProgressIndicatorStatus.ShowProgress = value;
				}
			}
		}


		protected override void OnUnsubscribeControlEvents(Control c)
		{
			if (tasksManager != null)
			{
				tasksManager.TaskListChanged -= new EventHandler(tasksManager_TaskListChanged);
			}
			// Call the base method so the basic events are unsubscribed.
			base.OnUnsubscribeControlEvents(c);


		}

	}
}
