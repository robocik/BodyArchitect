using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.UI.UserControls
{
    /// <summary>
    /// Interaction logic for TrainingDayCommentsControl.xaml
    /// </summary>
    public partial class TrainingDayCommentsControl
    {
        private bool _readOnly;
        private TrainingDayDTO day;
        private bool isCommentsAvailable;

        public TrainingDayCommentsControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public event EventHandler ObjectChanged;

        void onObjectChanged()
        {
            if (ObjectChanged != null)
            {
                ObjectChanged(this, EventArgs.Empty);
            }
        }

        public bool ReadOnly
        {
            get { return _readOnly; }
            set
            {
                _readOnly = value;
                grAllowComments.SetVisible(!ReadOnly);
                NotifyOfPropertyChange(()=>ReadOnly);
            }
        }

        public bool IsCommentsAvailable
        {
            get { return isCommentsAvailable; }
            set
            {
                isCommentsAvailable = value;
                NotifyOfPropertyChange(() => IsCommentsAvailable);
            }
        }

        public bool AllowComments
        {
            get { return cmbAllowComments.SelectedIndex==0; }
            set
            {
                cmbAllowComments.SelectedIndex = value?0:1;
                NotifyOfPropertyChange(() => AllowComments);
            }
        }

        public void Fill(TrainingDayDTO day)
        {
            this.day = day;
            blogCommentsList1.Fill(day);
            cmbAllowComments.SelectedIndex = day.AllowComments ? 0 : 1;
            IsCommentsAvailable = !day.IsNew && day.AllowComments;
        }

        void asyncOperationStateChange(OperationContext context)
        {
            bool startLoginOperation = context.State == OperationState.Started;
            btnSend.IsEnabled = !startLoginOperation && !string.IsNullOrWhiteSpace(txtComment.Text);
            progressIndicator.IsRunning = startLoginOperation;
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            TrainingDayCommentOperationParam arg = new TrainingDayCommentOperationParam();
            arg.TrainingDayId = day.GlobalId;
            arg.Comment = new TrainingDayCommentDTO();
            arg.Comment.Comment = txtComment.Text;
            arg.Comment.Profile = UserContext.Current.CurrentProfile;

            ParentWindow.RunAsynchronousOperation(delegate
            {
                try
                {
                    ServiceManager.TrainingDayCommentOperation(arg);
                }
                catch (InvalidOperationException ex)
                {
                    UIHelper.Invoke(() => ExceptionHandler.Default.Process(ex, "ErrSendComment_MustSaveDayFirst".TranslateStrings(), ErrorWindow.MessageBox), Dispatcher);
                    return;
                }

                Dispatcher.Invoke(new Action(delegate
                {
                    if (day != null && !day.IsNew)
                    {
                        blogCommentsList1.Fill(day);
                    }
                    txtComment.Text = string.Empty;
                }));


            }, asyncOperationStateChange);
        }

        private void rowSplitter_IsChanging(object sender, CancelEventArgs e)
        {
            if (rowSplitter.IsCollapsed)
            {//user is trying to expand this when comments are disabled
                e.Cancel = day.IsNew || cmbAllowComments.SelectedIndex == 1; //disable comments
            }
        }
        private void cmbAllowComments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (day != null)
            {
                onObjectChanged();
                //SetModifiedFlag();
                rowSplitter.Process(day.IsNew || cmbAllowComments.SelectedIndex == 1);
            }
        }

        private void txtComment_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnSend.IsEnabled = !string.IsNullOrWhiteSpace(txtComment.Text);
        }

    }
}
