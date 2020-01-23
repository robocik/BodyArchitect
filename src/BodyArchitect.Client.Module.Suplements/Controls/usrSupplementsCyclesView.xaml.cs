using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using AvalonDock.Layout;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.Suplements.PreviewGenerator;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.UserControls;
using BodyArchitect.Client.UI.Views;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;

namespace BodyArchitect.Client.Module.Suplements.Controls
{
    /// <summary>
    /// Interaction logic for usrSupplementsCyclesView.xaml
    /// </summary>
    public partial class usrSupplementsCyclesView : IWeakEventListener, IHasFloatingPane
    {
        private usrWorkoutCommentsList commentsControl;

        public usrSupplementsCyclesView()
        {
            InitializeComponent();
            DataContext = this;

            MainWindow.Instance.EnsureAnchorable(Strings.usrWorkoutCommentsList_Header_Comments, "pack://application:,,,/BodyArchitect.Client.Resources;component/Images/Comments16.png", "RatingsCommentsControl", AnchorableShowStrategy.Right);

            CollectionChangedEventManager.AddListener(SupplementsCycleDefinitionsReposidory.Instance, this);
        }



        public Control GetContentForPane(string paneId)
        {
            if(paneId=="RatingsCommentsControl")
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


        public bool IsInProgress
        {
            get { return isInProgress; }
            set
            {
                isInProgress = value;
                NotifyOfPropertyChange(() => IsInProgress);
            }
        }

        #region Ribbon

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

        private bool _canStartCycle;
        public bool CanStartCycle
        {
            get { return _canStartCycle; }
            set
            {
                _canStartCycle = value;
                NotifyOfPropertyChange(() => CanStartCycle);
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

        #endregion

        private bool isInProgress;
        private PageContext pageContext;


        private void updateButtons(bool startedOperation)
        {
            if (startedOperation)
            {//we must leave because the rest lines (like IsMine()) retrieves workout plans list and some cases block UI thread
                return;
            }
            bool isNull = SelectedDefinition == null;
            bool isMine = !isNull && SelectedDefinition.IsMine();

            CanStartCycle=CanClone = CanView = !isNull;
            CanDelete = CanEdit = isMine && SelectedDefinition.Status == PublishStatus.Private; 
            CanAddToFavorites = !isNull && SelectedDefinition.CanAddToFavorites();
            CanRemoveFromFavorites = !isNull && SelectedDefinition.CanRemoveFromFavorites();
            CanPublish = isMine && SelectedDefinition.Status == PublishStatus.Private;
            CanNew = true;
        }

        public SupplementCycleDefinitionDTO SelectedDefinition
        {
            get
            {
                if (tcPlans.SelectedItem == tpMyCycles)
                {
                    return supplementsCyclesList1.SelectedCycleDefinition;
                }
                else
                {
                    return searchPlansList.SelectedCycleDefinition;
                }
            }
        }

        public SupplementCycleViewModel SelectedDefinitionViewModel
        {
            get
            {
                if (tcPlans.SelectedItem == tpMyCycles)
                {
                    return supplementsCyclesList1.SelectedCycleViewModel;
                }
                else
                {
                    return searchPlansList.SelectedCycleViewModel;
                }
            }
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            Fill(pageContext);

            return true;
        }

        public void Fill(PageContext pageContext)
        {
            IsInProgress = true;
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action<PageContext>(Fill),pageContext);
            }
            else
            {
                this.pageContext = pageContext;
                tcPlans.SelectedIndex = pageContext.DisplayMode;
                SearchEnabled = UserContext.Current.LoginStatus == LoginStatus.Logged;
                SearchStatus = string.Empty;
                ParentWindow.RunAsynchronousOperation(delegate(OperationContext ctx)
                {
                    SupplementsCycleDefinitionsReposidory.Instance.EnsureLoaded();

                    Dispatcher.BeginInvoke(new Action(() =>supplementsCyclesList1.Fill(SupplementsCycleDefinitionsReposidory.Instance.Items.Values,pageContext.SelectedItem)));
                    IsInProgress = false;
                }, asyncOperationStateChange);
            }
        }

        void asyncOperationStateChange(OperationContext context)
        {
            bool startLoginOperation = context.TaskManager.StartedTasksCount > 1 || context.State == OperationState.Started;

            updateButtons(startLoginOperation);
        }

        public void RefreshView()
        {
            SupplementsCycleDefinitionsReposidory.Instance.ClearCache();
            Fill(pageContext);
        }


        private void rbtnView_Click(object sender, RoutedEventArgs e)
        {
            if (!UIHelper.EnsurePremiumLicence())
            {
                return;
            }
            MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.UI;component/Views/HtmlPreviewView.xaml"),()=>
                {
                    return new HtmlPreviewPageContext(new SupplementsCycleDefinitionHtmlExporter(SelectedDefinition));
                });
        }

        private void rbtnClone_Click(object sender, RoutedEventArgs e)
        {
            if (!UIHelper.EnsurePremiumLicence())
            {
                return;
            }

            var copyPlan = SelectedDefinition.StandardClone();
            CloneCleaner.Clean(copyPlan);
            copyPlan.BasedOnId = SelectedDefinition.GlobalId;
            copyPlan.Name = "SupplementsDefinitionNewName".TranslateSupple();

            MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.Module.Suplements;component/Controls/SupplementsCycleDefinitionEditorView.xaml"), () =>
            {
                return new SupplementsCycleDefinitionContext(copyPlan);
            });
            
        }

        private void rbtnNew_Click(object sender, RoutedEventArgs e)
        {
            if(!UIHelper.EnsurePremiumLicence())
            {
                return;
            }
            MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.Module.Suplements;component/Controls/SupplementsCycleDefinitionEditorView.xaml"), () =>
            {
                return new SupplementsCycleDefinitionContext(new SupplementCycleDefinitionDTO());
            });
        }

        private void rbtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!UIHelper.EnsurePremiumLicence())
            {
                return;
            }
            MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.Module.Suplements;component/Controls/SupplementsCycleDefinitionEditorView.xaml"), () =>
            {
                return new SupplementsCycleDefinitionContext(SelectedDefinition);
            });
        }

        private void rbtnDelete_Click(object sender, RoutedEventArgs e)
        {
            deleteSelectedSupplementDefinition();
        }

        private void deleteSelectedSupplementDefinition()
        {
            var plan = SelectedDefinition;
            if (plan == null || plan.Profile.GlobalId != UserContext.Current.CurrentProfile.GlobalId || !UIHelper.EnsurePremiumLicence())
            {
                return;
            }
            if (BAMessageBox.AskYesNo(SuplementsEntryStrings.Question_usrSupplementsCyclesView_DoYouWantToDeleteCycle) == MessageBoxResult.Yes)
            {
                ParentWindow.RunAsynchronousOperation(delegate
                                                          {
                    var param = new SupplementsCycleDefinitionOperationParam();
                    param.SupplementsCycleDefinitionId = plan.GlobalId;
                    param.Operation =SupplementsCycleDefinitionOperation.Delete;
                    ServiceManager.SupplementsCycleDefinitionOperation(param);
                    SupplementsCycleDefinitionsReposidory.Instance.Remove(plan.GlobalId);
                    //WorkoutPlansReposidory.Instance.ClearCache();
                    //Dispatcher.Invoke(new Action(delegate
                    //{
                    //    Fill();

                    //}));
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

        private void btnMoreResults_Click(object sender, RoutedEventArgs e)
        {
            MoreResults();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            DoSearch();
        }

        internal void Fill(Guid planId)
        {
            DoSearch(planId);
        }

        protected override void BeforeSearch(object param = null)
        {
            tcPlans.SelectedItem = tpSearch;
            base.BeforeSearch(param);
            if (param != null)
            {
                criteria.PlanId = (Guid)param;
            }
        }

        private void SupplementsCyclesList_SelectedPlanChanged(object sender, EventArgs e)
        {
            updateButtons(false);
            pageContext.SelectedItem = SelectedDefinition != null ? SelectedDefinition.GlobalId : (Guid?)null;
            //if (ShowComments)
            //{
            //    showCommentsPanel();
            //}
            showComments();
        }

        private void showComments()
        {
            if (commentsControl != null)
            {
                bool cannotVote = SelectedDefinition == null ||
                                  SelectedDefinition.Profile.GlobalId == UserContext.Current.CurrentProfile.GlobalId;
                commentsControl.CannotVote = cannotVote;
                commentsControl.Fill(SelectedDefinition);
            }
        }


        protected override void FillResults(ObservableCollection<SupplementCycleDefinitionDTO> result)
        {
            searchPlansList.Fill(Items, criteria.PlanId);
        }

        private void rbtnStartCycle_Click(object sender, RoutedEventArgs e)
        {
            if (!UIHelper.EnsurePremiumLicence())
            {
                return;
            }
            StartSupplementsCycleWindow dlg = new StartSupplementsCycleWindow();
            dlg.SelectedCycleDefinition = SelectedDefinition;
            if (dlg.ShowDialog() == true)
            {
                BAMessageBox.ShowInfo("Info_usrSupplementsCyclesView_rbtnStartCycle_Click_StartedCycle".TranslateSupple());
                
            }
        }

        private void rbtnAddToFavorites_Click(object sender, RoutedEventArgs e)
        {
            if (!UIHelper.EnsurePremiumLicence())
            {
                return;
            }
            SelectedDefinition.AddToFavorites();
        }

        private void rbtRemoveFromFavorites_Click(object sender, RoutedEventArgs e)
        {
            if (!UIHelper.EnsurePremiumLicence())
            {
                return;
            }
            SelectedDefinition.RemoveFromFavorites();
        }

        private void rbtPublish_Click(object sender, RoutedEventArgs e)
        {
            if (!UIHelper.EnsurePremiumLicence())
            {
                return;
            }
            var definition = SelectedDefinition;
            //todo:change to better window
            PleaseWait.Run(delegate
            {
                
                try
                {
                    var param = new SupplementsCycleDefinitionOperationParam();
                    param.SupplementsCycleDefinitionId = definition.GlobalId;
                    param.Operation = SupplementsCycleDefinitionOperation.Publish;
                    var result = ServiceManager.SupplementsCycleDefinitionOperation(param);
                    SupplementsCycleDefinitionsReposidory.Instance.Update(result);
                }
                catch (ProfileRankException ex)
                {
                    UIHelper.Invoke(() => ExceptionHandler.Default.Process(ex, string.Format("SupplementsCycleDefinitions_Publish_ErrProfileRank".TranslateSupple(), Portable.Constants.StrengthTrainingEntriesCount), ErrorWindow.MessageBox), Dispatcher);
                }
                catch (Exception ex)
                {
                    Dispatcher.BeginInvoke(new Action(() => ExceptionHandler.Default.Process(ex, "ErrCannotPublishSuppleDefinition".TranslateSupple(), ErrorWindow.EMailReport)));
                }

            });
        }

        private void usrWorkoutCommentsList1_Voted(object sender, RoutedEventArgs e)
        {
            var voted=(VoteEventArgs) e;
            
            SelectedDefinitionViewModel.Refresh((SupplementCycleDefinitionDTO) voted.Ratingable);
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
