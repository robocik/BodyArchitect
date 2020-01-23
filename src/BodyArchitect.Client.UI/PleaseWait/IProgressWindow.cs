using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Client.UI
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

        void ShowProgress(bool canBeCancelled=true,int? maxProgress=null);
    }
}
