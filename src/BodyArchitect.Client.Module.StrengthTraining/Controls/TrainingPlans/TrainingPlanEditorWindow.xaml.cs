using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.StrengthTraining.Localization;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using ValidationResult = Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls.TrainingPlans
{
    [Serializable]
    public class ItemContext<T>:PageContext
    {
        private T plan;

        public ItemContext(T plan)
        {
            this.plan = plan;
        }

        public T Item
        {
            get { return plan; }
            set { plan = value; }
        }
    }
    /// <summary>
    /// Interaction logic for TrainingPlanEditorWindow.xaml
    /// </summary>
    public partial class TrainingPlanEditorWindow 
    {
        private TrainingPlanViewModel viewModel;

        public TrainingPlanEditorWindow()
        {
            InitializeComponent();
            viewModel = new TrainingPlanViewModel();

            Binding binding = new Binding("Header");
            binding.Mode = BindingMode.OneWay;
            SetBinding(HeaderProperty, binding);

            binding = new Binding("IsModified");
            binding.Mode = BindingMode.OneWay;
            SetBinding(IsModifiedProperty, binding);

            DataContext = viewModel;
            //usrTrainingPlanSerieEditor1.TrainingPlanSetChanged += new EventHandler<ParameterEventArgs<TrainingPlanSerie>>(usrTrainingPlanSerieEditor1_TrainingPlanSetChanged);
            this.UsrTrainingPlanEntryEditor1.TrainingPlanChanged += new System.EventHandler<ParameterEventArgs<TrainingPlanEntry>>(this.UsrTrainingPlanEntryEditor1_TrainingPlanChanged);
            
            updateDetailsControl();

            foreach (WorkoutPlanPurpose purpose in Enum.GetValues(typeof(WorkoutPlanPurpose)))
            {
                cmbPurpose.Items.Add(new ListItem<WorkoutPlanPurpose>(EnumLocalizer.Default.Translate(purpose), purpose));
            }

            foreach (TrainingPlanDifficult diff in Enum.GetValues(typeof(TrainingPlanDifficult)))
            {

                cmbDifficult.Items.Add(new ListItem<TrainingPlanDifficult>(EnumLocalizer.Default.Translate(diff), diff));
            }

            var types = (TrainingType[])Enum.GetValues(typeof(TrainingType));
            cmbTypes.ItemsSource = types;

            this.cmbLanguages.ItemsSource = BodyArchitect.Service.V2.Model.Language.Languages;
        }


        //void fillSuperTips()
        //{
        //    ControlHelper.AddSuperTip(this.txtComment, grDescription.Text, StrengthTrainingEntryStrings.TrainingPlanEditor_DescriptionTXT);
        //    ControlHelper.AddSuperTip(this.txtName, lblName.Text, StrengthTrainingEntryStrings.TrainingPlanEditor_NameTXT);
        //    ControlHelper.AddSuperTip(this.txtAuthor, lblAuthor.Text, StrengthTrainingEntryStrings.TrainingPlanEditor_AuthorTXT);
        //    ControlHelper.AddSuperTip(this.cmbTrainingType, lblTrainingType.Text, StrengthTrainingEntryStrings.TrainingPlanEditor_TrainingTypeCMB);
        //    ControlHelper.AddSuperTip(this.cmbDifficult, lblDifficult.Text, StrengthTrainingEntryStrings.TrainingPlanEditor_DifficultCMB);
        //    ControlHelper.AddSuperTip(this.txtRestSeconds, lblRestSeconds.Text, StrengthTrainingEntryStrings.TrainingPlanEditor_RestTimeTXT);
        //    ControlHelper.AddSuperTip(this.btnValidate, btnValidate.Text, StrengthTrainingEntryStrings.TrainingPlanEditor_ValidateBTN);
        //    ControlHelper.AddSuperTip(this.btnSuperSets, btnSuperSets.Text, StrengthTrainingEntryStrings.TrainingPlanEditor_SuperSetsBTN);
        //}

        public ItemContext<TrainingPlan> Context
        {
            get { return (ItemContext<TrainingPlan>)PageContext; }
        }


        public TrainingPlan TrainingPlan { get { return Context.Item; } }

        //private void tvDetails_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        //{
        //    if (!(e.Node.Tag is TrainingPlanDay))
        //    {
        //        e.CancelEdit = true;
        //        return;
        //    }

        //}

        //private void tvDetails_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        //{
        //    if (e.Label != null)
        //    {
        //        if ((e.Node.Tag is TrainingPlanDay))
        //        {
        //            TrainingPlanDay day = (TrainingPlanDay)e.Node.Tag;
        //            day.Name = e.Label;
        //            return;
        //        }
        //    }
        //}


        private void updateDetailsControl()
        {
            var selectedSet = trainingPlanTreeView1.GetSelected<TrainingPlanSetViewModel>();
            var selectedEntry = trainingPlanTreeView1.GetSelected<TrainingPlanEntryViewModel>();
            usrTrainingPlanSerieEditor1.SetVisible(selectedSet != null);
            usrTrainingPlanSerieEditor1.Fill(selectedSet,viewModel);

            UsrTrainingPlanEntryEditor1.SetVisible(selectedEntry != null);
            UsrTrainingPlanEntryEditor1.Fill(selectedEntry != null ? selectedEntry : null, viewModel);
        }

        private void UsrTrainingPlanEntryEditor1_TrainingPlanChanged(object sender, ParameterEventArgs<TrainingPlanEntry> e)
        {
            var entry = trainingPlanTreeView1.GetSelected<TrainingPlanEntryViewModel>();
            if (entry == null)
            {
                return;
            }
            entry.Update();
        }

        private void btnSuperSets_Click(object sender, RoutedEventArgs e)
        {
            TrainingPlanSuperSetsEditor wnd = new TrainingPlanSuperSetsEditor();
            wnd.Fill(Context.Item);
            if (wnd.ShowDialog() ==true)
            {
                viewModel.SetModifiedFlag();
            }
        }


        private void btnValidate_Click(object sender, RoutedEventArgs e)
        {
            lvOutput.Items.Clear();

            var validator = new ObjectValidator(typeof(TrainingPlan));
            var result1 = validator.Validate(Context.Item);

            if (!result1.IsValid)
            {
                foreach (var message in result1)
                {
                    addOutputItem((BAGlobalObject) (message.Target != null ? message.Target : Context.Item),message.Message,TrainingPlanCheckItemStatus.Error);
                }
                return;
            }

            var pack = TrainingPlanPack.Create(TrainingPlan, ExercisesReposidory.Instance.Items);
            SplitPlanChecker checker = new SplitPlanChecker(pack);
            var result = checker.Check(ExercisesReposidory.Instance.Items);
            
            foreach (var resultItem in result.Results)
            {
                addOutputItem(resultItem.TrainingPlanBase,StrengthTrainingEntryStrings.ResourceManager.GetString(resultItem.ResourceKey),resultItem.Status);
            }
        }

        private void addOutputItem(BAGlobalObject obj,string message,TrainingPlanCheckItemStatus status)
        {
            OutputItem outputItem = new OutputItem();
            outputItem.Icon = getIcon(status);
            outputItem.Message = message;
            fillNameAndType(obj, outputItem);
            lvOutput.Items.Add(outputItem);
        }

        private bool ensurePlanCorrect(TrainingPlan plan)
        {
            var validator = new ObjectValidator(typeof(TrainingPlan));
            var result1 = validator.Validate(plan);

            if (!result1.IsValid)
            {
                var errorElement = epError.GetFirstInvalidElement();
                if (errorElement != null)
                {
                    errorElement.Focus();
                }

                BAMessageBox.ShowValidationError(result1.ToBAResults());
                return false;
            }
            return true;
        }

        string getIcon(TrainingPlanCheckItemStatus status)
        {
            switch (status)
            {
                    case TrainingPlanCheckItemStatus.Information:
                    return "Information.png".ToResourceString();
                    case TrainingPlanCheckItemStatus.Error:
                    return "HasError.png".ToResourceString();

            }
            return "Warning.png".ToResourceString();
        }


        private void fillNameAndType(BAGlobalObject planBase, OutputItem checkItem)
        {
            TrainingPlan plan = planBase as TrainingPlan;
            TrainingPlanEntry entry = planBase as TrainingPlanEntry;
            TrainingPlanDay day = planBase as TrainingPlanDay;
            TrainingPlanSerie set = planBase as TrainingPlanSerie;

            checkItem.Item = planBase;

            if (plan != null)
            {
                checkItem.Object = plan.Name;
                checkItem.Type = StrengthTrainingEntryStrings.TrainingPlan_Object;
            }
            else if (entry != null)
            {
                checkItem.Object = entry.Exercise!=null?entry.Exercise.GetLocalizedName():"";
                checkItem.Type = StrengthTrainingEntryStrings.TrainingPlanDayEntry_Object;
            }
            if (day != null)
            {
                checkItem.Object = day.Name;
                checkItem.Type = StrengthTrainingEntryStrings.TrainingPlanDay_Object;
            }
            if (set != null)
            {
                checkItem.Object = set.ToString();
                checkItem.Type = StrengthTrainingEntryStrings.TrainingPlanSerie_Object;
            }
        }


        void selectTrainingPlanBase(BAGlobalObject planBase)
        {
            viewModel.SelectedItem = selectProcess(viewModel, planBase);
        }

        TrainingPlanTreeItemViewModel selectProcess(TrainingPlanViewModel mainViewModel, BAGlobalObject planBase)
        {
            foreach (TrainingPlanDayViewModel node in mainViewModel.Days)
            {
                if (node.Item == planBase)
                {
                    return node;
                }
                foreach (var entryViewModel in node.Entries)
                {
                    if (entryViewModel.Item == planBase)
                    {
                        return entryViewModel;
                    }
                    foreach (var setViewModel in entryViewModel.Sets)
                    {
                        if (setViewModel.Item == planBase)
                        {
                            return setViewModel;
                        }
                    }
                }

            }
            return null;
        }


        //private void BaseWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    if (DialogResult!= true && BAMessageBox.AskYesNo(StrengthTrainingEntryStrings.QWorkoutPlanEditorCloseWindow) == MessageBoxResult.No)
        //    {
        //        e.Cancel = true;
        //    }
        //}

        private void trainingPlanTreeView1_SelectedItemChanged(object sender, EventArgs e)
        {
            updateDetailsControl();
        }

        private void lvOutput_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = (OutputItem)lvOutput.GetClickedItem(e);
            if (item != null)
            {
                selectTrainingPlanBase(item.Item);
            }
        }

        #region Implementation of IControlView

        public override void Fill()
        {
            NotifyOfPropertyChange(null);
            viewModel.Fill(Context.Item);
            trainingPlanTreeView1.FillDetailsTree(viewModel);
        }

        public override void RefreshView()
        {
            Fill();
        }

        public override Uri HeaderIcon
        {
            get
            {
                return new Uri("pack://application:,,,/BodyArchitect.Client.Module.StrengthTraining;component/Images/TrainingPlanEditor16.png", UriKind.Absolute);
            }
        }

        #endregion

        private void rbtnSave_Click(object sender, RoutedEventArgs e)
        {
            var context = Context;
            context.Item.Profile = UserContext.Current.CurrentProfile;

            if (!ensurePlanCorrect(context.Item))
            {
                return;
            }
            
            PleaseWait.Run(delegate
            {

                try
                {
                    TrainingPlanChecker checker = new TrainingPlanChecker();
                    checker.Process(context.Item);
                    
                    context.Item = ServiceManager.SaveTrainingPlan(context.Item);
                    WorkoutPlansReposidory.Instance.AddOrUpdate(context.Item);

                    Dispatcher.BeginInvoke(new Action(Fill));
                }
                catch (Exception ex)
                {
                    Dispatcher.BeginInvoke(new Action(() => ExceptionHandler.Default.Process(ex, StrengthTrainingEntryStrings.TrainingPlanEditorWindow_ErrSaveWorkoutPlan, ErrorWindow.EMailReport)));
                }

            });
        }

        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            viewModel.SetModifiedFlag();
        }

        

        private void txtRestTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            viewModel.SetModifiedFlag();
        }

        private void cmbTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewModel.SetModifiedFlag();
        }
    }

    public class OutputItem
    {
        public string Message { get; set; }

        public string Object { get; set; }

        public string Type { get; set; }

        public string Icon { get; set; }

        public BAGlobalObject Item { get; set; }
    }
}
