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
using BodyArchitect.Controls;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Service.Model;

namespace BodyArchitect.Module.StrengthTraining.Controls.WPF
{
    /// <summary>
    /// Interaction logic for WorkoutPlanCommentListControl.xaml
    /// </summary>
    public partial class WorkoutPlanCommentListControl : UserControl
    {
        public WorkoutPlanCommentListControl()
        {
            InitializeComponent();
        }

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
            if (!user.IsDeleted && !user.IsMe())
            {
                MainWindow.Instance.ShowUserInformation(user);
            }
        }
    }
}
