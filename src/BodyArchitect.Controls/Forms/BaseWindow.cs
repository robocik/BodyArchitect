using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BodyArchitect.Common;
using BodyArchitect.Controls.UserControls;
using DevExpress.XtraEditors;

namespace BodyArchitect.Controls.Forms
{
    public partial class BaseWindow : XtraForm
    {
        bool saveSizeAndLocation = true;
        private TasksManager tasksManager;

        public BaseWindow()
        {
            InitializeComponent();
            
        }

        #region Disable close window button

        private const int MF_BYPOSITION = 0x400;

        public void DisableCloseButton()
        {
            IntPtr hMenu = GetSystemMenu(this.Handle, false);
            int menuItemCount = GetMenuItemCount(hMenu);
            RemoveMenu(hMenu, menuItemCount - 1, MF_BYPOSITION);
            DrawMenuBar((int)this.Handle);
        }
        public void EnableCloseButton()
        {
            GetSystemMenu(this.Handle, true);
            DrawMenuBar((int)this.Handle);
        }

        // Win32 API declarations
        [DllImport("User32")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("User32")]
        private static extern int GetMenuItemCount(IntPtr hWnd);
        [DllImport("User32")]
        private static extern int RemoveMenu(IntPtr hMenu, int nPosition, int wFlags);
        [DllImport("User32")]
        private static extern IntPtr DrawMenuBar(int hwnd);

  
        #endregion

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            UserContext.LoginStatusChanged += new EventHandler<LoginStatusEventArgs>(UserContext_LoginStatusChanged);
        }
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            UserContext.LoginStatusChanged -= new EventHandler<LoginStatusEventArgs>(UserContext_LoginStatusChanged);
            base.OnFormClosed(e);
        }
        void UserContext_LoginStatusChanged(object sender, LoginStatusEventArgs e)
        {
            if (TasksManager == null)
            {
                return;
            }
            SynchronizationContext.Send(delegate
            {
                LoginStatusChanged(e.Status);
            }, null);
            //if(!Created)
            //{
            //    return;
            //}
            //if(InvokeRequired)
            //{
            //    BeginInvoke(new Action<LoginStatus>(LoginStatusChanged), e.Status);
            //}
            //else
            //{
            //    LoginStatusChanged(e.Status);
            //}
        }


        protected virtual void LoginStatusChanged(LoginStatus newStatus)
        {
            if (newStatus != LoginStatus.Logged)
            {
                //ThreadSafeClose(System.Windows.Forms.DialogResult.Cancel);
                DialogResult = System.Windows.Forms.DialogResult.Cancel;
                Close();
            }
        }
        
        public bool SaveSizeAndLocation
        {
            get
            {
                return saveSizeAndLocation;
            }
            set
            {
                saveSizeAndLocation = value;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            var con = System.Threading.SynchronizationContext.Current is WindowsFormsSynchronizationContext?System.Threading.SynchronizationContext.Current:new WindowsFormsSynchronizationContext();
            tasksManager = new TasksManager(con);
            base.OnLoad(e);
            
        }
        private void BaseLightWindow_Load(object sender, EventArgs e)
        {
            if (SaveSizeAndLocation && !this.DesignMode)
            {
                UserContext.Settings.GuiState.LoadProcess(this);
                LoadGuiState();
            }
        }

        protected virtual void SaveGuiState()
        {

        }

        protected virtual void LoadGuiState()
        {

        }

        private void BaseLightWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            cancelCurrentTask();
            UserContext.Settings.GuiState.SaveProcess(this);
            SaveGuiState();
        }

        [Browsable(false)]
        public SynchronizationContext SynchronizationContext
        {
            get { return TasksManager.SynchronizationContext; }
        }

        #region Thread operation

        public void ThreadSafeClose(DialogResult result=DialogResult.OK)
        {
            if(InvokeRequired)
            {
                Invoke(new Action<DialogResult>(ThreadSafeClose), result);
            }
            else
            {
                DialogResult = result;
                Close();
            }
        }


        private CancellationTokenSource tokenSource;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TasksManager TasksManager
        {
            get
            {
                return tasksManager;
            }
        }


        public Task LogUnhandledExceptions(Task task, Action<OperationContext> exceptionCallback,OperationContext operationContext)
        {
            task.ContinueWith(delegate(Task t)
                                  {
                                      ControlHelper.EnsureThreadLocalized();
                                      if (exceptionCallback != null)
                                      {
                                          Invoke(new Action<OperationContext>(delegate(OperationContext context)
                                                     {
                                                         exceptionCallback(context);
                                                     }), operationContext);
                                      }
                                      else
                                      {
                                          Invoke(new Action<Task>(delegate(Task ta)
                                                                      {
                                                                          throw ta.Exception;
                                                                      }), t);
                                      }

                                  }, TaskContinuationOptions.OnlyOnFaulted);
            return task;
        }

        public virtual CancellationTokenSource RunAsynchronousOperation(Action<OperationContext> method, Action<OperationContext> operationStateCallback = null, Action<OperationContext> exceptionCallback = null)
        {
            //cancelCurrentTask();
            tokenSource = TasksManager.RunAsynchronousOperation(method, operationStateCallback,
                                                                exceptionCallback);

            return tokenSource;
        }

        private void cancelCurrentTask()
        {
            if (tokenSource!=null)
            {
                tokenSource.Cancel();
                tokenSource = null;
            }
        }

        #endregion

    }

    

    public enum ApplicationState
    {
        Ready,
        Busy
    }
}