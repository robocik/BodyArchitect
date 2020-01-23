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
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.StrengthTraining.Localization;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls
{
    /// <summary>
    /// Interaction logic for ExercisesList.xaml
    /// </summary>
    public partial class ExercisesList
    {
        public ExercisesList()
        {
            InitializeComponent();
        }

        public void Fill(IList<ExerciseDTO> exercises)
        {
            lstExercises.ItemsSource = exercises.Select(x => new ExerciseItemViewModel(x));

            //if (ShowsGroups)
            //{
            //    CollectionView myView = (CollectionView)CollectionViewSource.GetDefaultView(lstExercises.ItemsSource);
            //    PropertyGroupDescription groupDescription = new PropertyGroupDescription("Group");
            //    myView.GroupDescriptions.Add(groupDescription);
            //}

        }


        public event EventHandler SelectedPlanChanged;

        public ExerciseDTO SelectedExercise
        {
            get
            {
                if (lstExercises.SelectedItem != null)
                {
                    return ((ExerciseItemViewModel)lstExercises.SelectedItem).Exercise;
                }
                return null;
            }
        }

        private void lstExercises_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedPlanChanged != null)
            {
                SelectedPlanChanged(this, EventArgs.Empty);
            }
        }

        public void ClearContent()
        {
            lstExercises.ItemsSource = null;
        }

        private void lblUserName_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)e.OriginalSource;
            UserDTO user = (UserDTO)btn.Tag;
            if (!user.IsDeleted && !user.IsMe())
            {
                MainWindow.Instance.ShowUserInformation(user);
            }
        }
    }

}
