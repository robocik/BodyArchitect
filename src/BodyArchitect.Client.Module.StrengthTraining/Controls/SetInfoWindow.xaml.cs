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
using BodyArchitect.Client.Module.StrengthTraining.Localization;
using BodyArchitect.Client.Module.StrengthTraining.Model;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls
{
    /// <summary>
    /// Interaction logic for SetInfoWindow.xaml
    /// </summary>
    public partial class SetInfoWindow
    {
        private SerieDTO serie;
        private bool readOnly;

        public SetInfoWindow()
        {
            InitializeComponent();

            foreach (SetType value in Enum.GetValues(typeof(SetType)))
            {
                var item = new ListItem<SetType>(EnumLocalizer.Default.Translate((SetType)value),value);
                cmbSetType.Items.Add(item);
            }
        }

        public SetInfoWindow(SerieDTO serie, bool readOnly)
            : this()
        {
            this.serie = serie;
            Fill(serie);
            this.readOnly = readOnly;
            updateReadOnly();
        }

        void updateReadOnly()
        {
            chkRestPause.IsEnabled = !readOnly && UserContext.IsPremium;
            chkSuperSlow.IsEnabled = !readOnly && UserContext.IsPremium;
            txtComment.IsReadOnly = readOnly;
            cmbSetType.IsEnabled =! readOnly;
            chkCiezarBezSztangi.IsEnabled = !readOnly;
            cmbDropSet.IsEnabled =! readOnly;
        }


        public void Fill(SerieDTO serie)
        {
            txtComment.Text = serie.Comment;
            cmbSetType.SelectedIndex = (int)serie.SetType;
            chkCiezarBezSztangi.IsChecked = serie.IsCiezarBezSztangi;
            cmbDropSet.SelectedIndex = (int)serie.DropSet;
            if (serie.RepetitionNumber.HasValue && serie.IsCardio())
            {
                txtDistance.Value = serie.RepetitionNumber.Value.ToDisplayDistance();
            }
            chkRestPause.IsChecked = serie.IsRestPause ;
            chkSuperSlow.IsChecked = serie.IsSuperSlow;
            cardioPane.SetVisible(serie.IsCardio());
            panel1.SetVisible(!serie.IsCardio());
        }

        public void UpdateSerie(SerieDTO serie)
        {
            serie.Comment = txtComment.Text;
            serie.SetType = (SetType)cmbSetType.SelectedIndex;
            serie.IsCiezarBezSztangi = chkCiezarBezSztangi.IsChecked.Value;
            serie.DropSet = (DropSetType)cmbDropSet.SelectedIndex;
            serie.IsRestPause = chkRestPause.IsChecked.Value;
            serie.IsSuperSlow = chkSuperSlow.IsChecked.Value;
            if (serie.IsCardio())
            {
                serie.RepetitionNumber = txtDistance.Value.Value.FromDisplayDistance();
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (!readOnly)
            {
                UpdateSerie(serie);
                DialogResult = true;
                Close();
            }
        }
    }
}
