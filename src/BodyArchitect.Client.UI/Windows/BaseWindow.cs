using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Settings;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace BodyArchitect.Client.UI.Windows
{
    public interface IBAWindow
    {
        CancellationTokenSource RunAsynchronousOperation(Action<OperationContext> method,
                                                         Action<OperationContext> operationStateCallback = null,Action<OperationContext> exceptionCallback = null);

        void CancelTask(CancellationTokenSource cancelSource);

        SynchronizationContext SynchronizationContext { get; }
        void SetException(Exception ex);

        bool? DialogResult { get; set; }
    }

    public class BaseWindow : Window, IBAWindow,INotifyPropertyChanged
    {
        bool saveSizeAndLocation = true;
        private TasksManager tasksManager;


        public BaseWindow()
        {
            Loaded += new RoutedEventHandler(BaseWindow_Loaded);
            DataContext = this;

        }

        public virtual void NotifyOfPropertyChange<TProperty>(Expression<Func<TProperty>> property)
        {
            NotifyOfPropertyChange(property.GetMemberInfo().Name);
        }

        public virtual void NotifyOfPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #region Disable close window button

        private const int MF_BYPOSITION = 0x400;

        public void DisableCloseButton()
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            IntPtr hMenu = GetSystemMenu(hwnd, false);
            int menuItemCount = GetMenuItemCount(hMenu);
            RemoveMenu(hMenu, menuItemCount - 1, MF_BYPOSITION);
            DrawMenuBar((int)hwnd);
        }
        public void EnableCloseButton()
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            GetSystemMenu(hwnd, true);
            DrawMenuBar((int)hwnd);
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

        protected virtual void SaveGuiState()
        {

        }

        protected virtual void LoadGuiState()
        {

        }

        protected virtual void LoginStatusChanged(LoginStatus newStatus)
        {
            if (newStatus != LoginStatus.Logged)
            {
                ThreadSafeClose(false);
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

        protected void SaveProcess()
        {
            //TODO:FINISH
            //if (GuiState.Default.WindowsState == null)
            //{
            //    Hashtable table = new Hashtable();
            //    GuiState.Default.WindowsState = table;
            //}
            //GuiState.Default.WindowsState[form.Name + "Size"] = form.Size;
            //GuiState.Default.WindowsState[form.Name + "WindowState"] = form.WindowState;

            //GuiState.Default.WindowsState[form.Name + "Location"] = form.Location;
        }

        protected void LoadProcess()
        {
            //TODO:FINISH
            //if (GuiState.Default.WindowsState != null)
            //{
            //    Hashtable dict = GuiState.Default.WindowsState;
            //    if (form.FormBorderStyle == FormBorderStyle.Sizable && GuiState.Default.WindowsState.ContainsKey(form.Name + "Size"))
            //    {
            //        form.Size = (Size)dict[form.Name + "Size"];
            //    }
            //    if (GuiState.Default.WindowsState.ContainsKey(form.Name + "WindowState"))
            //    {
            //        form.WindowState = (FormWindowState)dict[form.Name + "WindowState"];
            //    }
            //    if (GuiState.Default.WindowsState.ContainsKey(form.Name + "Location"))
            //    {
            //        form.Location = (Point)dict[form.Name + "Location"];
            //    }
            //}
        }

        void BaseWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (SaveSizeAndLocation && !Helper.IsDesignMode)
            {
                LoadProcess();
                LoadGuiState();
            }
        }

        void UserContext_LoginStatusChanged(object sender, LoginStatusEventArgs e)
        {
            if (TasksManager == null)
            {
                return;
            }
            UIHelper.BeginInvoke(new Action(delegate
            {
                LoginStatusChanged(e.Status);
            }), null);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            cancelCurrentTask();
            SaveProcess();
            SaveGuiState();
            UserContext.Current.LoginStatusChanged -= new EventHandler<LoginStatusEventArgs>(UserContext_LoginStatusChanged);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            UserContext.Current.LoginStatusChanged += new EventHandler<LoginStatusEventArgs>(UserContext_LoginStatusChanged);
            tasksManager = new TasksManager(new DispatcherSynchronizationContext(Dispatcher));
            
        }

        #region Thread operation


        public void ThreadSafeClose(bool? result)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action<bool?>(ThreadSafeClose), result);
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

        public void CancelTask(CancellationTokenSource cancelSource)
        {
            TasksManager.CancelTask(cancelSource);
        }

        [Browsable(false)]
        public SynchronizationContext SynchronizationContext
        {
            get { return tasksManager.SynchronizationContext; }
        }

        public void SetException(Exception ex)
        {
            TasksManager.SetException(ex);
        }

        public Task LogUnhandledExceptions(Task task, Action<OperationContext> exceptionCallback, OperationContext operationContext)
        {
            task.ContinueWith(delegate(Task t)
            {
                Helper.EnsureThreadLocalized();
                if (exceptionCallback != null)
                {
                    Dispatcher.BeginInvoke(new Action<OperationContext>(delegate(OperationContext context)
                    {
                        exceptionCallback(context);
                    }), operationContext);
                }
                else
                {
                    Dispatcher.BeginInvoke(new Action<Task>(delegate(Task ta)
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
            if (tokenSource != null)
            {
                tokenSource.Cancel();
                tokenSource = null;
            }
        }

        #endregion
    }
}
