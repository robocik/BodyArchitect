using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.UserControls;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;

namespace BodyArchitect.Client.Module.Suplements.Controls
{
    public abstract class SupplementsCyclesDefinitionsPagerList : PagerListUserControl<SupplementCycleDefinitionDTO>
    {
        protected GetSupplementsCycleDefinitionsParam criteria;

        protected SupplementsCyclesDefinitionsPagerList()
        {
            FillSearchControls();
        }

        #region Ribbon

        private SearchSortOrder selectedOrder;
        private bool sortAscending;
        private ObservableCollection<CheckListItem<SearchSortOrder>> sortOrders = new ObservableCollection<CheckListItem<SearchSortOrder>>();

        public ObservableCollection<CheckListItem<SearchSortOrder>> SortOrders
        {
            get { return sortOrders; }
        }

        public SearchSortOrder SelectedOrder
        {
            get { return selectedOrder; }
            set
            {
                selectedOrder = value;
                NotifyOfPropertyChange(() => SelectedOrder);
            }
        }

        public bool SortAscending
        {
            get { return sortAscending; }
            set
            {
                sortAscending = value;
                NotifyOfPropertyChange(() => SortAscending);
            }
        }

        protected void FillSearchControls()
        {
            foreach (SearchSortOrder test in Enum.GetValues(typeof(SearchSortOrder)))
            {
                var item = new CheckListItem<SearchSortOrder>(EnumLocalizer.Default.Translate(test), test);
                SortOrders.Add(item);
            }

        }

        #endregion

        protected override void BeforeSearch(object param = null)
        {
            criteria=new GetSupplementsCycleDefinitionsParam();
            criteria.SortOrder = SelectedOrder;
        }

        protected override PagedResult<SupplementCycleDefinitionDTO> RetrieveItems(PartialRetrievingInfo pagerInfo)
        {

            return ServiceManager.GetSupplementsCycleDefinitions(criteria, pagerInfo);
        }
    }
}
