using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using BodyArchitect.Client.Module.StrengthTraining.Controls;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;

namespace BodyArchitect.Client.Module.StrengthTraining
{
    public class SuperSetViewManager
    {
        Dictionary<string, Color> usedSuperSetColors = new Dictionary<string, Color>();
        List<Color> superSetColors = new List<Color>() { Colors.LightGreen, Colors.LightBlue, Colors.LightSkyBlue, Colors.LightSeaGreen, Colors.LightCyan, Colors.LightSalmon };

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
            return Colors.FloralWhite;
        }

        //public void ApplyColorScheme(SourceGrid.Grid grid, TrainingPlanDay planDay)
        //{
        //    foreach (var row in grid.Rows)
        //    {
        //        var item = (StrengthTrainingItemDTO)row.Tag;
        //        for (int i = 0; i < planDay.Entries.Count; i++)
        //        {
        //            var planEntry = planDay.Entries[i];
        //            for (int j = 0; j < planEntry.Sets.Count; j++)
        //            {
        //                if (grid.RowsCount > i + 1)
        //                {
        //                    //Cell cell = (Cell)grid.GetCell(i+1,usrStrengthTrainingSourceGridBase.StandardColumnNumber  + j);
        //                    //Serie serie = cell.Tag as Serie;
        //                    //if(serie!=null)
        //                    //{
        //                    //    cell.View.BackColor = Color.LightGoldenrodYellow;
        //                    //}
        //                    //else
        //                    //{
        //                    //    cell.View.BackColor = Color.LightPink;
        //                    //}
        //                    ApplyColorScheme(grid.Rows[i + 1], planEntry);
        //                }
        //            }
        //        }
        //    }
        //}

        //internal void ApplyColorScheme(StrengthTrainingItemViewModel row, TrainingPlanEntry planEntry)
        //{
        //    for (int j = 0; j < planEntry.Sets.Count; j++)
        //    {
        //        var setViewModel=row.GetSetViewModel(j);
        //        //Cell cell = (Cell)row.Grid.GetCell(row.Index, usrStrengthTrainingSourceGridBase.StandardColumnNumber + j);
        //        //var serie = cell.Tag as SerieDTO;
        //        if (setViewModel != null && setViewModel.Set!=null)
        //        {
        //            cell.View.BackColor = Color.LightGoldenrodYellow;
        //        }
        //        else
        //        {
        //            cell.View.BackColor = Color.LightPink;
        //        }
        //    }
        //}
    }
}
