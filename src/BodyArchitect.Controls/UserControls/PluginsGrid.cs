using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Common.Localization;
using BodyArchitect.Common.Plugins;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Settings.Model;
using SourceGrid;
using SourceGrid.Cells;

namespace BodyArchitect.Controls.UserControls
{
    public class PluginsGrid:Grid
    {

        public PluginsGrid()
        {
            
        }

        private void createGridHeader()
        {
            Redim(1, 3);
            var ttt = new SourceGrid.Cells.ColumnHeader(ApplicationStrings.PluginsGrid_PluginColumn);
            this[0, 0] = ttt;
            ttt.Column.AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize | SourceGrid.AutoSizeMode.EnableStretch;
            this[0, 1] = new SourceGrid.Cells.ColumnHeader(ApplicationStrings.PluginsGrid_IconColumn);
            this[0, 1].Column.Width = 70;
            this[0, 2] = new SourceGrid.Cells.ColumnHeader(ApplicationStrings.PluginsGrid_DefaultEntryColumn);
            this[0, 2].Column.Width = 70;
        }

        public void Fill(CalendarOptions options)
        {
            this.Rows.Clear();
            createGridHeader();
            for (int i = 0; i < PluginsManager.Instance.Modules.Length; i++)
            {
                int newRowIndex = Rows.Count;
                Rows.Insert(newRowIndex);
                var row =Rows[newRowIndex];
                var module = PluginsManager.Instance.Modules[i];
                row.Tag = module.GlobalId;
                string text = EntryObjectLocalizationManager.Instance.GetString(module.EntryObjectType,LocalizationConstants.EntryObjectName);
                var cell = new SourceGrid.Cells.Cell(text);
                cell.Image = module.ModuleImage;
                this[newRowIndex, 0] = cell;

                bool isChecked=true;
                if (options.ShowIcons.ContainsKey(module.GlobalId))
                {
                    isChecked = options.ShowIcons[module.GlobalId];
                }
                cell = new SourceGrid.Cells.CheckBox(null, isChecked);
                this[newRowIndex, 1] = cell;
                isChecked = options.GetDefaultEntry(module.GlobalId);
                cell = new SourceGrid.Cells.CheckBox(null, isChecked);
                this[newRowIndex, 2] = cell;
            }
            Columns.AutoSizeColumn(0);
        }

        public void Save(CalendarOptions options)
        {
            foreach (var gridViewRow in Rows)
            {
                if (gridViewRow.Tag == null)
                {
                    continue;
                }
                Guid moduleGlobalId = (Guid)gridViewRow.Tag;
                CheckBox cell = (CheckBox)GetCell(gridViewRow.Index, 1);
                options.ShowIcons[moduleGlobalId]= cell.Checked.Value;

                cell = (CheckBox)GetCell(gridViewRow.Index, 2);
                options.DefaultEntries[moduleGlobalId]= cell.Checked.Value;
            }
            //foreach (ListViewItem item in lvImages.Items)
            //{
            //    options.ShowIcons.Add((Guid)item.Tag, item.Checked);
            //}
            
        }

    }
}
