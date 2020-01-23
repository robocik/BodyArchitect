using System;
using BodyArchitect.Controls;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Controls.ProgressIndicator;
using BodyArchitect.Logger;
using BodyArchitect.Service.Model;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    public partial class AddExercise : BaseWindow
    {
        private ExerciseDTO newExercise;

        public AddExercise()
        {
            InitializeComponent();
        }

        public ExerciseDTO NewExercise
        {
            get { return newExercise; }
        }

        public void Fill(ExerciseDTO exercise)
        {

            bool isReadOnly = usrExerciseEditor1.Fill(exercise, ObjectsReposidory.Exercises.Values);
            usrProgressIndicatorButtons1.OkButton.Enabled = !isReadOnly;
        }

        public ExerciseDTO SaveExercise()
        {
            return usrExerciseEditor1.SaveExercise();
        }

        private void usrExerciseEditor1_ControlValidated(object sender, ControlValidatedEventArgs e)
        {
            usrProgressIndicatorButtons1.OkButton.Enabled = e.IsValid;
        }

        private void okButton1_Click(object sender, CancellationSourceEventArgs e)
        {
            //try
            //{
                newExercise=SaveExercise();
                ObjectsReposidory.ClearExerciseCache();
                //ObjectsReposidory.UpdateExercise(newExercise);
                ThreadSafeClose(System.Windows.Forms.DialogResult.OK);
           // }
            //catch (ActiveRecordValidationException uniqueException)
            //{
            //    //Guid exceptionID = ExceptionHandler.Process(uniqueException, true);
            //    DialogResult = System.Windows.Forms.DialogResult.None;
            //    ExceptionHandler.Default.Process(uniqueException);
            //}
            //catch (Exception ex)
            //{
            //    DialogResult = System.Windows.Forms.DialogResult.None;
            //    //Guid exceptionID = ExceptionHandler.Process(ex, true);
            //    //FMMessageBox.ShowError(ApplicationStrings.ErrorSaveExercise, exceptionID);
            //    ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorSaveExercise, ErrorWindow.EMailReport);
            //}
        }
    }
}
