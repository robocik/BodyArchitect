namespace BodyArchitect.Controls.UserControls
{
    partial class usrCalendarView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrCalendarView));
            this.cmnCalendarMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuOpenTrainingDay = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDeleteDay = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.mnuEditCut = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.monthCalendar1 = new BodyArchitect.Controls.External.MonthCalendar();
            this.cmnCalendarMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmnCalendarMenu
            // 
            resources.ApplyResources(this.cmnCalendarMenu, "cmnCalendarMenu");
            this.cmnCalendarMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuOpenTrainingDay,
            this.mnuDeleteDay,
            this.menuSeparator,
            this.mnuEditCut,
            this.mnuEditCopy,
            this.mnuEditPaste});
            this.cmnCalendarMenu.Name = "contextMenuStrip1";
            this.cmnCalendarMenu.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // mnuOpenTrainingDay
            // 
            resources.ApplyResources(this.mnuOpenTrainingDay, "mnuOpenTrainingDay");
            this.mnuOpenTrainingDay.Image = global::BodyArchitect.Controls.Icons.TrainingDayAdd;
            this.mnuOpenTrainingDay.Name = "mnuOpenTrainingDay";
            this.mnuOpenTrainingDay.Click += new System.EventHandler(this.mnuOpenTrainingDay_Click);
            // 
            // mnuDeleteDay
            // 
            this.mnuDeleteDay.Image = global::BodyArchitect.Controls.Icons.TrainingDayDelete;
            this.mnuDeleteDay.Name = "mnuDeleteDay";
            resources.ApplyResources(this.mnuDeleteDay, "mnuDeleteDay");
            this.mnuDeleteDay.Click += new System.EventHandler(this.mnuDeleteDay_Click);
            // 
            // menuSeparator
            // 
            this.menuSeparator.Name = "menuSeparator";
            resources.ApplyResources(this.menuSeparator, "menuSeparator");
            // 
            // mnuEditCut
            // 
            resources.ApplyResources(this.mnuEditCut, "mnuEditCut");
            this.mnuEditCut.Name = "mnuEditCut";
            this.mnuEditCut.Click += new System.EventHandler(this.mnuEditCut_Click);
            // 
            // mnuEditCopy
            // 
            resources.ApplyResources(this.mnuEditCopy, "mnuEditCopy");
            this.mnuEditCopy.Name = "mnuEditCopy";
            this.mnuEditCopy.Click += new System.EventHandler(this.mnuEditCopy_Click);
            // 
            // mnuEditPaste
            // 
            resources.ApplyResources(this.mnuEditPaste, "mnuEditPaste");
            this.mnuEditPaste.Name = "mnuEditPaste";
            this.mnuEditPaste.Click += new System.EventHandler(this.mnuEditPaste_Click);
            // 
            // monthCalendar1
            // 
            this.monthCalendar1.AllowDrop = true;
            this.monthCalendar1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(166)))), ((int)(((byte)(228)))));
            this.monthCalendar1.ContextMenuStrip = this.cmnCalendarMenu;
            resources.ApplyResources(this.monthCalendar1, "monthCalendar1");
            this.monthCalendar1.ExtendedSelectionKey = BodyArchitect.Controls.External.mcExtendedSelectionKey.None;
            this.monthCalendar1.Footer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.monthCalendar1.Header.BackColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(135)))), ((int)(((byte)(220)))));
            this.monthCalendar1.Header.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.monthCalendar1.Header.TextColor = System.Drawing.Color.White;
            this.monthCalendar1.ImageList = null;
            this.monthCalendar1.MaxDate = new System.DateTime(2020, 5, 21, 17, 3, 20, 437);
            this.monthCalendar1.MinDate = new System.DateTime(2000, 5, 21, 17, 3, 20, 437);
            this.monthCalendar1.Month.BackgroundImage = null;
            this.monthCalendar1.Month.Colors.Focus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(189)))), ((int)(((byte)(235)))));
            this.monthCalendar1.Month.Colors.Focus.Border = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(166)))), ((int)(((byte)(228)))));
            this.monthCalendar1.Month.Colors.Selected.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(166)))), ((int)(((byte)(228)))));
            this.monthCalendar1.Month.Colors.Selected.Border = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(82)))), ((int)(((byte)(177)))));
            this.monthCalendar1.Month.DateFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.monthCalendar1.Month.TextFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.monthCalendar1.Name = "monthCalendar1";
            this.monthCalendar1.SelectionMode = BodyArchitect.Controls.External.mcSelectionMode.One;
            this.monthCalendar1.Weekdays.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.monthCalendar1.Weekdays.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(135)))), ((int)(((byte)(220)))));
            this.monthCalendar1.Weeknumbers.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.monthCalendar1.Weeknumbers.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(135)))), ((int)(((byte)(220)))));
            this.monthCalendar1.DayDragDrop += new BodyArchitect.Controls.External.DayDragDropEventHandler(this.monthCalendar1_DayDragDrop);
            this.monthCalendar1.MonthChanged += new BodyArchitect.Controls.External.MonthChangedEventHandler(this.monthCalendar1_MonthChanged);
            this.monthCalendar1.DayClick += new BodyArchitect.Controls.External.DayClickEventHandler(this.monthCalendar1_DayClick);
            this.monthCalendar1.DayDoubleClick += new BodyArchitect.Controls.External.DayClickEventHandler(this.monthCalendar1_DayDoubleClick);
            this.monthCalendar1.DragOver += new System.Windows.Forms.DragEventHandler(this.monthCalendar1_DragOver);
            this.monthCalendar1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.monthCalendar1_KeyUp);
            // 
            // usrCalendarView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.monthCalendar1);
            this.Name = "usrCalendarView";
            this.cmnCalendarMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private BodyArchitect.Controls.External.MonthCalendar monthCalendar1;
        private System.Windows.Forms.ContextMenuStrip cmnCalendarMenu;
        private System.Windows.Forms.ToolStripMenuItem mnuDeleteDay;
        private System.Windows.Forms.ToolStripMenuItem mnuOpenTrainingDay;
        private System.Windows.Forms.ToolStripSeparator menuSeparator;
        private System.Windows.Forms.ToolStripMenuItem mnuEditCut;
        private System.Windows.Forms.ToolStripMenuItem mnuEditCopy;
        private System.Windows.Forms.ToolStripMenuItem mnuEditPaste;
    }
}
