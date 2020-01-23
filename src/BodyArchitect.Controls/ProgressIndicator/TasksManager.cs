using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Logger;
using BodyArchitect.Shared;
using OperationContext = BodyArchitect.Controls.Forms.OperationContext;

namespace BodyArchitect.Controls.UserControls
{
    public class TaskStateChangedEventArgs:EventArgs
    {
        private OperationContext context;

        public TaskStateChangedEventArgs(OperationContext context)
        {
            this.context = context;
        }

        public OperationContext Context
        {
            get { return context; }
        }
    }

    class TaskData
    {
        public TaskData(Task task, OperationContext context, Action<OperationContext> statusChangeOperation)
        {
            Task = task;
            Context = context;
            StatusChangeOperation = statusChangeOperation;
        }

        public Task Task { get; private set; }
        public OperationContext Context { get; private set; }
        public Action<OperationContext> StatusChangeOperation { get; private set; }
    }

    public class TasksManager
    {
        Dictionary<int,TaskData> tasksData = new Dictionary<int, TaskData>();
        object syncObject = new object();
        private SynchronizationContext context;
        public event EventHandler TaskListChanged;
        public event EventHandler<TaskStateChangedEventArgs> TaskStateChanged;

        public TasksManager(SynchronizationContext context)
        {
            this.context = context;

        }


        public SynchronizationContext SynchronizationContext
        {
            get
            {
                if (context != null)
                {
                    return context;
                }
                return new SynchronizationContext();
            }
        }

        public int StartedTasksCount
        {
            get
            {
                lock (syncObject)
                {
                    return tasksData.Count;
                }
            }
        }

        public void AddTask(Task task, OperationContext context, Action<OperationContext> statusChangeOperation)
        {
            lock(syncObject)
            {
                tasksData.Add(task.Id, new TaskData(task,context,statusChangeOperation));
            }
            onTaskListChanged();
        }
        private void onTaskListChanged()
        {
            if(TaskListChanged!=null)
            {
                TaskListChanged(this, EventArgs.Empty);
            }
        }

        private void onTaskStateChanged(OperationContext context)
        {
            if(TaskStateChanged!=null)
            {
                TaskStateChanged(this,new TaskStateChangedEventArgs(context));
            }
        }
        public void RemoveTask(Task task)
        {
            lock (syncObject)
            {
                tasksData.Remove(task.Id);
            }
            onTaskListChanged();
        }

        public bool ContainsTask(Task task)
        {
            lock (syncObject)
            {
                return tasksData.ContainsKey(task.Id);
            }
        }

        void sendThreadSafe(Action<OperationContext> method,OperationContext context)
        {
            //if (parentForm.Created)
            //{
            //    return;
            //}
            //if (parentForm.Created && parentForm.InvokeRequired)
            //{
            //    //parentForm.Invoke(new Action<Action<OperationContext>, OperationContext>(sendThreadSafe),method, context);
            //    parentForm.Invoke(method,  context);
            //}
            //else
            //{
            //    method(context);
            //}
            SynchronizationContext.Send(delegate(object arg)
                                            {
                                                method((OperationContext)arg);
                                            },context);
        }

        public CancellationTokenSource RunAsynchronousOperation(Action<OperationContext> method, Action<OperationContext> operationStateCallback = null, Action<OperationContext> exceptionCallback = null,bool addToList=true)
        {
            OperationContext context = new OperationContext(this);
            var tokenSource = new CancellationTokenSource();
            context.CancellatioToken = tokenSource.Token;

            var task = context.CurrentTask =  Task.Factory.StartNew(delegate(object cancellationToken)
            {
                ControlHelper.EnsureThreadLocalized();
                var arg = (OperationContext)cancellationToken;
                arg.State = OperationState.Started;
                onTaskStateChanged(arg);
                if (operationStateCallback != null)
                {
                   // SynchronizationContext.Send(state => operationStateCallback(arg), arg);
                    sendThreadSafe(operationStateCallback, arg);
                }
                try
                {
                    method((OperationContext)cancellationToken);
                    if (addToList && !ContainsTask(context.CurrentTask))
                    {
                        return;
                    }
                    arg.State = OperationState.Ended;
                    onTaskStateChanged(arg);
                    
                    if (operationStateCallback != null)
                    {
                        //SynchronizationContext.Send(state => operationStateCallback(arg), arg);
                        sendThreadSafe(operationStateCallback, arg);
                    }

                }
                catch(OperationCanceledException)
                {
                    arg.State = OperationState.Cancelled;
                    onTaskStateChanged(arg);
                    if (operationStateCallback != null)
                    {
                        sendThreadSafe(operationStateCallback, arg);
                    }
                }
                catch (ProfileDeletedException ex)
                {
                    setExceptionState(arg, operationStateCallback, ex);
                    UserContext.Logout(resetAutologon: false, skipLogoutOnServer: true);
                    SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorCurrentProfileDeleted, ErrorWindow.MessageBox), arg);
                }
                catch (UserDeletedException ex)
                {
                    setExceptionState(arg, operationStateCallback, ex);
                    SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorProfileDeleted, ErrorWindow.MessageBox), arg);
                }
                catch(OldDataException ex)
                {
                    setExceptionState(arg, operationStateCallback, ex);
                    SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex,ApplicationStrings.ErrorOldDataModification,ErrorWindow.MessageBox), arg);
                }
                catch(ValidationException ex)
                {
                    setExceptionState(arg, operationStateCallback, ex);
                    SynchronizationContext.Send(state =>FMMessageBox.ShowValidationError(ex.Results), arg);
                }
                catch(EndpointNotFoundException ex)
                {
                    setExceptionState(arg, operationStateCallback, ex);
                    UserContext.Logout(resetAutologon: false, skipLogoutOnServer: true);
                    SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex,ApplicationStrings.ErrorConnectionProblem,ErrorWindow.MessageBox), arg);
                }
                catch(TimeoutException ex)
                {
                    setExceptionState(arg, operationStateCallback, ex);
                    UserContext.Logout(resetAutologon: false, skipLogoutOnServer: true);
                    SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorConnectionProblem, ErrorWindow.MessageBox), arg);
                }
                catch (DatabaseVersionException ex)
                {
                    setExceptionState(arg, operationStateCallback, ex);
                    UserContext.Logout(resetAutologon: false, skipLogoutOnServer: true);
                    SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorOldVersionOfBodyArchitect, ErrorWindow.MessageBox), arg);
                }
                catch (MaintenanceException ex)
                {
                    setExceptionState(arg, operationStateCallback, ex);
                    UserContext.Logout(resetAutologon: false, skipLogoutOnServer: true);
                    SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorMaintenanceMode, ErrorWindow.MessageBox), arg);
                }
                catch (Exception ex)
                {
                    setExceptionState(arg, operationStateCallback, ex);
                    SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex, ex.Message, ErrorWindow.EMailReport), arg);
                    //Application.Exit();
                }
                finally
                {
                    RemoveTask(arg.CurrentTask);
                }

            }, context, tokenSource.Token);

            if (addToList)
            {
                AddTask(context.CurrentTask, context, operationStateCallback);
            }
            if (exceptionCallback != null)
            {
                LogUnhandledExceptions(task, exceptionCallback, context);
            }
            return tokenSource;
        }

        public void CancelTask(CancellationTokenSource cancelSource)
        {
            TaskData taskData = null;
            lock (syncObject)
            {
                var res=(from e in tasksData where e.Value.Context.CancellatioToken == cancelSource.Token select e.Value ).SingleOrDefault() ;

                if (res!=null)
                {
                    taskData = res;
                }

            }
            if (taskData != null)
            {
                if (taskData.StatusChangeOperation != null)
                {
                    cancelSource.Cancel();
                    taskData.Context.State = OperationState.Cancelled;
                    onTaskStateChanged(taskData.Context);
                    SynchronizationContext.Send(state => taskData.StatusChangeOperation(taskData.Context), taskData.Context);
                    RemoveTask(taskData.Task);
                }
            }
        }

        public void SetException(Exception ex)
        {
            TaskData taskData = null;
            lock(syncObject)
            {
                if(Task.CurrentId.HasValue && tasksData.ContainsKey(Task.CurrentId.Value))
                {
                    taskData = tasksData[Task.CurrentId.Value];
                }

            }
            if (taskData != null)
            {
                setExceptionState(taskData.Context, taskData.StatusChangeOperation, ex);
            }
        }
        private void setExceptionState(OperationContext arg, Action<OperationContext> operationStateCallback, Exception ex)
        {
            arg.ReturnedException = ex;
            onTaskStateChanged(arg);
            if (operationStateCallback != null)
            {
                SynchronizationContext.Send(state => operationStateCallback(arg), arg);
            }
        }

        public Task LogUnhandledExceptions(Task task, Action<OperationContext> exceptionCallback, OperationContext operationContext)
        {
            task.ContinueWith(delegate(Task t)
            {
                ControlHelper.EnsureThreadLocalized();
                if (exceptionCallback != null)
                {
                    SynchronizationContext.Send(delegate(object state)
                    {
                        exceptionCallback((OperationContext)state);
                    }, operationContext);
                }
                else
                {
                    SynchronizationContext.Send(delegate(object state)
                    {
                        throw ((Task)state).Exception;
                    }, operationContext);
                }

            }, TaskContinuationOptions.OnlyOnFaulted);
            return task;
        }

    }
}
