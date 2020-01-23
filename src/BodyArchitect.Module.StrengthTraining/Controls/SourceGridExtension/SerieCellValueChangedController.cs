using System;
using BodyArchitect.Service.Model;
using SourceGrid;
using SourceGrid.Cells;

namespace BodyArchitect.Module.StrengthTraining.Controls.SourceGridExtension
{
    public class SerieCellValueChangedController : SourceGrid.Cells.Controllers.ControllerBase
    {
        private usrStrengthTrainingSourceGridBase parent;
        private bool internalChange;

        public SerieCellValueChangedController(usrStrengthTrainingSourceGridBase parent)
        {
            this.parent = parent;
        }

        public override void OnValueChanged(SourceGrid.CellContext sender, EventArgs e)
        {

            base.OnValueChanged(sender, e);

            if (internalChange)
            {
                return;
            }

            internalChange = true;
            try
            {
                Grid grid = (Grid)sender.Grid;

                var row = grid.Rows[sender.Position.Row];
                var cell =grid[sender.Position.Row, sender.Position.Column];
                var value = (string)sender.Value;
                SerieDTO serie = (SerieDTO)cell.Tag;
                var item = (StrengthTrainingItemDTO)row.Tag;
                bool serieDeleted = false;

                if (string.IsNullOrEmpty(value))
                {//this cell has been cleaned (doesn't contain any value) so we have to remove existing Serie object
                    if (serie != null)
                    {
                        item.RemoveSerie(serie);
                        cell.Tag = null;
                        cell.ToolTipText = null;
                        serieDeleted = true;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(value) && serie == null)
                    {//user added new serie so we have to create Serie object
                        serie = new SerieDTO();
                        cell.Tag = serie;
                        item.AddSerie(serie);
                    }

                    serie.SetFromString(value);
                }
                if (serieDeleted)
                {
                    rebuildRow(row);
                }
                parent.UpdateCellsReadOnlyMode(row);
            }
            finally
            {
                internalChange = false;
            }
            
        }

        void rebuildRow(GridRow row)
        {
            if (row.Tag==null)
            {
                return;
            }

            var entry = (StrengthTrainingItemDTO)row.Tag;
            for (int i = usrStrengthTrainingSourceGridBase.StandardColumnNumber; i < row.Grid.Columns.Count; i++)
            {
                Cell cell = (Cell)row.Grid.GetCell(row.Index, i);
                if (cell != null)
                {
                    int serieIndex = i - usrStrengthTrainingSourceGridBase.StandardColumnNumber;
                    if (entry.Series.Count >= serieIndex + 1)
                    {
                        cell.Tag = entry.Series[serieIndex];
                        cell.Value = entry.Series[serieIndex].ToString();
                    }
                    else
                    {
                        cell.Tag = null;
                        cell.Value = null;

                    }
                }
            }
        }
    }
}
