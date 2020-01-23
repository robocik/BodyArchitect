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

namespace BodyArchitect.Client.Module.StrengthTraining.Controls
{
    public abstract class WorkoutPlansPagerList : PagerListUserControl<TrainingPlan>
    {
        protected WorkoutPlanSearchCriteria criteria;

        protected WorkoutPlansPagerList()
        {
            FillSearchControls();
        }

        #region Ribbon

        private ObservableCollection<CheckListItem<TrainingType>> traingingTypes = new ObservableCollection<CheckListItem<TrainingType>>();
        private ObservableCollection<CheckListItem<int>> days = new ObservableCollection<CheckListItem<int>>();
        private ObservableCollection<CheckListItem<TrainingPlanDifficult>> difficulties = new ObservableCollection<CheckListItem<TrainingPlanDifficult>>();
        private ObservableCollection<CheckListItem<Language>> languages = new ObservableCollection<CheckListItem<Language>>();
        private ObservableCollection<CheckListItem<SearchSortOrder>> sortOrders = new ObservableCollection<CheckListItem<SearchSortOrder>>();
        private ObservableCollection<CheckListItem<WorkoutPlanPurpose>> purposes = new ObservableCollection<CheckListItem<WorkoutPlanPurpose>>();
        private SearchSortOrder selectedOrder;
        private bool sortAscending;

        public ObservableCollection<CheckListItem<TrainingType>> TrainingTypes
        {
            get { return traingingTypes; }
        }

        public ObservableCollection<CheckListItem<int>> Days
        {
            get { return days; }
        }

        public ObservableCollection<CheckListItem<TrainingPlanDifficult>> Difficulties
        {
            get { return difficulties; }
        }

        public ObservableCollection<CheckListItem<Language>> Languages
        {
            get { return languages; }
        }

        public ObservableCollection<CheckListItem<SearchSortOrder>> SortOrders
        {
            get { return sortOrders; }
        }

        public ObservableCollection<CheckListItem<WorkoutPlanPurpose>> Purposes
        {
            get { return purposes; }
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
            traingingTypes.Clear();
            days.Clear();
            difficulties.Clear();
            languages.Clear();
            sortOrders.Clear();
            purposes.Clear();

            for (int i = 1; i < 8; i++)
            {
                CheckListItem<int> item = new CheckListItem<int>(i.ToString(), i);
                days.Add(item);
            }
            foreach (TrainingType test in Enum.GetValues(typeof(TrainingType)))
            {
                CheckListItem<TrainingType> item = new CheckListItem<TrainingType>(EnumLocalizer.Default.Translate(test), test);
                traingingTypes.Add(item);
            }

            foreach (TrainingPlanDifficult type in Enum.GetValues(typeof(TrainingPlanDifficult)))
            {
                CheckListItem<TrainingPlanDifficult> item = new CheckListItem<TrainingPlanDifficult>(EnumLocalizer.Default.Translate(type), type);
                difficulties.Add(item);
            }

            foreach (var test in BodyArchitect.Service.V2.Model.Language.Languages)
            {
                CheckListItem<Language> item = new CheckListItem<Language>(test.DisplayName, test);
                languages.Add(item);
            }

            foreach (SearchSortOrder type in Enum.GetValues(typeof(SearchSortOrder)))
            {
                CheckListItem<SearchSortOrder> item = new CheckListItem<SearchSortOrder>(EnumLocalizer.Default.Translate(type), type);
                sortOrders.Add(item);
            }

            foreach (WorkoutPlanPurpose type in Enum.GetValues(typeof(WorkoutPlanPurpose)))
            {
                CheckListItem<WorkoutPlanPurpose> item = new CheckListItem<WorkoutPlanPurpose>(EnumLocalizer.Default.Translate(type), type);
                purposes.Add(item);
            }


        }

        #endregion

        protected override void BeforeSearch(object param = null)
        {
            criteria = new WorkoutPlanSearchCriteria();
            foreach (var value in Days)
            {
                if (value.IsChecked)
                {
                    criteria.Days.Add(value.Value);
                }
            }
            foreach (var value in Difficulties)
            {
                if (value.IsChecked)
                {
                    criteria.Difficults.Add(value.Value);
                }
            }

            foreach (var value in Purposes)
            {
                if (value.IsChecked)
                {
                    criteria.Purposes.Add(value.Value);
                }
            }

            foreach (var value in Languages)
            {
                if (value.IsChecked)
                {
                    criteria.Languages.Add((value.Value).Shortcut);
                }
            }

            foreach (var value in TrainingTypes)
            {
                if (value.IsChecked)
                {
                    criteria.WorkoutPlanType.Add(value.Value);
                }
            }
            criteria.SortOrder = SelectedOrder;
            criteria.SortAscending = SortAscending;
        }

        protected override PagedResult<TrainingPlan> RetrieveItems(PartialRetrievingInfo pagerInfo)
        {

            return ServiceManager.GetWorkoutPlans(criteria, pagerInfo);
        }

    }
}
