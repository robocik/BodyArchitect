using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Controls;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Controls.ProgressIndicator;
using BodyArchitect.Logger;
using BodyArchitect.Module.StrengthTraining.Localization;
using BodyArchitect.Service.Model;
using BodyArchitect.Service.Model.TrainingPlans;
using BodyArchitect.Shared;
using DevExpress.XtraEditors;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    public partial class PublishWorkoutPlanWindow : BaseWindow
    {
        private WorkoutPlanDTO plan;

        public bool Publish { get; private set; }

        public PublishWorkoutPlanWindow()
        {
            InitializeComponent();

            
            
        }

        public void Fill(WorkoutPlanDTO plan,bool publish)
        {
            this.plan = plan;
            Publish = publish;
            lblPublishDescription.Visible = publish;
            lblAddToFavoriteDescription.Visible = !publish;

            if (Publish)
            {
                usrProgressIndicatorButtons1.OkButton.Image = Icons.Publish;
                pictureBox1.Image = StrengthTrainingResources.BigPublish;
                usrProgressIndicatorButtons1.OkButton.Text = ApplicationStrings.PublishButton;
            }
            else
            {
                Text = StrengthTrainingEntryStrings.PublishWorkoutPlanWindow_AddToFavorites_Text;
                pictureBox1.Image = StrengthTrainingResources.BigAddFavorite;
                usrProgressIndicatorButtons1.OkButton.Image = Icons.Favorite;
                usrProgressIndicatorButtons1.OkButton.Text = ApplicationStrings.AddButton;
            }
        }

        private void usrProgressIndicatorButtons1_OkClick(object sender, CancellationSourceEventArgs e)
        {
            try
            {
                if (Publish)
                {
                    ServiceManager.PublishWorkoutPlan(plan);
                }
                else
                {
                    ServiceManager.WorkoutPlanFavoritesOperation(plan, FavoriteOperation.Add);
                }
                ThreadSafeClose(System.Windows.Forms.DialogResult.OK);
            }
            catch (PublishedObjectOperationException ex)
            {
                TasksManager.SetException(ex);
                this.SynchronizationContext.Send(delegate
                {
                    ExceptionHandler.Default.Process(ex, StrengthTrainingEntryStrings.ErrorExercisesPrivateInWorkout, ErrorWindow.MessageBox);
                }, null);

            }
           
        }

        private CancellationTokenSource fillExercisesToken;

        private void PublishWorkoutPlanWindow_Load(object sender, EventArgs e)
        {
            fillExercisesToken=RunAsynchronousOperation(delegate(OperationContext ctx)
                                                            {
                         var trainingPlan = plan.ToTrainingPlan();
                         ctx.CancellatioToken.ThrowIfCancellationRequested();
                         ctx.CancellatioToken.ThrowIfCancellationRequested();
                         SynchronizationContext.Send(delegate
                                {
                                    exercisesListView1.Fill(trainingPlan);
                                },null);
                         fillExercisesToken = null;
                         
                     }
                     ,delegate(OperationContext context)
                           {
                               bool start = context.State == OperationState.Started;
                               usrProgressIndicatorButtons1.UpdateProgressIndicator(context);
                               
                               usrProgressIndicatorButtons1.OkButton.Enabled = !Publish ||
                                                                               exercisesListView1.Groups["private"].Items.Count == 0;
                               if(start)
                               {
                                   baGroupControl1.Text = StrengthTrainingEntryStrings.PublishWorkoutPlanWindow_ExercisesLoading;
                               }
                               else
                               {
                                   baGroupControl1.Text = StrengthTrainingEntryStrings.PublishWorkoutPlanWindow_ExercisesText;
                               }
                           });
            
            
        }


        private void usrProgressIndicatorButtons1_CancelClick(object sender, EventArgs e)
        {
            if(fillExercisesToken!=null)
            {
                fillExercisesToken.Cancel();
            }
        }
    }
}