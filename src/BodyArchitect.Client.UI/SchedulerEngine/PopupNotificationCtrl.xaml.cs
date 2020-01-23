using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;
using Hardcodet.Wpf.TaskbarNotification;

namespace BodyArchitect.Client.UI.SchedulerEngine
{
    /// <summary>
    /// Interaction logic for PopupNotificationCtrl.xaml
    /// </summary>
    public partial class PopupNotificationCtrl
    {
        private bool isClosing = false;
        public PopupNotificationCtrl()
        {
            InitializeComponent();
            TaskbarIcon.AddBalloonClosingHandler(this, OnBalloonClosing);
        }
        /// <summary>
        /// By subscribing to the <see cref="TaskbarIcon.BalloonClosingEvent"/>
        /// and setting the "Handled" property to true, we suppress the popup
        /// from being closed in order to display the fade-out animation.
        /// </summary>
        private void OnBalloonClosing(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            isClosing = true;
        }

        /// <summary>
        /// Closes the popup once the fade-out animation completed.
        /// The animation was triggered in XAML through the attached
        /// BalloonClosing event.
        /// </summary>
        private void OnFadeOutCompleted(object sender, EventArgs e)
        {
            
            Popup pp = (Popup)Parent;
            pp.IsOpen = false;
        }

        private ObservableCollection<BAGlobalObject> notifyContent;
        /// <summary>
        /// A collection of NotifyObjects that the main window can add to.
        /// </summary>
        public ObservableCollection<BAGlobalObject> NotifyContent
        {
            get
            {
                if (this.notifyContent == null)
                {
                    // Not yet created.
                    // Create it.
                    this.NotifyContent = new ObservableCollection<BAGlobalObject>();
                }

                return this.notifyContent;
            }
            set
            {
                this.notifyContent = value;
            }
        }

        private void Item_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink obj = (Hyperlink)sender;
            NotifyObject notifyObject = (NotifyObject)obj.Tag;
            if (notifyObject.ClickEvent != null)
            {
                notifyObject.ClickEvent(notifyObject.Tag);
            }
        }

        private void btnClosePopup_Click(object sender, RoutedEventArgs e)
        {
            TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon.CloseBalloon();
            NotifyContent.Clear();
        }

        private void grid_MouseEnter(object sender, MouseEventArgs e)
        {
            //if we're already running the fade-out animation, do not interrupt anymore
            //(makes things too complicated for the sample)
            if (isClosing) return;

            //the tray icon assigned this attached property to simplify access
            TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon.ResetBalloonCloseTimer();
            
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var notifyObject = (NotifyObject)btn.Tag;
            notifyObject.DeleteEvent(notifyObject);
            NotifyContent.Remove(notifyObject);
            if(NotifyContent.Count==0)
            {
                //if we removed last item then we can close this popup
                TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
                taskbarIcon.CloseBalloon();
            }
        }

        private void grid_MouseLeave(object sender, MouseEventArgs e)
        {
            //TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            //taskbarIcon.CloseBalloon();
        }
    }
}
