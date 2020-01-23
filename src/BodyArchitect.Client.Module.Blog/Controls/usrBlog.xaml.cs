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
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Module.Blog.Options;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.UserControls;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;

namespace BodyArchitect.Client.Module.Blog.Controls
{
    /// <summary>
    /// Interaction logic for usrBlog.xaml
    /// </summary>
    public partial class usrBlog : IEntryObjectControl
    {

        public BlogEntryDTO BlogEntry
        {
            get { return (BlogEntryDTO) Entry; }
        }

        public usrBlog()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(usrBlog_Loaded);
            Unloaded += new RoutedEventHandler(usrBlog_Unloaded);
        }

        void usrBlog_Unloaded(object sender, RoutedEventArgs e)
        {
            if (UserContext.Current.CurrentProfile != null)
            {
                BlogSettings.Default.SetVisitDate(BlogEntry.TrainingDay.TrainingDate,
                                                  UserContext.Current.CurrentProfile.GlobalId, BlogEntry.TrainingDay.ProfileId,
                                                  DateTime.Now);
            }
        }

        void usrBlog_Loaded(object sender, RoutedEventArgs e)
        {

            var parent = UIHelper.FindVisualParent<TrainingDayWindow>(this);
            if (parent != null)
            {
                parent.TrainingDayChanged += new EventHandler<TrainingDayChangedEventArgs>(parent_TrainingDayChanged);
            }
        }

        void parent_TrainingDayChanged(object sender, TrainingDayChangedEventArgs e)
        {
            BlogSettings.Default.SetVisitDate(e.OldDate, UserContext.Current.CurrentProfile.GlobalId, BlogEntry.TrainingDay.ProfileId, DateTime.Now);
        }

        protected override void FillImplementation(SaveTrainingDayResult originalEntry)
        {
            usrHtmlEditor1.ClearContent();
            usrApplicationName.Fill(BlogEntry);
            usrHtmlEditor1.SetHtml(BlogEntry.Comment);
            //cmbAllowComments.SelectedIndex = blogEntry.AllowComments ? 0 : 1;
            updateReadOnly();
        }

        protected override void UpdateEntryObjectImplementation()
        {
            BlogEntry.Comment = usrHtmlEditor1.GetHtml();
        }


        void updateReadOnly()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action(updateReadOnly));
            }
            else
            {
                usrHtmlEditor1.ReadOnly = ReadOnly;
                
            }

        }

        public override void AfterSave(bool isWindowClosing)
        {
            updateReadOnly();
        }
       

        private void usrHtmlEditor1_IsModifiedChanged(object sender, EventArgs e)
        {
            SetModifiedFlag();
        }
    }
}
