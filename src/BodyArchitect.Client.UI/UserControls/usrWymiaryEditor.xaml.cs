using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Settings.Model;

namespace BodyArchitect.Client.UI.UserControls
{
    /// <summary>
    /// Interaction logic for usrWymiaryEditor.xaml
    /// </summary>
    public partial class usrWymiaryEditor
    {
        public event EventHandler MeasurementChanged;
        private bool isFilling;

        public usrWymiaryEditor()
        {
            InitializeComponent();
            setMeasurementsType();

            foreach (TimeType item in Enum.GetValues(typeof(TimeType)))
            {
                cmbTimeType.Items.Add(new ListItem<TimeType>(EnumLocalizer.Default.Translate(item),item));
            }
            cmbTimeType.SelectedIndex = 0;
        }

        void onMeasurementChanged()
        {
            if (MeasurementChanged != null && !isFilling)
            {
                MeasurementChanged(this, EventArgs.Empty);
            }
        }
        void setMeasurementsType()
        {
            lblWeightType.Text = UIHelper.WeightType;

            lblPasType.Text = lblKlatkaType.Text = lblHeightType.Text =lblLeftBicepsType.Text =
                 lblLeftForearmType.Text = lblLeftLegType.Text =lblRightBicepsType.Text =
                 lblRightForearmType.Text =lblRightLegType.Text =UIHelper.LengthType;
        }

        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set
            {
                SetValue(ReadOnlyProperty, value);
            }
        }


        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register("ReadOnly", typeof(bool), typeof(usrWymiaryEditor), new UIPropertyMetadata(false, OnReadOnlyChanged));

        private static void OnReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (usrWymiaryEditor)d;

            ctrl.updateReadOnly();
        }


        void updateReadOnly()
        {
            txtWeight.IsEnabled = !ReadOnly;
            txtRightBiceps.IsEnabled =! ReadOnly;
            txtLeftBiceps.IsEnabled = !ReadOnly;
            txtKlata.IsEnabled = !ReadOnly;
            txtPas.IsEnabled =! ReadOnly;
            txtRightUdo.IsEnabled = !ReadOnly;
            txtLeftLeg.IsEnabled = !ReadOnly;
            txtRightForearm.IsEnabled = !ReadOnly;
            txtLeftForearm.IsEnabled = !ReadOnly;
            txtHeight.IsEnabled = !ReadOnly;
            txtTime.IsEnabled = !ReadOnly;
            cmbTimeType.IsReadOnly = ReadOnly;

            tbPas.SetVisible(ReadOnly);
            txtPas.SetVisible(!ReadOnly);
            txtWeight.SetVisible(!ReadOnly);
            tbWeight.SetVisible(ReadOnly);
            txtRightBiceps.SetVisible(!ReadOnly);
            tbRightBiceps.SetVisible(ReadOnly);
            txtLeftBiceps.SetVisible(!ReadOnly);
            tbLeftBiceps.SetVisible(ReadOnly);
            txtKlata.SetVisible(!ReadOnly);
            tbKlata.SetVisible(ReadOnly);
            txtRightUdo.SetVisible(!ReadOnly);
            tbRightUdo.SetVisible(ReadOnly);
            txtLeftLeg.SetVisible(!ReadOnly);
            tbLeftLeg.SetVisible(ReadOnly);
            txtRightForearm.SetVisible(!ReadOnly);
            tbRightForearm.SetVisible(ReadOnly);
            txtLeftForearm.SetVisible(!ReadOnly);
            tbLeftForearm.SetVisible(ReadOnly);
            txtHeight.SetVisible(!ReadOnly);
            tbHeight.SetVisible(ReadOnly);
            txtTime.SetVisible(!ReadOnly);
            tbTime.SetVisible(ReadOnly);
            cmbTimeType.SetVisible(!ReadOnly);
            tbTimeType.SetVisible(ReadOnly);
        }


        public void Fill(WymiaryDTO wymiary)
        {
            isFilling = true;
            txtTime.Value = DateTime.Now;
            if (wymiary != null)
            {
                txtPas.Value = (double?) wymiary.Pas.ToDisplayLength();
                txtWeight.Value = (double?) wymiary.Weight.ToDisplayWeight();
                txtRightBiceps.Value = (double?)wymiary.RightBiceps.ToDisplayLength();
                txtLeftBiceps.Value = (double?)wymiary.LeftBiceps.ToDisplayLength();
                txtKlata.Value = (double?)wymiary.Klatka.ToDisplayLength();
                txtRightUdo.Value = (double?)wymiary.RightUdo.ToDisplayLength();
                txtLeftLeg.Value = (double?)wymiary.LeftUdo.ToDisplayLength();
                txtRightForearm.Value = (double?)wymiary.RightForearm.ToDisplayLength();
                txtLeftForearm.Value = (double?)wymiary.LeftForearm.ToDisplayLength();
                txtHeight.Value = (double?)wymiary.Height.ToDisplayLength();

                tbPas.Text = wymiary.Pas.ToDisplayLength().ToString("#.##");
                tbWeight.Text = wymiary.Weight.ToDisplayWeight().ToString("#.##");
                tbRightBiceps.Text = wymiary.RightBiceps.ToDisplayLength().ToString("#.##");
                tbLeftBiceps.Text = wymiary.LeftBiceps.ToDisplayLength().ToString("#.##");
                tbKlata.Text = wymiary.Klatka.ToDisplayLength().ToString("#.##");
                tbRightUdo.Text = wymiary.RightUdo.ToDisplayLength().ToString("#.##");
                tbLeftLeg.Text = wymiary.LeftUdo.ToDisplayLength().ToString("#.##");
                tbRightForearm.Text = wymiary.RightForearm.ToDisplayLength().ToString("#.##");
                tbLeftForearm.Text = wymiary.LeftForearm.ToDisplayLength().ToString("#.##");
                tbHeight.Text = wymiary.Height.ToDisplayLength().ToString("#.##");

                txtTime.Value = wymiary.Time.DateTime;
                cmbTimeType.SelectedIndex =(int)wymiary.Time.TimeType;
                tbTime.Text = wymiary.Time.DateTime.ToShortTimeString();
                tbTimeType.Text = EnumLocalizer.Default.Translate(wymiary.Time.TimeType);
            }
            updateReadOnly();
            isFilling = false;
        }

        public WymiaryDTO SaveWymiary(WymiaryDTO wymiary)
        {
            if (wymiary == null)
            {
                wymiary = new WymiaryDTO();
            }
            wymiary.Weight = txtWeight.Value.ToSaveWeight();
            wymiary.RightBiceps = txtRightBiceps.Value.ToSaveLength();
            wymiary.LeftBiceps = txtLeftBiceps.Value.ToSaveLength();
            wymiary.Klatka = txtKlata.Value.ToSaveLength();
            wymiary.Pas = txtPas.Value.ToSaveLength();
            wymiary.RightUdo = txtRightUdo.Value.ToSaveLength();
            wymiary.LeftUdo = txtLeftLeg.Value.ToSaveLength();
            wymiary.RightForearm =txtRightForearm.Value.ToSaveLength();
            wymiary.LeftForearm = txtLeftForearm.Value.ToSaveLength();
            wymiary.Height = txtHeight.Value.ToSaveLength();
            wymiary.Time.DateTime = txtTime.Value.Value;
            wymiary.Time.TimeType = (TimeType) cmbTimeType.SelectedIndex;
            return wymiary;
        }

        private void txtPas_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            onMeasurementChanged();
        }

        private void cmbTimeType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            onMeasurementChanged();
        }
    }
}
