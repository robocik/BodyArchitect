using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Module.StrengthTraining.Controls;
using BodyArchitect.Client.Module.StrengthTraining.Localization;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Settings;
using Microsoft.Windows.Controls.Ribbon;


namespace BodyArchitect.Client.Module.StrengthTraining
{
    [Export(typeof(IEntryObjectProvider))]
    [Export(typeof(IBodyArchitectModule))]
    public class StrengthTrainingModule : IEntryObjectProvider, IBodyArchitectModule
    {
        public static readonly Guid ModuleId = new Guid("F69099BE-C42A-4EC5-B582-5EFF57758503");
        public Guid GlobalId
        {
            get { return ModuleId; }
        }

        public Type EntryObjectControl
        {
            get { return typeof(usrStrengthTraining); }
        }

        public void Startup( EntryObjectLocalizationManager localizationManager, EntryObjectControlManager controlManager)
        {
            localizationManager.RegisterResourceManager(typeof(StrengthTrainingEntryDTO), StrengthTrainingEntryStrings.ResourceManager);
            //controlManager.RegisterControl<StrengthTrainingEntryDTO>(typeof(usrStrengthTraining));
            ApplicationSettings.Register(StrengthTraining.Default);
        }

        public void AfterUserLogin()
        {
            
        }


        public ImageSource ModuleImage
        {
            get
            {
                return "pack://application:,,,/BodyArchitect.Client.Module.StrengthTraining;component/Images/StrengthTraining.png".ToBitmap();
            }
        }

        public Type EntryObjectType
        {
            get { return typeof(StrengthTrainingEntryDTO); }
        }


        public ShareSocialContent ShareToSocial(EntryObjectDTO entryObj)
        {
            ShareSocialContent content = new ShareSocialContent();
            content.Caption = "StrengthTrainingModule_ShareSocialContent_Caption_BodyArchitect".TranslateStrength();
            content.Name = "StrengthTrainingModule_ShareSocialContent_Name_NewEntry".TranslateStrength();
            StringBuilder descriptionBuilder = new StringBuilder();
            StrengthTrainingEntryDTO strengthEntry = (StrengthTrainingEntryDTO) entryObj;
            descriptionBuilder.AppendFormat("StrengthTrainingModule_ShareSocialContent_Date".TranslateStrength(), strengthEntry.TrainingDay.TrainingDate.ToShortDateString());

            foreach (var exerciseType in strengthEntry.Entries.Where(x=>x.Exercise!=null).Select(x => x.Exercise.ExerciseType).Distinct())
            {
                var exercises=strengthEntry.Entries.Where(x =>x.Exercise!=null && x.Exercise.ExerciseType == exerciseType);
                descriptionBuilder.AppendFormat(@"<br/><b>{0}</b>", EnumLocalizer.Default.Translate(exerciseType));
                foreach (var item in exercises)
                {
                    descriptionBuilder.AppendFormat(@"<br/>{0}: <i>", item.Exercise.Name);
                    foreach (var set in item.Series)
                    {
                        if (set.IsRecord)
                        {
                            descriptionBuilder.AppendFormat(" <b>{0}</b>", set.GetDisplayText(WorkoutPlanOperationHelper.SetDisplayMode.Medium));
                        }
                        else
                        {
                            descriptionBuilder.AppendFormat(" {0}", set.GetDisplayText(WorkoutPlanOperationHelper.SetDisplayMode.Medium));
                        }
                        
                    }
                    descriptionBuilder.Append("</i>");
                }
                descriptionBuilder.Append(@"<br/>");
            }

            content.Description = descriptionBuilder.ToString();
            return content;
        }

        public void CreateRibbon(Ribbon ribbon)
        {
            if (MainWindow.Instance.RibbonHomeTab != null
                && MainWindow.Instance.RibbonHomeTab.Items.Cast<RibbonGroup>().Where(x => x.Name == "StrengthTraining").Count() == 0)
            {
                RibbonGroup ribbonGroup = new RibbonGroup();
                ribbonGroup.Name = "StrengthTraining";
                ribbonGroup.Header = "StrengthTrainingModule_CreateRibbon_Header_StrengthTraining".TranslateStrength();
                MainWindow.Instance.RibbonHomeTab.Items.Add(ribbonGroup);

                var button = new RibbonButton();
                button.Label = "StrengthTrainingModule_CreateRibbon_Label_WorkoutPlans".TranslateStrength();
                button.LargeImageSource = "pack://application:,,,/BodyArchitect.Client.Module.StrengthTraining;component/Images/WorkoutPlan.png".ToBitmap();
                button.Click+=new System.Windows.RoutedEventHandler(button_Click );
                ribbonGroup.Items.Add(button);

                button = new RibbonButton();
                button.SmallImageSource = "pack://application:,,,/BodyArchitect.Client.Module.StrengthTraining;component/Images/Exercises.png".ToBitmap();
                button.Label = "StrengthTrainingModule_CreateRibbon_Label_Exercises".TranslateStrength();
                button.Click += new System.Windows.RoutedEventHandler(btnExercises_Click);
                ribbonGroup.Items.Add(button);
            }
        }


        void btnExercises_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.Module.StrengthTraining;component/Controls/ExercisesView.xaml"));
        }

        void button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.Module.StrengthTraining;component/Controls/TrainingPlansView.xaml"));
        }
    }
}
