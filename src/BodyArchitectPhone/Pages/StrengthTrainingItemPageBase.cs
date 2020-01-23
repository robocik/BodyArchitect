using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.Controls.Animations;
using BodyArchitect.WP7.UserControls;
using BodyArchitect.WP7.ViewModel;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BodyArchitect.WP7.Pages
{
    public class StrengthTrainingItemPageBase : StrengthTrainingPageBase, IExerciseListInvoker
    {
        private ReorderListBox list;
        private TextBlock lblNoSets;
        private TimerControl ctrlTimer;
        public StrengthTrainingItemViewModel SelectedItemView { get; set; }
        private ApplicationBarMenuItem mnuReorder;

        public StrengthTrainingItemDTO SelectedItem { get; set; }

        public void SetControls(ReorderListBox lstSets, TextBlock lblNoSets, Grid LayoutRoot, TimerControl ctrlTimer)
        {
            this.list = lstSets;
            list.ListReordered -= lsItems_ListReordered;
            list.ListReordering -= lsItems_ListReordering;
            list.ListReordered += lsItems_ListReordered;
            list.ListReordering += lsItems_ListReordering;
            this.ctrlTimer = ctrlTimer;
            this.lblNoSets = lblNoSets;
            AnimationContext = LayoutRoot;
            buildApplicationBar();
        }

        

        protected override AnimatorHelperBase GetAnimation(AnimationType animationType, Uri toOrFrom)
        {
            if (animationType == AnimationType.NavigateForwardIn || animationType == AnimationType.NavigateBackwardIn)
                return new SlideUpAnimator() { RootElement = AnimationContext };
            else
                return new TurnstileFeatherBackwardOutAnimator() { ListBox = list, RootElement = AnimationContext };
        }

        public virtual void btnAdd_Click(object sender, System.EventArgs e)
        {
            Add_Click();
        }

        public virtual void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteItem_Click();
        }

        public virtual void btnComment_Click(object sender, EventArgs e)
        {
            Comment_Click();
        }

        public virtual void btnJoinExercise_Click(object sender, EventArgs e)
        {
            JoinExercise_Click();
        }

        void buildApplicationBar()
        {
            ApplicationBar.Buttons.Clear();
            ApplicationBar.MenuItems.Clear();

            var menu = new ApplicationBarMenuItem(ApplicationStrings.AppBarButton_More);
            menu.Click += new EventHandler(btnComment_Click);
            ApplicationBar.MenuItems.Add(menu);

            if(EditMode)
            {
                ApplicationBarIconButton button1 =
                new ApplicationBarIconButton(new Uri("/icons/appbar.add.rest.png", UriKind.Relative));
                button1.Click += new EventHandler(btnAdd_Click);
                button1.Text = ApplicationStrings.AppBarButton_Add;
                ApplicationBar.Buttons.Add(button1);

                button1 = new ApplicationBarIconButton(new Uri("/icons/appbar.delete.rest.png", UriKind.Relative));
                button1.Click += new EventHandler(btnDelete_Click);
                button1.Text = ApplicationStrings.AppBarButton_Delete;
                ApplicationBar.Buttons.Add(button1);

                menu = new ApplicationBarMenuItem(ApplicationStrings.AppBarMenu_Join);
                menu.Click += new EventHandler(btnJoinExercise_Click);
                ApplicationBar.MenuItems.Add(menu);

                mnuReorder = new ApplicationBarMenuItem(this.list.IsReorderEnabled ? ApplicationStrings.AppBarButton_StopReorder : ApplicationStrings.AppBarButton_StartReorder);
                mnuReorder.Click += new EventHandler(mnuReorder_Click);
                ApplicationBar.MenuItems.Add(mnuReorder);
                ApplicationBar.Mode = ApplicationBarMode.Default;
            }
            else
            {
                ApplicationBar.Mode = ApplicationBarMode.Minimized;
            }
        }

        private void mnuReorder_Click(object sender, EventArgs e)
        {
            this.list.IsReorderEnabled = !this.list.IsReorderEnabled;
            mnuReorder.Text = this.list.IsReorderEnabled
                                  ? ApplicationStrings.AppBarButton_StopReorder
                                  : ApplicationStrings.AppBarButton_StartReorder;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            StateHelper stateHelper = new StateHelper(this.State);
            var item = stateHelper.GetValue<Guid>("SelectedItemId", Guid.Empty);
            if (item != Guid.Empty && (SelectedItem == null || SelectedItem.InstanceId == item))
            {
                //SelectedItem = ApplicationState.Current.TrainingDay.TrainingDay.StrengthWorkout.GetStrengthTrainingItem(item);
                SelectedItem = Entry.GetStrengthTrainingItem(item);

            }
            fill();

            base.OnNavigatedTo(e);

            if (SelectedExercise != null)
            {//join exercise in superset
                SelectedItemView.JoinExercise(SelectedExercise.GlobalId);
                SelectedExercise = null;

            }
            ctrlTimer.IsStarted = EditMode && ApplicationState.Current.IsTimerEnabled;
        }

        private void fill()
        {
            SelectedItemView = new StrengthTrainingItemViewModel(SelectedItem);
            DataContext = SelectedItemView;


            lblNoSets.Visibility = SelectedItem.Series.Count == 0
                                       ? System.Windows.Visibility.Visible
                                       : System.Windows.Visibility.Collapsed;
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.State["SelectedItemId"] = SelectedItem.InstanceId;
            var page = e.Content as SetPageBase;
            if (page != null)
            {
                page.SelectedSet = SelectedItemView.SelectedSet.Set;
            }

            var commentPage = e.Content as StrengthTrainingItemOptionsPage;
            if (commentPage != null)
            {
                commentPage.CommentableObject = SelectedItem;
            }
            ctrlTimer.IsStarted = false;
            base.OnNavigatedFrom(e);
        }


        protected virtual void NavigateToSetPage()
        {
            this.Navigate("/Pages/NewSetPage.xaml");
        }

        protected virtual void Add_Click()
        {
            lblNoSets.Visibility = Visibility.Collapsed;
            var newSet = SelectedItemView.AddNewSet();
            SelectedItemView.SelectedSet = newSet;
            list.UpdateLayout();
            list.ScrollIntoView(newSet);
            NavigateToSetPage();
        }

        protected virtual void DeleteItem_Click()
        {
            SelectedItemView.Delete();
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        protected virtual void DeleteSet_Click(object sender, RoutedEventArgs e)
        {
            if (!EditMode)
            {
                BAMessageBox.ShowError(ApplicationStrings.ErrCannotModifyEntriesOfAnotherUser);
                return;
            }
            var item = (SetViewModel)(sender as FrameworkElement).DataContext;
            item.Delete();
            //SelectedItemView.Delete(item);
            //DataContext = null;
            //DataContext = SelectedItemView;
            fill();
        }

        protected virtual void Menu_Opened(ContextMenu menu)
        {
            menu.Visibility = !EditMode ? Visibility.Collapsed : System.Windows.Visibility.Visible;
            IsHitTestVisible = false;
        }

        protected virtual void Menu_Closed()
        {
            IsHitTestVisible = true;
        }

        protected virtual void lstSets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                SelectedItemView.SelectedSet = (SetViewModel)e.AddedItems[0];

                NavigateToSetPage();
            }
            list.SelectedIndex = -1;
        }

        protected virtual void Comment_Click()
        {
            this.Navigate("/Pages/StrengthTrainingItemOptionsPage.xaml");
        }

        protected virtual void JoinExercise_Click()
        {
            this.Navigate("/Pages/ExerciseTypePage.xaml?Selector=true");
        }

        public ExerciseDTO SelectedExercise
        {
            get;
            set;
        }

        private void lsItems_ListReordered(object sender, ListReorderedEventArgs e)
        {
            SelectedItemView.Item.Series.RemoveAt(e.FromIndex);
            SelectedItemView.Item.Series.Insert(e.ToIndex,((SetViewModel)e.Object).Set);
            SelectedItemView.Refresh();
        }

        private void lsItems_ListReordering(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!UpgradeAccountControl.EnsureAccountType(ApplicationStrings.Feature_Premium_MoreFeatures, this))
            {
                e.Cancel = true;
            }
        }
    }
}
