using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.Model;
using BodyArchitect.Shared;
using DevExpress.XtraEditors;
using BodyArchitect.Common;
using BodyArchitect.Controls.Localization;
using DevExpress.XtraTab;
using DevExpress.Utils;
using DevExpress.XtraTab.ViewInfo;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Common.Controls;

using BodyArchitect.Common.Localization;
using BodyArchitect.Common.Plugins;

namespace BodyArchitect.Controls.UserControls
{
    public partial class usrAddTrainingDay : usrBaseControl
    {
        private TrainingDayDTO day;
        private bool dayRemoved;
        public event CancelEventHandler TrainingDayChanging;
        public event EventHandler<TrainingDayChangedEventArgs> TrainingDayChanged;

        public usrAddTrainingDay()
        {
            InitializeComponent();
            createEntryObjectMenu();
            fillSuperTips();
            updateButtons();
            toolStrip1.ShowItemToolTips = UserContext.Settings.GuiState.ShowToolTips;
        }

        private void onTrainingDayChanging(CancelEventArgs e)
        {
            if(TrainingDayChanging!=null)
            {
                TrainingDayChanging(this, e);
            }
        }

        private void onTrainingDayChanged(TrainingDayChangedEventArgs e)
        {
            if (TrainingDayChanged != null)
            {
                TrainingDayChanged(this, e);
            }
        }
        public bool ReadOnly
        {
            get { return day == null || day.ProfileId != UserContext.CurrentProfile.Id; }
        }

        public TrainingDayDTO CurrentDay
        {
            get
            {
                return day;
            }
            private set
            {
                day = value;
            }
        }

        void createEntryObjectMenu()
        {
            foreach (var entryControl in EntryObjectControlManager.Instance.Controls)
            {
                string text = EntryObjectLocalizationManager.Instance.GetString(entryControl.Key, LocalizationConstants.EntryObjectName);
                ToolStripMenuItem tsMenuItem = new ToolStripMenuItem(text);
                tsMenuItem.Tag = entryControl;
                tsMenuItem.Image = PluginsManager.Instance.GetEntryObjectProvider(entryControl.Key).ModuleImage;
                tsMenuItem.Click += tsStrengthTraining_Click;
                tbAddEntry.DropDownItems.Add(tsMenuItem);
            }
        }

        void fillSuperTips()
        {
            ControlHelper.AddSuperTip(this.txtDate, lblDate.Text, SuperTips.AddTrainingDay_DateTE);
        }

        void updateButtons()
        {
            tsRename.Enabled = xtraTabControl1.SelectedTabPage != null;
            tsRename.Visible = !ReadOnly;
            //show delete button only when we open existing day (saved in db)
            this.tsbDeleteTrainingDay.Enabled = day!=null && day.Id > Constants.UnsavedObjectId;
            tsbDeleteTrainingDay.Visible = !ReadOnly;
            this.tbAddEntry.Visible = !ReadOnly;
            toolStripSeparator1.Visible = !ReadOnly;
            toolStripSeparator2.Visible = !ReadOnly;
        }

        public bool DayRemoved
        {
            get { return dayRemoved; }
        }

        public UserDTO User { get; private set; }

        public void Fill(TrainingDayDTO day,UserDTO user)
        {
            this.SuspendLayout();
            this.day = day;
            User = user;
            txtDate.Text = day.TrainingDate.ToShortDateString();
            xtraTabControl1.TabPages.Clear();
            for (int index = day.Objects.Count-1; index >=0 ; index--)
            {
                var entry = day.Objects[index];
                if (!createNewEntryControl(entry, false))
                {
                    //exception during creating new entry so we delete it from training day
                    day.RemoveEntry(entry);
                }
            }
            updateButtons();
            this.ResumeLayout();
        }

        private bool createNewEntryControl(EntryObjectDTO entry, bool select)
        {
            if (entry.IsLoaded)
            {
                try
                {

                    PluginsManager.Instance.GetEntryObjectProvider(entry.GetType()).PrepareNewEntryObject(UserContext.SessionData, entry, day);

                    Control ctrl = (Control)Activator.CreateInstance(EntryObjectControlManager.Instance.Controls[entry.GetType()]);
                    XtraTabPage tabPage = new XtraTabPage();
                    tabPage.Text = getEntryTabText(entry);
                    ctrl.Dock = DockStyle.Fill;
                    tabPage.ShowCloseButton = ReadOnly ? DefaultBoolean.False : DefaultBoolean.True;
                    ctrl.Tag = tabPage.Tag = entry;
                    tabPage.Controls.Add(ctrl);
                    xtraTabControl1.TabPages.Add(tabPage);
                    xtraTabControl1.SelectedTabPage = tabPage;

                    var entryCtrl = (IEntryObjectControl)ctrl;
                    entryCtrl.ReadOnly = ReadOnly;
                    entryCtrl.Fill(entry);
                    updateButtons();
                }
                catch (TrainingIntegrationException ex)
                {
                    ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorTrainingIntegrity, ErrorWindow.MessageBox);
                    return false;
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorUnhandledException, ErrorWindow.EMailReport);
                    return false;
                }
                
            }
            return true;
        }

        private string getEntryTabText(EntryObjectDTO entry)
        {
            return string.IsNullOrWhiteSpace(entry.Name) ? EntryObjectLocalizationManager.Instance.GetString(entry.GetType(), LocalizationConstants.EntryObjectName) : entry.Name;
        }

        public bool SaveTrainingDay(bool isWindowClosing)
        {
            if(!validateControls())
            {
                return false;
            }
            foreach (XtraTabPage tabPage in xtraTabControl1.TabPages)
            {
                IEntryObjectControl entryControl =(IEntryObjectControl) tabPage.Controls[0];
                entryControl.UpdateEntryObject();
            }

            if (day.Id==Constants.UnsavedObjectId && day.IsEmpty)
            {
                return true;
            }

            day = ServiceManager.SaveTrainingDay(day);

            foreach(var module in PluginsManager.Instance.Modules)
            {
                module.AfterSave(UserContext.SessionData, day);
            }

            if (isWindowClosing)
            {//optimalization - we are closing window so we don't need to refresh the tabs (code below)
                return true;
            }
            for (int index = xtraTabControl1.TabPages.Count-1; index >=0; index--)
            {
                XtraTabPage tabPage = xtraTabControl1.TabPages[index];
                IEntryObjectControl entryControl = tabPage.Controls[0] as IEntryObjectControl;
                if (entryControl != null)
                {
                    ParentWindow.SynchronizationContext.Send(delegate
                               {
                                   entryControl.AfterSave(isWindowClosing);
                                   EntryObjectDTO entry =
                                       (EntryObjectDTO) tabPage.Controls[0].Tag;
                                   //here we assume that training day contains only one instance of the specific entry type
                                   var newEntry =
                                       day.GetSpecifiedEntries(entry.GetType());
                                   if (newEntry != null)
                                   {
                                       tabPage.Controls[0].Tag = newEntry;
                                       entryControl.Fill(newEntry);
                                   }
                                   else
                                   {
                                       xtraTabControl1.TabPages.Remove(tabPage);
                                   }
                               }, null);
                }
            }

            return true;
        }

        private bool validateControls()
        {
            foreach (XtraTabPage tabPage in xtraTabControl1.TabPages)
            {
                IValidationControl entryControl = tabPage.Controls[0] as IValidationControl;
                if (entryControl != null)
                {
                    if(!entryControl.ValidateControl())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void tsStrengthTraining_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                ToolStripDropDownItem menuItem = (ToolStripDropDownItem) sender;
                KeyValuePair<Type, Type> pairEntry = (KeyValuePair<Type, Type>) menuItem.Tag;

                EntryObjectInstanceAttribute entryAttribute = new EntryObjectInstanceAttribute();
                var attributes = pairEntry.Key.GetCustomAttributes(typeof (EntryObjectInstanceAttribute), true);
                if (attributes.Length > 0)
                {
                    entryAttribute = (EntryObjectInstanceAttribute) attributes[0];
                }
                //if this type can be only once added then we need to check if we can add it
                if (entryAttribute.Instance == EntryObjectInstance.Single)
                {
                    if (day.ContainsSpecifiedEntry(pairEntry.Key))
                    {
                        FMMessageBox.ShowError(ApplicationStrings.ErrorEntryObjectTypeAlreadyExists);
                        return;
                    }
                }
                var entry = day.CreateEntryObject(pairEntry.Key);

                try
                {

                    createNewEntryControl(entry, true);

                    day.AddEntry(entry);
                    Cursor = Cursors.Default;
                }
                catch (TrainingIntegrationException ex)
                {
                    ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorTrainingIntegrity, ErrorWindow.MessageBox);
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorUnhandledException,ErrorWindow.EMailReport);
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
            
        }

        private void xtraTabControl1_CloseButtonClick(object sender, EventArgs e)
        {
            ClosePageButtonEventArgs closeArgs =(ClosePageButtonEventArgs) e;
            XtraTabPage page = (XtraTabPage)closeArgs.Page;
            var obj = (EntryObjectDTO)page.Tag;
            if (FMMessageBox.AskYesNo(ApplicationStrings.QAskForDeletingEntryObject, page.Text) == DialogResult.Yes)
            {
                day.Objects.Remove(obj);
                xtraTabControl1.TabPages.Remove(page);
                updateButtons();
            }
        }

        private void tsRename_Click(object sender, EventArgs e)
        {
            InputWindow dlg = new InputWindow();
            dlg.MaxLength = Constants.NameColumnLength;
            var obj = (EntryObjectDTO)xtraTabControl1.SelectedTabPage.Tag;
            dlg.Value = obj.Name;
            if (dlg.ShowDialog(this)==DialogResult.OK)
            {
                obj.Name = dlg.Value;
                xtraTabControl1.SelectedTabPage.Text = getEntryTabText(obj);
            }
        }

        private void tsbDeleteTrainingDay_Click(object sender, EventArgs e)
        {
            if (DomainObjectOperatonHelper.DeleteTrainingDay(UserContext.SessionData, day))
            {
                dayRemoved = true;
                XtraForm parentForm = ControlHelper.GetParentControl<XtraForm>(this);
                parentForm.DialogResult = DialogResult.OK;
                parentForm.Close();
            }
        }

        private void tsbPrevious_Click(object sender, EventArgs e)
        {
            previousNextEntry(CurrentDay, GetOperation.Previous, ApplicationStrings.InfoPreviousEntryLimit);
        }

        private void tsbNext_Click(object sender, EventArgs e)
        {
            previousNextEntry(CurrentDay, GetOperation.Next, ApplicationStrings.InfoNextEntryLimit);
        }

        //private void previousNextEntry(TrainingDayDTO currentDay,string limitMessage)
        //{
        //    if (currentDay != null)
        //    {
        //        CurrentDay = currentDay;
        //        Fill(day);
        //    }
        //    else
        //    {
        //        FMMessageBox.ShowInfo(limitMessage);
        //    }
        //}

        private void previousNextEntry(TrainingDayDTO currentDay, GetOperation operationType, string limitMessage)
        {
            CancelEventArgs e= new CancelEventArgs();
            onTrainingDayChanging(e);
            if(e.Cancel)
            {
                return;
            }
            ParentWindow.RunAsynchronousOperation(delegate(OperationContext context)
            {
                WorkoutDayGetOperation operation = new WorkoutDayGetOperation();
                operation.Operation = operationType;
                operation.UserId = User.Id;
                operation.WorkoutDateTime = currentDay.TrainingDate;
                var day = ServiceManager.GetTrainingDay(operation);
                context.CancellatioToken.ThrowIfCancellationRequested();
                ParentWindow.SynchronizationContext.Send(delegate
                {
                    if (day != null)
                    {
                        DateTime oldDate = CurrentDay.TrainingDate;
                        CurrentDay = day;
                        Fill(day, User);
                        onTrainingDayChanged(new TrainingDayChangedEventArgs(oldDate,CurrentDay.TrainingDate));
                    }
                    else
                    {
                        FMMessageBox.ShowInfo(limitMessage);//ApplicationStrings.InfoNextEntryLimit);
                    }
                }, null);
            },  delegate(OperationContext ctx)
            {
                tsbNext.Enabled = tsbPrevious.Enabled = ctx.State != OperationState.Started;
            });

            
        }

    }

    public class TrainingDayChangedEventArgs:EventArgs
    {
        public TrainingDayChangedEventArgs(DateTime oldDate, DateTime newDate)
        {
            OldDate = oldDate;
            NewDate = newDate;
        }

        public DateTime OldDate { get; private set; }

        public DateTime NewDate { get; private set; }
    }
}
