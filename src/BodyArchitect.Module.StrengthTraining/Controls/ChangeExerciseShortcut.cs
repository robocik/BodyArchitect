using System;
using System.Collections.Generic;
using BodyArchitect.Controls;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Logger;
using BodyArchitect.Service.Model;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    public partial class ChangeExerciseShortcut : DevExpress.XtraEditors.XtraForm
    {
        private ExerciseDTO newExercise;

        public ChangeExerciseShortcut()
        {
            InitializeComponent();
        }

        public void Fill(ExerciseDTO exercise,IList<ExerciseDTO> exercises)
        {
            usrExerciseEditor1.Fill(exercise, exercises);
        }

        public ExerciseDTO NewExercise
        {
            get { return newExercise; }
        }

        public ExerciseDTO SaveExercise()
        {
            return usrExerciseEditor1.SaveExercise();
        }

        private void usrExerciseEditor1_ControlValidated(object sender, ControlValidatedEventArgs e)
        {
            okButton1.Enabled = e.IsValid;
        }

        private void okButton1_Click(object sender, EventArgs e)
        {
            try
            {
                newExercise = SaveExercise();
            }
            //catch (ActiveRecordValidationException uniqueException)
            //{
            //    //Guid exceptionID = ExceptionHandler.Process(uniqueException, true);
            //    DialogResult = System.Windows.Forms.DialogResult.None;
            //    ExceptionHandler.Default.Process(uniqueException);
            //}
            catch (Exception ex)
            {
                DialogResult = System.Windows.Forms.DialogResult.None;
                //Guid exceptionID = ExceptionHandler.Process(ex, true);
                //FMMessageBox.ShowError(ApplicationStrings.ErrorSaveExercise, exceptionID);
                ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorSaveExercise, ErrorWindow.EMailReport);
            }
        }
    }
}