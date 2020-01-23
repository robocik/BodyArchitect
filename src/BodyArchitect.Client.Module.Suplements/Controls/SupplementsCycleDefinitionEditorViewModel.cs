using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Localization;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using BodyArchitect.Service.V2.Model.Validators;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using ValidationException = BodyArchitect.Portable.Exceptions.ValidationException;

namespace BodyArchitect.Client.Module.Suplements.Controls
{
    public class SupplementsCycleDefinitionEditorViewModel:ViewModelBase
    {
        private SupplementCycleDefinitionDTO definition=new SupplementCycleDefinitionDTO();//we create an empty instance only for not geting object null exception during creating this view
        private SupplementsCycleDefinitionContext context;
        private List<ListItem<WorkoutPlanPurpose>> purposes = new List<ListItem<WorkoutPlanPurpose>>();
        private List<ListItem<TrainingPlanDifficult>> difficulties = new List<ListItem<TrainingPlanDifficult>>();
        private bool canDelete;
        private bool canAddWeek;
        private bool canSave;
        private bool canAddDosage;
        private SupplementCycleItemTreeItemViewModel selectedItem;
        private IBAWindow parentView;
        private bool isModified;

        public SupplementsCycleDefinitionEditorViewModel()
        {
            
            foreach (WorkoutPlanPurpose purpose in Enum.GetValues(typeof(WorkoutPlanPurpose)))
            {
                Purposes.Add(new ListItem<WorkoutPlanPurpose>(EnumLocalizer.Default.Translate(purpose), purpose));
            }

            foreach (TrainingPlanDifficult diff in Enum.GetValues(typeof(TrainingPlanDifficult)))
            {

                Difficulties.Add(new ListItem<TrainingPlanDifficult>(EnumLocalizer.Default.Translate(diff), diff));
            }

            Weeks = new ObservableCollection<SupplementsCycleWeekViewModel>();
        }
        public void Fill(IBAWindow parentView, SupplementsCycleDefinitionContext context)
        {
            this.context = context;
            this.parentView = parentView;
            this.Cycle = context.CycleDefinition.StandardClone();
            Weeks.Clear();
            
            foreach (var day in definition.Weeks.OrderBy(x=>x.CycleWeekStart))
            {
                Weeks.Add(new SupplementsCycleWeekViewModel(this, day));
            }
            updateButtons();
        }

        public string Header
        {
            get
            {
                return EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Suplements:SuplementsEntryStrings:SupplementsCycleDefinitionEditorViewModel_Header_CEditor") + Cycle.Name;
            }
        }

        public ObservableCollection<SupplementsCycleWeekViewModel> Weeks { get; private set; }

        void updateButtons(bool operationStarted=false)
        {
            CanAddWeek = true;
            CanAddDosage = SelectedItem is SupplementsCycleWeekViewModel;
            CanDelete = SelectedItem!=null;
            CanSave = !operationStarted;
        }

        public SupplementCycleItemTreeItemViewModel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                if (selectedItem != null)
                {
                    selectedItem.IsSelected = true;
                }
                updateButtons();
                NotifyOfPropertyChange(() => SelectedItem);
                NotifyOfPropertyChange(() => SelectedWeek);
                NotifyOfPropertyChange(() => SelectedDosage);
            }
        }

        public WorkoutPlanPurpose Purpose
        {
            get { return Cycle.Purpose; }
            set
            {
                Cycle.Purpose = value;
                IsModified = true;
            }
        }

        [LanguageValidator(MessageTemplateResourceName = "SupplementsCycleDefinitionEditorViewModel_Language_Validator", MessageTemplateResourceType = typeof(SuplementsEntryStrings))]
        public string Language
        {
            get { return Cycle.Language; }
            set
            {
                Cycle.Language = value;
                IsModified = true;
            }
        }

        [Required]
        [StringLengthValidator(Constants.NameColumnLength, MessageTemplateResourceName = "SupplementsCycleDefinitionEditorViewModel_Name_Length_Validator", MessageTemplateResourceType = typeof(SuplementsEntryStrings))]
        public string Name
        {
            get { return Cycle.Name; }
            set
            {
                Cycle.Name = value;
                NotifyOfPropertyChange(()=>Header);
                IsModified = true;
            }
        }

        public TrainingPlanDifficult Difficult
        {
            get { return Cycle.Difficult; }
            set
            {
                Cycle.Difficult = value;
                IsModified = true;
            }
        }

        [ValidatorComposition(CompositionType.Or)]
        [StringLengthValidator(0, Constants.NameColumnLength, MessageTemplateResourceName = "SupplementsCycleDefinitionEditorViewModel_Author_Length_Validator", MessageTemplateResourceType = typeof(SuplementsEntryStrings))]
        [NotNullValidator(Negated = true)]
        public string Author
        {
            get { return Cycle.Author; }
            set
            {
                Cycle.Author = value;
                IsModified = true;
            }
        }



        [ValidatorComposition(CompositionType.Or)]
        [StringLengthValidator(0, Constants.UrlLength, MessageTemplateResourceName = "SupplementsCycleDefinitionEditorViewModel_Url_Length_Validator", MessageTemplateResourceType = typeof(SuplementsEntryStrings))]
        [NotNullValidator(Negated = true)]
        public string Url
        {
            get { return Cycle.Url; }
            set
            {
                Cycle.Url = value;
                IsModified = true;
            }
        }

        public string Comment
        {
            get { return Cycle.Comment; }
            set
            {
                Cycle.Comment = value;
                IsModified = true;
            }
        }

        public SupplementCycleWeekDTO SelectedWeek
        {
            get
            {
                var weekModel = SelectedItem as SupplementsCycleWeekViewModel;
                if(weekModel!=null)
                {
                    return weekModel.Week;
                }
                return null;
            }
        }

        public SupplementCycleDosageDTO SelectedDosage
        {
            get
            {
                var dosageModel = SelectedItem as SupplementsCycleDosageViewModel;
                if (dosageModel != null)
                {
                    return dosageModel.Entry;
                }
                return null;
            }
        }

        public bool CanDelete
        {
            get { return canDelete; }
            set
            {
                canDelete = value;
                NotifyOfPropertyChange(() => CanDelete);
            }
        }

        public bool CanAddWeek
        {
            get { return canAddWeek; }
            set
            {
                canAddWeek = value;
                NotifyOfPropertyChange(() => CanAddWeek);
            }
        }

        public bool CanSave
        {
            get { return canSave; }
            set
            {
                canSave = value;
                NotifyOfPropertyChange(() => CanSave);
            }
        }


        public bool CanAddDosage
        {
            get { return canAddDosage; }
            set
            {
                canAddDosage = value;
                NotifyOfPropertyChange(() => CanAddDosage);
            }
        }

        public SupplementCycleDefinitionDTO Cycle
        {
            get { return definition; }
            set
            {
                definition = value;
                NotifyOfPropertyChange((string)null);
            }
        }

        public ICollection<Language>  Languages
        {
            get { return BodyArchitect.Service.V2.Model.Language.Languages; }
        }

        public List<ListItem<WorkoutPlanPurpose>> Purposes
        {
            get { return purposes; }
        }

        public List<ListItem<TrainingPlanDifficult>> Difficulties
        {
            get { return difficulties; }
        }


        public bool IsModified
        {
            get { return isModified; }
            set
            {
                setModifiedFlag();
                NotifyOfPropertyChange(() => IsModified);
            }
        }

        void setModifiedFlag()
        {
            var val = definition.IsModified(context.CycleDefinition);
            isModified = val;
        }

        public void Save()
        {
            var validator = new ObjectValidator(typeof(SupplementCycleDefinitionDTO));
            if(definition.Profile==null)
            {
                definition.Profile = UserContext.Current.CurrentProfile;
            }
            var result = validator.Validate(definition);

            if (!result.IsValid)
            {
                BAMessageBox.ShowValidationError(result.ToBAResults());
                return;
            }
            parentView.RunAsynchronousOperation(delegate
            {
                try
                {
                    
                    context.CycleDefinition = ServiceManager.SaveSupplementsCycleDefinition(definition);
                    //todo:add here clone of saved definition to the cache
                    SupplementsCycleDefinitionsReposidory.Instance.Update(context.CycleDefinition.StandardClone());
                    parentView.SynchronizationContext.Send(delegate
                    {
                        Fill(parentView, context);
                        IsModified = false;
                    }, null);
                }
                catch (ValidationException ex)
                {
                    parentView.SynchronizationContext.Send(state => BAMessageBox.ShowValidationError(ex.Results), null);
                }
                catch (Exception ex)
                {
                    parentView.SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex, SuplementsEntryStrings.Exception_SupplementsCycleDefinitionEditorViewModel_Save, ErrorWindow.EMailReport), null);
                }


            },v=>
                  {
                      updateButtons(v.State==OperationState.Started);
                  });
        }

        public void AddWeek()
        {
            SupplementCycleWeekDTO week = new SupplementCycleWeekDTO();
            week.CycleWeekStart = week.CycleWeekEnd = getNextWeekNumber();
            definition.Weeks.Add(week);

            var viewModel = new SupplementsCycleWeekViewModel(this, week);
            Weeks.Add(viewModel);
            viewModel.IsSelected = true;
            IsModified = true;
        }

        public void AddDosage()
        {
            var week = SelectedWeek;
            var dosage = new SupplementCycleDosageDTO();
            week.Dosages.Add(dosage);
            var parent=(SupplementsCycleWeekViewModel)SelectedItem;
            var viewModel = new SupplementsCycleDosageViewModel(parent, dosage);
            parent.Dosages.Add(viewModel);
            parent.IsExpanded = true;
            viewModel.IsSelected = true;
            IsModified = true;
        }

        int getNextWeekNumber()
        {
            int max = 0;
            foreach (var week in Weeks)
            {
                max = Math.Max(week.CycleWeekEnd, max);
            }
            return max + 1;
        }

        public void Delete()
        {
            var weekViewModel = SelectedItem as SupplementsCycleWeekViewModel;
            var dosageViewModel = SelectedItem as SupplementsCycleEntryViewModel;
            if(weekViewModel!=null)
            {
                Weeks.Remove(weekViewModel);
                definition.Weeks.Remove(weekViewModel.Week);
                IsModified = true;
                recalculateWeeksNumber();
            }
            else if(dosageViewModel!=null)
            {
                dosageViewModel.ParentWeek.Dosages.Remove(dosageViewModel);
                dosageViewModel.ParentWeek.Week.Dosages.Remove(dosageViewModel.Entry);
                IsModified = true;
            }
        }

        void recalculateWeeksNumber()
        {
            //TODO:Is this good?
            //int lastWeek = 0;
            //for (int index = 0; index < Weeks.Count; index++)
            //{
            //    var week = Weeks[index];
            //    int diff = Math.Abs(week.Week.CycleWeekEnd - week.CycleWeekStart);
            //    week.CycleWeekStart = lastWeek + 1;
            //    lastWeek=week.CycleWeekEnd = week.CycleWeekStart + diff;

            //}
        }

        public void AddMeasurements()
        {
            var week = SelectedWeek;
            var dosage = new SupplementCycleMeasurementDTO();
            week.Dosages.Add(dosage);
            var parent = (SupplementsCycleWeekViewModel)SelectedItem;
            var viewModel = new SupplementsCycleEntryViewModel(parent, dosage);
            parent.Dosages.Add(viewModel);
            parent.IsExpanded = true;
            viewModel.IsSelected = true;
            IsModified = true;
        }
    }

    public abstract class SupplementCycleItemTreeItemViewModel : TreeItemViewModel
    {

        private string _header;
        public virtual string Header
        {
            get { return _header; }
            set
            {
                _header = value;
                NotifyOfPropertyChange(() => Header);
            }
        }

        private string _image;
        public virtual string Image
        {
            get { return _image; }
            set
            {
                _image = value;
                NotifyOfPropertyChange(() => Image);
            }
        }

        public virtual void Update()
        {
            NotifyOfPropertyChange(() => Header);
        }
    }

    public class SupplementsCycleWeekViewModel : SupplementCycleItemTreeItemViewModel
    {
        private SupplementCycleWeekDTO week;
        private SupplementsCycleDefinitionEditorViewModel parent;
        
        public SupplementsCycleWeekViewModel(SupplementsCycleDefinitionEditorViewModel parent, SupplementCycleWeekDTO week)
        {
            this.week = week;
            this.parent = parent;
            Dosages = new ObservableCollection<SupplementsCycleEntryViewModel>();
            Image = "pack://application:,,,/BodyArchitect.Client.Module.Suplements;component/Resources/Week16.png";
            foreach (var entry in week.Dosages.OfType<SupplementCycleDosageDTO>())
            {
                Dosages.Add(new SupplementsCycleDosageViewModel(this, entry));
            }
            foreach (var entry in week.Dosages.OfType<SupplementCycleMeasurementDTO>())
            {
                Dosages.Add(new SupplementsCycleEntryViewModel(this, entry));
            }
        }

        public string Comment
        {
            get { return Week.Comment; }
            set
            {
                Week.Comment = value;
                NotifyOfPropertyChange(() => Comment);
                Update();
            }
        }

        public int CycleWeekStart
        {
            get { return Week.CycleWeekStart; }
            set
            {
                Week.CycleWeekStart = value;
                Parent.IsModified = true;
                Update();
            }
        }

        public int CycleWeekEnd
        {
            get { return Week.CycleWeekEnd; }
            set
            {
                Week.CycleWeekEnd = value;
                Parent.IsModified = true;
                Update();
            }
        }


        public override string Header
        {
            get { return string.Format(SuplementsEntryStrings.SupplementsCycleDefinitionEditorViewModel_Header_Weeks, Week.CycleWeekStart, Week.CycleWeekEnd); }
            
        }

        public ObservableCollection<SupplementsCycleEntryViewModel> Dosages { get; private set; }

        public SupplementCycleWeekDTO Week
        {
            get { return week; }
        }

        public SupplementsCycleDefinitionEditorViewModel Parent
        {
            get { return parent; }
        }
    }

    public class SupplementsCycleEntryViewModel : SupplementCycleItemTreeItemViewModel
    {
        private SupplementCycleEntryDTO entry;
        private SupplementsCycleWeekViewModel parentDay;

        public SupplementsCycleEntryViewModel(SupplementsCycleWeekViewModel parentDay, SupplementCycleEntryDTO entry)
        {
            this.parentDay = parentDay;
            this.entry = entry;
            Image = "pack://application:,,,/BodyArchitect.Client.Resources;component/Images/Measurements.png";
            Header = SuplementsEntryStrings.SupplementsCycleDefinitionEditorViewModel_Measurements;
        }

        public override void Update()
        {
            parentDay.Parent.IsModified = true;
        }


        public string Comment
        {
            get { return entry.Comment; }
            set
            {
                entry.Comment = value;
                NotifyOfPropertyChange(() => Comment);
                Update();
            }
        }

        public SupplementCycleDayRepetitions Repetitions
        {
            get { return entry.Repetitions; }
            set
            {
                entry.Repetitions = value;
                Update();
            }
        }

        public TimeType TimeType
        {
            get { return entry.TimeType; }
            set
            {
                entry.TimeType = value;
                Update();
            }
        }

        public SupplementsCycleWeekViewModel ParentWeek
        {
            get { return parentDay; }
        }

        public SupplementCycleEntryDTO Entry
        {
            get { return entry; }
        }
    }

    public class SupplementsCycleDosageViewModel : SupplementsCycleEntryViewModel
    {
        private SupplementCycleDosageDTO entry;

        public SupplementsCycleDosageViewModel(SupplementsCycleWeekViewModel parentDay, SupplementCycleDosageDTO entry):base(parentDay,entry)
        {
            this.entry = entry;
            Update();
        }

        public override void Update()
        {
            
            var exercise = Entry.Supplement;
            if(exercise!=null)
            {
                Header = string.IsNullOrEmpty(Name) ? exercise.Name : Name;
            }
            else
            {
                Header = EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Suplements:SuplementsEntryStrings:SupplementsCycleDefinitionEditorViewModel_Update_SelectSuplement");
            }
            
            if (exercise != null)
            {
                if (exercise.CanBeIllegal)
                {
                    Image = "pack://application:,,,/BodyArchitect.Client.Module.Suplements;component/Resources/IllegalSupplement.gif";    
                }
                else
                {
                    Image = "pack://application:,,,/BodyArchitect.Client.Module.Suplements;component/Resources/LegalSupplement.png";    
                }
                
            }
            else
            {
                Image = "pack://application:,,,/BodyArchitect.Client.Module.Suplements;component/Resources/SupplementWarning16.png";
            }
            base.Update();
        }

        public string Name
        {
            get { return entry.Name; }
            set
            {
                entry.Name = value;
                NotifyOfPropertyChange(() => Name);
                Update();
            }
        }

        public SuplementDTO Supplement
        {
            get { return entry.Supplement; }
            set
            {
                entry.Supplement = value;
                NotifyOfPropertyChange(()=>Supplement);
                Update();
            }
        }

        public DosageType DosageType
        {
            get { return entry.DosageType; }
            set
            {
                entry.DosageType = value;
                NotifyOfPropertyChange(() => DosageType);
                Update();
            }
        }

        public decimal Dosage
        {
            get { return entry.Dosage; }
            set
            {
                entry.Dosage = value;
                NotifyOfPropertyChange(() => DosageType);
                Update();
            }
        }

        public DosageUnit DosageUnit
        {
            get { return entry.DosageUnit; }
            set
            {
                entry.DosageUnit = value;
                NotifyOfPropertyChange(() => DosageType);
                Update();
            }
        }


        public new  SupplementCycleDosageDTO Entry
        {
            get { return entry; }
        }


    }
}
