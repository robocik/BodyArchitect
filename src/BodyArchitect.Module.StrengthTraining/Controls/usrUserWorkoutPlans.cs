using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Common.Plugins;
using BodyArchitect.Controls;
using BodyArchitect.Module.StrengthTraining.Localization;
using BodyArchitect.Service.Model;
using DevExpress.XtraEditors;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    [Export(typeof(IUserDetailControl))]
    public partial class usrUserWorkoutPlans : XtraUserControl, IUserDetailControl
    {
        private ProfileInformationDTO currentUser;
        private bool filled;

        public usrUserWorkoutPlans()
        {
            InitializeComponent();
        }

        public void Fill(ProfileInformationDTO user, bool isActive)
        {
            if (user != null)
            {
                if (isActive && (!filled || currentUser == null || currentUser.User.Id != user.User.Id))
                {
                    filled = true;
                    usrWorkoutPlansPagerListView1.Clear();
                    usrWorkoutCommentsList1.Fill(null);
                    WorkoutPlanSearchCriteria criteria = WorkoutPlanSearchCriteria.CreateFindAllCriteria();
                    criteria.UserName = user.User.UserName;
                    usrWorkoutPlansPagerListView1.Fill(criteria);
                }
            }
            else
            {
                filled = false;
                usrWorkoutPlansPagerListView1.Clear();
                usrWorkoutCommentsList1.Fill(null);
            }

            currentUser = user;
        }

        public string Caption
        {
            get { return StrengthTrainingEntryStrings.usrUserWorkoutPlans_Caption; }
        }

        public Image SmallImage
        {
            get { return StrengthTrainingResources.TrainingPlan; }
        }

        public bool UpdateGui(ProfileInformationDTO user)
        {
            return user != null && !user.User.IsMe();
        }

        private void usrWorkoutPlansPagerListView1_SelectedPlanChanged(object sender, EventArgs e)
        {
            
            usrWorkoutCommentsList1.Fill(usrWorkoutPlansPagerListView1.SelectedPlan);
        }
    }
}
