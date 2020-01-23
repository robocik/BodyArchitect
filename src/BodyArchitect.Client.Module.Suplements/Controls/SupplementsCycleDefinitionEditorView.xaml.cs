using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Suplements.Controls
{
    [Serializable]
    public class SupplementsCycleDefinitionContext:PageContext
    {
        public SupplementCycleDefinitionDTO CycleDefinition { get; set; }

        public SupplementsCycleDefinitionContext(SupplementCycleDefinitionDTO cycleDefinition)
        {
            CycleDefinition = cycleDefinition;
        }
    }

    public partial class SupplementsCycleDefinitionEditorView
    {
        private SupplementsCycleDefinitionEditorViewModel viewModel;

        public SupplementsCycleDefinitionEditorView()
        {
            InitializeComponent();
            viewModel = new SupplementsCycleDefinitionEditorViewModel();
            var binding = new Binding("IsModified");
            binding.Mode = BindingMode.OneWay;
            SetBinding(IsModifiedProperty, binding);

            binding = new Binding("Header");
            binding.Mode = BindingMode.OneWay;
            SetBinding(HeaderProperty, binding);

            DataContext = viewModel;
            
        }

        public SupplementsCycleDefinitionContext Context
        {
            get { return (SupplementsCycleDefinitionContext) PageContext; }
        }

        public override void Fill()
        {
            viewModel.Fill(ParentWindow, Context);
        }
        
        public override void RefreshView()
        {
            Fill();
        }

        public override Uri HeaderIcon
        {
            get { return new Uri("pack://application:,,,/BodyArchitect.Client.Module.Suplements;component/Resources/CycleDefinitionEditor16.png", UriKind.Absolute); }
        }

        private void rbtnSave_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Save();
        }

        private void tvDetails_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            usrDosageEditor.Clear();
            var item = tvDetails.SelectedItem;

            viewModel.SelectedItem = item as SupplementCycleItemTreeItemViewModel;
            
        }

        private void rbtnAddWeek_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AddWeek();
        }

        private void rbtnAddDosage_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AddDosage();
        }

        private void rbtnDelete_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Delete();
        }

        private void rbtnAddMeasurements_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AddMeasurements();
        }
    }
}
