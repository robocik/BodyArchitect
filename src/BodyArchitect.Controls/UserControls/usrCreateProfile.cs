using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Common;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Service.Model;
using BodyArchitect.Shared;
using DevExpress.XtraEditors.DXErrorProvider;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Gender = BodyArchitect.Service.Model.Gender;

namespace BodyArchitect.Controls.UserControls
{
    public partial class usrCreateProfile : usrBaseControl
    {
        public event EventHandler<ControlValidatedEventArgs> ControlValidated;
        private bool updateMode;

        public usrCreateProfile()
        {
            InitializeComponent();
            fillSuperTips();
            txtUserName.Properties.MaxLength = Constants.NameColumnLength;
            this.luCountries.Properties.DataSource = Country.Countries;
            this.luCountries.Properties.DisplayMember = "EnglishName";
            this.luCountries.Properties.ValueMember = "GeoId";
            rbMale.Properties.ValueChecked = Gender.Male;
            rbFemale.Properties.ValueChecked = Gender.Female;
        }

        public bool UpdateMode
        {
            get { return updateMode; }
        }

        void fillSuperTips()
        {
            //ControlHelper.AddSuperTip(txtPassword, lblPassword.Text, SuperTips.CreateProfile_PasswordTE);
            //ControlHelper.AddSuperTip(txtName, lblProfileName.Text, SuperTips.CreateProfile_NameTE);
            //ControlHelper.AddSuperTip(chkAutologin, chkAutologin.Text, SuperTips.CreateProfile_Autologin_CHK);
       }

        public bool ValidateData()
        {
            //validationProvider1.PerformValidation(luCountries);

            return (rbMale.Checked || rbFemale.Checked) && !string.IsNullOrWhiteSpace(txtUserName.Text) && dtaBornDate.DateTime > DateTime.MinValue &&
                   !string.IsNullOrWhiteSpace(txtPassword1.Text) && !string.IsNullOrWhiteSpace(txtEmail.Text) && txtVerifyPassword.Text == txtPassword1.Text && luCountries.EditValue != null;
        }

        public void Fill(ProfileDTO profile)
        {
            updateMode = profile != null && profile.Id !=Constants.UnsavedObjectId;
            txtUserName.Properties.ReadOnly = UpdateMode;
            txtEmail.Properties.ReadOnly = UpdateMode;
            if (profile != null)
            {
                txtUserName.Text = profile.UserName;
                dtaBornDate.DateTime = profile.Birthday;
                txtEmail.Text = profile.Email;
                luCountries.EditValue = profile.CountryId;
                rbMale.Checked = profile.Gender == Gender.Male;
                rbFemale.Checked = profile.Gender == Gender.Female;
            }
        }


        public bool SaveProfile(ProfileDTO profile)
        {
            if (!validationProvider1.DoValidate(this))
            {
                return false;
            }
            profile.UserName = txtUserName.Text;
            profile.Birthday = dtaBornDate.DateTime;
            profile.Email = txtEmail.Text;
            if (luCountries.EditValue != null)
            {
                profile.CountryId = (int) luCountries.EditValue;
            }
            if(rbMale.Checked)
            {
                profile.Gender = Gender.Male;
            }
            else if(rbFemale.Checked)
            {
                profile.Gender = Gender.Female;
            }
            else
            {
                profile.Gender = Gender.NotSet;
            }
            
            if (!string.IsNullOrEmpty(txtPassword1.Text))
            {
                profile.Password = txtPassword1.Text.ToSHA1Hash();
            }
            return true;
        }
        
        protected void OnControlValidated(bool isValid)
        {
            if (ControlValidated != null)
            {
                ControlValidated(this, new ControlValidatedEventArgs(isValid));
            }
        }

        private void validationProvider1_ValidationPerformed(object sender, Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WinForms.ValidationPerformedEventArgs e)
        {
            dxErrorProvider1.SetError(e.ValidatedControl, null, ErrorType.None);
            foreach (ValidationResult validationResult in e.ValidationResults)
            {
                dxErrorProvider1.SetError(e.ValidatedControl, validationResult.Message, ErrorType.Default);
            }
        }

        void setAvailabilityPicture(bool isAvailable, bool isVisible = true)
        {
            ParentWindow.SynchronizationContext.Send(delegate
            {
                picUserNameAvailability.Image = isAvailable ? Icons.Ok : Icons.Cancel;
                picUserNameAvailability.Visible = isVisible;
            }, null);
        }

        private CancellationTokenSource tokenSource;
        private void txtUserName_EditValueChanged(object sender, EventArgs e)
        {
            if(UpdateMode)
            {
                return;
            }

            OnControlValidated(ValidateData());
            if (tokenSource!=null)
            {
                tokenSource.Cancel();
            }
            tokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(delegate(object obj)
                                      {
                                          try
                                          {
                                              CancellationToken cancelToken = (CancellationToken)obj;
                                              if (string.IsNullOrWhiteSpace(txtUserName.Text))
                                              {
                                                  setAvailabilityPicture(false, false);
                                              }
                                              else
                                              {
                                                  var isAvailable =
                                                      ServiceManager.CheckProfileNameAvailability(txtUserName.Text);
                                                  if(cancelToken.IsCancellationRequested)
                                                  {
                                                      return;
                                                  }
                                                  setAvailabilityPicture(isAvailable);
                                              }
                                          }
                                          catch (Exception)
                                          {
                                          }
                                      },tokenSource.Token);

        }

        private void txtEmail_EditValueChanged(object sender, EventArgs e)
        {
            OnControlValidated(ValidateData());
        }

        private void txtVerifyPassword_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            dxValidationProvider1.Validate(txtVerifyPassword);
        }

    }
}
