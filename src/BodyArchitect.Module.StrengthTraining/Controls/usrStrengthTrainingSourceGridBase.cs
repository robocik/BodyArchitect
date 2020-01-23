using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Module.StrengthTraining.Localization;
using BodyArchitect.Service.Model;
using BodyArchitect.Service.Model.TrainingPlans;
using DevExpress.XtraEditors.Controls;
using BodyArchitect.Common;
using BodyArchitect.Controls;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Controls.SourceGridExtension;
using BodyArchitect.Module.StrengthTraining.Controls.SourceGridExtension;
using SourceGrid;
using SourceGrid.Cells;
using SourceGrid.Selection;
using ControlHelper = BodyArchitect.Controls.ControlHelper;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    public partial class usrStrengthTrainingSourceGridBase : DevExpress.XtraEditors.XtraUserControl
    {
        private StrengthTrainingEntryDTO strengthEntry = new StrengthTrainingEntryDTO();
        private PopupMenu popupMenuController = null;
        private ExerciseCellValueChangedController exerciseCellController;
        private CommentableCellValueChangedController CommentableCellController;
        public const int StandardColumnNumber = 3;
        public const int DeleteRowColumnIndex = 0;
        //public const int UpColumnIndex = 1;
        //public const int DownColumnIndex = 2;
        public const int ExerciseColumnIndex = 1;
        public const int CommentColumnIndex = 2;
        SuperSetViewManager superSetViewManager = new SuperSetViewManager();
        private TrainingPlanDay planDay;

        public usrStrengthTrainingSourceGridBase()
        {
            InitializeComponent();
            this.defaultToolTipController1.DefaultController.Active = UserContext.Settings.GuiState.ShowToolTips;
            fillSuperTips();
            CommentableCellController = new CommentableCellValueChangedController();
            popupMenuController = new PopupMenu(this.contextMenuStrip1);

            grid1.Controller.AddController(new SourceGrid.Cells.Controllers.ToolTipText());
            grid1.SelectionMode = GridSelectionMode.Row;
            grid1.Selection.EnableMultiSelection = true;
            grid1.Selection.SelectionChanged += new RangeRegionChangedEventHandler(Selection_SelectionChanged);
            fillExerciseCombobox();
            this.tcmbSeries.SelectedIndex = UserContext.Settings.SerieNumberComboBoxSelectedItem;
            fillSuperTips();
            createFixedColumns();

            this.tcmbSeries.SelectedIndexChanged += new System.EventHandler(this.tcmbSeries_SelectedIndexChanged);
            this.cmbExerciseColumns.SelectedIndexChanged += new System.EventHandler(this.cmbExerciseColumns_SelectedIndexChanged);
        }

        public bool ReadOnly { get;private set; }

        void fillSuperTips()
        {
            if (UserContext.Settings.GuiState.ShowToolTips)
            {
                ControlHelper.AddSuperTip(defaultToolTipController1.DefaultController, grid1, null, StrengthTrainingEntryStrings.usrStrengthTrainingSourceGrid_Tip_Grid);
                ControlHelper.AddSuperTip(defaultToolTipController1.DefaultController, tcmbSeries.ComboBox, tslSerieCount.Text, StrengthTrainingEntryStrings.usrStrengthTrainingSourceGrid_Tip_SetsNumber);
                ControlHelper.AddSuperTip(defaultToolTipController1.DefaultController, cmbExerciseColumns.ComboBox, tslExerciseDisplayProperty.Text, StrengthTrainingEntryStrings.usrStrengthTrainingSourceGrid_Tip_ExerciseViewType);
                
            }
        }

        void createFixedColumns()
        {
            int setNumber = createGridHeader();
            cmbExerciseColumns.SelectedIndex = UserContext.Settings.ExerciseComboBoxSelectedItem;
            AddEmptyRow(setNumber);

        }

        private void fillExerciseCombobox()
        {
            cmbExerciseColumns.ComboBox.DisplayMember = "Caption";
            cmbExerciseColumns.ComboBox.ValueMember = "FieldName";
            cmbExerciseColumns.ComboBox.DataSource = ExerciseLookUp.CreateColumns();
        }

        private int createGridHeader()
        {
            int setNumber = GetSeriesNumber();
            grid1.Redim(1, StandardColumnNumber + setNumber);
            grid1[0, DeleteRowColumnIndex] = new SourceGrid.Cells.ColumnHeader();
            grid1[0, DeleteRowColumnIndex].Column.Width = 20;
            var ttt = new SourceGrid.Cells.ColumnHeader(StrengthTrainingEntryStrings.ExerciseColumnHeader );
            grid1[0, ExerciseColumnIndex] = ttt;
            ttt.Column.AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize | SourceGrid.AutoSizeMode.EnableStretch;
            grid1[0, CommentColumnIndex] = new SourceGrid.Cells.ColumnHeader("Info");

            for (int i = 0; i < setNumber; i++)
            {
                grid1[0, StandardColumnNumber + i] = new SourceGrid.Cells.ColumnHeader(string.Format(StrengthTrainingEntryStrings.SerieColumnHeader, i + 1));
            }
            
            return setNumber;
        }

        

        public void AddEmptyRow(int setNumber)
        {
            SerieCellValueChangedController serieCellValueChangedController = new SerieCellValueChangedController(this);

            int newRowIndex = grid1.Rows.Count;
            grid1.Rows.Insert(newRowIndex);


            var lookupEditor = new LookupEditEditor(typeof(Guid));
            LookUpColumnInfo selectedCol = (LookUpColumnInfo)cmbExerciseColumns.SelectedItem;
            lookupEditor.Control.SetDisplayColumn(selectedCol);

            grid1[newRowIndex, ExerciseColumnIndex] = new SourceGrid.Cells.Cell(null, lookupEditor);
            SourceGrid.Cells.Views.Cell readOnlyView = new SourceGrid.Cells.Views.Cell();
            grid1[newRowIndex, ExerciseColumnIndex].View = readOnlyView;
            grid1[newRowIndex, ExerciseColumnIndex].View.ForeColor = ApplicationColors.FGNullText;
            grid1[newRowIndex, ExerciseColumnIndex].AddController(exerciseCellController);
            grid1[newRowIndex, ExerciseColumnIndex].Editor.NullDisplayString =StrengthTrainingEntryStrings.SelectExercise;
            grid1[newRowIndex, ExerciseColumnIndex].Column.Width = 250;
            grid1[newRowIndex, CommentColumnIndex] = new SourceGrid.Cells.Cell(null, new MemoExEditEditor());
            readOnlyView = new SourceGrid.Cells.Views.Cell();
            //grid1[newRowIndex, 1].AddController(ee);
            grid1[newRowIndex, CommentColumnIndex].View = readOnlyView;
            grid1[newRowIndex, CommentColumnIndex].AddController(CommentableCellController);
            grid1[newRowIndex, CommentColumnIndex].Column.Width = 80;

            //string SerieRepetitionMask = string.Format(@"[0-9]*x[0-9]*[\{0}]?[0-9]*",System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            for (int i = 0; i < setNumber; i++)
            {
                //readOnlyView = new SourceGrid.Cells.Views.Cell();
                var maskEditor = new SetEditor();
                //maskEditor.MaskRegEx = SerieRepetitionMask;
                grid1[newRowIndex, StandardColumnNumber + i] = new SourceGrid.Cells.Cell(null, maskEditor);
                grid1[newRowIndex, StandardColumnNumber + i].AddController(serieCellValueChangedController);
                grid1[newRowIndex, StandardColumnNumber + i].AddController(popupMenuController);
                grid1[newRowIndex, StandardColumnNumber + i].View = new SetView();
            }
            UpdateCellsReadOnlyMode(grid1.Rows[newRowIndex]);
        }



        public void UpdateCellsReadOnlyMode(GridRow row,bool readOnly=false)
        {
            grid1.GetCell(row.Index, ExerciseColumnIndex).Editor.EditableMode = readOnly ? EditableMode.None : EditableMode.SingleClick;
            grid1.GetCell(row.Index, ExerciseColumnIndex).Editor.EnableEdit = !readOnly;

            for (int i = StandardColumnNumber; i < grid1.Columns.Count; i++)
            {
                if (grid1.GetCell(row.Index, i + 1) != null)
                {
                    //Cell cell = (Cell)grid1.GetCell(row.Index, i);
                    var serie = ((Cell)grid1.GetCell(row.Index, i)).Tag as SerieDTO;
                    setCellReadOnly(grid1.GetCell(row.Index, i + 1), serie == null, readOnly,true);

                }
            }

            var entry = row.Tag as StrengthTrainingItemDTO;
            setCellReadOnly(grid1.GetCell(row.Index, CommentColumnIndex), entry == null, readOnly);
            //column Serie 1 should be always editable (in case of course that there is exercise selected)
            setCellReadOnly(grid1.GetCell(row.Index, StandardColumnNumber), entry == null, readOnly);
            if (planDay!=null && entry!=null && entry.TrainingPlanItemId.HasValue && this.tbShowPlan.Checked)
            {
                superSetViewManager.ApplyColorScheme(row, planDay.GetEntry(entry.TrainingPlanItemId.Value));
            }
        }

        void setCellReadOnly(ICellVirtual vCell, bool readOnly,bool viewMode,bool setColumn=false)
        {
            Cell cell = (Cell)vCell;
            //set edit for set column to double click (if editable of course) and the rest on single click
            if(setColumn)
            {
                cell.Editor.EditableMode = viewMode || readOnly ? EditableMode.None : EditableMode.Default;
            }
            else
            {
                cell.Editor.EditableMode = viewMode || readOnly ? EditableMode.None : EditableMode.SingleClick;
            }
            
            cell.Editor.EnableEdit = !viewMode && !readOnly;
            if (!viewMode)
            {
                cell.View.BackColor = readOnly ? ApplicationColors.BGReadOnlyCell : ApplicationColors.BGStandardCell;
            }
        }

        public int GetSeriesNumber()
        {//this method estimate series column count to display. We never cannot show less serie columns than entered Series in any StrengthTrainingItem
            int comboBoxSeries = int.Parse(tcmbSeries.SelectedItem.ToString());
            int daySeries = strengthEntry != null ? strengthEntry.GetMaximumSeriesCount() : 0;
            return Math.Max(comboBoxSeries, daySeries);
        }

        public void Fill(StrengthTrainingEntryDTO entry, TrainingPlanDay planDay,bool readOnly)
        {
            ReadOnly = readOnly;
            this.planDay = planDay;
            this.strengthEntry = entry;
            exerciseCellController = new ExerciseCellValueChangedController(this, strengthEntry);
            refreshGridLayout(readOnly);
            updateToolbar();
        }

        private void refreshGridLayout(bool readOnly)
        {
            if (strengthEntry == null)
            {
                return;
            }
            grid1.Rows.Clear();
            int serieCount = createGridHeader();
            tcmbSeries.SelectedItem = serieCount.ToString();


            for (int i = 0; i < strengthEntry.Entries.Count; i++)
            {
                int rowIndex = i + 1;
                AddEmptyRow(serieCount);

                GridRow row = grid1.Rows[rowIndex];
                var entry = strengthEntry.Entries[i];
                row.Tag = entry;
                Cell cell = (Cell)grid1.GetCell(rowIndex, ExerciseColumnIndex);
                cell.Value = entry.ExerciseId;
                if (!string.IsNullOrEmpty(entry.SuperSetGroup))
                {
                    cell.View.BackColor = superSetViewManager.GetSuperSetColor(entry.SuperSetGroup);
                }
                cell = (Cell)grid1.GetCell(rowIndex, CommentColumnIndex);
                cell.Value = entry.Comment;
                cell.ToolTipText = entry.Comment;

                
                for (int j = 0; j < entry.Series.Count; j++)
                {
                    cell = (Cell)grid1.GetCell(rowIndex, StandardColumnNumber + j);
                    cell.Tag = entry.Series[j];
                    cell.Value = entry.Series[j].GetDisplayText(false);
                    cell.ToolTipText = createSerieToolTip(entry.Series[j]);
                }
                UpdateCellsReadOnlyMode(row, readOnly);
            }
            if (!ReadOnly)
            {
                AddEmptyRow(serieCount);
            }
            if (planDay != null && this.tbShowPlan.Checked)
            {
                superSetViewManager.ApplyColorScheme(grid1, planDay);
            }
        }

        private string createSerieToolTip(SerieDTO serie)
        {
            string tooltip = string.Empty;
            if (serie.IsCiezarBezSztangi)
            {
                tooltip += StrengthTrainingEntryStrings.SerieCellCiezarBezSztangiToolTip + Environment.NewLine;
            }
            //TODO:Add more tooltips
            if (serie.SetType==SetType.MuscleFailure)
            {
                tooltip += StrengthTrainingEntryStrings.SerieCellIsLastRepetitionWithHelpToolTip + Environment.NewLine;
            }
            if (!string.IsNullOrEmpty(serie.Comment))
            {
                tooltip += Environment.NewLine + serie.Comment;
            }
            return tooltip;
        }

        public List<StrengthTrainingItemDTO> GetTrainingDayEntries()
        {
            List<StrengthTrainingItemDTO> entries = new List<StrengthTrainingItemDTO>();
            foreach (var gridViewRow in grid1.Rows)
            {
                if (gridViewRow.Tag == null)
                {
                    continue;
                }
                var entry = (StrengthTrainingItemDTO)gridViewRow.Tag;
                Cell cell = (Cell)grid1.GetCell(gridViewRow.Index, ExerciseColumnIndex);
                entry.ExerciseId = (Guid)cell.Value;
                entry.Position = gridViewRow.Index;
                cell = (Cell)grid1.GetCell(gridViewRow.Index, CommentColumnIndex);
                entry.Comment = (string)cell.Value;

                for (int i = StandardColumnNumber; i < grid1.Columns.Count; i++)
                {
                    cell = (Cell)grid1.GetCell(gridViewRow.Index, i);
                    if (cell.Tag != null)
                    {
                        //DataGridViewCell cell = gridViewRow.Cells[colName];
                        var serie = (SerieDTO)cell.Tag;
                        string serieString = (string)cell.Value;
                        if (string.IsNullOrEmpty(serieString))
                        {
                            continue;
                        }

                        serie.SetFromString(serieString);
                        if(serie.IsEmpty)
                        {
                            entry.RemoveSerie(serie);
                        }
                    }
                }
                entries.Add(entry);
            }

            return entries;
        }

        private void tcmbSeries_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (strengthEntry != null)
            {
                int serieCount = int.Parse(tcmbSeries.SelectedItem.ToString());
                int trainingDayCount = strengthEntry.GetMaximumSeriesCount();
                if (serieCount < strengthEntry.GetMaximumSeriesCount())
                {
                    FMMessageBox.ShowError(ApplicationStrings.ErrorSerieColumnCount);
                    tcmbSeries.SelectedItem = trainingDayCount.ToString();
                    return;
                }
            }
            UserContext.Settings.SerieNumberComboBoxSelectedItem = this.tcmbSeries.SelectedIndex;
            refreshGridLayout(ReadOnly);
        }

        private void cmbExerciseColumns_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (strengthEntry == null)
            {
                return;
            }

            UserContext.Settings.ExerciseComboBoxSelectedItem = cmbExerciseColumns.SelectedIndex;
            this.refreshGridLayout(ReadOnly);
        }

        public Cell SelectedCell
        {
            get
            {
                RowSelection rowSelection = (RowSelection)grid1.Selection;
                Position position = rowSelection.ActivePosition;
                if (position != Position.Empty)
                {
                    Cell selectedCell = (Cell)grid1.GetCell(position);
                    return selectedCell;
                }
                return null;
            }
        }

        public Grid Grid
        {
            get { return grid1; }
        }

        public GridRow SelectedRow
        {
            get
            {
                RowSelection rowSelection = (RowSelection)grid1.Selection;
                var region=rowSelection.GetSelectionRegion();
                if (region.Count > 0)
                {
                    return grid1.Rows[region[0].Start.Row];
                }
                return null;
            }
        }

        public SerieDTO SelectedSerie
        {
            get
            {
                Cell cell = SelectedCell;
                if (cell != null)
                {
                    return cell.Tag as SerieDTO;
                }
                return null;
            }
        }

        private void dodajInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SerieInfoWindow window = new SerieInfoWindow(SelectedSerie,ReadOnly);
            if (window.ShowDialog() == DialogResult.OK)
            {
                SelectedCell.ToolTipText = createSerieToolTip(SelectedSerie);
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = SelectedSerie == null;

        }

        private void grid1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            Cell col = SelectedCell;
            var serie = SelectedSerie;
            GridRow row = SelectedRow;

            if (e.KeyCode == Keys.F4)
            {//repete value from current cell to the next serie cell (for example: copy value from Serie 1 (current) to Serie 2)
                if (col.Column.Index >= StandardColumnNumber/*is serie column*/ && grid1.Columns.Count > col.Column.Index + 1)
                {
                    Cell nextCell = (Cell)grid1.GetCell(row.Index, col.Column.Index + 1);
                    nextCell.Value = col.Value;
                    grid1.Selection.Focus(new Position(row.Index, col.Column.Index + 1), true);

                    UpdateCellsReadOnlyMode(row);

                }
            }
            else if (e.KeyCode == Keys.F3)
            {//repete value from previous Serie cell to the current Serie cell (for example: copy value from Serie 1 to Serie 2 (current))
                if (col.Column.Index > StandardColumnNumber/*is serie column*/)
                {
                    Cell previousCell = (Cell)grid1.GetCell(row.Index, col.Column.Index - 1);
                    string value = (string)previousCell.Value;
                    col.Value = value;

                    if (grid1.Columns.Count > col.Column.Index + 1)
                    {
                        grid1.Selection.Focus(new Position(row.Index, col.Column.Index + 1), true);
                    }
                    UpdateCellsReadOnlyMode(row);

                }
            }
        }

        private void tbShowPlan_Click(object sender, EventArgs e)
        {
            tbShowPlan.Checked = !tbShowPlan.Checked;
            refreshGridLayout(ReadOnly);
        }

        private void tbMoveUp_Click(object sender, EventArgs e)
        {
            var regions = grid1.Selection.GetSelectionRegion();
            grid1.Selection.ResetSelection(false);
            for (int i = regions[0].Start.Row; i <= regions[0].End.Row; i++)
            {
                if(!rowMoveUp(grid1.Rows[i]))
                {
                    grid1.Selection.SelectRange(regions[0],true);
                    return;
                }
            }
            
        }

        private bool rowMoveUp(GridRow dayRow)
        {
            if (dayRow == null || dayRow.Tag==null)
            {
                return false;
            }
            int index = dayRow.Index;
            int limit = 1;

            if (index > limit)
            {
                grid1.Rows.Swap(index, index - 1);
                strengthEntry.RepositionEntry(index - 1, index - 2);
                //grid1.Selection.ResetSelection(false);
                grid1.Selection.SelectRow(dayRow.Index,true);
                //grid1.Selection.FocusRow(dayRow.Index);
                return true;
            }
            return false;
        }

        private void tbMoveDown_Click(object sender, EventArgs e)
        {
            //var dayRow = this.SelectedRow;
            var regions=grid1.Selection.GetSelectionRegion();
            grid1.Selection.ResetSelection(false);
            for (int i = regions[0].End.Row; i >= regions[0].Start.Row; i--)
            {
                if(!rowMoveDown(grid1.Rows[i]))
                {
                    grid1.Selection.SelectRange(regions[0], true);
                    return;
                }
            }
            //grid1.Selection.SelectRange(regions[0],true);
        }

        private bool  rowMoveDown(GridRow dayRow)
        {
            if (dayRow == null || dayRow.Tag == null)
            {
                return false;
            }
            int index = dayRow.Index;
            int limit = grid1.Rows.Count - 2;

            if (index < limit)
            {
                grid1.Rows.Swap(index, index + 1);
                strengthEntry.RepositionEntry(index - 1, index);
                //grid1.Selection.ResetSelection(false);
                grid1.Selection.SelectRow(dayRow.Index, true);
                //grid1.Selection.FocusRow(dayRow.Index);
                return true;
            }
            return false;
        }

        private void updateToolbar()
        {
            var region = grid1.Selection.GetSelectionRegion();
            tbMoveUp.Enabled = region.Count==1;
            tbMoveDown.Enabled = region.Count == 1;
            tbShowPlan.Enabled = strengthEntry.TrainingPlanItemId.HasValue && planDay!=null;
            tbAddSuperSet.Enabled = region.Count == 1;
            tbRemoveSuperSet.Enabled = region.Count > 0;
            tbDelete.Enabled = region.Count > 0;


            tbMoveUp.Visible = !ReadOnly;
            tbMoveDown.Visible = !ReadOnly;
            tbSupersets.Visible = !ReadOnly;
            tbDelete.Visible = !ReadOnly;
            toolStripSeparator1.Visible = toolStripSeparator2.Visible = !ReadOnly;
            tcmbSeries.Enabled = !ReadOnly;
        }

        void Selection_SelectionChanged(object sender, RangeRegionChangedEventArgs e)
        {
            updateToolbar();
        }

        private void tbDelete_Click(object sender, EventArgs e)
        {
            DeleteSelectedEntries();
        }

        public void DeleteSelectedEntries()
        {
            var regions =grid1.Selection.GetSelectionRegion();
            if (ReadOnly || FMMessageBox.AskYesNo(StrengthTrainingEntryStrings.QAskDeleteExerciseRow) == DialogResult.No)
            {
                return;
            }
            for (int j = regions.Count - 1; j >= 0; j--)
            {
                var region = regions[j];
                for (int i = region.End.Row; i >= region.Start.Row; i--)
                {
                    GridRow row = grid1.Rows[i];
                    if (row.Tag == null)
                    {//empty row
                        continue;
                    }

                    grid1.Rows.Remove(row.Index);
                    var entry = row.Tag as StrengthTrainingItemDTO;
                    if (entry != null)
                    {
                        strengthEntry.RemoveEntry(entry);
                        row.Tag = null;
                    }
                }
            }

        }

        private void tbAddSuperSet_Click(object sender, EventArgs e)
        {
            string superSetGroup = Guid.NewGuid().ToString();
            var regions = grid1.Selection.GetSelectionRegion();
            for (int i = regions[0].End.Row; i >= regions[0].Start.Row; i--)
            {
                var item = grid1.Rows[i].Tag as StrengthTrainingItemDTO;
                if(item!=null)
                {
                    item.SuperSetGroup = superSetGroup;
                }
            }
            refreshGridLayout(ReadOnly);
        }

        private void tbRemoveSuperSet_Click(object sender, EventArgs e)
        {
            var regions = grid1.Selection.GetSelectionRegion();
            for (int i = regions[0].End.Row; i >= regions[0].Start.Row; i--)
            {
                var item = grid1.Rows[i].Tag as StrengthTrainingItemDTO;
                if (item != null)
                {
                    item.SuperSetGroup = null;
                }
            }
            refreshGridLayout(ReadOnly);
        }

    }
}