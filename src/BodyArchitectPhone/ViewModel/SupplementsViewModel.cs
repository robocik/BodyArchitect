using System;
using System.Collections.ObjectModel;
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

namespace BodyArchitect.WP7.ViewModel
{
    public class SupplementsViewModel
    {
        private SuplementsEntryDTO suplementsEntryDTO;
        private ObservableCollection<SupplementItemViewModel> _supplements;
        private ObservableCollection<SupplementItemViewModel> _oldSupplements=new ObservableCollection<SupplementItemViewModel>();

        public SupplementsViewModel(SuplementsEntryDTO suplementsEntryDTO)
        {
            this.suplementsEntryDTO = suplementsEntryDTO;
            _supplements=new ObservableCollection<SupplementItemViewModel>();
            foreach (var item in suplementsEntryDTO.Items)
            {
                _supplements.Add(new SupplementItemViewModel(item));
            }
        }

        public SupplementItemViewModel AddSupplementItem(SuplementDTO supplement=null)
        {
            var item = new SuplementItemDTO();
            item.Suplement = supplement;
            item.Time.DateTime = DateTime.Now;
            item.SuplementsEntry = Entry;
            Entry.Items.Add(item);
            var viewModel = new SupplementItemViewModel(item);
            _supplements.Add(viewModel);
            return viewModel;
        }

        public ObservableCollection<SupplementItemViewModel> Supplements
        {
            get { return _supplements; }
        }

        public string TrainingDate
        {
            get { return Entry.TrainingDay.TrainingDate.ToLongDateString(); }
        }

        public SuplementsEntryDTO Entry
        {
            get { return suplementsEntryDTO; }
        }

        public bool EditMode
        {
            get { return Entry.TrainingDay.IsMine; }
        }

        public void Delete(SupplementItemViewModel item)
        {
            Entry.Items.Remove(item.Item);
            _supplements.Remove(item);
        }

        public ObservableCollection<SupplementItemViewModel> OldSupplements
        {
            get { return _oldSupplements; }
        }


        public void ShowOldTraining(EntryObjectDTO oldEntry)
        {
            var oldSupple = (SuplementsEntryDTO) oldEntry;
            OldSupplements.Clear();
            //if(ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays.ContainsKey(oldDate))
            //{
            //    var oldDay = ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays[oldDate];
            //    if (oldEntry.Supplements != null)
            //    {
            //        foreach (var item in oldDay.TrainingDay.Supplements.Items)
            //        {
            //            OldSupplements.Add(new SupplementItemViewModel(item));
            //        }
            //    }
            //}
            if (oldSupple != null)
            {
                foreach (var item in oldSupple.Items)
                {
                    OldSupplements.Add(new SupplementItemViewModel(item));
                }
            }
        }
        
    }
}
