using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Xceed.Wpf.Toolkit;

namespace BodyArchitect.Client.UI.Controls
{
    public class MyMultiLineTextEditor : MultiLineTextEditor
    {
        public MyMultiLineTextEditor()
        {
            Loaded += new RoutedEventHandler(MyMultiLineTextEditor_Loaded);
        }

        void MyMultiLineTextEditor_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox p = (TextBox)Template.FindName("PART_TextBox", this);
            var popup = (Popup)((FrameworkElement)((FrameworkElement)p.Parent).Parent).Parent;

            Binding binding = new Binding("IsKeyboardFocused");
            binding.Source = p;
            popup.SetBinding(Popup.StaysOpenProperty, binding);
            if (popup != null)
            {
                popup.Opened += delegate
                {
                    p.Focus();
                };
            }
        }
    }
}
