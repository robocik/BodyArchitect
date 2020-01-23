using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.Suplements.Controls
{
    public class SupplementsGridViewModel : ViewModelBase
    {
        private List<ListItem<DosageType>> dosageTypes = new List<ListItem<DosageType>>();
        private List<ListItem<TimeType>> timeTypes = new List<ListItem<TimeType>>();
        ObservableCollection<SupplementItemViewModel> items = new ObservableCollection<SupplementItemViewModel>();
        private SuplementsEntryDTO entry;
        private bool readOnly;

        public SupplementsGridViewModel(SuplementsEntryDTO entry, bool readOnly)
        {
            this.readOnly = readOnly;
            this.entry = entry;
            foreach (DosageType type in Enum.GetValues(typeof(DosageType)))
            {
                DosageTypes.Add(new ListItem<DosageType>(EnumLocalizer.Default.Translate(type), type));
            }
            foreach (TimeType type in Enum.GetValues(typeof(TimeType)))
            {
                timeTypes.Add(new ListItem<TimeType>(EnumLocalizer.Default.Translate(type), type));
            }
            foreach (var itemDto in entry.Items.Select(x => new SupplementItemViewModel(x, readOnly)))
            {
                items.Add(itemDto);
            }
            EnsureNewRowAdded();
        }

        public void EnsureNewRowAdded()
        {
            if(readOnly)
            {//in readonly mode we don't need an empty row
                return;
            }
            foreach (var model in Items)
            {
                if (model.IsNew)
                {
                    return;
                }
            }
            
            var i = new SuplementItemDTO();
            //entry.AddItem(i);
            Items.Add(new SupplementItemViewModel(i, readOnly));
            //UpdateReadOnly();
        }

        public ICollection<SuplementDTO>  Supplements
        {
            get { return SuplementsReposidory.Instance.Items.Values; }
        }
        public ObservableCollection<SupplementItemViewModel> Items
        {
            get { return items; }
        }

        public List<ListItem<TimeType>> TimeTypes
        {
            get { return timeTypes; }
        }

        public List<ListItem<DosageType>> DosageTypes
        {
            get { return dosageTypes; }
        }

        public SuplementsEntryDTO Entry
        {
            get { return entry; }
        }

        public void ApplyChanges(SuplementsEntryDTO entry)
        {
            entry.Items.Clear();
            foreach (var supplementItemViewModel in Items)
            {
                if (!supplementItemViewModel.IsNew)
                {
                    entry.AddItem(supplementItemViewModel.GetItem());
                }
            }
        }
    }
}
