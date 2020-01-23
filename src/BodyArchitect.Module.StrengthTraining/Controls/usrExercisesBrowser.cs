using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Common.Plugins;
using BodyArchitect.Controls;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Service.Model;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    public partial class usrExercisesBrowser : usrBaseControl,IMainTabControl
    {
        private ExerciseSearchCriteria criteria;
        private PartialRetrievingInfo pagerInfo;
        private CancellationTokenSource currentTask;
        private PagedResult<ExerciseDTO> result;

        public usrExercisesBrowser()
        {
            InitializeComponent();

            foreach (ExerciseType type in Enum.GetValues(typeof(ExerciseType)))
            {
                CheckedListBoxItem item = new CheckedListBoxItem(type, EnumLocalizer.Default.Translate(type));
                cmbTypes.Properties.Items.Add(item);
            }

            foreach (WorkoutPlanSearchOrder type in Enum.GetValues(typeof(WorkoutPlanSearchOrder)))
            {
                BodyArchitect.Controls.ComboBoxItem item = new BodyArchitect.Controls.ComboBoxItem(type, EnumLocalizer.Default.Translate(type));
                cmbSortOrder.Properties.Items.Add(item);
            }
            cmbSortOrder.SelectedIndex = 0;
        }

        public void ClearContent()
        {
            listView1.Items.Clear();
            usrExerciseEditor1.Visible = false;
            if (currentTask != null && !currentTask.IsCancellationRequested)
            {
                currentTask.Cancel();
                currentTask = null;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            doSearch();
        }

        private void doSearch()
        {
            ClearContent();
            pagerInfo = new PartialRetrievingInfo();

            criteria = new ExerciseSearchCriteria();
            //criteria = ExerciseSearchCriteria.CreateAllCriteria();
            criteria.SearchGroups.Add(ExerciseSearchCriteriaGroup.PendingPublish);

            foreach (CheckedListBoxItem value in cmbTypes.Properties.Items)
            {
                if (value.CheckState == CheckState.Checked)
                {
                    criteria.ExerciseTypes.Add((ExerciseType) value.Value);
                }
            }

            criteria.SortOrder = (WorkoutPlanSearchOrder) cmbSortOrder.SelectedIndex;

            fillPage(0);
        }

        private void fillPage(int pageIndex)
        {
            currentTask = ParentWindow.TasksManager.RunAsynchronousOperation(delegate(OperationContext ctx)
            {
                if (ctx.CancellatioToken == null || ctx.CancellatioToken.IsCancellationRequested || pagerInfo == null)
                {
                    return;
                }
                pagerInfo.PageIndex = pageIndex;
                result = ServiceManager.GetExercises(criteria, pagerInfo);
                ctx.CancellatioToken.ThrowIfCancellationRequested();
                ParentWindow.SynchronizationContext.Send(delegate
                {
                    foreach (var exerciseDto in result.Items)
                    {
                        string profileName = exerciseDto.Profile != null ? exerciseDto.Profile.UserName : string.Empty;
                        ListViewItem item = new ListViewItem(new string[] { exerciseDto.Name, 
                                                                EnumLocalizer.Default.Translate(exerciseDto.ExerciseType),
                                                                exerciseDto.Rating.ToString("F2"),profileName
                                                                });
                        item.Tag = exerciseDto;
                        listView1.Items.Add(item);
                    }
                    lblStatus.Text = string.Format(ApplicationStrings.PartialLoadedStatus, listView1.Items.Count, result.AllItemsCount);
                    lblStatus.Visible = true;
                }, null);

            }, asyncOperationStateChange, null, false);
        }

        void asyncOperationStateChange(OperationContext context)
        {
            bool startLoginOperation = context.State == OperationState.Started;

            changeOperationState(startLoginOperation);
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
            btnSearch.Enabled = UserContext.LoginStatus == LoginStatus.Logged && !startLoginOperation;
            btnMoreResults.Enabled = UserContext.LoginStatus == LoginStatus.Logged && !startLoginOperation && (result != null && listView1.Items.Count < result.AllItemsCount);
            progressIndicator1.Visible = startLoginOperation;
        }

        private void btnMoreResults_Click(object sender, EventArgs e)
        {
            fillPage(pagerInfo.PageIndex + 1);
        }

        public ExerciseDTO SelectedExercise
        {
            get
            {
                if(listView1.SelectedIndices.Count>0)
                {
                    return (ExerciseDTO)listView1.SelectedItems[0].Tag;
                }
                return null;
            }
        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(SelectedExercise!=null)
            {
                usrExerciseEditor1.Fill(SelectedExercise, null,true);
                usrExerciseEditor1.Visible = true;
            }
            else
            {
                usrExerciseEditor1.Visible = false;
            }
        }

        public void InvokeSearch()
        {
            doSearch();
        }

        public void Fill()
        {
            //doSearch();
        }

        public void RefreshView()
        {
            
        }

        private void mnuShowUserDetails_Click(object sender, EventArgs e)
        {
            MainWindow.Instance.ShowUserInformation(SelectedExercise.Profile);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = SelectedExercise == null || SelectedExercise.Profile.IsMe();
        }
    }
}
