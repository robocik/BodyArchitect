using System;
using System.Windows;

namespace BodyArchitect.WP7.UserControls
{
    public partial class MeasurementsTimeControl
    {
        private bool readOnly;

        public MeasurementsTimeControl()
        {
            InitializeComponent();
        }

        public bool ReadOnly
        {
            get { return readOnly; }
            set
            {
                readOnly = value;
                updateReadOnly();
            }
        }

        void updateReadOnly()
        {
            tpMeasurementsTime.IsHitTestVisible = !ReadOnly;
            hlNow.IsEnabled = !ReadOnly;
            lpTimeType.IsEnabled = !ReadOnly;
        }
        private void btnEndTraining_Click(object sender, RoutedEventArgs e)
        {
            tpMeasurementsTime.Value = DateTime.Now;
        }
    }
}
