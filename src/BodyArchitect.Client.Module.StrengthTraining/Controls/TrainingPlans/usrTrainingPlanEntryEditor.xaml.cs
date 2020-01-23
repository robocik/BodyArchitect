using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls.TrainingPlans
{
    /// <summary>
    /// Interaction logic for usrTrainingPlanEntryEditor.xaml
    /// </summary>
    public partial class usrTrainingPlanEntryEditor
    {
        private TrainingPlanEntryViewModel entry;
        private TrainingPlanViewModel viewModel;
        public event EventHandler<ParameterEventArgs<TrainingPlanEntry>> TrainingPlanChanged;


        public usrTrainingPlanEntryEditor()
        {
            InitializeComponent();
            List<ListItem<ExerciseDoneWay>> list = new List<ListItem<ExerciseDoneWay>>();
            foreach (ExerciseDoneWay doneWay in Enum.GetValues(typeof(ExerciseDoneWay)))
            {
                list.Add(new ListItem<ExerciseDoneWay>(EnumLocalizer.Default.Translate(doneWay), doneWay));
            }
            cmbExerciseDoneWay.ItemsSource = list;
        }


        public void Fill(TrainingPlanEntryViewModel entry,TrainingPlanViewModel viewModel)
        {
            this.entry = entry;
            this.viewModel = viewModel;
            cmbExercises.SelectedIndex = -1;
            cmbExercises.ClearFilter();
            if (entry != null)
            {
                cmbExerciseDoneWay.SelectedValue = entry.Entry.DoneWay;
                cmbExercises.SelectedValue = entry.Entry.Exercise;

                txtRestTime.Value = entry.Entry.RestSeconds;
                if (entry.Entry.Comment != null)
                {
                    entry.Entry.Comment = entry.Entry.Comment.Replace("\n", "\r\n");
                }
                txtComment.Text = entry.Entry.Comment;
            }
        }

        private void cmbExercises_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbExercises.SelectedValue==null)
            {
                return;
            }
            var oldCardio = entry.Entry.Exercise == null || entry.Entry.Exercise.ExerciseType == ExerciseType.Cardio;
            entry.Entry.Exercise = (ExerciseLightDTO)cmbExercises.SelectedValue;
            var newCardio = entry.Entry.Exercise.ExerciseType == ExerciseType.Cardio;
            if (oldCardio!=newCardio)
            {
                entry.Entry.Sets.Clear();
                entry.Sets.Clear();
            }
            viewModel.SetModifiedFlag();
            if (TrainingPlanChanged != null)
            {
                TrainingPlanChanged(this, new ParameterEventArgs<TrainingPlanEntry>(entry.Entry));
            }
        }

        private void txtRestTime_TextChanged(object sender, EventArgs e)
        {
            entry.Entry.RestSeconds = txtRestTime.Value;
            viewModel.SetModifiedFlag();
        }

        private void txtComment_TextChanged(object sender, EventArgs e)
        {
            entry.Entry.Comment = txtComment.Text;
            viewModel.SetModifiedFlag();
        }

        private void cmbExerciseDoneWay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbExerciseDoneWay.SelectedValue == null)
            {
                return;
            }
            entry.Entry.DoneWay = (ExerciseDoneWay)cmbExerciseDoneWay.SelectedValue;
            viewModel.SetModifiedFlag();
        }

    }
}
