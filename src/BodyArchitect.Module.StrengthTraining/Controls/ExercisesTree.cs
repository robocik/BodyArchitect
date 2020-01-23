using System;
using BodyArchitect.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Common.ProgressOperation;
using BodyArchitect.Controls;
using BodyArchitect.Controls.External;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Logger;
using BodyArchitect.Module.StrengthTraining.Localization;
using BodyArchitect.Service.Model;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    public class ExercisesTree : TreeListView
    {
        TreeListColumn colName;
        TreeListColumn colShortcut;
        private System.Windows.Forms.ImageList imageList1;
        private IContainer components;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuAddExercise;
        private System.Windows.Forms.ToolStripMenuItem mnuDelete;
        private System.Windows.Forms.ToolStripMenuItem mnuEditExercise;
        //private System.Windows.Forms.ToolStripMenuItem mnuAddToFavorites;
        //private System.Windows.Forms.ToolStripMenuItem mnuRemoveFromFavorites;
        private ToolStripMenuItem mnuOpenExercise;
        Dictionary<ExerciseType,Node> muscleNodes = new Dictionary<ExerciseType, Node>();

        public event EventHandler FillReqest;

        public ExercisesTree()
        {
            createTreeColumns();
            InitializeComponent();
            this.MultiSelect = false;
            mnuDelete.Click+=new EventHandler(mnuDelete_Click);
            mnuEditExercise.Click+=new EventHandler(mnuEditExercise_Click);
            mnuAddExercise.Click += new EventHandler(mnuEditExercise_Click);
            mnuOpenExercise.Click += new EventHandler(mnuOpenExercise_Click);
        }

        public bool AllowsAdd { get; set; }


        protected virtual void OnFillReqest()
        {
            if(FillReqest!=null)
            {
                FillReqest(this, null);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public override TreeListColumnCollection Columns
        {
            get
            {
                return base.Columns;
            }
        }
        private void createTreeColumns()
        {
            colName = new BodyArchitect.Controls.External.TreeListColumn("Name", "Name");
            colShortcut = new BodyArchitect.Controls.External.TreeListColumn("Shortcut", "Shortcut");

            colName.Caption = StrengthTrainingEntryStrings.ColumnExerciseName;
            colShortcut.Caption = StrengthTrainingEntryStrings.ColumnExerciseShortcut;

            colName.AutoSize = true;
            colName.AutoSizeMinSize = 0;
            colName.Width = 50;
            colShortcut.AutoSizeMinSize = 0;
            colShortcut.Width = 50;
            this.Columns.AddRange(new BodyArchitect.Controls.External.TreeListColumn[] {
            colName,
            colShortcut});
        }

        public ExerciseDTO SelectedExercise
        {
            get
            {
                if (FocusedNode != null)
                {
                    return FocusedNode.Tag as ExerciseDTO;
                }
                return null;
            }
        }

        public ExerciseType? SelectedExerciseType
        {
            get
            {
                if (FocusedNode != null && FocusedNode.Tag is ExerciseType)
                {
                    return (ExerciseType)FocusedNode.Tag ;
                }
                return null;
            }
        }

        public void ClearContent()
        {
            muscleNodes.Clear();
            Nodes.Clear();
        }
        public void Fill(ICollection<ExerciseDTO> exercises)
        {
            var enums = Enum.GetValues(typeof(ExerciseType));
            if (muscleNodes.Count == 0)
            {
                foreach (ExerciseType exerciseType in enums)
                {
                    if (exerciseType == ExerciseType.NotSet)
                    {
                        continue;
                    }
                    //var exercises = ObjectsReposidory.GetExercises((Muscle)exerciseType);
                    Node advanceNode = null;
                    if (!muscleNodes.ContainsKey(exerciseType))
                    {
                        string text = string.Format("{0}",EnumLocalizer.Default.Translate<ExerciseType>((ExerciseType) exerciseType));
                        advanceNode = Nodes.Add(new Node(text));
                        advanceNode.Tag = exerciseType;
                        muscleNodes.Add(exerciseType, advanceNode);
                    }
                }
            }
            foreach (var exercise in exercises)
            {
                Node exerciseNode = muscleNodes[exercise.ExerciseType].Nodes.Add(new Node(new object[] { exercise.GetLocalizedName(), exercise.GetLocalizedShortcut() }));
                exerciseNode.ImageId = exercise.Status == PublishStatus.Published ? 1 : 0;
                exerciseNode.Tag = exercise;
            }
        }

        public List<ExerciseDTO> SelectedExercises
        {
            get
            {
                List<ExerciseDTO> exercises = new List<ExerciseDTO>();
                foreach (Node node in NodesSelection)
                {
                    var exercise = node.Tag as ExerciseDTO;
                    if (exercise != null)
                    {
                        exercises.Add(exercise);
                    }
                }
                return exercises;
            }
        }
        public bool CanDelete
        {
            get
            {
                return this.SelectedExercise != null && SelectedExercise.IsMine() &&
                       SelectedExercise.Status!= PublishStatus.Published;
            }
        }

        protected override Image GetNodeBitmap(Node node)
        {
            ExerciseDTO dto = node.Tag as ExerciseDTO;
            if (dto != null)
            {
                if (dto.Status == PublishStatus.Published)
                {
                    return imageList1.Images["Published"];
                }
                if (dto.Status == PublishStatus.PendingPublish)
                {
                    return imageList1.Images["Pending"];
                }
                if (dto.Status == PublishStatus.Private)
                {
                    return imageList1.Images["Private"];
                }
                 
            }
            return null;
        }

        public bool DeleteSelectedExercises()
        {
            var exercisesToDelete = this.SelectedExercise;
            if (exercisesToDelete==null || exercisesToDelete.Status == PublishStatus.Published || !exercisesToDelete.IsMine())
            {
                return false;
            }
            try
            {
                if (FMMessageBox.AskYesNo(ApplicationStrings.QRemoveExercise, SelectedExercise.GetLocalizedName()) == System.Windows.Forms.DialogResult.Yes)
                {
                    PleaseWait.Run(delegate
                                       {
                                           ServiceManager.DeleteExercise(exercisesToDelete);
                                           
 
                                       },false,null);
                    ObjectsReposidory.ClearExerciseCache();
                    OnFillReqest();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorRemoveExercise, ErrorWindow.EMailReport);
                return false;
            }
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExercisesTree));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuAddExercise = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditExercise = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOpenExercise = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Published");
            this.imageList1.Images.SetKeyName(1, "Private");
            this.imageList1.Images.SetKeyName(2, "Pending");
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAddExercise,
            this.mnuDelete,
            this.mnuEditExercise,
            this.mnuOpenExercise});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(196, 136);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // mnuAddExercise
            // 
            this.mnuAddExercise.Name = "mnuAddExercise";
            this.mnuAddExercise.Size = new System.Drawing.Size(195, 22);
            this.mnuAddExercise.Text = "&Add exercise...";
            // 
            // mnuDelete
            // 
            this.mnuDelete.Name = "mnuDelete";
            this.mnuDelete.Size = new System.Drawing.Size(195, 22);
            this.mnuDelete.Text = "&Delete...";
            // 
            // mnuEditExercise
            // 
            this.mnuEditExercise.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.mnuEditExercise.Name = "mnuEditExercise";
            this.mnuEditExercise.Size = new System.Drawing.Size(195, 22);
            this.mnuEditExercise.Text = "&Edit...";
            // 
            // mnuOpenExercise
            // 
            this.mnuOpenExercise.Name = "mnuOpenExercise";
            this.mnuOpenExercise.Size = new System.Drawing.Size(195, 22);
            this.mnuOpenExercise.Text = "&Open...";
            // 
            // ExercisesTree
            // 
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Images = this.imageList1;
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ExercisesTree_MouseDoubleClick);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        public int GetExercisesCount()
        {
            int count = 0;
            foreach (Node rootNodes in Nodes)
            {
                count += rootNodes.Nodes.Count;
            }
            return count;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            Node node = GetHitNode();
            FocusedNode = node;
            e.Cancel = node == null || (SelectedExerciseType!=null && !AllowsAdd);
            mnuDelete.Visible = SelectedExercise.IsEditable();
            mnuAddExercise.Visible =AllowsAdd && SelectedExercise == null;
            mnuEditExercise.Visible = SelectedExercise.IsEditable();
            mnuOpenExercise.Visible = SelectedExercise != null && !SelectedExercise.IsEditable();
        }

        public void OpenExercise(ExerciseDTO exercise)
        {
            if (exercise.GlobalId!=Guid.Empty)
            {
                exercise = exercise.Clone();
            }
            
            AddExercise dlg = new AddExercise();
            dlg.Fill(exercise);
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                OnFillReqest();
            }
        }

        private void mnuDelete_Click(object sender, EventArgs e)
        {
            DeleteSelectedExercises();
        }

        private void mnuEditExercise_Click(object sender, EventArgs e)
        {
            EditSelectedExercise();
        }

        public void EditSelectedExercise()
        {
            var exercise = this.SelectedExercise;
            if (exercise == null)
            {
                exercise = new ExerciseDTO(Guid.Empty);
                exercise.ExerciseType = SelectedExerciseType.Value;
            }
            OpenExercise(exercise);
        }

        private void ExercisesTree_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (SelectedExercise != null)
            {
                OpenExercise(SelectedExercise);
            }
        }

        void mnuOpenExercise_Click(object sender, EventArgs e)
        {
            if (SelectedExercise != null)
            {
                OpenExercise(SelectedExercise);
            }
        }

        //void mnuRemoveFromFavorites_Click(object sender, EventArgs e)
        //{
        //    if(SelectedExercise.IsFavorite() && FMMessageBox.AskYesNo("Do you want to remove selected exercise from your favorites list?")==DialogResult.Yes)
        //    {
        //        ServiceManager.ExerciseFavoritesOperation(SelectedExercise,FavoriteOperation.Remove);
        //        ObjectsReposidory.ClearExerciseCache();
        //        OnFillReqest();
        //    }
        //}

        //void mnuAddToFavorites_Click(object sender, EventArgs e)
        //{
        //    if (SelectedExercise.CanBeFavorite() && !SelectedExercise.IsFavorite() && FMMessageBox.AskYesNo("Do you want to add selected exercise to your favorites list?") == DialogResult.Yes)
        //    {
        //        ServiceManager.ExerciseFavoritesOperation(SelectedExercise, FavoriteOperation.Add);
        //        ObjectsReposidory.ClearExerciseCache();
        //        OnFillReqest();
        //    }
        //}
    }
}
