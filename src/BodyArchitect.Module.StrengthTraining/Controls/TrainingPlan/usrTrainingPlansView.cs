
using System;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Common;
using BodyArchitect.Common.Plugins;
using BodyArchitect.Common.ProgressOperation;
using BodyArchitect.Controls;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Logger;
using BodyArchitect.Module.StrengthTraining.Localization;
using BodyArchitect.Service.Model;
using BodyArchitect.Service.Model.TrainingPlans;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using ControlHelper = BodyArchitect.Controls.ControlHelper;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    public partial class usrTrainingPlansView : usrBaseControl, IMainTabControl
    {
        public usrTrainingPlansView()
        {
            InitializeComponent();
            defaultToolTipController1.DefaultController.Active = UserContext.Settings.GuiState.ShowToolTips;

            updateButtons(false);
            tbShowComments.Checked = true;
        }

        protected override void LoginStatusChanged(LoginStatus newStatus)
        {
            workoutPlansListView1.Items.Clear();
            updateButtons(false);
        }

        public void Fill()
        {
            if(UserContext.LoginStatus!=LoginStatus.Logged)
            {
                return;
            }

            ParentWindow.TasksManager.RunAsynchronousOperation(delegate(OperationContext ctx)
            {
                ObjectsReposidory.EnsurePlansLoaded();
                ObjectsReposidory.EnsureExercisesLoaded();
                fillTreeList();

            }, asyncOperationStateChange);
        }

        public void RefreshView()
        {
            ObjectsReposidory.ClearWorkoutPlansCache();
            Fill();
        }


        void asyncOperationStateChange(OperationContext context)
        {
            //bool startLoginOperation = context.State == OperationState.Started;
            bool startLoginOperation = context.TaskManager.StartedTasksCount > 1 || context.State == OperationState.Started;

            updateButtons(startLoginOperation);
        }

        void fillTreeList()
        {
            var plans = ObjectsReposidory.WorkoutPlans;
            if (ParentWindow==null)
            {
                return;
            }

            ParentWindow.SynchronizationContext.Send(delegate
                                                         {
                                                             workoutPlansListView1.Items.Clear();
                                                             workoutPlansListView1.Fill(plans);
                                                         },null);
        }


        private WorkoutPlanDTO TrainingPlan
        {
            get
            {
                if (tcPlans.SelectedTabPage==tpPersnalPlans)
                {
                    return workoutPlansListView1.SelectedPlan;
                }
                else
                {
                    return usrWorkoutPlansSearch1.SelectedPlan;
                }
            }
        }

        private void updateButtons(bool startedOperation)
        {
            bool isNull = TrainingPlan == null;
            bool isMine = !isNull && TrainingPlan.IsMine();
            bool isPublished = !isNull && TrainingPlan.Status == PublishStatus.Published;

            tbClone.Enabled =  tbView.Enabled = !isNull;
            tbEdit.Enabled = tbDelete.Enabled = isMine && !isPublished;
            tbPublish.Enabled = !isPublished && isMine;
            splitContainerControl1.Collapsed = isNull;
            toolbar.Enabled = UserContext.LoginStatus== LoginStatus.Logged &&  !startedOperation;
            tbAddToFavorites.Visible = !isNull && !isMine && !TrainingPlan.IsFavorite();
            tbRemoveFromFavorites.Visible = !isNull && !isMine && TrainingPlan.IsFavorite();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            deleteSelectedTrainingPlan();
        }

        private void deleteSelectedTrainingPlan()
        {
            var plan = TrainingPlan;
            if (plan == null || plan.Status == PublishStatus.Published || plan.Profile.Id != UserContext.CurrentProfile.Id)
            {
                return;
            }
            if (FMMessageBox.AskYesNo(StrengthTrainingEntryStrings.QRemoveTrainingPlan) == System.Windows.Forms.DialogResult.Yes)
            {
                ParentWindow.RunAsynchronousOperation(delegate
                                                          {
                                                              ServiceManager.DeleteWorkoutPlan( plan);
                                                              ObjectsReposidory.ClearWorkoutPlansCache();
                                                              ParentWindow.SynchronizationContext.Send(delegate
                                                              {
                                                                  Fill();
                                                              
                                                              }, null);
                                                          },asyncOperationStateChange);
                //try
                //{
                    
                //}
                //catch (Exception ex)
                //{
                //    ExceptionHandler.Default.Process(ex, StrengthTrainingEntryStrings.ErrorDuringDeleteTrainingPlan, ErrorWindow.EMailReport);
                //}
            }
        }


        private void viewTrainingPlan()
        {
            ControlHelper.RunWithExceptionHandling(delegate
                                                       {
                                                           TrainingPlan.Open();
                                                       });
            
        }


        private void btnPrintPreview_Click(object sender, EventArgs e)
        {
            viewTrainingPlan();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (TrainingPlan == null)
            {
                return;
            }
            ControlHelper.RunWithExceptionHandling(delegate
                                                       {
                                                           var plan = TrainingPlan.ToTrainingPlan();
                                                           plan.Tag = TrainingPlan;
                                                           editTrainingPlan(plan);
                                                       });
            
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            var plan = new BodyArchitect.Service.Model.TrainingPlans.TrainingPlan();
            plan.Tag = new WorkoutPlanDTO();
            editTrainingPlan(plan);
        }

        void editTrainingPlan(BodyArchitect.Service.Model.TrainingPlans.TrainingPlan plan)
        {
            ControlHelper.RunWithExceptionHandling(delegate
             {
                 WorkoutPlanDTO planDto = (WorkoutPlanDTO)plan.Tag;
                 TrainingPlanEditorWindow wnd = new TrainingPlanEditorWindow();
                 wnd.Fill(plan);
                 if (wnd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                 {
                     ParentWindow.RunAsynchronousOperation(delegate
                     {
                         TrainingPlanChecker checker = new TrainingPlanChecker();
                         checker.Process(wnd.TrainingPlan);
                         planDto.SetTrainingPlan(wnd.TrainingPlan);

                         XmlSerializationTrainingPlanFormatter formatter = new XmlSerializationTrainingPlanFormatter();
                         planDto.PlanContent = formatter.ToXml(wnd.TrainingPlan).ToString();

                         planDto.Profile = UserContext.CurrentProfile;
                         ServiceManager.SaveTrainingPlan(planDto);
                         ObjectsReposidory.ClearWorkoutPlansCache();
                         ParentWindow.SynchronizationContext.Send(delegate
                         {
                             Fill();
                         }, null);
                     });
                 }
             });
            
        }


        private void btnClone_Click(object sender, EventArgs e)
        {
            var plan = TrainingPlan;
            if(plan==null)
            {
                return;
            }
            ParentWindow.RunAsynchronousOperation(delegate
                                                      {
                                                          var workoutPlan = plan.ToTrainingPlan();
                                                          workoutPlan.BasedOnId = plan.GlobalId;
                                                          var copyPlan = workoutPlan.Clone();
                                                          copyPlan.BasedOnId = plan.GlobalId;
                                                          copyPlan.GlobalId = Guid.NewGuid();
                                                          copyPlan.Name =StrengthTrainingEntryStrings.TrainingPlanNewName;
                                                          WorkoutPlanDTO dto = new WorkoutPlanDTO();
                                                          copyPlan.Tag = dto;
                                                          dto.SetTrainingPlan(copyPlan);
                                                          

                                                          ParentWindow.SynchronizationContext.Send(delegate
                                                          {
                                                              editTrainingPlan(copyPlan);
                                                          }, null);
                                                      }, asyncOperationStateChange);
        }
        
        private void tbPublish_Click(object sender, EventArgs e)
        {
            if(TrainingPlan==null)
            {
                return;
            }
            PublishWorkoutPlanWindow dlg = new PublishWorkoutPlanWindow();
            dlg.Fill(TrainingPlan,true);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                ObjectsReposidory.ClearWorkoutPlansCache();
                Fill();
            }
        }

        private void tbShowComments_Click(object sender, EventArgs e)
        {
            tbShowComments.Checked = !tbShowComments.Checked;
            splitContainerControl1.PanelVisibility = tbShowComments.Checked ? SplitPanelVisibility.Both : SplitPanelVisibility.Panel1;
            if (tbShowComments.Checked)
            {
                showCommentsPanel();
            }
        }

        private void showCommentsPanel()
        {
            bool cannotVote = TrainingPlan==null || TrainingPlan.Profile.Id == UserContext.CurrentProfile.Id;
            usrWorkoutCommentsList1.CannotVote = cannotVote;
            usrWorkoutCommentsList1.Fill(TrainingPlan);
        }


        private void workoutPlansListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateButtons(false);
            
            if (tbShowComments.Checked)
            {
                showCommentsPanel();
            }
            
        }

        private void tcPlans_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
        {
            updateButtons(false);
        }


        private void workoutPlansListView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                deleteSelectedTrainingPlan();
            }
        }

        private void usrWorkoutPlansSearch1_DeletePlanRequest(object sender, WorkoutPlanEventArgs e)
        {
            deleteSelectedTrainingPlan();
        }

        private void tbAddToFavorites_Click(object sender, EventArgs e)
        {
            ControlHelper.RunWithExceptionHandling(delegate
            {
                if (TrainingPlan.AddToFavorites())
                {
                    Fill();
                }
            });
            
        }

        private void tbRemoveFromFavorites_Click(object sender, EventArgs e)
        {
            ControlHelper.RunWithExceptionHandling(delegate
            {
                if (TrainingPlan.RemoveFromFavorites())
                {
                    Fill();
                }
            });
            
        }
    }
}
