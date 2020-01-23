using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI;
using BodyArchitect.Service.V2.Model;
using System.Collections.ObjectModel;

namespace BodyArchitect.Client.Module.Suplements.Controls
{
    /// <summary>
    /// Interaction logic for usrSupplementsCycleDosageEditor.xaml
    /// </summary>
    public partial class usrSupplementsCycleDosageEditor
    {
        private ObservableCollection<ListItem<DosageType>> dosageTypes = new ObservableCollection<ListItem<DosageType>>();
        private ObservableCollection<ListItem<DosageUnit>> dosageUnits = new ObservableCollection<ListItem<DosageUnit>>();
        private ObservableCollection<ListItem<SupplementCycleDayRepetitions>> repetitions = new ObservableCollection<ListItem<SupplementCycleDayRepetitions>>();
        private ObservableCollection<ListItem<TimeType>> timeTypes = new ObservableCollection<ListItem<TimeType>>();

        public usrSupplementsCycleDosageEditor()
        {
            InitializeComponent();

            foreach (DosageType dosageType in Enum.GetValues(typeof(DosageType)))
            {
                DosageTypes.Add(new ListItem<DosageType>(EnumLocalizer.Default.Translate(dosageType), dosageType));
            }
            foreach (DosageUnit dosageUnit in Enum.GetValues(typeof(DosageUnit)))
            {
                DosageUnits.Add(new ListItem<DosageUnit>(EnumLocalizer.Default.Translate(dosageUnit), dosageUnit));
            }
            foreach (SupplementCycleDayRepetitions repetition in Enum.GetValues(typeof(SupplementCycleDayRepetitions)))
            {
                Repetitions.Add(new ListItem<SupplementCycleDayRepetitions>(EnumLocalizer.Default.Translate(repetition), repetition));
            }
            foreach (TimeType timeType in Enum.GetValues(typeof(TimeType)))
            {
                TimeTypes.Add(new ListItem<TimeType>(EnumLocalizer.Default.Translate(timeType), timeType));
            }
        }

        public SupplementsCycleDosageViewModel SelectedDosage
        {
            get { return (SupplementsCycleDosageViewModel)GetValue(SelectedDosageProperty); }
            set
            {
                SetValue(SelectedDosageProperty, value);
            }
        }

        public ObservableCollection<ListItem<DosageType>> DosageTypes
        {
            get { return dosageTypes; }
        }

        public ObservableCollection<ListItem<DosageUnit>> DosageUnits
        {
            get { return dosageUnits; }
        }

        public ObservableCollection<ListItem<SupplementCycleDayRepetitions>> Repetitions
        {
            get { return repetitions; }
        }

        public ObservableCollection<ListItem<TimeType>> TimeTypes
        {
            get { return timeTypes; }
        }

        public static readonly DependencyProperty SelectedDosageProperty =
            DependencyProperty.Register("SelectedDosage", typeof(SupplementsCycleDosageViewModel), typeof(usrSupplementsCycleDosageEditor), new UIPropertyMetadata(null));



        public void Clear()
        {
            SetCurrentValue(SelectedDosageProperty,null);
            cmbSupplements.SelectedIndex = -1;
            cmbSupplements.ClearFilter();
        }
    }
}
