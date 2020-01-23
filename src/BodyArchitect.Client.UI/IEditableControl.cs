using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Client.UI
{
    public interface IEditableControl
    {
        object Object { get; set; }
        bool ReadOnly { get; set; }
        object Save();
    }
}
