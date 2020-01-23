using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;

namespace BodyArchitect.WP7.ViewModel
{
    public class CustomerViewModel:ViewModelBase
    {
        private CustomerDTO customer;

        public CustomerViewModel(CustomerDTO customer)
        {
            this.customer = customer;
        }

        public CustomerDTO Customer
        {
            get { return customer; }
        }

        public string FULLNAME
        {
            get { return customer.FullName.ToUpper(); }
        }

        public PictureInfoDTO Picture
        {
            get
            {
                if (customer.Picture != null)
                {
                    return customer.Picture;
                }
                return PictureInfoDTO.Empty;
            }
            set { }
        }

        public WymiaryDTO Wymiary
        {
            get
            {
                if (customer != null)
                {
                    return customer.Wymiary;
                }
                return null;
            }
            set { }
        }

        public string CreatedDate
        {
            get { return customer.CreationDate.ToRelativeDate(); }
            set { }
        }

        public string Gender
        {
            get { return EnumLocalizer.Default.Translate(customer.Gender); }
            set { }
        }

        public bool HasMeasurements
        {
            get { return Wymiary != null; }
        }

        public bool HasBirthday
        {
            get { return customer.Birthday != null; }
        }

        public bool HasEmail
        {
            get { return !string.IsNullOrEmpty(customer.Email); }
        }

        public bool HasPhone
        {
            get { return !string.IsNullOrEmpty(customer.PhoneNumber); }
        }
    }
}
