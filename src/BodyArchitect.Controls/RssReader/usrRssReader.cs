using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using DevExpress.Utils.Controls;
using DevExpress.XtraEditors;
using System.Net;
using BodyArchitect.Logger;
using BodyArchitect.Controls.Localization;
using System.Threading;
using BodyArchitect.Common;

namespace BodyArchitect.Controls.RssReader
{
    public partial class usrRssReader : usrBaseControl
    {
        List<SyndicationItem> feeds = new List<SyndicationItem>();

        public usrRssReader()
        {
            InitializeComponent();
        }

        public void Fill()
        {
            progressIndicator1.Visible = true;
            progressIndicator1.Start();
            ThreadPool.QueueUserWorkItem(asyncRetrievingRssFeed);
        }

        void setMessage(ControlBase ctrl,string message)
        {
            if(InvokeRequired)
            {
                BeginInvoke(new Action<ControlBase,string>(setMessage),ctrl, message);
            }
            else
            {
                ctrl.Text = message;
            }
        }

        SyndicationItem getNewsLastFeed()
        {
            try
            {
                WebClient client = new WebClient();
                client.Proxy = null;
                client.Headers.Add("User-agent", "BodyArchitect .NET Client");
                using (var stream = client.OpenRead(getFeedUrl(UserContext.Settings.RssNewsChannelAddress)))
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
                using (var stream = client.OpenRead(getFeedUrl(UserContext.Settings.RssTipsChannelAddress)))
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
                ControlHelper.EnsureThreadLocalized();
                string header = ApplicationStrings.RssHeaderNews;
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
                int index=rand.Next(feeds.Count);
                var itemToDisplay = feeds[index];
                ParentWindow.SynchronizationContext.Send(delegate
                {
                    if (itemToDisplay.Links.Count > 0)
                    {
                        hlRssTitle.Tag = itemToDisplay.Links[0];
                    }
                    string title = itemToDisplay.Title.Text;
                    TextSyndicationContent content = itemToDisplay.Summary ?? itemToDisplay.Content as TextSyndicationContent;
                    if (content != null)
                    {
                        setMessage(lblRssDescription, content.Text);
                    }
                    if (itemToDisplay.Categories.Count > 0 && itemToDisplay.Categories[0].Name!="BA")
                    {
                        header=ApplicationStrings.RssHeaderTips;
                        title = string.Format(ApplicationStrings.usrRssReader_TipTitle,index+1,feeds.Count,title);
                    }
                    setMessage(hlRssTitle, title);
                    grHeadline.Text = header;
                    //run timer (automatic change tips) when there are more items to show
                    timer1.Enabled = feeds.Count > 1;
                }, null);
            }
            catch (WebException webEx)
            {
                ExceptionHandler.Default.Process(webEx, ApplicationStrings.ErrorRetrievingRssFeed, ErrorWindow.None);
                setMessage(lblRssDescription,ApplicationStrings.ErrorRetrievingRssFeed);
            }
            finally
            {
                try
                {
                    ParentWindow.SynchronizationContext.Send(delegate
                                                                 {
                        progressIndicator1.Visible = false;
                        progressIndicator1.Stop();
                    }, null);
                }
                catch (Exception)
                {
                }
                
            }
        }

        string getFeedUrl(string baseUrl)
        {
            if(Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName=="pl")
            {
                return string.Format(baseUrl, "_pl");
            }
            return string.Format(baseUrl,"");
        }
        private void hlRssTitle_OpenLink(object sender, DevExpress.XtraEditors.Controls.OpenLinkEventArgs e)
        {
            var url = (SyndicationLink)hlRssTitle.Tag;
            if (url==null)
            {
                return;
            }
            try
            {
                System.Diagnostics.Process.Start(url.Uri.ToString());
            }
            catch (Exception)
            {
                
            }
            
        }

        private void grHeadline_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Fill();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Fill();
        }
    }
}
