using System;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Logger;
using BodyArchitect.Module.StrengthTraining.Localization;
using BodyArchitect.Module.StrengthTraining.Model;
using System.Linq;
using BodyArchitect.Common;
using System.Collections.Generic;
using System.Windows.Forms;
using BodyArchitect.Controls;
using BodyArchitect.Service.Model;
using BodyArchitect.Service.Model.TrainingPlans;
using DevExpress.XtraEditors.DXErrorProvider;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    public partial class TrainingPlanEditorWindow : BaseWindow
    {
        private TrainingPlan plan;

        public TrainingPlanEditorWindow()
        {
            InitializeComponent();
            fillSuperTips();
            usrTrainingPlanSerieEditor1.TrainingPlanSetChanged += new EventHandler<ParameterEventArgs<TrainingPlanSerie>>(usrTrainingPlanSerieEditor1_TrainingPlanSetChanged);
            this.UsrTrainingPlanEntryEditor1.TrainingPlanChanged += new System.EventHandler<BodyArchitect.Module.StrengthTraining.Controls.ParameterEventArgs<TrainingPlanEntry>>(this.UsrTrainingPlanEntryEditor1_TrainingPlanChanged);
            updateToolbar();
            updateDetailsControl();
            fillTrainingTypes();
            fillLanguages();

            foreach (var purpose in Enum.GetValues(typeof(WorkoutPlanPurpose)))
            {
                cmbPurpose.Properties.Items.Add(EnumLocalizer.Default.Translate((WorkoutPlanPurpose)purpose));
            }

            foreach (var diff in Enum.GetValues(typeof(TrainingPlanDifficult)))
            {
                cmbDifficult.Properties.Items.Add(EnumLocalizer.Default.Translate((TrainingPlanDifficult)diff));
            }
        }


        void fillLanguages()
        {
            this.luCountries.Properties.DataSource = Language.Languages;
            this.luCountries.Properties.DisplayMember = "EnglishName";
            this.luCountries.Properties.ValueMember = "Shortcut";
            this.luCountries.Properties.NullValuePrompt = StrengthTrainingEntryStrings.SelectLanguageText;

        }
        void fillSuperTips()
        {
            ControlHelper.AddSuperTip(this.txtComment, grDescription.Text, StrengthTrainingEntryStrings.TrainingPlanEditor_DescriptionTXT);
            ControlHelper.AddSuperTip(this.txtName, lblName.Text, StrengthTrainingEntryStrings.TrainingPlanEditor_NameTXT);
            ControlHelper.AddSuperTip(this.txtAuthor, lblAuthor.Text, StrengthTrainingEntryStrings.TrainingPlanEditor_AuthorTXT);
            ControlHelper.AddSuperTip(this.cmbTrainingType, lblTrainingType.Text, StrengthTrainingEntryStrings.TrainingPlanEditor_TrainingTypeCMB);
            ControlHelper.AddSuperTip(this.cmbDifficult, lblDifficult.Text, StrengthTrainingEntryStrings.TrainingPlanEditor_DifficultCMB);
            ControlHelper.AddSuperTip(this.txtRestSeconds, lblRestSeconds.Text, StrengthTrainingEntryStrings.TrainingPlanEditor_RestTimeTXT);
            ControlHelper.AddSuperTip(this.btnValidate, btnValidate.Text, StrengthTrainingEntryStrings.TrainingPlanEditor_ValidateBTN);
            ControlHelper.AddSuperTip(this.btnSuperSets, btnSuperSets.Text, StrengthTrainingEntryStrings.TrainingPlanEditor_SuperSetsBTN);
        }

        void fillTrainingTypes()
        {
            cmbTrainingType.Properties.Items.Clear();
            var types = (TrainingType[])Enum.GetValues(typeof(TrainingType));
            cmbTrainingType.Properties.Items.AddRange(types);

        }

        public void Fill(TrainingPlan plan)
        {
            this.plan = plan;
            fillInfo(plan);
            fillDetailsTree(plan);
        }


        private void fillDetailsTree(TrainingPlan plan)
        {
            tvDetails.BeginUpdate();
            tvDetails.Nodes.Clear();
            foreach (var day in plan.Days)
            {
                TreeNode node = addTrainingDayNode(day);
                tvDetails.Nodes.Add(node);
                fillExercises(day, node);

            }
            tvDetails.EndUpdate();
        }

        private TreeNode addTrainingDayNode(TrainingPlanDay day)
        {
            TreeNode node = new TreeNode(day.Name);
            node.Tag = day;
            node.ImageKey = "TrainingDay";
            return node;
        }

        private void fillExercises(TrainingPlanDay day, TreeNode dayNode)
        {
            foreach (var entry in day.Entries)
            {
                TreeNode entryNode = addTrainingPlanEntryNode(entry);
                dayNode.Nodes.Add(entryNode);
                foreach (var set in entry.Sets)
                {
                    addSet(entryNode, set);
                }
            }
        }

        private TreeNode addTrainingPlanEntryNode(TrainingPlanEntry entry)
        {
            var exercise = ObjectsReposidory.GetExercise(entry.ExerciseId);
            TreeNode entryNode = new TreeNode(exercise.GetLocalizedName());
            entryNode.ImageKey = entryNode.SelectedImageKey = getTrainingPlanEntryIcon(entry);
            entryNode.Tag = entry;
            return entryNode;
        }

        string getTrainingPlanEntryIcon(TrainingPlanEntry entry)
        {
            var exercise = ObjectsReposidory.GetExercise(entry.ExerciseId);
            if (exercise != ExerciseDTO.Removed)
            {
                return "TrainingPlanEntry";
            }
            else
            {
                return "TrainingPlanEntryError";
            }
        }

        private TreeNode addSet(TreeNode entryNode, TrainingPlanSerie set)
        {
            TreeNode setNode = new TreeNode(set.GetDisplayText());
            setNode.Tag = set;
            setNode.ImageKey = setNode.SelectedImageKey = "Set";
            entryNode.Nodes.Add(setNode);
            return setNode;
        }

        private void fillInfo(TrainingPlan plan)
        {
            if(!string.IsNullOrWhiteSpace(plan.Comment))
            {
                plan.Comment = plan.Comment.Replace("\n", "\r\n");
            }
            txtComment.Text = plan.Comment;
            txtName.Text = plan.Name;
            txtAuthor.Text = plan.Author;
            if (string.IsNullOrEmpty(plan.Language))
            {
                plan.Language = "en";
            }
            luCountries.EditValue = plan.Language;
            txtUrl.Text = plan.Url;
            txtRestSeconds.Value = plan.RestSeconds;
            cmbTrainingType.SelectedIndex = (int)plan.TrainingType;
            cmbDifficult.SelectedIndex = (int)plan.Difficult;
            cmbPurpose.SelectedIndex = (int) plan.Purpose;

        }

        public TrainingPlan TrainingPlan { get { return plan; } }

        private void tvDetails_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (!(e.Node.Tag is TrainingPlanDay))
            {
                e.CancelEdit = true;
                return;
            }

        }

        private void tvDetails_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label != null)
            {
                if ((e.Node.Tag is TrainingPlanDay))
                {
                    TrainingPlanDay day = (TrainingPlanDay) e.Node.Tag;
                    day.Name = e.Label;
                    return;
                }
            }
        }

        private void tbMoveUp_Click(object sender, EventArgs e)
        {
            IRepositionableChild selectedDay = tvDetails.SelectedNode.Tag as IRepositionableChild;
            if (selectedDay != null && selectedDay.Position > 0)
            {
                selectedDay.RepositionableParent.RepositionEntry(selectedDay.Position, selectedDay.Position - 1);
                int index = tvDetails.SelectedNode.Index;
                var node = tvDetails.SelectedNode;
                var parentNode = node.Parent;
                tvDetails.SelectedNode.Remove();


                if (parentNode == null)
                {
                    tvDetails.Nodes.Insert(index - 1, node);
                }
                else
                {
                    parentNode.Nodes.Insert(index - 1, node);
                }
                tvDetails.SelectedNode = node;
            }
        }

        private void tbMoveDown_Click(object sender, EventArgs e)
        {
            var node = tvDetails.SelectedNode;
            IRepositionableChild selectedDay = node.Tag as IRepositionableChild;
            if (selectedDay != null && node.NextNode != null)
            {
                selectedDay.RepositionableParent.RepositionEntry(selectedDay.Position, selectedDay.Position + 1);
                int index = node.Index;
                var parentNode = node.Parent;
                node.Remove();
                if (parentNode == null)
                {
                    tvDetails.Nodes.Insert(index + 1, node);
                }
                else
                {
                    parentNode.Nodes.Insert(index + 1, node);
                }
                tvDetails.SelectedNode = node;
            }
        }

        private void tbNewDay_Click(object sender, EventArgs e)
        {
            var day = new TrainingPlanDay();
            plan.AddDay(day);
            day.Name = string.Format(StrengthTrainingEntryStrings.TrainingPlanNewDayName, tvDetails.Nodes.Count + 1);
            TreeNode node = addTrainingDayNode(day);
            tvDetails.Nodes.Add(node);
            node.BeginEdit();
        }

        T getSelected<T>() where T : class
        {
            if (tvDetails.SelectedNode != null)
            {
                return tvDetails.SelectedNode.Tag as T;
            }
            return null;
        }

        private void tbEditDay_Click(object sender, EventArgs e)
        {
            if (getSelected<TrainingPlanDay>() != null)
            {
                tvDetails.SelectedNode.BeginEdit();
            }
        }

        private void tbDeleteDay_Click(object sender, EventArgs e)
        {
            if (getSelected<TrainingPlanDay>() == null)
            {
                return;
            }
            if (FMMessageBox.AskYesNo(StrengthTrainingEntryStrings.QDeleteTrainingDay) == System.Windows.Forms.DialogResult.Yes)
            {
                plan.RemoveDay(getSelected<TrainingPlanDay>());
                tvDetails.SelectedNode.Remove();
            }
        }

        private void tvDetails_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var selectedEntry = getSelected<TrainingPlanEntry>();
            if (selectedEntry != null)
            {
                UsrTrainingPlanEntryEditor1.Visible = true;
                UsrTrainingPlanEntryEditor1.Fill(selectedEntry);
            }
            else
            {
                UsrTrainingPlanEntryEditor1.Visible = false;
            }
            updateToolbar();

            updateDetailsControl();

        }

        private void updateDetailsControl()
        {
            usrTrainingPlanSerieEditor1.Visible = getSelected<TrainingPlanSerie>() != null;
            usrTrainingPlanSerieEditor1.Fill(getSelected<TrainingPlanSerie>());
            UsrTrainingPlanEntryEditor1.Visible = getSelected<TrainingPlanEntry>() != null;
            UsrTrainingPlanEntryEditor1.Fill(getSelected<TrainingPlanEntry>());
        }

        private void UsrTrainingPlanEntryEditor1_TrainingPlanChanged(object sender, ParameterEventArgs<TrainingPlanEntry> e)
        {
            var entry = getSelected<TrainingPlanEntry>();
            if (entry == null)
            {
                return;
            }
            var exercise = ObjectsReposidory.GetExercise(entry.ExerciseId);
            tvDetails.SelectedNode.Text = exercise.GetLocalizedName();
            tvDetails.SelectedNode.SelectedImageKey = tvDetails.SelectedNode.ImageKey = getTrainingPlanEntryIcon(entry);
        }

        private void tbNewEntry_Click(object sender, EventArgs e)
        {
            var day = getSelected<TrainingPlanDay>();
            if (day != null)
            {
                TrainingPlanEntry newEntry = new TrainingPlanEntry();
                TreeNode entryNode = addTrainingPlanEntryNode(newEntry);
                entryNode.Text = StrengthTrainingEntryStrings.SelectExercise;
                day.AddEntry(newEntry);
                tvDetails.SelectedNode.Nodes.Add(entryNode);
                tvDetails.SelectedNode = entryNode;
            }
        }

        private void tbDeleteEntry_Click(object sender, EventArgs e)
        {
            var entry = getSelected<TrainingPlanEntry>();
            if (entry == null)
            {
                return;
            }
            if (FMMessageBox.AskYesNo(StrengthTrainingEntryStrings.QDeleteTrainingPlanEntry) == System.Windows.Forms.DialogResult.Yes)
            {
                entry.Day.RemoveEntry(entry);
                tvDetails.SelectedNode.Remove();
            }
        }

        void usrTrainingPlanSerieEditor1_TrainingPlanSetChanged(object sender, ParameterEventArgs<TrainingPlanSerie> e)
        {
            var entry = getSelected<TrainingPlanSerie>();
            if (entry == null)
            {
                return;
            }
            tvDetails.SelectedNode.Text = entry.GetDisplayText();
        }

        void updateToolbar()
        {
            tbMoveUp.Enabled = getSelected<IRepositionableChild>() != null && tvDetails.SelectedNode.PrevNode != null;
            tbMoveDown.Enabled = getSelected<IRepositionableChild>() != null && tvDetails.SelectedNode.NextNode != null;
            tbNewEntry.Visible = getSelected<TrainingPlanDay>() != null;
            tbDeleteEntry.Visible = getSelected<TrainingPlanEntry>() != null;
            tbEditDay.Visible = getSelected<TrainingPlanDay>() != null;
            tbDeleteDay.Visible = getSelected<TrainingPlanDay>() != null;
            tbDeleteSet.Visible = getSelected<TrainingPlanSerie>() != null;
            tbNewSet.Visible = getSelected<TrainingPlanEntry>() != null;
        }

        private void tbNewSet_Click(object sender, EventArgs e)
        {
            var entry = getSelected<TrainingPlanEntry>();
            if (entry == null)
            {
                return;
            }
            TrainingPlanSerie set = new TrainingPlanSerie(10);
            entry.Sets.Add(set);
            tvDetails.SelectedNode = addSet(tvDetails.SelectedNode, set);
        }

        private void tbDeleteSet_Click(object sender, EventArgs e)
        {
            var set = getSelected<TrainingPlanSerie>();
            if (set == null)
            {
                return;
            }

            if (FMMessageBox.AskYesNo(StrengthTrainingEntryStrings.QAskDeleteSet) == System.Windows.Forms.DialogResult.Yes)
            {
                var entry = (TrainingPlanEntry)tvDetails.SelectedNode.Parent.Tag;
                entry.Sets.Remove(set);
                tvDetails.SelectedNode.Remove();
            }
        }

        private void btnSuperSets_Click(object sender, EventArgs e)
        {
            TrainingPlanSuperSetsEditor wnd = new TrainingPlanSuperSetsEditor();
            wnd.Fill(plan);
            if (wnd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {

            }
        }


        private void okButton1_Click(object sender, EventArgs e)
        {
            updateTrainingPlan();
            if (!validationProvider1.DoValidate(this))
            {
                DialogResult = System.Windows.Forms.DialogResult.None;
                return;
            }

            //ValidatorFactory valFactory= EnterpriseLibraryContainer.Current.GetInstance<ValidatorFactory>();
            var validator = new ObjectValidator(typeof(TrainingPlan)); 
            var result = validator.Validate(plan);
            if(!result.IsValid)
            {
                DialogResult = System.Windows.Forms.DialogResult.None;
                FMMessageBox.ShowValidationError(result);
            }
        }

        private void updateTrainingPlan()
        {
            plan.Author = txtAuthor.Text;
            plan.Difficult = (TrainingPlanDifficult)cmbDifficult.SelectedIndex;
            plan.TrainingType = (TrainingType)cmbTrainingType.SelectedIndex;
            plan.RestSeconds = (int) txtRestSeconds.Value;
            plan.Name = txtName.Text;
            plan.Comment = txtComment.Text;
            plan.Url = txtUrl.Text;
            plan.Purpose = (WorkoutPlanPurpose) cmbPurpose.SelectedIndex;
            if (luCountries.EditValue != null)
            {
                plan.Language = (string)luCountries.EditValue;
            }
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {
            updateTrainingPlan();
            var pack = TrainingPlanPack.Create(TrainingPlan, ObjectsReposidory.Exercises);
            SplitPlanChecker checker = new SplitPlanChecker(pack);
            var result = checker.Check(ObjectsReposidory.Exercises);
            lvOutput.Items.Clear();
            foreach (var resultItem in result.Results)
            {
                List<string> texts = new List<string>();
                texts.Add(LocalizedPropertyGridStrings.ResourceManager.GetString(resultItem.ResourceKey));
                texts.AddRange(getNameAndType(resultItem.TrainingPlanBase));
                
                ListViewItem item = new ListViewItem(texts.ToArray());
                item.Tag = resultItem.TrainingPlanBase;
                item.ImageKey = resultItem.Status.ToString();
                lvOutput.Items.Add(item);
            }
        }


        private string[] getNameAndType(TrainingPlanBase planBase)
        {
            TrainingPlan plan = planBase as TrainingPlan;
            TrainingPlanEntry entry = planBase as TrainingPlanEntry;
            TrainingPlanDay day = planBase as TrainingPlanDay;
            TrainingPlanSerie set = planBase as TrainingPlanSerie;

            if (plan != null)
            {
                return new string[] { plan.Name, LocalizedPropertyGridStrings.TrainingPlan_Object };
            }
            else if (entry != null)
            {
                return new string[] { ObjectsReposidory.GetExercise(entry.ExerciseId).GetLocalizedName(), LocalizedPropertyGridStrings.TrainingPlanDayEntry_Object };

            }
            if (day != null)
            {
                return new string[] { day.Name, LocalizedPropertyGridStrings.TrainingPlanDay_Object };
            }
            if (set != null)
            {
                return new string[] { set.ToString(), LocalizedPropertyGridStrings.TrainingPlanSerie_Object };
            }
            return null;
        }

        private void lvOutput_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var item = lvOutput.GetItemAt(e.X, e.Y);
            if (item != null)
            {
                TrainingPlanBase planBase = (TrainingPlanBase)item.Tag;
                selectTrainingPlanBase(planBase);
            }
        }

        void selectTrainingPlanBase(TrainingPlanBase planBase)
        {
            tvDetails.SelectedNode = selectProcess(tvDetails.Nodes, planBase);
        }

        TreeNode selectProcess(TreeNodeCollection nodes,TrainingPlanBase planBase)
        {
            foreach (TreeNode node in nodes)
            {
                if(node.Tag==planBase)
                {
                    return node;
                }
                var nodeToSelect = selectProcess(node.Nodes, planBase);
                if(nodeToSelect!=null)
                {
                    return nodeToSelect;
                }

            }
            return null;
        }

        private void validationProvider1_ValidationPerformed(object sender, Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WinForms.ValidationPerformedEventArgs e)
        {
            dxErrorProvider1.SetError(e.ValidatedControl, null, ErrorType.None);
            foreach (ValidationResult validationResult in e.ValidationResults)
            {
                dxErrorProvider1.SetError(e.ValidatedControl, validationResult.Message, ErrorType.Default);
            }
        }

        private void TrainingPlanEditorWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(DialogResult==DialogResult.Cancel && FMMessageBox.AskYesNo(StrengthTrainingEntryStrings.QWorkoutPlanEditorCloseWindow)==System.Windows.Forms.DialogResult.No)
            {
                e.Cancel = true;
            }
        }
    }
}