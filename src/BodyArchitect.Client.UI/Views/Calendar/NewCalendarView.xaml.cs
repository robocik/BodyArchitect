using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
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
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI.Cache;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Controls.Calendar;
using BodyArchitect.Client.UI.Controls.Calendar.Common;
using BodyArchitect.Client.UI.UserControls;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Portable;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Shared;
using vhCalendar;

namespace BodyArchitect.Client.UI.Views.Calendar
{
    [Serializable]
    public class CalendarPageContext : PageContext
    {
        public CalendarPageContext(UserDTO user, CustomerDTO customer) : base(user, customer)
        {
            ActiveMonthDateTime = DateTime.Now.Date;
        }

        public CalendarPageContext()
        {
        }

        public DateTime ActiveMonthDateTime
        {
            get; set;
        }
    }

    public partial class NewCalendarView : IWeakEventListener
    {
        private bool isFilled;
        private CancellationTokenSource fillCancelSource;
        private TrainingDaysReposidory cache;
        

        public NewCalendarView()
        {
            InitializeComponent();

            DataContext = this;
            entriesViewer.CalendarControl.ExDrop += monthCalendar1_Drop;
            entriesViewer.CalendarControl.ExDragOver += monthCalendar1_DragOver;
            entriesViewer.CalendarControl.CanStartDragDrop += monthCalendar1_CanStartDragDrop;
            entriesViewer.CalendarControl.DayDoubleClick += monthCalendar1_DayDoubleClick;
            entriesViewer.CalendarControl.DisplayDateChanged += new DisplayDateChangedEventHandler(monthCalendar1_DisplayDateChanged);
        }

        

        void monthCalendar1_DisplayDateChanged(object sender, DisplayDateChangedEventArgs e)
        {
            ActiveMonthDateTime = e.NewDate;
            Fill(User, ActiveMonthDateTime.MonthDate());
        }

        #region Context menu properties
        public Visibility MnuDeleteDayVisbility
        {
            get { return _mnuDeleteDayVisibility; }
            set
            {
                _mnuDeleteDayVisibility = value;
                NotifyOfPropertyChange(()=>MnuDeleteDayVisbility);
            }
        }
        public Visibility MnuOpenTrainingDayVisbility
        {
            get { return _mnuOpenTrainingDayVisibility; }
            set
            {
                _mnuOpenTrainingDayVisibility = value;
                NotifyOfPropertyChange(() => MnuOpenTrainingDayVisbility);
            }
        }
        public Visibility MnuNewTrainingDayVisbility
        {
            get { return _mnuNewTrainingDayVisibility; }
            set
            {
                _mnuNewTrainingDayVisibility = value;
                NotifyOfPropertyChange(() => MnuNewTrainingDayVisbility);
            }
        }
        public Visibility MnuEditCopyVisbility
        {
            get { return _mnuEditCopyVisibility; }
            set
            {
                _mnuEditCopyVisibility = value;
                NotifyOfPropertyChange(() => MnuEditCopyVisbility);
            }
        }
        public Visibility MnuEditCutVisbility
        {
            get { return _mnuEditCutVisibility; }
            set
            {
                _mnuEditCutVisibility = value;
                NotifyOfPropertyChange(() => MnuEditCutVisbility);
            }
        }
        public Visibility MnuEditPasteVisbility
        {
            get { return _mnuEditPasteVisibility; }
            set
            {
                _mnuEditPasteVisibility = value;
                NotifyOfPropertyChange(() => MnuEditPasteVisbility);
            }
        }
        public Visibility MnuSeparatorVisbility
        {
            get { return _mnuSeparatorVisibility; }
            set
            {
                _mnuSeparatorVisibility = value;
                NotifyOfPropertyChange(() => MnuSeparatorVisbility);
            }
        }
        public bool MnuEditCopyEnabled
        {
            get { return _mnuEditCopyEnabled; }
            set
            {
                _mnuEditCopyEnabled = value;
                NotifyOfPropertyChange(() => MnuEditCopyEnabled);
            }
        }
        public bool MnuEditCutEnabled
        {
            get { return _mnuEditCutEnabled; }
            set
            {
                _mnuEditCutEnabled = value;
                NotifyOfPropertyChange(() => MnuEditCutEnabled);
            }
        }
        public bool MnuEditPasteEnabled
        {
            get { return _mnuEditPasteEnabled; }
            set
            {
                _mnuEditPasteEnabled = value;
                NotifyOfPropertyChange(() => MnuEditPasteEnabled);
            }
        }
        #endregion

        public bool IsFilterViewAll
        {
            get { return entriesViewer.FilterView == CalendarFilter.All; }
        }

        public bool IsFilterViewOnlyPlanned
        {
            get { return entriesViewer.FilterView == CalendarFilter.OnlyPlanned; }
        }

        public bool IsFilterViewOnlyDone
        {
            get { return entriesViewer.FilterView == CalendarFilter.OnlyDone; }
        }

        public bool ReadOnly
        {
            get { return User != null && User.GlobalId != UserContext.Current.CurrentProfile.GlobalId; }
        }

        private DateTime _activeMonthDateTime;
        private Visibility _mnuDeleteDayVisibility;
        private Visibility _mnuOpenTrainingDayVisibility;
        private Visibility _mnuNewTrainingDayVisibility;
        private Visibility _mnuEditCopyVisibility;
        private Visibility _mnuEditCutVisibility;
        private Visibility _mnuEditPasteVisibility;
        private Visibility _mnuSeparatorVisibility;
        private bool _mnuEditCopyEnabled;
        private bool _mnuEditPasteEnabled;
        private bool _mnuEditCutEnabled;

        public DateTime ActiveMonthDateTime
        {
            get { return _activeMonthDateTime; }
            set
            {
                _activeMonthDateTime = value;
                CalendarPageContext.ActiveMonthDateTime = value;
                NotifyOfPropertyChange(()=>ActiveMonthDateTime);
            }
        }

        public CalendarPageContext CalendarPageContext
        {
            get
            {
                if (PageContext==null)
                {
                    PageContext = new CalendarPageContext(null,null);
                }
                return (CalendarPageContext)PageContext;
            }
        }

        public DateTime SelectedDate
        {
            get { return entriesViewer.CalendarControl.SelectedDate; }
        }



        public override Uri HeaderIcon
        {
            get { return "Calendar16.png".ToResourceUrl(); }
        }


        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            var notify = (NotifyCollectionChangedEventArgs)e;
            UIHelper.BeginInvoke(delegate
            {
                if(notify.Action==NotifyCollectionChangedAction.Add)
                {
                    entriesViewer.UpdateDay((TrainingDayDTO)notify.NewItems[0]);
                }
                else if (notify.Action == NotifyCollectionChangedAction.Remove)
                {
                    entriesViewer.RemoveDay((TrainingDayDTO)notify.OldItems[0]);
                }
                else
                {
                    entriesViewer.ItemsSource = cache.Days.Values;
                }
            }, Dispatcher);
            return true;
        }

        public override void Fill()
        {
            setHeader();
            ActiveMonthDateTime = CalendarPageContext.ActiveMonthDateTime.MonthDate();
            Fill(User, ActiveMonthDateTime);
        }

        private void setHeader()
        {
            string title = EnumLocalizer.Default.GetStringsString("NewCalendarView_setHeader_Calendar");
            if (Customer != null)
            {
                title += Customer.FullName;
            }
            else
            {
                title += User.UserName;
            }
            Header = title;
        }

        public override void RefreshView()
        {
            cache.Clear();
            Fill();
        }

        private void fillImplementation(DateTime activeMonth, UserDTO user, OperationContext context)
        {
            try
            {

                DateTime startDate = DateHelper.GetFirstDayOfMonth(activeMonth);
                DateTime endDate = DateHelper.GetLastDayOfMonth(activeMonth);
                //WorkoutDaysSearchCriteria searchCriteria = new WorkoutDaysSearchCriteria();
                //searchCriteria.StartDate = startDate;
                //searchCriteria.EndDate = endDate;
                //if(Customer!=null)
                //{
                //    searchCriteria.CustomerId = Customer.GlobalId;
                //}
                //searchCriteria.UserId = user.Id;
                //var pageInfo = new PartialRetrievingInfo();
                //pageInfo.PageSize = PartialRetrievingInfo.AllElementsPageSize;
                //var days = ServiceManager.GetTrainingDays(searchCriteria, pageInfo);
                if (!cache.IsMonthLoaded(activeMonth))
                {
                    cache.RetrieveDays(startDate, endDate);
                }

                if (context != null && context.CancellatioToken.IsCancellationRequested)
                {
                    return;
                }
                
                
                if (ActiveMonthDateTime.Month == activeMonth.Month && ActiveMonthDateTime.Year == activeMonth.Year)
                {
                    UIHelper.BeginInvoke(new Action(delegate
                    {
                        entriesViewer.Fill(cache.Days.Values);
                    }),Dispatcher);
                }
            }
            finally
            {
                isFilled = true;
            }
        }

        void ensureCacheExists()
        {
            if (cache == null)
            {
                Guid? customerId = Customer != null ? Customer.GlobalId : (Guid?) null;
                Guid? userId = User != null ? User.GlobalId : (Guid?)null;
                cache = TrainingDaysReposidory.GetCache(customerId, userId);
                CollectionChangedEventManager.AddListener(cache, this);
            }
        }

        

        private void Fill(UserDTO user, DateTime activeMonth)
        {
            ensureCacheExists();
            isFilled = false;
            if (UserContext.Current.IsConnected && UserContext.Current.LoginStatus == LoginStatus.Logged)
            {
                if (fillCancelSource != null)
                {
                    fillCancelSource.Cancel();
                    fillCancelSource = null;
                }
                fillCancelSource = ParentWindow.RunAsynchronousOperation(delegate(OperationContext context)
                {
                    try
                    {
                        fillImplementation(activeMonth, user, context);
                    }
                    catch (Exception ex)
                    {
                        UIHelper.BeginInvoke(new Action(() => ExceptionHandler.Default.Process(ex, EnumLocalizer.Default.GetStringsString("Exception_NewCalendarView_CannotRetrieveEntries_Fill"), ErrorWindow.EMailReport)), Dispatcher);
                    }
                });
            }
            else
            {
                entriesViewer.CalendarControl.Appointments.Clear();
            }

        }

        #region Copy/Paste

        public void Paste()
        {
            if(IsSelectedDateFuture && !UIHelper.EnsurePremiumLicence())
            {
                return;
            }
            if (Clipboard.ContainsData(Shared.Constants.ClipboardFormat))
            {
                if (SelectedTrainingDay == null)
                {
                    var day = (TrainingDayDTO)Clipboard.GetData(Shared.Constants.ClipboardFormat);
                    bool isCut = !day.IsNew;
                    var originalDay = day.StandardClone();
                    var selectedDate = entriesViewer.CalendarControl.SelectedDate;
                    var dayInfo = GetDayInfo(day.TrainingDate);
                    dayInfo.IsProcessing = true;
                    if (isCut)
                    {
                        Clipboard.Clear();
                    }
                    ParentWindow.RunAsynchronousOperation(x =>
                        {
                            try
                            {
                                if (isCut)
                                {
                                    //user select Cut operation so we should only move this one time (not many time like with Copy operation)
                                    //WorkoutDayGetOperation operation = new WorkoutDayGetOperation();
                                    //operation.UserId = User.Id;
                                    //operation.Operation = GetOperation.Current;
                                    //operation.WorkoutDateTime = day.TrainingDate;
                                    //day = ServiceManager.GetTrainingDay(operation);
                                    
                                }

                                day.ChangeDate(selectedDate);
                                var result1 = ServiceManager.SaveTrainingDay(day);
                                if (isCut)
                                {
                                    cache.Remove(originalDay.TrainingDate);
                                }
                                if (result1.TrainingDay!=null)
                                {
                                    day = result1.TrainingDay;
                                    cache.Add(day);    
                                }
                                
                                //ServiceManager.SaveTrainingDay(day);
                                //fillImplementation(day.TrainingDate, User, null);
                                //this.Fill(User, day.TrainingDate);
                            }
                            catch (OldDataException ex)
                            {
                                UIHelper.Invoke(() => ExceptionHandler.Default.Process(ex, EnumLocalizer.Default.GetStringsString("ErrorOldTrainingDay"),ErrorWindow.MessageBox),Dispatcher);
                            }
                            catch (Exception ex)
                            {
                                UIHelper.Invoke(() => ExceptionHandler.Default.Process(ex,EnumLocalizer.Default.GetStringsString("ErrorMoveTrainingDay"),ErrorWindow.EMailReport),Dispatcher);
                            }
                            finally
                            {
                                dayInfo.IsProcessing = false;
                            }
                        });
                }
                else
                {
                    BAMessageBox.ShowError(EnumLocalizer.Default.GetStringsString("ErrorCannotPaste"));
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
                    Clipboard.SetData(Shared.Constants.ClipboardFormat, day);
                }
                else
                {
                    BAMessageBox.ShowError(EnumLocalizer.Default.GetStringsString("ErrorCannotMoveTrainingDayFixedEntries"));
                }
            }
        }

        public void Copy()
        {
            var day = SelectedTrainingDay;
            if (day != null)
            {
                //PleaseWait.Run(delegate
                //{
                //    WorkoutDayGetOperation operation = new WorkoutDayGetOperation();
                //    operation.UserId = User.Id;
                //    operation.Operation = GetOperation.Current;
                //    operation.WorkoutDateTime = day.TrainingDate;
                //    day = ServiceManager.GetTrainingDay(operation);
                day = day.Copy();

                //},false,MainWindow.Instance);
                //if (day.Objects.Count == 0)
                //{
                //    BAMessageBox.ShowError(Strings.ErrorCannotCopyTrainingDayFixedEntries);
                //    return;
                //}
                Clipboard.SetData(Shared.Constants.ClipboardFormat, day);
            }
        }

        public bool CanPaste
        {
            get
            {
                return entriesViewer.CalendarControl.SelectedDate != null /*&& !IsSelectedDateFuture*/ && Clipboard.ContainsData(Shared.Constants.ClipboardFormat);
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

        public bool CanDelete
        {
            get { return CanCut; }
        }

        #endregion


        private void openTrainingDay(DateTime selectedDate)
        {
            if (!isFilled || UserContext.Current.LoginStatus != LoginStatus.Logged /*|| IsSelectedDateFuture*/)
            {
                return;
            }

            if (IsSelectedDateFuture && !UIHelper.EnsurePremiumLicence())
            {
                return;
            }

            var day = new TrainingDayDTO(selectedDate, UserContext.Current.CurrentProfile.GlobalId);
            if(Customer!=null)
            {
                day.CustomerId = Customer.GlobalId;
            }
            var info = entriesViewer.CalendarControl.GetDateInfo(selectedDate);
            if (info.Count > 0)
            {
                day = (TrainingDayDTO)info[0].Tag;
            }
            else if (ReadOnly)
            {
                return;
            }
            var dayInfo = GetDayInfo(day.TrainingDate);
            if (dayInfo != null)
            {
                dayInfo.IsProcessing = true;
            }
            //MainWindow.Instance.ShowView(delegate
            //    {
            //        var view= DomainObjectOperatonHelper.CreateTrainingDayWindow(day, User, Customer, null);
            //        if (dayInfo != null)
            //        {
            //            dayInfo.IsProcessing = false;
            //        }
            //        return view;
            //    });

            
            MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.UI;component/Windows/TrainingDayWindow.xaml"), () =>
                {
                    var view = DomainObjectOperatonHelper.CreateTrainingDayWindow(day, User, Customer, null);
                    if (dayInfo != null)
                    {
                        dayInfo.IsProcessing = false;
                    }
                    return view;
                });
            //MainWindow.Instance.ShowPage(delegate
            //{
            //    var view = DomainObjectOperatonHelper.CreateTrainingDayWindow(day, User, Customer, null);
            //    if (dayInfo != null)
            //    {
            //        dayInfo.IsProcessing = false;
            //    }
            //    return view;
            //});
        }

        private TrainingDayDTO getTrainingDay(DateTime dateTime)
        {
            var info = entriesViewer.CalendarControl.GetDateInfo(dateTime);
            return (TrainingDayDTO)(info.Count > 0 ? info[0].Tag : null);
        }

        public TrainingDayInfo GetDayInfo(DateTime date)
        {
            var dayInfoList = entriesViewer.CalendarControl.GetDateInfo(date);
            if(dayInfoList.Count>0)
            {
                var dayInfo = (TrainingDayInfo)dayInfoList[0];
                return dayInfo;    
            }
            return null;
        }

        public TrainingDayDTO SelectedTrainingDay
        {
            get
            {
                if (entriesViewer.CalendarControl.SelectedDate!=null)
                {
                    return getTrainingDay(entriesViewer.CalendarControl.SelectedDate);
                }
                return null;
            }
        }

        public bool IsSelectedDateFuture
        {
            get { return entriesViewer.CalendarControl.SelectedDate != null && entriesViewer.CalendarControl.SelectedDate.Date.IsFuture(); }
        }

        

        private void mnuCopy_Click(object sender, RoutedEventArgs e)
        {
            Copy();
        }

        private void monthCalendar1_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)e.OriginalSource;
            if(element.DataContext is DateInfo)
            {
                DateInfo dateInfo = (DateInfo)element.DataContext;
                entriesViewer.CalendarControl.SelectedDate = dateInfo.Date;
            }else if (element.DataContext is DateTime)
            {
                DateTime dateTime = (DateTime) element.DataContext;
                entriesViewer.CalendarControl.SelectedDate = dateTime;
            }

            TrainingDayDTO day = SelectedTrainingDay;
            if (!isFilled || UserContext.Current.LoginStatus != LoginStatus.Logged ||/* IsSelectedDateFuture ||*/ (day == null && ReadOnly/*we browse workout for another users so if there is no entry then we should skip*/))
            {
                e.Handled = true;
                return;
            }

            MnuDeleteDayVisbility = !ReadOnly && day != null ? Visibility.Visible : Visibility.Collapsed;
            //mnuDeleteDay.SetVisible(!ReadOnly && day != null);
            MnuOpenTrainingDayVisbility = day != null ? Visibility.Visible : Visibility.Collapsed;
            //mnuOpenTrainingDay.SetVisible(day != null);
            MnuNewTrainingDayVisbility = day == null && !ReadOnly ? Visibility.Visible : Visibility.Collapsed;
            //mnuNewTrainingDay.SetVisible(day == null && !ReadOnly);
            MnuEditCopyEnabled = CanCopy;
            //mnuEditCopy.IsEnabled = CanCopy;
            MnuEditCutEnabled = CanCut;
            //mnuEditCut.IsEnabled = CanCut;
            MnuEditPasteEnabled = CanPaste;
            //mnuEditPaste.IsEnabled = CanPaste;

            MnuEditCutVisbility=MnuEditPasteVisbility=MnuEditCopyVisbility = !ReadOnly ? Visibility.Visible : Visibility.Collapsed;
            //mnuEditCopy.SetVisible(!ReadOnly);
            //mnuEditCut.SetVisible(!ReadOnly);
            //mnuEditPaste.SetVisible(!ReadOnly);
            //menuSeparator.SetVisible(!ReadOnly);
        }

        private void mnuOpen_Click(object sender, RoutedEventArgs e)
        {
            if (entriesViewer.CalendarControl.SelectedDate != null)
            {
                openTrainingDay(entriesViewer.CalendarControl.SelectedDate);
            }
        }

        private void mnuNew_Click(object sender, RoutedEventArgs e)
        {
            if (entriesViewer.CalendarControl.SelectedDate != null)
            {
                openTrainingDay(entriesViewer.CalendarControl.SelectedDate);
            }
        }

        private void mnuDelete_Click(object sender, RoutedEventArgs e)
        {
            deleteTrainingDay();
        }

        private void mnuCut_Click(object sender, RoutedEventArgs e)
        {
            Cut();
        }

        private void mnuPaste_Click(object sender, RoutedEventArgs e)
        {
            Paste();
        }



        private void deleteTrainingDay()
        {
            TrainingDayDTO day = SelectedTrainingDay;
            if (!ReadOnly && day != null && DomainObjectOperatonHelper.DeleteTrainingDay(day,  GetDayInfo(day.TrainingDate)))
            {
                //entriesViewer.CalendarControl.RemoveDateInfo(day.TrainingDate);
            }
        }

        private void monthCalendar1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                deleteTrainingDay();
            }
        }

        public void GoToTheLastEntry()
        {
            WorkoutDayGetOperation operation = new WorkoutDayGetOperation();
            operation.UserId = User.GlobalId;
            operation.Operation = GetOperation.Last;
            TrainingDayDTO day = ServiceManager.GetTrainingDay(operation);
            if (day != null)
            {
                entriesViewer.CalendarControl.DisplayDate = day.TrainingDate;
            }
            else
            {
                BAMessageBox.ShowInfo("InfoNoTrainingDayEntires".TranslateStrings());
            }
        }

        public void GoToTheFirstEntry()
        {
            WorkoutDayGetOperation operation = new WorkoutDayGetOperation();
            operation.UserId = User.GlobalId;
            operation.Operation = GetOperation.First;
            TrainingDayDTO day = ServiceManager.GetTrainingDay(operation);
            if (day != null)
            {
                entriesViewer.CalendarControl.DisplayDate = day.TrainingDate;
            }
            else
            {
                BAMessageBox.ShowInfo("InfoNoTrainingDayEntires".TranslateStrings());
            }
        }

        private void btnFirstEntry_Click(object sender, RoutedEventArgs e)
        {
            GoToTheFirstEntry();
        }

        private void btnLastEntry_Click(object sender, RoutedEventArgs e)
        {
            GoToTheLastEntry();
        }

        private void btnToday_Click(object sender, RoutedEventArgs e)
        {
            entriesViewer.CalendarControl.DisplayDate = DateTime.Now;
        }

        private void monthCalendar1_Drop(object sender, RoutedEventArgs e)
        {
            ExDragEventArgs arg = (ExDragEventArgs)e;
            if (arg.DragEventArgs.Data.GetDataPresent("DateTime"))
            {
                DateTime date = (DateTime)arg.DragEventArgs.Data.GetData("DateTime");
                DateTime targetDate = (DateTime) arg.DirectTarget.Tag;
                var result=entriesViewer.CalendarControl.GetDateInfo(date);
                if(result.Count==1)
                {
                    var dayInfo = (TrainingDayInfo)result[0];
                    
                        TrainingDayDTO day = getTrainingDay(date);
                        if (targetDate.IsFuture() && !UIHelper.EnsurePremiumLicence())
                        {
                            return;
                        }
                        if (Keyboard.IsKeyDown(Key.LeftCtrl))
                        {
                            dayInfo.IsProcessing = true;
                            ParentWindow.RunAsynchronousOperation(x =>
                            {
                                //if (day.GlobalId != Constants.UnsavedGlobalId)
                                //{//user select Cut operation so we should only move this one time (not many time like with Copy operation)
                                //    WorkoutDayGetOperation operation = new WorkoutDayGetOperation();
                                //    operation.UserId = User.Id;
                                //    operation.Operation = GetOperation.Current;
                                //    operation.WorkoutDateTime = day.TrainingDate;
                                //    day = ServiceManager.GetTrainingDay(operation);
                                //}
                                try
                                {
                                    day = day.Copy();
                                    day.ChangeDate(targetDate);
                                    var result1=ServiceManager.SaveTrainingDay(day);
                                    if(result1.TrainingDay!=null)
                                    {
                                        cache.Add(result1.TrainingDay);    
                                    }
                                    
                                }
                                catch (OldDataException ex)
                                {
                                    UIHelper.Invoke(() => ExceptionHandler.Default.Process(ex, "ErrorOldTrainingDay".TranslateStrings(), ErrorWindow.MessageBox), Dispatcher);
                                }
                                catch (Exception ex)
                                {
                                    UIHelper.Invoke(() => ExceptionHandler.Default.Process(ex, "ErrorMoveTrainingDay".TranslateStrings(), ErrorWindow.EMailReport), Dispatcher);
                                }
                                finally
                                {
                                    dayInfo.IsProcessing = false;
                                }
                            });

                        }
                        else if (BAMessageBox.AskYesNo("QMoveTrainingDay".TranslateStrings()) == MessageBoxResult.Yes)
                        {
                            if (day.CanMove)
                            {
                                dayInfo.IsProcessing = true;
                                ParentWindow.RunAsynchronousOperation(x =>
                                {
                                    //if (day.GlobalId != Constants.UnsavedGlobalId)
                                    //{//user select Cut operation so we should only move this one time (not many time like with Copy operation)
                                    //    WorkoutDayGetOperation operation = new WorkoutDayGetOperation();
                                    //    operation.UserId = User.Id;
                                    //    operation.Operation = GetOperation.Current;
                                    //    operation.WorkoutDateTime = day.TrainingDate;
                                    //    day = ServiceManager.GetTrainingDay(operation);
                                    //}
                                    try
                                    {
                                        day = day.StandardClone();
                                        day.ChangeDate(targetDate);
                                        var result1 = ServiceManager.SaveTrainingDay(day);
                                        cache.Remove(date);
                                        
                                        if (result1.TrainingDay != null)
                                        {
                                            day = result1.TrainingDay;
                                            cache.Add(day);

                                        }
                                        
                                    }
                                    catch (OldDataException ex)
                                    {
                                        UIHelper.Invoke(() => ExceptionHandler.Default.Process(ex, "ErrorOldTrainingDay".TranslateStrings(), ErrorWindow.MessageBox), Dispatcher);
                                    }
                                    catch (Exception ex)
                                    {
                                        UIHelper.Invoke(() => ExceptionHandler.Default.Process(ex, "ErrorMoveTrainingDay".TranslateStrings(), ErrorWindow.EMailReport), Dispatcher);
                                    }
                                    finally
                                    {
                                        dayInfo.IsProcessing = false;
                                    }
                                    
                                    
                                });
                            }
                            else
                            {
                                BAMessageBox.ShowError("ErrorCannotMoveTrainingDayFixedEntries".TranslateStrings());
                            }

                        }

                    

                }
            }
        }

        void monthCalendar1_CanStartDragDrop(object sender, CanStartDragDrop e)
        {
            var day=getTrainingDay(e.SelectedDate);
            e.Cancel = day == null;
        }

        private void monthCalendar1_DragOver(object sender, RoutedEventArgs e)
        {
            ExDragEventArgs arg = (ExDragEventArgs)e;
            DateTime time = (DateTime)arg.DirectTarget.Tag;
            TrainingDayDTO day = getTrainingDay(time);
            if (day==null)
            {

                if ((arg.DragEventArgs.AllowedEffects & DragDropEffects.Copy) == DragDropEffects.Copy && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                {
                    arg.DragEventArgs.Effects = DragDropEffects.Copy;
                }
                else if ((arg.DragEventArgs.AllowedEffects & DragDropEffects.Move) == DragDropEffects.Move )
                {
                    arg.DragEventArgs.Effects = DragDropEffects.Move;
                }
                return;
            }
            arg.DragEventArgs.Effects = DragDropEffects.None;
            arg.DragEventArgs.Handled = true;
        }



        private void monthCalendar1_DayDoubleClick(object sender, RoutedEventArgs e)
        {
            var day = (DayDoubleClickEventArgs) e;
            openTrainingDay(day.NewDate);
        }

        private void rbtnFilterAll_Click(object sender, RoutedEventArgs e)
        {
            //BUG FIX
            e.Handled = true;
            entriesViewer.FilterView = CalendarFilter.All;
            updateRibbon();
        }

        private void rbtnFilterPlannedOnly_Click(object sender, RoutedEventArgs e)
        {
            //BUG FIX
            e.Handled = true;
            entriesViewer.FilterView = CalendarFilter.OnlyPlanned;
            updateRibbon();
        }

        private void rbtnFilterDoneOnly_Click(object sender, RoutedEventArgs e)
        {
            //BUG FIX
            e.Handled = true;
            entriesViewer.FilterView = CalendarFilter.OnlyDone;
            updateRibbon();
        }

        void updateRibbon()
        {
            NotifyOfPropertyChange(() => IsFilterViewAll);
            NotifyOfPropertyChange(() => IsFilterViewOnlyDone);
            NotifyOfPropertyChange(() => IsFilterViewOnlyPlanned);
        }
    }
}
