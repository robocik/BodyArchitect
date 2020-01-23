using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Module.StrengthTraining.Localization;
using BodyArchitect.Service.Model;
using BodyArchitect.Shared;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using BodyArchitect.Common;
using BodyArchitect.Controls;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Module.StrengthTraining.Model;
using DevExpress.XtraEditors.DXErrorProvider;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using ControlHelper = BodyArchitect.Controls.ControlHelper;


namespace BodyArchitect.Module.StrengthTraining.Controls
{
    public partial class usrExerciseEditor : usrBaseControl
    {
        private ExerciseDTO exercise;
        public event EventHandler<ControlValidatedEventArgs> ControlValidated;
        private IEnumerable<ExerciseDTO> exercises = new List<ExerciseDTO>();

        public usrExerciseEditor()
        {
            InitializeComponent();
            fillLocalizedExerciseTypes();
            zoomDifficuilt.Properties.Minimum = 0;
            zoomDifficuilt.Properties.Maximum = 4;
            txtName.Properties.MaxLength = Constants.NameColumnLength;
            fillSuperTips();

            foreach (MechanicsType item in Enum.GetValues(typeof(MechanicsType)))
            {
                cmbMechanicsType.Properties.Items.Add(EnumLocalizer.Default.Translate(item));
            }

            foreach (ExerciseForceType item in Enum.GetValues(typeof(ExerciseForceType)))
            {
                cmbForce.Properties.Items.Add(EnumLocalizer.Default.Translate(item));
            }
        }

        [DefaultValue(true)]
        public bool AllowRedirectToDetails
        {
            get { return usrWorkoutCommentsList1.AllowRedirectToDetails; }
            set { usrWorkoutCommentsList1.AllowRedirectToDetails = value; }
        }

        private void fillLocalizedExerciseTypes()
        {
            var enums = Enum.GetValues(typeof(ExerciseType));
            cmbExerciseType.Properties.Columns.Add(new LookUpColumnInfo("LocalizedName", ApplicationStrings.CMBExerciseName));
            cmbExerciseType.Properties.DisplayMember = "LocalizedName";
            cmbExerciseType.Properties.ValueMember = "ExerciseType";
            var list = new List<LocalizedExerciseType>();
            foreach (var type in enums)
            {
                LocalizedExerciseType localizedExerciseType = new LocalizedExerciseType((ExerciseType)type);
                list.Add(localizedExerciseType);
                
            }
            cmbExerciseType.Properties.DataSource = list;
        }

        void fillSuperTips()
        {
            ControlHelper.AddSuperTip(this.txtName, lblName.Text, StrengthTrainingEntryStrings.ExerciseEditor_NameTE);
            ControlHelper.AddSuperTip(this.txtDescription, lblDescription.Text, StrengthTrainingEntryStrings.ExerciseEditor_DescriptionTE);
            ControlHelper.AddSuperTip(this.txtShortcut, lblShortcut.Text, StrengthTrainingEntryStrings.ExerciseEditor_ShortcutTE);
            ControlHelper.AddSuperTip(this.txtUrl, lblUrl.Text, StrengthTrainingEntryStrings.ExerciseEditor_UrlTE);
            ControlHelper.AddSuperTip(this.cmbExerciseType, lblExerciseType.Text, StrengthTrainingEntryStrings.ExerciseEditor_ExerciseTypeCMB);
            ControlHelper.AddSuperTip(this.zoomDifficuilt, lblDifficuilt.Text, StrengthTrainingEntryStrings.ExerciseEditor_DifficuiltTE);

            ControlHelper.AddSuperTip(this.cmbMechanicsType, lblMechanicType.Text, StrengthTrainingEntryStrings.ExerciseEditor_MechanicsTypeCMB);
            ControlHelper.AddSuperTip(this.cmbForce, lblForce.Text, StrengthTrainingEntryStrings.ExerciseEditor_ForceCMB);
        }

        void setReadOnly(bool readOnly)
        {
            txtUrl.Properties.ReadOnly = readOnly;
            txtDescription.Properties.ReadOnly = readOnly;
            txtName.Properties.ReadOnly = readOnly;
            txtShortcut.Properties.ReadOnly = readOnly;
            cmbExerciseType.Properties.ReadOnly = readOnly;
            cmbMechanicsType.Properties.ReadOnly = readOnly;
            cmbForce.Properties.ReadOnly = readOnly;
            zoomDifficuilt.Properties.ReadOnly = readOnly;
        }
        public bool Fill(ExerciseDTO exercise, IEnumerable<ExerciseDTO> exercises,bool readOnly=false)
        {
            bool isReadOnly =readOnly ||  exercise.Status!=PublishStatus.Private || (exercise.Profile == null && exercise.GlobalId != Guid.Empty);
            setReadOnly(isReadOnly);
            this.exercise = exercise.Clone();
            txtUrl.Text = exercise.GetLocalizedUrl();
            txtDescription.Text = exercise.GetLocalizedDescription();
            txtName.Text = exercise.GetLocalizedName();
            txtShortcut.Text = exercise.GetLocalizedShortcut();
            cmbExerciseType.EditValue = exercise.ExerciseType;
            cmbMechanicsType.SelectedIndex =(int) exercise.MechanicsType;
            cmbForce.SelectedIndex = (int) exercise.ExerciseForceType;
            zoomDifficuilt.Value = (int)exercise.Difficult;
            if(exercises!=null)
            {
                this.exercises = exercises;
            }

            bool showComments =exercise.Status!= PublishStatus.Private &&  exercise.GlobalId != Guid.Empty;
            tpComments.PageVisible = showComments;
            if (showComments && xtraTabControl.SelectedTabPage == tpComments)
            {
                usrWorkoutCommentsList1.CannotVote = exercise.IsMine();
                usrWorkoutCommentsList1.Fill(exercise);
                
            }

            validateData();
            return isReadOnly;
        }

        public void SetChangeShortcutMode(bool shortcutMode)
        {
            txtName.Properties.ReadOnly = !shortcutMode;
            txtUrl.Properties.ReadOnly = !shortcutMode;
            txtDescription.Properties.ReadOnly = !shortcutMode;
            zoomDifficuilt.Properties.ReadOnly = !shortcutMode;
        }

        public ExerciseDTO SaveExercise()
        {
            exercise.Url = txtUrl.Text;
            exercise.Description = txtDescription.Text;
            exercise.Name = txtName.Text;
            exercise.Shortcut = txtShortcut.Text;
            exercise.ExerciseType = (ExerciseType)cmbExerciseType.EditValue;
            exercise.Difficult = (ExerciseDifficult) zoomDifficuilt.Value;
            exercise.ExerciseForceType = (ExerciseForceType)cmbForce.SelectedIndex;
            exercise.MechanicsType = (MechanicsType)cmbMechanicsType.SelectedIndex;
            exercise.Profile = UserContext.CurrentProfile;

            ServiceManager.SaveExercise(exercise);
            ObjectsReposidory.ClearExerciseCache();
            return exercise;
        }

        protected void OnControlValidated(bool isValid)
        {
            if (ControlValidated != null)
            {
                ControlValidated(this, new ControlValidatedEventArgs(isValid));
            }
        }

        private void txtShortcut_EditValueChanged(object sender, EventArgs e)
        {
            validateData();
        }

        private void validateData()
        {
            var res = from ex in exercises where ex.Shortcut == txtShortcut.Text select ex;
            var existingExercises = res.ToList();
            bool uniqueShortcut = (existingExercises.Count == 0 || existingExercises[0].GlobalId == exercise.GlobalId);
            bool isValid = false;
            if (uniqueShortcut && txtName.Text.Length > 0 && txtShortcut.Text.Length > 0)
            {
                isValid = true;
            }
            OnControlValidated(isValid);
            dxErrorProvider1.SetError(txtShortcut, txtShortcut.Text.Length > 0 && uniqueShortcut ? null : ApplicationStrings.ErrorMustBeUnique);
            dxErrorProvider1.SetError(txtName, txtName.Text.Length > 0 ? null : ApplicationStrings.ErrorCannotBeEmpty);
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            validateData();
        }

        private void xtraTabControl_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if(e.Page==tpComments)
            {
                usrWorkoutCommentsList1.CannotVote = exercise.IsMine();
                usrWorkoutCommentsList1.Fill(exercise);
            }
        }

        private void validationProvider1_ValidationPerformed(object sender, Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WinForms.ValidationPerformedEventArgs e)
        {
            dxErrorProvider1.SetError(e.ValidatedControl, null, ErrorType.None);
            foreach (ValidationResult validationResult in e.ValidationResults)
            {
                dxErrorProvider1.SetError(e.ValidatedControl, validationResult.Message, ErrorType.Default);
            }
        }
    }
}
