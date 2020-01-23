using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using BodyArchitect.Common;
using BodyArchitect.Common.Localization;
using BodyArchitect.Common.Plugins;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Settings.Model;

namespace BodyArchitect.Controls.UserControls
{
    public partial class usrCalendarOptions : XtraUserControl, IOptionsControl
    {
        public usrCalendarOptions()
        {
            InitializeComponent();
            fillSuperTips();
        }

        void fillSuperTips()
        {
            ControlHelper.AddSuperTip(this.cmbTextFrom, lblTextFrom.Text, SuperTips.OptionCalendarTextFrom);
            ControlHelper.AddSuperTip(this.defaultToolTipController1.DefaultController,this.pluginsGrid1, lblShowIconsFor.Text, SuperTips.OptionCalendarIcons);
            ControlHelper.AddSuperTip(this.chkShowRelativeDates, chkShowRelativeDates.Text, SuperTips.OptionCalendarShowRelativeDates);
        }

        
        public void Fill()
        {
            
            CalendarOptions options = UserContext.Settings.GuiState.CalendarOptions;

            pluginsGrid1.Fill(options);
            fillCalendarDayContents(options);
            chkShowRelativeDates.Checked = UserContext.Settings.GuiState.ShowRelativeDates;

        }

        public bool RestartRequired
        {
            get { return false; }
        }

        public string Title
        {
            get { return ApplicationStrings.OptionsCalendarTitle; }
        }

        public Image Image
        {
            get { return Icons.Calendar; }
        }

        void fillCalendarDayContents(CalendarOptions options)
        {
            ImageList imgList = new ImageList();
            cmbTextFrom.Properties.SmallImages = imgList;
            cmbTextFrom.Properties.Items.Clear();
            foreach (var dayContent in PluginsManager.Instance.CalendarDayContents)
            {
                imgList.Images.Add(dayContent.Image);
                ImageComboBoxItem item = new ImageComboBoxItem(dayContent.Name, dayContent.GlobalId, imgList.Images.Count - 1);
                cmbTextFrom.Properties.Items.Add(item);
            }

            if (cmbTextFrom.Properties.Items.Count > 0)
            {
                var item = cmbTextFrom.Properties.Items.GetItem(options.CalendarTextType);
                if (item != null)
                {
                    cmbTextFrom.SelectedItem = item;
                }
                else
                {
                    cmbTextFrom.SelectedIndex = 0;
                }
            }
        }

        public void Save()
        {
            CalendarOptions options = new CalendarOptions();
            if (cmbTextFrom.EditValue != null)
            {
                options.CalendarTextType = (Guid)cmbTextFrom.EditValue;
            }
            else
            {
                options.CalendarTextType = Guid.Empty;
            }
            UserContext.Settings.GuiState.ShowRelativeDates = chkShowRelativeDates.Checked;

            pluginsGrid1.Save(options);
            UserContext.Settings.GuiState.CalendarOptions = options;

        }
    }
}
