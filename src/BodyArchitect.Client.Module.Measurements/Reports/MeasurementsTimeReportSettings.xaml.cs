using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Data;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Service.V2.Model.Reports;
using BodyArchitect.Settings.Model;
using Visiblox.Charts;

namespace BodyArchitect.Client.Module.Measurements.Reports
{
    public partial class MeasurementsTimeReportSettings
    {
        private ObservableCollection<CheckListItem<string>> items = new ObservableCollection<CheckListItem<string>>();
        
        public MeasurementsTimeReportSettings()
        {
            InitializeComponent();
            lstSizes.ItemsSource = Items;
        }

        public void Initialize(UserDTO user, CustomerDTO customer)
        {
            
            Items.Clear();

            Items.Add(new CheckListItem<string>(Strings.Wymiary_Chest, "Klatka"));
            Items.Add(new CheckListItem<string>(Strings.Wymiary_Height, "Height"));
            Items.Add(new CheckListItem<string>(Strings.Wymiary_LeftBiceps, "LeftBiceps"));
            Items.Add(new CheckListItem<string>(Strings.Wymiary_LeftForearm, "LeftForearm"));
            Items.Add(new CheckListItem<string>(Strings.Wymiary_LeftUdo, "LeftUdo"));
            Items.Add(new CheckListItem<string>(Strings.Wymiary_Pas, "Pas"));
            Items.Add(new CheckListItem<string>(Strings.Wymiary_RightBiceps, "RightBiceps"));
            Items.Add(new CheckListItem<string>(Strings.Wymiary_RightForearm, "RightForearm"));
            Items.Add(new CheckListItem<string>(Strings.Wymiary_RightUdo, "RightUdo"));
            Items.Add(new CheckListItem<string>(Strings.Wymiary_Weight, "Weight"));

            CollectionView myView = (CollectionView)CollectionViewSource.GetDefaultView(lstSizes.ItemsSource);
            myView.SortDescriptions.Add(new SortDescription("Text", ListSortDirection.Ascending));
        }

        public string GetTranslatedWymiar(string propertyName)
        {
            string result = propertyName;
            UIHelper.Invoke(()=>
                {
                    foreach (var listViewItem in Items)
                    {
                        if (listViewItem.Value == propertyName)
                        {
                            result= listViewItem.Text;
                        }
                    }
                },Dispatcher);

            return result;
        }

        public bool CanGenerateReport
        {
            get { return Items.Where(x => x.IsChecked).Count() > 0; }
        }

        public ObservableCollection<CheckListItem<string>> Items
        {
            get { return items; }
        }

        public WorkoutDaysSearchCriteria GetWorkoutDaysCriteria()
        {
            var criteria = new WorkoutDaysSearchCriteria();
            criteria.StartDate = this.usrDateRange1.DateFrom;
            criteria.EndDate = this.usrDateRange1.DateTo;
            return criteria;
        }

        public event EventHandler CanGenerateReportChanged;

        protected virtual void OnCanGenerateReportChanged()
        {
            if (CanGenerateReportChanged != null)
            {
                CanGenerateReportChanged(this, EventArgs.Empty);
            }
        }

        public  List<PropertyInfo> GetSelectedProperties()
        {
            List<PropertyInfo> list = new List<PropertyInfo>();
            UIHelper.Invoke(()=>
                {
                    foreach (var item in Items)
                    {
                        if (item.IsChecked)
                        {
                            list.Add(typeof(WymiaryDTO).GetProperty(item.Value));
                        }
                    }
                },Dispatcher);
            
            return list;
        }

        public ReportMeasurementsTimeParams GetReportParameters()
        {
            
            var param = new ReportMeasurementsTimeParams();
            UIHelper.Invoke(()=>
                {
                    param.StartDate = this.usrDateRange1.DateFrom;
                    param.EndDate = this.usrDateRange1.DateTo;
                    param.UseAllEntries = usrReportingEntryStatus1.UseAllEntries;
                },Dispatcher);
            
            return param;
        }
        
        

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            OnCanGenerateReportChanged();
        }
    }
}
