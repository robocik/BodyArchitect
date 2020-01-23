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
using System.Windows.Shapes;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Portable;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.UI.Windows
{
    /// <summary>
    /// Interaction logic for WilksCalculatorWindow.xaml
    /// </summary>
    public partial class WilksCalculatorWindow
    {
        private bool isMale;
        private bool isFemale;
        private double bodyWeight;
        private double totalWeight;
        private string wilks;
        private string weightType;

        public WilksCalculatorWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string WeightType
        {
            get { return weightType; }
            set
            {
                weightType = value;
                NotifyOfPropertyChange(() => WeightType);
            }
        }

        public double BodyWeight
        {
            get { return bodyWeight; }
            set
            {
                bodyWeight = value;
                NotifyOfPropertyChange(() => BodyWeight);
            }
        }

        public double TotalWeight
        {
            get { return totalWeight; }
            set
            {
                totalWeight = value;
                NotifyOfPropertyChange(() => TotalWeight);
            }
        }

        public bool IsMale
        {
            get { return isMale; }
            set
            {
                isMale = value; 
                NotifyOfPropertyChange(()=>IsMale);
            }
        }

        public bool IsFemale
        {
            get { return isFemale; }
            set
            {
                isFemale = value;
                NotifyOfPropertyChange(() => IsFemale);
            }
        }

        public string Wilks
        {
            get { return wilks; }
            set
            {
                wilks = value;
                NotifyOfPropertyChange(() => Wilks);
            }
        }

        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            var bodyKgWeight = ((double?)BodyWeight).ToSaveWeight();
            var totalKgWeight = ((double?)TotalWeight).ToSaveWeight();
            if (IsMale)
            {

                var result = WilksFormula.CalculateForMenUsingTables(bodyKgWeight, totalKgWeight);
                Wilks = result.ToString("#.####");
            }
            else
            {
                var result = WilksFormula.CalculateForWomenUsingTables(bodyKgWeight, totalKgWeight);
                Wilks = result.ToString("#.####");
            }
        }

        private void WilksCalculatorWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            IsMale = UserContext.Current.CurrentProfile.Gender == Gender.Male;
            IsFemale = !IsMale;
            if (UserContext.Current.ProfileInformation.Wymiary != null && UserContext.Current.ProfileInformation.Wymiary.Weight>0)
            {
                BodyWeight = (double) UserContext.Current.ProfileInformation.Wymiary.Weight;
            }
            if (UserContext.Current.ProfileInformation.Settings.WeightType == Service.V2.Model.WeightType.Pounds)
            {
                WeightType= Strings.WeightType_Pound;
            }
            else
            {
                WeightType = Strings.WeightType_Kg;
            }
        }

    }
}
