using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BodyArchitect.Common;
using BodyArchitect.Controls.Basic;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Controls.UserControls;
using DevExpress.XtraEditors;

namespace BodyArchitect.Controls.ProgressIndicator
{
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
            okButton1.DialogResult = DialogResult.None;
            AllowCancel = true;

        }

        [DefaultValue(true)]
        public bool AllowCancel { get; set; }

        public OKButton OkButton
        {
            get { return okButton1; }
        }

        public CancelButton CancelButton
        {
            get { return cancelButton1; }
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            if (!ServicesManager.IsDesignMode)
            {
                XtraForm form = ControlHelper.GetParentControl<XtraForm>(this);
                if (form != null)
                {
                    form.AcceptButton = okButton1;
                    form.CancelButton = cancelButton1;
                }
                
            }
        }

        public void UpdateProgressIndicator(OperationContext context)
        {
            bool startLoginOperation = context.State == OperationState.Started;
            if(!AllowCancel)
            {
                if(startLoginOperation)
                {
                    ParentWindow.DisableCloseButton();
                }
                else
                {
                    ParentWindow.EnableCloseButton();
                }
            }
            if (TaskProgressChanged!=null)
            {
                TaskProgressChanged(this, new TaskStateChangedEventArgs(context));
            }
            okButton1.Enabled = !startLoginOperation;
            cancelButton1.Enabled = AllowCancel || !startLoginOperation;
            if (startLoginOperation)
            {
                progressIndicator1.Start();
            }
            else
            {
                progressIndicator1.Stop();
            }
            progressIndicator1.Visible = startLoginOperation;

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

        private void okButton1_Click(object sender, EventArgs e)
        {
            if (OkClick != null)
            {
                ParentWindow.DialogResult = DialogResult.None;
                cancelToken=ParentWindow.RunAsynchronousOperation(delegate(OperationContext context)
                                                          {
                                                              OkClick(sender,new CancellationSourceEventArgs(context.CancellatioToken));

                                                          }, UpdateProgressIndicator);
            }
        }

        private void cancelButton1_Click(object sender, EventArgs e)
        {
            if (cancelToken!=null)
            {
                cancelToken.Cancel();
            }
            if (CancelClick != null)
            {
                CancelClick(sender, e);
            }
        }

        private void usrProgressIndicatorButtons_Load(object sender, EventArgs e)
        {
            //if (ParentWindow != null && ParentWindow.TasksManager != null)
            {
                //ParentWindow.TasksManager.TaskStateChanged +=(TasksManager_TaskStateChanged);
            }
        }
    }

    public class CancellationSourceEventArgs:EventArgs
    {
        public CancellationSourceEventArgs(CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
        }

        public CancellationToken CancellationToken { get; private set; }
    }
}
