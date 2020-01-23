using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Service.Model;
using BodyArchitect.Shared;
using DevExpress.XtraEditors.Controls;
using BodyArchitect.Controls;
using BodyArchitect.Controls.SourceGridExtension;
using BodyArchitect.Module.Suplements.Controls.SourceGridExtension;
using BodyArchitect.Module.Suplements.Model;
using SourceGrid;
using SourceGrid.Cells;
using SourceGrid.Selection;
using ComboBoxItem = BodyArchitect.Controls.ComboBoxItem;

namespace BodyArchitect.Module.Suplements.Controls
{
    class SuplementsGrid : SourceGrid.Grid
    {
        private SuplementsEntryDTO entry;
        public const int DeleteButtonColumn = 0;
        public const int SuplementTypeColumn = 1;
        public const int SuplementNameColumn = 2;
        public const int InfoColumn = 3;
        public const int DosageColumn = 4;
        public const int DosageTypeColumn = 5;
        public const int TimeColumn = 6;
        public const int StandardColumnNumber = 7;
        private CommentableCellValueChangedController commentableCellController;

        public SuplementsGrid()
        {
            Selection.SelectionChanged += new RangeRegionChangedEventHandler(Selection_SelectionChanged);
            commentableCellController = new CommentableCellValueChangedController();
            SelectionMode = GridSelectionMode.Row;
            ClipboardUseOnlyActivePosition = true;
            ClipboardMode = ((SourceGrid.ClipboardMode)((((SourceGrid.ClipboardMode.Copy | SourceGrid.ClipboardMode.Cut)
                        | SourceGrid.ClipboardMode.Paste)
                        | SourceGrid.ClipboardMode.Delete)));
        }

        void Selection_SelectionChanged(object sender, RangeRegionChangedEventArgs e)
        {
            
        }
        public void Fill(SuplementsEntryDTO entry)
        {
            this.entry = entry;
            refreshGridLayout();
        }

        public GridRow SelectedRow
        {
            get
            {
                RowSelection rowSelection = (RowSelection)Selection;
                var region = rowSelection.GetSelectionRegion();
                if (region.Count > 0)
                {
                    return Rows[region[0].Start.Row];
                }
                return null;
            }
        }

        public bool ReadOnly
        {
            get; set;
        }


        private void createHeader()
        {
            Redim(1, StandardColumnNumber);
            this[0, DeleteButtonColumn] = new SourceGrid.Cells.ColumnHeader();
            this[0, DeleteButtonColumn].Column.Width = 20;
            var ttt = new SourceGrid.Cells.ColumnHeader(SuplementsEntryStrings.SuplementGrid_SuplementColumn);
            this[0, SuplementTypeColumn] = ttt;
            ttt.Column.Width = 200;
            ttt.Column.AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize | SourceGrid.AutoSizeMode.EnableStretch;
            this[0, SuplementNameColumn] = new SourceGrid.Cells.ColumnHeader(SuplementsEntryStrings.SuplementGrid_NameColumn);
            this[0, InfoColumn] = new SourceGrid.Cells.ColumnHeader(SuplementsEntryStrings.SuplementGrid_InfoColumn);
            this[0, DosageTypeColumn] = new SourceGrid.Cells.ColumnHeader(SuplementsEntryStrings.SuplementGrid_DosageTypeColumn);
            this[0, DosageColumn] = new SourceGrid.Cells.ColumnHeader(SuplementsEntryStrings.SuplementGrid_DosageColumn);
            this[0, TimeColumn] = new SourceGrid.Cells.ColumnHeader(SuplementsEntryStrings.SuplementGrid_TimeColumn);
        }

        private void refreshGridLayout()
        {
            if (entry == null)
            {
                return;
            }
            Rows.Clear();
            createHeader();
            for (int i = 0; i < entry.Items.Count; i++)
            {
                int rowIndex = i + 1;
                AddEmptyRow();
                GridRow row = Rows[rowIndex];
                var item = entry.Items[i];
                row.Tag = item;
                Cell cell = (Cell)GetCell(rowIndex, SuplementTypeColumn);
                cell.Value = item.SuplementId;
                cell = (Cell)GetCell(rowIndex, InfoColumn);
                cell.Value = item.Comment;
                cell.ToolTipText = item.Comment;
                cell = (Cell)GetCell(rowIndex, SuplementNameColumn);
                cell.Value = item.Name;
                cell.ToolTipText = item.Comment;
                cell = (Cell)GetCell(rowIndex, DosageColumn);
                cell.Value = item.Dosage.ToString();
                cell = (Cell)GetCell(rowIndex, DosageTypeColumn);

                var selectedItem = from t in ((SourceGrid.Cells.Editors.ComboBox)cell.Editor).Control.Items.Cast<ComboBoxItem>() where (DosageType)t.Tag == item.DosageType select t;
                cell.Value = selectedItem.SingleOrDefault();
                cell = (Cell)GetCell(rowIndex, TimeColumn);
                cell.Value = item.Time;
                UpdateCellsReadOnlyMode(row);
            }
            if(!ReadOnly)
            {
                AddEmptyRow();    
            }
            
        }

        public void AddEmptyRow()
        {
            int newRowIndex = Rows.Count;
            Rows.Insert(newRowIndex);

            SuplementTypeCellValueChangedController suplementController = new SuplementTypeCellValueChangedController(this,entry);
            var lookupEditor = new LookupEditEditor(typeof(Guid));

            this[newRowIndex, SuplementTypeColumn] = new SourceGrid.Cells.Cell(null, lookupEditor);
            SourceGrid.Cells.Views.Cell readOnlyView = new SourceGrid.Cells.Views.Cell();
            this[newRowIndex, SuplementTypeColumn].View = readOnlyView;
            this[newRowIndex, SuplementTypeColumn].View.ForeColor = ApplicationColors.FGNullText;
            this[newRowIndex, SuplementTypeColumn].AddController(suplementController);
            this[newRowIndex, SuplementTypeColumn].Editor.NullDisplayString = SuplementsEntryStrings.SelectSuplementType;

            readOnlyView = new SourceGrid.Cells.Views.Cell();
            var textBox = new SourceGrid.Cells.Editors.TextBox(typeof (string));
            textBox.Control.MaxLength = Constants.NameColumnLength;
            this[newRowIndex, SuplementNameColumn] = new SourceGrid.Cells.Cell(null, textBox);
            this[newRowIndex, SuplementNameColumn].View = readOnlyView;
            readOnlyView = new SourceGrid.Cells.Views.Cell();
            ////grid1[newRowIndex, 1].AddController(ee);
            this[newRowIndex, InfoColumn] = new SourceGrid.Cells.Cell(null, new MemoExEditEditor());
            this[newRowIndex, InfoColumn].View = readOnlyView;
            this[newRowIndex, InfoColumn].AddController(commentableCellController);
            this[newRowIndex, InfoColumn].Column.Width = 80;

            var maskEditor = new MaskEditEditor();
            maskEditor.MaskRegEx = string.Format("[0-9]*{0}?[0-9]*", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            this[newRowIndex, DosageColumn] = new SourceGrid.Cells.Cell(null, maskEditor);
            this[newRowIndex, DosageColumn].View = readOnlyView;

            SourceGrid.Cells.Editors.ComboBox cbEditor = new SourceGrid.Cells.Editors.ComboBox(typeof(ComboBoxItem));
            //cbEditor.StandardValues = Enum.GetValues(typeof (DosageType));
            foreach (DosageType type in Enum.GetValues(typeof(DosageType)))
            {
                cbEditor.Control.Items.Add(new BodyArchitect.Controls.ComboBoxItem(type,EnumLocalizer.Default.Translate(type)));
            }
            
            cbEditor.Control.DropDownStyle = ComboBoxStyle.DropDownList;

            this[newRowIndex, DosageTypeColumn] = new SourceGrid.Cells.Cell(null, cbEditor);
            this[newRowIndex, DosageTypeColumn].View = readOnlyView;
            this[newRowIndex, DosageTypeColumn].Column.Width = 80;

            var timeEditor = new TimeEditEditor();
            this[newRowIndex, TimeColumn] = new SourceGrid.Cells.Cell(null, timeEditor);
            this[newRowIndex, TimeColumn].View = new TimeEditView();

            UpdateCellsReadOnlyMode(Rows[newRowIndex]);
        }

        public List<SuplementItemDTO> GetSuplementItems()
        {
            List<SuplementItemDTO> entries = new List<SuplementItemDTO>();
            foreach (var gridViewRow in Rows)
            {
                if (gridViewRow.Tag == null)
                {
                    continue;
                }
                SuplementItemDTO entry = (SuplementItemDTO)gridViewRow.Tag;
                Cell cell = (Cell)GetCell(gridViewRow.Index, SuplementTypeColumn);
                entry.SuplementId = (Guid)cell.Value;
                //entry.SuplementId = ((SuplementDTO)((LookupEditEditor)cell.Editor).SelectedItem).SuplementId;
                cell = (Cell)GetCell(gridViewRow.Index, InfoColumn);
                entry.Comment = (string)cell.Value;
                cell = (Cell)GetCell(gridViewRow.Index, SuplementNameColumn);
                entry.Name = (string)cell.Value;
                cell = (Cell)GetCell(gridViewRow.Index, DosageColumn);
                double tempDosage = 0;
                if (cell.Value!=null && double.TryParse(cell.Value.ToString(), out tempDosage))
                {
                }
                entry.Dosage = tempDosage;
                cell = (Cell)GetCell(gridViewRow.Index, DosageTypeColumn);
                entry.DosageType = (DosageType)((ComboBoxItem)cell.Value).Tag;
                cell = (Cell)GetCell(gridViewRow.Index, TimeColumn);
                if (cell.Value != null)
                {
                    entry.Time = DateHelper.MoveToNewDate((DateTime) cell.Value, this.entry.TrainingDay.TrainingDate);
                }
                else
                {
                    entry.Time = this.entry.TrainingDay.TrainingDate.Date;
                }
                entries.Add(entry);
            }

            return entries;
        }

        public void UpdateCellsReadOnlyMode(GridRow row)
        {

            var entry = row.Tag as SuplementItemDTO;
            GetCell(row.Index, SuplementTypeColumn).Editor.EditableMode = ReadOnly ? EditableMode.None : EditableMode.SingleClick;
            GetCell(row.Index, SuplementTypeColumn).Editor.EnableEdit = !ReadOnly;

            setCellReadOnly(GetCell(row.Index, InfoColumn), entry == null);
            setCellReadOnly(GetCell(row.Index, DosageTypeColumn), entry == null);
            setCellReadOnly(GetCell(row.Index, DosageColumn), entry == null);
            setCellReadOnly(GetCell(row.Index, SuplementNameColumn), entry == null);
            setCellReadOnly(GetCell(row.Index, TimeColumn), entry == null);
            this.Refresh();
        }

        void setCellReadOnly(ICellVirtual vCell, bool readOnly)
        {
            Cell cell = (Cell)vCell;
            cell.Editor.EditableMode = ReadOnly || readOnly ? EditableMode.None : EditableMode.SingleClick;
            cell.Editor.EnableEdit = !ReadOnly && !readOnly;
            cell.View.BackColor = readOnly ? ApplicationColors.BGReadOnlyCell : ApplicationColors.BGStandardCell;
        }
    }
}
