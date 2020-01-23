using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Client.UI.Controls
{
    public class ControlValidatedEventArgs : EventArgs
    {
        private bool isValid;

        public ControlValidatedEventArgs(bool isValid)
        {
            this.isValid = isValid;
        }

        public bool IsValid
        {
            get { return isValid; }
        }
    }
}
