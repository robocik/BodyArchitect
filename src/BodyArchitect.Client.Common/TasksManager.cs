using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Logger;
using BodyArchitect.Portable.Exceptions;
using BodyArchitect.Shared;
using OperationContext = BodyArchitect.Client.Common.OperationContext;

namespace BodyArchitect.Client.Common
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
        ConcurrentDictionary<int, TaskData> tasksData = new ConcurrentDictionary<int, TaskData>();
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
            get { return tasksData.Count; }
        }

        public void AddTask(OperationContext context, Action<OperationContext> statusChangeOperation)
        {
            tasksData.TryAdd(Task.CurrentId.Value, new TaskData(context.Task, context, statusChangeOperation));
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
        public void RemoveTask(int taskId)
        {
            TaskData data;
            tasksData.TryRemove(taskId, out data);
            onTaskListChanged();
        }

        public bool ContainsTask(Task task)
        {
            if (task==null)
            {
                return false;
            }
            return tasksData.ContainsKey(task.Id);
        }

        void sendThreadSafe(Action<OperationContext> method,OperationContext context)
        {
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
            context.Task = Task.Factory.StartNew(delegate(object cancellationToken)
            {
                var arg = (OperationContext)cancellationToken;
                try
                {
                    if (addToList)
                    {
                        AddTask(context, operationStateCallback);
                    }

                    Helper.EnsureThreadLocalized();
                    
                    arg.State = OperationState.Started;
                    onTaskStateChanged(arg);
                    if (operationStateCallback != null)
                    {
                        sendThreadSafe(operationStateCallback, arg);
                    }
                
                    method((OperationContext)cancellationToken);
                    if (addToList && !ContainsTask(arg.Task))
                    {
                        return;
                    }
                    arg.State = OperationState.Ended;
                    onTaskStateChanged(arg);
                    
                    if (operationStateCallback != null)
                    {
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
                catch (LicenceException ex)
                {
                    setExceptionState(arg, operationStateCallback, ex);
                    SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex, Strings.ErrorLicence, ErrorWindow.MessageBox),arg);
                }
                catch (ProfileDeletedException ex)
                {
                    setExceptionState(arg, operationStateCallback, ex);
                    UserContext.Current.Logout(resetAutologon: false, skipLogoutOnServer: true);
                    SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex, Strings.ErrorCurrentProfileDeleted, ErrorWindow.MessageBox), arg);
                }
                catch (UserDeletedException ex)
                {
                    setExceptionState(arg, operationStateCallback, ex);
                    SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex, Strings.ErrorProfileDeleted, ErrorWindow.MessageBox), arg);
                }
                catch(OldDataException ex)
                {
                    setExceptionState(arg, operationStateCallback, ex);
                    SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex, Strings.ErrorOldDataModification, ErrorWindow.MessageBox), arg);
                }
                catch(ValidationException ex)
                {
                    setExceptionState(arg, operationStateCallback, ex);
                    SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex, Strings.ErrorCurrentProfileDeleted, ErrorWindow.MessageBox), arg);
                }
                catch(EndpointNotFoundException ex)
                {
                    setExceptionState(arg, operationStateCallback, ex);
                    UserContext.Current.Logout(resetAutologon: false, skipLogoutOnServer: true);
                    SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex, Strings.ErrorConnectionProblem, ErrorWindow.MessageBox), arg);
                }
                catch(TimeoutException ex)
                {
                    setExceptionState(arg, operationStateCallback, ex);
                    UserContext.Current.Logout(resetAutologon: false, skipLogoutOnServer: true);
                    SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex, Strings.ErrorConnectionProblem, ErrorWindow.MessageBox), arg);
                }
                catch (DatabaseVersionException ex)
                {
                    setExceptionState(arg, operationStateCallback, ex);
                    UserContext.Current.Logout(resetAutologon: false, skipLogoutOnServer: true);
                    SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex, Strings.ErrorOldVersionOfBodyArchitect, ErrorWindow.MessageBox), arg);
                }
                catch (MaintenanceException ex)
                {
                    setExceptionState(arg, operationStateCallback, ex);
                    UserContext.Current.Logout(resetAutologon: false, skipLogoutOnServer: true);
                    SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex, Strings.ErrorMaintenanceMode, ErrorWindow.MessageBox), arg);
                }
                catch (Exception ex)
                {
                    setExceptionState(arg, operationStateCallback, ex);
                    SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex, ex.Message, ErrorWindow.EMailReport), arg);
                    //Application.Exit();
                }
                finally
                {
                    RemoveTask(Task.CurrentId.Value);
                }

            }, context,tokenSource.Token);

            
            if (exceptionCallback != null)
            {
                LogUnhandledExceptions(context.Task, exceptionCallback, context);
            }
            return tokenSource;
        }

        public void CancelTask(CancellationTokenSource cancelSource)
        {
            TaskData taskData = null;
            //lock (syncObject)
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
                    RemoveTask(taskData.Task.Id);
                }
            }
        }

        public void SetException(Exception ex)
        {
            TaskData taskData = null;
            //lock(syncObject)
            {
               
                if(Task.CurrentId.HasValue && tasksData.TryGetValue(Task.CurrentId.Value,out taskData))
                {
                    //taskData = tasksData[Task.CurrentId.Value];
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
                Helper.EnsureThreadLocalized();
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

        public void ClearTasks()
        {
            tasksData.Clear();
            onTaskListChanged();
        }
    }
}
