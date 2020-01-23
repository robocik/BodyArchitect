using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.UI.UserControls
{
    /// <summary>
    /// Interaction logic for usrProfilePersonalInfo.xaml
    /// </summary>
    public partial class usrProfilePersonalInfo
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
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action<string>(setControls), aboutInfo);
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
