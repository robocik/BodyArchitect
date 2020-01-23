using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Shared;

namespace BodyArchitect.UnitTests.DbConverter.V4V5
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
