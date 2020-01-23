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

namespace BodyArchitect.Client.Module.GPSTracker.Controls
{
    /// <summary>
    /// Interaction logic for usrLapPushpinInfo.xaml
    /// </summary>
    public partial class usrLapPushpinInfo
    {
        public usrLapPushpinInfo(LapViewModel lap)
        {
            InitializeComponent();
            DataContext = lap;
        }
    }
}
