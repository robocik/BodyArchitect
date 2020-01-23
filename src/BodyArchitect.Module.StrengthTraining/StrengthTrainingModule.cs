using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Common;
using BodyArchitect.Common.Localization;
using BodyArchitect.Common.Plugins;
using BodyArchitect.Controls;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Module.StrengthTraining.Localization;
using BodyArchitect.Service.Model;
using BodyArchitect.Settings;
using BodyArchitect.Module.StrengthTraining.Controls;
using DevExpress.Utils;


namespace BodyArchitect.Module.StrengthTraining
{
    [Export(typeof(IEntryObjectProvider))]
    public class StrengthTrainingModule : IEntryObjectProvider
    {
        public static readonly Guid ModuleId = new Guid("F69099BE-C42A-4EC5-B582-5EFF57758503");
        public Guid GlobalId
        {
            get { return ModuleId; }
        }

        public void Startup( EntryObjectLocalizationManager localizationManager, EntryObjectControlManager controlManager)
        {
            localizationManager.RegisterResourceManager(typeof(StrengthTrainingEntryDTO), StrengthTrainingEntryStrings.ResourceManager);
            controlManager.RegisterControl<StrengthTrainingEntryDTO>(typeof(usrStrengthTraining));
            ApplicationSettings.Register(BodyArchitect.Module.StrengthTraining.Options.StrengthTraining.Default);
        }


        public Image ModuleImage
        {
            get { return StrengthTrainingResources.StrengthTrainingModule; }
        }

        public Type EntryObjectType
        {
            get { return typeof(StrengthTrainingEntryDTO); }
        }

        public ICalendarDayContent CalendarDayContent
        {
            get { return new StrengthTrainingCalendarDayContent(); }
        }


        public void CreateGui(IMainWindow mainWnd)
        {
            usrTrainingPlansView plansView = new usrTrainingPlansView();
            var tabPage = mainWnd.AddTabPage(plansView, StrengthTrainingEntryStrings.StrengthTrainingModule_WorkoutPlansTabText, StrengthTrainingResources.TrainingPlan,false);

        }

        public void PrepareNewEntryObject(SessionData sessionData, EntryObjectDTO obj, TrainingDayDTO day)
        {
            
        }
        public void CreateMainMenuItems(MenuStrip menu)
        {
            ToolStripMenuItem mnuExercises = new ToolStripMenuItem(StrengthTrainingEntryStrings.MNUExercises);
            mnuExercises.ShortcutKeys = Keys.F2;
            mnuExercises.Click += new EventHandler(exercisesToolStripMenuItem_Click);
            ((ToolStripMenuItem) menu.Items["mnuTools"]).DropDownOpening += new EventHandler(delegate
                      {
                          mnuExercises.Enabled =UserContext.SessionData !=null;
                      });
            ((ToolStripMenuItem)menu.Items["mnuTools"]).DropDownItems.Add(mnuExercises);


            ToolStripMenuItem mnuPublicationExercises = new ToolStripMenuItem(StrengthTrainingEntryStrings.MNUExerciseForPublication);
            mnuPublicationExercises.ShortcutKeys = Keys.F4;
            mnuPublicationExercises.Click += new EventHandler(mnuPublicationExercises_Click);
            ((ToolStripMenuItem)menu.Items["mnuTools"]).DropDownItems.Add(mnuPublicationExercises);
            
        }

        void mnuPublicationExercises_Click(object sender, EventArgs e)
        {
            usrExercisesBrowser browser = new usrExercisesBrowser();
            var tab=MainWindow.Instance.AddTabPage(browser, StrengthTrainingEntryStrings.MNUExerciseForPublication,null,true);
            tab.ShowCloseButton = DefaultBoolean.True;
            browser.InvokeSearch();
        }

        private void exercisesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExercisesView exercisesView = new ExercisesView();
            exercisesView.ShowDialog();
            ControlHelper.MainWindow.Fill();
        }

        public void AfterSave(SessionData sessionData, TrainingDayDTO day)
        {
            
        }


    }
}
