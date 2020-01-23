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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.UI.Views.MyPlace
{
    /// <summary>
    /// Interaction logic for usrMyPlaceDetails.xaml
    /// </summary>
    public partial class usrMyPlaceDetails : IEditableControl
    {
        private MyPlaceDTO myPlace;

        public usrMyPlaceDetails()
        {
            InitializeComponent();
        }

        public void Fill(MyPlaceDTO myPlace)
        {
            if (myPlace.Address == null)
            {
                myPlace.Address = new AddressDTO();
            }
            Object = myPlace;
        }

        public object Object
        {
            get { return DataContext; }
            set
            {
                myPlace = (MyPlaceDTO)value;
                DataContext = value;
            }
        }

        public bool ReadOnly
        {
            get;
            set;
        }

        public MyPlaceDTO MyPlace
        {
            get { return myPlace; }
        }

        public object Save()
        {
            return ServiceManager.SaveMyPlace(myPlace);
        }
    }
}
