using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Common.Plugins;
using BodyArchitect.Controls;
using BodyArchitect.Module.StrengthTraining.Localization;
using BodyArchitect.Service.Model;
using DevExpress.XtraEditors;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    [Export(typeof(IUserDetailControl))]
    public partial class usrUserExercises : DevExpress.XtraEditors.XtraUserControl, IUserDetailControl
    {
        private ProfileInformationDTO currentUser;
        private bool filled;

        public usrUserExercises()
        {
            InitializeComponent();
        }

        #region IUserDetailControl Members

        public void Fill(ProfileInformationDTO user, bool isActive)
        {
            if (user != null)
            {
                if (isActive && (!filled || currentUser == null || currentUser.User.Id != user.User.Id))
                {
                    filled = true;
                    ExerciseSearchCriteria criteria = ExerciseSearchCriteria.CreateAllCriteria();
                    criteria.UserId = user.User.Id;
                    usrExercisesView1.Fill(criteria);
                }
            }
            else
            {
                usrExercisesView1.ClearContent();
            }

            currentUser = user;
        }

        public string Caption
        {
            get { return StrengthTrainingEntryStrings.usrUserExercises_Caption; }
        }

        public Image SmallImage
        {
            get { return StrengthTrainingResources.StrengthTrainingModule; }
        }

        public bool UpdateGui(ProfileInformationDTO user)
        {
            return user != null && !user.User.IsMe();
        }

        #endregion
    }
}
