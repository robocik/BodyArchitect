using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.StrengthTraining.Localization;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Service.V2.Model.Localization;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls
{
    /// <summary>
    /// Interaction logic for usrExerciseEditor.xaml
    /// </summary>
    public partial class usrExerciseEditor : IEditableControl
    {
        private IEnumerable<ExerciseDTO> exercises = new List<ExerciseDTO>();
        private ExerciseDTO exercise;

        public usrExerciseEditor()
        {
            InitializeComponent();
            DataContext = this;
            foreach (ExerciseType type in Enum.GetValues(typeof(ExerciseType)))
            {
                if (type != ExerciseType.NotSet)
                {
                    cmbExerciseTypes.Items.Add(new ListItem<ExerciseType>(EnumLocalizer.Default.Translate(type), type));
                }
            }
            SortDescription sd = new SortDescription("Text", ListSortDirection.Ascending);
            cmbExerciseTypes.Items.SortDescriptions.Add(sd);

            foreach (ExerciseForceType force in Enum.GetValues(typeof(ExerciseForceType)))
            {
                cmbForces.Items.Add(new ListItem<ExerciseForceType>(EnumLocalizer.Default.Translate(force), force));
            }
            sd = new SortDescription("Text", ListSortDirection.Ascending);
            cmbForces.Items.SortDescriptions.Add(sd);

            foreach (MechanicsType mechanics in Enum.GetValues(typeof(MechanicsType)))
            {
                cmbMechanicsTypes.Items.Add(new ListItem<MechanicsType>(EnumLocalizer.Default.Translate(mechanics), mechanics));
            }
            sd = new SortDescription("Text", ListSortDirection.Ascending);
            cmbMechanicsTypes.Items.SortDescriptions.Add(sd);

            txtName.MaxLength = Constants.NameColumnLength;
            txtDescription.MaxLength = Constants.CommentColumnLength;
            txtUrl.MaxLength = Constants.UrlLength;
            txtShortcut.MaxLength = 20;
            setReadOnly();
        }
        #region Properties

        private string _name;
        [Required]
        public string ExerciseName
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyOfPropertyChange(() => ExerciseName);
            }
        }
        private string _description;

        [ValidatorComposition(CompositionType.Or)]
        [StringLengthValidator(0, Constants.CommentColumnLength)]
        [NotNullValidator(Negated = true)]
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                NotifyOfPropertyChange(() => Description);
            }
        }

        private string _url;
        public string Url
        {
            get { return _url; }
            set
            {
                _url = value;
                NotifyOfPropertyChange(() => Url);
            }
        }

        private string _shortcut;
        [Required]
        public string Shortcut
        {
            get { return _shortcut; }
            set
            {
                //var res = from ex in exercises where ex.GlobalId!=exercise.GlobalId &&  ex.Shortcut == value select ex;
                //if(res.Count()>0)
                //{
                //    throw new UniqueException(Strings.ErrorMustBeUnique);
                //}
                _shortcut = value;
                NotifyOfPropertyChange(() => Shortcut);
            }
        }
        private ExerciseType _exerciseType;
        public ExerciseType ExerciseType
        {
            get { return _exerciseType; }
            set
            {
                _exerciseType = value;
                NotifyOfPropertyChange(() => ExerciseType);
            }
        }
        private ExerciseForceType _force;
        public ExerciseForceType Force
        {
            get { return _force; }
            set
            {
                _force = value;
                NotifyOfPropertyChange(() => Force);
            }
        }
        private MechanicsType _mechanics;
        public MechanicsType Mechanics
        {
            get { return _mechanics; }
            set
            {
                _mechanics = value;
                NotifyOfPropertyChange(() => Mechanics);
            }
        }

        private double _difficult;
        public double Difficult
        {
            get { return _difficult; }
            set
            {
                _difficult = value;
                NotifyOfPropertyChange(() => Difficult);
            }
        }

        private decimal _met;
        public decimal Met
        {
            get { return _met; }
            set
            {
                _met = value;
                NotifyOfPropertyChange(() => Met);
            }
        }
        #endregion

        //void fillSuperTips()
        //{
        //    ControlHelper.AddSuperTip(this.txtName, lblName.Text, StrengthTrainingEntryStrings.ExerciseEditor_NameTE);
        //    ControlHelper.AddSuperTip(this.txtDescription, lblDescription.Text, StrengthTrainingEntryStrings.ExerciseEditor_DescriptionTE);
        //    ControlHelper.AddSuperTip(this.txtShortcut, lblShortcut.Text, StrengthTrainingEntryStrings.ExerciseEditor_ShortcutTE);
        //    ControlHelper.AddSuperTip(this.txtUrl, lblUrl.Text, StrengthTrainingEntryStrings.ExerciseEditor_UrlTE);
        //    ControlHelper.AddSuperTip(this.cmbExerciseType, lblExerciseType.Text, StrengthTrainingEntryStrings.ExerciseEditor_ExerciseTypeCMB);
        //    ControlHelper.AddSuperTip(this.zoomDifficuilt, lblDifficuilt.Text, StrengthTrainingEntryStrings.ExerciseEditor_DifficuiltTE);

        //    ControlHelper.AddSuperTip(this.cmbMechanicsType, lblMechanicType.Text, StrengthTrainingEntryStrings.ExerciseEditor_MechanicsTypeCMB);
        //    ControlHelper.AddSuperTip(this.cmbForce, lblForce.Text, StrengthTrainingEntryStrings.ExerciseEditor_ForceCMB);
        //}

        public void ClearContent()
        {
            Url = string.Empty;
            Description = string.Empty;
            ExerciseName = string.Empty;
            Shortcut = string.Empty;
            ExerciseType = ExerciseType.NotSet;
            Mechanics = MechanicsType.NotSet;
            Force = ExerciseForceType.NotSet;
            Met = 0;
            Difficult = 0;
        }
        public void Fill(ExerciseDTO exercise, IEnumerable<ExerciseDTO> exercises)
        {
            this.exercise = exercise;
            setReadOnly();
            Url = exercise.GetLocalizedUrl();
            Description = exercise.GetLocalizedDescription();
            ExerciseName = exercise.GetLocalizedName();
            Shortcut = exercise.GetLocalizedShortcut();
            ExerciseType = exercise.ExerciseType;
            Mechanics= exercise.MechanicsType;
            Force = exercise.ExerciseForceType;
            Met = exercise.Met;
            Difficult = (double)exercise.Difficult;
            if (exercises != null)
            {
                this.exercises = exercises;
            }

        }

        private void updateExercise()
        {
            exercise.Url = Url;
            exercise.Description = Description;
            exercise.Name = ExerciseName;
            exercise.Shortcut = Shortcut;
            exercise.ExerciseType = ExerciseType;
            exercise.Difficult = (ExerciseDifficult)Difficult;
            exercise.ExerciseForceType = Force;
            exercise.MechanicsType = Mechanics;
            exercise.Met = Met;
            exercise.Profile = UserContext.Current.CurrentProfile;
        }

        void setReadOnly()
        {
            var nameReadOnly = exercise!=null && !exercise.IsNew;
            txtUrl.IsReadOnly = ReadOnly;
            txtDescription.IsReadOnly = ReadOnly;
            txtName.IsReadOnly  = ReadOnly || nameReadOnly;
            txtShortcut.IsReadOnly = ReadOnly;
            cmbExerciseTypes.IsEnabled = !(ReadOnly || nameReadOnly);
            cmbMechanicsTypes.IsEnabled = !ReadOnly;
            cmbForces.IsEnabled = !ReadOnly;
            sliderDifficult.IsEnabled = !ReadOnly;
            txtMet.IsReadOnly = ReadOnly;
        }

        public object Object
        {
            get { return DataContext; }
            set
            {
                exercise = (ExerciseDTO)value;
                DataContext = value;
            }
        }

        private bool _readOnly;

        public bool ReadOnly
        {
            get { return _readOnly; }
            set
            {
                _readOnly = value;
                setReadOnly();
            }
        }

        public object Save()
        {
            updateExercise();
            return ServiceManager.SaveExercise(exercise);
        }

        private void btnMetWebSite_Click(object sender, RoutedEventArgs e)
        {
            Helper.OpenUrl("https://sites.google.com/site/compendiumofphysicalactivities/Activity-Categories");
        }
    }
}
