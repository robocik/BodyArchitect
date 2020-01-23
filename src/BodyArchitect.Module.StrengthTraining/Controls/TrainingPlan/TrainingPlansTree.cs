using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Common;
using BodyArchitect.Controls;
using BodyArchitect.Controls.External;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Module.StrengthTraining.Localization;
using BodyArchitect.Module.StrengthTraining.Model.TrainingPlans;
using BodyArchitect.Service.Model;
using BodyArchitect.Service.Model.TrainingPlans;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    public enum TrainingPlansTreeGroup
    {
        TrainingType,
        Author,
        Difficult
    }

    public class TrainingPlansTree : TreeListView
    {
        private TreeListColumn colName;
        private TreeListColumn colAuthor;
        private TreeListColumn colTrainingType;
        private TreeListColumn colDifficult;
        private IContainer components;
        private System.Windows.Forms.ImageList imageList1;
        private TreeListColumn colCreationDate;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override TreeListColumnCollection Columns
        {
            get
            {
                return base.Columns;
            }
        }
        public TrainingPlansTree()
        {
            InitializeComponent();
            colName = new BodyArchitect.Controls.External.TreeListColumn("Name", StrengthTrainingEntryStrings.TrainingPlansTree_ColumnName);
            colAuthor = new BodyArchitect.Controls.External.TreeListColumn("Author", StrengthTrainingEntryStrings.TrainingPlansTree_ColumnAuthor);
            colTrainingType = new BodyArchitect.Controls.External.TreeListColumn("TrainingType", StrengthTrainingEntryStrings.TrainingPlansTree_ColumnWorkoutType);
            colDifficult = new BodyArchitect.Controls.External.TreeListColumn("Difficult", StrengthTrainingEntryStrings.TrainingPlansTree_ColumnDifficult);
            colCreationDate = new BodyArchitect.Controls.External.TreeListColumn("CreationDate", StrengthTrainingEntryStrings.TrainingPlansTree_ColumnCreationDate);

            colName.AutoSize = true;
            colName.AutoSizeMinSize = 0;
            colName.Width = 50;
            colAuthor.AutoSizeMinSize = 0;
            colAuthor.Width = 50;
            colAuthor.AutoSize = true;
            colTrainingType.AutoSize = true;
            colTrainingType.AutoSizeMinSize = 0;
            colTrainingType.Width = 50;
            colDifficult.AutoSize = true;
            colDifficult.AutoSizeMinSize = 0;
            colDifficult.Width = 50;
            colCreationDate.AutoSize = true;
            colCreationDate.AutoSizeMinSize = 0;
            colCreationDate.Width = 50;
            this.Columns.AddRange(new BodyArchitect.Controls.External.TreeListColumn[] {
            colName,colAuthor,colTrainingType,colDifficult,colCreationDate});
        }

        public void Fill(TrainingPlansTreeGroup treeFiller)
        {
            var parentWnd = this.GetParentControl<BaseWindow>();
            
            //var plans = ServiceManager.Instance.GetWorkoutPlans(UserContext.Token,null);
            //if (parentWnd == null)
            //{
            //    return;
            //}
            //parentWnd.SynchronizationContext.Send(delegate
            //             {
            //                 FocusedNode = null;
            //                 Nodes.Clear();
            //                 var mainNodes = getFiller(treeFiller).Fill(plans);
            //                 foreach (Node value in mainNodes)
            //                 {
            //                     if (value.Nodes.Count > 0)
            //                     {
            //                         Nodes.Add(value);
            //                     }
            //                 }
            //                 RecalcLayout();
            //             },null);

        }

        ITrainingPlanTreeFiller getFiller(TrainingPlansTreeGroup group)
        {
            switch (group)
            {
                    case TrainingPlansTreeGroup.Difficult:
                    return new TrainingPlanDifficultFiller();
                    case TrainingPlansTreeGroup.Author:
                    return new TrainingPlanAuthorFiller();
            }
            return new TrainingPlanTypeFiller();
        }

        public static Node CreateNode(WorkoutPlanDTO planDto)
        {
            string difficultText = new EnumLocalizer(StrengthTrainingEntryStrings.ResourceManager).Translate(planDto.Difficult);
            Node item = new Node(new[]{planDto.Name,planDto.Author, planDto.TrainingType.ToString(),difficultText,
                                       planDto.CreationDate.ToLocalTime().ToShortDateString()});
            item.Tag = planDto;
            return item;
        }

        protected override Image GetNodeBitmap(Node node)
        {
            WorkoutPlanDTO dto = node.Tag as WorkoutPlanDTO;
            if(dto!=null)
            {
                if(dto.Status==PublishStatus.Published)
                {
                    return imageList1.Images["Published"];
                }
                return imageList1.Images["TrainingPlan"];
            }
            return null;
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrainingPlansTree));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "TrainingPlanSigned");
            this.imageList1.Images.SetKeyName(1, "TrainingPlan");
            this.imageList1.Images.SetKeyName(2, "Published");
            // 
            // TrainingPlansTree
            // 
            this.MultiSelect = false;
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        
    }

    public interface ITrainingPlanTreeFiller
    {
        List<Node> Fill(IList<WorkoutPlanDTO> plans);
    }

    public class TrainingPlanDifficultFiller : ITrainingPlanTreeFiller
    {
        public List<Node> Fill(IList<WorkoutPlanDTO> plans)
        {
            Dictionary<TrainingPlanDifficult, Node> mainNodes = new Dictionary<TrainingPlanDifficult, Node>();

            foreach (TrainingPlanDifficult diff in Enum.GetValues(typeof(TrainingPlanDifficult)))
            {
                Node item = new Node(diff.ToString());
                mainNodes.Add(diff, item);
            }

            foreach (WorkoutPlanDTO planDto in plans)
            {
                var item = TrainingPlansTree.CreateNode(planDto);
                mainNodes[planDto.Difficult].Nodes.Add(item);
            }
            return mainNodes.Values.ToList();
        }
    }

    public class TrainingPlanTypeFiller : ITrainingPlanTreeFiller
    {
        public List<Node> Fill(IList<WorkoutPlanDTO> plans)
        {
            Dictionary<TrainingType, Node> mainNodes = new Dictionary<TrainingType, Node>();

            foreach (TrainingType diff in Enum.GetValues(typeof(TrainingType)))
            {
                Node item = new Node(diff.ToString());
                mainNodes.Add(diff, item);
            }

            foreach (WorkoutPlanDTO planDto in plans)
            {
                var item = TrainingPlansTree.CreateNode(planDto);
                mainNodes[planDto.TrainingType].Nodes.Add(item);
            }
            return mainNodes.Values.ToList();
        }
    }

    public class TrainingPlanAuthorFiller : ITrainingPlanTreeFiller
    {
        public List<Node> Fill(IList<WorkoutPlanDTO> plans)
        {
            Dictionary<string, Node> mainNodes = new Dictionary<string, Node>();


            foreach (WorkoutPlanDTO planDto in plans)
            {
                Node mainNode = null;
                if(!mainNodes.ContainsKey(planDto.Author))
                {
                    mainNode= new Node(planDto.Author);
                    mainNodes.Add(planDto.Author,mainNode);
                }
                else
                {
                    mainNode = mainNodes[planDto.Author];
                }
                var item = TrainingPlansTree.CreateNode(planDto);
                mainNode.Nodes.Add(item);
            }
            return mainNodes.Values.ToList();
        }
    }
}
