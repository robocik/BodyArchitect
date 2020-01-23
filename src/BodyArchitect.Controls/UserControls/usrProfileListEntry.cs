using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Controls.Cache;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Service.Model;

namespace BodyArchitect.Controls.UserControls
{
    public partial class usrProfileListEntry : usrPictureBaseControl
    {
        private UserDTO user;
        private bool allowRedirect;

        public usrProfileListEntry():base()
        {
            InitializeComponent();
            AllowRedirectToDetails = true;
        }

        public void Fill(UserDTO user)
        {
            this.user = user;
            if (user != null)
            {
                lblUserName.Text = user.UserName;
                lblGender.Text = EnumLocalizer.Default.Translate(user.Gender);
                lblCreatedDateValue.Text = user.CreationDate.ToRelativeDate();
                lblCountry.Text = Country.GetCountry(user.CountryId).DisplayName;
            }
            if (user!=null && user.IsDeleted)
            {
                pictureBox1.Image = Icons.DeletedProfile;
            }
            else
            {
                base.Fill(pictureBox1, user);    
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
                    lblUserName.Cursor = value && user!=null && !user.IsDeleted  ? Cursors.Hand : Cursors.Default;
                    allowRedirect = value;
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.InvokeOnClick(this, EventArgs.Empty);
        }

        private void lblUserName_Click(object sender, EventArgs e)
        {
            if (AllowRedirectToDetails && user!=null && !user.IsDeleted)
            {
                MainWindow.Instance.ShowUserInformation(user);
            }
            else
            {
                InvokeOnClick(this, EventArgs.Empty);
            }
        }

    }
}
