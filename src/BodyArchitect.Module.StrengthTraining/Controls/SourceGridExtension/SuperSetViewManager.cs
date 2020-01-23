using System;
using System.Collections.Generic;
using System.Drawing;
using BodyArchitect.Service.Model;
using BodyArchitect.Service.Model.TrainingPlans;
using SourceGrid;
using SourceGrid.Cells;

namespace BodyArchitect.Module.StrengthTraining.Controls.SourceGridExtension
{
    public class SuperSetViewManager
    {
        Dictionary<string, Color> usedSuperSetColors = new Dictionary<string, Color>();
        List<Color> superSetColors = new List<Color>() { Color.LightGreen, Color.LightBlue, Color.LightSkyBlue, Color.LightSeaGreen, Color.LightCyan, Color.LightSalmon };

        public Color GetSuperSetColor(string superSetGroup)
        {
            if (usedSuperSetColors.ContainsKey(superSetGroup))
            {
                return usedSuperSetColors[superSetGroup];
            }
            //we need to create a new color
            foreach (Color color in superSetColors)
            {
                if (!usedSuperSetColors.ContainsValue(color))
                {
                    usedSuperSetColors.Add(superSetGroup, color);
                    return color;
                }
            }
            return Color.FloralWhite;
        }

        public void ApplyColorScheme(SourceGrid.Grid grid,TrainingPlanDay planDay)
        {
            foreach (var row in grid.Rows)
            {
                var item = (StrengthTrainingItemDTO)row.Tag;
                for (int i = 0; i < planDay.Entries.Count; i++)
                {
                    var planEntry = planDay.Entries[i];
                    for (int j = 0; j < planEntry.Sets.Count; j++)
                    {
                        if(grid.RowsCount>i+1)
                        {
                            //Cell cell = (Cell)grid.GetCell(i+1,usrStrengthTrainingSourceGridBase.StandardColumnNumber  + j);
                            //Serie serie = cell.Tag as Serie;
                            //if(serie!=null)
                            //{
                            //    cell.View.BackColor = Color.LightGoldenrodYellow;
                            //}
                            //else
                            //{
                            //    cell.View.BackColor = Color.LightPink;
                            //}
                            ApplyColorScheme(grid.Rows[i + 1], planEntry);
                        }
                    }   
                }
            }
        }

        public void ApplyColorScheme(GridRow row, TrainingPlanEntry planEntry)
        {
            for (int j = 0; j < planEntry.Sets.Count; j++)
            {
                Cell cell = (Cell)row.Grid.GetCell(row.Index , usrStrengthTrainingSourceGridBase.StandardColumnNumber + j);
                var serie = cell.Tag as SerieDTO;
                if (serie != null)
                {
                    cell.View.BackColor = Color.LightGoldenrodYellow;
                }
                else
                {
                    cell.View.BackColor = Color.LightPink;
                }
            }   
        }
    }
}
