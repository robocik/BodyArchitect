using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Settings;
using BodyArchitect.Shared;

namespace BodyArchitect.Client.UI.Windows
{
    /// <summary>
    /// Interaction logic for ImportLicenceKeyWindow.xaml
    /// </summary>
    public partial class ImportLicenceKeyWindow 
    {
        private string licenceKey;

        public ImportLicenceKeyWindow()
        {
            InitializeComponent();
            this.DataContext = this;


            usrProgressIndicatorButtons1.OkButton.Content = EnumLocalizer.Default.GetGUIString("ImportLicenceKeyWindow_Button_Import");
            var binding = new Binding("CanImport");
            binding.Mode = BindingMode.OneWay;
            usrProgressIndicatorButtons1.OkButton.SetBinding(Button.IsEnabledProperty, binding);
        }

        public string LicenceKey
        {
            get { return licenceKey; }
            set
            {
                licenceKey = value;
                NotifyOfPropertyChange(()=>LicenceKey);
                NotifyOfPropertyChange(() => CanImport);
            }
        }

        public bool CanImport
        {
            get { return !string.IsNullOrWhiteSpace(LicenceKey); }
        }

        private void usrProgressIndicatorButtons_OkClick(object sender, Controls.CancellationSourceEventArgs e)
        {
            try
            {
                ServiceManager.ImportLicence(LicenceKey);
                UserContext.Current.RefreshUserData();
                ThreadSafeClose(true);
            }
            catch (AlreadyOccupiedException ex)
            {
                TasksManager.SetException(ex);
                this.SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex, EnumLocalizer.Default.GetGUIString("ImportLicenceKeyWindow_ErrKeyAlreadyImported"), ErrorWindow.MessageBox), null);
            }
            catch (Exception ex)
            {
                TasksManager.SetException(ex);
                this.SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex, EnumLocalizer.Default.GetGUIString("ImportLicenceKeyWindow_ErrImportKey"), ErrorWindow.MessageBox), null);
            }
            
        }

    }
}
