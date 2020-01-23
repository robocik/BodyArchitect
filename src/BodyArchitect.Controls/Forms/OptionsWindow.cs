using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Controls.Cache;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTab;
using BodyArchitect.Common;
using BodyArchitect.Common.Localization;
using BodyArchitect.Common.Plugins;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Controls.Reporting;
using BodyArchitect.Controls.UserControls;
using BodyArchitect.Settings.Model;

namespace BodyArchitect.Controls.Forms
{
    public partial class OptionsWindow : BaseWindow
    {
        public OptionsWindow()
        {
            InitializeComponent();
            fillSuperTips();
        }

        void fillSuperTips()
        {
            //tpCalendar.Tooltip = SuperTips.OptionCalendarTabPage;
            //tpCalendar.TooltipTitle = tpCalendar.Text;
            ControlHelper.AddSuperTip(btnResetAllOptions, btnResetAllOptions.Text,SuperTips.OptionResetAllBtn);
        }

        private void OptionsWindow_Load(object sender, EventArgs e)
        {
            List<IOptionsControl> optionsControls = new List<IOptionsControl>();
            optionsControls.Add(new usrCalendarOptions());
            optionsControls.Add(new usrReportingOptions());
            optionsControls.AddRange(PluginsManager.Instance.GetOptionsControls());
            foreach (var optionsControl in optionsControls)
            {
                AddOptionsControl(optionsControl);
            }

            fillSettings();
        }

        void fillSettings()
        {
            foreach (XtraTabPage tabPage in xtraTabControl1.TabPages)
            {
                IOptionsControl ctrl = tabPage.Controls[0] as IOptionsControl;
                if(ctrl!=null)
                {
                    ctrl.Fill();
                }
            }
        }

        public void AddOptionsControl(IOptionsControl control)
        {
            XtraTabPage tabPage = new XtraTabPage();
            tabPage.Text = control.Title;
            tabPage.Image = control.Image;
            Control ctrl = (Control) control;
            ctrl.Dock = DockStyle.Fill;
            tabPage.Controls.Add(ctrl);
            xtraTabControl1.TabPages.Add(tabPage);
        }

        

        private void okButton1_Click(object sender, EventArgs e)
        {
            bool restartRequired = false;
            foreach (XtraTabPage tabPage in xtraTabControl1.TabPages)
            {
                IOptionsControl optionsControl = (IOptionsControl)tabPage.Controls[0];
                optionsControl.Save();
                if(optionsControl.RestartRequired)
                {
                    restartRequired = true;
                }
            }

            UserContext.Settings.GuiState.Save();
            if (restartRequired)
            {
                FMMessageBox.ShowInfo(ApplicationStrings.OptionsRestartRequiredMsg);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (FMMessageBox.AskYesNo(ApplicationStrings.QResetAllOptions) == DialogResult.Yes)
            {
                //TODO: maybe clear cache should be under another button
                PicturesCache.Instance.Cache.Flush();
                UserContext.Settings.Reset();
                fillSettings();
            }
        }
    }
}