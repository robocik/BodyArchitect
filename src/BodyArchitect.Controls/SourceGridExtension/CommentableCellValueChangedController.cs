using System;
using BodyArchitect.Shared;
using SourceGrid;
using SourceGrid.Cells.Controllers;
using SourceGrid.Selection;

namespace BodyArchitect.Controls.SourceGridExtension
{
    public class CommentableCellValueChangedController : ControllerBase
    {
        public override void OnValueChanged(SourceGrid.CellContext sender, EventArgs e)
        {
            base.OnValueChanged(sender, e);

            Grid grid = (Grid)sender.Grid;
            var row = grid.Rows[sender.Position.Row];
            if (row.Tag == null)
            {
                return;
            }

            ICommentable entry = (ICommentable)row.Tag;
            entry.Comment = (string)sender.Value;
        }
    }
}
