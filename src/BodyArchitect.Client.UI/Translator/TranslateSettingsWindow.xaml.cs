using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BodyArchitect.Client.UI.Cache;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.UI.Translator
{
    public partial class TranslateSettingsWindow
    {
        private string _selectedLanguage;
        private HtmlTranslator translator;
        ObservableCollection<Language> languages = new ObservableCollection<Language>();

        public TranslateSettingsWindow(HtmlTranslator translator)
        {
            InitializeComponent();
            DataContext = this;
            
            this.translator = translator;
            Loaded += TranslateSettingsWindow_Loaded;

        }

        void TranslateSettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {

            var supportedLanguages = TranslationsCache.Instance.GetSupportedLanguages();
            if (supportedLanguages == null)
            {
                retrieveSupportedLanguages();
            }
            else
            {
                fillSupportedLanguages(supportedLanguages);
            }
            
        }

        private void retrieveSupportedLanguages()
        {
            progressIndicator.IsRunning = true;
            btnOK.IsEnabled = false;
            RunAsynchronousOperation(x =>
                                         {
                                             try
                                             {
                                                 var languages = translator.GetSupportedLanguages();
                                                 TranslationsCache.Instance.SetSupportedLanguages(languages);
                                                 UIHelper.Invoke(() =>
                                                                     {
                                                                         fillSupportedLanguages(languages);
                                                                     }, Dispatcher);
                                             }
                                             finally
                                             {
                                                 UIHelper.Invoke(() =>
                                                                     {
                                                                         btnOK.IsEnabled = true;
                                                                         progressIndicator.IsRunning = false;
                                                                     }, Dispatcher);
                                             }
                                         });
        }

        private void fillSupportedLanguages(string[] languages)
        {
            Languages.Clear();
            foreach (var language in BodyArchitect.Service.V2.Model.Language.Languages)
            {
                if (languages.Contains(language.Shortcut))
                {
                    Languages.Add(language);
                }
            }
            SelectedLanguage = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
        }

        public IList<Language> Languages
        {
            get { return languages; }
        }

        public string SelectedLanguage
        {
            get { return _selectedLanguage; }
            set
            {
                if(_selectedLanguage!=value)
                {
                    _selectedLanguage = value;
                    NotifyOfPropertyChange(()=>SelectedLanguage);
                }
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
