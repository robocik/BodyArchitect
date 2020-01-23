using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Common.ProgressOperation
{
    public interface IProgressWindow
    {
        void SetMessage(string message);
        void SetProgress(int value);

        MethodParameters Parameters
        {
            get;
            set;
        }

        void ThreadSafeClose();
    }
}
