using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using BodyArchitect.Module.StrengthTraining.Localization;
using BodyArchitect.Service.Model;
using SourceGrid;
using SourceGrid.Cells.Controllers;

namespace BodyArchitect.Module.StrengthTraining.Controls.SourceGridExtension
{
    class MapperGridExerciseController : ControllerBase
    {
        private ExerciseMapperWindow parent;

        public MapperGridExerciseController(ExerciseMapperWindow parent)
        {
            this.parent = parent;
        }

        public override void OnValueChanged(SourceGrid.CellContext sender, EventArgs e)
        {
            base.OnValueChanged(sender, e);
            Grid grid1 = (Grid) sender.Grid;

            var row = grid1.Rows[sender.Position.Row];

            if (row == null)
            {
                return;
            }
            MapperEntry entry = null;
            if (row.Tag == null)
            {
                entry = new MapperEntry();
                row.Tag = entry;
                parent.AddEmptyRow();
                parent.MapperData.Entries.Add(entry);
            }
            else
            {
                entry = (MapperEntry)row.Tag;
            }
            Guid newId = (Guid)sender.Value;

            if(sender.Position.Column==ExerciseMapperWindow.FromExerciseColumn)
            {
                entry.FromExerciseId = newId;
            }
            else
            {
                entry.ToExerciseId = newId;
                
            }
            

            parent.UpdateCellsReadOnlyMode(row);
            grid1[row.Index, sender.Position.Column].View.ForeColor = Color.Black;
            if (!(grid1[row.Index, ExerciseMapperWindow.DeleteColumn] is SourceGrid.Cells.Button))
            {
                

                var bnCol1 = new SourceGrid.Cells.Button(null);
                bnCol1.Image = StrengthTrainingResources.Delete;
                bnCol1.ToolTipText = StrengthTrainingEntryStrings.DeleteThisEntryBtn;
                var ctrButton1 = new SourceGrid.Cells.Controllers.Button();
                ctrButton1.Executed += new EventHandler(ctrButton1_Executed);
                bnCol1.View = new SourceGrid.Cells.Views.Button();
                bnCol1.AddController(ctrButton1);
                grid1[row.Index, usrStrengthTrainingSourceGridBase.DeleteRowColumnIndex] = bnCol1;
            }
        }

        void ctrButton1_Executed(object sender, EventArgs e)
        {
            parent.DeleteSelectedRow();
        }
    }
}
