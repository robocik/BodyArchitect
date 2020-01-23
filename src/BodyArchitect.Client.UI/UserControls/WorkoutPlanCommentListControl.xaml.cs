using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.UI.UserControls
{
    /// <summary>
    /// Interaction logic for WorkoutPlanCommentListControl.xaml
    /// </summary>
    public partial class WorkoutPlanCommentListControl
    {
        public WorkoutPlanCommentListControl()
        {
            InitializeComponent();
            DataContext = this;
            AllowRedirectToDetails = true;
        }

        [DefaultValue(true)]
        public bool AllowRedirectToDetails { get; set; }

        public ItemCollection Items
        {
            get { return lstInvitations.Items; }
        }

        public void Fill(IEnumerable<CommentEntryDTO> comments)
        {
            lstInvitations.ItemsSource = comments;
        }
        private void btnUserInfo_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)e.OriginalSource;
            UserDTO user = (UserDTO)btn.Tag;
            if (!user.IsDeleted && !user.IsMe() && AllowRedirectToDetails)
            {
                MainWindow.Instance.ShowUserInformation(user);
            }
        }
    }
}
