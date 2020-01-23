using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.StrengthTraining.Localization;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.UserControls;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls
{
    public abstract class ExercisesPagerList : PagerListUserControl<ExerciseDTO>
    {
        private ExerciseSearchCriteria criteria;

        private ObservableCollection<CheckListItem<ExerciseType>> exerciseTypes = new ObservableCollection<CheckListItem<ExerciseType>>();
        private ObservableCollection<CheckListItem<ExerciseSearchCriteriaGroup>> searchGroups = new ObservableCollection<CheckListItem<ExerciseSearchCriteriaGroup>>();
        private ObservableCollection<CheckListItem<SearchSortOrder>> sortOrders = new ObservableCollection<CheckListItem<SearchSortOrder>>();
        private SearchSortOrder selectedOrder;
        private string exerciseName;
        private bool sortAscending;

        protected ExercisesPagerList()
        {
            FillSearchControls();
        }


        public ObservableCollection<CheckListItem<ExerciseSearchCriteriaGroup>> SearchGroups
        {
            get { return searchGroups; }
        }

        public ObservableCollection<CheckListItem<ExerciseType>> SearchExerciseTypes
        {
            get { return exerciseTypes; }
        }

        public ObservableCollection<CheckListItem<SearchSortOrder>> SortOrders
        {
            get { return sortOrders; }
        }

        public string ExerciseName
        {
            get { return exerciseName; }
            set
            {
                exerciseName = value;
                NotifyOfPropertyChange(() => ExerciseName);
            }
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

        protected override void BeforeSearch(object param = null)
        {
            criteria = ExerciseSearchCriteria.CreateAllCriteria();

            foreach (var value in exerciseTypes)
            {
                if (value.IsChecked)
                {
                    criteria.ExerciseTypes.Add(value.Value);
                }
            }
            foreach (var value in SearchGroups)
            {
                if (value.IsChecked)
                {
                    criteria.SearchGroups.Add(value.Value);
                }
            }

            criteria.Name = ExerciseName;

            criteria.SortOrder = SelectedOrder;
            criteria.SortAscending = SortAscending;
        }

        protected void FillSearchControls()
        {
            sortOrders.Clear();
            exerciseTypes.Clear();
            searchGroups.Clear();
            ExerciseName = null;

            foreach (SearchSortOrder type in Enum.GetValues(typeof(SearchSortOrder)))
            {
                CheckListItem<SearchSortOrder> item = new CheckListItem<SearchSortOrder>(EnumLocalizer.Default.Translate(type), type);
                sortOrders.Add(item);
            }

            var strengthTrainingLocalizer=new EnumLocalizer(StrengthTrainingEntryStrings.ResourceManager);
            foreach (ExerciseSearchCriteriaGroup type in Enum.GetValues(typeof(ExerciseSearchCriteriaGroup)))
            {
                CheckListItem<ExerciseSearchCriteriaGroup> item = new CheckListItem<ExerciseSearchCriteriaGroup>(strengthTrainingLocalizer.Translate(type), type);
                if(type==ExerciseSearchCriteriaGroup.Other)
                {//by default search only in other user exercises
                    item.IsChecked = true;
                }
                SearchGroups.Add(item);
            }
            foreach (ExerciseType type in Enum.GetValues(typeof(ExerciseType)))
            {
                if (type != ExerciseType.NotSet)
                {
                    CheckListItem<ExerciseType> item =new CheckListItem<ExerciseType>(EnumLocalizer.Default.Translate(type), type);
                    exerciseTypes.Add(item);
                }
            }
        }

        protected override PagedResult<ExerciseDTO> RetrieveItems(PartialRetrievingInfo pagerInfo)
        {
            return ServiceManager.GetExercises(criteria, pagerInfo);
        }
    }
}
