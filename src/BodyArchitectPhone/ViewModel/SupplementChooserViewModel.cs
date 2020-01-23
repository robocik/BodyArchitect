using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;

namespace BodyArchitect.WP7.ViewModel
{
    public interface IInGroup
    {
        bool HasItems { get; }
        string Key { get; }
    }

    public class InGroup<T> : List<T>, IInGroup
    {
        public InGroup(string category)
        {
            Key = category;
        }

        public string Key { get; set; }

        public bool HasItems { get { return Count > 0; } }

        public object Tag
        {
            get; set; }
    }

    public class SupplementChooserViewModel : ViewModelBase
    {
        private ObservableCollection<InGroup<SuplementDTO>> _supplements;
        public event EventHandler SupplementsLoaded;
        private static readonly string Groups = "#abcdefghijklmnopqrstuvwxyz";

        public void LoadSupplements()
        {
            ApplicationState.Current.Cache.Supplements.Loaded += new EventHandler(ObjectsReposidory_ExercisesLoaded);
            ApplicationState.Current.Cache.Supplements.Load();
        }

        public ObservableCollection<InGroup<SuplementDTO>> GroupedSupplements
        {
            get
            {
                return _supplements;
            }

            private set
            {
                _supplements = value;
                NotifyPropertyChanged("GroupedSupplements");
            }
        }

        public static string GetFirstNameKey(SuplementDTO suplement)
        {
            char key = char.ToLower(suplement.Name[0]);

            if (key < 'a' || key > 'z')
            {
                key = '#';
            }

            return key.ToString();
        }

        void ObjectsReposidory_ExercisesLoaded(object sender, EventArgs e)
        {
            if (!ApplicationState.Current.Cache.Supplements.IsLoaded)
            {
                onSupplementsLoaded();
                BAMessageBox.ShowError(ApplicationStrings.SupplementChooserViewModel_ErrRetrieveSupplements);
                return;
            }
            if (GroupedSupplements == null)
            {
                GroupedSupplements = new ObservableCollection<InGroup<SuplementDTO>>();
            }
            else
            {
                GroupedSupplements.Clear();
            }

            Dictionary<string, InGroup<SuplementDTO>> groups = new Dictionary<string, InGroup<SuplementDTO>>();

            foreach (char c in Groups)
            {
                var group = new InGroup<SuplementDTO>(c.ToString());
                GroupedSupplements.Add(group);
                groups[c.ToString()] = group;
            }

            foreach (var supplement in ApplicationState.Current.Cache.Supplements.Items.Values.OrderBy(x => x.Name))
            {
                groups[GetFirstNameKey(supplement)].Add(supplement);
            }


            onSupplementsLoaded();
            
        }

        private void onSupplementsLoaded()
        {
            ApplicationState.Current.Cache.Supplements.Loaded -= new EventHandler(ObjectsReposidory_ExercisesLoaded);
            if (SupplementsLoaded != null)
            {
                SupplementsLoaded(this, EventArgs.Empty);
            }
        }

        public void Refresh()
        {
            ApplicationState.Current.Cache.Supplements.Loaded += new EventHandler(ObjectsReposidory_ExercisesLoaded);
            ApplicationState.Current.Cache.Supplements.Refresh();
        }
    }
}
