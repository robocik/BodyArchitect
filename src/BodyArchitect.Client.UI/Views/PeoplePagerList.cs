using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI.UserControls;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.UI.Views
{
    public abstract class PeoplePagerList : PagerListUserControl<UserSearchDTO>
    {
        private UserSearchCriteria criteria;

        protected PeoplePagerList()
        {
            FillSearchCriterias();
        }

        #region Ribbon

        ObservableCollection<CheckListItem<Country>> countries = new ObservableCollection<CheckListItem<Country>>();
        private ObservableCollection<CheckListItem<UserSearchGroup>> groups = new ObservableCollection<CheckListItem<UserSearchGroup>>();
        private ObservableCollection<CheckListItem<PictureCriteria>> photos = new ObservableCollection<CheckListItem<PictureCriteria>>();
        private ObservableCollection<CheckListItem<Gender>> genders = new ObservableCollection<CheckListItem<Gender>>();
        private ObservableCollection<CheckListItem<UserPlanCriteria>> plans = new ObservableCollection<CheckListItem<UserPlanCriteria>>();
        private ObservableCollection<CheckListItem<PrivacyCriteria>> calendarPrivacy = new ObservableCollection<CheckListItem<PrivacyCriteria>>();
        private ObservableCollection<CheckListItem<PrivacyCriteria>> measurementPrivacy = new ObservableCollection<CheckListItem<PrivacyCriteria>>();
        private ObservableCollection<CheckListItem<UsersSortOrder>> sortOrders = new ObservableCollection<CheckListItem<UsersSortOrder>>();
        private UsersSortOrder selectedSortOrder;
        private string username;
        private PictureCriteria selectedPhoto;
        private UserPlanCriteria selectedPlan;
        private PrivacyCriteria selectedCalendarPrivacy;
        private PrivacyCriteria selectedMeasurementPrivacy;
        private bool sortAscending;


        public ObservableCollection<CheckListItem<Country>> Countries
        {
            get { return countries; }
        }

        public ObservableCollection<CheckListItem<PictureCriteria>> Photos
        {
            get { return photos; }
        }

        public ObservableCollection<CheckListItem<UserSearchGroup>> Groups
        {
            get { return groups; }
        }

        public ObservableCollection<CheckListItem<Gender>> Genders
        {
            get { return genders; }
        }

        public ObservableCollection<CheckListItem<UserPlanCriteria>> Plans
        {
            get { return plans; }
        }

        public ObservableCollection<CheckListItem<PrivacyCriteria>> CalendarPrivacy
        {
            get { return calendarPrivacy; }
        }

        public ObservableCollection<CheckListItem<PrivacyCriteria>> MeasurementPrivacy
        {
            get { return measurementPrivacy; }
        }

        public ObservableCollection<CheckListItem<UsersSortOrder>> SortOrders
        {
            get { return sortOrders; }
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

        public UsersSortOrder SelectedSortOrder
        {
            get { return selectedSortOrder; }
            set
            {
                selectedSortOrder = value;
                NotifyOfPropertyChange(() => SelectedSortOrder);
            }
        }

        public PrivacyCriteria SelectedMeasurementPrivacy
        {
            get { return selectedMeasurementPrivacy; }
            set
            {
                selectedMeasurementPrivacy = value;
                NotifyOfPropertyChange(() => SelectedMeasurementPrivacy);
            }
        }
        public PrivacyCriteria SelectedCalendarPrivacy
        {
            get { return selectedCalendarPrivacy; }
            set
            {
                selectedCalendarPrivacy = value;
                NotifyOfPropertyChange(() => SelectedCalendarPrivacy);
            }
        }

        public UserPlanCriteria SelectedPlan
        {
            get { return selectedPlan; }
            set
            {
                selectedPlan = value;
                NotifyOfPropertyChange(() => SelectedPlan);
            }
        }

        public string Username
        {
            get { return username; }
            set
            {
                username = value;
                NotifyOfPropertyChange(() => Username);
            }
        }

        public PictureCriteria SelectedPhoto
        {
            get { return selectedPhoto; }
            set
            {
                selectedPhoto = value;
                NotifyOfPropertyChange(() => SelectedPhoto);
            }
        }

        protected void FillSearchCriterias()
        {
            SortAscending = false;
            Countries.Clear();
            Groups.Clear();
            Genders.Clear();
            CalendarPrivacy.Clear();
            MeasurementPrivacy.Clear();
            Photos.Clear();
            Plans.Clear();
            SearchStatus = string.Empty;
            SortOrders.Clear();

            foreach (var test in Country.Countries)
            {
                CheckListItem<Country> item = new CheckListItem<Country>(test.DisplayName, test);
                Countries.Add(item);
            }

            foreach (UserSearchGroup test in Enum.GetValues(typeof(UserSearchGroup)))
            {
                var item = new CheckListItem<UserSearchGroup>(EnumLocalizer.Default.Translate(test), test);
                Groups.Add(item);
            }

            foreach (Gender test in Enum.GetValues(typeof(Gender)))
            {
                var item = new CheckListItem<Gender>(EnumLocalizer.Default.Translate(test), test);
                Genders.Add(item);
            }

            foreach (PrivacyCriteria test in Enum.GetValues(typeof(PrivacyCriteria)))
            {
                var item = new CheckListItem<PrivacyCriteria>(EnumLocalizer.Default.Translate(test), test);
                CalendarPrivacy.Add(item);
            }
            foreach (PrivacyCriteria test in Enum.GetValues(typeof(PrivacyCriteria)))
            {
                var item = new CheckListItem<PrivacyCriteria>(EnumLocalizer.Default.Translate(test), test);
                MeasurementPrivacy.Add(item);
            }

            foreach (UsersSortOrder test in Enum.GetValues(typeof(UsersSortOrder)))
            {
                var item = new CheckListItem<UsersSortOrder>(EnumLocalizer.Default.Translate(test), test);
                SortOrders.Add(item);
            }

            foreach (PictureCriteria test in Enum.GetValues(typeof(PictureCriteria)))
            {
                var item = new CheckListItem<PictureCriteria>(EnumLocalizer.Default.Translate(test), test);
                Photos.Add(item);
            }
            foreach (UserPlanCriteria test in Enum.GetValues(typeof(UserPlanCriteria)))
            {
                var item = new CheckListItem<UserPlanCriteria>(EnumLocalizer.Default.Translate(test), test);
                Plans.Add(item);
            }


            SelectedCalendarPrivacy = PrivacyCriteria.All;
            SelectedMeasurementPrivacy = PrivacyCriteria.All;
            SelectedPhoto = PictureCriteria.All;
            SelectedPlan = UserPlanCriteria.All;
            SelectedSortOrder = UsersSortOrder.ByLastLoginDate;
        }

        #endregion

        protected override void BeforeSearch(object param = null)
        {

            criteria = new UserSearchCriteria();
            foreach (var value in Countries)
            {
                if (value.IsChecked)
                {
                    criteria.Countries.Add(value.Value.GeoId);
                }
            }

            foreach (var value in Genders)
            {
                if (value.IsChecked)
                {
                    criteria.Genders.Add(value.Value);
                }
            }
            if (param!=null)
            {
                criteria.ProfileId = (Guid) param;
            }
            criteria.SortAscending = SortAscending;
            criteria.AccessCalendar = SelectedCalendarPrivacy;
            criteria.AccessSizes = SelectedMeasurementPrivacy;
            criteria.Picture = SelectedPhoto;
            criteria.WorkoutPlan = SelectedPlan;
            criteria.SortOrder = SelectedSortOrder;

            foreach (var value in Groups)
            {
                if (value.IsChecked)
                {
                    criteria.UserSearchGroups.Add(value.Value);
                }
            }

            criteria.UserName = Username;
        }

        protected override PagedResult<UserSearchDTO> RetrieveItems(PartialRetrievingInfo pagerInfo)
        {

            return ServiceManager.GetUsers(criteria, pagerInfo);
        }

    }
}
