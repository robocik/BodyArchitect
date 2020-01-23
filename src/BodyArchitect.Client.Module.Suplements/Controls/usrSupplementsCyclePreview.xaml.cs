using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI;
using BodyArchitect.Service.V2.Model;
using vhCalendar;

namespace BodyArchitect.Client.Module.Suplements.Controls
{
    /// <summary>
    /// Interaction logic for usrSupplementsCyclePreview.xaml
    /// </summary>
    public partial class usrSupplementsCyclePreview
    {
        ObservableCollection<ImageListItem<string>> statistics = new ObservableCollection<ImageListItem<string>>();
        public usrSupplementsCyclePreview()
        {
            InitializeComponent();
            DataContext = this;
        }


        public IList<ImageListItem<string>> Statistics
        {
            get { return statistics; }
        }
        public void Fill(IList<EntryObjectDTO> trainingEntries)
        {
            entriesViewer.Fill(trainingEntries.ToTrainingDays());
            generateStatistics(trainingEntries);
        }

        void generateStatistics(IList<EntryObjectDTO> trainingEntries)
        {
            List<IGrouping<Tuple<SuplementDTO, string>, SuplementItemDTO>> supplementsGroup = trainingEntries.Cast<SuplementsEntryDTO>().SelectMany(x => x.Items).GroupBy(x => new Tuple<SuplementDTO, string>(x.Suplement, x.Name)).ToList();

            Dictionary<string, SupplementStatisticItem> supplementsAmout = new Dictionary<string, SupplementStatisticItem>();
            //foreach (SuplementsEntryDTO entry in trainingEntries)
            //{
            //    foreach (var item in entry.Items)
            //    {
            //        string key = item.Suplement.GlobalId.ToString() + item.DosageType;
            //        if (!supplementsAmout.ContainsKey(key))
            //        {
            //            var tmp = new SupplementStatisticItem();
            //            tmp.Supplement = item.Suplement;
            //            tmp.DosageType = item.DosageType;
            //            supplementsAmout.Add(key, tmp);
            //        }
            //        supplementsAmout[key].TotalAmount += item.Dosage;
            //    }
                
            //}
            foreach (var entry in supplementsGroup)
            {
                foreach (var item in entry)
                {
                    string name = item.Suplement.Name;
                    if (!string.IsNullOrEmpty(entry.Key.Item2))
                    {
                        name = entry.Key.Item2;
                    }

                    string key = name + item.DosageType;
                    if (!supplementsAmout.ContainsKey(key))
                    {
                        var tmp = new SupplementStatisticItem();
                        tmp.Supplement = name;
                        tmp.DosageType = item.DosageType;
                        supplementsAmout.Add(key, tmp);
                    }
                    supplementsAmout[key].TotalAmount += item.Dosage;
                }

            }

            Statistics.Clear();
            foreach (var statisticItem in supplementsAmout)
            {
                ImageListItem<string> item = new ImageListItem<string>(string.Format("usrSupplementsCyclePreview_generateStatistics_Item".TranslateSupple(), statisticItem.Value.Supplement, statisticItem.Value.TotalAmount.ToString("0.##"), EnumLocalizer.Default.Translate(statisticItem.Value.DosageType)), null, null);
                Statistics.Add(item);
            }
        }

        class SupplementStatisticItem
        {
            public string Supplement { get; set; }

            public DosageType DosageType { get; set; }

            public decimal TotalAmount { get; set; }
        }
    }
}
