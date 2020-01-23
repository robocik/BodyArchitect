using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Shared
{
    public interface ITimerService
    {
        DateTime UtcNow { get; }
    }

    public class TimerService : ITimerService
    {
        public DateTime UtcNow
        {
            get { return DateTime.UtcNow; }
        }
    }
}
