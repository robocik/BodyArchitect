using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.Instructor.ViewModel;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Controls.Calendar;
using BodyArchitect.Client.UI.Controls.Calendar.Common;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using Microsoft.Windows.Controls;
using Microsoft.Windows.Controls.Ribbon;

namespace BodyArchitect.Client.Module.Instructor.Controls
{
    [Serializable]
    public class ScheduleEntriesViewContext:PageContext
    {
        public DateTime DateTime { get; set; }

        public ScheduleEntriesViewContext(DateTime dateTime)
        {
            DateTime = dateTime;
        }
    }
    /// <summary>
    /// Interaction logic for ScheduleEntriesView.xaml
    /// </summary>
    public partial class ScheduleEntriesView
    {
        private ScheduleEntriesDesignViewModel viewModel;
        public ScheduleEntriesView()
        {
            InitializeComponent();

            ctrlCalendar.ExDrop += new System.Windows.RoutedEventHandler(ScheduleEntriesCalendar_ExDrop);
            ctrlCalendar.DeleteAppointment += new RoutedEventHandler(ScheduleEntriesCalendar_DeleteAppointment);
            ctrlCalendar.AddAppointment += new RoutedEventHandler(ScheduleEntriesCalendar_AddAppointment);
        }

        public override AccountType AccountType
        {
            get { return AccountType.Instructor; }
        }

        public ScheduleEntriesViewContext Context
        {
            get { return PageContext as ScheduleEntriesViewContext; }
        }

        void ScheduleEntriesCalendar_AddAppointment(object sender, RoutedEventArgs e)
        {
            if (ctrlCalendar.ReadOnly)
            {
                return;
            }
            var item = (CalendarTimeslotItem)e.OriginalSource;
            var entry = new ScheduleEntryDTO();
            entry.StartTime = item.StartTime.ToUniversalTime();
            entry.EndTime = entry.StartTime.AddHours(1);
            var viewModelItem = new ScheduleEntryViewModel(entry, ctrlCalendar);
            viewModel.Entries.Add(viewModelItem);
            viewModel.SelectedAppointment = viewModelItem;
            viewModel.EditSelected();
            //if (ctrlCalendar.DisplayMode == ScheduleDisplayMode.Room)
            //{
            //    entry.RoomId = Identifier;
            //}
            //else
            //{
            //    entry.EmployeeId = Identifier;
            //}

            //EditDomainObjectView dlg = new EditDomainObjectView();
            //var ctrl = new ScheduleEntryEditorView();

            //dlg.SetControl(ctrl);
            //ctrl.Fill(entry, ctrlCalendar.DisplayMode);
            //if (dlg.ShowDialog() == true)
            //{
            //    ServiceManager.Default.SaveScheduleEntry(entry);
            //    var usluga = DomainModelCache.Uslugi[entry.UslugaId];

            //    IList<Appointment> list = (IList<Appointment>)ctrlCalendar.Appointments;
            //    Appointment app = new Appointment();
            //    app.Color = usluga.Color;
            //    app.Subject = usluga.Name;
            //    app.Tag = new ScheduleEntryViewModel(entry, ctrlCalendar);
            //    app.StartTime = entry.StartTime;
            //    app.EndTime = entry.EndTime;
            //    list.Add(app);
            //    ctrlCalendar.RefreshAppointments();
            //}
        }

        void ScheduleEntriesCalendar_DeleteAppointment(object sender, RoutedEventArgs e)
        {
            var app = (IAppointment)e.OriginalSource;
            viewModel.DeleteEntry(app);
        }

        void ScheduleEntriesCalendar_ExDrop(object sender, System.Windows.RoutedEventArgs e)
        {
            viewModel.Drop((ExDragEventArgs)e);
            ctrlCalendar.RefreshAppointments();
        }


        private void ctrlCalendar_ChangeAppointmentTime(object sender, RoutedEventArgs e)
        {
            var param = (ChangeAppointmentTimeEventArgs)e;
            var item = (CalendarAppointmentItem)e.OriginalSource;
            var app = (IAppointment)item.Content;
            viewModel.ChangeTime(app, param.ChangeTime);
            ctrlCalendar.RefreshAppointments();
        }

        private void ctrlCalendar_CurrentDateChanged(object sender, EventArgs e)
        {
            viewModel.Fill(ctrlCalendar.StartDate, ctrlCalendar.EndDate);
        }

        void ScheduleEntriesCalendar_EditAppointment(object sender, RoutedEventArgs e)
        {
            if (ctrlCalendar.ReadOnly)
            {
                return;
            }
            viewModel.EditSelected();
        }

        private void ctrlCalendar_SelectedAppointmentChanged(object sender, EventArgs e)
        {
            //fillReservations();
        }

        public override void Fill()
        {
            Header = "ScheduleEntriesView_Fill_Header_MySchedule".TranslateInstructor();
            viewModel = new ScheduleEntriesDesignViewModel(ParentWindow, ctrlCalendar);
            var binding = new Binding("IsInProgress");
            binding.Mode = BindingMode.OneWay;
            SetBinding(IsInProgressProperty, binding);


            DataContext = viewModel;

            if(Context!=null)
            {
                ctrlCalendar.CurrentDate = Context.DateTime;
            }
            binding = new Binding("IsModified");
            binding.Mode = BindingMode.OneWay;
            SetBinding(IsModifiedProperty, binding);
            viewModel.Fill(ctrlCalendar.StartDate,ctrlCalendar.EndDate);
        }


        public override void RefreshView()
        {
            viewModel.RefreshView();
        }

        public IEnumerable<CustomerGroupDTO> SelectedGroups
        {
            get { return lstGroups.SelectedItems.Cast<CustomerGroupDTO>(); }
        }

        public override Uri HeaderIcon
        {
            get { return new Uri("pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Images/ScheduleEntries.png", UriKind.Absolute); }
        }


        private void rbtnSave_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Save();
            //BUG FIX
            e.Handled = true;
        }

        private void rbtnSaveAndCopy_Click(object sender, RoutedEventArgs e)
        {
            viewModel.SaveAndCopy();
            //BUG FIX
            e.Handled = true;
        }

        private void rbtnReservation_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Reserve(usrCustomers.SelectedCustomers);
        }

        private void rbtnGroupReservation_Click(object sender, RoutedEventArgs e)
        {
            viewModel.GroupReserve(SelectedGroups);
        }

        private void rbtnCancelReservation_Click(object sender, RoutedEventArgs e)
        {
            viewModel.CancelReserve(lvReservations.SelectedItems.Cast<ScheduleEntryReservationViewModel>().Select(x=>x.Reservation).ToList());
        }

        private void rbtnMarkAsPaid_Click(object sender, RoutedEventArgs e)
        {
            viewModel.MarkAsPaid();
            //BUG FIX
            e.Handled = true;
        }

        private void rbtnAddTrainingDay_Click(object sender, RoutedEventArgs e)
        {
            viewModel.OpenTrainingDay();
        }

        private void ctrlCalendar_CurrentDayChanging(object sender, CancelEventArgs e)
        {
            e.Cancel = viewModel.ShouldCancelChaningDay();
        }

        private void rbtnSetStatusDone_Click(object sender, RoutedEventArgs e)
        {
            viewModel.SetStatus(ScheduleEntryState.Done);
            //BUG FIX
            e.Handled = true;
        }

        private void rbtnSetStatusCancelled_Click(object sender, RoutedEventArgs e)
        {
            viewModel.SetStatus(ScheduleEntryState.Cancelled);
            //BUG FIX
            e.Handled = true;
        }

        private void rchkPresent_Checked(object sender, RoutedEventArgs e)
        {
            RibbonToggleButton button = (RibbonToggleButton) sender;
            viewModel.SetPresent(lvReservations.SelectedItems.Cast<ScheduleEntryReservationViewModel>().Select(x=>x.Reservation).ToList(),button.IsChecked.Value);
        }

        private void btnCancelCurrentOperation_Click(object sender, RoutedEventArgs e)
        {
            Button cancelButton = (Button) sender;
            var reservationViewModel = (ScheduleEntryViewModel)cancelButton.Tag;
            if(reservationViewModel.CancellationToken!=null)
            {
                reservationViewModel.CancellationToken.Cancel();
                reservationViewModel.CancellationToken = null;
                cancelButton.Visibility = Visibility.Collapsed;
            }
        }

        private void rbtnShowChampionship_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Controls/ChampionshipView.xaml"), () => new ChampionshipPageContext((ScheduleChampionshipDTO)viewModel.SelectedEntry));
        }
    }
}
