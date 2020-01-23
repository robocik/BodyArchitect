using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BodyArchitect.Common.Plugins
{
    public interface IOptionsControl
    {
        void Save();
        void Fill();
        bool RestartRequired { get; }
        string Title { get; }
        Image Image { get; }
    }
}
