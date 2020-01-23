using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Common;
using BodyArchitect.Common.Plugins;
using System.ComponentModel.Composition;
using System.Reflection;
using BodyArchitect.Controls;
using BodyArchitect.Module.A6W.Localization;
using BodyArchitect.Common.Localization;
using BodyArchitect.Module.A6W.Controls;
using System.Drawing;
using BodyArchitect.Service.Model;
using BodyArchitect.Shared;


namespace BodyArchitect.Module.A6W
{
    [Export(typeof(IEntryObjectProvider))]
    public class A6WModule : IEntryObjectProvider
    {
        public static readonly Guid ModuleId = new Guid("A7B7503A-FF26-414D-A8A5-242C137B426C");
        
        public Guid GlobalId
        {
            get { return ModuleId; }
        }

        public void Startup(EntryObjectLocalizationManager localizationManager, EntryObjectControlManager controlManager)
        {
            localizationManager.RegisterResourceManager(typeof(A6WEntryDTO), A6WEntryStrings.ResourceManager);
            controlManager.RegisterControl<A6WEntryDTO>(typeof(usrA6W));
        }


        public Image ModuleImage
        {
            get { return A6WResources.A6WModule; }
        }


        public Type EntryObjectType
        {
            get { return typeof(A6WEntryDTO); }
        }

        public ICalendarDayContent CalendarDayContent
        {
            get { return new A6WCalendarDayContent(); }
        }

        public void CreateGui(IMainWindow mainWnd)
        {
            
        }

        public void PrepareNewEntryObject(SessionData sessionData, EntryObjectDTO obj, TrainingDayDTO day)
        {
            var a6w = (A6WEntryDTO)obj;

            if (a6w.MyTraining == null)
            {
                var trainings = ServiceManager.GetStartedTrainings(A6WEntryDTO.EntryTypeId);
                if (trainings.Count > 0)
                {
                    a6w.MyTraining = trainings[0];
                }
            }

            if (a6w.MyTraining == null)
            {
                a6w.MyTraining = new MyTrainingDTO();
                a6w.MyTraining.ProfileId = UserContext.CurrentProfile.Id;
                a6w.MyTraining.Name = A6WEntryStrings.EntryTypeName;
                a6w.MyTraining.StartDate = day.TrainingDate;
                a6w.MyTraining.TypeId = A6WEntryDTO.EntryTypeId;
                a6w.DayNumber = 1;
            }
            else
            {
                if (day.TrainingDate.Date < a6w.MyTraining.StartDate.Date)
                {
                    throw new TrainingIntegrationException(string.Format(A6WEntryStrings.ErrorA6WTrainingAlreadyStarted, a6w.MyTraining.StartDate));
                }
            }

            if (a6w.Id == Constants.UnsavedObjectId)
            {
                var myEntries = ServiceManager.GetMyTrainingEntries(a6w.MyTraining);
                if (myEntries.Count > 0)
                {
                    var maxDate = myEntries.Max(x => x.TrainingDay.TrainingDate);
                    if (maxDate > day.TrainingDate)
                    {
                        throw new TrainingIntegrationException("You cannot insert new A6W entry in the middle of training");
                    }
                }
                a6w.DayNumber = a6w.GetNextDayNumber(myEntries);
                a6w.MyTraining.Tag = myEntries;
            }
            
        }

        public void CreateMainMenuItems(MenuStrip menu)
        {
            
        }

        public void AfterSave(SessionData sessionData, TrainingDayDTO day)
        {
            
        }
    }
}
