using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.Instructor.ViewModel;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor.Controls
{
    /// <summary>
    /// Interaction logic for ChampionshipsView.xaml
    /// </summary>
    public partial class ChampionshipsView : IWeakEventListener
    {
        ObservableCollection<ChampionshipViewModel> championships = new ObservableCollection<ChampionshipViewModel>();
        private bool canEdit;
        private ChampionshipViewModel selectedChampionship;

        public ChampionshipsView()
        {
            InitializeComponent();
            DataContext = this;
            CollectionChangedEventManager.AddListener(ChampionshipsReposidory.Instance, this);
        }

        public bool CanEdit
        {
            get { return canEdit; }
            set
            {
                canEdit = value;
                NotifyOfPropertyChange(()=>CanEdit);
            }
        }

        public ChampionshipViewModel SelectedChampionship
        {
            get { return selectedChampionship; }
            set
            {
                selectedChampionship = value;
                CanEdit = selectedChampionship != null && selectedChampionship.Championship.State == ScheduleEntryState.Done;
                NotifyOfPropertyChange(()=>SelectedChampionship);
            }
        }

        public override void Fill()
        {
            Header = InstructorStrings.ChampionshipsView_Header;

            FillItems();
        }

        public void FillItems()
        {
            IsInProgress = true;
            
            ParentWindow.RunAsynchronousOperation(delegate(OperationContext ctx)
            {
                ChampionshipsReposidory.Instance.EnsureLoaded();

                Dispatcher.BeginInvoke(new Action(delegate
                {
                    championships.Clear();
                    foreach (var item in ChampionshipsReposidory.Instance.Items.Values)
                    {
                        championships.Add(new ChampionshipViewModel(item));
                    }
                    IsInProgress = false;
                }));

            }, null);
        }


        public ICollection<ChampionshipViewModel> Championships
        {
            get { return championships; }
        }

        public override void RefreshView()
        {
            ChampionshipsReposidory.Instance.ClearCache();
            FillItems();
        }

        public override Uri HeaderIcon
        {
            get { return new Uri("pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Images/Championship16.png", UriKind.Absolute); }
        }

        private void rbtnNew_Click(object sender, RoutedEventArgs e)
        {
            BAMessageBox.ShowInfo(InstructorStrings.ChampionshipsView_CreateNew_Message);
        }

        private void rbtnEdit_Click(object sender, RoutedEventArgs e)
        {
            editChampionship();
        }

        private void editChampionship()
        {
            if (!UIHelper.EnsureInstructorLicence())
            {
                return;
            }
            MainWindow.Instance.ShowPage(
                new Uri("pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Controls/ChampionshipView.xaml"),
                () => new ChampionshipPageContext(SelectedChampionship.Championship));
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            UIHelper.BeginInvoke(FillItems, Dispatcher);

            return true;
        }

        private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = (ChampionshipViewModel)lvChampionships.GetClickedItem(e);
            if (item != null && item.IsDone)
            {
                editChampionship();
            }
        }
    }
}
