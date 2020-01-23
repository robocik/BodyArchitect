using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FM.StrengthTraining.Model;
using SourceGrid;

namespace FM.StrengthTraining.Controls.SourceGridExtension
{
    public class CellValueChangedController : SourceGrid.Cells.Controllers.ControllerBase
    {
        private StrengthTrainingEntry strengthEntry = new StrengthTrainingEntry();
        private usrTrainingDaySourceGrid parent;
        public CellValueChangedController(StrengthTrainingEntry strengthEntry, usrTrainingDaySourceGrid parent)
        {
            this.parent = parent;
            this.strengthEntry = strengthEntry;
        }

        public override void OnValueChanged(SourceGrid.CellContext sender, EventArgs e)
        {

            base.OnValueChanged(sender, e);
            Grid grid = (Grid)sender.Grid;
            //RowSelection rowSelection = ;

            var row = grid.Rows[sender.Position.Row];
            var cell = grid[sender.Position.Row, sender.Position.Column];
            Serie serie = (Serie)cell.Tag;
            StrengthTrainingItem item = (StrengthTrainingItem)row.Tag;

            if (string.IsNullOrEmpty((string)sender.Value))
            {//this cell has been cleaned (doesn't contain any value) so we have to remove existing Serie object
                if (serie != null)
                {
                    item.RemoveSerie(serie);
                    cell.Tag = null;
                    cell.ToolTipText = null;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty((string)sender.Value) && serie == null)
                {//user added new serie so we have to create Serie object
                    serie = new Serie();
                    cell.Tag = serie;
                    item.AddSerie(serie);
                }

                serie.SetFromString((string)sender.Value);
            }
            parent.UpdateCellsReadOnlyMode(row);
        }
    }
}
