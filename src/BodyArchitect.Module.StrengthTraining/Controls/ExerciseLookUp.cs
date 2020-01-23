using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using BodyArchitect.Common;
using BodyArchitect.Module.StrengthTraining.Localization;
using BodyArchitect.Service.Model;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using BodyArchitect.Controls;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Module.StrengthTraining.Model;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    public class ExerciseLookUp : LookUpEdit
    {
        public ExerciseLookUp()
        {
            Properties.BestFitMode = BestFitMode.BestFitResizePopup;
            Properties.DropDownRows = 20;
            Properties.ValueMember = "GlobalId";
            Properties.Columns.AddRange(CreateColumns());
            Properties.NullValuePrompt = StrengthTrainingEntryStrings.SelectExercise;
            Properties.DataSource = getExerciseList();
            SetDisplayColumn(Properties.Columns["Name"]);
        }

        private IList<LocalizedExercise> getExerciseList()
        {
            if(ServicesManager.IsDesignMode)
            {
                return new List<LocalizedExercise>();
            }
            List<LocalizedExercise> exercises = new List<LocalizedExercise>();
            foreach (ExerciseDTO exercise in ObjectsReposidory.Exercises.Values)
            {
                LocalizedExercise localizedExercise = new LocalizedExercise(exercise);
                exercises.Add(localizedExercise);
            }
            return exercises;
        }

        public void SetDisplayColumn(LookUpColumnInfo column)
        {
            Properties.DisplayMember = column.FieldName;

        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new RepositoryItemLookUpEdit Properties
        {
            get { return base.Properties; }
        }

        public static LookUpColumnInfo[] CreateColumns()
        {
            return new LookUpColumnInfo[] { 
                    new LookUpColumnInfo("Name", ApplicationStrings.CMBExerciseName) ,
                    new LookUpColumnInfo("Shortcut", ApplicationStrings.CMBExerciseShortcut),
                    new LookUpColumnInfo("Muscle", ApplicationStrings.CMBExerciseType)
                };
        }
    }
}
