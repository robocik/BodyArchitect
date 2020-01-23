using System.Windows.Input;
using Microsoft.Phone.Controls;

namespace BodyArchitect.WP7.Controls
{
    public class NumericTextBox : PhoneTextBox
    {
        public NumericTextBox()
        {
            InputScope = new InputScope() { Names = { new InputScopeName() { NameValue = InputScopeNameValue.TelephoneNumber } } };
            GotFocus += delegate
                            {
                                SelectAll();
                            };
        }

        public bool AllowDecimals { get; set; }

        private void maskNumericInput()
        {
            string[] invalidcharacters = { "*", "#", ",", "(", ")", "X", "x", "-", "+", " ", "@" };
            if (!AllowDecimals)
            {
                invalidcharacters[invalidcharacters.Length - 1] = ".";
            }
            for (int i = 0; i < invalidcharacters.Length; i++)
            {
                Text = Text.Replace(invalidcharacters[i],"");
            }
            SelectionStart = Text.Length;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.Key != Key.Unknown)
            {
                maskNumericInput();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if(Text.Contains(".") && e.PlatformKeyCode==190)
            {
                e.Handled = true;
            }
        }
    }
}
