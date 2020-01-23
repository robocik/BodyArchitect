using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Common;
using BodyArchitect.Controls.Cache;
using BodyArchitect.Service.Model;
using DevExpress.XtraEditors;

namespace BodyArchitect.Controls.UserControls
{
    public partial class usrProfilePersonalInfo : DevExpress.XtraEditors.XtraUserControl
    {
        public usrProfilePersonalInfo()
        {
            InitializeComponent();
        }

        public void Fill(ProfileDTO profile)
        {

            baPictureEdit1.Fill(profile);

            setControls(profile.AboutInformation);
        }

        void setControls(string aboutInfo)
        {
            if(InvokeRequired)
            {
                BeginInvoke(new Action<string>(setControls), aboutInfo);
            }
            else
            {
                txtAbout.Text = aboutInfo;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ForceUploadImage
        {
            get { return baPictureEdit1.ErrorOccured; }
        }

        public PictureDTO Save(ProfileDTO profile)
        {
            PictureDTO pic = baPictureEdit1.Save(profile);
            profile.AboutInformation = txtAbout.Text;

            return pic;
        }

    }
}
