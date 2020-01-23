using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;

namespace BodyArchitect.Client.UI.Controls.PlansUI
{
    public enum FeaturedItem
    {
        Random,
        Latest,
        None,
    }

    public class PlanViewModel : ViewModelBase
    {
        private PlanBase plan;

        public PlanViewModel(PlanBase plan, FeaturedItem featured = FeaturedItem.None)
        {
            this.plan = plan;
            FeaturedType = featured;
        }

        public FeaturedItem FeaturedType { get; private set; }

        protected virtual bool IsFavorite()
        {
            return false;
        }

        public string Group
        {
            get
            {
                if (FeaturedType != FeaturedItem.None)
                {
                    return "WorkoutPlansListView_GroupFeatured".TranslateGUI();
                }
                if (IsFavorite())
                {
                    return "WorkoutPlansListView_GroupFavorites".TranslateGUI();
                }
                else if (plan.IsMine())
                {
                    return "WorkoutPlansListView_GroupMine".TranslateGUI();
                }
                else
                {
                    return "WorkoutPlansListView_GroupOthers".TranslateGUI();
                }
            }
        }

        public string StatusIcon
        {
            get
            {
                if (IsFavorite())
                {
                    return "Favorites16.png".ToResourceString();
                }
                if (plan.Status == PublishStatus.Published)
                {
                    return "StatusPublic16.png".ToResourceString();
                }
                if (plan.IsMine())
                {
                    return "StatusPrivate.png".ToResourceString();
                }

                return null;
            }
        }

        public string StatusIconToolTip
        {
            get
            {
                if (IsFavorite())
                {
                    return "WorkoutPlanViewModel_StatusIconToolTip_Favorite".TranslateGUI();
                }
                if (plan.Status == PublishStatus.Published)
                {
                    return "WorkoutPlanViewModel_StatusIconToolTip_Published".TranslateGUI();
                }
                if (plan.IsMine())
                {
                    return "WorkoutPlanViewModel_StatusIconToolTip_Private".TranslateGUI();
                }

                return null;
            }
        }

        public bool AllowRedirectToDetails
        {
            get { return !plan.Profile.IsMe(); }
        }

        public UserDTO User
        {
            get { return plan.Profile; }
        }

        public string Name
        {
            get { return plan.Name; }
        }

        public decimal Rating
        {
            get { return (decimal)plan.Rating; }
        }

        public string PublicationDate
        {

            get
            {
                if (plan.PublishDate.HasValue)
                {
                    return plan.PublishDate.Value.ToLocalTime().ToRelativeDate();
                }
                return "WorkoutPlanViewModel_PublicationDate_Never".TranslateGUI();
            }
        }

        public PlanBase Plan
        {
            get { return plan; }
        }

        public void Refresh(PlanBase plan)
        {
            this.plan = plan;
            NotifyOfPropertyChange(null);
        }
    }

}
