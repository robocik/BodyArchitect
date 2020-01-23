using System;
using BodyArchitect.WCF;
using BodyArchitect.Common;
using BodyArchitect.Common.ProgressOperation;
using BodyArchitect.Controls.Localization;
using System.Windows.Forms;
using BodyArchitect.Logger;
using BodyArchitect.Service.Model;
using BodyArchitect.Shared;


namespace BodyArchitect.Controls
{
    class DomainObjectOperatonHelper
    {
        public static bool DeleteTrainingDay(SessionData sessionData, TrainingDayDTO day)
        {
            bool res = false;
            if (day != null)
            {
                if (FMMessageBox.AskYesNo(ApplicationStrings.QRemoveTrainingDay, day.TrainingDate.ToShortDateString()) == DialogResult.Yes)
                {

                        PleaseWait.Run(delegate(MethodParameters par)
                        {
                            try
                            {
                                ServiceManager.DeleteTrainingDay(day);
                                res = true;
                            }
                            catch (TrainingIntegrationException te)
                            {
                                res = false;
                                par.CloseProgressWindow();
                                ExceptionHandler.Default.Process(te, ApplicationStrings.ErrorCannotDeleteTrainingDayPartOfTraining, ErrorWindow.MessageBox);
                            }
                            
                        });

                    }

                    
                }
            return res;
        }
    }
}
