using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Authentication;
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
using AvalonDock.Layout;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.StrengthTraining.Controls.TrainingPlans;
using BodyArchitect.Client.Module.StrengthTraining.Localization;
using BodyArchitect.Client.Module.StrengthTraining.Model;
using BodyArchitect.Client.Module.StrengthTraining.Model.TrainingPlans;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Controls.PlansUI;
using BodyArchitect.Client.UI.UserControls;
using BodyArchitect.Client.UI.Views;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Service.V2.Model.TrainingPlans;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls
{
    /// <summary>
    /// Interaction logic for usrTrainingPlansView.xaml
    /// </summary>
    public partial class usrTrainingPlansView : IWeakEventListener, IHasFloatingPane
    {
        private PageContext pageContext;
        private usrWorkoutCommentsList commentsControl;

        public usrTrainingPlansView()
        {
            InitializeComponent();
            DataContext = this;
            CollectionChangedEventManager.AddListener(WorkoutPlansReposidory.Instance, this);
            MainWindow.Instance.EnsureAnchorable(Strings.usrWorkoutCommentsList_Header_Comments, "pack://application:,,,/BodyArchitect.Client.Resources;component/Images/Comments16.png", "RatingsCommentsControl", AnchorableShowStrategy.Right);
            
        }

        public Control GetContentForPane(string paneId)
        {
            if (paneId == "RatingsCommentsControl")
            {
                if (commentsControl == null)
                {
                    commentsControl = new usrWorkoutCommentsList();
                    commentsControl.Voted += usrWorkoutCommentsList1_Voted;
                    showComments();
                }
                
                return commentsControl;
            }
            return null;
        }

        #region Ribbon

        private bool _EditGroupEnable=true;
        public bool EditGroupEnable
        {
            get { return _EditGroupEnable; }
            set
            {
                _EditGroupEnable = value;
                NotifyOfPropertyChange(() => EditGroupEnable);
            }
        }

        private bool _canNew;
        public bool CanNew
        {
            get { return _canNew; }
            set
            {
                _canNew = value;
                NotifyOfPropertyChange(() => CanNew);
            }
        }

        private bool _canClone;
        public bool CanClone
        {
            get { return _canClone; }
            set
            {
                _canClone = value;
                NotifyOfPropertyChange(() => CanClone);
            }
        }

        private bool _canView;
        public bool CanView
        {
            get { return _canView; }
            set
            {
                _canView = value;
                NotifyOfPropertyChange(() => CanView);
            }
        }

        private bool _canEdit;
        public bool CanEdit
        {
            get { return _canEdit; }
            set
            {
                _canEdit = value;
                NotifyOfPropertyChange(() => CanEdit);
            }
        }

        private bool _canDelete;
        public bool CanDelete
        {
            get { return _canDelete; }
            set
            {
                _canDelete = value;
                NotifyOfPropertyChange(() => CanDelete);
            }
        }

        private bool _canPublish;
        public bool CanPublish
        {
            get { return _canPublish; }
            set
            {
                _canPublish = value;
                NotifyOfPropertyChange(() => CanPublish);
            }
        }

        private bool _canAddToFavorites;
        public bool CanAddToFavorites
        {
            get { return _canAddToFavorites; }
            set
            {
                _canAddToFavorites = value;
                NotifyOfPropertyChange(() => CanAddToFavorites);
            }
        }

        private bool _canRemoveFromFavorites;
        public bool CanRemoveFromFavorites
        {
            get { return _canRemoveFromFavorites; }
            set
            {
                _canRemoveFromFavorites = value;
                NotifyOfPropertyChange(() => CanRemoveFromFavorites);
            }
        }

        //private bool showComments=true;
        private bool isInProgress;

        //public bool ShowComments
        //{
        //    get { return showComments; }
        //    set
        //    {

        //        showComments = value;
        //        commentsSplitter.Process(!showComments);
        //        if (showComments)
        //        {
        //            showComments();
        //        }

        //        NotifyOfPropertyChange(() => ShowComments);
        //    }
        //}

        #endregion
        
        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            var param = (NotifyCollectionChangedEventArgs) e;
            if(param.Action==NotifyCollectionChangedAction.Reset)
            {
                Dispatcher.BeginInvoke(new Action(ClearContent));
            }
            else
            {
                Dispatcher.BeginInvoke(new Action(fillTreeList));
            }
            
            return true;
        }

        public void Fill(PageContext pageContext)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action<PageContext>(Fill), pageContext);
            }
            else
            {
                IsInProgress = true;
                this.pageContext = pageContext;
                tcPlans.SelectedIndex = pageContext.DisplayMode;
                //FillSearchControls();
                SearchEnabled = UserContext.Current.LoginStatus == LoginStatus.Logged;
                SearchStatus = string.Empty;

                ParentWindow.RunAsynchronousOperation(delegate(OperationContext ctx)
                {
                    WorkoutPlansReposidory.Instance.EnsureLoaded();
                    //ExercisesReposidory.Instance.EnsureLoaded();
                    fillTreeList();
                    IsInProgress = false;

                }, asyncOperationStateChange);
            }
            
        }

        public void RefreshView()
        {
            WorkoutPlansReposidory.Instance.ClearCache();
            FillSearchControls();
            Fill(pageContext);
        }

        
        protected override void LoginStatusChanged(LoginStatus newStatus)
        {
            WorkoutPlansList1.ClearContent();
            updateButtons(false);
        }

        void asyncOperationStateChange(OperationContext context)
        {
            bool startLoginOperation = context.TaskManager.StartedTasksCount > 1 || context.State == OperationState.Started;

            updateButtons(startLoginOperation);
        }


        void fillTreeList()
        {
            var plans = WorkoutPlansReposidory.Instance.Items.Values.ToList();

            Dispatcher.BeginInvoke(new Action(delegate
                                                  {
                WorkoutPlansList1.Fill(plans,pageContext.SelectedItem);
            }));
        }

        internal void Fill(Guid planId)
        {
            DoSearch(planId);
        }


        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (UserContext.Current.LoginStatus != LoginStatus.Logged)
            {
                throw new Portable.Exceptions.AuthenticationException("You must be logged");
            }

            DoSearch();
        }

        protected override void BeforeSearch(object param=null)
        {
            tcPlans.SelectedItem = tpSearch;
            base.BeforeSearch(param);
            if (param != null)
            {
                criteria.PlanId = (Guid) param;
            }
        }

        protected override void FillResults(ObservableCollection<TrainingPlan> result)
        {
            tcPlans.SelectedItem = tpSearch;
            searchPlansList.Fill(Items,criteria.PlanId);
        }

        public override void ClearContent()
        {
            
            WorkoutPlansList1.ClearContent();
            searchPlansList.ClearContent();
            base.ClearContent();
        }

        private void btnMoreResults_Click(object sender, RoutedEventArgs e)
        {
            MoreResults();
        }

        private void updateButtons(bool startedOperation)
        {
            EditGroupEnable = UserContext.Current.LoginStatus == LoginStatus.Logged && !startedOperation;
            if(startedOperation)
            {//we must leave because the rest lines (like IsMine()) retrieves workout plans list and some cases block UI thread
                return;
            }
            bool isNull = TrainingPlan == null;
            bool isMine = !isNull && TrainingPlan.IsMine();
            bool isPublished = !isNull && TrainingPlan.Status == PublishStatus.Published;

            CanClone = CanView = !isNull;
            CanEdit = CanDelete = isMine && !isPublished;
            CanPublish = !isPublished && isMine;
            CanNew = true;
            
            CanAddToFavorites = !isNull && !isMine && !TrainingPlan.IsFavorite();
            CanRemoveFromFavorites = !isNull && !isMine && TrainingPlan.IsFavorite();
        }

        private void WorkoutPlansList1_SelectedPlanChanged(object sender, EventArgs e)
        {
            updateButtons(false);
            pageContext.SelectedItem = TrainingPlan!=null?TrainingPlan.GlobalId:(Guid?) null;
            //if (ShowComments)
            //{
                
            //    showComments();
            //}
            if (commentsControl != null)
            {
                showComments();
            }
        }

        private void showComments()
        {
            bool cannotVote = TrainingPlan == null || TrainingPlan.Profile.GlobalId == UserContext.Current.CurrentProfile.GlobalId;
            commentsControl.CannotVote = cannotVote;
            commentsControl.Fill(TrainingPlan);
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

        private TrainingPlan TrainingPlan
        {
            get
            {
                if (tcPlans.SelectedItem == tpMyPlans)
                {
                    return WorkoutPlansList1.SelectedPlan;
                }
                else
                {
                    return searchPlansList.SelectedPlan;
                }
            }
        }

        private WorkoutPlanViewModel TrainingPlanViewModel
        {
            get
            {
                if (tcPlans.SelectedItem == tpMyPlans)
                {
                    return WorkoutPlansList1.SelectedPlanViewModel;
                }
                else
                {
                    return searchPlansList.SelectedPlanViewModel;
                }
            }
        }

        private void rbtnView_Click(object sender, RoutedEventArgs e)
        {

            //MainWindow.Instance.ShowView(delegate
            //{
            //    var ctrl = new HtmlPreviewView();
            //    ctrl.CurrentHtmlProvider = new TrainingPlanHtmlExporter(TrainingPlan.ToTrainingPlan());
            //    return ctrl;
            //});
            MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.UI;component/Views/HtmlPreviewView.xaml"), () =>
            {
                return new HtmlPreviewPageContext(new TrainingPlanHtmlExporter(TrainingPlan));
            });
        }

        private void rbtnClone_Click(object sender, RoutedEventArgs e)
        {
            var plan = TrainingPlan;
            if (plan == null)
            {
                return;
            }
            ParentWindow.RunAsynchronousOperation(delegate
            {
                //TODO:Check Id's for entries sets and days
                var workoutPlan = plan;
                var copyPlan = workoutPlan.StandardClone();
                CloneCleaner.Clean(copyPlan);
                copyPlan.BasedOnId = plan.GlobalId;
                copyPlan.GlobalId = Guid.NewGuid();
                copyPlan.Name = StrengthTrainingEntryStrings.TrainingPlanNewName;


                Dispatcher.BeginInvoke(new Action(() => editTrainingPlan(copyPlan)));
                
            }, asyncOperationStateChange);
        }

        private void rbtnNew_Click(object sender, RoutedEventArgs e)
        {
            var plan = new TrainingPlan();
            plan.Language = "en";
            plan.Profile = UserContext.Current.CurrentProfile;
            editTrainingPlan(plan);
        }

        private void rbtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (TrainingPlan == null)
            {
                return;
            }
            UIHelper.RunWithExceptionHandling(delegate
            {
                var plan = TrainingPlan;
                plan.Tag = TrainingPlan;
                plan.Profile = UserContext.Current.CurrentProfile;
                editTrainingPlan(plan.StandardClone());
            });
        }

        private void rbtnDelete_Click(object sender, RoutedEventArgs e)
        {
            deleteSelectedTrainingPlan();
        }

        

        private void deleteSelectedTrainingPlan()
        {
            var plan = TrainingPlan;
            if (plan == null || plan.Status == PublishStatus.Published || plan.Profile.GlobalId != UserContext.Current.CurrentProfile.GlobalId)
            {
                return;
            }
            if (!UIHelper.EnsurePremiumLicence())
            {
                return;
            }
            
            if (BAMessageBox.AskYesNo(StrengthTrainingEntryStrings.QRemoveTrainingPlan) == MessageBoxResult.Yes)
            {
                ParentWindow.RunAsynchronousOperation(delegate
                                                          {
                                                              var param = new WorkoutPlanOperationParam();
                                                              param.WorkoutPlanId = plan.GlobalId;
                                                              param.Operation=SupplementsCycleDefinitionOperation.Delete;
                                                              ServiceManager.WorkoutPlanOperation(param);
                    WorkoutPlansReposidory.Instance.ClearCache();
                    Dispatcher.BeginInvoke(new Action<PageContext>(Fill),this.pageContext);
                }, asyncOperationStateChange);
                //try
                //{

                //}
                //catch (Exception ex)
                //{
                //    ExceptionHandler.Default.Process(ex, StrengthTrainingEntryStrings.ErrorDuringDeleteTrainingPlan, ErrorWindow.EMailReport);
                //}
            }
        }

        private void rbtnPublish_Click(object sender, RoutedEventArgs e)
        {
            if (TrainingPlan == null || !UIHelper.EnsurePremiumLicence())
            {
                return;
            }
            
            PublishWorkoutPlanWindow dlg = new PublishWorkoutPlanWindow();
            dlg.Fill(TrainingPlan, true);
            if (dlg.ShowDialog() == true)
            {
                WorkoutPlansReposidory.Instance.ClearCache();
                Fill(pageContext);
            }
        }

        private void rbtnCanAddToFavorites_Click(object sender, RoutedEventArgs e)
        {
            if (!UIHelper.EnsurePremiumLicence())
            {
                return;
            }
            UIHelper.RunWithExceptionHandling(delegate
            {
                if (TrainingPlan.AddToFavorites())
                {
                    Fill(pageContext);
                }
            });
        }

        private void rbtnRemoveFromFavorites_Click(object sender, RoutedEventArgs e)
        {
            if (!UIHelper.EnsurePremiumLicence())
            {
                return;
            }
            UIHelper.RunWithExceptionHandling(delegate
            {
                if (TrainingPlan.RemoveFromFavorites())
                {
                    Fill(pageContext);
                }
            });
        }

        //private void commentsSplitter_Collapsed(object sender, EventArgs e)
        //{
        //    ShowComments = !commentsSplitter.IsCollapsed;
        //}

        void editTrainingPlan(TrainingPlan plan)
        {
            if (!UIHelper.EnsurePremiumLicence())
            {
                return;
            }

            MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.Module.StrengthTraining;component/Controls/TrainingPlans/TrainingPlanEditorWindow.xaml"), () =>
                {
                    return new ItemContext<TrainingPlan>(plan);
                });
        }

        private void rbtnViewReport_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.Module.StrengthTraining;component/Controls/TrainingPlanReportView.xaml"), () =>
            {
                return new ItemContext<TrainingPlan>(TrainingPlan);
            });
        }

        private void usrWorkoutCommentsList1_Voted(object sender, RoutedEventArgs e)
        {
            var voted = (VoteEventArgs)e;

            TrainingPlanViewModel.Refresh((TrainingPlan)voted.Ratingable);
        }

        private void xtraTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //BUG FIX: this if is because wihtout it when we select any comment then this event (tabcontrol selection changed) is invoked)
            if (e.AddedItems.Count == 1 && e.AddedItems[0] is TabItem)
            {
                if (pageContext != null)
                {
                    pageContext.DisplayMode = tcPlans.SelectedIndex;
                }
            }
        }

        private void tbsShowComments_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.EnsureVisible("RatingsCommentsControl");
        }
    }
}
