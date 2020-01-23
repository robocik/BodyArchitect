using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Service.Model;
using DevExpress.XtraEditors;
using BodyArchitect.Common.Controls;
using BodyArchitect.Controls;
using BodyArchitect.Controls.UserControls;

namespace BodyArchitect.Module.Size.Controls
{
    public partial class usrSizeEntry : usrBaseControl, IEntryObjectControl
    {
        private TableLayoutPanel tableLayoutPanel1;
        private BaGroupControl groupControl1;
        private usrWymiaryEditor usrWymiaryEditor1;
        private usrReportStatus usrReportStatus1;
        SizeEntryDTO sizeEntry;

        public usrSizeEntry()
        {
            InitializeComponent();
        }

        public void Fill(EntryObjectDTO entry)
        {
            sizeEntry = (SizeEntryDTO)entry;
            updateReadOnly();
            this.usrWymiaryEditor1.Fill(sizeEntry.Wymiary);
            usrReportStatus1.Fill(entry);
            
        }

        public void UpdateEntryObject()
        {

            usrWymiaryEditor1.SaveWymiary(sizeEntry.Wymiary);
            usrReportStatus1.Save(sizeEntry);
        }

        void updateReadOnly()
        {
            usrWymiaryEditor1.ReadOnly = ReadOnly;
            usrReportStatus1.ReadOnly = ReadOnly;
            usrReportStatus1.Visible = !ReadOnly;

        }
        public void AfterSave(bool isWindowClosing)
        {

        }

        public bool ReadOnly
        {
            get; set;
        }

        public Type EntryObjectType
        {
            get { return typeof(SizeEntryDTO); }
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrSizeEntry));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupControl1 = new BodyArchitect.Controls.UserControls.BaGroupControl();
            this.usrWymiaryEditor1 = new BodyArchitect.Controls.UserControls.usrWymiaryEditor();
            this.usrReportStatus1 = new BodyArchitect.Controls.UserControls.usrReportStatus();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.groupControl1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.usrReportStatus1, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // groupControl1
            // 
            this.groupControl1.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("groupControl1.AppearanceCaption.BackColor")));
            this.groupControl1.AppearanceCaption.Options.UseBackColor = true;
            this.groupControl1.AppearanceCaption.Options.UseTextOptions = true;
            this.groupControl1.AppearanceCaption.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
            this.groupControl1.Controls.Add(this.usrWymiaryEditor1);
            resources.ApplyResources(this.groupControl1, "groupControl1");
            this.groupControl1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.groupControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.groupControl1.Name = "groupControl1";
            // 
            // usrWymiaryEditor1
            // 
            resources.ApplyResources(this.usrWymiaryEditor1, "usrWymiaryEditor1");
            this.usrWymiaryEditor1.Name = "usrWymiaryEditor1";
            // 
            // usrReportStatus1
            // 
            resources.ApplyResources(this.usrReportStatus1, "usrReportStatus1");
            this.usrReportStatus1.Name = "usrReportStatus1";
            this.usrReportStatus1.ReadOnly = false;
            // 
            // usrSizeEntry
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "usrSizeEntry";
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
