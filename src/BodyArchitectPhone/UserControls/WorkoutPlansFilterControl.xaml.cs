using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;

namespace BodyArchitect.WP7.UserControls
{
    public partial class WorkoutPlansFilterControl : UserControl
    {
        public WorkoutPlansFilterControl()
        {
            InitializeComponent();
        }

        public WorkoutPlanSearchCriteria GetSearchCriteria()
        {
            var allCriteria = new WorkoutPlanSearchCriteria();
            allCriteria.SearchGroups.Add(WorkoutPlanSearchCriteriaGroup.Other);
            if (lpType.SelectedIndex>0)
            {
                allCriteria.WorkoutPlanType.Add((TrainingType)lpType.SelectedIndex-1);
            }
            if (lpPurpose.SelectedIndex > 0)
            {
                allCriteria.Purposes.Add((WorkoutPlanPurpose)lpPurpose.SelectedIndex - 1);
            }
            if (lpDifficult.SelectedIndex > 0)
            {
                allCriteria.Difficults.Add((TrainingPlanDifficult)lpDifficult.SelectedIndex - 1);
            }
            allCriteria.SortOrder = tsOrderBy.IsChecked == true ? SearchSortOrder.Newest : SearchSortOrder.HighestRating;
            return allCriteria;
        }

        private void tsOrder_Checked(object sender, RoutedEventArgs e)
        {
            tsOrderBy.Content = tsOrderBy.IsChecked == true ? ApplicationStrings.WorkoutPlansFilterControl_Order_Newest : ApplicationStrings.WorkoutPlansFilterControl_Order_TopRated;
        }
    }
}
