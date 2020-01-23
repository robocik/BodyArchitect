using System;
using System.Collections.Generic;
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
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.UI.UserControls
{
    /// <summary>
    /// Interaction logic for BlogCommentsList.xaml
    /// </summary>
    public partial class BlogCommentsList
    {
        TrainingDayInfoDTO day;

        public BlogCommentsList()
        {
            InitializeComponent();
        }

        public void Fill(TrainingDayInfoDTO day)
        {
            this.day = day;
            fillPage(0);
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
            Task.Factory.StartNew(delegate(object cancellationToken)
            {
                CancellationToken cancelToken = (CancellationToken)cancellationToken;
                PartialRetrievingInfo info = new PartialRetrievingInfo();
                info.PageIndex = pageIndex;
                var result = ServiceManager.GetBlogComments(day, info);
                if (cancelToken.IsCancellationRequested)
                {
                    return;
                }
                lstComments.Dispatcher.BeginInvoke(new Action(delegate
                {
                    buildPager(result.AllItemsCount, info.PageSize, pageIndex);
                    lstComments.ItemsSource = result.Items;
                    progressIndicator.IsRunning = false;
                }));

            }, tokenSource.Token, tokenSource.Token);

        }

        private void btnPage_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)e.OriginalSource;

            fillPage((int)btn.Tag);
        }

        private void lblUserName_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)e.OriginalSource;
            UserDTO user = (UserDTO)btn.Tag;
            if (!user.IsDeleted && !user.IsMe())
            {
                MainWindow.Instance.ShowUserInformation(user);
            }
        }
    }
}
