using System;
using System.Windows.Forms;
using BodyArchitect.Common.Plugins;
using BodyArchitect.Common.ProgressOperation;
using BodyArchitect.Controls.Localization;


namespace BodyArchitect.Controls
{
    /// <summary>
    /// This window is display when the operation is run in background using PleaseWait.Run method. Do not use it directly. 
    /// </summary>
    internal partial class PleaseWaitWindow : DevExpress.XtraEditors.XtraForm, IProgressWindow
    {
        private MethodParameters par;
        private bool pendingClosing;

        private PleaseWaitWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PleaseWaitWindow"/> class.
        /// </summary>
        /// <param name="canBeCanceled">if set to <c>true</c> cancel button is visible.</param>
        public PleaseWaitWindow(bool canBeCanceled, int? progressMax)
        {
            InitializeComponent();
            btnCancel.Visible = canBeCanceled;
            this.Load += pleaseWaitWindow_Load;
            if (progressMax.HasValue)
            {
                progressBarControl1.Properties.Maximum = progressMax.Value;
            }
            progressBarControl1.Visible = progressMax.HasValue;
            this.marqueeProgressBarControl1.Visible = !progressMax.HasValue;
        }

        /// <summary>
        /// Gets or sets the parameters of runned operation
        /// </summary>
        /// <value>The parameters object.</value>
        public MethodParameters Parameters
        {
            get
            {
                return par;
            }
            set
            {
                par = value;
            }
        }

        /// <summary>
        /// This is thread safe version of Close() method <see cref="Form.Close"/>
        /// </summary>
        public void ThreadSafeClose()
        {
            //sometimes when operation is very quick and window is not created completelly when PleaseWait class invoke ThreadSafeClose we get many exceptions.
            //so here we check this and if we are in such situation (ThreadSafeClose is invoked before creating window), we remember information about closing the window
            //and we will do this in Load event
            if (!Created)
            {
                pendingClosing = true;
                return;
            }
            if (InvokeRequired)
            {
                BeginInvoke(new Action(ThreadSafeClose));
            }
            else
            {
                Close();
            }
        }

        /// <summary>
        /// Sets the message which will be displayed in PleaseWait window.
        /// </summary>
        /// <param name="message">The message to display.</param>
        public void SetMessage(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new System.Action<string>(SetMessage), message);
            }
            else
            {
                labelControl1.Text = message;
                Application.DoEvents();
            }
        }

        public void SetProgress(int value)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new System.Action<int>(SetProgress), value);
            }
            else
            {
                progressBarControl1.Position = value;
                Application.DoEvents();
            }
        }

        private void pleaseWaitWindow_Load(object sender, EventArgs e)
        {
            if (pendingClosing)
            {
                ThreadSafeClose();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            labelControl1.Text = ApplicationStrings.CancelingOperation;
            Parameters.Cancel = true;
            btnCancel.Enabled = false;
        }
    }
}