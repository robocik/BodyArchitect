using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Controls;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Module.StrengthTraining.Controls.SourceGridExtension;
using BodyArchitect.Module.StrengthTraining.Localization;
using BodyArchitect.Service.Model;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using SourceGrid;
using SourceGrid.Cells;
using SourceGrid.Selection;
using ComboBoxItem = BodyArchitect.Controls.ComboBoxItem;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    public partial class ExerciseMapperWindow :BaseWindow
    {
        public const int ColumnsNumber = 4;
        public const int DeleteColumn = 0;
        public const int FromExerciseColumn = 1;
        public const int ToExerciseColumn = 2;
        public const int OperationColumn = 3;
        private MapperGridExerciseController mapperCtrl;
        private MapperData data;

        public ExerciseMapperWindow()
        {
            InitializeComponent();

            fillExerciseCombobox();
            mapperCtrl=new MapperGridExerciseController(this);
            usrProgressIndicatorButtons1.OkButton.Enabled = false;
            this.cmbExerciseViewType.SelectedIndexChanged += new System.EventHandler(this.cmbExerciseViewType_SelectedIndexChanged);
        }

        public void Fill()
        {
            data=new MapperData();
            refreshGrid();
        }

        private void refreshGrid()
        {
            grid1.Rows.Clear();
            createGridHeader();
            for (int index = 0; index < MapperData.Entries.Count; index++)
            {
                int rowIndex = index + 1;
                
                AddEmptyRow();

                GridRow row = grid1.Rows[rowIndex];
                var entry = MapperData.Entries[index];
                row.Tag = entry;
                Cell cell = (Cell)grid1.GetCell(rowIndex, FromExerciseColumn);
                cell.Value = entry.FromExerciseId;
                cell = (Cell)grid1.GetCell(rowIndex, ToExerciseColumn);
                cell.Value = entry.ToExerciseId;
                cell = (Cell)grid1.GetCell(rowIndex, OperationColumn);
                cell.Value = entry.Operation;

                UpdateCellsReadOnlyMode(row);
            }
            AddEmptyRow();
        }

        private void fillExerciseCombobox()
        {
            cmbExerciseViewType.Properties.Items.Clear();
            foreach (var lookUpColumnInfo in  ExerciseLookUp.CreateColumns())
            {
                cmbExerciseViewType.Properties.Items.Add(new ComboBoxItem(lookUpColumnInfo,lookUpColumnInfo.Caption));
            }
            cmbExerciseViewType.SelectedIndex = UserContext.Settings.ExerciseComboBoxSelectedItem;
        }

        private void createGridHeader()
        {
            grid1.Redim(1, ColumnsNumber);
            grid1[0, DeleteColumn] = new SourceGrid.Cells.ColumnHeader();
            grid1[0, DeleteColumn].Column.Width = 20;
            var ttt = new SourceGrid.Cells.ColumnHeader(StrengthTrainingEntryStrings.ExerciseMapperWindow_ColumnFrom);
            grid1[0, FromExerciseColumn] = ttt;
            ttt.Column.AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize | SourceGrid.AutoSizeMode.EnableStretch;
            grid1[0, ToExerciseColumn] = new SourceGrid.Cells.ColumnHeader(StrengthTrainingEntryStrings.ExerciseMapperWindow_ColumnTo);
            grid1[0, ToExerciseColumn].Column.AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize | SourceGrid.AutoSizeMode.EnableStretch;

            grid1[0, OperationColumn] = new SourceGrid.Cells.ColumnHeader(StrengthTrainingEntryStrings.ExerciseMapperWindow_ColumnOperation);
        }

        public void AddEmptyRow()
        {
           int newRowIndex = grid1.Rows.Count;
            grid1.Rows.Insert(newRowIndex);

            
            var lookupEditor = new LookupEditEditor(typeof(Guid));
            LookUpColumnInfo selectedCol = (LookUpColumnInfo)((ComboBoxItem)cmbExerciseViewType.SelectedItem).Tag;
            lookupEditor.Control.SetDisplayColumn(selectedCol);

            grid1[newRowIndex, FromExerciseColumn] = new SourceGrid.Cells.Cell(null, lookupEditor);
            SourceGrid.Cells.Views.Cell readOnlyView = new SourceGrid.Cells.Views.Cell();
            grid1[newRowIndex, FromExerciseColumn].View = readOnlyView;
            grid1[newRowIndex, FromExerciseColumn].View.ForeColor = ApplicationColors.FGNullText;
            grid1[newRowIndex, FromExerciseColumn].AddController(mapperCtrl);
            grid1[newRowIndex, FromExerciseColumn].Editor.NullDisplayString = StrengthTrainingEntryStrings.SelectExercise;
            grid1[newRowIndex, FromExerciseColumn].Column.Width = 250;

            readOnlyView = new SourceGrid.Cells.Views.Cell();
            lookupEditor = new LookupEditEditor(typeof(Guid));
            lookupEditor.Control.SetDisplayColumn(selectedCol);

            grid1[newRowIndex, ToExerciseColumn] = new SourceGrid.Cells.Cell(null, lookupEditor);
            readOnlyView = new SourceGrid.Cells.Views.Cell();
            grid1[newRowIndex, ToExerciseColumn].View = readOnlyView;
            grid1[newRowIndex, ToExerciseColumn].View.ForeColor = ApplicationColors.FGNullText;
            grid1[newRowIndex, ToExerciseColumn].AddController(mapperCtrl);
            grid1[newRowIndex, ToExerciseColumn].Editor.NullDisplayString = StrengthTrainingEntryStrings.SelectExercise;
            grid1[newRowIndex, ToExerciseColumn].Column.Width = 250;


            SourceGrid.Cells.Editors.ComboBox cbEditor = new SourceGrid.Cells.Editors.ComboBox(typeof(ComboBoxItem));
            
            //cbEditor.StandardValues = Enum.GetValues(typeof(MapperEntryOperation));
            foreach (MapperEntryOperation type in Enum.GetValues(typeof(MapperEntryOperation)))
            {
                cbEditor.Control.Items.Add(new ComboBoxItem(type, EnumLocalizer.Default.Translate(type)));
            }
            cbEditor.Control.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEditor.Control.SelectedIndex = 0;

            readOnlyView = new SourceGrid.Cells.Views.Cell();
            grid1[newRowIndex, OperationColumn] = new SourceGrid.Cells.Cell(null, cbEditor);
            grid1[newRowIndex, OperationColumn].View = readOnlyView;
            grid1[newRowIndex, OperationColumn].Column.Width = 80;
            UpdateCellsReadOnlyMode(grid1.Rows[newRowIndex]);
        }

        void setCellReadOnly(ICellVirtual vCell, bool readOnly)
        {
            Cell cell = (Cell)vCell;
            cell.Editor.EditableMode = readOnly ? EditableMode.None : EditableMode.SingleClick;
            cell.Editor.EnableEdit =!readOnly;
            cell.View.BackColor = readOnly ? ApplicationColors.BGReadOnlyCell : ApplicationColors.BGStandardCell;

        }

        public GridRow SelectedRow
        {
            get
            {
                RowSelection rowSelection = (RowSelection)grid1.Selection;
                var region = rowSelection.GetSelectionRegion();
                if (region.Count > 0)
                {
                    return grid1.Rows[region[0].Start.Row];
                }
                return null;
            }
        }

        public MapperData MapperData
        {
            get { return data; }
        }

        public void DeleteSelectedRow()
        {
            if(SelectedRow!=null)
            {
                grid1.Rows.Remove(SelectedRow.Index);
            }
        }

        public void UpdateCellsReadOnlyMode(GridRow row)
        {
            var entry = row.Tag as MapperEntry;
            setCellReadOnly(grid1.GetCell(row.Index, FromExerciseColumn),false);
            setCellReadOnly(grid1.GetCell(row.Index, ToExerciseColumn), entry == null);
            ExerciseDTO fromExercise = null;
            if(entry!=null)
            {
                fromExercise=ObjectsReposidory.GetExercise(entry.FromExerciseId);
            }
            bool cantDelete = fromExercise == null || !fromExercise.IsMine() ||
                                 fromExercise.Status == PublishStatus.Published;
            setCellReadOnly(grid1.GetCell(row.Index, OperationColumn), cantDelete);
            if (cantDelete)
            {
                var cell = (Cell) grid1.GetCell(row.Index, OperationColumn);
                //cell.Value = MapperEntryOperation.OnlyReMap;
                var selectedItem = from t in ((SourceGrid.Cells.Editors.ComboBox)cell.Editor).Control.Items.Cast<ComboBoxItem>() where (MapperEntryOperation)t.Tag == MapperEntryOperation.OnlyReMap select t;
                cell.Value = selectedItem.SingleOrDefault();
            }
            usrProgressIndicatorButtons1.OkButton.Enabled = grid1.RowsCount>2;
            grid1.Refresh();
        }

        MapperData getMapperDate()
        {
            MapperData data = new MapperData();
             foreach (var gridViewRow in grid1.Rows)
             {
                 if (gridViewRow.Tag == null)
                 {
                     continue;
                 }
                 var entry = (MapperEntry) gridViewRow.Tag;
                 Cell cell = (Cell) grid1.GetCell(gridViewRow.Index, FromExerciseColumn);
                 entry.FromExerciseId = (Guid) cell.Value;
                 cell = (Cell)grid1.GetCell(gridViewRow.Index, ToExerciseColumn);
                 entry.ToExerciseId = (Guid)cell.Value;
                 cell = (Cell)grid1.GetCell(gridViewRow.Index, OperationColumn);
                 entry.Operation = (MapperEntryOperation)((ComboBoxItem)cell.Value).Tag;
                 data.Entries.Add(entry);
             }
            return data;
        }

        void showWrongEntries(List<MapperEntry > entries)
        {
            foreach (var gridRow in grid1.Rows)
            {
                if (gridRow.Tag == null)
                {
                    continue;
                }
                var entry = (MapperEntry)gridRow.Tag;
                if(entries.Contains(entry))
                {
                    setRowBackColor(gridRow, Color.Red);
                }
                else
                {
                    setRowBackColor(gridRow, Color.White);
                }
            }
            grid1.Refresh();
        }

        void setRowBackColor(RowInfo row,Color backColor)
        {
            for (int i = 0; i < ColumnsNumber-1; i++)
            {
                var cell =(Cell)grid1.GetCell(row.Index, i);
                cell.View.BackColor = backColor;
             
            }
        }
        private void usrProgressIndicatorButtons1_OkClick(object sender, BodyArchitect.Controls.ProgressIndicator.CancellationSourceEventArgs e)
        {
            data = getMapperDate();
            var wrongList = data.Validate();
            showWrongEntries(wrongList);
            if(wrongList.Count>0)
            {
                FMMessageBox.ShowError(StrengthTrainingEntryStrings.ExerciseMapper_DuplicatedExercisesError);
                return;
            }
            if(dtaFrom.DateTime!=DateTime.MinValue)
            {
                data.StartDate = dtaFrom.DateTime;
            }
            if (dtaTo.DateTime != DateTime.MinValue)
            {
                data.EndDate = dtaTo.DateTime;
            }
            ServiceManager.MapExercises(data);
            ThreadSafeClose();
        }

        private void cmbExerciseViewType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(MapperData==null)
            {
                return;
            }
           UserContext.Settings.ExerciseComboBoxSelectedItem = cmbExerciseViewType.SelectedIndex;
           refreshGrid();
        }
    }
}