using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Common.ProgressOperation;
using BodyArchitect.Controls;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Logger;
using BodyArchitect.Service.Model;
using DevExpress.XtraEditors;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    public partial class usrExercisesView : usrBaseControl
    {
        private ExerciseSearchCriteria searchCriteria;
        private CancellationTokenSource currentTask;
        private int currentPage = 0;
        PagedResult<ExerciseDTO> exercisesListPack;

        public event EventHandler SelectedExerciseChanged;

        public usrExercisesView()
        {
            InitializeComponent();
        }

        public void Fill(ExerciseSearchCriteria searchCriteria)
        {
            this.searchCriteria = searchCriteria;
            this.exercisesTree1.ClearContent();
            changeOperationState(false);
            currentPage = 0;
            exercisesListPack = null;
            if(searchCriteria!=null)
            {
                fillExercisesPage(0);
            }
        }

        

        void asyncOperationStateChange(OperationContext context)
        {
            bool startLoginOperation = context.State == OperationState.Started;

            changeOperationState(startLoginOperation);
        }

        protected override void LoginStatusChanged(LoginStatus newStatus)
        {
            Fill(null);
        }

        void changeOperationState(bool startLoginOperation)
        {
            if (startLoginOperation)
            {
                progressIndicator1.Start();
            }
            else
            {
                progressIndicator1.Stop();
            }

            btnMore.Enabled = !startLoginOperation && (exercisesListPack != null && exercisesTree1.GetExercisesCount() < exercisesListPack.AllItemsCount);
            progressIndicator1.Visible = startLoginOperation;
        }

        

        public List<ExerciseDTO> SelectedExercises
        {
            get { return exercisesTree1.SelectedExercises; }
        }

        public ExerciseDTO SelectedExercise
        {
            get { return exercisesTree1.SelectedExercise; }
        }

        public ExerciseType? SelectedExerciseType
        {
            get { return exercisesTree1.SelectedExerciseType; }
        }
        private void fillExercisesPage(int pageIndex)
        {
            currentTask = ParentWindow.TasksManager.RunAsynchronousOperation(delegate(OperationContext ctx)
            {

                PartialRetrievingInfo info = new PartialRetrievingInfo();
                info.PageIndex = pageIndex;
                exercisesListPack = ServiceManager.GetExercises(searchCriteria, info);
                ctx.CancellatioToken.ThrowIfCancellationRequested();
                ParentWindow.SynchronizationContext.Send(delegate
                {
                    exercisesTree1.Fill(exercisesListPack.Items);
                    //lblStatus.Text = string.Format("Loaded {0} from {1}", listViewEx1.Items.Count, commentsListPack.AllItemsCount);
                    //listViewEx1.Visible = listViewEx1.Items.Count > 0;
                    //tableLayoutPanel1.Visible = listViewEx1.Items.Count > 0;
                }, null);


            }, asyncOperationStateChange, null, false);
        }

        private void exercisesTree1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if(SelectedExerciseChanged!=null)
            {
                SelectedExerciseChanged(this, EventArgs.Empty);
            }
        }

        private void btnMore_Click(object sender, EventArgs e)
        {
            fillExercisesPage(++currentPage);
        }

        public void ClearContent()
        {
            exercisesTree1.ClearContent();
            changeOperationState(false);
        }

        private void exercisesTree1_FillReqest(object sender, EventArgs e)
        {
            Fill(searchCriteria);
        }
    }
}
