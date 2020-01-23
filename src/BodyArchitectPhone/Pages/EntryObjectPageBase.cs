using System;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using BodyArchitect.Portable.Exceptions;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.Client.WP7.ModelExtensions;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.Controls.Animations;
using BodyArchitect.WP7.UserControls;
using BodyArchitectCustom;
using BugSense;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Shell;
using ExtensionMethods = BodyArchitect.WP7.Controls.ExtensionMethods;

namespace BodyArchitect.WP7.Pages
{
    public abstract class EntryObjectPageBase : SimpleEntryObjectPage<EntryObjectDTO>
    {
        private ProgressStatus progressBar;
        private Pivot pivot;
        private bool isNextPending = false;
        private bool IsClosing { get; set; }
        private ApplicationBarIconButton btnSave;

        public bool ReadOnly
        {
            get
            {
                if (ApplicationState.Current.TrainingDay != null)
                {
                    //var entry = ApplicationState.Current.TrainingDay.TrainingDay.GetEntry(EntryType);
                    return !ApplicationState.Current.CurrentBrowsingTrainingDays.IsMine || (Entry != null && Entry.Status == EntryObjectStatus.System);
                }
                return true;
            }
        }
        public static bool EnsureRemoveEntryTypeFromToday(Type entryType)
        {
            if (ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays.ContainsKey(DateTime.Now.Date))
            {//if today already has training day object then we should use it
                var day = ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays[DateTime.Now.Date].Copy();
                var entriesWithTheSameType=day.TrainingDay.Objects.Where(x => x.Status != EntryObjectStatus.System && x.GetType() == entryType).ToList();

                if (entriesWithTheSameType.Count>0)
                {
                    //inform user that he has the same type of entry in today and if he continue, then we add a new entry to today (without overwriting existing one)
                    if (BAMessageBox.Ask(ApplicationStrings.EntryObjectPageBase_QCopyToTodayOverwrite) == MessageBoxResult.Cancel)
                    {
                        return false;
                    }
                    //else
                    //{
                    //    //instead of overwriting, we basically add this new entry to today
                    //    //overwrite
                    //    //day.TrainingDay.Objects.Remove(existingEntry);
                    //}

                }
                ApplicationState.Current.TrainingDay = day;
            }
            else
            {
                ApplicationState.Current.TrainingDay = new TrainingDayInfo(new TrainingDayDTO());
                ApplicationState.Current.TrainingDay.TrainingDay.ProfileId = ApplicationState.Current.SessionData.Profile.GlobalId;
                if(ApplicationState.Current.CurrentViewCustomer!=null)
                {
                    ApplicationState.Current.TrainingDay.TrainingDay.CustomerId =ApplicationState.Current.CurrentViewCustomer.GlobalId;
                }
                ApplicationState.Current.TrainingDay.TrainingDay.TrainingDate = DateTime.Now.Date;
            }
            return true;
        }

        protected virtual void PrepareCopiedEntry(EntryObjectDTO origEntry,EntryObjectDTO entry)
        {
            
        }

        protected void CopyEntryToToday()
        {
            //copy only if selected entry is not today entry
            if (ApplicationState.Current.CurrentBrowsingTrainingDays.IsMine && ApplicationState.Current.TrainingDay.TrainingDay.TrainingDate != DateTime.Now.Date)
            {
                //var origEntry = ApplicationState.Current.TrainingDay.TrainingDay.GetEntry(EntryType);
                var origEntry = Entry;
                var entry = origEntry.Copy(true);

                if(!EnsureRemoveEntryTypeFromToday(EntryType))
                {//cancel overwrite
                    return;
                }
                PrepareCopiedEntry(origEntry,entry);


                ApplicationState.Current.TrainingDay.TrainingDay.Objects.Add(entry);
                entry.TrainingDay = ApplicationState.Current.TrainingDay.TrainingDay;
                ApplicationState.Current.CurrentEntryId=new LocalObjectKey(entry);
                show(true);
                BAMessageBox.ShowInfo(ApplicationStrings.EntryObjectPageBase_CopyEntryToTodayCompleted);
            }
            
        }


        protected void SetControls(ProgressStatus progress, Pivot pivot)
        {
            progressBar = progress;
            this.pivot = pivot;
        }

        public bool CanSave
        {
            get { return btnSave!=null && btnSave.IsEnabled; }
            set
            {
                if (btnSave != null)
                {
                    btnSave.IsEnabled = value;
                }
            }
        }

        protected virtual void buildApplicationBar()
        {
            if (ApplicationState.Current == null || ApplicationState.Current.TrainingDay==null)
            {
                return;
            }
            ApplicationBar = ExtensionMethods.CreateApplicationBar();
            ApplicationBarIconButton button1;
            ApplicationBarMenuItem menu;

            if (!ReadOnly)
            {
                btnSave = new ApplicationBarIconButton(new Uri("/icons/appbar.save.rest.png", UriKind.Relative));
                btnSave.Click += new EventHandler(btnSave_Click);
                btnSave.Text = ApplicationStrings.AppBarButton_Save;
                ApplicationBar.Buttons.Add(btnSave);
            }
            button1 = new ApplicationBarIconButton(new Uri("/icons/appbar.back.rest.png", UriKind.Relative));
            button1.Click += new EventHandler(btnShowPrevious_Click);
            button1.Text = ApplicationStrings.AppBarButton_Previous;
            ApplicationBar.Buttons.Add(button1);

            button1 = new ApplicationBarIconButton(new Uri("/icons/appbar.next.rest.png", UriKind.Relative));
            button1.Click += new EventHandler(btnShowNext_Click);
            button1.Text = ApplicationStrings.AppBarButton_Next;
            ApplicationBar.Buttons.Add(button1);

            if (!ReadOnly)
            {
                if (!ApplicationState.Current.IsOffline)
                {
                    menu = new ApplicationBarMenuItem(ApplicationStrings.AppBarMenu_Delete);
                    menu.Click += new EventHandler(btnDelete_Click);
                    ApplicationBar.MenuItems.Add(menu);
                }
                //we add copy to today menu only when the current entry is not from today
                if (ApplicationState.Current.TrainingDay.TrainingDay.TrainingDate.Date != DateTime.Now.Date)
                {
                    menu = new ApplicationBarMenuItem(ApplicationStrings.AppBarMenu_CopyToToday);
                    menu.Click += new EventHandler(mnuCopyEntryToToday_Click);
                    ApplicationBar.MenuItems.Add(menu);
                }
            }
            //ApplicationBar.IsMenuEnabled = !ApplicationState.Current.IsOffline;

            if (!ApplicationState.Current.IsOffline)
            {
                menu = new ApplicationBarMenuItem(ApplicationStrings.AppBarButton_Refresh);
                menu.Click += new EventHandler(mnuRefresh_Click);
                ApplicationBar.MenuItems.Add(menu);
            }
        }

        private void mnuCopyEntryToToday_Click(object sender, EventArgs e)
        {
            CopyEntryToToday();
        }

        void mnuRefresh_Click(object sender, EventArgs e)
        {
            var m = new ServiceManager<GetTrainingDayCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<GetTrainingDayCompletedEventArgs> operationCompleted)
            {
                client1.GetTrainingDayCompleted -= operationCompleted;
                client1.GetTrainingDayCompleted += operationCompleted;
                WorkoutDayGetOperation data = new WorkoutDayGetOperation();
                data.WorkoutDateTime = ApplicationState.Current.TrainingDay.TrainingDay.TrainingDate;
                data.UserId = ApplicationState.Current.TrainingDay.TrainingDay.ProfileId;
                data.CustomerId = ApplicationState.Current.TrainingDay.TrainingDay.CustomerId;
                data.Operation = GetOperation.Current;
                client1.GetTrainingDayAsync(ApplicationState.Current.SessionData.Token, data, new RetrievingInfo());

            });


            m.OperationCompleted += (s, a) =>
            {
                progressBar.ShowProgress(false);
                if (ApplicationState.Current.TrainingDay == null || IsClosing)
                {
                    return;
                }
                if (a.Error != null)
                {
                    BAMessageBox.ShowError(ApplicationStrings.EntryObjectPageBase_ErrDuringRefresh);
                }
                else
                {
                    OfflineModeManager manager = new OfflineModeManager(ApplicationState.Current.MyDays, ApplicationState.Current.SessionData.Profile.GlobalId);
                    manager.MergeNew(a.Result.Result, ApplicationState.Current,false, delegate
                                {
                                    return BAMessageBox.Ask(ApplicationStrings.EntryObjectPageBase_QResfreshMerge) == MessageBoxResult.OK;
                                });

                    show(true);
                }

            };
            progressBar.ShowProgress(true, ApplicationStrings.EntryObjectPageBase_ProgressRefresh);
            if (!m.Run())
            {
                progressBar.ShowProgress(false);
                if (ApplicationState.Current.IsOffline)
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrOfflineMode);
                }
                else
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrNoNetwork);
                }
            }
        }

        protected abstract void btnDelete_Click(object sender, EventArgs e);

        private void btnSave_Click(object sender, EventArgs e)
        {
            saveToTheDb();
        }

        private void btnShowPrevious_Click(object sender, EventArgs e)
        {
            showPrevious();
        }

        private void btnShowNext_Click(object sender, EventArgs e)
        {
            showNext();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (!IsPopupOpen() && isModified() && BAMessageBox.Ask(ApplicationStrings.EntryObjectPageBase_BackKeyQuestion) == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
                return;
            }

            if (!IsPopupOpen())
            {
                if (!BeforeClose())
                {
                    e.Cancel = true;
                    return;
                }
                IsClosing = true;
                ApplicationState.Current.TrainingDay = null;
            }
            base.OnBackKeyPress(e);
        }

        protected virtual bool BeforeClose()
        {
            return true;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (ApplicationState.Current.TrainingDay != null)
            {
                show( false);
            }
            buildApplicationBar();
            if (pivot != null)
            {
                StateHelper stateHelper = new StateHelper(this.State);
                var pivotItem = stateHelper.GetValue<int>("PivotSelectedTab", 0);
                pivot.SelectedIndex = pivotItem;
            }

        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            ExtensionMethods.BindFocusedTextBox();
            base.OnNavigatedFrom(e);
            ApplicationState.Current.TrainingDaysRetrieved -= Current_TrainingDaysRetrieved;

            if (pivot != null)
            {
                this.State["PivotSelectedTab"] = pivot.SelectedIndex;
            }
        }

        protected bool isModified()
        {
            if (ApplicationState.Current == null || ApplicationState.Current.TrainingDay == null || ReadOnly)
            {
                return false;
            }
            TrainingDayInfo orgDay;
            ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays.TryGetValue(ApplicationState.Current.TrainingDay.TrainingDay.TrainingDate, out orgDay);
            if (orgDay == null)
            {
                return true;
            }

            return ApplicationState.Current.TrainingDay.TrainingDay.IsModified(orgDay.TrainingDay);
        }

        async protected Task deleteEntry(EntryObjectDTO dto)
        {
            if (!dto.IsNew)
            {
                //send request to the db only when we removed persisted entry
                ApplicationState.Current.TrainingDay.TrainingDay.Objects.Remove(dto);
                ApplicationState.Current.TrainingDay.CleanUpGpsCoordinates();
                await saveToTheDb(true);
            }
            else
            {
                if (!ReadOnly && ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays.ContainsKey(dto.TrainingDay.TrainingDate))
                {
                    //this situation is when entry is added in offline mode
                    var day = ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays[dto.TrainingDay.TrainingDate];
                    
                    if(day.TrainingDay.Objects.Count==1)//only this entry
                    {
                        ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays.Remove(dto.TrainingDay.TrainingDate);    
                    }
                    else
                    {
                        var itemToRemove = day.TrainingDay.Objects.Where(x => x.InstanceId == dto.InstanceId).SingleOrDefault();
                        if (itemToRemove != null)
                        {
                            day.TrainingDay.Objects.Remove(itemToRemove);
                        }
                        else
                        {//don't know if this else is invoked. everything should be catch by if block
                            day.TrainingDay.Objects.Remove(dto);
                        }
                        ApplicationState.Current.TrainingDay.CleanUpGpsCoordinates();
                    }

                }
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
        }

        async protected virtual Task SavingCompleted()
        {

        }

        protected virtual void BeforeSaving()
        {
        }

        protected virtual bool ValidateBeforeSave()
        {
            return true;
        }

        async protected Task saveToTheDb(bool goBack=false)
        {
            if (ApplicationState.Current==null)
            {
                return;
            }

            if (ReadOnly)
            {
                //Translation i huj wie co
                BAMessageBox.ShowError("Cannot modify entry for another user");
                return;
            }
            ExtensionMethods.BindFocusedTextBox();
            if(!ValidateBeforeSave())
            {
                return;
            }

            progressBar.ShowProgress(true,ApplicationStrings.EntryObjectPageBase_ProgressUpdating);
            BeforeSaving();
            DateTime date = ApplicationState.Current.TrainingDay.TrainingDay.TrainingDate;

            //first save a copy to the local storage
            ThreadPool.QueueUserWorkItem(delegate
            {
                var copy = ApplicationState.Current.TrainingDay.Copy();
                copy.IsModified = true;
                ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays[copy.TrainingDay.TrainingDate] = copy;
            });

            try
            {
                var entryBeforeSave = Entry;//see the comment where this is used belove
                var result = await BAService.SaveTrainingDayAsync(ApplicationState.Current.TrainingDay.TrainingDay);
                if (result.TrainingDay != null)
                {
                    var savedDay = result.TrainingDay;
                    savedDay.FillInstaneId(ApplicationState.Current.TrainingDay.TrainingDay);
                    savedDay.TrainingDate = date;//bug fixing: web service converts date to local or something like this. but this is only in SaveTrainingDay method
                    var tdi = ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays[savedDay.TrainingDate];
                    tdi.TrainingDay = savedDay;
                    tdi.IsModified = false;
                    ApplicationState.Current.TrainingDay = tdi.Copy();
                    
                    //if we use instance id for determining current entry then after save we should always use globalid
                    if (Entry != null)
                    {
                        ApplicationState.Current.CurrentEntryId = new LocalObjectKey(Entry);    
                    }
                    
                    await SavingCompleted();

                    progressBar.ShowProgress(false);
                    if (result.NewRecords.Count > 0)
                    {
                        BAMessageBox.ShowInfo(ApplicationStrings.MessageNewRecords, new Uri("/Images/Records32.png", UriKind.RelativeOrAbsolute));
                    }
                }
                else
                {
                    progressBar.ShowProgress(false);
                    //if (ApplicationState.Current.TrainingDay != null && ApplicationState.Current.TrainingDay.TrainingDay.TrainingDate == date)
                    //{
                    //    ApplicationState.Current.TrainingDay = null;
                    //}
                    if (!goBack)
                    {//user remove all data from the trainig day so on the server this day is removed. so on the client we "mark" this item as a new item
                        ApplicationState.Current.TrainingDay.TrainingDay.GlobalId = Guid.Empty;
                        var tmpEntry = Entry;
                        tmpEntry.GlobalId = Guid.Empty;
                        tmpEntry.Version = 0;
                        ApplicationState.Current.CurrentEntryId=new LocalObjectKey(tmpEntry);
                    }
                    ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays.Remove(date);
                }
                if (ApplicationState.Current.TrainingDay != null && !IsClosing && !goBack)
                {
                    if (Entry == null && !ApplicationState.Current.TrainingDay.TrainingDay.Objects.Contains(entryBeforeSave))
                    {//this is invoked when for example user saved empty supplements entry so on the server this entry is removed and therefore after save Entry returns null. so we must restore the entry before saving operation (this is a special case)
                        ApplicationState.Current.TrainingDay.TrainingDay.Objects.Add(entryBeforeSave);
                        entryBeforeSave.TrainingDay = ApplicationState.Current.TrainingDay.TrainingDay;
                        entryBeforeSave.GlobalId = Guid.Empty;
                        entryBeforeSave.Version = 0;
                    }
                    show(true);
                }

                if (goBack)
                {
                    ApplicationState.Current.TrainingDay = null;
                    if (NavigationService.CanGoBack && !IsClosing)
                    {
                        NavigationService.GoBack();
                    }
                }
            }
            catch (NetworkException)
            {
                progressBar.ShowProgress(false);
                if (ApplicationState.Current.IsOffline)
                {
                    BAMessageBox.ShowWarning(ApplicationStrings.EntryObjectPageBase_SavedLocallyOnly);
                }
                else
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrNoNetwork);
                }
            }
            catch (ValidationException validEx)
            {
                progressBar.ShowProgress(false);
                if (IsClosing)
                {
                    return;
                }
                BAMessageBox.ShowError(validEx.Results.First().Key + ":" + validEx.Results.First().Message);
            }
            catch (OldDataException oldData)
            {
                progressBar.ShowProgress(false);
                if (IsClosing)
                {
                    return;
                }
                BAMessageBox.ShowError(ApplicationStrings.ErrOldData);
            }
            catch (LicenceException licence)
            {
                progressBar.ShowProgress(false);
                if (IsClosing)
                {
                    return;
                }
                BAMessageBox.ShowError(ApplicationStrings.ErrLicence);
            }
            catch (Exception ex)
            {
                progressBar.ShowProgress(false);
                BugSenseHandler.Instance.SendExceptionAsync(ex);
                if (IsClosing)
                {
                    return;
                }
                
                BAMessageBox.ShowWarning(ApplicationStrings.EntryObjectPageBase_SavedLocallyOnly);
            }
            


            //var m = new ServiceManager<SaveTrainingDayCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<SaveTrainingDayCompletedEventArgs> operationCompleted)
            //{
            //    client1.SaveTrainingDayCompleted -= operationCompleted;
            //    client1.SaveTrainingDayCompleted += operationCompleted;
            //    client1.SaveTrainingDayAsync(ApplicationState.Current.SessionData.Token, ApplicationState.Current.TrainingDay.TrainingDay);

            //});


            //m.OperationCompleted += async (s, a) =>
            //{
            //    if (a.Error != null)
            //    {
            //        progressBar.ShowProgress(false);
            //        if(IsClosing)
            //        {
            //            return;
            //        }
            //        FaultException<ValidationFault> faultEx = a.Error as FaultException<ValidationFault>;
            //        FaultException<BAServiceException> serviceEx = a.Error as FaultException<BAServiceException>;
            //        if (faultEx != null)
            //        {
            //            BAMessageBox.ShowError(faultEx.Detail.Details[0].Key + ":" + faultEx.Detail.Details[0].Message);
            //            return;
            //        }
            //        if (serviceEx!=null)
            //        {
            //            if(serviceEx.Detail.ErrorCode==ErrorCode.OldDataException)
            //            {
            //                BAMessageBox.ShowError(ApplicationStrings.ErrOldData);
            //                return;
            //            }
            //            else if(serviceEx.Detail.ErrorCode==ErrorCode.LicenceException)
            //            {
            //                BAMessageBox.ShowError(ApplicationStrings.ErrLicence);
            //                return;
            //            }
            //        }

            //        BAMessageBox.ShowWarning(ApplicationStrings.EntryObjectPageBase_SavedLocallyOnly);
            //        return;
            //    }
            //    else
            //    {
            //        if (a.Result.Result.TrainingDay != null)
            //        {
            //            var savedDay = a.Result.Result.TrainingDay;
            //            //savedDay.InstanceId = dayInstanceId;
            //            //var savedEntry=savedDay.GetEntry(EntryType);
            //            //if (savedEntry != null)
            //            //{
            //            //    savedEntry.InstanceId = entryInstanceId;
            //            //}
            //            savedDay.FillInstaneId(ApplicationState.Current.TrainingDay.TrainingDay);
            //            savedDay.TrainingDate = date;//bug fixing: web service converts date to local or something like this. but this is only in SaveTrainingDay method
            //            var tdi=ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays[savedDay.TrainingDate];
            //            tdi.TrainingDay = savedDay;
            //            tdi.IsModified = false;
            //            ApplicationState.Current.TrainingDay = tdi.Copy();
            //            await SavingCompleted();

            //            progressBar.ShowProgress(false);
            //            if(a.Result.Result.NewRecords.Count>0)
            //            {
            //                BAMessageBox.ShowInfo(ApplicationStrings.MessageNewRecords, new Uri("/Images/Records32.png", UriKind.RelativeOrAbsolute));
            //            }
            //        }
            //        else
            //        {
            //            progressBar.ShowProgress(false);
            //            if (ApplicationState.Current.TrainingDay != null && ApplicationState.Current.TrainingDay.TrainingDay.TrainingDate == date)
            //            {
            //                ApplicationState.Current.TrainingDay = null;
            //            }
            //            ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays.Remove(date);
            //        }
            //        if (ApplicationState.Current.TrainingDay != null && !IsClosing)
            //        {
            //            show(ApplicationState.Current.TrainingDay.TrainingDay, true);
            //        }

            //        if (goBack)
            //        {
            //            ApplicationState.Current.TrainingDay = null;
            //            if (NavigationService.CanGoBack && !IsClosing)
            //            {
            //                NavigationService.GoBack();
            //            }
            //        }
            //    }

            //};

            //if(!m.Run())
            //{
            //    progressBar.ShowProgress(false);
            //    if (ApplicationState.Current.IsOffline)
            //    {
            //        BAMessageBox.ShowWarning(ApplicationStrings.EntryObjectPageBase_SavedLocallyOnly);
            //    }
            //    else
            //    {
            //        BAMessageBox.ShowError(ApplicationStrings.ErrNoNetwork);
            //    }
            //}
        }

        protected abstract void show(bool reload);

        protected virtual void showPrevious()
        {
            //var list = ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays.Values.Where(day => day.TrainingDay.TrainingDate <= Entry.TrainingDay.TrainingDate).SelectMany(x => x.TrainingDay.Objects).OrderByDescending(x => x.TrainingDay.TrainingDate).Where(x => x.GetType() == EntryType).ToList();
            //var currentItem=list.Where(x =>x.IsSame(ApplicationState.Current.CurrentEntryId)).SingleOrDefault();
            //var position = list.IndexOf(currentItem);

            //var newEntry = list.Skip(position+1).FirstOrDefault();
            //showImplementation(newEntry,false);


            var list = ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays.Values.SelectMany(x => x.TrainingDay.Objects).OrderByDescending(x => x.TrainingDay.TrainingDate).Where(x => x.GetType() == EntryType ).ToList();
#if !RELEASE
            var currentItem = list.Where(x => x.IsSame(Entry)).SingleOrDefault();
#else
            var currentItem = list.Where(x => x.IsSame(Entry)).FirstOrDefault();
#endif
            var position = list.IndexOf(currentItem);

            if (position == list.Count - 1)
            {
                showImplementation(null, false);
            }
            else
            {
                var newEntry = list.ElementAt(position + 1);
                showImplementation(newEntry, false);
            }
        }

        protected virtual void showNext()
        {
            //var list = ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays.Values.Where(day => day.TrainingDay.TrainingDate >= Entry.TrainingDay.TrainingDate).SelectMany(x => x.TrainingDay.Objects).OrderBy(x => x.TrainingDay.TrainingDate).Where(x => x.GetType() == EntryType).ToList();
            //var currentItem = list.Where(x => x.IsSame(ApplicationState.Current.CurrentEntryId)).SingleOrDefault();
            //var position = list.IndexOf(currentItem);

            //var newEntry = list.Skip(position + 1).FirstOrDefault();
            //showImplementation(newEntry, true);
            var list = ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays.Values.SelectMany(x => x.TrainingDay.Objects).OrderByDescending(x => x.TrainingDay.TrainingDate).Where(x => x.GetType() == EntryType).ToList();
#if !RELEASE
            var currentItem = list.Where(x => x.IsSame(Entry)).SingleOrDefault();
#else
            var currentItem = list.Where(x => x.IsSame(Entry)).FirstOrDefault();
#endif
            var position = list.IndexOf(currentItem);

            if (position <= 0)
            {
                showImplementation(null, true);
            }
            else
            {
                var newEntry = list.ElementAt(position - 1);

                showImplementation(newEntry, true);
            }
        }
              

        protected abstract Type EntryType { get; }

        void showImplementation(EntryObjectDTO newEntry,bool next)
        {
            if (newEntry == null)
            {
                if(ApplicationState.Current.IsOffline)
                {
                    BAMessageBox.ShowError(ApplicationStrings.EntryObjectPageBase_ErrOfflineRetrieveEntries);
                    return;
                }
                if(BAMessageBox.Ask(ApplicationStrings.EntryObjectPageBase_QRetrieveNextMonth)==MessageBoxResult.OK)
                {
                    isNextPending = next;
                    progressBar.ShowProgress(true, ApplicationStrings.TrainingDaySelectorControl_ProgressRetrieveEntries);
                    IsHitTestVisible = false;
                    DateTime monthDate;
                    if (next)
                    {
                        var lastDay = ApplicationState.Current.CurrentBrowsingTrainingDays.GetLastLoadedEntry();
                        if (lastDay == null)
                        {
                            monthDate = DateTime.Now; 
                        }
                        else
                        {
                            monthDate = lastDay.TrainingDate;
                        }
                        monthDate = monthDate.AddMonths(1).MonthDate();
                    }
                    else
                    {
                        var firstDay = ApplicationState.Current.CurrentBrowsingTrainingDays.GetFirstLoadedEntry();
                        if (firstDay==null)
                        {
                            monthDate = DateTime.Now;
                        }
                        else
                        {
                            monthDate = firstDay.TrainingDate;
                        }
                        monthDate = monthDate.AddMonths(-1).MonthDate();
                    }
                    ApplicationState.Current.TrainingDaysRetrieved += Current_TrainingDaysRetrieved;
                    ApplicationState.Current.RetrieveMonth( monthDate, ApplicationState.Current.CurrentBrowsingTrainingDays);
                }
                return;
            }
            if (isModified() && BAMessageBox.Ask(ApplicationStrings.EntryObjectPageBase_BackKeyQuestion) == MessageBoxResult.Cancel)
            {
                return;
            }

            ApplicationState.Current.TrainingDay = ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays[newEntry.TrainingDay.TrainingDate].Copy();
            ApplicationState.Current.CurrentEntryId=new LocalObjectKey(newEntry);
            show(true);
        }

        private void Current_TrainingDaysRetrieved(object sender, DateEventArgs e)
        {
            ApplicationState.Current.TrainingDaysRetrieved -= Current_TrainingDaysRetrieved;
            progressBar.ShowProgress(false);
            IsHitTestVisible = true;
            if(isNextPending)
            {
                showNext();
            }
            else
            {
                showPrevious();
            }
        }
    }
}
