using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.Instructor.ViewModel;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model.Instructor;
using Microsoft.Windows.Controls.Ribbon;

namespace BodyArchitect.Client.Module.Instructor.Controls
{
    /// <summary>
    /// Interaction logic for ActivitiesView.xaml
    /// </summary>
    public partial class ActivitiesView
    {
        private ActivitiesViewModel viewModel;

        public ActivitiesView()
        {
            InitializeComponent();
        }


        public override void Fill()
        {
            IsInProgress = true;
            Header = "ActivitiesView_Fill_Header_Activities".TranslateInstructor();
            viewModel = new ActivitiesViewModel(ParentWindow);
            DataContext = viewModel;
            var binding = new Binding("IsInProgress");
            binding.Mode = BindingMode.OneWay;
            SetBinding(IsInProgressProperty, binding);

            viewModel.FillActivities();
            
        }

        public override void RefreshView()
        {
            ActivitiesReposidory.Instance.ClearCache();
            viewModel.FillActivities();
        }


        public override Uri HeaderIcon
        {
            get
            {
                return new Uri("pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Images/Activity16.png", UriKind.Absolute);
            }
        }

        private void rbtnNew_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            viewModel.NewActivity();
        }

        private void rbtnEdit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            viewModel.EditSelectedActivity();
        }

        private void rbtnDelete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            viewModel.DeleteSelectedActivity();
        }

        private void lsItems_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var item = (ActivityDTO)lsItems.GetClickedItem(e);
            if (item != null)
            {
                viewModel.SelectedActivity = item;
                viewModel.EditSelectedActivity();
            }
        }

        private void lsItems_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key==Key.Delete)
            {
                viewModel.DeleteSelectedActivity();
            }
        }

    }
}
