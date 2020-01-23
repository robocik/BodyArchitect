using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Controls;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Logger;
using BodyArchitect.Controls.External;
using BodyArchitect.Module.StrengthTraining.Localization;
using BodyArchitect.Service.Model;
using ControlHelper=BodyArchitect.Controls.ControlHelper;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    public partial class ExercisesView : BaseWindow
    {

        public ExercisesView()
        {
            InitializeComponent();
            fillSuperTips();
            
        }


        void fillSuperTips()
        {
            ControlHelper.AddSuperTip(btnAddExercise, btnAddExercise.Text, StrengthTrainingEntryStrings.ExerciseView_AddExerciseBTN);
            ControlHelper.AddSuperTip(btnDeleteExercise, btnDeleteExercise.Text, StrengthTrainingEntryStrings.ExerciseView_DeleteExerciseBTN);
            //ControlHelper.AddSuperTip(defaultToolTipController1, treeListView1, null, StrengthTrainingEntryStrings.ExerciseView_ExerciseList);
        }

        private  void AsyncOperationStateChange(OperationContext context)
        {
            bool startOperation = context.State == OperationState.Started;
            if(startOperation)
            {
                progressIndicator1.Start();
            }
            else
            {
                progressIndicator1.Stop();
            }
            progressIndicator1.Visible = startOperation;
            updateButtons(startOperation);
        }

        
        public void Fill()
        {
            exercisesTree1.ClearContent();
            RunAsynchronousOperation(delegate
                 {
      
                     ObjectsReposidory.EnsureExercisesLoaded();
                     SynchronizationContext.Send(delegate
                     {
                         
                         exercisesTree1.Fill(ObjectsReposidory.Exercises.Values);
                     },null);

                 }, AsyncOperationStateChange);
            
        }


        private void btnPublish_Click(object sender, EventArgs e)
        {
            if (FMMessageBox.AskYesNo(ApplicationStrings.QAskForPublicationExercise) == DialogResult.Yes)
            {
                var exercise = exercisesTree1.SelectedExercise;
                RunAsynchronousOperation(delegate
                {
                    try
                    {
                        exercise = ServiceManager.PublishExercise(exercise);
                        ObjectsReposidory.UpdateExercise(exercise);


                        SynchronizationContext.Send(delegate
                        {
                            Fill();
                        }, null);
                    }
                    catch (ArgumentException ex)
                    {
                        TasksManager.SetException(ex);
                        this.SynchronizationContext.Send(delegate
                                                             {
                                                                 ExceptionHandler.Default.Process(ex,StrengthTrainingResources.ErrorPublishExercise_EmptyUrl,ErrorWindow.MessageBox);

                                                             }, null);
                    }


                }, AsyncOperationStateChange);
            }
        }

        void updateButtons(bool startOperation)
        {
            exercisesTree1.Enabled = !startOperation;
            btnDeleteExercise.Enabled =!startOperation && exercisesTree1.CanDelete;
            btnAddExercise.Text = exercisesTree1.SelectedExercise == null ? ApplicationStrings.BTNNewExercise : ApplicationStrings.BTNEditExercise;
            btnAddExercise.Enabled = !startOperation && (exercisesTree1.SelectedExercise!=null || exercisesTree1.SelectedExerciseType.HasValue);
            btnPublish.Enabled = !startOperation && exercisesTree1.SelectedExercise != null && exercisesTree1.SelectedExercise.Status == PublishStatus.Private;

        }
        private void exercisesTree1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            updateButtons(false);
        }

        private void btnDeleteExercise_Click(object sender, EventArgs e)
        {
            var selectedExercise = exercisesTree1.SelectedExercise;
            if (selectedExercise != null)
            {
                exercisesTree1.DeleteSelectedExercises();
            }
        }

        private void exercisesTree1_FillReqest(object sender, EventArgs e)
        {
            Fill();
        }

        private void btnAddExercise_Click(object sender, EventArgs e)
        {
            exercisesTree1.EditSelectedExercise();
        }

        private void ExercisesView_Load(object sender, EventArgs e)
        {
            Fill();
        }

        private void btnMapper_Click(object sender, EventArgs e)
        {
            ExerciseMapperWindow dlg = new ExerciseMapperWindow();
            dlg.Fill();
            if(dlg.ShowDialog(this)==System.Windows.Forms.DialogResult.OK)
            {
                ObjectsReposidory.ClearExerciseCache();
                Fill();
            }
        }

    }
}
