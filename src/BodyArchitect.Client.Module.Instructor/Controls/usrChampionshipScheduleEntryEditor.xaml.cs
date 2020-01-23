using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using BodyArchitect.Client.Module.Instructor.ViewModel;
using BodyArchitect.Client.UI;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor.Controls
{
    /// <summary>
    /// Interaction logic for usrChampionshipScheduleEntryEditor.xaml
    /// </summary>
    public partial class usrChampionshipScheduleEntryEditor : IEditableControl
    {
        private ObservableCollection<ChampionshipCategoryViewModel> categories = new ObservableCollection<ChampionshipCategoryViewModel>();

        public usrChampionshipScheduleEntryEditor()
        {
            InitializeComponent();
            updateGui();
        }

        private ScheduleChampionshipDTO entry;


        public void Fill(ScheduleEntryBaseDTO entry)
        {
            Object = entry;
            foreach (var categoryDto in this.entry.Categories)
            {
                WinCategories.Add(new ChampionshipCategoryViewModel(categoryDto));
            }
        }

        public object Object
        {
            get { return entry; }
            set
            {
                entry = (ScheduleChampionshipDTO)value;
                DataContext = entry;
            }
        }

        void updateGui()
        {
            btnDeleteCategory.IsEnabled = lstCategories.SelectedIndex > -1;
        }

        public bool ReadOnly
        {
            get;
            set;
        }

        public ICollection<ChampionshipCategoryViewModel> WinCategories
        {
            get { return categories; }
        }

        public object Save()
        {
            entry.Categories.Clear();
            foreach (var category in WinCategories)
            {
                entry.Categories.Add(category.Category);
            }
            return entry;
        }

        private void btnAddCategory_Click(object sender, RoutedEventArgs e)
        {
            ChampionshipCategoryEditor dlg = new ChampionshipCategoryEditor();
            if(dlg.ShowDialog()==true)
            {
                if (categories.Where(x => x.Category.IsTheSame(dlg.Category)).Count() > 0)
                {
                    BAMessageBox.ShowError(InstructorStrings.usrChampionshipScheduleEntryEditor_ErrCategoryAlreadySelected);
                    return;
                }
                categories.Add(new ChampionshipCategoryViewModel(dlg.Category));
            }
        }

        private void btnDeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            categories.Remove((ChampionshipCategoryViewModel) lstCategories.SelectedItem);
        }

        private void LstCategories_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updateGui();
        }
    }
}
