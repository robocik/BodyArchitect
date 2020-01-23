using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI.Cache;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Translator;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.UI.Views
{
    [Serializable]
    public class HtmlPreviewPageContext:PageContext
    {
        public HtmlPreviewPageContext(IHtmlProvider htmlProvider)
        {
            HtmlProvider = htmlProvider;
        }
        public IHtmlProvider HtmlProvider { get; set; }
    }
    /// <summary>
    /// Interaction logic for HtmlPreviewView.xaml
    /// </summary>
    public partial class HtmlPreviewView
    {
        public HtmlPreviewView()
        {
            
            InitializeComponent();
        }


        public IHtmlProvider CurrentHtmlProvider
        {
            get
            {
                HtmlPreviewPageContext pageContext = (HtmlPreviewPageContext) PageContext;
                return pageContext.HtmlProvider;
            }
        }

        public override AccountType AccountType
        {
            get
            {
                return AccountType.PremiumUser;
            }
        }

        public override void Fill()
        {
            Header = EnumLocalizer.Default.GetStringsString("HtmlPreviewView_Fill_Header_Preview") + CurrentHtmlProvider.Title;
            propertyGrid2.SelectedObject = CurrentHtmlProvider;
            
            webBrowser.DocumentText = CurrentHtmlProvider.GetHtml();
        }

        public override void RefreshView()
        {
            Fill();
        }



        public override Uri HeaderIcon
        {
            get { return "Preview16.png".ToResourceUrl(); }
        }

        private void rbtnPrint_Click(object sender, RoutedEventArgs e)
        {
            if(!UIHelper.EnsurePremiumLicence())
            {
                return;
            }
            webBrowser.ShowPrintDialog();
            
        }

        private void rbtnPrintPreview_Click(object sender, RoutedEventArgs e)
        {
            if (!UIHelper.EnsurePremiumLicence())
            {
                return;
            }
            webBrowser.ShowPrintPreviewDialog();
        }


        

        string getCurrentHtmlKey(string language)
        {
            var properties = CurrentHtmlProvider.GetType().GetProperties().Where(prop => !prop.IsDefined(typeof(BrowsableAttribute),true));
            var key = string.Format("{0}_{1}", CurrentHtmlProvider.GlobalId, language);
            foreach (var propertyInfo in properties)
            {
                var value= propertyInfo.GetValue(CurrentHtmlProvider, null);
                if(value!=null)
                {
                    key += "_"+value.ToString();
                }
            }
            return key;
        }
        private void rbtnTranslate_Click(object sender, RoutedEventArgs e)
        {
            var text = CurrentHtmlProvider.GetHtml();
            var sourceLanguage = CurrentHtmlProvider.Language;
            HtmlTranslator translator = new HtmlTranslator(text, sourceLanguage);
            
            TranslateSettingsWindow dlg = new TranslateSettingsWindow(translator);
            if(dlg.ShowDialog()==false)
            {
                return;
            }

            if (!UIHelper.EnsurePremiumLicence())
            {
                return;
            }

            var language = dlg.SelectedLanguage;
            if (sourceLanguage == language)
            {
                Fill();
                return;
            }
            var key = getCurrentHtmlKey(language);
            var cachedItem = TranslationsCache.Instance.Get(key);
            if (cachedItem != null)
            {
                webBrowser.DocumentText = cachedItem;
                return;
            }

            test.Visibility = System.Windows.Visibility.Collapsed;

            PleaseWait.Run(x =>{
                                   try
                                   {
                                       var translatedHtml = translator.Translate(language, x);
                                       if (translatedHtml == null)
                                       {
                                           return;
                                       }
                                       //var translatedHtml = "";
                                       //for (int i = 0; i < 100; i++)
                                       //{
                                       //    if(x.Cancel)
                                       //    {
                                       //        return;
                                       //    }
                                       //    Thread.Sleep(70);
                                       //}
                                       TranslationsCache.Instance.Add(key, translatedHtml);

                                       UIHelper.BeginInvoke(() =>
                                       {
                                           test.Visibility = System.Windows.Visibility.Visible;
                                           webBrowser.DocumentText = translatedHtml;
                                       }, Dispatcher);
                                   }
                                   catch (Exception ex)
                                   {
                                       UIHelper.Invoke(() =>
                                       {
                                           test.Visibility = System.Windows.Visibility.Visible;
                                           ExceptionHandler.Default.Process(ex, "HtmlPreviewView_ErrTranslate".TranslateStrings(), ErrorWindow.MessageBox);
                                       }, Dispatcher);
                                   }


                               }, true, progressMax: translator.MaxRequests);
            
        }

        private void propertyGrid2_PropertyValueChanged(object sender, Xceed.Wpf.Toolkit.PropertyGrid.PropertyValueChangedEventArgs e)
        {
            Fill();
        }
    }
}
