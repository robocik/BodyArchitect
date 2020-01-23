using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Windows;
using BodyArchitect.Admin.ServiceReference1;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Payments;

namespace BodyArchitect.Admin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            //InstanceContext context = new InstanceContext(new DbConverterCallback(lstResults,lstResultsDetail,progress));

            cmbCountries.ItemsSource = Country.Countries;
            
        }

        private void btnSendMessageWeb_Click(object sender, RoutedEventArgs e)
        {
            SendMessageMode mode = SendMessageMode.All;
            if (rbSelectedCountries.IsChecked.Value)
            {
                mode = SendMessageMode.SelectedCountries;
            }
            else if (rbExceptSelectedCountries.IsChecked.Value)
            {
                mode = SendMessageMode.ExceptSelectedCountries;
            }
            var start1 = Stopwatch.StartNew();
            var test = cmbCountries.SelectedItems.Cast<Country>().Select(x => x.GeoId).ToList();
            AdminServiceClient client = new AdminServiceClient();
            client.SendMessage(txtTopic.Text, txtContent.Text, mode, test);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DateTime start = DateTime.Now;
            DateTime end;

            bool retry = false;
            do
            {
                try
                {
                    AdminServiceClient client = new AdminServiceClient();
                    var result = client.DeleteOldProfiles(new DeleteOldProfilesParam() { OnlyShowUsers = false });
                    lstDeletedUsers.ItemsSource = result;
                    end = DateTime.Now;
                    retry = false;
                }
                catch (Exception ex)
                {
                    //retry = true;
                }
            } while (retry);
            
            MessageBox.Show("End");

        }

        private void ShowEmptyUsersButton_Click(object sender, RoutedEventArgs e)
        {
            AdminServiceClient client = new AdminServiceClient();
            var result=client.DeleteOldProfiles(new DeleteOldProfilesParam(){OnlyShowUsers = true});
            lstDeletedUsers.ItemsSource = result;
        }

        private void btnDeleteImages_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AdminServiceClient client = new AdminServiceClient();
                var result = client.DeleteUnusedImages(new DeleteOldProfilesParam() {OnlyShowUsers = false});
                lstDeletedUsers.ItemsSource = result;
                MessageBox.Show("Deleted: " + result.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnShowImages_Click(object sender, RoutedEventArgs e)
        {
            AdminServiceClient client = new AdminServiceClient();
            var result = client.DeleteUnusedImages(new DeleteOldProfilesParam() { OnlyShowUsers = true });
            lstDeletedUsers.ItemsSource = result;
        }

        private void btnDeleteOrphanRecords_Click(object sender, RoutedEventArgs e)
        {
            AdminServiceClient client = new AdminServiceClient();
            client.DeleteOrphandExerciseRecords(new DeleteOldProfilesParam() { OnlyShowUsers = false });
            //var result = client.(new DeleteOldProfilesParam() { OnlyShowUsers = true });
        }

       
    }
}
