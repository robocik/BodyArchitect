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

namespace BodyArchitect.Client.Module.Suplements.Controls
{
    /// <summary>
    /// Interaction logic for usrSupplementCycleWeekEditor.xaml
    /// </summary>
    public partial class usrSupplementCycleWeekEditor
    {
        public usrSupplementCycleWeekEditor()
        {
            InitializeComponent();
        }

        public SupplementsCycleWeekViewModel SelectedWeek
        {
            get { return (SupplementsCycleWeekViewModel)GetValue(SelectedWeekProperty); }
            set
            {
                SetValue(SelectedWeekProperty, value);
            }
        }

        

        public static readonly DependencyProperty SelectedWeekProperty =
            DependencyProperty.Register("SelectedWeek", typeof(SupplementsCycleWeekViewModel), typeof(usrSupplementCycleWeekEditor), new UIPropertyMetadata(null));

    }
}
