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
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.UI.UserControls
{
    /// <summary>
    /// Interaction logic for usrProfileListEntry.xaml
    /// </summary>
    public partial class usrProfileListEntry
    {
        private UserDTO user;
        private bool allowRedirect;

        public usrProfileListEntry()
        {
            InitializeComponent();
            AllowRedirectToDetails = true;
        }

        public void Fill(UserDTO user)
        {
            this.user = user;
            if (user != null)
            {
                btnShowProfile.Content = user.UserName;
                lblGender.Text = EnumLocalizer.Default.Translate(user.Gender);
                lblCreatedDateValue.Text = user.CreationDate.ToRelativeDate();
                lblCountry.Text = Country.GetCountry(user.CountryId).DisplayName;
            }
            if (user != null && user.IsDeleted)
            {
                profileImage.Source= "DeletedProfile.png".ToResourceUrl().ToBitmap();
            }
            else
            {
                profileImage.Fill(user);
            }

        }



        [DefaultValue(true)]
        public bool AllowRedirectToDetails
        {
            get { return allowRedirect; }
            set
            {
                if (allowRedirect != value)
                {
                    btnShowProfile.Cursor = value && user != null && !user.IsDeleted ? Cursors.Hand : Cursors.Arrow;
                    btnShowProfile.IsHitTestVisible = value;
                    allowRedirect = value;
                }
            }
        }

        //private void pictureBox1_Click(object sender, EventArgs e)
        //{
        //    this.InvokeOnClick(this, EventArgs.Empty);
        //}

        private void lblUserName_Click(object sender, EventArgs e)
        {
            if (AllowRedirectToDetails && user != null && !user.IsDeleted)
            {
                MainWindow.Instance.ShowUserInformation(user);
            }
            //else
            //{
            //    InvokeOnClick(this, EventArgs.Empty);
            //}
        }
    }
}
