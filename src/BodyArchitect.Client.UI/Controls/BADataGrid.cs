using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using BodyArchitect.Settings;
using Xceed.Wpf.Toolkit;

namespace BodyArchitect.Client.UI.Controls
{
    public class BADataGrid : DataGrid
    {

        public BADataGrid()
        {

            PreparingCellForEdit += new EventHandler<DataGridPreparingCellForEditEventArgs>(StrengthTrainingDataGrid_PreparingCellForEdit);
        }

        void StrengthTrainingDataGrid_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            if (!GuiState.Default.SmartEditingDataGrid)
            {
                return;
            }
            ContentPresenter cp = e.EditingElement as ContentPresenter;

            if (cp != null)
            {
                UIElement destinationTextBox = VisualTreeHelper.GetChild(cp, 0) as UIElement;
                if (destinationTextBox is ContentPresenter)
                {
                    destinationTextBox = VisualTreeHelper.GetChild(destinationTextBox, 0) as UIElement;
                }
                if (destinationTextBox != null)
                {
                    destinationTextBox.Focus();

                }
                if(destinationTextBox is Panel)
                {
                    destinationTextBox = UIHelper.GetVisualChild<TextBox>(destinationTextBox);
                }

                MultiLineTextEditor multi = destinationTextBox as MultiLineTextEditor;
                if (multi != null)
                {
                    multi.IsOpen = true;
                }
                TextBox tb = destinationTextBox as TextBox;
                if (tb != null)
                {
                    tb.SelectAll();
                    tb.Focus();
                }
            }
        }

        public  static void GridColumnFastEdit(DataGridCell cell, RoutedEventArgs e)
        {
            if (cell == null || cell.IsEditing || cell.IsReadOnly || !GuiState.Default.SmartEditingDataGrid)
                return;

            var dataGrid = UIHelper.FindVisualParent<BADataGrid>(cell);
            if (dataGrid == null)
                return;

            dataGrid.EnsureNotEditMode();

            try
            {
                if (!cell.IsFocused)
                {
                    cell.Focus();
                    var info = new DataGridCellInfo(dataGrid.SelectedItem, cell.Column);

                    dataGrid.SelectedCells.Clear();
                    dataGrid.SelectedCells.Add(info);
                    dataGrid.CurrentCell = info;

                }


                if (cell.Content is CheckBox)
                {
                    if (dataGrid.SelectionUnit != DataGridSelectionUnit.FullRow)
                    {
                        if (!cell.IsSelected)
                            cell.IsSelected = true;
                    }
                    else
                    {
                        DataGridRow row = UIHelper.FindVisualParent<DataGridRow>(cell);
                        if (row != null && !row.IsSelected)
                        {
                            row.IsSelected = true;
                        }
                    }
                }
                else
                {

                    dataGrid.BeginEdit(e);
                    ComboBox cb = cell.Content as ComboBox;
                    if (cb != null)
                    {
                        cell.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                        cb.IsDropDownOpen = true;
                    }
                }
            }
            catch (Exception)
            {
                //sometimes this throws exception but except this everything is ok
            }
            
        }


        public void RefreshItems()
        {
            //without Commit and Cancel sometimes we get exception
            EnsureNotEditMode();
            try
            {
                Items.Refresh();
            }
            catch (Exception)
            {
                
            }
            
        }

        public void EnsureNotEditMode()
        {
            try
            {
                CommitEdit(DataGridEditingUnit.Row, true);
            }
            catch (Exception)
            {//sometimes this throws exception but except this everything is ok
            }
            
            CancelEdit();
        }

        public DataGridCell GetCell(DataGridRow row, int column)
        {
            if (row != null)
            {
                DataGridCellsPresenter presenter =UIHelper.GetVisualChild<DataGridCellsPresenter>(row);

                if (presenter == null)
                {
                    ScrollIntoView(row, Columns[column]);
                    presenter = UIHelper.GetVisualChild<DataGridCellsPresenter>(row);
                }

                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                return cell;
            }
            return null;
        }

        public DataGridRow GetSelectedRow()
        {
            return (DataGridRow)ItemContainerGenerator.ContainerFromItem(SelectedItem);
        }
    }
}
