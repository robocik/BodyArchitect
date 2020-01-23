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
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.WP7.UserControls
{
    public partial class UsersFilterControl : UserControl
    {
        public UsersFilterControl()
        {
            InitializeComponent();
        }

        public UserSearchCriteria GetSearchCriteria()
        {
            var allCriteria = new UserSearchCriteria();
            allCriteria.UserSearchGroups.Add(UserSearchGroup.Others);
            switch (lpOrderBy.SelectedIndex)
            {
                case 0:
                    allCriteria.SortOrder = UsersSortOrder.ByLastLoginDate;
                    break;
                case 1:
                    allCriteria.SortOrder = UsersSortOrder.ByTrainingDaysCount;
                    break;
                case 2:
                    allCriteria.SortOrder = UsersSortOrder.ByName;
                    break;
            }

            if (lpGender.SelectedIndex == 0 || lpGender.SelectedIndex == 1)
            {
                allCriteria.Genders.Add(Gender.Male);
            }
            if (lpGender.SelectedIndex == 0 || lpGender.SelectedIndex == 2)
            {
                allCriteria.Genders.Add(Gender.Female);
            }
            return allCriteria;
        }
    }
}
