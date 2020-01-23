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
using BodyArchitect.Client.Module.A6W.Localization;
using BodyArchitect.Service.V2.Model;
using Visiblox.Charts;

namespace BodyArchitect.Client.Module.A6W.Controls
{
    /// <summary>
    /// Interaction logic for usrA6WProgress.xaml
    /// </summary>
    public partial class usrA6WProgress
    {
        public usrA6WProgress()
        {
            InitializeComponent();
        }

        public override void Fill(EntryObjectDTO entry)
        {
            generateProgress((A6WEntryDTO) entry);
        }

        void generateProgress(A6WEntryDTO entry)
        {

            var series = new DataSeries<string, double>();
            series.Add(new DataPoint<string, double>(A6WEntryStrings.usrA6WProgress_Completed, entry.MyTraining.PercentageCompleted));
            series.Add(new DataPoint<string, double>(A6WEntryStrings.usrA6WProgress_NotCompleted, 100 - entry.MyTraining.PercentageCompleted));
            MainChart.DataSeries = series;
        }
    }
}
