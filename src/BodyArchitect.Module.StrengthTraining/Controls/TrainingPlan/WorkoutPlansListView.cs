using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Controls;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Module.StrengthTraining.Localization;
using BodyArchitect.Service.Model;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    public class WorkoutPlansListView:ListView
    {
        private ColumnHeader colName;
        private ColumnHeader colType;
        private ColumnHeader colDifficult;
        private ImageList imageList1;
        private System.ComponentModel.IContainer components;
        private ColumnHeader colRating;
        private ColumnHeader colPublishedDate;
        private ColumnHeader colPurpose;
        private ColumnHeader colDays;
        private ColumnHeader colLanguage;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem mnuOpen;
        private ToolStripMenuItem mnuAddToFavorites;
        private ToolStripMenuItem mnuRemoveFromFavorites;
        private ColumnHeader colAuthor;
        private ToolStripMenuItem mnuShowUserDetails;

        private List<WorkoutPlanDTO> allPlans=new List<WorkoutPlanDTO>();

        public WorkoutPlansListView()
        {
            InitializeComponent();
            Groups.Add("Favorites", StrengthTrainingEntryStrings.WorkoutPlansListView_GroupFavorites);
            Groups.Add("Mine", StrengthTrainingEntryStrings.WorkoutPlansListView_GroupMine);
            Groups.Add("Others", StrengthTrainingEntryStrings.WorkoutPlansListView_GroupOthers);
        }

        public void Fill(IList<WorkoutPlanDTO> plans)
        {
            allPlans.AddRange(plans);
            fill(plans);
        }

        void fill(IList<WorkoutPlanDTO> plans)
        {
            foreach (WorkoutPlanDTO workoutPlanDto in plans)
            {
                AddWorkoutPlan(workoutPlanDto);
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public WorkoutPlanDTO SelectedPlan
        {
            get
            {
                if(this.SelectedIndices.Count>0)
                {
                    return (WorkoutPlanDTO)SelectedItems[0].Tag;
                }
                return null;
            }
        }

        protected void AddWorkoutPlan(WorkoutPlanDTO plan)
        {
            UserDTO author = plan.IsMine()? UserContext.CurrentProfile: plan.Profile;
            string publishedDate = plan.PublishDate.HasValue ? plan.PublishDate.Value.ToLocalTime().ToString() : "";
            ListViewItem item = new ListViewItem(new string[]{plan.Name,
                                                plan.TrainingType.ToString(),
                                                EnumLocalizer.Default.Translate(plan.Difficult),
                                                author.UserName,
                                                plan.Rating.ToString("F2"),
                                                publishedDate,
                                                EnumLocalizer.Default.Translate(plan.Purpose),
                                                plan.DaysCount.ToString(),
                                                Language.GetLanguage(plan.Language).DisplayName});
            item.ImageKey = getNodeBitmap(plan);
            item.StateImageIndex = getStateImageIndex(plan);
            item.Tag = plan;
            selectAGroup(item);
            Items.Add(item);
        }

        private void selectAGroup(ListViewItem item)
        {
            WorkoutPlanDTO plan = (WorkoutPlanDTO)item.Tag;
            if(ShowGroups)
            {
                if (plan.IsFavorite())
                {
                    item.Group = Groups["Favorites"];
                }
                else if(plan.IsMine())
                {
                    item.Group = Groups["Mine"];
                }
                else
                {
                    item.Group = Groups["Others"];
                }
            }
        }


        int getStateImageIndex(WorkoutPlanDTO plan)
        {
            if (plan.IsFavorite())
            {
                return 7;
            }
            else if(plan.Status == PublishStatus.Published)
            {
                return 5;
            }
            return 6;
        }

        string getNodeBitmap(WorkoutPlanDTO plan)
        {
            if(plan.IsMine())
            {
                return "Owner";
            }
            return "TrainingPlan";
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorkoutPlansListView));
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDifficult = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colAuthor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.colRating = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colPublishedDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colPurpose = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDays = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colLanguage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAddToFavorites = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRemoveFromFavorites = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuShowUserDetails = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // colName
            // 
            resources.ApplyResources(this.colName, "colName");
            // 
            // colType
            // 
            resources.ApplyResources(this.colType, "colType");
            // 
            // colDifficult
            // 
            resources.ApplyResources(this.colDifficult, "colDifficult");
            // 
            // colAuthor
            // 
            resources.ApplyResources(this.colAuthor, "colAuthor");
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "TrainingPlanSigned");
            this.imageList1.Images.SetKeyName(1, "TrainingPlan");
            this.imageList1.Images.SetKeyName(2, "Published");
            this.imageList1.Images.SetKeyName(3, "Favorite");
            this.imageList1.Images.SetKeyName(4, "Owner");
            this.imageList1.Images.SetKeyName(5, "PublishedStatus");
            this.imageList1.Images.SetKeyName(6, "PrivateStatus");
            this.imageList1.Images.SetKeyName(7, "Favorite");
            // 
            // colRating
            // 
            resources.ApplyResources(this.colRating, "colRating");
            // 
            // colPublishedDate
            // 
            resources.ApplyResources(this.colPublishedDate, "colPublishedDate");
            // 
            // colPurpose
            // 
            resources.ApplyResources(this.colPurpose, "colPurpose");
            // 
            // colDays
            // 
            resources.ApplyResources(this.colDays, "colDays");
            // 
            // colLanguage
            // 
            resources.ApplyResources(this.colLanguage, "colLanguage");
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuOpen,
            this.mnuAddToFavorites,
            this.mnuRemoveFromFavorites,
            this.mnuShowUserDetails});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // mnuOpen
            // 
            resources.ApplyResources(this.mnuOpen, "mnuOpen");
            this.mnuOpen.Name = "mnuOpen";
            this.mnuOpen.Click += new System.EventHandler(this.mnuOpen_Click);
            // 
            // mnuAddToFavorites
            // 
            resources.ApplyResources(this.mnuAddToFavorites, "mnuAddToFavorites");
            this.mnuAddToFavorites.Name = "mnuAddToFavorites";
            this.mnuAddToFavorites.Click += new System.EventHandler(this.mnuAddToFavorites_Click);
            // 
            // mnuRemoveFromFavorites
            // 
            resources.ApplyResources(this.mnuRemoveFromFavorites, "mnuRemoveFromFavorites");
            this.mnuRemoveFromFavorites.Name = "mnuRemoveFromFavorites";
            this.mnuRemoveFromFavorites.Click += new System.EventHandler(this.mnuRemoveFromFavorites_Click);
            // 
            // mnuShowUserDetails
            // 
            resources.ApplyResources(this.mnuShowUserDetails, "mnuShowUserDetails");
            this.mnuShowUserDetails.Name = "mnuShowUserDetails";
            this.mnuShowUserDetails.Click += new System.EventHandler(this.mnuShowUserDetails_Click);
            // 
            // WorkoutPlansListView
            // 
            this.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colType,
            this.colDifficult,
            this.colAuthor,
            this.colRating,
            this.colPublishedDate,
            this.colPurpose,
            this.colDays,
            this.colLanguage});
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.FullRowSelect = true;
            this.GridLines = true;
            this.HideSelection = false;
            this.MultiSelect = false;
            this.SmallImageList = this.imageList1;
            this.StateImageList = this.imageList1;
            this.View = System.Windows.Forms.View.Details;
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.WorkoutPlansListView_MouseDoubleClick);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            var pkt=this.PointToClient(MousePosition);
            var item = this.GetItemAt(pkt.X, pkt.Y);
            if(item!=null)
            {
                item.Selected = true;
            }
            else
            {
                e.Cancel = true;
                return;
            }
            mnuAddToFavorites.Visible = !SelectedPlan.IsFavorite() && !SelectedPlan.IsMine();
            mnuRemoveFromFavorites.Visible = SelectedPlan.IsFavorite() && !SelectedPlan.IsMine();
            mnuShowUserDetails.Visible = !SelectedPlan.Profile.IsMe();
        }

        private void mnuOpen_Click(object sender, EventArgs e)
        {
            ControlHelper.RunWithExceptionHandling(delegate
                                                       {
                                                           SelectedPlan.Open();
                                                       });
            
        }

        private void mnuAddToFavorites_Click(object sender, EventArgs e)
        {
            ControlHelper.RunWithExceptionHandling(delegate
                {
                    if (SelectedPlan.AddToFavorites())
                    {
                        Items.Clear();
                        fill(allPlans);
                    }
                });
            
        }

        private void mnuRemoveFromFavorites_Click(object sender, EventArgs e)
        {
            ControlHelper.RunWithExceptionHandling(delegate
            {
                if (SelectedPlan.RemoveFromFavorites())
                {
                    Items.Clear();
                    fill(allPlans);
                }
            });
            
        }

        public void ClearContent()
        {
            Items.Clear();
            allPlans.Clear();
        }

        private void WorkoutPlansListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ControlHelper.RunWithExceptionHandling(delegate
                                                       {
                                                           SelectedPlan.Open();
                                                       });
            
        }

        private void mnuShowUserDetails_Click(object sender, EventArgs e)
        {
            MainWindow.Instance.ShowUserInformation(SelectedPlan.Profile);
        }
    }
}
