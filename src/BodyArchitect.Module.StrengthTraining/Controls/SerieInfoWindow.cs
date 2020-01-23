using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Service.Model;


namespace BodyArchitect.Module.StrengthTraining.Controls
{
    public partial class SerieInfoWindow : DevExpress.XtraEditors.XtraForm
    {
        private SerieDTO serie;
        private bool readOnly;

        public SerieInfoWindow()
        {
            InitializeComponent();
        }

        public SerieInfoWindow(SerieDTO serie,bool readOnly)
            : this()
        {
            this.serie = serie;
            this.usrSerieInfo1.Fill(serie);
            usrSerieInfo1.ReadOnly = readOnly;
            this.readOnly = readOnly;
        }

        private void okButton1_Click(object sender, EventArgs e)
        {
            if (!readOnly)
            {
                usrSerieInfo1.UpdateSerie(serie);
            }
        }
    }
}