using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using ValidationResult = Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult;

namespace BodyArchitect.Client.UI.UserControls
{
    /// <summary>
    /// Interaction logic for usrCreateProfile.xaml
    /// </summary>
    public partial class usrCreateProfile : usrBaseControl
    {
        private bool updateMode;

        public usrCreateProfile()
        {
            InitializeComponent();
            DataContext = this;
            txtUserName.MaxLength = Constants.NameColumnLength;
            
        }

        

        public bool UpdateMode
        {
            get { return updateMode; }
        }

        public ProfileDTO Profile
        {
            get { return profile; }
            set
            {
                profile = value;
                NotifyOfPropertyChange(()=>Profile);
                NotifyOfPropertyChange(() => IsMale);
                NotifyOfPropertyChange(() => IsFemale);
            }
        }

        public int CountryId
        {
            get { return profile != null ?profile.CountryId:0; }
            set
            {
                if (profile != null )
                {
                    profile.CountryId = value;
                }
            }
        }

        public bool? IsMale
        {
            get { return profile != null && profile.Gender == Gender.Male; }
            set
            {
                if (profile!=null && value == true)
                {
                    profile.Gender = Gender.Male;
                }
            }
        }

        public bool? IsFemale
        {
            get { return profile != null && profile.Gender == Gender.Female; }
            set
            {
                if (profile != null && value == true)
                {
                    profile.Gender = Gender.Female;
                }
            }
        }

        public override bool ValidateData()
        {
            //validationProvider1.PerformValidation(luCountries);
            //bool valid = epError.Validate();
            //if (!valid)
            //{
            //    epError.GetFirstInvalidElement().Focus();
            //}
            return (rbMale.IsChecked.Value || rbFemale.IsChecked.Value) && !string.IsNullOrWhiteSpace(txtUserName.Text) && dpBirthday.SelectedDate > DateTime.MinValue &&
                   !string.IsNullOrWhiteSpace(txtPassword.Password) && !string.IsNullOrWhiteSpace(txtEmail.Text) && txtVerifyPassword.Password == txtPassword.Password && cmbCountry.SelectedItem != null;
        }

        private ProfileDTO profile;

        public void Fill(ProfileDTO profile)
        {
            updateMode = profile != null && profile.GlobalId != Constants.UnsavedGlobalId;
            this.Profile = profile;
            DataContext = this;
            cmbWeightType.SelectedIndex = (int)profile.Settings.WeightType;
            cmbLengthType.SelectedIndex = (int)profile.Settings.LengthType;
            txtUserName.IsReadOnly = UpdateMode;
            txtEmail.IsReadOnly = UpdateMode;
            if (profile != null)
            {
                txtUserName.Text = profile.UserName;
                dpBirthday.SelectedDate = profile.Birthday;
                txtEmail.Text = profile.Email;
                cmbCountry.SelectedValue = profile.CountryId;
                rbMale.IsChecked = profile.Gender == Gender.Male;
                rbFemale.IsChecked = profile.Gender == Gender.Female;
            }
        }


        public bool SaveProfile(ProfileDTO profile)
        {
            //if (!validationProvider1.DoValidate(this))
            //{
            //    return false;
            //}
            //bool valid = epError.Validate();
            //if (!valid)
            //{
            //    epError.GetFirstInvalidElement().Focus();
            //    return false;
            //}

            profile.Settings.LengthType = (LengthType)cmbLengthType.SelectedIndex;
            profile.Settings.WeightType = (WeightType)cmbWeightType.SelectedIndex;
            profile.UserName = txtUserName.Text;
            profile.Birthday = dpBirthday.SelectedDate.Value;
            profile.Email = txtEmail.Text;
            if (cmbCountry.SelectedItem != null)
            {
                profile.CountryId = (int)cmbCountry.SelectedValue;
            }
            if (rbMale.IsChecked.Value)
            {
                profile.Gender = Gender.Male;
            }
            else if (rbFemale.IsChecked.Value)
            {
                profile.Gender = Gender.Female;
            }
            else
            {
                profile.Gender = Gender.NotSet;
            }

            if (!string.IsNullOrEmpty(txtPassword.Password))
            {
                profile.Password = txtPassword.Password.ToSHA1Hash();
            }
            return true;
        }

        

        //private void validationProvider1_ValidationPerformed(object sender, Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WinForms.ValidationPerformedEventArgs e)
        //{
        //    dxErrorProvider1.SetError(e.ValidatedControl, null, ErrorType.None);
        //    foreach (ValidationResult validationResult in e.ValidationResults)
        //    {
        //        dxErrorProvider1.SetError(e.ValidatedControl, validationResult.Message, ErrorType.Default);
        //    }
        //}

        void setAvailabilityPicture(bool isAvailable, bool isVisible = true)
        {
            UIHelper.BeginInvoke(delegate
            {
                imgCancel.Visibility = Visibility.Collapsed;
                imgOK.Visibility = Visibility.Collapsed;
                if (isVisible)
                {
                    if (isAvailable)
                    {
                        imgOK.Visibility = Visibility.Visible;

                    }
                    else
                    {
                        imgCancel.Visibility = Visibility.Visible;

                    }
                }
            },Dispatcher);
            
        }

        private CancellationTokenSource tokenSource;

        private void txtUserName_TextChanged(object sender, EventArgs e)
        {
            if (UpdateMode)
            {
                return;
            }

            OnControlValidated(ValidateData());
            if (tokenSource != null)
            {
                tokenSource.Cancel();
            }

            string userName = txtUserName.Text;
            tokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(delegate(object obj)
            {
                try
                {
                    Helper.EnsureThreadLocalized();
                    CancellationToken cancelToken = (CancellationToken)obj;
                    if (string.IsNullOrWhiteSpace(userName))
                    {
                        setAvailabilityPicture(false, false);
                    }
                    else
                    {
                        var isAvailable = ServiceManager.CheckProfileNameAvailability(userName);
                        if (cancelToken.IsCancellationRequested)
                        {
                            return;
                        }
                        setAvailabilityPicture(isAvailable);
                    }
                }
                catch (Exception)
                {
                }
            }, tokenSource.Token);

        }

        private void txtEmail_EditValueChanged(object sender, EventArgs e)
        {
            OnControlValidated(ValidateData());
        }


        private void dpBirthday_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            OnControlValidated(ValidateData());
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            OnControlValidated(ValidateData());
        }
    }
}
