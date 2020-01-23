using System.Windows;
using System.Windows.Shell;

namespace BodyArchitect.Client.UI.WindowsSevenIntegration
{
    public static class TaskbarProgressWrapper
    {
        private static TaskbarItemInfo _taskbar;
        private static bool _initialized = false;

        /// <summary>
        /// Enum definition of possible Taskbar progress states
        /// </summary>
        public enum State
        {
            NoProgress,
            Normal,
            Indeterminate,
            Error,
            Paused
        }
        
        public static void Initialize(Window wnd)
        {
            if(!_initialized)
            {
                _taskbar = wnd.TaskbarItemInfo = new TaskbarItemInfo();
                _taskbar.ProgressState = TaskbarItemProgressState.None;
                _initialized = true;
            }
        }

        /// <summary>
        /// Update Taskbar progress state and value
        /// </summary>
        /// <param name="state">Progress <see cref="State"/></param>
        /// <param name="value">Double value of progress. Should be between 0 and 1.</param>
        public static void UpdateProgressState(State state, double value = 0)
        {
            if (!_initialized) return;
            switch (state)
            {
                case State.Normal:
                    _taskbar.ProgressState = TaskbarItemProgressState.Normal;
                    _taskbar.ProgressValue = (value >= 1 || value <= 0) ? 1 : value;
                    break;
                case State.Indeterminate:
                    _taskbar.ProgressState = TaskbarItemProgressState.Indeterminate;
                    _taskbar.ProgressValue = (value >= 1 || value <= 0) ? 1 : value;
                    break;
                case State.Error:
                    _taskbar.ProgressState = TaskbarItemProgressState.Error;
                    _taskbar.ProgressValue = (value >= 1 || value <= 0) ? 1 : value;
                    break;
                case State.Paused:
                    _taskbar.ProgressState = TaskbarItemProgressState.Paused;
                    _taskbar.ProgressValue = (value >= 1 || value <= 0) ? 1 : value;
                    break;
                case State.NoProgress:
                    _taskbar.ProgressState = TaskbarItemProgressState.None;
                    _taskbar.ProgressValue = 0;
                    break;
            }
        }
    }
}
