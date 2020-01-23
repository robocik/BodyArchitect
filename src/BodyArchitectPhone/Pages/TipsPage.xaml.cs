using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.Controls.Animations;
using BodyArchitect.WP7.Controls.Cache;
using Microsoft.Phone.Controls;

namespace BodyArchitect.WP7.Pages
{
    public partial class TipsPage : AnimatedBasePage
    {
        private List<SyndicationItem> items;
        private int position;
        

        public TipsPage()
        {
            InitializeComponent();
            AnimationContext = LayoutRoot;
        }

        protected override AnimatorHelperBase GetAnimation(AnimationType animationType, Uri toOrFrom)
        {
            if (animationType == AnimationType.NavigateForwardIn || animationType == AnimationType.NavigateBackwardIn)
                return new SlideUpAnimator() { RootElement = LayoutRoot };
            else
                return new SlideDownAnimator() { RootElement = LayoutRoot };
        }


        public string RssTipsChannelAddress
        {
            get
            {
                return "http://service.bodyarchitectonline.com/Rss/tips{0}.rss";
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            
            StateHelper stateHelper = new StateHelper(this.State);
            var pivotItem = stateHelper.GetValue<int>("Position", -1);
            position = pivotItem;

            enusureRssFeed();
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.State["Position"] = position;
            base.OnNavigatedFrom(e);
        }

        void showItem()
        {
            if(position==-1)
            {
                Random rand = new Random();
                position = rand.Next(items.Count-1);
            }
        
            var item = items[position];
            this.PageTitle.Text=item.Title.Text;
            PageTipNumber.Text = string.Format("({0}/{1})",position+1,items.Count);
            TextSyndicationContent content = item.Summary ?? item.Content as TextSyndicationContent;
            htmlBlock.MakeFormattedTextBlock(content.Text);
        }


        

        private void GestureListener_Flick(object sender, FlickGestureEventArgs e)
        {
            if (e.Direction == System.Windows.Controls.Orientation.Horizontal && items != null && items.Count>0)
            {
                bool increase = e.HorizontalVelocity > 0;//? "Right" : "Left";

                SlideTransition sTx = new SlideTransition();
                sTx.Mode = !increase ? SlideTransitionMode.SlideLeftFadeIn : SlideTransitionMode.SlideRightFadeIn;
                ITransition transition = sTx.GetTransition(htmlBlock);
                transition.Completed += delegate
                {
                    transition.Stop();
                };
                transition.Begin();


                
                if(!increase)
                {
                    if (position<items.Count-1)
                    {
                        position++;    
                    }
                    else
                    {
                        position = 0;//we do a loop
                    }
                }
                else
                {
                    if (position >0)
                    {
                        position--;
                    }
                    else
                    {
                        position = items.Count - 1;//we do a loop
                    }
                }
                showItem();
                //
            }
        }

        string getFeedUrl()
        {
            if (ApplicationState.CurrentServiceLanguage == "pl")
            {
                return string.Format(RssTipsChannelAddress, "_pl");
            }
            return string.Format(RssTipsChannelAddress, "");
        }

        void enusureRssFeed()
        {
            var feed=loadTips();
            if (feed != null && feed.Language == ApplicationState.CurrentServiceLanguage)
            {
                filterFeed(feed);
                return;
            }
            if(ApplicationState.Current.IsOffline)
            {
                PageTitle.Text ="";
                htmlBlock.Text = ApplicationStrings.OfflineModeFeatureNotRetrieved;
                return;
            }
            WebClient client = new WebClient();
            progressBar.ShowProgress(true,ApplicationStrings.TipsPage_ProgressRetrieveTips);
            client.OpenReadCompleted += delegate(object sender, OpenReadCompletedEventArgs e)
            {
                if(e.Error!=null)
                {
                    progressBar.ShowProgress(false);
                    BAMessageBox.ShowError(ApplicationStrings.ErrCannotRetrieveTips);
                    return;
                }

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.DtdProcessing = DtdProcessing.Ignore;

                feed = SyndicationFeed.Load(XmlReader.Create(e.Result, settings));
                saveTips(feed);
                filterFeed(feed);
                progressBar.ShowProgress(false);
            };
            client.OpenReadAsync(new Uri(getFeedUrl(), UriKind.Absolute));
        }

        private void filterFeed(SyndicationFeed feed)
        {
            items = new List<SyndicationItem>();
            var list = feed.Items.Where(x => x.Categories.Any(syndicationCategory => syndicationCategory.Name == "WP7") ||
                                             x.Categories.Any(syndicationCategory => syndicationCategory.Name == "Training")||
                                             x.Categories.Any(syndicationCategory => syndicationCategory.Name == "Application"))
                .Reverse();

            items.AddRange(list);

            showItem();
        }

        SyndicationFeed loadTips()
        {
            try
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (store.FileExists(Constants.TipsFileName))
                    {
                        using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(Constants.TipsFileName, FileMode.Open, store))
                        {
                            return SyndicationFeed.Load(XmlReader.Create(stream));
                        }
                    }
                }
            }
            catch 
            {

            }
            return null;
        }

        void saveTips(SyndicationFeed  feed)
        {
            ThreadPool.QueueUserWorkItem(delegate
               {
                   try
                   {
                       using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                       {
                           using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(Constants.TipsFileName, FileMode.Create, store))
                           {
                               var destXml = XmlWriter.Create(stream);
                               feed.SaveAsRss20(destXml);
                               destXml.Close();
                               //store date when the tips cache has been created
                               Settings.TipsDateTime = DateTime.Now;
                           }
                       }
                   }
                   catch
                   {
                   }
               });
               }
    }
}