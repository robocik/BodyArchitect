using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor.Controls
{
    public class ChampionshipItem:ListItem<ChampionshipType>
    {
        public ChampionshipItem(ChampionshipType type):base(InstructorHelper.Translate(type),type)
        {
            Brush = Brushes.Thistle;
        }
    }
    /// <summary>
    /// Interaction logic for ActivitiesListSelector.xaml
    /// </summary>
    public partial class ActivitiesListSelector
    {
        private ObservableCollection<ChampionshipItem> championships = new ObservableCollection<ChampionshipItem>();

        public ActivitiesListSelector()
        {
            InitializeComponent();

            foreach (ChampionshipType championshipType in Enum.GetValues(typeof(ChampionshipType)))
            {
                championships.Add(new ChampionshipItem(championshipType));
            }

            lsItems.DataContext = this;
            Connect(lsItems);
            Connect(lsChampionshipsItems);
            
        }

        public IList<ChampionshipItem> Championships
        {
            get { return championships; }
        }

        #region Activities

        public static readonly DependencyProperty ActivitiesProperty =
            DependencyProperty.Register("Activities", typeof(IEnumerable<ActivityDTO>), typeof(ActivitiesListSelector),
            new FrameworkPropertyMetadata(null));

        public IEnumerable<ActivityDTO> Activities
        {
            get { return (IEnumerable<ActivityDTO>)GetValue(ActivitiesProperty); }
            set { SetValue(ActivitiesProperty, value); }
        }


        #endregion   

    }
}
