using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.StrengthTraining.Localization;
using BodyArchitect.Client.Module.StrengthTraining.Model;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Reports;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls
{
    /// <summary>
    /// Interaction logic for usrExercisesView.xaml
    /// </summary>
    public partial class usrExercisesView
    {

        private bool _canNew = false;
        private bool _canEdit = false;
        private bool _canDelete = false;
        private bool _canAddToFavorites = false;
        private bool _canRemoveFromFavorites = false;
        Dictionary<ExerciseType, ExerciseTypeViewModel> muscleNodes = new Dictionary<ExerciseType, ExerciseTypeViewModel>();
        private bool isInProgress;
        private PageContext pageContext;

        public usrExercisesView()
        {
            InitializeComponent();
            DataContext = this;
            updateButtons(true);
        }

        public bool IsInProgress
        {
            get { return isInProgress; }
            set
            {
                isInProgress = value;
                NotifyOfPropertyChange(() => IsInProgress);
            }
        }

        public bool ShowUsersRecords
        {
            get { return usrExerciseRecords.RecordMode == RecordMode.AllUsers; }
            set
            {
                usrExerciseRecords.RecordMode = RecordMode.AllUsers;
                NotifyOfPropertyChange(() => ShowUsersRecords);
                NotifyOfPropertyChange(() => ShowFriendsRecords);
                NotifyOfPropertyChange(() => ShowCustomersRecords);
            }
        }

        public bool ShowFriendsRecords
        {
            get { return usrExerciseRecords.RecordMode == RecordMode.Friends; }
            set
            {
                usrExerciseRecords.RecordMode = RecordMode.Friends;
                NotifyOfPropertyChange(() => ShowFriendsRecords);
                NotifyOfPropertyChange(() => ShowCustomersRecords);
                NotifyOfPropertyChange(() => ShowUsersRecords);
            }
        }

        public bool ShowCustomersRecords
        {
            get { return usrExerciseRecords.RecordMode == RecordMode.Customer; }
            set
            {
                usrExerciseRecords.RecordMode = RecordMode.Customer;
                NotifyOfPropertyChange(() => ShowCustomersRecords);
                NotifyOfPropertyChange(() => ShowFriendsRecords);
                NotifyOfPropertyChange(() => ShowUsersRecords);
            }
        }

        void updateButtons(bool startOperation)
        {
            //exercisesTree1.Enabled = !startOperation;
            CanDelete = false;
            CanEdit =!startOperation && this.SelectedExercise != null && SelectedExercise.IsMine() && !SelectedExercise.IsNew;
            CanNew = !startOperation && (SelectedExercise != null || SelectedExerciseType.HasValue);
            CanAddToFavorites = !startOperation && SelectedExercise.CanAddToFavorites();
            CanRemoveFromFavorites = !startOperation && SelectedExercise.CanRemoveFromFavorites();
        }

        public void Fill(PageContext pageContext)
        {
            this.pageContext = pageContext;
            xtraTabControl.SelectedIndex = pageContext.DisplayMode;
            SearchEnabled = UserContext.Current.LoginStatus == LoginStatus.Logged;
            SearchStatus = string.Empty;

            ParentWindow.RunAsynchronousOperation(delegate
            {

                ExercisesReposidory.Instance.EnsureLoaded();

                Dispatcher.BeginInvoke(new Action(delegate
                      {
                          usrExerciseEditor.ClearContent();
                          usrWorkoutCommentsList1.ClearContent();
                          usrExerciseRecords.ClearContent();
                          fillExerciseTypes();
                          selectedExerciseChanged();

                      }));
            }, asyncOperationStateChange);
        }

        void fillExerciseTypes()
        {
            var enums = Enum.GetValues(typeof(ExerciseType));
            muscleNodes.Clear();
            foreach (ExerciseType exerciseType in enums)
            {
                if (exerciseType == ExerciseType.NotSet)
                {
                    continue;
                }
                if (!muscleNodes.ContainsKey(exerciseType))
                {
                    muscleNodes.Add(exerciseType, new ExerciseTypeViewModel(exerciseType));
                }
            }

            ExerciseTypeViewModel exerciseTypeToExpand = null;
            foreach (var exercise in ExercisesReposidory.Instance.Items.Values.OrderBy(x=>x.Name))
            {
                ExerciseTypeViewModel exerciseNode = muscleNodes[exercise.ExerciseType];
                bool isSelected = this.pageContext.SelectedItem != null && this.pageContext.SelectedItem.Value == exercise.GlobalId;
                if (isSelected)
                {
                    exerciseTypeToExpand = exerciseNode;
                }
                exerciseNode.Exercises.Add(new ExerciseItemViewModel(exercise) { IsSelected = isSelected});
            }
            tvExercises.ItemsSource = ExerciseTypes;

            if (exerciseTypeToExpand != null)
            {
                exerciseTypeToExpand.IsExpanded = true;
            }
            xtraTabControl.SelectedIndex = this.pageContext.DisplayMode;
        }
        

        #region Properties
        public ICollection ExerciseTypes
        {
            get
            {
                return muscleNodes.Values.OrderBy(x=>x.Name.ToString()).ToList();
            }
        }



        public bool CanAddToFavorites
        {
            get { return _canAddToFavorites; }
            set
            {
                _canAddToFavorites = value;
                NotifyOfPropertyChange(() => CanAddToFavorites);
            }
        }

        public bool CanRemoveFromFavorites
        {
            get { return _canRemoveFromFavorites; }
            set
            {
                _canRemoveFromFavorites = value;
                NotifyOfPropertyChange(() => CanRemoveFromFavorites);
            }
        }

        public bool CanNew
        {
            get { return _canNew; }
            set
            {
                _canNew = value;
                NotifyOfPropertyChange(() => CanNew);
            }
        }

        public bool CanEdit
        {
            get { return _canEdit; }
            set
            {
                _canEdit = value;
                NotifyOfPropertyChange(() => CanEdit);
            }
        }

        public bool CanDelete
        {
            get { return _canDelete; }
            set
            {
                _canDelete = value;
                NotifyOfPropertyChange(() => CanDelete);
            }
        }

        


        public ExerciseDTO SelectedExercise
        {
            get
            {
                if (tcExercises.SelectedItem == tpMyExercises)
                {
                    var exercise = tvExercises.SelectedItem as ExerciseItemViewModel;
                    if (exercise != null)
                    {
                        return exercise.Exercise;
                    }
                    return null;
                }
                else
                {
                    return exercisesList.SelectedExercise;
                }


            }
        }

        public ExerciseType? SelectedExerciseType
        {
            get
            {
                var exercise = tvExercises.SelectedItem as ExerciseTypeViewModel;
                if (exercise != null)
                {
                    return exercise.ExerciseType;
                }
                return null;
            }
        }
        #endregion
        protected override void FillResults(ObservableCollection<ExerciseDTO> result)
        {
            exercisesList.Fill(result);
        }

        private void asyncOperationStateChange(OperationContext context)
        {
            bool startOperation = context.State == OperationState.Started;
            updateButtons(startOperation);
            IsInProgress = startOperation;
        }

        public void RefreshView()
        {
            ExercisesReposidory.Instance.ClearCache();
            Fill(pageContext);
        }

        protected override void BeforeSearch(object param = null)
        {
            tcExercises.SelectedItem = tpSearch;
            base.BeforeSearch();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            pageContext.SelectedItem = SelectedExercise != null ? SelectedExercise.GlobalId : (Guid?) null;
            selectedExerciseChanged();
        }

        private void xtraTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //BUG FIX: this if is because wihtout it when we select any comment then this event (tabcontrol selection changed) is invoked)
            if (e.AddedItems.Count == 1 && e.AddedItems[0] is TabItem)
            {
                if (pageContext != null)
                {
                    pageContext.DisplayMode = xtraTabControl.SelectedIndex;
                }
                selectedExerciseChanged();
            }
        }

        private void selectedExerciseChanged()
        {
            if (SelectedExercise != null)
            {
                usrExerciseEditor.Fill(SelectedExercise, ExercisesReposidory.Instance.Items.Values);

                bool showComments = !SelectedExercise.IsNew;
                tpComments.SetVisible(showComments);
                if (showComments && SelectedExercise != null)
                {
                    if (xtraTabControl.SelectedItem == tpComments)
                    {
                        usrWorkoutCommentsList1.CannotVote = SelectedExercise.IsMine();
                        usrWorkoutCommentsList1.Fill(SelectedExercise);    
                    }
                    else if (xtraTabControl.SelectedItem == tpRecords)
                    {
                        usrExerciseRecords.Fill(SelectedExercise);    
                    }
                    
                    
                }

            }
            else
            {
                usrExerciseEditor.ClearContent();
                usrWorkoutCommentsList1.ClearContent();
                usrExerciseRecords.ClearContent();

            }

            
            updateButtons(false);
        }

        private void exercisesList_SelectedPlanChanged(object sender, EventArgs e)
        {
            selectedExerciseChanged();
        }

        private void rbtnNew_Click(object sender, RoutedEventArgs e)
        {
            //ExerciseEditorWindow dlg = new ExerciseEditorWindow();
            //var newExercise = new ExerciseDTO();
            //if (SelectedExerciseType.HasValue)
            //{
            //    newExercise.ExerciseType = SelectedExerciseType.Value;
            //}
            //else if(SelectedExercise!=null)
            //{
            //    newExercise.ExerciseType = SelectedExercise.ExerciseType;
            //}
            //dlg.Fill(newExercise);
            //if(dlg.ShowDialog()==true)
            //{
            //    Fill();
            //}

            if(!UIHelper.EnsurePremiumLicence())
            {
                return;
            }
            var dlg = new EditDomainObjectWindow();

            var ctrl = new usrExerciseEditor();
            var newExercise = new ExerciseDTO();
            if (SelectedExerciseType.HasValue)
            {
                newExercise.ExerciseType = SelectedExerciseType.Value;
            }
            else if (SelectedExercise != null)
            {
                newExercise.ExerciseType = SelectedExercise.ExerciseType;
            }
            ctrl.Fill(newExercise,ExercisesReposidory.Instance.Items.Values);
            dlg.SetControl(ctrl);

            if (dlg.ShowDialog() == true)
            {
                ExercisesReposidory.Instance.Add((ExerciseDTO) ctrl.Object);
                Fill(pageContext);
            }
        }

        private void rbtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!UIHelper.EnsurePremiumLicence())
            {
                return;
            }

            var dlg = new EditDomainObjectWindow();

            var ctrl = new usrExerciseEditor();

            ctrl.Fill(SelectedExercise.StandardClone(), ExercisesReposidory.Instance.Items.Values);
            dlg.SetControl(ctrl);

            if (dlg.ShowDialog() == true)
            {
                ExercisesReposidory.Instance.Update((ExerciseDTO)ctrl.Object);
                Fill(pageContext);
            }
        }

        private void btnMoreResults_Click(object sender, RoutedEventArgs e)
        {
            MoreResults();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            DoSearch();
        }

        private void rbtnAddToFavorites_Click(object sender, RoutedEventArgs e)
        {
            if (!UIHelper.EnsurePremiumLicence())
            {
                return;
            }

            UIHelper.RunWithExceptionHandling(delegate
            {
                if (SelectedExercise.AddToFavorites())
                {
                    Fill(pageContext);
                }
            });
        }

        private void rbtRemoveFromFavorites_Click(object sender, RoutedEventArgs e)
        {
            if (!UIHelper.EnsurePremiumLicence())
            {
                return;
            }
            UIHelper.RunWithExceptionHandling(delegate
            {
                if (SelectedExercise.RemoveFromFavorites())
                {
                    Fill(pageContext);
                }
            });
        }

        
    }


    public class ExerciseTypeViewModel : TreeItemViewModel
    {
        private ExerciseType exerciseType;
        private ObservableCollection<ExerciseItemViewModel> exercises = new ObservableCollection<ExerciseItemViewModel>();

        public ExerciseTypeViewModel(ExerciseType exerciseType)
        {
            this.exerciseType = exerciseType;
        }

        public string Name
        {
            get { return EnumLocalizer.Default.Translate(exerciseType); }
        }

        public ObservableCollection<ExerciseItemViewModel> Exercises
        {
            get { return exercises; }
        }

        public ExerciseType? ExerciseType
        {
            get { return exerciseType; }
        }
    }

    public class ExerciseItemViewModel:TreeItemViewModel
    {
        private ExerciseDTO exercise;

        public ExerciseItemViewModel(ExerciseDTO exercise)
        {
            this.exercise = exercise;
        }
        
        public string Name
        {
            get { return Exercise.GetLocalizedName(); }
        }

        public Guid GlobalId
        {
            get { return Exercise.GlobalId; }
        }

        public bool AllowRedirectToDetails
        {
            get { return exercise.Profile!=null && !exercise.Profile.IsMe(); }
        }

        public decimal Rating
        {
            get { return (decimal)Exercise.Rating; }
        }

        public UserDTO User
        {
            get { return exercise.Profile; }
        }


        public string ExerciseType
        {
            get { return EnumLocalizer.Default.Translate(Exercise.ExerciseType); }
        }
        public string Group
        {
            get { return null; }
        }

        

        public string StatusIconToolTip
        {
            get
            {
                if (exercise.IsFavorite())
                {
                    return "usrExercisesView_StatusIconToolTip_Favorite".TranslateStrength();
                }
                if (exercise.Profile == null)
                {
                    return "usrExercisesView_StatusIconToolTip_Global".TranslateStrength();
                }
                if (exercise.IsMine())
                {
                    return "usrExercisesView_StatusIconToolTip_Private".TranslateStrength();
                }

                return null;
            }
        }

        public ExerciseDTO Exercise
        {
            get { return exercise; }
        }
    }
}
