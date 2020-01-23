using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using BodyArchitect.Client.Common;

namespace BodyArchitect.Client.UI.Views
{
    /// <summary>
    /// Interaction logic for MyTrainingsView.xaml
    /// </summary>
    public partial class MyTrainingsView
    {
        private MyTrainingsViewModel viewModel;

        public MyTrainingsView()
        {
            InitializeComponent();

        }

        public override void Fill()
        {
            setHeader();
            viewModel = new MyTrainingsViewModel(ParentWindow,User, Customer);
            DataContext = viewModel;

            var binding = new Binding("IsInProgress");
            binding.Mode = BindingMode.OneWay;
            SetBinding(IsInProgressProperty, binding);

            viewModel.Fill();
        }

        private void setHeader()
        {
            string title = EnumLocalizer.Default.GetStringsString("MyTrainingsView_SetHeader_MyTrainings");
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
            viewModel.Refresh();
        }



        public override Uri HeaderIcon
        {
            get { return "MyTrainings16.png".ToResourceUrl(); }
        }


        private void rbtnBreakMyTraining_Click(object sender, RoutedEventArgs e)
        {
            viewModel.BreakTraining();
        }
    }
}
