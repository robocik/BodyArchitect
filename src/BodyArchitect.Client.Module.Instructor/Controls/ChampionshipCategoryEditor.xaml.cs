using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.Instructor.Controls
{
    /// <summary>
    /// Interaction logic for ChampionshipCategoryEditor.xaml
    /// </summary>
    public partial class ChampionshipCategoryEditor
    {
        private ChampionshipCategoryDTO category;

        public ChampionshipCategoryEditor()
        {
            InitializeComponent();
            List<ListItem<ChampionshipWinningCategories>> list = new List<ListItem<ChampionshipWinningCategories>>();
            foreach (ChampionshipWinningCategories cat in Enum.GetValues(typeof(ChampionshipWinningCategories)))
            {
                list.Add(new ListItem<ChampionshipWinningCategories>(EnumLocalizer.Default.Translate(cat), cat));
            }
            
            cmbCategories.ItemsSource = list;
        }

        public ChampionshipCategoryDTO Category
        {
            get { return category; }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            category = new ChampionshipCategoryDTO();
            category.Gender = getGender();
            category.Type = rbTypeOpen.IsChecked.Value ? ChampionshipCategoryType.Open : ChampionshipCategoryType.Weight;
            category.IsAgeStrict = chkIsAgeStrict.IsChecked.Value;
            category.IsOfficial = chkIsOfficial.IsChecked.Value;
            var item=(ListItem < ChampionshipWinningCategories > )cmbCategories.SelectedItem;
            if(item!=null)
            {
                category.Category = item.Value;
            }
            DialogResult = true;
            Close();
        }

        private Gender getGender()
        {
            if(rbGenderAll.IsChecked.Value)
            {
                return Gender.NotSet;
            }
            if(rbGenderMale.IsChecked.Value)
            {
                return Gender.Male;
            }
            return Gender.Female;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            rbGenderMale.IsChecked = true;
            rbTypeOpen.IsChecked = true;
            cmbCategories.SelectedIndex = 0;
            chkIsAgeStrict.IsChecked = true;
            chkIsOfficial.IsChecked = true;
        }

        private void cmbCategories_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var item=(ListItem < ChampionshipWinningCategories > )cmbCategories.SelectedItem;
            pnlIsAgeStrict.SetVisible(item.Value >= ChampionshipWinningCategories.JuniorzyMlodsi &&
                                        item.Value <= ChampionshipWinningCategories.Weterani3);
        }
    }
}
