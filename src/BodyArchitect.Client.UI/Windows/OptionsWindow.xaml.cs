using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI.Cache;
using BodyArchitect.Client.UI.UserControls;
using BodyArchitect.Shared;

namespace BodyArchitect.Client.UI.Windows
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow 
    {
        public OptionsWindow()
        {
            InitializeComponent();
            Loaded += OptionsWindow_Load;
        }

        private void OptionsWindow_Load(object sender, EventArgs e)
        {
            List<IOptionsControl> optionsControls = new List<IOptionsControl>();
            optionsControls.Add(new usrCalendarOptions());
            optionsControls.AddRange(PluginsManager.Instance.GetOptionsControls());
            foreach (var optionsControl in optionsControls)
            {
                AddOptionsControl(optionsControl);
            }

            fillSettings();
        }

        void fillSettings()
        {
            foreach (ImageSourceListItem<IOptionsControl> tabPage in xtraTabControl1.Items)
            {
                IOptionsControl ctrl = tabPage.Value as IOptionsControl;
                if (ctrl != null)
                {
                    ctrl.Fill();
                }
            }
            if(xtraTabControl1.Items.Count>0)
            {
                xtraTabControl1.SelectedIndex = 0;
            }
        }

        public void AddOptionsControl(IOptionsControl control)
        {
            //FIX: strange bug but this solves it
            var parent = ((Control)control).Parent as ContentControl;
            if (parent != null)
            {
                parent.Content = null;
            }
            //end fix

            var tabItem = new ImageSourceListItem<IOptionsControl>();
            tabItem.Text = control.Title;
            tabItem.Value = control;
            tabItem.Image = control.Image;
            xtraTabControl1.Items.Add(tabItem);
        }


        private void btnResetAll_Click(object sender, EventArgs e)
        {
            if (BAMessageBox.AskYesNo(Strings.QResetAllOptions) == MessageBoxResult.Yes)
            {
                //TODO: maybe clear cache should be under another button
                try
                {
                    PicturesCache.Instance.Cache.Flush();
                    TranslationsCache.Instance.Cache.Flush();
                    UserContext.Current.Settings.Reset();
                    fillSettings();
                    File.Delete(Constants.AvalonLayoutFile);
                    MainWindow.Instance.SaveSizeAndLocation = false;
                    BAMessageBox.ShowInfo(Strings.OptionsWindow_MsgApplyResetLayoutSettings);
                }
                catch (Exception)
                {
                    BAMessageBox.ShowError(Strings.OptionsWindow_ErrResetLayoutSettings);
                }
                
            }
        }
        
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            bool restartRequired = false;
            foreach (ImageSourceListItem<IOptionsControl> tabPage in xtraTabControl1.Items)
            {
                IOptionsControl optionsControl = tabPage.Value;
                optionsControl.Save();
                if (optionsControl.RestartRequired)
                {
                    restartRequired = true;
                }
            }

            UserContext.Current.Settings.GuiState.Save();
            if (restartRequired)
            {
                BAMessageBox.ShowInfo(Strings.OptionsRestartRequiredMsg);
            }
            DialogResult = true;
            Close();
        }
    }
}
