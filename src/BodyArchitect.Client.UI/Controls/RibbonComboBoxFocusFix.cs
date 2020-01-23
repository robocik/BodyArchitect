using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using BodyArchitect.Client.Common;
using Microsoft.Windows.Controls.Ribbon;

namespace BodyArchitect.Client.UI.Controls
{
    public class RibbonComboBoxFocusFix : RibbonComboBox
    {
        protected override void OnIsKeyboardFocusWithinChanged(System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (IsFocused == false)
            {
                base.OnIsKeyboardFocusWithinChanged(e);
            }
        }
    }

}
