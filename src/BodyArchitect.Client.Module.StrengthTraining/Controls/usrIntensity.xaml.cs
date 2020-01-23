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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls
{
    /// <summary>
    /// Interaction logic for usrIntensity.xaml
    /// </summary>
    public partial class usrIntensity
    {
        public event EventHandler IntensityChanged;

        public usrIntensity()
        {
            InitializeComponent();
            updateLabels();
        }


        public Intensity Intensity
        {
            get { return (Intensity)ztbIntensive.Value; }
            set
            {
                ztbIntensive.Value = (int)value;
                updateLabels();
            }
        }

        public bool ReadOnly
        {
            get { return !ztbIntensive.IsEnabled; }
            set { ztbIntensive.IsEnabled = !value; }
        }


        void onIntensityChanged()
        {
            if(IntensityChanged!=null)
            {
                IntensityChanged(this, EventArgs.Empty);
            }
        }
        void updateLabels()
        {
            lblNotSet.SetVisible(((Intensity)ztbIntensive.Value) == Intensity.NotSet);
            lblLow.SetVisible(((Intensity)ztbIntensive.Value) == Intensity.Low);
            lblMedium.SetVisible(((Intensity)ztbIntensive.Value) == Intensity.Medium);
            lblHight.SetVisible(((Intensity)ztbIntensive.Value) == Intensity.Hight);
        }

        private void ztbIntensive_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            updateLabels();
            onIntensityChanged();
        }
    }
}
