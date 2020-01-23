using System;
using System.Windows;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls.TrainingPlans
{
    /// <summary>
    /// Interaction logic for usrTrainingPlanSerieEditor.xaml
    /// </summary>
    public partial class usrTrainingPlanSerieEditor 
    {
        //public event EventHandler<ParameterEventArgs<TrainingPlanSerie>> TrainingPlanSetChanged;
        //private TrainingPlanSetViewModel set;
        private TrainingPlanViewModel viewModel;

        public usrTrainingPlanSerieEditor()
        {
            InitializeComponent();

            foreach (SetType value in Enum.GetValues(typeof(SetType)))
            {
                ListItem<SetType> item = new ListItem<SetType>(EnumLocalizer.Default.Translate(value), value);
                cmbSet.Items.Add(item);
            }

            foreach (DropSetType value in Enum.GetValues(typeof(DropSetType)))
            {
                ListItem<DropSetType> item = new ListItem<DropSetType>(EnumLocalizer.Default.Translate(value), value);
                cmbDropSet.Items.Add(item);
            }
        }


        public void Fill(TrainingPlanSetViewModel set,TrainingPlanViewModel viewModel)
        {
            //this.set = set;
            DataContext = set;
            this.viewModel = viewModel;
            //if (set != null)
            //{
            //    cmbSet.SelectedIndex = (int)set.RepetitionsType;
            //    cmbDropSet.SelectedIndex = (int)set.DropSet;
            //    txtRepetitionsRange.Text = set.ToStringRepetitionsRange();

            //    if (set.Comment != null)
            //    {
            //        set.Comment = set.Comment.Replace("\n", "\r\n");
            //    }
            //    txtComment.Text = set.Comment;
            //}
        }

        private void cmbSet_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            viewModel.SetModifiedFlag();
        }

        private void txtRepetitionsRange_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            viewModel.SetModifiedFlag();
        }

        void TimeSpanUpDown_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            viewModel.SetModifiedFlag();
        }

        //private void cmbRepetitionsType_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    set.RepetitionsType = (SetType)cmbSet.SelectedIndex;
        //    trainingPlanSerieChanged();
        //}

        //private void trainingPlanSerieChanged()
        //{
        //    if (TrainingPlanSetChanged != null)
        //    {
        //        TrainingPlanSetChanged(this, new ParameterEventArgs<TrainingPlanSerie>(set));
        //    }
        //}

        //private void txtRepetitionsRange_Validated(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        set.FromString(txtRepetitionsRange.Text);
        //        trainingPlanSerieChanged();
        //    }
        //    catch (Exception ex)
        //    {
        //        
        //        ExceptionHandler.Default.Process(ex,"Value is not correct",ErrorWindow.MessageBox);
        //    }
        //}

        //private void txtComment_Validated(object sender, RoutedEventArgs e)
        //{
        //    set.Comment = txtComment.Text;
        //}

        //private void cmbDropSet_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    set.DropSet = (DropSetType)cmbDropSet.SelectedIndex;
        //    trainingPlanSerieChanged();
        //}

        private void ChkIsSuperSlow_OnChecked(object sender, RoutedEventArgs e)
        {
            viewModel.SetModifiedFlag();
        }
    }

    public class ParameterEventArgs<T> : EventArgs
    {
        private T entry;

        public ParameterEventArgs(T entry)
        {
            this.entry = entry;
        }

        public T Parameter
        {
            get { return entry; }
        }
    }
}
