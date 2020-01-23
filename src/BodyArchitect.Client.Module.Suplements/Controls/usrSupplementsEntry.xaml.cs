using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using AvalonDock.Layout;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.Suplements.Controls
{
    /// <summary>
    /// Interaction logic for usrSupplementsEntry.xaml
    /// </summary>
    public partial class usrSupplementsEntry : IEntryObjectControl
    {
        private SupplementsGridViewModel viewModel;

        public usrSupplementsEntry()
        {
            InitializeComponent();
        }

        public override bool HasDetailsPane
        {
            get { return true; }
        }

        public SuplementsEntryDTO SupplementsEntry
        {
            get { return (SuplementsEntryDTO) Entry; }
        }

        protected override UI.UserControls.usrEntryObjectDetailsBase CreateDetailsControl()
        {
            return new usrSupplementsEntryDetails();
        }

        protected override void FillImplementation(SaveTrainingDayResult originalEntry)
        {
            viewModel = new SupplementsGridViewModel(SupplementsEntry, ReadOnly);
            DataContext = viewModel;
            updateReadOnly();
        }

        protected override void UpdateEntryObjectImplementation()
        {
            viewModel.ApplyChanges(SupplementsEntry);
        }
        
        void updateReadOnly()
        {
            grid.IsReadOnly = ReadOnly;
            if (detailsControl != null)
            {
                detailsControl.UpdateReadOnly(ReadOnly);
            }
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button) sender;
            var item = btn.Tag as SupplementItemViewModel;
            if (item != null)
            {
                viewModel.Items.Remove(item);
                SetModifiedFlag();
                //grid.RefreshItems();
                //grid.CanUserAddRows = false;
                //grid.CanUserAddRows = true;
            }
        }

        private void grid_CurrentCellChanged(object sender, EventArgs e)
        {
            grid.CommitEdit();
            SetModifiedFlag();
        }
      

        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;
            BADataGrid.GridColumnFastEdit(cell, e);
        }

        private void DataGridCell_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;
            BADataGrid.GridColumnFastEdit(cell, e);
        }

        private void grid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            viewModel.EnsureNewRowAdded();
        }

        




        //private void grid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        //{
        //    //if(e.EditAction==DataGridEditAction .Commit)
        //    //{
        //    //    bool hasError = false;
        //    //    foreach (var item in viewModel.Items)
        //    //    {
        //    //        if(item.SuplementId==Guid.Empty)
        //    //        {
        //    //            e.Cancel = true;
        //    //            return;
        //    //            hasError = true;
        //    //        }
        //    //    }
        //    //    //grid.Items.DeferRefresh();
        //    //}
        //}
    }


    

    //public class RowDataInfoValidationRule : ValidationRule
    //{
    //    public override ValidationResult Validate(object value,
    //                    CultureInfo cultureInfo)
    //    {
    //        BindingGroup group = (BindingGroup)value;

    //        StringBuilder error = null;
    //        foreach (var item in group.Items)
    //        {
    //            // aggregate errors
    //            IDataErrorInfo info = item as IDataErrorInfo;
    //            if (info != null)
    //            {
    //                if (!string.IsNullOrEmpty(info.Error))
    //                {
    //                    if (error == null)
    //                        error = new StringBuilder();
    //                    error.Append((error.Length != 0 ? ", " : "") + info.Error);
    //                }
    //            }
    //        }

    //        if (error != null)
    //            return new ValidationResult(false, error.ToString());

    //        //return ValidationResult.ValidResult;
    //        return new ValidationResult(true, "");
    //    }
    //}

    

}
