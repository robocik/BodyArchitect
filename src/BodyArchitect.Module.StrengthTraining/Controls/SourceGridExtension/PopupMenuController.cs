using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BodyArchitect.Module.StrengthTraining.Controls.SourceGridExtension
{
    public class PopupMenu : SourceGrid.Cells.Controllers.ControllerBase
    {
        private ContextMenuStrip menu;

        public PopupMenu(ContextMenuStrip menu)
        {
            this.menu = menu;
        }

        public override void OnMouseDown(SourceGrid.CellContext sender, MouseEventArgs e)
        {
            //sender.Grid.Selection.ResetSelection(false);
            sender.Grid.Selection.SelectCell(sender.Grid.PositionAtPoint(e.Location),true);
            sender.Grid.Selection.Focus(sender.Grid.PositionAtPoint(e.Location), true);
            
            //sender.Grid.MouseCellPosition)
            base.OnMouseDown(sender, e);
        }
        public override void OnMouseUp(SourceGrid.CellContext sender, MouseEventArgs e)
        {
            base.OnMouseUp(sender, e);
            
            if (e.Button == MouseButtons.Right)
                menu.Show(sender.Grid, new Point(e.X, e.Y));
        }

    }
}
