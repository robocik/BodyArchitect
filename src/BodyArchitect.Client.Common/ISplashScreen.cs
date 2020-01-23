using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Client.Common
{
    public interface ISplashScreen
    {
        void SetStatus(string message);

        void SetProgressBar(bool visible, int max, int value);
    }
}
