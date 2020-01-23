using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.UserControls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BodyArchitect.WP7.Pages
{
    public abstract class PreviousEntryObjectPageBase : EntryObjectPageBase
    {
        private Pivot pivot;
        private HeaderControl headerOldTrainingDate;
        private Action<EntryObjectDTO> ShowOldTraining;
        private MultiselectList previewList;
        private EntryObjectDTO oldEntry;

        public void Connect(Pivot pivot, HeaderControl headerOldTrainingDate, Action<EntryObjectDTO> ShowOldTraining, MultiselectList previewList)
        {
            this.pivot = pivot;
            pivot.SelectionChanged -= new SelectionChangedEventHandler(pivot_SelectionChanged);
            pivot.SelectionChanged += new SelectionChangedEventHandler(pivot_SelectionChanged);
            this.previewList = previewList;
            previewList.SelectionChanged -= lstOldItems_SelectionChanged;
            previewList.IsSelectionEnabledChanged -= lstOldItems_IsSelectionEnabledChanged;
            previewList.SelectionChanged += lstOldItems_SelectionChanged;
            previewList.IsSelectionEnabledChanged += lstOldItems_IsSelectionEnabledChanged;
            this.headerOldTrainingDate = headerOldTrainingDate;
            this.ShowOldTraining = ShowOldTraining;
        }

        private void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pivot.SelectedIndex == 2 )
            {
                showOldTraining();
            }
            buildApplicationBar();
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (previewList.IsSelectionEnabled)
            {
                previewList.IsSelectionEnabled = false;
                e.Cancel = true;
                return;
            }
            base.OnBackKeyPress(e);
            
        }

        protected void showOldTraining()
        {
            if (ApplicationState.Current == null)
            {
                return;
            }
            if (oldEntry != null)
            {
                ShowOldTraining(oldEntry);
                return;
            }
            var oldTrainingDate = ApplicationState.Current.TrainingDay.TrainingDay.TrainingDate.AddDays(-6);

            oldEntry = ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays.Values.Where(x => x.TrainingDay.TrainingDate <= oldTrainingDate).SelectMany(x => x.TrainingDay.Objects).OrderByDescending(x => x.TrainingDay.TrainingDate).Where(x => x.GetType() == EntryType && !x.IsSame(Entry)).FirstOrDefault();
            if (oldEntry != null)
            {
                displayOldEntry(oldEntry);    
            }
            else
            {
                btnShowOldPrevious_Click(this, EventArgs.Empty);
            }
        }


        protected void btnShowOldPrevious_Click(object sender, EventArgs e)
        {
            if (pivot.SelectedIndex == 2)
            {
                var list = ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays.Values.SelectMany(x => x.TrainingDay.Objects).OrderByDescending(x => x.TrainingDay.TrainingDate).Where(x => x.GetType() == EntryType && !x.IsSame(Entry)).ToList();
                int position = -1;
                if (oldEntry != null)
                {
                    var currentItem = list.Where(x => x.IsSame(oldEntry)).SingleOrDefault();
                    position = list.IndexOf(currentItem);
                }

                if (list.Count==0 || position == list.Count - 1)
                {
                    BAMessageBox.ShowInfo(ApplicationStrings.StrengthWorkoutPage_NoMoreTrainingsLoaded);
                }
                else
                {
                    var newEntry = list.ElementAt(position + 1);


                    displayOldEntry(newEntry);
                }
                
            }
        }

        protected void btnShowOldNext_Click(object sender, EventArgs e)
        {
            if (pivot.SelectedIndex == 2)
            {
                var list = ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays.Values.SelectMany(x => x.TrainingDay.Objects).OrderByDescending(x => x.TrainingDay.TrainingDate)
                    .Where(x => x.GetType() == EntryType && !x.IsSame(Entry)).ToList();
                int position = 1;
                if (oldEntry != null)
                {
                    var currentItem = list.SingleOrDefault(x => x.IsSame(oldEntry));
                    position = list.IndexOf(currentItem);
                }

                if (list.Count == 0 || position == 0)
                {
                    BAMessageBox.ShowInfo(ApplicationStrings.StrengthWorkoutPage_NoMoreTrainingsLoaded);
                }
                else
                {
                    var newEntry = list.ElementAt(position - 1);

                    displayOldEntry(newEntry);
                }
            }
        }

        private void displayOldEntry(EntryObjectDTO newEntry)
        {
            oldEntry = newEntry;
            headerOldTrainingDate.Text = newEntry.TrainingDay.TrainingDate.ToLongDateString();
            headerOldTrainingDate.Visibility = System.Windows.Visibility.Visible;
            ShowOldTraining(newEntry);
        }

        override protected void buildApplicationBar()
        {
            base.buildApplicationBar();
            if (pivot.SelectedIndex == 2)
            {
                ApplicationBar.Buttons.Clear();
                ApplicationBar.MenuItems.Clear();

                if (previewList.IsSelectionEnabled)
                {
                    var button1 = new ApplicationBarIconButton(new Uri("/icons/copy32.png", UriKind.Relative));
                    button1.Click += new EventHandler(btnCopySelectedOldItems_Click);
                    button1.Text = ApplicationStrings.AppBarButton_Copy;
                    ApplicationBar.Buttons.Add(button1);
                }
                else
                {
                    var button1 = new ApplicationBarIconButton(new Uri("/icons/appbar.back.rest.png", UriKind.Relative));
                    button1.Click += new EventHandler(btnShowOldPrevious_Click);
                    button1.Text = ApplicationStrings.AppBarButton_Previous;
                    ApplicationBar.Buttons.Add(button1);

                    button1 = new ApplicationBarIconButton(new Uri("/icons/appbar.next.rest.png", UriKind.Relative));
                    button1.Click += new EventHandler(btnShowOldNext_Click);
                    button1.Text = ApplicationStrings.AppBarButton_Next;
                    ApplicationBar.Buttons.Add(button1);

                    button1 = new ApplicationBarIconButton(new Uri("/Toolkit.Content/ApplicationBar.Select.png", UriKind.Relative));
                    button1.Click += new EventHandler(btnSelectOldItems_Click);
                    button1.Text = ApplicationStrings.AppBarButton_Select;
                    ApplicationBar.Buttons.Add(button1);  
                }
                
            }
        }

        private void btnSelectOldItems_Click(object sender, EventArgs e)
        {
            //by default select all items
            foreach(var  item in previewList.Items)
            {
                DependencyObject visualItem = previewList.ItemContainerGenerator.ContainerFromItem(item);
                MultiselectItem multiselectItem = visualItem as MultiselectItem;
                multiselectItem.IsSelected = true;
            }
            previewList.IsSelectionEnabled = true;
        }

        private void btnCopySelectedOldItems_Click(object sender, EventArgs e)
        {
            if (!UpgradeAccountControl.EnsureAccountType(ApplicationStrings.Feature_Premium_CopyAllExercisesToNewTraining, this))
            {
                return;
            }

            
            if (oldEntry!=null )
            {
                copyAllImplementation(oldEntry);
                pivot.SelectedIndex = 0;
                previewList.IsSelectionEnabled = false;
            }
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (pivot.SelectedIndex == 2)
            {
                showOldTraining();
            }
        }


        protected abstract void copyAllImplementation(EntryObjectDTO oldEntry);

        private void lstOldItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MultiselectList target = (MultiselectList)sender;
            if (ApplicationBar.Buttons.Count == 0)
            {
                return;
            }
            ApplicationBarIconButton btnCopy = (ApplicationBarIconButton)ApplicationBar.Buttons[0];

            if (target.IsSelectionEnabled)
            {

                if (target.SelectedItems.Count > 0)
                {
                    btnCopy.IsEnabled = true;
                }
                else
                {
                    btnCopy.IsEnabled = false;
                }
            }
        }

        private void lstOldItems_IsSelectionEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            buildApplicationBar();
        }
    }
}
