using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using AvalonDock;
using AvalonDock.Layout;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Logger;

namespace BodyArchitect.Client.UI.UserControls
{
    /// <summary>
    /// Interaction logic for RssReader.xaml
    /// </summary>
    public partial class RssReader
    {
        List<SyndicationItem> feeds = new List<SyndicationItem>();
        private bool _isProgress;
        private System.Timers.Timer timer1;
        private int currentIndex;

        public RssReader()
        {
            InitializeComponent();
            DataContext = this;
            timer1=new System.Timers.Timer(TimeSpan.FromMinutes(10).TotalMilliseconds);
            timer1.Enabled = false;
            timer1.Elapsed+=timer1_Elapsed;
        }

        void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Fill();
        }

        public bool IsProgress
        {
            get { return _isProgress; }
            set
            {
                _isProgress = value;
                NotifyOfPropertyChange(()=>IsProgress);
            }
        }

        public void Fill()
        {
            IsProgress = true;
            ThreadPool.QueueUserWorkItem(asyncRetrievingRssFeed);
        }


        SyndicationItem getNewsLastFeed()
        {
            try
            {
                
                WebClient client = new WebClient();
                client.Proxy = null;
                client.Headers.Add("User-agent", "BodyArchitect .NET Client");
                using (var stream = client.OpenRead(getFeedUrl(UserContext.Current.Settings.RssNewsChannelAddress)))
                {
                    var feed = SyndicationFeed.Load(XmlReader.Create(stream));
                    return feed.Items.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex);
                return null;
            }

        }

        IList<SyndicationItem> getRandomTipsFeed()
        {
            try
            {
                List<SyndicationItem> itemList = new List<SyndicationItem>();
                WebClient client = new WebClient();
                client.Proxy = null;
                client.Headers.Add("User-agent", "BodyArchitect .NET Client");
                using (var stream = client.OpenRead(getFeedUrl(UserContext.Current.Settings.RssTipsChannelAddress)))
                {
                    //get random tip
                    var feed = SyndicationFeed.Load(XmlReader.Create(stream));
                    itemList.AddRange(feed.Items);
                    return itemList;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex);
                return new List<SyndicationItem>();
            }

        }

        void asyncRetrievingRssFeed(object state)
        {
            try
            {
                Helper.EnsureThreadLocalized();
                
                if (feeds.Count == 0)
                {

                    //first we retrieve the news feed
                    var item = getNewsLastFeed();
                    if (item != null)
                    {
                        feeds.Add(item);
                    }
                    if (feeds.Count == 0 || (DateTime.Now - item.PublishDate.DateTime).Days > 5)
                    {
                        //if the news is older then 5 days we show random tips
                        var tmp = getRandomTipsFeed();
                        feeds.AddRange(tmp);
                        //if(tmp!=null)
                        //{//we must be sure that tip exists
                        //    item = tmp;
                        //    header=ApplicationStrings.RssHeaderTips;
                        //}
                    }
                    if (feeds.Count == 0)
                    {
                        return;
                    }
                }
                Random rand = new Random();
                currentIndex = rand.Next(feeds.Count);
                var itemToDisplay = feeds[currentIndex];
                UIHelper.Invoke(()=>
                                         {
                                             fillEntry(itemToDisplay);
                                             //run timer (automatic change tips) when there are more items to show
                                             timer1.Enabled = feeds.Count > 1;
                                         },Dispatcher);

            }
            catch (WebException webEx)
            {
                ExceptionHandler.Default.Process(webEx, Strings.ErrorRetrievingRssFeed, ErrorWindow.None);
                UIHelper.BeginInvoke(()=> MakeFormattedTextBlock(lblRssDescription, Strings.ErrorRetrievingRssFeed),Dispatcher);
            }
            finally
            {
                try
                {
                    IsProgress = false;
                }
                catch (Exception)
                {
                }

            }
        }

        private void fillEntry(SyndicationItem itemToDisplay)
        {
            string header = Strings.RssHeaderNews;
            if (itemToDisplay.Links.Count > 0)
            {
                hlRssTitle.Tag = itemToDisplay.Links[0];
                hlRssTitle.IsHitTestVisible = true;
            }
            else
            {
                hlRssTitle.IsHitTestVisible = false;
            }
            string title = itemToDisplay.Title.Text;
            TextSyndicationContent content = itemToDisplay.Summary ?? itemToDisplay.Content as TextSyndicationContent;
            if (content != null)
            {
                MakeFormattedTextBlock(lblRssDescription, content.Text);
            }
            if (itemToDisplay.Categories.Count > 0 && itemToDisplay.Categories[0].Name != "BA")
            {
                header = Strings.RssHeaderTips;
                title = string.Format(Strings.usrRssReader_TipTitle, currentIndex + 1, feeds.Count, title);
            }
            hlRssTitle.Content = title;
            var toolsPane = MainWindow.Instance.GetAnchorable("RssReader");
            if (toolsPane != null)
            {
                toolsPane.Title = header;
            }
        }

        

        private TextBlock MakeFormattedTextBlock(TextBlock tb,string shtml)
        {
            tb.Inlines.Clear();
            Run temprun = new Run();

            int bold = 0;
            int italic = 0;

            do
            {
                if ((shtml.StartsWith("<b>")) | (shtml.StartsWith("<i>")) |
                    (shtml.StartsWith("</b>")) | (shtml.StartsWith("</i>")))
                {
                    bold += (shtml.StartsWith("<b>") ? 1 : 0);
                    italic += (shtml.StartsWith("<i>") ? 1 : 0);
                    bold -= (shtml.StartsWith("</b>") ? 1 : 0);
                    italic -= (shtml.StartsWith("</i>") ? 1 : 0);
                    shtml = shtml.Remove(0, shtml.IndexOf('>') + 1);
                    if (temprun.Text != null)
                        tb.Inlines.Add(temprun);
                    temprun = new Run();
                    temprun.FontWeight = ((bold > 0) ? FontWeights.Bold : FontWeights.Normal);
                    temprun.FontStyle = ((italic > 0) ? FontStyles.Italic : FontStyles.Normal);
                }
                else // just a piece of plain text
                {
                    int nextformatthing = shtml.IndexOf('<');
                    if (nextformatthing < 0) // there isn't any more formatting
                        nextformatthing = shtml.Length;
                    temprun.Text += shtml.Substring(0, nextformatthing);
                    shtml = shtml.Remove(0, nextformatthing);
                }
            } while (shtml.Length > 0);
            // Flush the last buffer
            if (temprun.Text != null)
                tb.Inlines.Add(temprun);
            return tb;
        }

        string getFeedUrl(string baseUrl)
        {
            if (Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "pl")
            {
                return string.Format(baseUrl, "_pl");
            }
            return string.Format(baseUrl, "");
        }

        private void btnTitle_Click(object sender, RoutedEventArgs e)
        {
            var url = (SyndicationLink)hlRssTitle.Tag;
            if (url!=null)
            {
                Helper.OpenUrl(url.Uri.ToString());
            }
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (currentIndex == 0)
            {
                currentIndex = feeds.Count;
            }
            currentIndex--;
            var itemToDisplay = feeds[currentIndex];
            fillEntry(itemToDisplay);
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (currentIndex == feeds.Count-1)
            {
                currentIndex = -1;
            }

            currentIndex++;
            var itemToDisplay = feeds[currentIndex];
            fillEntry(itemToDisplay);
        }
    }
}
