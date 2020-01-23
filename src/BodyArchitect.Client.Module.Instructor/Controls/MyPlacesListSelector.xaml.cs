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
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor.Controls
{
    /// <summary>
    /// Interaction logic for MyPlacesListSelector.xaml
    /// </summary>
    public partial class MyPlacesListSelector
    {
        public MyPlacesListSelector()
        {
            InitializeComponent();
            lsItems.DataContext = this;
            Connect(lsItems);

        }

        #region MyPlaces

        public static readonly DependencyProperty MyPlacesProperty =
            DependencyProperty.Register("MyPlaces", typeof(IEnumerable<MyPlaceDTO>), typeof(MyPlacesListSelector),
            new FrameworkPropertyMetadata(null));

        public IEnumerable<MyPlaceDTO> MyPlaces
        {
            get { return (IEnumerable<MyPlaceDTO>)GetValue(MyPlacesProperty); }
            set { SetValue(MyPlacesProperty, value); }
        }


        #endregion 
    }
}
