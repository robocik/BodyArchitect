using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Service.Model;
using BodyArchitect.Shared;
using SourceGrid;
using BodyArchitect.Controls;

namespace BodyArchitect.Module.Suplements.Controls.SourceGridExtension
{
    class SuplementTypeCellValueChangedController:SourceGrid.Cells.Controllers.ControllerBase
    {
        private SuplementsGrid parent;
        private SuplementsEntryDTO suplementEntry;

        public SuplementTypeCellValueChangedController(SuplementsGrid parent, SuplementsEntryDTO suplementEntry)
        {
            this.parent = parent;
            this.suplementEntry = suplementEntry;
        }

        public override void OnValueChanged(SourceGrid.CellContext sender, EventArgs e)
        {
            base.OnValueChanged(sender, e);
            SuplementsGrid grid1 = (SuplementsGrid)sender.Grid;

            SuplementItemDTO entry = null;
            var row = grid1.Rows[sender.Position.Row];

            if (row == null)
            {
                return;
            }
            if (row.Tag == null)
            {
                entry = new SuplementItemDTO();
                row.Tag = entry;
                suplementEntry.AddItem(entry);
                parent.AddEmptyRow();
                if (SuplementsSettings.Default.SetCurrentTime)
                {
                    entry.Time = DateTime.Now;
                    entry.Time = DateHelper.MoveToNewDate(entry.Time, suplementEntry.TrainingDay.TrainingDate);
                }

            }
            else
            {
                entry = (SuplementItemDTO)row.Tag;
            }

            entry.SuplementId = (Guid)sender.Value;
            //grid1[row.Index, SuplementsGrid.DosageTypeColumn].Value = DosageType.Grams;
            var selectedItem = from t in ((SourceGrid.Cells.Editors.ComboBox)grid1[row.Index, SuplementsGrid.DosageTypeColumn].Editor).Control.Items.Cast<ComboBoxItem>() where (DosageType)t.Tag == DosageType.Grams select t;
            grid1[row.Index, SuplementsGrid.DosageTypeColumn].Value = selectedItem.SingleOrDefault();

            grid1[row.Index, SuplementsGrid.SuplementTypeColumn].View.ForeColor = Color.Black;
            grid1[row.Index, SuplementsGrid.TimeColumn].Value = entry.Time;
            grid1[row.Index, SuplementsGrid.DosageColumn].Value = entry.Dosage;
            parent.UpdateCellsReadOnlyMode(row);

            if (!parent.ReadOnly && !(grid1[row.Index, SuplementsGrid.DeleteButtonColumn] is SourceGrid.Cells.Button))
            {
                var bnCol1 = new SourceGrid.Cells.Button(null);
                bnCol1.Image = SuplementsResources.Delete;
                bnCol1.ToolTipText = SuplementsEntryStrings.DeleteThisEntryTip;
                var ctrButton1 = new SourceGrid.Cells.Controllers.Button();
                ctrButton1.Executed += new EventHandler(deleteRowBtn_Execute);
                bnCol1.View = new SourceGrid.Cells.Views.Button();
                bnCol1.AddController(ctrButton1);
                grid1[row.Index, SuplementsGrid.DeleteButtonColumn] = bnCol1;
            }
        }

        void deleteRowBtn_Execute(object sender, EventArgs e)
        {
            GridRow row = parent.SelectedRow;
            if (parent.ReadOnly || row.Tag == null)
            {//empty row
                return;
            }
            if (FMMessageBox.AskYesNo(SuplementsEntryStrings.QDeleteSuplementRow) == DialogResult.Yes)
            {
                parent.Rows.Remove(row.Index);
                var entry = row.Tag as SuplementItemDTO;
                if (entry != null)
                {
                    suplementEntry.RemoveItem(entry);
                    row.Tag = null;
                }
            }

        }
    }
}
