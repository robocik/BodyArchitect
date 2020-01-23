using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Common;
using BodyArchitect.Common.Plugins;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Controls.Localization;

using BodyArchitect.Controls.External;
using BodyArchitect.Logger;
using BodyArchitect.Service.Model;
using BodyArchitect.Shared;


namespace BodyArchitect.Controls.UserControls
{
    public partial class usrCalendarView : usrBaseControl, IMainTabControl
    {
        ICalendarViewDayInfo dayInfoFiller= new CalendarViewDayInfo();
        private UserDTO user;
        private bool isFilled;

        public usrCalendarView()
        {
            InitializeComponent();
            this.monthCalendar1.ActiveMonth.Month = DateTime.Now.Month;
            this.monthCalendar1.ActiveMonth.Year = DateTime.Now.Year;
        }

        public bool ReadOnly 
        {
            get { return User != null && User.Id != UserContext.CurrentProfile.Id; }
        }

        protected override void LoginStatusChanged(LoginStatus newStatus)
        {
            if (newStatus != LoginStatus.Logged)
            {
                User = null;
                monthCalendar1.ResetDateInfo();
            }
        }

        public void SetActiveMonth(DateTime dateTime)
        {
            this.monthCalendar1.ActiveMonth.Month = dateTime.Month;
            this.monthCalendar1.ActiveMonth.Year = dateTime.Year;
        }

        public DateTime ActiveMonthDateTime
        {
         
            get
            {
                return new DateTime(monthCalendar1.ActiveMonth.Year,monthCalendar1.ActiveMonth.Month,1);
            }
        }

        #region Copy/Paste

        public void Paste()
        {
            if (Clipboard.ContainsData(Constants.ClipboardFormat) && SelectedDate.HasValue && !IsSelectedDateFuture)
            {
                if (SelectedTrainingDay == null )
                {
                    var day = (TrainingDayDTO)Clipboard.GetData(Constants.ClipboardFormat);
                    PleaseWait.Run(delegate
                                       {
                                           
                                           try
                                           {
                                               if (day.Id != Constants.UnsavedObjectId)
                                               {
                                                   //user select Cut operation so we should only move this one time (not many time like with Copy operation)
                                                   WorkoutDayGetOperation operation = new WorkoutDayGetOperation();
                                                   operation.UserId = User.Id;
                                                   operation.Operation = GetOperation.Current;
                                                   operation.WorkoutDateTime = day.TrainingDate;
                                                   day = ServiceManager.GetTrainingDay(operation);
                                                   ParentWindow.SynchronizationContext.Send(delegate
                                                          {
                                                              Clipboard.Clear();
                                                          },null);
                                                   
                                               }

                                               day.ChangeDate(SelectedDate.Value);
                                               ServiceManager.SaveTrainingDay(day);
                                               fillImplementation(day.TrainingDate,User,null);
                                               //this.Fill(User, day.TrainingDate);
                                           }
                                           catch (OldDataException ex)
                                           {
                                               ExceptionHandler.Default.Process(ex,
                                                                                ApplicationStrings.ErrorOldTrainingDay,
                                                                                ErrorWindow.MessageBox);
                                           }
                                           catch (Exception ex)
                                           {
                                               ExceptionHandler.Default.Process(ex,
                                                                                ApplicationStrings.ErrorMoveTrainingDay,
                                                                                ErrorWindow.EMailReport);
                                           }
                                       });
                }
                else
                {
                    FMMessageBox.ShowError(ApplicationStrings.ErrorCannotPaste);
                }
            }
        }

        public void Cut()
        {
            var day = SelectedTrainingDay;
            if (day != null)
            {
                if (day.CanMove)
                {
                    Clipboard.SetData(Constants.ClipboardFormat, day);
                }
                else
                {
                    FMMessageBox.ShowError(ApplicationStrings.ErrorCannotMoveTrainingDayFixedEntries);
                }
            }
        }

        public void Copy()
        {
            var day = SelectedTrainingDay;
            if (day != null)
            {
                PleaseWait.Run(delegate
                    {
                        WorkoutDayGetOperation operation = new WorkoutDayGetOperation();
                        operation.UserId = User.Id;
                        operation.Operation = GetOperation.Current;
                        operation.WorkoutDateTime = day.TrainingDate;
                        day = ServiceManager.GetTrainingDay(operation);
                        day = day.Copy();
                        
                    });
                if (day.Objects.Count == 0)
                {
                    FMMessageBox.ShowError(ApplicationStrings.ErrorCannotCopyTrainingDayFixedEntries);
                    return;
                }
                Clipboard.SetData(Constants.ClipboardFormat, day);
            }
        }

        public bool CanPaste
        {
            get
            {
                return SelectedDate != null && !IsSelectedDateFuture && Clipboard.ContainsData(Constants.ClipboardFormat);
            }
        }

        public bool CanCopy
        {
            get
            {
                return CanCut;
            }
        }

        public bool CanCut
        {
            get
            {
                return SelectedTrainingDay != null;
            }
        }

        #endregion

        private CancellationTokenSource fillCancelSource;

        public void Fill()
        {
            Fill(User,ActiveMonthDateTime);
        }

        public void RefreshView()
        {
            Fill();
        }

        private void Fill(UserDTO user,DateTime activeMonth)
        {
            isFilled = false;
            if(UserContext.IsConnected && UserContext.LoginStatus==LoginStatus.Logged)
            {
                if (fillCancelSource!=null)
                {
                    fillCancelSource.Cancel();
                    fillCancelSource = null;
                }
                fillCancelSource = ParentWindow.RunAsynchronousOperation(delegate(OperationContext context)
                                                                             {
                                                                                 fillImplementation(activeMonth, user, context);
                                                                             }, null,delegate (OperationContext errorContext)
                {     
                            if (errorContext.CurrentTask.Exception != null)
                            {
                                FMMessageBox.ShowError(errorContext.CurrentTask.Exception.GetBaseException().ToString());
                            }
                });
            }
            else
            {
                monthCalendar1.ResetDateInfo();
            }
            
        }

        private void fillImplementation(DateTime activeMonth, UserDTO user, OperationContext context)
        {
            try
            {
                DateTime startDate = DateHelper.GetFirstDayOfMonth(activeMonth);
                DateTime endDate = DateHelper.GetLastDayOfMonth(activeMonth);
                WorkoutDaysSearchCriteria searchCriteria = new WorkoutDaysSearchCriteria();
                searchCriteria.StartDate = startDate;
                searchCriteria.EndDate = endDate;
                searchCriteria.UserId = user.Id;
                var pageInfo = new PartialRetrievingInfo();
                pageInfo.PageSize = PartialRetrievingInfo.AllElementsPageSize;
                var days = ServiceManager.GetTrainingDays(searchCriteria, pageInfo);
                if (context!=null && context.CancellatioToken.IsCancellationRequested)
                {
                    return;
                }
                if (ParentWindow != null && monthCalendar1.ActiveMonth.Month == activeMonth.Month && monthCalendar1.ActiveMonth.Year == activeMonth.Year)
                {
                    dayInfoFiller.PrepareData();
                    ParentWindow.SynchronizationContext.Send(delegate
                                                                 {
                                                                     monthCalendar1.ResetDateInfo();
                                                                     //DateTime start = DateTime.Now;
                                                                     foreach (var day in days.Items)
                                                                     {
                                                                         addTrainingDayInfo(day);
                                                                     }
                                                                 }, context);
                }
            }
            finally
            {
                isFilled = true;
            }
        }

        private void addTrainingDayInfo(TrainingDayDTO day)
        {
            if (day != null)
            {
                var item = dayInfoFiller.AddDayInfo(monthCalendar1, day);
                item.Tag = day;
            }
        }

        public TrainingDayDTO SelectedTrainingDay
        {
            get
            {
                if (monthCalendar1.SelectedDates.Count > 0)
                {
                    return getTrainingDay(monthCalendar1.SelectedDates[0]);
                }
                return null;
            }
        }

        private TrainingDayDTO getTrainingDay(DateTime dateTime)
        {
            var info = monthCalendar1.GetDateInfo(dateTime);
            return (TrainingDayDTO)(info.Length > 0 ? info[0].Tag : null);
        }

        public DateTime? SelectedDate
        {
            get
            {
                if (monthCalendar1.SelectedDates.Count > 0)
                {
                    return monthCalendar1.SelectedDates[0];
                }
                return null;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public UserDTO User
        {
            get
            {
                if (user != null)
                {
                    return user;
                }
                return UserContext.CurrentProfile;
            }
            set { user = value; }
        }

        private void monthCalendar1_MonthChanged(object sender, MonthChangedEventArgs e)
        {
            Fill(User,new DateTime(e.Year, e.Month, 1));
        }

        public bool IsSelectedDateFuture
        {
            get { return SelectedDate != null && SelectedDate.Value.Date > DateTime.Now.Date; }
        }
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            TrainingDayDTO day = SelectedTrainingDay;
            if (!isFilled || UserContext.LoginStatus != LoginStatus.Logged || IsSelectedDateFuture || (day == null && ReadOnly/*we browse workout for another users so if there is no entry then we should skip*/))
            {
                e.Cancel = true;
                return;
            }
            
            mnuDeleteDay.Visible =!ReadOnly && day != null;
            mnuOpenTrainingDay.Text = day != null || ReadOnly ? ApplicationStrings.MNUOpenTrainingDay : ApplicationStrings.MNUNewTrainingDay;
            mnuOpenTrainingDay.Image = day != null || ReadOnly ? Icons.TrainingDayEdit : Icons.TrainingDayAdd;
            mnuEditCopy.Enabled = CanCopy;
            mnuEditCut.Enabled =  CanCut;
            mnuEditPaste.Enabled =  CanPaste;
            
            mnuEditCopy.Visible = !ReadOnly;
            mnuEditCut.Visible = !ReadOnly;
            mnuEditPaste.Visible = !ReadOnly;
            menuSeparator.Visible = !ReadOnly;
        }

        
        private void mnuDeleteDay_Click(object sender, EventArgs e)
        {
            deleteTrainingDay();
        }

        private void monthCalendar1_DayDoubleClick(object sender, BodyArchitect.Controls.External.DayClickEventArgs e)
        {
            DateTime selectedDate = DateTime.Parse(e.Date);
            openTrainingDay(selectedDate);
        }

        private void openTrainingDay(DateTime selectedDate)
        {
            AddTrainingDay dlg = new AddTrainingDay();

            if (!isFilled || UserContext.LoginStatus != LoginStatus.Logged || IsSelectedDateFuture)
            {
                return;
            }

            var day = new TrainingDayDTO(selectedDate, UserContext.CurrentProfile.Id);
            var info = monthCalendar1.GetDateInfo(selectedDate);
            if (info.Length > 0)
            {
                day = (TrainingDayDTO)info[0].Tag;
            }
            else if (ReadOnly)
            {
                return;
            }

            if (day.Id == Constants.UnsavedObjectId)
            {
                //set default entries for newly created TrainingDay
                var options = UserContext.Settings.GuiState.CalendarOptions;
                foreach (var defaultEntry in options.DefaultEntries)
                {
                    if (defaultEntry.Value)
                    {
                        var plugin = PluginsManager.Instance.GetEntryObjectProvider(defaultEntry.Key);
                        if (plugin != null)
                        {
                            day.AddEntry(plugin.EntryObjectType);
                        }
                    }
                }
                //needed for SizeEntryDTO for example
                day.ChangeDate(selectedDate);
            }
            dlg.Fill(day, User);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //save day only when we didn't press Delete button (deleting is performed directly in usrAddTrainingDay control)
                    if (dlg.DayRemoved)
                    {
                        monthCalendar1.RemoveDateInfo(dlg.CurrentDay.TrainingDate);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorRemoveTrainingDay, ErrorWindow.EMailReport);
                    return;
                }
            }
            //day = ServiceManager.Instance.SaveTrainingDay(UserContext.Token, day);
            if (dlg.FillRequired)
            {
                Fill(User, day.TrainingDate);
            }
        }

        private void mnuOpenTrainingDay_Click(object sender, EventArgs e)
        {
            if (SelectedDate != null)
            {
                openTrainingDay(SelectedDate.Value);
            }
        }

        private void monthCalendar1_DayClick(object sender, DayClickEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                monthCalendar1.SelectDate(DateTime.Parse(e.Date));
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Left && SelectedTrainingDay != null)
            {
                DoDragDrop(SelectedTrainingDay, DragDropEffects.Copy | DragDropEffects.Move);
            }
        }

        private void monthCalendar1_DayDragDrop(object sender, DayDragDropEventArgs e)
        {
            try
            {
                TrainingDayDTO day = (TrainingDayDTO)e.Data.GetData(typeof(TrainingDayDTO));
                if ((e.KeyState & 8) == 8)
                {
                    //using (var scope = new TransactionScope())
                    //{
                    //    day = TrainingDay.GetById(day.Id);
                    //    var newDay = day.Copy();
                    //    if (newDay.Objects.Count == 0)
                    //    {
                    //        FMMessageBox.ShowError(ApplicationStrings.ErrorCannotCopyTrainingDayFixedEntries);
                    //        return;
                    //    }

                    //    newDay.ChangeDate(e.Date);
                    //    newDay.Save();
                    //    scope.VoteCommit();
                    //}
                    if (day.Id != Constants.UnsavedObjectId)
                    {//user select Cut operation so we should only move this one time (not many time like with Copy operation)
                        WorkoutDayGetOperation operation = new WorkoutDayGetOperation();
                        operation.UserId = User.Id;
                        operation.Operation = GetOperation.Current;
                        operation.WorkoutDateTime = day.TrainingDate;
                        day = ServiceManager.GetTrainingDay(operation);
                    }
                    day = day.Copy();
                    day.ChangeDate(e.Date);
                    ServiceManager.SaveTrainingDay(day);
                    this.Fill(User,e.Date);
                }
                else if (FMMessageBox.AskYesNo(ApplicationStrings.QMoveTrainingDay) == DialogResult.Yes)
                {
                    if (day.CanMove)
                    {
                        //using (var scope = new TransactionScope())
                        //{
                        //    day = TrainingDay.GetById(day.Id);
                        //    day.ChangeDate(e.Date);
                        //    day.Save();
                        //    scope.VoteCommit();
                        //}
                        if (day.Id != Constants.UnsavedObjectId)
                        {//user select Cut operation so we should only move this one time (not many time like with Copy operation)
                            WorkoutDayGetOperation operation = new WorkoutDayGetOperation();
                            operation.UserId = User.Id;
                            operation.Operation = GetOperation.Current;
                            operation.WorkoutDateTime = day.TrainingDate;
                            day = ServiceManager.GetTrainingDay(operation);
                        }
                        day.ChangeDate(e.Date);
                        ServiceManager.SaveTrainingDay(day);
                        this.Fill(User,e.Date);
                    }
                    else
                    {
                        FMMessageBox.ShowError(ApplicationStrings.ErrorCannotMoveTrainingDayFixedEntries);
                    }

                }
                
            }
            catch (OldDataException ex)
            {
                ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorOldTrainingDay, ErrorWindow.MessageBox);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorMoveTrainingDay, ErrorWindow.EMailReport);
            }
        }

        private void monthCalendar1_DragOver(object sender, DragEventArgs e)
        {
            DateTime? time = monthCalendar1.GetDateAt(e.X, e.Y);
            if (time.HasValue)
            {
                TrainingDayDTO day = getTrainingDay(time.Value);
                if (day == null)
                {

                    if ((e.KeyState & 8) == 8 && (e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
                    {
                        e.Effect = DragDropEffects.Copy;
                    }
                    else if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
                    {
                        e.Effect = DragDropEffects.Move;
                    }
                    return;
                }
            }
            e.Effect = DragDropEffects.None;
        }

        private void mnuEditCut_Click(object sender, EventArgs e)
        {
            Cut();
        }

        private void mnuEditCopy_Click(object sender, EventArgs e)
        {
            Copy();
        }

        private void mnuEditPaste_Click(object sender, EventArgs e)
        {
            Paste();
        }

        private void monthCalendar1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                deleteTrainingDay();
            }
        }

        private void deleteTrainingDay()
        {
            TrainingDayDTO day = SelectedTrainingDay;
            if (!ReadOnly && day != null && DomainObjectOperatonHelper.DeleteTrainingDay(UserContext.SessionData, day))
            {
                monthCalendar1.RemoveDateInfo(day.TrainingDate);
            }
            
            
        }

        public void GoToTheLastEntry()
        {
            WorkoutDayGetOperation operation = new WorkoutDayGetOperation();
            operation.UserId = User.Id;
            operation.Operation = GetOperation.Last;
            TrainingDayDTO day = ServiceManager.GetTrainingDay( operation);
            if (day != null)
            {
                SetActiveMonth(day.TrainingDate);
            }
            else
            {
                FMMessageBox.ShowInfo(ApplicationStrings.InfoNoTrainingDayEntires);
            }
        }

        public void GoToTheFirstEntry()
        {
            WorkoutDayGetOperation operation = new WorkoutDayGetOperation();
            operation.UserId = User.Id;
            operation.Operation = GetOperation.First;
            TrainingDayDTO day = ServiceManager.GetTrainingDay( operation);
            if (day != null)
            {
                SetActiveMonth(day.TrainingDate);
            }
            else
            {
                FMMessageBox.ShowInfo(ApplicationStrings.InfoNoTrainingDayEntires);
            }
        }
    }
}
