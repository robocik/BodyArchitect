using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
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
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.UserControls;
using BodyArchitect.Client.UI.Views;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Reports;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls
{
    [Export(typeof(IUserDetailControlBuilder))]
    public class usrUserRecordsBuilder : IUserDetailControlBuilder
    {
        public IUserDetailControl Create()
        {
            return new usrUserRecords();
        }
    }

    public partial class usrUserRecords : IUserDetailControl
    {
        ObservableCollection<ExerciseRecordViewModel> records = new ObservableCollection<ExerciseRecordViewModel>();
        public usrUserRecords()
        {
            InitializeComponent();

            DataContext = this;
        }

        public void Fill(ProfileInformationDTO profileInfo, bool isActive)
        {
            records.Clear();
            if (isActive && profileInfo != null )
            {
                foreach (var record in profileInfo.Records)
                {
                    Records.Add(new ExerciseRecordViewModel(record,null,0));
                }
            }

        }

        public ObservableCollection<ExerciseRecordViewModel> Records
        {
            get { return records; }
        }
        public string Caption
        {
            get { return EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.StrengthTraining:StrengthTrainingEntryStrings:usrUserRecords_Header"); }
        }

        public ImageSource SmallImage
        {
            get
            {
                BitmapImage source = "Records16.png".ToResourceUrl().ToBitmap();
                return source;
            }
        }

        public bool UpdateGui(ProfileInformationDTO user)
        {
            return user != null && user.Records.Count>0;
        }

        private void btnTrainingDayInfo_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            ExerciseRecordsReportResultItem item = (ExerciseRecordsReportResultItem)btn.Tag;
            MainWindow.Instance.ShowTrainingDayReadOnly(item.TrainingDate, item.User, item.CustomerId);
        }

        private void btnShowExerciseRecords_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            ExerciseRecordsReportResultItem item = (ExerciseRecordsReportResultItem)btn.Tag;

            MainWindow.Instance.ShowPage(
                new Uri("pack://application:,,,/BodyArchitect.Client.Module.StrengthTraining;component/Controls/ExercisesView.xaml"),
                () => new PageContext()
                          {
                              SelectedItem = item.Exercise.GlobalId,
                              DisplayMode = 2
                          });
        }
    }
}
