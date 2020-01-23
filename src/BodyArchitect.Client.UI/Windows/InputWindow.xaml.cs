using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace BodyArchitect.Client.UI.Windows
{
    /// <summary>
    /// Interaction logic for InputWindow.xaml
    /// </summary>
    public partial class InputWindow
    {
        public InputWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public InputWindow(bool isRequired):this()
        {
            IsRequired = isRequired;
        }

        public bool IsRequired { get; private set; }


        public string Value
        {
            get
            {
                return txtValue.Text;
            }
            set
            {
                txtValue.Text = value;
                btnOK.IsEnabled = !IsRequired || !string.IsNullOrWhiteSpace(value);
            }
        }

        [DefaultValue(0)]
        public int MaxLength
        {
            get { return txtValue.MaxLength; }
            set { txtValue.MaxLength = value; }
        }

        private void btnOK_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Window_Loaded_1(object sender, System.Windows.RoutedEventArgs e)
        {
            Keyboard.Focus(txtValue);
        }
    }
}
