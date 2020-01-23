using System;
using System.Windows.Media;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.UI.UserControls
{
    /// <summary>
    /// Interaction logic for usrMyTrainingStatus.xaml
    /// </summary>
    public partial class usrMyTrainingStatus
    {
        private MyTrainingLightDTO myTraining;
        private bool readOnly;

        public usrMyTrainingStatus()
        {
            InitializeComponent();
        }

        private Color getStatusColor(MyTrainingLightDTO value)
        {
            if (value.TrainingEnd == TrainingEnd.Complete)
            {
                if(value.PercentageCompleted==100)
                {
                    return Colors.Green;    
                }
                return Colors.Red;
            }
            return Colors.Blue;
        }

        public void Fill(MyTrainingLightDTO myTraining)
        {
            this.myTraining = myTraining;
            lblTrainingState.Text = EnumLocalizer.Default.Translate(myTraining.TrainingEnd);
            lblTrainingState.Foreground =new SolidColorBrush(getStatusColor(myTraining));
            lblPercentageResult.Text = string.Format(Strings.CompletePercentageResult, myTraining.PercentageCompleted);
            tbStart.Text = myTraining.StartDate.ToShortDateString();
            if (myTraining.EndDate.HasValue)
            {
                lblEndDate.Visibility = System.Windows.Visibility.Visible;
                tbEnd.Visibility = System.Windows.Visibility.Visible;
                btnAbortMyTraining.Visibility = System.Windows.Visibility.Collapsed;
                tbEnd.Text = myTraining.EndDate.Value.ToShortDateString();
            }
            else
            {
                lblEndDate.Visibility = System.Windows.Visibility.Collapsed;
                tbEnd.Visibility = System.Windows.Visibility.Collapsed;
                btnAbortMyTraining.Visibility = System.Windows.Visibility.Visible;
            }

        }

        private void btnAbortMyTraining_Click(object sender, EventArgs e)
        {
            myTraining.Complete();
            ReadOnly = true;
            Fill(myTraining);
            setModifiedFlag();
        }

        public bool ReadOnly
        {
            get { return readOnly; }
            set
            {
                readOnly = value;
                btnAbortMyTraining.IsEnabled = !readOnly && (myTraining == null || myTraining.TrainingEnd == TrainingEnd.NotEnded);
            }
        }

        private void setModifiedFlag()
        {
            if (Parent != null)
            {

                var mainWnd = UIHelper.FindVisualParent<usrEntryObjectDetailsBase>(Parent);
                mainWnd.SetModifiedFlag();
            }
        }
    }
}
