using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls
{
    /// <summary>
    /// Interaction logic for SetRestTimeWindow.xaml
    /// </summary>
    public partial class SetRestTimeWindow
    {
        public SetRestTimeWindow()
        {
            InitializeComponent();
            txtRestTime.Value = DateTime.Today+System.TimeSpan.FromSeconds(90);
        }

        public TimeSpan TimeSpan
        {
            get { return txtRestTime.Value.Value.TimeOfDay; }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
