using System;
using System.Windows;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;

namespace BodyArchitect.WP7.ViewModel
{
    public class WorkoutPlanViewModel:ViewModelBase
    {
        private TrainingPlan plan;
        public event EventHandler Loaded;

        public WorkoutPlanViewModel()
        {
            
        }
        public WorkoutPlanViewModel(TrainingPlan plan)
        {
            this.plan = plan;
        }

        public PictureInfoDTO Picture
        {
            get
            {
                if (plan.Profile != null)
                {
                    if (plan.Profile.Picture != null)
                    {
                        return plan.Profile.Picture;
                    }
                    return PictureInfoDTO.Empty;
                }
                return ApplicationState.Current.SessionData.Profile.Picture;
            }
            set { }
        }

        public TrainingPlan Plan
        {
            get { return plan; }
            set { plan = value; }
        }

        public string PublishedDate
        {
            get
            {
                if (Plan.PublishDate.HasValue)
                {
                    return Plan.PublishDate.Value.ToLocalTime().ToRelativeDate();
                }
                return string.Empty;
            }
            set { }
        }

        public string PublicStatusImage
        {
            get { return Plan.Status == PublishStatus.Published ? "/Images/public.png" : "/Images/private.png"; }
            set { }
        }

        public Visibility ShowGoToWeb
        {
            get
            {
                
                if (Plan != null)
                {
                    return string.IsNullOrEmpty(Plan.Url) ? Visibility.Collapsed : Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public string Url
        {
            get
            {
                if (Plan != null)
                {
                    return Plan.Url;
                }
                return string.Empty;
            }
        }

        public string CreatedDate
        {
            get
            {
                return Plan.CreationDate.ToLocalTime().ToRelativeDate();
            }
            set { }
        }


        public string Difficult
        {
            get { return EnumLocalizer.Default.Translate(plan.Difficult); }
            set { }
        }

        public string Purpose
        {
            get { return EnumLocalizer.Default.Translate(plan.Purpose); }
            set { }
        }

        public string Description
        {
            get
            {
                if (string.IsNullOrEmpty(Plan.Comment))
                {
                    return ApplicationStrings.WorkoutPlanViewModel_Description_Empty;
                }
                return Plan.Comment;
            }
            set { }
        }

        public string TrainingType
        {
            get { return EnumLocalizer.Default.Translate(plan.TrainingType); }
            set { }
        }

        public double Rating
        {
            get
            {
                return Plan.Rating / Constants.MaxRatingValue;
            }
            set { }
        }

        public string Author
        {
            get
            {
                if (plan.Profile != null)
                {
                    return plan.Profile.UserName;
                }
                return ApplicationState.Current.SessionData.Profile.UserName;
            }
            set { }
        }

        void onLoaded()
        {
            if (Loaded != null)
            {
                Loaded(this, EventArgs.Empty);
            }
        }

        //public void Load()
        //{
        //    if (!Plan.IsContentLoaded)
        //    {
        //        var m =new ServiceManager<GetWorkoutPlanCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1,EventHandler<GetWorkoutPlanCompletedEventArgs> operationCompleted)
        //                    {
        //                        client1.GetWorkoutPlanAsync(ApplicationState.Current.SessionData.Token, Plan.GlobalId,
        //                                                    new RetrievingInfo() {LongTexts = true});
        //                        client1.GetWorkoutPlanCompleted -= operationCompleted;
        //                        client1.GetWorkoutPlanCompleted += operationCompleted;

        //                    });


        //        m.OperationCompleted += (s, a) =>
        //                    {

        //                        if (a.Error != null)
        //                        {
        //                            onLoaded();
        //                            BAMessageBox.ShowError(ApplicationStrings.WorkoutPlanViewModel_LoadError);
        //                            return;
        //                        }

        //                        //Plan.SetContent(a.Result.Result.PlanContent);
        //                        Plan.Tag = a.Result.Result;
        //                        if (ApplicationState.Current.Cache.Exercises.IsLoaded)
        //                        {
        //                            onLoaded();
        //                            NotifyPropertyChanged("ShowGoToWeb");
        //                            NotifyPropertyChanged("Description");
        //                            NotifyPropertyChanged("TrainingPlan");
        //                        }
        //                        else
        //                        {
        //                            ApplicationState.Current.Cache.Exercises.Loaded +=
        //                                (s1, a1) =>
        //                                    {
        //                                        onLoaded();
        //                                        NotifyPropertyChanged("Description");
        //                                        if (ApplicationState.Current.Cache.Exercises.IsLoaded)
        //                                        {
        //                                            NotifyPropertyChanged("TrainingPlan");    
        //                                        }
        //                                        else
        //                                        {
        //                                            BAMessageBox.ShowError(ApplicationStrings.ExerciseTypeViewModel_ErrRetrieveExercises);
        //                                        }
        //                                    };
        //                            ApplicationState.Current.Cache.Exercises.Load();
        //                        }


        //                    };
        //        if (!m.Run())
        //        {
        //            onLoaded();
        //            if (ApplicationState.Current.IsOffline)
        //            {
        //                BAMessageBox.ShowError(ApplicationStrings.ErrOfflineMode);
        //            }
        //            else
        //            {
        //                BAMessageBox.ShowError(ApplicationStrings.ErrNoNetwork);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        onLoaded();
        //    }
        //}
    }
}
