using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.Instructor.Controls;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Cache;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Controls.Calendar;
using BodyArchitect.Client.UI.Controls.Calendar.Common;
using BodyArchitect.Client.UI.Views.MyPlace;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Portable.Exceptions;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Shared;
using BodyArchitect.Client.UI.SchedulerEngine;

namespace BodyArchitect.Client.Module.Instructor.ViewModel
{
    public class ScheduleEntriesDesignViewModel : ViewModelBase, IWeakEventListener
    {
        private IBAWindow parentView;
        private DateTime startDate;
        private DateTime endDate;
        private ObservableCollection<ScheduleEntryBaseViewModel> entries = new ObservableCollection<ScheduleEntryBaseViewModel>();
        private IEnumerable<CustomerGroupDTO> groups;
        private ObservableCollection<ScheduleEntryReservationViewModel> reservations = new ObservableCollection<ScheduleEntryReservationViewModel>();
        private IEnumerable<ActivityDTO> activities;
        private IEnumerable<CustomerDTO> customers;
        private bool canSave=true;
        private CustomerDTO selectedCustomer;
        private CancellationTokenSource fillCancelSource;
        private bool readOnly = true;
        private bool canReserve;
        private bool canCancelReserve;
        private ScheduleEntryBaseViewModel selectedAppointment;
        private ScheduleEntryReservationViewModel selectedReservation;
        private bool canMarkAsPaid;
        private ScheduleEntriesCalendar calendar;
        private bool showGroupsSelector=false;
        private bool showActivitesSelector=false;
        private bool isModified;
        private ScheduleColorMode _colorMode;
        private ScheduleEntryBaseDTO[] originalEntries = new  ScheduleEntryBaseDTO[0];//for comparing for IsModified
        private bool canBeDone;
        private bool canBeCancelled;
        private bool canBePresent;
        private bool isInProgress;
        private CustomerGroupDTO selectedGroup;
        private bool canGroupReserve;
        private bool showMyPlacesSelector;
        private IEnumerable<MyPlaceDTO> myPlaces;


        public ScheduleEntriesDesignViewModel(IBAWindow parentView, ScheduleEntriesCalendar calendar)
        {
            this.calendar = calendar;
            this.parentView = parentView;
            CollectionChangedEventManager.AddListener(ActivitiesReposidory.Instance, this);
            CollectionChangedEventManager.AddListener(CustomerGroupsReposidory.Instance, this);
            CollectionChangedEventManager.AddListener(CustomersReposidory.Instance, this);
        }

        #region Properties

        public bool IsInProgress
        {
            get { return isInProgress; }
            set
            {
                isInProgress = value;
                NotifyOfPropertyChange(() => IsInProgress);
            }
        }

        public bool ReadOnly
        {
            get { return readOnly; }
            set
            {
                readOnly = value;
                
                if (readOnly && IsModified && BAMessageBox.AskYesNo("ScheduleEntriesDesignViewModel_ReadOnly".TranslateInstructor()) == MessageBoxResult.No)
                {
                    readOnly = false;
                    return;
                }
                if (readOnly)
                {
                    FillCalendar(StartDate, EndDate.AddDays(1));
                }
                ShowActivitesSelector = !ReadOnly && ColorMode == ScheduleColorMode.Activities;
                ShowGroupsSelector = !ReadOnly && ColorMode == ScheduleColorMode.CustomerGroups;
                ShowMyPlacesSelector = !ReadOnly && ColorMode == ScheduleColorMode.MyPlaces;
                NotifyOfPropertyChange(()=>ReadOnly);
            }
        }

        
        public bool ShowActivitesSelector
        {
            get { return showActivitesSelector; }
            set
            {
                showActivitesSelector = value;
                NotifyOfPropertyChange(() => ShowActivitesSelector);
            }
        }

        public bool ShowMyPlacesSelector
        {
            get { return showMyPlacesSelector; }
            set
            {
                showMyPlacesSelector = value;
                NotifyOfPropertyChange(() => ShowMyPlacesSelector);
            }
        }
        

        public bool ShowGroupsSelector
        {
            get { return showGroupsSelector; }
            set
            {
                showGroupsSelector = value;
                NotifyOfPropertyChange(() => ShowGroupsSelector);
            }
        }

        public bool CanBeDone
        {
            get { return canBeDone; }
            set
            {
                canBeDone = value;
                NotifyOfPropertyChange(() => CanBeDone);
            }
        }

        public bool CanBePresent
        {
            get { return canBePresent; }
            set
            {
                canBePresent = value;
                NotifyOfPropertyChange(() => CanBePresent);
            }
        }

        public bool CanBeCancelled
        {
            get { return canBeCancelled; }
            set
            {
                canBeCancelled = value;
                NotifyOfPropertyChange(() => CanBeCancelled);
            }
        }

        
        public ScheduleColorMode ColorMode
        {
            get { return _colorMode; }
            set
            {
                if (_colorMode != value)
                {
                    _colorMode = value;
                    //ctrlCalendar.ScheduleColorMode = value;
                    ShowActivitesSelector = !ReadOnly && ColorMode == ScheduleColorMode.Activities;
                    ShowGroupsSelector = !ReadOnly && ColorMode == ScheduleColorMode.CustomerGroups;
                    ShowMyPlacesSelector = !ReadOnly && ColorMode == ScheduleColorMode.MyPlaces;
                    NotifyOfPropertyChange(()=>ColorMode);
                }
            }
        }

        public IList<ListItem<ScheduleColorMode>> ColorModes
        {
            get
            {
                return new ReadOnlyCollectionBuilder<ListItem<ScheduleColorMode>>()
                           {
                               new ImageListItem<ScheduleColorMode>("ScheduleEntriesDesignViewModel_ColorModes_Activities".TranslateInstructor(),@"/BodyArchitect.Client.Module.Instructor;component/Images\Activity16.png", ScheduleColorMode.Activities),
                               new ImageListItem<ScheduleColorMode>("ScheduleEntriesDesignViewModel_ColorModes_Groups".TranslateInstructor(),@"/BodyArchitect.Client.Module.Instructor;component/Images\CustomerGroup16.png",ScheduleColorMode.CustomerGroups),
                               new ImageListItem<ScheduleColorMode>("ScheduleEntriesDesignViewModel_ColorModes_MyPlaces".TranslateInstructor(),@"/BodyArchitect.Client.Resources;component/Images\MyGyms16.png",ScheduleColorMode.MyPlaces)
                           };
            }
        }

        public IEnumerable<CustomerDTO> Customers
        {
            get { return customers; }
            private set
            {
                customers = value;
                updateButtons();
                NotifyOfPropertyChange(() => Customers);
            }
        }
        

        public IEnumerable<CustomerGroupDTO> CustomerGroups
        {
            get { return groups; }
            set
            {
                groups = value;
                NotifyOfPropertyChange(() => CustomerGroups);
            }
        }

        public IEnumerable<MyPlaceDTO> MyPlaces
        {
            get { return myPlaces; }
            set
            {
                myPlaces = value;
                NotifyOfPropertyChange(() => MyPlaces);
            }
        }

        public IEnumerable<ActivityDTO> Activities
        {
            get { return activities; }
            set
            {
                activities = value;
                NotifyOfPropertyChange(()=>Activities);
            }
        }

        public DateTime StartDate
        {
            get { return startDate.Date; }
            set
            {
                startDate = value;
                NotifyOfPropertyChange(()=>StartDate);
            }
        }

        public bool CanCancelReserve
        {
            get { return canCancelReserve; }
            set
            {
                canCancelReserve = value;
                NotifyOfPropertyChange(() => CanCancelReserve);
            }
        }

        public bool CanMarkAsPaid
        {
            get { return canMarkAsPaid; }
            set
            {
                canMarkAsPaid = value;
                NotifyOfPropertyChange(() => CanMarkAsPaid);
            }
        }

        
        public ScheduleEntryReservationViewModel SelectedReservation
        {
            get { return selectedReservation; }
            set
            {
                selectedReservation = value;
                updateButtons();
                NotifyOfPropertyChange(() => SelectedReservation);
            }
        }

        public CustomerGroupDTO SelectedGroup
        {
            get { return selectedGroup; }
            set
            {
                selectedGroup = value;
                updateButtons();
                NotifyOfPropertyChange(() => SelectedGroup);
            }
        }

        public CustomerDTO SelectedCustomer
        {
            get { return selectedCustomer; }
            set
            {
                selectedCustomer = value;
                updateButtons();
                NotifyOfPropertyChange(() => SelectedCustomer);
            }
        }

        public ScheduleEntryBaseViewModel SelectedAppointment
        {
            get { return selectedAppointment; }
            set
            {
                selectedAppointment = value;
                NotifyOfPropertyChange(() => SelectedAppointment);
                NotifyOfPropertyChange(() => IsChampionship);
                NotifyOfPropertyChange(() => CanShowChampionship);
                fillReservations();
                updateButtons();
            }
        }

        public bool IsChampionship
        {
            get { return SelectedEntry is ScheduleChampionshipDTO; }
        }

        public bool CanShowChampionship
        {
            get { return SelectedEntry!=null && SelectedEntry.IsLocked; }
        }

        public bool CanGroupReserve
        {
            get { return canGroupReserve; }
            set
            {
                canGroupReserve = value;
                NotifyOfPropertyChange(() => CanGroupReserve);
            }
        }

        public bool CanReserve
        {
            get { return canReserve; }
            set
            {
                canReserve = value;
                NotifyOfPropertyChange(() => CanReserve);
            }
        }

        public bool CanSave
        {
            get { return canSave; }
            set
            {
                canSave = value;
                NotifyOfPropertyChange(() => CanSave);
            }
        }

        public DateTime EndDate
        {
            get { return endDate.Date; }
            set
            {
                endDate = value;
                NotifyOfPropertyChange(() => EndDate);
            }
        }

        public ObservableCollection<ScheduleEntryBaseViewModel> Entries
        {
            get { return entries; }
        }

        public ObservableCollection<ScheduleEntryReservationViewModel> Reservations
        {
            get { return reservations; }
        }

        public bool IsModified
        {
            get { return isModified; }
            set
            {
                setModifiedFlag();
                NotifyOfPropertyChange(() => IsModified);
            }
        }

        void setModifiedFlag()
        {
            var currentItems = Entries.Select(x => x.Item).ToArray();
            var val = currentItems.IsModified(originalEntries);
            isModified = val;
        }

        public ScheduleEntryBaseDTO SelectedEntry
        {
            get
            {
                if (SelectedAppointment != null)
                {
                    return SelectedAppointment.Item;
                }
                return null;
            }
        }
        #endregion

        void fillReservations()
        {
            reservations.Clear();
            if (SelectedEntry != null)
            {
                foreach (var reservation in SelectedEntry.Reservations)
                {
                    reservations.Add(new ScheduleEntryReservationViewModel(reservation));
                }
            }
        }

        public void RefreshView()
        {
            FillCalendar(StartDate, EndDate);
        }

        public void Fill(DateTime startDate,DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
            fillRelatedObjects();

            FillCalendar(StartDate, EndDate.AddDays(1));
        }

        private void fillRelatedObjects()
        {
            IsInProgress = true;
            parentView.RunAsynchronousOperation(delegate
                {
                                                    var myPlacesCache = MyPlacesReposidory.GetCache(null);
                                                        var activitiesHandle=ActivitiesReposidory.Instance.BeginEnsure();
                                                        var customersHandle = CustomersReposidory.Instance.BeginEnsure();
                                                        var groupsHandle = CustomerGroupsReposidory.Instance.BeginEnsure();
                                                        var myPlacesHandle = myPlacesCache.BeginEnsure();

                                                        WaitHandle.WaitAll(new WaitHandle[] { activitiesHandle, customersHandle, groupsHandle, myPlacesHandle });


                                                        parentView.SynchronizationContext.Send(delegate
                                                                                                   {
                                                                                                       Activities = ActivitiesReposidory.Instance.Items.Values;
                                                                                                       Customers = CustomersReposidory.Instance.Items.Values;
                                                                                                       CustomerGroups = CustomerGroupsReposidory.Instance.Items.Values;
                                                                                                       MyPlaces = myPlacesCache.Items.Values;
                                                                                                       IsInProgress = false;
                                                                                                   },null);
                                                    });
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            parentView.SynchronizationContext.Send(state => fillRelatedObjects(), null);

            return true;
        }

        private void FillCalendar(DateTime startDate, DateTime endDate)
        {
            if (!UserContext.IsInstructor)
            {
                return;
            }
            if (fillCancelSource != null)
            {
                fillCancelSource.Cancel();
                fillCancelSource = null;
            }
            fillCancelSource = parentView.RunAsynchronousOperation(delegate(OperationContext context)
            {
                var param = new GetScheduleEntriesParam();
                param.StartTime = startDate.ToUniversalTime();
                param.EndTime = endDate.ToUniversalTime();
                var pageInfo = new PartialRetrievingInfo();
                pageInfo.PageSize = -1;
                var items = ServiceManager.GetScheduleEntries(param, pageInfo);
                if (context != null && context.CancellatioToken.IsCancellationRequested)
                {
                    return;
                }
                originalEntries = items.Items.StandardClone().ToArray();
                parentView.SynchronizationContext.Send(delegate
                {
                    fillAppointments(items.Items);
                    IsModified = false;
                }, null);
            });
        }


        void updateButtons()
        {
            var championship=SelectedEntry as ScheduleChampionshipDTO;
            bool entryAvailable = SelectedAppointment != null && !SelectedAppointment.IsRunning;
            CanSave = true;
            CanReserve = entryAvailable && SelectedCustomer != null && SelectedEntry.State==ScheduleEntryState.Planned && (championship==null || !SelectedCustomer.IsVirtual);//for championships we cannot reserve for virtual customers
            CanGroupReserve = entryAvailable && SelectedGroup != null;
            CanCancelReserve = entryAvailable && SelectedReservation != null;
            CanMarkAsPaid = entryAvailable && SelectedReservation != null && !SelectedReservation.Reservation.IsPaid;
            //TODO:Block changing status on the server side also
            CanBeCancelled = entryAvailable ;//&& SelectedEntry.State != ScheduleEntryState.Cancelled;//&& SelectedEntry.State == ScheduleEntryState.Planned;
            CanBeDone = entryAvailable && (championship == null || championship.Categories.Count>0);//&& SelectedEntry.State != ScheduleEntryState.Done;
            CanBePresent = entryAvailable && SelectedReservation != null && SelectedEntry.State==ScheduleEntryState.Done;
        }

        
        void fillAppointments(IList<ScheduleEntryBaseDTO> items)
        {
            Entries.Clear();
            SelectedAppointment = null;
            foreach (var item in items)
            {
                ScheduleEntryBaseViewModel app = createAppointment(item);
                Entries.Add(app);
            }
        }

        private ScheduleEntryBaseViewModel createAppointment(ScheduleEntryBaseDTO item)
        {
            if (item is ScheduleEntryDTO)
            {
                return new ScheduleEntryViewModel(item, calendar);    
            }
            return new ScheduleChampionshipViewModel(item, calendar);    
            
        }

        void update(ScheduleEntryBaseDTO entry)
        {
            var res=Entries.Where(x => x.Item.Equals(entry)).SingleOrDefault();
            if (res != null)
            {
                res.Item =  entry;
                if (SelectedAppointment.Item == entry)
                {
                    var reservation = SelectedReservation != null ? SelectedReservation.Reservation : null;
                    SelectedAppointment = res;
                    if (reservation != null)
                    {
                        var newReservation = Reservations.Where(x => x.Reservation == reservation).SingleOrDefault();
                        SelectedReservation = newReservation;
                    }
                }
            }
            updateButtons();
            
        }

        public void Drop(ExDragEventArgs arg)
        {
            var app = (ScheduleEntryBaseViewModel)arg.DragEventArgs.Data.GetData("myFormat");
            var championship = (ChampionshipItem)arg.DragEventArgs.Data.GetData("ChampionshipItem");
            var activity = (ActivityDTO)arg.DragEventArgs.Data.GetData("ActivityDTO");
            var group = (CustomerGroupDTO)arg.DragEventArgs.Data.GetData("CustomerGroupDTO");
            var myPlace = (MyPlaceLightDTO)arg.DragEventArgs.Data.GetData("MyPlaceDTO");
            CalendarTimeslotItem emptySlot = arg.DirectTarget as CalendarTimeslotItem;
            if (emptySlot == null)
            {
                return;
            }
            ScheduleEntryBaseDTO baseDTO;
            if (activity != null)
            {
                var dto = new ScheduleEntryDTO();
                dto.ActivityId = activity.GlobalId;
                dto.MaxPersons = activity.MaxPersons;
                dto.Price = activity.Price;
                dto.StartTime = emptySlot.StartTime.ToUniversalTime();
                dto.EndTime = dto.StartTime + activity.Duration;
                app = createAppointment(dto);
                baseDTO = dto;
            }
            else if(championship!=null)
            {
                var dto = new ScheduleChampionshipDTO();
                dto.StartTime = emptySlot.StartTime.ToUniversalTime();
                dto.ChampionshipType = championship.Value;

                dto.Name = InstructorHelper.Translate(dto.ChampionshipType);
                dto.EndTime = dto.StartTime + TimeSpan.FromHours(4);
                app = createAppointment(dto);
            }
            else if(group!=null)
            {
                var dto = new ScheduleEntryDTO();
                dto.MaxPersons = group.MaxPersons;
                dto.StartTime = emptySlot.StartTime.ToUniversalTime();
                dto.EndTime = dto.StartTime + TimeSpan.FromHours(1);//default entry length is one hour
                if (group.DefaultActivityId.HasValue)
                {
                    activity = ActivitiesReposidory.Instance.GetItem(group.DefaultActivityId.Value);
                    dto.MaxPersons = activity.MaxPersons;
                    dto.ActivityId = activity.GlobalId;
                    dto.Price = activity.Price;
                    dto.EndTime = dto.StartTime + activity.Duration;
                }
                
                dto.CustomerGroupId = group.GlobalId;
                app = createAppointment(dto);
                baseDTO = dto;
            }
            else if (myPlace!=null)
            {
                var dto = new ScheduleEntryDTO();
                dto.MyPlaceId = myPlace.GlobalId;
                dto.StartTime = emptySlot.StartTime.ToUniversalTime();
                dto.EndTime = dto.StartTime + TimeSpan.FromHours(1);//default entry length is one hour
                app = createAppointment(dto);
                baseDTO=dto;
            }

            if ((arg.DragEventArgs.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey)
            {
                app = app.Clone(true);
                baseDTO = app.Item;
                baseDTO.Version = 0;
            }

            
            TimeSpan diff = app.RealEndTime - app.StartTime;
            app.Item.StartTime = emptySlot.StartTime.ToUniversalTime();
            app.Item.EndTime = (app.StartTime + diff).ToUniversalTime();
            if (Entries.IndexOf(app)==-1)
            {
                Entries.Add(app);
            }
            IsModified = true;
        }

        public void ChangeTime(IAppointment app, TimeSpan changeTime)
        {
            var viewModel = (ScheduleEntryBaseViewModel)app;
            var entry = viewModel.Item;
            DateTime predictEndDateTime = entry.EndTime + changeTime;
            if (entry.IsLocked || Entries.Where(x => x.Item != entry && (
                        x.Item.StartTime <= entry.StartTime && x.Item.EndTime > entry.StartTime ||
                        x.Item.StartTime < predictEndDateTime && x.Item.EndTime >= predictEndDateTime))
                        .Count() > 0)
            {
                return;
            }

            if (((entry.EndTime - entry.StartTime) + changeTime) < changeTime.Duration())
            {//can decrease anymore (less than 30 minutes))
                return;
            }
            entry.EndTime = predictEndDateTime;
            NotifyOfPropertyChange(()=>Entries);
            IsModified = true;
        }

        SaveScheduleEntryRangeParam createSaveParam()
        {
            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            param.Entries.AddRange(Entries.Select(x => x.Item));
            param.StartDay = StartDate.ToUniversalTime();
            param.EndDay = EndDate.AddDays(1).ToUniversalTime();
            return param;
        }

        public void Save()
        {
            if (!UIHelper.EnsureInstructorLicence())
            {
                return;
            }
            SaveScheduleEntryRangeParam param = createSaveParam();
            parentView.RunAsynchronousOperation(delegate
            {
                try
                {
                    var items = ServiceManager.SaveScheduleEntriesRange(param).ToList();
                    originalEntries = items.StandardClone().ToArray();
                    //refresh reminders
                    ReminderItemsReposidory.Instance.ClearCache();
                    ChampionshipsReposidory.Instance.Reset();
                    parentView.SynchronizationContext.Send(delegate
                    {
                        fillAppointments(items);
                        IsModified = false;
                    }, null);
                }
                catch (ValidationException ex)
                {
                    parentView.SynchronizationContext.Send(state => BAMessageBox.ShowValidationError(ex.Results), null);
                }
                catch (AlreadyOccupiedException ex)
                {
                    parentView.SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex, "AlreadyOccupiedException_ScheduleEntriesDesignViewModel_Save".TranslateInstructor(), ErrorWindow.MessageBox), null);
                }
                catch (LicenceException ex)
                {
                    parentView.SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex, "ErrInstructorAccountRequired".TranslateInstructor(), ErrorWindow.MessageBox), null);
                }
                catch (DeleteConstraintException ex)
                {
                    parentView.SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex, "DeleteConstraintException_ScheduleEntriesDesignViewModel_Save".TranslateInstructor(), ErrorWindow.MessageBox), null);
                }
                catch (Exception ex)
                {
                    parentView.SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex, "Exception_ScheduleEntriesDesignViewModel_Save".TranslateInstructor(), ErrorWindow.EMailReport), null);
                }

                
            });
        }

        public void SaveAndCopy()
        {
            if(!UIHelper.EnsureInstructorLicence())
            {
                return;
            }
            SaveScheduleEntryRangeParam param = createSaveParam();
            var dlg = new SaveScheduleEntriesWindow(param);
            if (dlg.ShowDialog() == true)
            {
                //refresh reminders
                ReminderItemsReposidory.Instance.ClearCache();
                ChampionshipsReposidory.Instance.Reset();
                BAMessageBox.ShowInfo("ScheduleEntriesDesignViewModel_SaveAndCopy_Finished".TranslateInstructor(), dlg.Result.Count);
            }
        }
        
        public void OpenTrainingDay()
        {
            MainWindow.Instance.ShowTrainingDayReadOnly(SelectedEntry.StartTime.Date, UserContext.Current.CurrentProfile, SelectedReservation.Reservation.CustomerId, new InstructorEntryObjectBuilder(SelectedReservation.Reservation, (ScheduleEntryDTO) SelectedEntry));
        }

        public void EditSelected()
        {
            if(SelectedEntry.IsLocked)
            {
                return;
            }
            var dlg = new EditDomainObjectWindow();
            IEditableControl ctrl = null;
            
            
            var viewModel = SelectedEntry;
            if(!viewModel.IsNew)
            {
                viewModel = viewModel.Clone();    
            }

            if (SelectedEntry is ScheduleEntryDTO)
            {
                var control = new usrScheduleEntryDetails();
                ctrl = control;
                control.Fill(viewModel);
                dlg.SetControl(control);
            }
            else
            {
                var control = new usrChampionshipScheduleEntryEditor();
                ctrl = control;
                control.Fill(viewModel);
                dlg.SetControl(control);
            }

            
            if (dlg.ShowDialog() == true)
            {
                IsModified = true;
                update((ScheduleEntryBaseDTO) ctrl.Object);
            }
        }

        public void DeleteEntry(IAppointment app)
        {
            if (!app.ReadOnly)
            {
                Entries.Remove((ScheduleEntryBaseViewModel)app);
                IsModified = true;
            }
        }

        public bool ShouldCancelChaningDay()
        {
            if(!ReadOnly && IsModified)
            {
                return BAMessageBox.AskYesNo("ScheduleEntriesDesignViewModel_ShouldCancelChaningDay".TranslateInstructor()) == MessageBoxResult.No;
            }
            return false;
        }

        public void SetStatus(ScheduleEntryState scheduleEntryState)
        {
            var list = new BAGlobalObject[] { SelectedEntry };
            reservationOperation(list.ToList(), delegate(BAGlobalObject obj)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.EntryId = obj.GlobalId;
                param.OperationType = scheduleEntryState == ScheduleEntryState.Done ? ReservationsOperationType.StatusDone : ReservationsOperationType.StatusCancelled;
                return param;
            });

        }

        public void MarkAsPaid()
        {
            SelectedAppointment.IsRunning = true;
            parentView.RunAsynchronousOperation(delegate
            {

                try
                {
                    PaymentBasketDTO basket = new PaymentBasketDTO();
                    PaymentDTO payment = new PaymentDTO();
                    payment.Product = SelectedReservation.Reservation;
                    payment.Count = 1;
                    payment.Price = SelectedReservation.Reservation.Price;
                    basket.Payments.Add(payment);
                    basket.TotalPrice = payment.Price;
                    basket = ServiceManager.PaymentBasketOperation(basket);

                    var newReservation = new ScheduleEntryReservationViewModel((ScheduleEntryReservationDTO)basket.Payments[0].Product);
                    parentView.SynchronizationContext.Send(delegate
                    {
                        SelectedAppointment.IsRunning = false;
                        Reservations.Remove(SelectedReservation);
                        Reservations.Add(newReservation);
                        SelectedReservation = newReservation;
                    }, null);
                }
                catch (Exception ex)
                {
                    parentView.SynchronizationContext.Send(
                        delegate
                        {
                            SelectedAppointment.IsRunning = false;
                            ExceptionHandler.Default.Process(ex, "Exception_ScheduleEntriesDesignViewModel_MarkAsPaid".TranslateInstructor(), ErrorWindow.EMailReport);
                        }, null);
                }

            });

        }

        public void SetPresent(IList<ScheduleEntryReservationDTO> reservations,bool isPresent)
        {
            reservationOperation(reservations.Cast<BAGlobalObject>().ToList(), delegate(BAGlobalObject obj)
            {
                var reservation = (ScheduleEntryReservationDTO)obj;
                if (reservation.IsPresent != isPresent)
                {
                    ReservationsOperationParam param = new ReservationsOperationParam();
                    param.ReservationId = obj.GlobalId;
                    param.OperationType = isPresent ? ReservationsOperationType.Presnet : ReservationsOperationType.Absent;
                    return param;
                }
                return null;

           });
        }

        public void Reserve(IEnumerable<CustomerDTO> customers)
        {
            var selectedEntry = SelectedEntry;
            reservationOperation(customers.Cast<BAGlobalObject>().ToList(), delegate(BAGlobalObject obj)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = obj.GlobalId;
                param.EntryId = selectedEntry.GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                return param;
            });
        }


        public void CancelReserve(IList<ScheduleEntryReservationDTO> reservations)
        {
            reservationOperation(reservations.Cast<BAGlobalObject>().ToList(), delegate(BAGlobalObject obj)
                        {
                            ReservationsOperationParam param=new ReservationsOperationParam();
                            param.ReservationId =obj.GlobalId;
                            param.OperationType =ReservationsOperationType.Undo;
                            return param;
                        });
        }


        void reservationOperation(IList<BAGlobalObject> objects,Func<BAGlobalObject,ReservationsOperationParam> createReservationsOperationParam)
        {
            var selectedApp = SelectedAppointment;
            selectedApp.IsRunning = true;
            var cancelToken = parentView.RunAsynchronousOperation(delegate(OperationContext context)
            {
                for (int i = objects.Count - 1; i >= 0; i--)
                {
                    var reservation = objects[i];
                    ScheduleEntryBaseDTO entry = null;
                    try
                    {
                        if (context != null && context.CancellatioToken.IsCancellationRequested)
                        {
                            break;
                        }
                        var param = createReservationsOperationParam(reservation);
                        if(param==null)
                        {
                            continue;
                        }
                        var res = ServiceManager.ReservationsOperation(param);
                        
                        entry = res.ScheduleEntry;
                        if (entry is ScheduleChampionshipDTO)
                        {
                            ChampionshipsReposidory.Instance.Reset();
                        }
                    }
                    catch (ConsistencyException ex)
                    {
                        parentView.SynchronizationContext.Send(
                            delegate
                            {
                                ExceptionHandler.Default.Process(ex, "ScheduleEntriesDesignViewModel_ErrChampionshipWithoutCategories".TranslateInstructor(), ErrorWindow.MessageBox);
                            }, null);

                    }
                    catch (AlreadyOccupiedException ex)
                    {
                        parentView.SynchronizationContext.Send(
                            delegate
                            {
                                //TODO: this message doesn't make sense for cancel
                                ExceptionHandler.Default.Process(ex, "AlreadyOccupiedException_ScheduleEntriesDesignViewModel_reservationOperation".TranslateInstructor(), ErrorWindow.MessageBox);
                            }, null);

                    }
                    catch (Exception ex)
                    {
                        parentView.SynchronizationContext.Send(
                            delegate
                            {
                                ExceptionHandler.Default.Process(ex, "Exception_ScheduleEntriesDesignViewModel_reservationOperation".TranslateInstructor(), ErrorWindow.EMailReport);
                            }, null);
                    }
                    if (entry != null)
                    {
                        parentView.SynchronizationContext.Send(state => update(entry), null);
                    }
                }
                

                parentView.SynchronizationContext.Send(delegate
                {
                    selectedApp.IsRunning = false;
                    calendar.RefreshAppointments();
                    updateButtons();
                }, null);
            });

            selectedApp.CancellationToken = cancelToken;
        }


        public void GroupReserve(IEnumerable<CustomerGroupDTO> groups )
        {
            var selectedEntry = SelectedEntry;
            reservationOperation(groups.Cast<BAGlobalObject>().ToList(), delegate(BAGlobalObject obj)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = obj.GlobalId;
                param.EntryId = selectedEntry.GlobalId;
                param.OperationType = ReservationsOperationType.MakeGroup;
                return param;
            });
        }
    }
}
