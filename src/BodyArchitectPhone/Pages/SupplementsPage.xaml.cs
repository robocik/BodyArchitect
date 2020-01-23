using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.Controls.Animations;
using BodyArchitect.WP7.ViewModel;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BodyArchitect.WP7.Pages
{
    public partial class SupplementsPage : PreviousEntryObjectPageBase
    {
        private SupplementsViewModel viewModel;
        

        public SupplementsPage()
        {
            InitializeComponent();
            AnimationContext = LayoutRoot;
            SetControls(progressBar, pivot);
            
        }

        new public SuplementsEntryDTO Entry
        {
            get { return (SuplementsEntryDTO)base.Entry; }
        }

        protected override AnimatorHelperBase GetAnimation(AnimationType animationType, Uri toOrFrom)
        {
            if (pivot.SelectedIndex == 0)
            {
                if (animationType == AnimationType.NavigateForwardIn ||
                    animationType == AnimationType.NavigateBackwardIn)
                    return new SlideUpAnimator() { RootElement = LayoutRoot };
                else
                    return new TurnstileFeatherBackwardOutAnimator() { ListBox = lsItems, RootElement = LayoutRoot };
            }
            else
            {
                if (animationType == AnimationType.NavigateForwardIn || animationType == AnimationType.NavigateBackwardIn)
                    return new SlideUpAnimator() { RootElement = LayoutRoot };
                else
                    return new SlideDownAnimator() { RootElement = LayoutRoot };
            }
        }

        public SuplementDTO SelectedSupplement
        {
            get;
            set;
        }

        protected override Type EntryType
        {
            get { return typeof(SuplementsEntryDTO); }
        }

        override protected void buildApplicationBar()
        {
            base.buildApplicationBar();
            if (viewModel.EditMode && pivot.SelectedIndex==0)
            {
                ApplicationBarIconButton button1 =
                    new ApplicationBarIconButton(new Uri("/icons/appbar.add.rest.png", UriKind.Relative));
                button1.Click += new EventHandler(btnAdd_Click);
                button1.Text = ApplicationStrings.AppBarButton_Add;
                ApplicationBar.Buttons.Add(button1);
            }
        }

        

        protected override void show( bool reload)
        {
            //if (ApplicationState.Current.TrainingDay.TrainingDay.Supplements == null)
            //{
            //    var entry = new SuplementsEntryDTO();
            //    ApplicationState.Current.TrainingDay.TrainingDay.Objects.Add(entry);
            //    entry.TrainingDay = ApplicationState.Current.TrainingDay.TrainingDay;
            //}

            
            viewModel = new SupplementsViewModel(Entry);

            Connect(pivot, headerOldTrainingDate, viewModel.ShowOldTraining,lstOldItems);

            if (SelectedSupplement != null)
            {
                var model = viewModel.AddSupplementItem();
                model.Item.Suplement = SelectedSupplement;
                SelectedSupplement = null;
            }

            DataContext = viewModel;
            headerTrainingDate.Text = viewModel.TrainingDate;

            //if (ApplicationState.Current.Cache.Supplements.IsLoaded || viewModel.Supplements.Count == 0)
            //{
            //    DataContext = viewModel;
            //    headerTrainingDate.Text = viewModel.TrainingDate;
            //}
            //else
            //{
            //    progressBar.ShowProgress(true, ApplicationStrings.SupplementsPage_ProgressRetrieveSupplements);
            //    ApplicationState.Current.Cache.Supplements.Loaded += (s, a) =>
            //    {
            //        if (ApplicationState.Current.Cache.Supplements.IsLoaded)
            //        {
            //            DataContext = viewModel;
                        
            //        }
            //        else
            //        {
            //            BAMessageBox.ShowError(ApplicationStrings.SupplementChooserViewModel_ErrRetrieveSupplements);
            //        }
            //        headerTrainingDate.Text = viewModel.TrainingDate;
            //        progressBar.ShowProgress(false);
            //    };
            //    ApplicationState.Current.Cache.Supplements.Load();
            //}

            buildApplicationBar();

            lblNoSupplements.Visibility = Entry.Items.Count == 0? System.Windows.Visibility.Visible: System.Windows.Visibility.Collapsed;
        }


        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            var page = e.Content as SupplementItemPage;
            if (page != null)
            {
                page.SelectedItem = SelectedItem.Item;
            }
            
        }


        protected override void btnDelete_Click(object sender, EventArgs e)
        {
            if (BAMessageBox.Ask(ApplicationStrings.SupplementsPage_QRemoveSupplement) == MessageBoxResult.OK)
            {
                deleteEntry(Entry);
            }
        }

        public SupplementItemViewModel SelectedItem { get; set; }



        private void btnAdd_Click(object sender, EventArgs e)
        {
            this.Navigate("/Pages/SupplementChooserPage.xaml");
        }

        private void lsItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (e.AddedItems.Count > 0)
            {
                SelectedItem = (SupplementItemViewModel)e.AddedItems[0];
                this.Navigate("/Pages/SupplementItemPage.xaml");
            }
            lsItems.SelectedIndex = -1;
            
        }

        private void mnuDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationState.Current.TrainingDay == null)
            {
                return;
            }
            if (ReadOnly)
            {
                BAMessageBox.ShowError(ApplicationStrings.ErrCannotModifyEntriesOfAnotherUser);
                return;
            }
            var item = (SupplementItemViewModel)(sender as FrameworkElement).DataContext;
            viewModel.Delete(item);
            DataContext = null;
            DataContext = viewModel;
            lblNoSupplements.Visibility =Entry.Items.Count == 0? System.Windows.Visibility.Visible: System.Windows.Visibility.Collapsed;
        }

        private void Menu_Opened(object sender, RoutedEventArgs e)
        {
            LayoutRoot.IsHitTestVisible = false;
            ContextMenu menu = (ContextMenu)sender;
            menu.Visibility = ReadOnly ? Visibility.Collapsed : System.Windows.Visibility.Visible;
        }

        private void Menu_Closed(object sender, RoutedEventArgs e)
        {
            LayoutRoot.IsHitTestVisible = true;
        }

        private void tsShowInReports_Checked(object sender, RoutedEventArgs e)
        {
            tsShowInReports.Content = viewModel.Entry.ReportStatus == ReportStatus.ShowInReport ? ApplicationStrings.ShowInReports : ApplicationStrings.HideInReports;
        }

        private void tsEntryStatus_Checked(object sender, RoutedEventArgs e)
        {
            tsEntryStatus.Content = viewModel.Entry.Status == EntryObjectStatus.Done ? ApplicationStrings.EntryStatusDone : ApplicationStrings.EntryStatusPlanned;
        }

        

        

        //void Supplements_Loaded(object sender, EventArgs e)
        //{
        //    ApplicationState.Current.Cache.Supplements.Loaded -= Supplements_Loaded;
        //    if (ApplicationState.Current.TrainingDay != null)
        //    {
        //        if (ApplicationState.Current.Cache.Supplements.IsLoaded)
        //        {
        //            showOldTraining();
        //        }
        //        else
        //        {
        //            BAMessageBox.ShowError(ApplicationStrings.SupplementChooserViewModel_ErrRetrieveSupplements);
        //        }
        //    }
        //    progressBar.ShowProgress(false);
        //}

        private void mnuCopyPreviousItem_Click(object sender, RoutedEventArgs e)
        {
            var item = (SupplementItemViewModel)(sender as FrameworkElement).DataContext;
            var newItem = item.Item.Copy(true);
            Entry.Items.Add(newItem);
            newItem.SuplementsEntry = Entry;

            show( true);
            pivot.SelectedIndex = 0;
        }

        protected override void copyAllImplementation(EntryObjectDTO oldEntry)
        {
            if (oldEntry != null)
            {
                var itemsToCopy = lstOldItems.SelectedItems.Cast<SupplementItemViewModel>().Select(x => x.Item).ToList();
                foreach (var itemDto in itemsToCopy)
                {
                    var newItem = itemDto.Copy(true);
                    Entry.Items.Add(newItem);
                    newItem.SuplementsEntry = Entry;
                }

                show( true);
            }
        }
    }
}