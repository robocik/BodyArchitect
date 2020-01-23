using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.ViewModel;

namespace BodyArchitect.WP7.UserControls
{
    public partial class AwardsControl : INotifyPropertyChanged
    {
        public AwardsControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty UserProperty =
                DependencyProperty.Register("User",
                typeof(UserSearchDTO), typeof(AwardsControl),
                new PropertyMetadata(null, OnSourceChanged));

        public UserSearchDTO User
        {
            get
            {
                return (UserSearchDTO)GetValue(UserProperty);
            }
            set { SetValue(UserProperty, value); }
        }

        static void OnSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var img = (AwardsControl)obj;
            if (args.NewValue != null)
            {
                img.DataContext = new AwardsViewModel((UserSearchDTO)args.NewValue);
 
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
