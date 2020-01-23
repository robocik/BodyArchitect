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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BodyArchitect.WCF;
using BodyArchitect.Controls.UserControls;
using BodyArchitect.Service.Model;
using System.Collections.ObjectModel;

namespace BodyArchitect.Module.Blog.Controls.WPF
{
    /// <summary>
    /// Interaction logic for BlogCommentsList.xaml
    /// </summary>
    public partial class BlogCommentsList : UserControl
    {
        BlogEntryDTO entry;

        public BlogCommentsList()
        {
            InitializeComponent();
        }

        public void Fill(BlogEntryDTO entry)
        {
            this.entry = entry;
            fillPage(0);
        }

        private void buildPager(int allItems,int pageSize,int currentPageIndex)
        {
            //int pageCount = (int)((float)allItems / pageSize +1);
            int pageCount = (int) Math.Ceiling((double) allItems/pageSize);
            //if (pagerPanel.Children.Count != pageCount)
            {
                pagerPanel.Children.Clear();
                for (int i = 0; i < pageCount; i++)
                {
                    Button btn = new Button();
                    btn.Template =(ControlTemplate) this.Resources["hyperLinkButton"];
                    btn.Style = (Style)this.Resources["hyperLinkButtonStyle"];
                    btn.Content = i;
                    btn.Margin=new Thickness(3,0,0,0);
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

        #region Spinner progress indicator

        void startProgress(bool start)
        {
            if (start)
            {
                if (canvas2.Visibility == System.Windows.Visibility.Hidden)
                {
                    drawCanvas();
                    canvas2.Visibility = Visibility.Visible;
                    DoubleAnimation a = new DoubleAnimation();
                    a.From = 0;
                    a.To = 360;
                    a.RepeatBehavior = RepeatBehavior.Forever;
                    a.SpeedRatio = 1;

                    spin.BeginAnimation(RotateTransform.AngleProperty, a);
                }
            }
            else
            {
                canvas2.Visibility = Visibility.Hidden;
            }
        }

        //Spinner draw
        void drawCanvas()
        {
            for (int i = 0; i < 12; i++)
            {
                Line line = new Line()
                {
                    X1 = 15,
                    X2 = 15,
                    Y1 = 0,
                    Y2 = 20,
                    StrokeThickness = 1,
                    Stroke = Brushes.Gray,
                    Width = 30,
                    Height = 30
                };
                line.VerticalAlignment = VerticalAlignment.Center;
                line.HorizontalAlignment = HorizontalAlignment.Center;
                line.RenderTransformOrigin = new Point(.5, .5);
                line.RenderTransform = new RotateTransform(i * 30);
                line.Opacity = (double)i / 12;

                canvas1.Children.Add(line);

            }
        }

        #endregion

        private CancellationTokenSource tokenSource;
        void fillPage(int pageIndex)
        {
            if(tokenSource!=null)
            {
                tokenSource.Cancel();
                tokenSource = null;
            }
            tokenSource = new CancellationTokenSource();
            startProgress(true);
            var task = Task.Factory.StartNew(delegate(object cancellationToken)
                                                 {
                                                     CancellationToken cancelToken =(CancellationToken) cancellationToken;
                           PartialRetrievingInfo info = new PartialRetrievingInfo();
                           info.PageIndex = pageIndex;
                           var result = ServiceManager.GetBlogComments(entry, info);
                           if (cancelToken.IsCancellationRequested)
                           {
                               //lstComments.Dispatcher.Invoke(new Action(delegate
                               //{
                               //    startProgress(false);
                               //}));
                               return;
                           }
                           lstComments.Dispatcher.Invoke(new Action(delegate
                                                             {
                                                                 buildPager(result.AllItemsCount, info.PageSize, pageIndex);
                                                                 lstComments.ItemsSource = result.Items;
                                                                 startProgress(false);
                                                             }));

                       }, tokenSource.Token, tokenSource.Token);

        }

        private void btnPage_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)e.OriginalSource;

            fillPage((int)btn.Tag);
        }
    }
}
