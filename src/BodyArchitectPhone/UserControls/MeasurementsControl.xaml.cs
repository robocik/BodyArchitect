using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;

namespace BodyArchitect.WP7.UserControls
{
    public partial class MeasurementsControl : UserControl,INotifyPropertyChanged
    {

        public MeasurementsControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public static readonly DependencyProperty ReadOnlyProperty =
                DependencyProperty.Register("ReadOnly",
                typeof(bool), typeof(MeasurementsControl),
                new PropertyMetadata(false, OnReadOnlyChanged));

        public bool ReadOnly
        {
            get
            {
                return (bool)GetValue(ReadOnlyProperty);
            }
            set { SetValue(ReadOnlyProperty, value); }
        }

        static void OnReadOnlyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var img = (MeasurementsControl)obj;
            //img.grControl.DataContext = args.NewValue;
            var readOnly = (bool)args.NewValue;
            img.IsHitTestVisible = !readOnly;

            if (img.Wymiary != null)
            {
                img.lblEmpty.Visibility = img.Wymiary.IsEmpty
                                              ? System.Windows.Visibility.Visible
                                              : System.Windows.Visibility.Collapsed;
                img.grControl.Visibility = img.Wymiary.IsEmpty
                                               ? System.Windows.Visibility.Collapsed
                                               : System.Windows.Visibility.Visible;
            }
        }
 
        public static readonly DependencyProperty WymiaryProperty =
                DependencyProperty.Register("Wymiary",
                typeof(WymiaryDTO), typeof(MeasurementsControl),
                new PropertyMetadata(null, OnSourceChanged));

        public WymiaryDTO Wymiary
        {
            get
            {
                return (WymiaryDTO)GetValue(WymiaryProperty);
            }
            set { SetValue(WymiaryProperty, value); }
        }

        public void Fill(WymiaryDTO currentSizes,TrainingDaysHolder holder)
        {
            Wymiary = currentSizes;
            updateGui(this, currentSizes);
            if (currentSizes!=null && holder != null /*&& !ApplicationState.IsFree*/)
            {
                ThreadPool.QueueUserWorkItem(delegate
                                                 {
                                                     updateChanges(currentSizes, holder);
                                                 });
                
            }
        }

        private void updateChanges(WymiaryDTO currentSizes,TrainingDaysHolder holder)
        {
            var newWay = holder.TrainingDays.SelectMany(d => d.Value.TrainingDay.Objects.OfType<SizeEntryDTO>().Where(h => h.TrainingDay.TrainingDate < currentSizes.Time.DateTime.Date));
            var res = (from i in newWay where i.Wymiary.Weight > 0 orderby i.TrainingDay.TrainingDate descending select i).FirstOrDefault();
            if (res != null)
            {
                PreviousWeight = res.Wymiary.Weight;
            }

            res = (from i in newWay where i.Wymiary.Height > 0 orderby i.TrainingDay.TrainingDate descending select i).FirstOrDefault();
            if (res != null)
            {
                PreviousHeight = res.Wymiary.Height;
            }

            res = (from i in newWay where i.Wymiary.Pas > 0 orderby i.TrainingDay.TrainingDate descending select i).FirstOrDefault();
            if (res != null)
            {
                PreviousPas = res.Wymiary.Pas;
            }

            res = (from i in newWay where i.Wymiary.Klatka > 0 orderby i.TrainingDay.TrainingDate descending select i).FirstOrDefault();
            if (res != null)
            {
                PreviousKlatka = res.Wymiary.Klatka;
            }

            res = (from i in newWay where i.Wymiary.LeftBiceps > 0 orderby i.TrainingDay.TrainingDate descending select i).FirstOrDefault();
            if (res != null)
            {
                PreviousLeftBiceps = res.Wymiary.LeftBiceps;
            }

            res = (from i in newWay where i.Wymiary.RightBiceps > 0 orderby i.TrainingDay.TrainingDate descending select i).FirstOrDefault();
            if (res != null)
            {
                PreviousRightBiceps = res.Wymiary.RightBiceps;
            }

            res = (from i in newWay where i.Wymiary.RightForearm > 0 orderby i.TrainingDay.TrainingDate descending select i).FirstOrDefault();
            if (res != null)
            {
                PreviousRightForearm = res.Wymiary.RightForearm;
            }

            res = (from i in newWay where i.Wymiary.LeftForearm > 0 orderby i.TrainingDay.TrainingDate descending select i).FirstOrDefault();
            if (res != null)
            {
                PreviousLeftForearm = res.Wymiary.LeftForearm;
            }

            res = (from i in newWay where i.Wymiary.RightUdo > 0 orderby i.TrainingDay.TrainingDate descending select i).FirstOrDefault();
            if (res != null)
            {
                PreviousRightUdo = res.Wymiary.RightUdo;
            }

            res = (from i in newWay where i.Wymiary.LeftUdo > 0 orderby i.TrainingDay.TrainingDate descending select i).FirstOrDefault();
            if (res != null)
            {
                PreviousLeftUdo = res.Wymiary.LeftUdo;
            }

            Dispatcher.BeginInvoke(delegate
            {
                updateChanges();
            });

            
        }

        static void OnSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var img = (MeasurementsControl)obj;
            //img.grControl.DataContext = args.NewValue;
            var wymiary=(WymiaryDTO) args.NewValue;
            updateGui(img, wymiary);
        }

        private static void updateGui(MeasurementsControl img, WymiaryDTO wymiary)
        {
            var isEmpty = img.ReadOnly && (wymiary == null || wymiary == WymiaryDTO.Empty || wymiary.IsEmpty);
            img.lblEmpty.Visibility = isEmpty ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            img.grControl.Visibility = isEmpty ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            img.lblEmpty.Text = ApplicationStrings.MeasurementsControl_Empty;
        }

        private decimal _prevWeight;
        public decimal PreviousWeight
        {
            get { return _prevWeight; }
            set
            {
                _prevWeight = value;
            }
        }

        private decimal _prevHeight;
        public decimal PreviousHeight
        {
            get { return _prevHeight; }
            set { _prevHeight = value; }
        }

        private decimal _prevKlatka;
        public decimal PreviousKlatka
        {
            get { return _prevKlatka; }
            set { _prevKlatka = value; }
        }

        private decimal prevPas;
        public decimal PreviousPas
        {
            get { return prevPas; }
            set { prevPas = value; }
        }

        private decimal prevRightBiceps;
        public decimal PreviousRightBiceps
        {
            get { return prevRightBiceps; }
            set { prevRightBiceps = value; }
        }

        private decimal prevLeftBiceps;
        public decimal PreviousLeftBiceps
        {
            get { return prevLeftBiceps; }
            set { prevLeftBiceps = value; }
        }

        private decimal prevRightForearm;
        public decimal PreviousRightForearm
        {
            get { return prevRightForearm; }
            set { prevRightForearm = value; }
        }

        private decimal prevLeftForearm;
        public decimal PreviousLeftForearm
        {
            get { return prevLeftForearm; }
            set { prevLeftForearm = value; }
        }

        private decimal prevLeftUdo;
        public decimal PreviousLeftUdo
        {
            get { return prevLeftUdo; }
            set { prevLeftUdo = value; }
        }

        private decimal prevRightUdo;
        public decimal PreviousRightUdo
        {
            get { return prevRightUdo; }
            set { prevRightUdo = value; }
        }

        public decimal WeightChange
        {
            get
            {
                if (Wymiary != null && Wymiary.Weight != 0 && PreviousWeight != 0)
                {
                    return ((Wymiary.Weight - PreviousWeight)).ToDisplayWeight();
                }
                return decimal.Zero;
            }
        }


        public decimal HeightChange
        {
            get
            {
                if (Wymiary != null && Wymiary.Height != 0 && PreviousHeight != 0)
                {
                    return ((Wymiary.Height - PreviousHeight)).ToDisplayLength();
                }
                return decimal.Zero;
            }
        }

        public decimal KlatkaChange
        {
            get 
            {
                if (Wymiary != null && Wymiary.Klatka != 0 && PreviousKlatka != 0)
                {
                    return ((Wymiary.Klatka - PreviousKlatka)).ToDisplayLength();
                }
                return decimal.Zero;
            }
        }


        public decimal PasChange
        {
            get
            {
                if (Wymiary != null && Wymiary.Pas != 0 && PreviousPas != 0)
                {
                    return ((Wymiary.Pas - PreviousPas)).ToDisplayLength();
                }
                return decimal.Zero;
            }
        }

        public decimal RightBicepsChange
        {
            get
            {
                if (Wymiary != null && Wymiary.RightBiceps != 0 && PreviousRightBiceps != 0)
                {
                    return ((Wymiary.RightBiceps - PreviousRightBiceps)).ToDisplayLength();
                }
                return decimal.Zero;
            }
        }

        public decimal LeftBicepsChange
        {
            get
            {
                if (Wymiary != null && Wymiary.LeftBiceps != 0 && PreviousLeftBiceps != 0)
                {
                    return ((Wymiary.LeftBiceps - PreviousLeftBiceps)).ToDisplayLength();
                }
                return decimal.Zero;
            }
        }

        public decimal RightForearmChange
        {
            get
            {
                if (Wymiary != null && Wymiary.RightForearm != 0 && PreviousRightForearm != 0)
                {
                    return ((Wymiary.RightForearm - PreviousRightForearm)).ToDisplayLength();
                }
                return decimal.Zero;
            }
        }

        public decimal LeftForearmChange
        {
            get
            {
                if (Wymiary != null && Wymiary.LeftForearm != 0 && PreviousLeftForearm != 0)
                {
                    return ((Wymiary.LeftForearm - PreviousLeftForearm)).ToDisplayLength();
                }
                return decimal.Zero;
            }
        }

        public decimal LeftUdoChange
        {
            get
            {
                if (Wymiary != null && Wymiary.LeftUdo != 0 && PreviousLeftUdo != 0)
                {
                    return ((Wymiary.LeftUdo - PreviousLeftUdo)).ToDisplayLength();
                }
                return decimal.Zero;
            }
        }

        public decimal RightUdoChange
        {
            get
            {
                if (Wymiary != null && Wymiary.RightUdo != 0 && PreviousRightUdo != 0)
                {
                    return ((Wymiary.RightUdo - PreviousRightUdo)).ToDisplayLength();
                }
                return decimal.Zero;
            }
        }

        public string WeightType
        {
            get
            {
                if (ApplicationState.Current.ProfileInfo.Settings.WeightType == Service.V2.Model.WeightType.Kg)
                {
                    return ApplicationStrings.Kg;
                }
                else
                {
                    return ApplicationStrings.Pound;
                }
            }
        }

        public string LengthType
        {
            get
            {
                if (ApplicationState.Current.ProfileInfo.Settings.LengthType == Service.V2.Model.LengthType.Cm)
                {
                    return ApplicationStrings.Cm;
                }
                else
                {
                    return ApplicationStrings.Inch;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void onPropertyChanged(string name)
        {
            if(PropertyChanged!=null)
            {
                PropertyChanged(this,new PropertyChangedEventArgs(name));
            }
        }

        private void txtWeight_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if(string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = "0";
            }
            // Update the binding source
            BindingExpression bindingExpr = textBox.GetBindingExpression(TextBox.TextProperty);
            bindingExpr.UpdateSource();

            updateChanges();
        }

        private void updateChanges()
        {
            onPropertyChanged("WeightChange");
            onPropertyChanged("HeightChange");
            onPropertyChanged("KlatkaChange");
            onPropertyChanged("PasChange");
            onPropertyChanged("RightBicepsChange");
            onPropertyChanged("LeftBicepsChange");
            onPropertyChanged("RightForearmChange");
            onPropertyChanged("LeftForearmChange");
            onPropertyChanged("LeftUdoChange");
            onPropertyChanged("RightUdoChange");
        }
    }
}
