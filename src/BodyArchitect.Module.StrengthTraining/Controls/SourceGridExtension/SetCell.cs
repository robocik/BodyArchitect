using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Controls.SourceGridExtension;
using BodyArchitect.Module.StrengthTraining.Model;
using BodyArchitect.Service.Model;
using SourceGrid;
using SourceGrid.Cells;

namespace BodyArchitect.Module.StrengthTraining.Controls.SourceGridExtension
{
    public class SetEditor : MaskEditEditor
    {
        public bool IsCardio
        {
            get
            {

                var item = (StrengthTrainingItemDTO)((Cell)this.EditCell).Row.Tag;
                var exercise = ObjectsReposidory.GetExercise(item.ExerciseId);
                return exercise.ExerciseType == ExerciseType.Cardio;
            }
        }
        protected override void ShowControl(System.Windows.Forms.Control editorControl)
        {
            setCorrectMaskForSets();
            base.ShowControl(editorControl);
        }
        void setCorrectMaskForSets()
        {
            var SerieRepetitionMask = string.Format(@"[0-9]*x[0-9]*[\{0}]?[0-9]*", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            if (IsCardio)
            {
                MaskRegEx = @"((10|11|12|[1-9]):[0-5]\d:[0-5]\d)|((2[0-3]|[01][0-9]):[0-5][0-9]:[0-5][0-9])";
            }
            else
            {
                MaskRegEx = SerieRepetitionMask;
            }
        }

        public override void SetEditValue(object editValue)
        {
            var cell = (Cell)this.EditCell;
            SerieDTO serie = (SerieDTO)cell.Tag;

            if (IsCardio)
            {
                TimeSpan span = serie!=null && serie.RepetitionNumber.HasValue
                                    ? TimeSpan.FromSeconds(serie.RepetitionNumber.Value)
                                    : TimeSpan.Zero;
                base.SetEditValue(span);    
            }
            else
            {
                base.SetEditValue(editValue);
            }
            
        }

        //protected override void OnConvertingObjectToValue(DevAge.ComponentModel.ConvertingObjectEventArgs e)
        //{
        //    var cell = (SetCell) this.EditCell;
        //    SerieDTO serie = (SerieDTO)(cell).Value;
        //    string value = (string) e.Value;
        //    if(string.IsNullOrWhiteSpace(value))
        //    {
        //        serie.RepetitionNumber = null;
        //        serie.Weight = null;
        //    }
        //    else
        //    {
        //        if (cell.IsCardio)
        //        {
                
        //            TimeSpan span = TimeSpan.Parse(value);
        //            serie.RepetitionNumber = (int)span.TotalSeconds;
        //        }
        //        else
        //        {
        //            serie.SetFromString(value);
        //        }
        //    }
           
        //    e.Value = serie;
        //    base.OnConvertingObjectToValue(e);
        //}
    }

    public class SetView : SourceGrid.Cells.Views.Cell
    {
        protected override void PrepareVisualElementText(CellContext context)
        {
            base.PrepareVisualElementText(context);
            //if (context.Cell.Editor.EnableEdit && context.Value != null)
            //{
            //    var value = (DateTime)context.Value;
            //    //context.Value = "test";
            //    var timeEditControl = ((TimeEditEditor)context.Cell.Editor).Control;
            //    if (value.TimeOfDay != TimeSpan.Zero)
            //    {
            //        timeEditControl.Time = value;
            //        ElementText.Value = timeEditControl.Text;
            //        return;
            //    }
            //}
            Cell cell = (Cell)context.Cell;
            SerieDTO serie = (SerieDTO)cell.Tag;
            if (serie != null)
            {
                var exercise = ObjectsReposidory.GetExercise(serie.StrengthTrainingItem.ExerciseId);
                if (exercise.ExerciseType == ExerciseType.Cardio)
                {
                    TimeSpan timeSpan = TimeSpan.FromSeconds(serie.RepetitionNumber.HasValue ? serie.RepetitionNumber.Value : 0);
                    ElementText.Value = timeSpan.ToString();
                }
                else
                {
                    ElementText.Value = serie.GetDisplayText(Options.StrengthTraining.Default.ShowExtendedInfoInSets);
                }

            }
            //ElementText.Value = string.Empty;
        }
    }
}
