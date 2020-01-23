using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Controls.SourceGridExtension;
using BodyArchitect.Module.StrengthTraining.Localization;
using BodyArchitect.Service.Model;
using BodyArchitect.Service.Model.TrainingPlans;
using DevExpress.XtraEditors;
using BodyArchitect.Controls;
using SourceGrid;
using SourceGrid.Cells;
using SourceGrid.Selection;

namespace BodyArchitect.Module.StrengthTraining.Controls.SourceGridExtension
{
    class ExerciseCellValueChangedController:SourceGrid.Cells.Controllers.ControllerBase
    {
        private usrStrengthTrainingSourceGridBase parent;
        private StrengthTrainingEntryDTO strengthEntry;

        public ExerciseCellValueChangedController(usrStrengthTrainingSourceGridBase parent, StrengthTrainingEntryDTO strengthEntry)
        {
            this.parent = parent;
            this.strengthEntry = strengthEntry;
        }

        public override void OnValueChanged(SourceGrid.CellContext sender, EventArgs e)
        {
            base.OnValueChanged(sender, e);
            Grid grid1 = (Grid)sender.Grid;

            StrengthTrainingItemDTO entry = null;
            var row = grid1.Rows[sender.Position.Row];

            if (row == null)
            {
                return;
            }
            if (row.Tag == null)
            {
                entry = new StrengthTrainingItemDTO();
                row.Tag = entry;

                strengthEntry.AddEntry(entry);
                parent.AddEmptyRow(parent.GetSeriesNumber());
            }
            else
            {
                entry = (StrengthTrainingItemDTO)row.Tag;
            }
            Guid newId = (Guid) sender.Value;
            if (newId==Guid.Empty)
            {
                sender.Value = entry.ExerciseId;
                return;
            }
            entry.ExerciseId = newId;
            parent.UpdateCellsReadOnlyMode(row);

            if (!(grid1[row.Index, usrStrengthTrainingSourceGridBase.DeleteRowColumnIndex] is SourceGrid.Cells.Button))
            {
                grid1[row.Index, usrStrengthTrainingSourceGridBase.ExerciseColumnIndex].View.ForeColor = Color.Black;

                if(parent.ReadOnly)
                {
                    return;
                }
                var bnCol1 = new SourceGrid.Cells.Button(null);
                bnCol1.Image = StrengthTrainingResources.Delete;
                bnCol1.ToolTipText = StrengthTrainingEntryStrings.DeleteThisEntryBtn;
                var ctrButton1 = new SourceGrid.Cells.Controllers.Button();
                ctrButton1.Executed += new EventHandler(deleteRowBtn_Execute);
                bnCol1.View = new SourceGrid.Cells.Views.Button();
                bnCol1.AddController(ctrButton1);
                grid1[row.Index, usrStrengthTrainingSourceGridBase.DeleteRowColumnIndex] = bnCol1;
            }
        }

        

        void deleteRowBtn_Execute(object sender, EventArgs e)
        {
            parent.DeleteSelectedEntries();
        }
    }
}
