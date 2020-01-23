using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Cache;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Views;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Service.V2.Model.Reports;
using BodyArchitect.Shared;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls
{
    /// <summary>
    /// Interaction logic for usrExerciseRecords.xaml
    /// </summary>
    public partial class usrExerciseRecords
    {
        private ExerciseLightDTO exercise;

        public usrExerciseRecords()
        {
            InitializeComponent();
        }

        private void btnUserInfo_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            ExerciseRecordsReportResultItem item = (ExerciseRecordsReportResultItem)btn.Tag;
            if (item.CustomerId.HasValue)
            {
                var customer = CustomersReposidory.Instance.GetItem(item.CustomerId.Value);
                MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Controls/Customers/CustomersView.xaml"), () => new PageContext() { Customer = customer });
            }
            if (!item.User.IsDeleted && !item.User.IsMe())
            {
                MainWindow.Instance.ShowUserInformation(item.User);
            }
            
        }

        public RecordMode RecordMode
        {
            get { return (RecordMode)GetValue(RecordModeProperty); }
            set
            {
                SetValue(RecordModeProperty, value);
            }
        }


        public static readonly DependencyProperty RecordModeProperty =
            DependencyProperty.Register("RecordMode", typeof(RecordMode), typeof(usrExerciseRecords), new UIPropertyMetadata(RecordMode.AllUsers, OnRecordModeChanged));

        private static void OnRecordModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (usrExerciseRecords)d;

            ctrl.Fill(ctrl.exercise);
        }

        public void Fill(ExerciseLightDTO exercise)
        {
            this.exercise = exercise;
            if (exercise != null)
            {
                fillPage(0);
            }
        }

        private void buildPager(int allItems, int pageSize, int currentPageIndex)
        {
            //int pageCount = (int)((float)allItems / pageSize +1);
            int pageCount = (int)Math.Ceiling((double)allItems / pageSize);
            //if (pagerPanel.Children.Count != pageCount)
            {
                pagerPanel.Children.Clear();
                for (int i = 0; i < pageCount; i++)
                {
                    Button btn = new Button();
                    btn.Style = (Style)this.FindResource("LinkButon");
                    btn.Content = i;
                    btn.Margin = (Thickness)this.FindResource("MarginSmallLeft");
                    btn.Click += new RoutedEventHandler(btnPage_Click);
                    btn.Tag = i;
                    if (currentPageIndex == i)
                    {
                        btn.IsEnabled = false;
                        btn.Foreground = Brushes.DarkGray;
                    }
                    pagerPanel.Children.Add(btn);
                }
            }
        }

        private CancellationTokenSource tokenSource;
        void fillPage(int pageIndex)
        {
            if (tokenSource != null)
            {
                tokenSource.Cancel();
                tokenSource = null;
            }
            tokenSource = new CancellationTokenSource();
            progressIndicator.IsRunning = true;
            var recordsForUsers = RecordMode;
            Task.Factory.StartNew(delegate(object cancellationToken)
            {
                CancellationToken cancelToken = (CancellationToken)cancellationToken;
                PartialRetrievingInfo info = new PartialRetrievingInfo();
                info.PageIndex = pageIndex;
                var param = new ExerciseRecordsParams();
                param.ExerciseId = exercise.GlobalId;
                param.Mode = recordsForUsers;
                var result = ServiceManager.ReportExerciseRecords(param, info);
                if (cancelToken.IsCancellationRequested)
                {
                    return;
                }
                lstRecords.Dispatcher.BeginInvoke(new Action(delegate
                {
                    buildPager(result.AllItemsCount, info.PageSize, pageIndex);
                    lstRecords.ItemsSource = result.Items.Select(x => new ExerciseRecordViewModel(x, result, info.PageSize));
                    progressIndicator.IsRunning = false;
                }));

            }, tokenSource.Token, tokenSource.Token);

        }

        private void btnPage_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)e.OriginalSource;

            fillPage((int)btn.Tag);
        }

        public void ClearContent()
        {
            lstRecords.ItemsSource = null;
            pagerPanel.Children.Clear();
        }

        private void btnTrainingDayInfo_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            ExerciseRecordsReportResultItem item = (ExerciseRecordsReportResultItem)btn.Tag;
            MainWindow.Instance.ShowTrainingDayReadOnly(item.TrainingDate,item.User,item.CustomerId);
        }
    }

    //public class ExerciseRecordViewModel:ViewModelBase
    //{
    //    private ExerciseRecordsReportResultItem item;
    //    private PagedResult<ExerciseRecordsReportResultItem> result;
    //    private int pageSize;

    //    public ExerciseRecordViewModel(ExerciseRecordsReportResultItem item, PagedResult<ExerciseRecordsReportResultItem> result,int pageSize)
    //    {
    //        this.pageSize = pageSize;
    //        this.item = item;
    //        this.result = result;
    //    }

    //    public bool CalendarAvailable
    //    {
    //        get { return item.User.IsMe() || item.User.HaveAccess(item.User.Privacy.CalendarView); }
    //    }

    //    public ExerciseRecordsReportResultItem Item
    //    {
    //        get { return item; }
    //    }

    //    public string CardioValue
    //    {
    //        get
    //        {
    //            var time = TimeSpan.FromSeconds((double)item.MaxWeight);
    //            return time.ToString();
    //        }
    //    }

    //    public bool IsCardio
    //    {
    //        get { return item.Exercise.ExerciseType == ExerciseType.Cardio; }
    //    }

    //    public int Position
    //    {
    //        get
    //        {
    //            return (result.PageIndex*pageSize)+result.Items.IndexOf(item)+1;
    //        }
    //    }

    //    public decimal Weight
    //    {
    //        get { return item.MaxWeight.ToDisplayWeight(); }
    //    }

    //    public string WeightType
    //    {
    //        get
    //        {
    //            if (UserContext.Current.ProfileInformation.Settings.WeightType == BodyArchitect.Service.V2.Model.WeightType.Pounds)
    //            {
    //                return Strings.WeightType_Pound;
    //            }
    //            else
    //            {
    //                return Strings.WeightType_Kg;
    //            }
    //        }
    //    }
    //}
}
