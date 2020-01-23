using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using BodyArchitect.Controls;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Model;
using BodyArchitect.Module.StrengthTraining.Model;
using BodyArchitect.Module.StrengthTraining.Controls;

using BodyArchitect.Common;
using BodyArchitect.DataAccess;
using BodyArchitect.Controls.Forms;
using System.Globalization;
using DevExpress.Utils;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    public partial class usrTrainingDayGridView : XtraUserControl
    {
        //public const string SerieRepetitionMask = "99x999";
        public readonly string SerieRepetitionMask = @"[0-9]*x[0-9]+[\{0}]?[0-9]*";
        private StrengthTrainingEntry strengthEntry;
        private const string ColumnSeriePrefix = "colSerie";

        

        public usrTrainingDayGridView()
        {
            InitializeComponent();
            //dataGridView1.CellValueChanged += new DataGridViewCellEventHandler(dataGridView1_CellValueChanged);
            SerieRepetitionMask = string.Format(SerieRepetitionMask,
                                                CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

            this.tcmbSeries.SelectedIndex = UserContext.Settings.SerieNumberComboBoxSelectedItem;
            dataGridView1.AutoGenerateColumns = false;
            createFixedColumns();
            fillSuperTips();
        }

        void fillSuperTips()
        {
            if (UserContext.Settings.GuiState.ShowToolTips)
            {
                //ControlHelper.AddSuperTip(defaultToolTipController1, dataGridView1,  null,SuperTips.TrainingDayGridView_Grid);
                //ControlHelper.AddSuperTip(defaultToolTipController1, tcmbSeries.ComboBox, tslSerieCount.Text, SuperTips.TrainingDayGridView_SerieNumber);
                //ControlHelper.AddSuperTip(defaultToolTipController1, cmbExerciseColumns.ComboBox, tslExerciseDisplayProperty.Text, SuperTips.TrainingDayGridView_ExerciseViewType);
            }
        }

        void createFixedColumns()
        {
            colExercise.DisplayMember = "Name";
            colExercise.ValueMember = "GlobalId";
            colExercise.Columns.Add(new LookUpColumnInfo("Name", ApplicationStrings.CMBExerciseName));
            colExercise.Columns.Add(new LookUpColumnInfo("Shortcut", ApplicationStrings.CMBExerciseShortcut));
            colExercise.Columns.Add(new LookUpColumnInfo("ExerciseType", ApplicationStrings.CMBExerciseType));
            cmbExerciseColumns.ComboBox.DisplayMember = "Caption";
            cmbExerciseColumns.ComboBox.ValueMember = "FieldName";
            cmbExerciseColumns.ComboBox.DataSource = colExercise.Columns;
            cmbExerciseColumns.SelectedIndex = UserContext.Settings.ExerciseComboBoxSelectedItem;
        }

        private void createSerieGridColumns(int serieColumnCount)
        {
            //removing old series columns
            removeSerieColumns();

            for (int i = 1; i <= serieColumnCount; i++)
            {
                DataGridViewMaskedTextColumn column = new DataGridViewMaskedTextColumn(SerieRepetitionMask);
                //column.DataPropertyName = "Serie" + i;
                column.HeaderText = string.Format(ApplicationStrings.SerieColumnHeader,i);
                column.Name = ColumnSeriePrefix+i;
                column.SortMode = DataGridViewColumnSortMode.Automatic;
                column.Width = 70;
                this.dataGridView1.Columns.Add(column);
            }
        }

        private void removeSerieColumns()
        {
            for (int i = dataGridView1.Columns.Count; i >= 0; i--)
            {
                if(dataGridView1.Columns.Contains(ColumnSeriePrefix+i))
                {
                    this.dataGridView1.Columns.Remove(ColumnSeriePrefix+i);
                }
            }
        }

        public void Fill(StrengthTrainingEntry entry)
        {
            this.strengthEntry = entry;
            
            refreshGridLayout();
        }

        int getSeriesNumber()
        {//this method estimate series column count to display. We never cannot show less serie columns than entered Series in any StrengthTrainingItem
            int comboBoxSeries= int.Parse(tcmbSeries.SelectedItem.ToString());
            int daySeries = strengthEntry != null ? strengthEntry.GetMaximumSeriesCount() : 0;
            return Math.Max(comboBoxSeries, daySeries);
        }

        private void refreshGridLayout()
        {
            if (strengthEntry == null)
            {
                return;
            }
            int serieCount = getSeriesNumber();
            tcmbSeries.SelectedItem = serieCount.ToString();
            
            fillExerciseComboBox();
            createSerieGridColumns(serieCount);

            dataGridView1.Rows.Clear();
            foreach (StrengthTrainingItem entry in strengthEntry.Entries)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.Tag = entry;
                int index=dataGridView1.Rows.Add(row);

                row = dataGridView1.Rows[index];
                
                row.Cells[colExercise.Name].Value  = entry.ExerciseId;
                row.Cells[colComment.Name].Value = entry.Comment;
                for (int i = 0; i < entry.Series.Count; i++)
                {
                    int serieNo = i + 1;
                    string colName = ColumnSeriePrefix + serieNo;
                    if (dataGridView1.Columns.Contains(colName))
                    {
                        row.Cells[colName].ToolTipText = createSerieToolTip(entry.Series[i]);
                        row.Cells[colName].Value = entry.Series[i].ToString();
                        row.Cells[colName].Tag = entry.Series[i];
                    }
                }
                updateCellsReadOnlyMode(row);
            }

            var row1 = dataGridView1.Rows[dataGridView1.Rows.Count - 1];
            setRowReadOnly(row1, true);

        }

        private string createSerieToolTip(Serie serie)
        {
            string tooltip = string.Empty;
            if(serie.IsCiezarBezSztangi)
            {
                tooltip += ApplicationStrings.SerieCellCiezarBezSztangiToolTip+Environment.NewLine;
            }
            if (serie.IsLastRepetitionWithHelp)
            {
                tooltip += ApplicationStrings.SerieCellIsLastRepetitionWithHelpToolTip + Environment.NewLine;
            }
            if(!string.IsNullOrEmpty(serie.Comment))
            {
                tooltip += Environment.NewLine +serie.Comment;
            }
            return tooltip;
        }

        private void fillExerciseComboBox()
        {
            LookUpColumnInfo selectedCol = (LookUpColumnInfo)cmbExerciseColumns.SelectedItem;
            colExercise.DisplayMember = selectedCol.FieldName;

            List<LocalizedExercise> exercises = new List<LocalizedExercise>();
            foreach (ExerciseDTO exercise in ObjectsReposidory.Exercises)
            {
                LocalizedExercise localizedExercise = new LocalizedExercise(exercise);
                exercises.Add(localizedExercise);
            }
            ExerciseDTO ex = new ExerciseDTO(Guid.Empty);
            ex.Name = "<Dodaj>";
            //exercises.Add(ex);
            colExercise.DataSource = exercises;
        }

        void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var row = dataGridView1.Rows[e.RowIndex];
            var cell = row.Cells[e.ColumnIndex];
            if(cell.OwningColumn==colExercise)
            {
                Guid exerciseId= (Guid)cell.Value;
                if(exerciseId==Guid.Empty)
                {//user wants to add a new exercise
                    var exercise = new ExerciseDTO(Guid.NewGuid());
                    exercise.ExerciseType = ExerciseType.Biceps;
                    AddExercise dlg = new AddExercise();
                    dlg.Fill(exercise);
                    if(dlg.ShowDialog()==DialogResult.OK)
                    {
                        
                        ObjectsReposidory.ClearExerciseCache();
                        fillExerciseComboBox();
                        cell.Value = dlg.NewExercise.GlobalId;
                        dataGridView1.NotifyCurrentCellDirty(true);
                        dataGridView1.EndEdit();
                    }
                    
                }
            }
        }

        void setRowReadOnly(DataGridViewRow row,string columnName, bool readOnly)
        {
            row.Cells[columnName].ReadOnly = readOnly;
            row.Cells[columnName].Style.BackColor = readOnly ? Color.LightGray : Color.White;
        }


        void setRowReadOnly(DataGridViewRow row,bool readOnly)
        {
            for (int i = 1; i < row.Cells.Count; i++)
            {
                row.Cells[i].ReadOnly = readOnly;
                row.Cells[i].Style.BackColor = readOnly ? Color.LightGray : Color.White;
            }
        }
  
        public List<StrengthTrainingItem> GetTrainingDayEntries()
        {
            List<StrengthTrainingItem> entries = new List<StrengthTrainingItem>();
            foreach (DataGridViewRow gridViewRow in dataGridView1.Rows)
            {
                if(gridViewRow.IsNewRow)
                {
                    continue;
                }
                StrengthTrainingItem entry;
                if (gridViewRow.Tag == null)
                {
                    entry = new StrengthTrainingItem();
                }
                else
                {
                    entry = (StrengthTrainingItem)gridViewRow.Tag;
                }
                entry.ExerciseId = (Guid)gridViewRow.Cells[colExercise.Name].Value;
                entry.Comment = (string)gridViewRow.Cells[colComment.Name].Value;

                for (int i = 2; i < dataGridView1.Columns.Count; i++)
                {
                    int serieNo = i - 1;
                    string colName = ColumnSeriePrefix + serieNo;
                    if (dataGridView1.Columns.Contains(colName))
                    {
                        DataGridViewCell cell = gridViewRow.Cells[colName];
                        Serie serie = (Serie) cell.Tag;
                        string serieString = (string) cell.Value;
                        if (serie == null && string.IsNullOrEmpty(serieString))
                        {
                            continue;
                        }
                        if (serie == null)
                        {
                            serie = new Serie();
                            cell.Tag = serie;
                            entry.AddSerie(serie);
                        }

                        serie.SetFromString(serieString);
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
            refreshGridLayout();
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var row=dataGridView1.Rows[e.RowIndex];
            var cell = row.Cells[e.ColumnIndex];
            endEditCell(row, cell);
            this.rebuildRow(row);
        }

        private void endEditCell(DataGridViewRow row, DataGridViewCell cell)
        {
            if (row.Tag == null && !row.IsNewRow)
            {
                //!row.IsNewRow this condition means that in this row user selected exercise (we skip situation when exercise is not selected and user fill serie)
                StrengthTrainingItem entry=new StrengthTrainingItem();
                row.Tag = entry;
                entry.ExerciseId = (Guid)row.Cells[colExercise.Name].Value;
                strengthEntry.AddEntry(entry);

            }
            if(cell.OwningColumn.Name.StartsWith(ColumnSeriePrefix))
            {
                Serie serie = (Serie)cell.Tag;
                StrengthTrainingItem entry = (StrengthTrainingItem)row.Tag;
                if (string.IsNullOrEmpty((string)cell.Value))
                {//this cell has been cleaned (doesn't contain any value) so we have to remove existing Serie object
                    if (serie != null)
                    {
                        entry.RemoveSerie(serie);
                        cell.Tag = null;
                        cell.ToolTipText = null;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty((string)cell.Value) && serie == null)
                    {//user added new serie so we have to create Serie object
                        serie = new Serie();
                        cell.Tag = serie;
                        entry.AddSerie(serie);

                    }
                    serie.SetFromString(cell.Value.ToString());
                }
                
            }
            else if(cell.OwningColumn==colComment)
            {
                StrengthTrainingItem entry = (StrengthTrainingItem)row.Tag;
                entry.Comment = (string)cell.Value;
            }
        }

        void rebuildRow(DataGridViewRow row)
        {
            if (row.IsNewRow)
            {
                return;
            }
            for (int i = 0; i < row.Cells.Count; i++)
            {
                string columnName = ColumnSeriePrefix+(i+1);
                StrengthTrainingItem entry = (StrengthTrainingItem)row.Tag;

                if (dataGridView1.Columns.Contains(columnName))
                {
                    if(entry.Series.Count>=i+1)
                    {
                        row.Cells[columnName].Tag = entry.Series[i];
                        row.Cells[columnName].Value = entry.Series[i].ToString();
                    }
                    else
                    {
                        row.Cells[columnName].Tag = null;
                        row.Cells[columnName].Value = null;

                    }
                }
            }

            updateCellsReadOnlyMode(row);
        }

        void updateCellsReadOnlyMode(DataGridViewRow row)
        {
            if(row.IsNewRow)
            {
                return;
            }
            for (int i = 1; i < dataGridView1.Columns.Count; i++)
            {
                if (dataGridView1.Columns.Contains(ColumnSeriePrefix + (i + 1)))
                {
                    Serie serie = row.Cells[ColumnSeriePrefix+i].Tag as Serie;
                    setRowReadOnly(row, ColumnSeriePrefix + (i + 1), serie == null);
                }
            }
            setRowReadOnly(row, colComment.Name,(row.Tag as StrengthTrainingItem)==null);
            //column Serie 1 should be always editable (in case of course that there is exercise selected)
            setRowReadOnly(row, ColumnSeriePrefix+"1", (row.Tag as StrengthTrainingItem) == null);

        }

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            setRowReadOnly(e.Row, true);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            if(dataGridView1.SelectedCells.Count >0)
            {
                //we show menu only when user right click on the cell with correct serie (serie with values)
                e.Cancel = SelectedSerie == null;
            }
            
        }

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            //selecting cell when user right click on it
            if(e.Button==MouseButtons.Right)
            {
                DataGridView.HitTestInfo info = dataGridView1.HitTest(e.X, e.Y);
                dataGridView1.ClearSelection();
                if(info.Type == DataGridViewHitTestType.Cell )
                {
                    if(!dataGridView1.Rows[info.RowIndex].Selected )
                    {
                        dataGridView1.Rows[info.RowIndex].Cells[info.ColumnIndex].Selected = true;
                    }
                }
            }
        }

        private void dodajInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SerieInfoWindow window = new SerieInfoWindow(SelectedSerie);
            if(window.ShowDialog()==DialogResult.OK)
            {
                dataGridView1.SelectedCells[0].ToolTipText = createSerieToolTip(SelectedSerie);
            }
        }

        public Serie SelectedSerie
        {
            get
            {
                if (dataGridView1.SelectedCells.Count > 0)
                {
                    //we show menu only when user right click on the cell with correct serie (serie with values)
                    Serie serie = dataGridView1.SelectedCells[0].Tag as Serie;
                    return serie;
                }
                return null;
            }
        }


        private void cmbExerciseColumns_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (strengthEntry == null)
            {
                return;
            }
            
            UserContext.Settings.ExerciseComboBoxSelectedItem = cmbExerciseColumns.SelectedIndex;
            this.refreshGridLayout();
        }

        private void dataGridView1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            var row = this.dataGridView1.CurrentRow;
            if(row.IsNewRow)
            {
                return;
            }
            if (e.KeyCode == Keys.Delete)
            {//delete cell content
                StrengthTrainingItem entry = (StrengthTrainingItem)row.Tag;
                Serie serie = SelectedSerie;
                if (serie != null)
                {
                    this.dataGridView1.CurrentCell.Value = null;
                    this.dataGridView1.CurrentCell.Tag = null;
                    this.dataGridView1.CurrentCell.ToolTipText = null;
                    entry.RemoveSerie(serie);
                    rebuildRow(dataGridView1.CurrentRow);
                }
                else if(this.dataGridView1.CurrentCell.OwningColumn==colComment)
                {
                    this.dataGridView1.CurrentCell.Value = null;
                    entry.Comment = null;
                }
            }
            else if (e.KeyCode == Keys.F4)
            {//repete value from current cell to the next serie cell (for example: copy value from Serie 1 (current) to Serie 2)
                DataGridViewColumn col = dataGridView1.CurrentCell.OwningColumn;
                if (col.Name.StartsWith(ColumnSeriePrefix))
                {
                    string value = (string)dataGridView1.CurrentCell.Value;
                    int index=dataGridView1.Columns.IndexOf(col);
                    if (!string.IsNullOrEmpty(value) && dataGridView1.Columns.Count > index + 1)
                    {
                        row.Cells[index + 1].Value = value;
                        row.Cells[index].Selected = false;
                        row.Cells[index + 1].Selected = true;
                        dataGridView1.CurrentCell = row.Cells[index + 1];
                        endEditCell(row, row.Cells[index + 1]);
                        updateCellsReadOnlyMode(row);
                    }
                }
            }
            else if (e.KeyCode == Keys.F3)
            {//repete value from previous Serie cell to the current Serie cell (for example: copy value from Serie 1 to Serie 2 (current))
                DataGridViewColumn col = dataGridView1.CurrentCell.OwningColumn;
                if (col.Name.StartsWith(ColumnSeriePrefix) && col.Name!=ColumnSeriePrefix+"1")
                {
                    
                    int index = dataGridView1.Columns.IndexOf(col);
                    string value = (string)row.Cells[index - 1].Value;
                    row.Cells[index].Value = value;
                    row.Cells[index].Selected = false;
                    endEditCell(row, row.Cells[index]);
                    if (dataGridView1.Columns.Count > index + 1)
                    {
                        row.Cells[index + 1].Selected = true;
                        dataGridView1.CurrentCell = row.Cells[index + 1];
                    }
                    updateCellsReadOnlyMode(row);

                }
            }
        }

        private void dataGridView1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            StrengthTrainingItem entry = e.Row.Tag as StrengthTrainingItem;
            if (entry != null)
            {
                strengthEntry.RemoveEntry(entry);
                e.Row.Tag = null;
            }
        }

    }
}
