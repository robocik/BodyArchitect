using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
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
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Module.StrengthTraining.Localization;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.UserControls;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls
{
    [Export(typeof(IUserDetailControlBuilder))]
    public class usrUserWorkoutPlansBuilder : IUserDetailControlBuilder
    {
        public IUserDetailControl Create()
        {
            return new usrUserWorkoutPlans();
        }
    }

    public partial class usrUserWorkoutPlans : IUserDetailControl
    {
        private ProfileInformationDTO currentUser;
        private bool filled;
        private WorkoutPlanSearchCriteria criteria;

        public usrUserWorkoutPlans()
        {
            InitializeComponent();
        }

        public void Fill(ProfileInformationDTO user, bool isActive)
        {
            currentUser = user;
            if (user != null)
            {
                if (isActive && (!filled || currentUser == null || currentUser.User.GlobalId != user.User.GlobalId))
                {
                    filled = true;
                    usrWorkoutPlansPagerListView1.ClearContent();
                    usrWorkoutCommentsList1.ClearContent();
                    DoSearch();
                }
            }
            else
            {
                filled = false;
                usrWorkoutPlansPagerListView1.ClearContent();
                usrWorkoutCommentsList1.Fill(null);
            }

        }

        protected override PagedResult<TrainingPlan> RetrieveItems(PartialRetrievingInfo pagerInfo)
        {
            return ServiceManager.GetWorkoutPlans(criteria, pagerInfo);
        }



        protected override void FillResults(ObservableCollection<TrainingPlan> result)
        {
            usrWorkoutPlansPagerListView1.Fill(Items,null);
        }

        protected override void BeforeSearch(object param = null)
        {
            criteria = new WorkoutPlanSearchCriteria();
            criteria.SearchGroups.Add(WorkoutPlanSearchCriteriaGroup.Mine);
            criteria.UserId = currentUser.User.GlobalId;
        }

        public string Caption
        {
            get { return StrengthTrainingEntryStrings.usrUserWorkoutPlans_Caption; }
        }

        public ImageSource SmallImage
        {
            get
            {
                return @"pack://application:,,,/BodyArchitect.Client.Module.StrengthTraining;component/Images/WorkoutPlan.png".ToBitmap();
            }
        }

        public bool UpdateGui(ProfileInformationDTO user)
        {
            return user != null && !user.User.IsMe();
        }

        private void commentsSplitter_Collapsed(object sender, EventArgs e)
        {
            if(!commentsSplitter.IsCollapsed)
            {
                usrWorkoutCommentsList1.Fill(usrWorkoutPlansPagerListView1.SelectedPlan);
            }
        }

        private void WorkoutPlansList1_SelectedPlanChanged(object sender, EventArgs e)
        {
            if (!commentsSplitter.IsCollapsed)
            {
                usrWorkoutCommentsList1.Fill(usrWorkoutPlansPagerListView1.SelectedPlan);
            }
        }
    }

    public abstract class UserWorkoutPlansPagerList : PagerListUserControl<TrainingPlan>
    {
    }
}
