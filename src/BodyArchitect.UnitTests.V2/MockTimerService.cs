using System;
using BodyArchitect.Shared;

namespace BodyArchitect.UnitTests.V2
{
    public class MockTimerService : ITimerService
    {
        private DateTime? _utcNow;
        public DateTime UtcNow
        {
            get
            {
                if (!_utcNow.HasValue)
                {
                    return DateTime.UtcNow;
                }
                return _utcNow.Value;
            }
            set { _utcNow = value; }
        }
    }
}
