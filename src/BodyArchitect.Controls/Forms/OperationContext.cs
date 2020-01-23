using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BodyArchitect.Controls.UserControls;

namespace BodyArchitect.Controls.Forms
{
    public enum OperationState
    {
        Started,
        Ended,
        Cancelled,
        Failed
    }

    public class OperationContext
    {
        private volatile Exception returnedException;
        private CancellationToken cancellatioToken;
        private volatile OperationState state;
        private volatile Task currentTask;
        private Guid guid = Guid.NewGuid();
        private TasksManager taskManager;

        public OperationContext(TasksManager taskManager)
        {
            this.taskManager = taskManager;
        }

        public CancellationToken CancellatioToken
        {
            get { return cancellatioToken; }
            set { cancellatioToken = value; }
        }

        public Exception ReturnedException
        {
            get { return returnedException; }
            set
            {
                returnedException = value;
                state = OperationState.Failed;
            }
        }

        public OperationState State
        {
            get { return state; }
            set { state = value; }
        }

        public Task CurrentTask
        {
            get { return currentTask; }
            set { currentTask = value; }
        }

        public TasksManager TaskManager
        {
            get { return taskManager; }
        }
    }
}
