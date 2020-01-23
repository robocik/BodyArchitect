using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;

namespace BodyArchitect.WP7.ViewModel
{
    public class WorkoutPlansViewModel:ViewModelBase
    {
        private ObservableCollection<WorkoutPlanViewModel> _myPlans;
        private ObservableCollection<WorkoutPlanViewModel> _favoritePlans;
        public event EventHandler WorkoutPlansLoaded;

        public void LoadPlans()
        {
            ApplicationState.Current.Cache.TrainingPlans.Loaded += new EventHandler(ObjectsReposidory_WorkoutPlansLoaded);
            ApplicationState.Current.Cache.TrainingPlans.Load();
        }

        public void Refresh()
        {
            ApplicationState.Current.Cache.TrainingPlans.Loaded += new EventHandler(ObjectsReposidory_WorkoutPlansLoaded);
            ApplicationState.Current.Cache.TrainingPlans.Refresh();
        }

        public WorkoutPlanViewModel SelectedPlan { get; set; }

        public bool HasFavorites
        {
            get
            {
                if (FavoritePlans==null)
                {
                    return true;
                }
                return FavoritePlans.Count > 0;
            }
        }
        public bool HasPlans
        {
            get
            {
                if (_myPlans == null)
                {
                    return true;
                }
                return _myPlans.Count > 0;
            }
        }

        public ObservableCollection<WorkoutPlanViewModel> MyPlans
        {
            get
            {
                return _myPlans;
            }

            private set
            {
                _myPlans = value;
                NotifyPropertyChanged("MyPlans");
            }
        }

        public ObservableCollection<WorkoutPlanViewModel> FavoritePlans
        {
            get
            {
                return _favoritePlans;
            }

            private set
            {
                _favoritePlans = value;
                NotifyPropertyChanged("FavoritePlans");
            }
        }
        
        void ObjectsReposidory_WorkoutPlansLoaded(object sender, EventArgs e)
        {
            if (MyPlans == null)
            {
                MyPlans = new ObservableCollection<WorkoutPlanViewModel>();
                FavoritePlans=new ObservableCollection<WorkoutPlanViewModel>();
            }
            else
            {
                MyPlans.Clear();
                FavoritePlans.Clear();
            }
            if (ApplicationState.Current.Cache.TrainingPlans.IsLoaded)
            {
                foreach (var plan in ApplicationState.Current.Cache.TrainingPlans.Items.Values.OrderBy(x => x.Name))
                {
                    if (plan.IsMine)
                    {
                        MyPlans.Add(new WorkoutPlanViewModel(plan));
                    }
                    else
                    {
                        FavoritePlans.Add(new WorkoutPlanViewModel(plan));
                    }
                }
                NotifyPropertyChanged("HasFavorites");
                NotifyPropertyChanged("HasPlans");
            }
            if (WorkoutPlansLoaded != null)
            {
                WorkoutPlansLoaded(this, e);
            }
            ApplicationState.Current.Cache.TrainingPlans.Loaded -= new EventHandler(ObjectsReposidory_WorkoutPlansLoaded);
        }

        public void RemoveFromFavorites(WorkoutPlanViewModel item)
        {
            FavoritePlans.Remove(item);
        }
    }
}
