using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.Instructor.Controls;
using BodyArchitect.Client.Module.StrengthTraining;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor.ViewModel
{
    public class ChampionshipExerciseViewModel : ViewModelBase
    {
        private ChampionshipEntryDTO entry;

        public ChampionshipExerciseViewModel(ChampionshipEntryDTO entry)
        {
            this.entry = entry;
            
        }

        public static string GetDisplayWeight(decimal weight, Gender gender)
        {
            string weightType = Strings.WeightType_Kg;
            if (UserContext.Current.ProfileInformation.Settings.WeightType == Service.V2.Model.WeightType.Pounds)
            {
                weightType = Strings.WeightType_Pound;
            }
            //this means highest available weight category (for female is 90+). The 99999999999999.99999M comes from db and I think this is the Decimal.MaxValue representation in mysql
            if (weight >= 99999999999999.99999M)
            {
                var categories = gender == Gender.Male ? ChampionshipDTO.MenWeights : ChampionshipDTO.WomenWeights;
                return string.Format("{0}+ {1}", categories[categories.Count-1].ToDisplayWeight(), weightType);    
            }
            return string.Format("{0} {1}", weight.ToDisplayWeight(), weightType);
        }

        public string DisplayExercise1Try1
        {
            get
            {
                return GetDisplayWeight(Exercise1Try1Weight, Gender.NotSet);
            }
        }
        public string DisplayExercise1Try2
        {
            get
            {
                return GetDisplayWeight(Exercise1Try2Weight, Gender.NotSet);
            }
        }
        public string DisplayExercise1Try3
        {
            get
            {
                return GetDisplayWeight(Exercise1Try3Weight, Gender.NotSet);
            }
        }

        public decimal Exercise1Try1Weight
        {
            get { return entry.Try1.Weight; }
            set
            {
                entry.Try1.Weight = value;
                NotifyOfPropertyChange(() => Exercise1Try1Weight);
                NotifyOfPropertyChange(() => DisplayExercise1Try1);
            }
        }

        public decimal Exercise1Try2Weight
        {
            get { return entry.Try2.Weight; }
            set
            {
                entry.Try2.Weight = value;
                NotifyOfPropertyChange(() => Exercise1Try2Weight);
                NotifyOfPropertyChange(() => DisplayExercise1Try2);
            }
        }

        public decimal Exercise1Try3Weight
        {
            get { return entry.Try3.Weight; }
            set
            {
                entry.Try3.Weight = value;
                NotifyOfPropertyChange(() => Exercise1Try3Weight);
                NotifyOfPropertyChange(() => DisplayExercise1Try3);
            }
        }

        private bool _isExercise1Try1Record;
        private bool _isExercise1Try2Record;
        private bool _isExercise1Try3Record;

        public bool IsExercise1Try1Record
        {
            get { return _isExercise1Try1Record; }
            set
            {
                _isExercise1Try1Record = value;
                NotifyOfPropertyChange(() => IsExercise1Try1Record);
            }
        }

        public bool IsExercise1Try2Record
        {
            get { return _isExercise1Try2Record; }
            set
            {
                _isExercise1Try2Record = value;
                NotifyOfPropertyChange(() => IsExercise1Try2Record);
            }
        }

        public bool IsExercise1Try3Record
        {
            get { return _isExercise1Try3Record; }
            set
            {
                _isExercise1Try3Record = value;
                NotifyOfPropertyChange(() => IsExercise1Try3Record);
            }
        }

        public bool IsExercise1Try1Planned
        {
            get { return entry.Try1.Result == ChampionshipTryResult.NotDone; }
            set
            {
                if (value)
                {
                    entry.Try1.Result = ChampionshipTryResult.NotDone;
                    NotifyOfPropertyChange(() => IsExercise1Try1Planned);
                }
            }
        }

        public bool IsExercise1Try1Ok
        {
            get { return entry.Try1.Result == ChampionshipTryResult.Success; }
            set
            {
                if (value)
                {
                    entry.Try1.Result = ChampionshipTryResult.Success;
                    NotifyOfPropertyChange(() => IsExercise1Try1Ok);
                }
            }
        }

        public bool IsExercise1Try1Failed
        {
            get { return entry.Try1.Result == ChampionshipTryResult.Fail; }
            set
            {
                if (value)
                {
                    entry.Try1.Result = ChampionshipTryResult.Fail;
                    NotifyOfPropertyChange(() => IsExercise1Try1Failed);
                }
            }
        }



        public bool IsExercise1Try2Planned
        {
            get { return entry.Try2.Result == ChampionshipTryResult.NotDone; }
            set
            {
                if (value)
                {
                    entry.Try2.Result = ChampionshipTryResult.NotDone;
                    NotifyOfPropertyChange(() => IsExercise1Try2Planned);
                }
            }
        }

        public bool IsExercise1Try2Ok
        {
            get { return entry.Try2.Result == ChampionshipTryResult.Success; }
            set
            {
                if (value)
                {
                    entry.Try2.Result = ChampionshipTryResult.Success;
                    NotifyOfPropertyChange(() => IsExercise1Try2Ok);
                }
            }
        }

        public bool IsExercise1Try2Failed
        {
            get { return entry.Try2.Result == ChampionshipTryResult.Fail; }
            set
            {
                if (value)
                {
                    entry.Try2.Result = ChampionshipTryResult.Fail;
                    NotifyOfPropertyChange(() => IsExercise1Try2Failed);
                }
            }
        }

        public bool IsExercise1Try3Planned
        {
            get { return entry.Try3.Result == ChampionshipTryResult.NotDone; }
            set
            {
                if (value)
                {
                    entry.Try3.Result = ChampionshipTryResult.NotDone;
                    NotifyOfPropertyChange(() => IsExercise1Try3Planned);
                }
            }
        }

        public bool IsExercise1Try3Ok
        {
            get { return entry.Try3.Result == ChampionshipTryResult.Success; }
            set
            {
                if (value)
                {
                    entry.Try3.Result = ChampionshipTryResult.Success;
                    NotifyOfPropertyChange(() => IsExercise1Try3Ok);
                }
            }
        }

        public bool IsExercise1Try3Failed
        {
            get { return entry.Try3.Result == ChampionshipTryResult.Fail; }
            set
            {
                if (value)
                {
                    entry.Try3.Result = ChampionshipTryResult.Fail;
                    NotifyOfPropertyChange(() => IsExercise1Try3Failed);
                }
            }
        }

        public ChampionshipEntryDTO Entry
        {
            get { return entry; }
        }
    }
    public class ChampionshipEntryViewModel:ViewModelBase
    {
        private CustomerDTO customer;
        private ChampionshipCustomerDTO championshipCustomer;
        private ChampionshipExerciseViewModel exercise1;
        private ChampionshipExerciseViewModel exercise2;
        private ChampionshipExerciseViewModel exercise3;
        
        private decimal weightCategory;
        private ListItem<ChampionshipGroupDTO> selectedGroup;

        public ChampionshipEntryViewModel(CustomerDTO customer, ChampionshipDTO championship,ChampionshipView mainViewModel)
        {
            this.customer = customer;
            championshipCustomer = championship.Customers.Where(x => x.CustomerId == customer.GlobalId).SingleOrDefault();
            if (championshipCustomer == null)
            {
                championshipCustomer = new ChampionshipCustomerDTO();
                championshipCustomer.CustomerId = customer.GlobalId;
                championship.Customers.Add(championshipCustomer);
            }
            var itemGroup = mainViewModel.Groups.Where(x => x.Value == championshipCustomer.Group).SingleOrDefault();
            SelectedGroup = itemGroup;

            var benchPress = ExercisesReposidory.Instance.GetItem(new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var przysiad = ExercisesReposidory.Instance.GetItem(new Guid("3e06a130-b811-4e45-9285-f087403615bf"));
            var deadLift = ExercisesReposidory.Instance.GetItem(new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));

            var benchPressEntry =championship.Entries.Where(x => x.Customer.CustomerId == customer.GlobalId && x.Exercise.GlobalId == benchPress.GlobalId).SingleOrDefault();
            if (benchPressEntry == null)
            {
                benchPressEntry = new ChampionshipEntryDTO();
                benchPressEntry.Exercise = benchPress;
                benchPressEntry.Customer = championshipCustomer;
            }
            var przysiadEntry = championship.Entries.Where(x => x.Customer.CustomerId == customer.GlobalId && x.Exercise.GlobalId == przysiad.GlobalId).SingleOrDefault();
            if (przysiadEntry == null)
            {
                przysiadEntry = new ChampionshipEntryDTO();
                przysiadEntry.Exercise = przysiad;
                przysiadEntry.Customer = championshipCustomer;
            }

            var deadLiftEntry = championship.Entries.Where(x => x.Customer.CustomerId == customer.GlobalId && x.Exercise.GlobalId == deadLift.GlobalId).SingleOrDefault();
            if (deadLiftEntry == null)
            {
                deadLiftEntry = new ChampionshipEntryDTO();
                deadLiftEntry.Exercise = deadLift;
                deadLiftEntry.Customer = championshipCustomer;
            }

            exercise1 = new ChampionshipExerciseViewModel(benchPressEntry);
            exercise2 = new ChampionshipExerciseViewModel(przysiadEntry);
            exercise3 = new ChampionshipExerciseViewModel(deadLiftEntry);

            weightCategory = getWeightCategory();
        }

        public CustomerDTO Customer
        {
            get { return customer; }
        }

        public bool CustomerHasErrors
        {
            get { return customer.Gender == Gender.NotSet || customer.Birthday == null; }
        }

        public bool HasAdvancedOptions
        {
            get { return CustomerType != ChampionshipCustomerType.Normal; }
        }

        public IList<ListItem<ChampionshipCustomerType>> CustomerTypes
        {
            get
            {
                List<ListItem<ChampionshipCustomerType>> list = new List<ListItem<ChampionshipCustomerType>>();
                foreach (ChampionshipCustomerType customerType in Enum.GetValues(typeof(ChampionshipCustomerType)))
                {

                    list.Add(new ListItem<ChampionshipCustomerType>(EnumLocalizer.Default.Translate(customerType), customerType));
                }
                return list.OrderBy(x => x.Text).ToList();
            }
        }

        public ChampionshipCustomerType CustomerType
        {
            get { return championshipCustomer.Type; }
            set
            {
                championshipCustomer.Type = value;

                NotifyOfPropertyChange(() => CustomerType);
                NotifyOfPropertyChange(() => HasAdvancedOptions);
            }
        }

        public ListItem<ChampionshipGroupDTO> SelectedGroup
        {
            get { return selectedGroup; }
            set
            {
                selectedGroup = value;
                championshipCustomer.Group = value!=null?value.Value:null;
                NotifyOfPropertyChange(()=>SelectedGroup);
            }
        }

        public string DisplayGroup
        {
            get
            {
                if(SelectedGroup!=null)
                {
                    return SelectedGroup.Text;
                }
                return InstructorStrings.ChampionshipEntryViewModel_WithoutTeam;
            }
        }
        
        public decimal Weight
        {
            get { return championshipCustomer.Weight; }
            set
            {
                championshipCustomer.Weight = value;
                championshipCustomer.WeightDateTime = DateTime.UtcNow;
                NotifyOfPropertyChange(()=>Weight);
                NotifyOfPropertyChange(() => DisplayWeight);
            }
        }

        public string DisplayWeight
        {
            get
            {
                if(Weight==0)
                {
                    return InstructorStrings.ChampionshipExerciseViewModel_NeedsMeasure;
                }
                return ChampionshipExerciseViewModel.GetDisplayWeight(Weight, Gender.NotSet);
            }
        }

        public decimal WeightCategory
        {
            get
            {
                return weightCategory;
            }
        }

        public string DisplayGender
        {
            get
            {
                return EnumLocalizer.Default.Translate(customer.Gender);
            }
        }

        public string DisplayWeightCategory
        {
            get
            {
                string weightStr = weightCategory.ToString();
                if (weightCategory >= 99999999999999.99999M)
                {
                    var categories = customer.Gender == Gender.Male ? ChampionshipDTO.MenWeights : ChampionshipDTO.WomenWeights;
                    weightStr= categories[categories.Count - 1].ToDisplayWeight()+"+";
                }
                return string.Format(InstructorStrings.ChampionshipExerciseViewModel_DisplayWeightCategory, EnumLocalizer.Default.Translate(customer.Gender), weightStr);
            }
        }

        decimal getWeightCategory()
        {
            if(customer.Gender==Gender.Male)
            {
                return ModelHelper.GetWeightCategory(ChampionshipDTO.MenWeights, Weight);
            }
            else if (customer.Gender == Gender.Female)
            {
                return ModelHelper.GetWeightCategory(ChampionshipDTO.WomenWeights, Weight);
            }

            return 0;
        }

        public ChampionshipExerciseViewModel Exercise1
        {
            get { return exercise1; }
        }

        public ChampionshipExerciseViewModel Exercise2
        {
            get { return exercise2; }
        }

        public ChampionshipExerciseViewModel Exercise3
        {
            get { return exercise3; }
        }

        public string Comment
        {
            get { return championshipCustomer.Comment; }
            set
            {
                championshipCustomer.Comment = value;
                NotifyOfPropertyChange(()=>Comment);
            }
        }

        public string Total
        {
            get { return ChampionshipExerciseViewModel.GetDisplayWeight(championshipCustomer.Total, Gender.NotSet); }
        }

        public string Wilks
        {
            get { return string.Format(InstructorStrings.ChampionshipExerciseViewModel_Wilks, championshipCustomer.TotalWilks); }
        }
    }
}
