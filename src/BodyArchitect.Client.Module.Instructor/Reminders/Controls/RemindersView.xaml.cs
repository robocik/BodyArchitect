using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BodyArchitect.Client.Module.Instructor.ViewModel;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.SchedulerEngine;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor.Controls
{
    /// <summary>
    /// Interaction logic for RemindersView.xaml
    /// </summary>
    public partial class RemindersView
    {
        private RemindersViewModel viewModel;

        public RemindersView()
        {
            InitializeComponent();
        }

        #region Implementation of IControlView

        public override void Fill()
        {
            Header = "RemindersView_Fill_Header_Reminders".TranslateInstructor();
            viewModel = new RemindersViewModel(this.ParentWindow);
            var binding = new Binding("IsInProgress");
            binding.Mode = BindingMode.OneWay;
            SetBinding(IsInProgressProperty, binding);

            DataContext = viewModel;
            viewModel.FillReminders();
        }

        public override void RefreshView()
        {
            ReminderItemsReposidory.Instance.ClearCache();
            viewModel.FillReminders();
        }


        public override Uri HeaderIcon
        {
            get
            {
                return new Uri("pack://application:,,,/BodyArchitect.Client.Resources;component/Images/Reminder16.png", UriKind.Absolute);
            }
        }

        #endregion

        private void rbtnNew_Click(object sender, RoutedEventArgs e)
        {
            viewModel.New();
        }

        private void rbtnEdit_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Edit();
        }

        private void rbtnDelete_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Delete();
        }

        private void lvReminders_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(viewModel.SelectedReminder!=null)
            {
                viewModel.Edit();
            }
        }

        private void lvReminders_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                viewModel.Delete();
            }
        }
    }
}
