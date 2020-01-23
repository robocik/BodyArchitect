using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.StrengthTraining.Localization;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using BodyArchitect.Shared;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls
{
    /// <summary>
    /// Interaction logic for PublishWorkoutPlanWindow.xaml
    /// </summary>
    public partial class PublishWorkoutPlanWindow
    {
        private TrainingPlan plan;

        public bool Publish { get; private set; }

        public PublishWorkoutPlanWindow()
        {
            InitializeComponent();

            Loaded += PublishWorkoutPlanWindow_Load;

        }

        public void Fill(TrainingPlan plan, bool publish)
        {
            this.plan = plan;
            Publish = publish;

            if (Publish)
            {

                //TODO:Translate (add info about 30 strength training entries)
                lblDescription.Text = StrengthTrainingEntryStrings.PublishWorkoutPlanWindow_PublishDescription;
                BitmapImage img = "Publish.png".ToResourceUrl().ToBitmap();
                this.usrProgressIndicatorButtons1.OkButton.SetValue(ImageButtonExt.ImageProperty, img);
                imgAddToFavorites.Visibility = Visibility.Collapsed;
                imgPublish.Visibility = Visibility.Visible;
                usrProgressIndicatorButtons1.OkButton.Content = Strings.PublishButton;
            }
            else
            {
                lblDescription.Text = StrengthTrainingEntryStrings.PublishWorkoutPlanWindow_AddToFavoritesDescription;
                Title = StrengthTrainingEntryStrings.PublishWorkoutPlanWindow_AddToFavorites_Text;
                imgAddToFavorites.Visibility = Visibility.Visible;
                imgPublish.Visibility = Visibility.Collapsed;
                BitmapImage img = "AddToFavorites16.png".ToResourceUrl().ToBitmap();
                this.usrProgressIndicatorButtons1.OkButton.SetValue(ImageButtonExt.ImageProperty, img);

                usrProgressIndicatorButtons1.OkButton.Content = Strings.AddButton;
            }
        }

        private void usrProgressIndicatorButtons1_OkClick(object sender, CancellationSourceEventArgs e)
        {

           var param = new WorkoutPlanOperationParam();
           param.WorkoutPlanId = plan.GlobalId;
           if (Publish)
           {
               param.Operation = SupplementsCycleDefinitionOperation.Publish;
           }
           else
           {
               param.Operation = SupplementsCycleDefinitionOperation.AddToFavorites;
           }
            try
            {
                ServiceManager.WorkoutPlanOperation(param);
                WorkoutPlansReposidory.Instance.ClearCache();
                ThreadSafeClose(true);
            }
            catch (ProfileRankException ex)
            {
                UIHelper.Invoke(() => ExceptionHandler.Default.Process(ex, string.Format("PublishWorkoutPlanWindow_ErrProfileRank".TranslateStrength(), Portable.Constants.StrengthTrainingEntriesCount), ErrorWindow.MessageBox), Dispatcher);
                ThreadSafeClose(false);
            }

        }

        private CancellationTokenSource fillExercisesToken;

        private void PublishWorkoutPlanWindow_Load(object sender, EventArgs e)
        {
            fillExercisesToken = RunAsynchronousOperation(delegate(OperationContext ctx)
                                                              {
                                                                  var trainingPlan = plan;
                ctx.CancellatioToken.ThrowIfCancellationRequested();
                Dispatcher.BeginInvoke(new Action(delegate
                     {
                        fill(trainingPlan);
                     }));
                fillExercisesToken = null;
            }, delegate(OperationContext context)
                     {
                         bool start = context.State == OperationState.Started;
                         usrProgressIndicatorButtons1.UpdateProgressIndicator(context);

                         
                         if (start)
                         {
                             baGroupControl1.Header = StrengthTrainingEntryStrings.PublishWorkoutPlanWindow_ExercisesLoading;
                         }
                         else
                         {
                             baGroupControl1.Header = StrengthTrainingEntryStrings.PublishWorkoutPlanWindow_ExercisesHeader;
                             
                         }
                         usrProgressIndicatorButtons1.OkButton.IsEnabled = !start;// && !Publish;
                     });


        }

        private void fill(TrainingPlan trainingPlan)
        {
            List<ExerciseViewModel> items = new List<ExerciseViewModel>();
            foreach (var planDay in trainingPlan.Days)
            {
                foreach (var planEntry in planDay.Entries)
                {
                    var exercise = planEntry.Exercise;
                    var item = new ExerciseViewModel(exercise);
                    items.Add(item);
                }
            }
            lvExercises.ItemsSource = items;
        }

        private void usrProgressIndicatorButtons1_CancelClick(object sender, EventArgs e)
        {
            if (fillExercisesToken != null)
            {
                fillExercisesToken.Cancel();
            }
        }
    }

    public class ExerciseViewModel
    {
        private ExerciseLightDTO exercise;

        public ExerciseViewModel(ExerciseLightDTO exercise)
        {
            this.exercise = exercise;
        }

        public string Name
        {
            get { return Exercise.GetLocalizedName(); }
        }

        public Guid GlobalId
        {
            get { return Exercise.GlobalId; }
        }

        public string ExerciseType
        {
            get { return EnumLocalizer.Default.Translate(Exercise.ExerciseType); }
        }

        public ExerciseLightDTO Exercise
        {
            get { return exercise; }
        }
    }
}
