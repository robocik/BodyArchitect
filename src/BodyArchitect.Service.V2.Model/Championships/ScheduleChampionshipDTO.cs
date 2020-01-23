using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Service.V2.Model.Localization;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace BodyArchitect.Service.V2.Model
{
    public enum ChampionshipTryResult
    {
        NotDone,
        Success,
        Fail
    }

    public enum ChampionshipType
    {
        ZawodyWyciskanieSztangi,
        Trojboj
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    [Serializable]
    public class ChampionshipTryDTO
    {
        [DataMember]
        public decimal Weight { get; set; }

        [DataMember]
        public ChampionshipTryResult Result { get; set; }
    }

    public enum ChampionshipCategoryType
    {
        Weight,
        Open
    }

    public enum ChampionshipWinningCategories
    {
        Seniorzy,
        JuniorzyMlodsi,
        Juniorzy,
        Weterani1,
        Weterani2,
        Weterani3,
        Weterani4,
        MistrzMistrzow,
        Druzynowa
    }

    [DataContract(Namespace = Const.ServiceNamespace, IsReference = true)]
    [Serializable]
    public class ChampionshipCategoryDTO : BAGlobalObject
    {
        public ChampionshipCategoryDTO()
        {
            IsAgeStrict = true;
            IsOfficial = true;
        }

        [DataMember]
        public bool IsOfficial { get; set; }

        [DataMember]
        public ChampionshipCategoryType Type { get; set; }

        [DataMember]
        public ChampionshipWinningCategories Category { get; set; }

        [DataMember]
        public bool IsAgeStrict { get; set; }

        [DataMember]
        public Gender Gender { get; set; }

        public bool IsTheSame(ChampionshipCategoryDTO category)
        {
            return IsOfficial==category.IsOfficial && Type == category.Type && Category == category.Category && Gender == category.Gender;
        }
    }

    /*
     nk - niesklasyfikowany (brak limitu III klasy)
     pk - poza konkursem
     */
    public enum ChampionshipCustomerType
    {
        Normal,
        OutsideCompetition,//pk
        NotClassified,//nk
        Disqualified
    }

    [DataContract(Namespace = Const.ServiceNamespace, IsReference = true)]
    [Serializable]
    public class ChampionshipCustomerDTO : BAGlobalObject
    {
        [DataMember]
        public Guid CustomerId { get; set; }

        [DataMember]
        public decimal Total { get; set; }

        [DataMember]
        public ChampionshipCustomerType Type { get; set; }

        [DataMember]
        public decimal TotalWilks { get; set; }

        [DataMember]
        public decimal Weight { get; set; }

        [DataMember]
        public DateTime WeightDateTime { get; set; }

        [DataMember]
        public ChampionshipGroupDTO Group { get; set; }

        [DataMember]
        public string Comment { get; set; }
    }

    [DataContract(Namespace = Const.ServiceNamespace, IsReference = true)]
    [Serializable]
    public class ChampionshipEntryDTO : BAGlobalObject
    {
        public ChampionshipEntryDTO()
        {
            Try1 = new ChampionshipTryDTO();
            Try2 = new ChampionshipTryDTO();
            Try3 = new ChampionshipTryDTO();
        }

        [DataMember]
        public ExerciseLightDTO Exercise { get; set; }

        [DataMember]
        public ChampionshipCustomerDTO Customer { get; set; }

        [DataMember]
        public ChampionshipTryDTO Try1 { get; set; }

        [DataMember]
        public ChampionshipTryDTO Try2 { get; set; }

        [DataMember]
        public ChampionshipTryDTO Try3 { get; set; }

        [DataMember]
        public decimal Max { get; set; }

        [DataMember]
        public decimal Wilks { get; set; }
    }

    [DataContract(Namespace = Const.ServiceNamespace, IsReference = true)]
    [Serializable]
    public class ChampionshipGroupDTO : BAGlobalObject
    {
        public ChampionshipGroupDTO()
        {
            Members = new List<ChampionshipCustomerDTO>();
        }

        [DataMember]
        [Required()]
        [StringLengthValidator(Constants.NameColumnLength)]
        public string Name { get; set; }

        [DataMember]
        public IList<ChampionshipCustomerDTO> Members { get; set; }
    }

    [DataContract(Namespace = Const.ServiceNamespace, IsReference = true)]
    [Serializable]
    public class ScheduleChampionshipDTO : ScheduleEntryBaseDTO, IHasName
    {
        private List<ChampionshipCategoryDTO> _categories;

        public ScheduleChampionshipDTO()
        {
            Categories = new List<ChampionshipCategoryDTO>();
            TeamCount = 5;
        }

        [DataMember]
        [ObjectCollectionValidator(typeof(ChampionshipCategoryDTO))]
        public List<ChampionshipCategoryDTO> Categories
        {
            get { return _categories; }
            set { _categories = value; }
        }

        [DataMember]
        public ChampionshipType ChampionshipType { get; set; }

        [DataMember]
        [Required()]
        [StringLengthValidator(Constants.NameColumnLength)]
        public string Name { get; set; }

        /// <summary>
        /// określa ile najlepszych miejsc wchodzi do obliczenia punktacji dla druzyny
        /// </summary>
        [DataMember]
        public int TeamCount { get; set; }
    }

    [DataContract(Namespace = Const.ServiceNamespace, IsReference = true)]
    [Serializable]
    public class ChampionshipResultItemDTO:BAGlobalObject
    {
        [DataMember]
        public ChampionshipCategoryDTO Category { get; set; }

        [DataMember]
        public ChampionshipCustomerDTO Customer { get; set; }

        [DataMember]
        public int Position { get; set; }

        [DataMember]
        public decimal Value { get; set; }

        [DataMember]
        public ChampionshipGroupDTO Group { get; set; }
    }

    [DataContract(Namespace = Const.ServiceNamespace, IsReference = true)]
    [Serializable]
    public class ChampionshipDTO : BAGlobalObject, IHasName
    {
        private List<ChampionshipEntryDTO> _entries;

        private List<ChampionshipGroupDTO> _groups;

        private List<ChampionshipCustomerDTO> _customers;


        List<decimal> weightMaleCategories=new List<decimal>();
        List<decimal> weightFemaleCategories=new List<decimal>();
        

        static private List<decimal> weightMenCategories;
        static private List<decimal> weightWomenCategories;

        static ChampionshipDTO()
        {
            buildMenWeightCategories();
            buildWomenWeightCategories();
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            weightMaleCategories = new List<decimal>();
            weightFemaleCategories = new List<decimal>();
        } 

        [DataMember]
        public List<ChampionshipResultItemDTO> Results { get; set; }

        [DataMember]
        public string Comment { get; set; }

        public static List<decimal> MenWeights
        {
            get { return weightMenCategories; }
        }

        public static List<decimal> WomenWeights
        {
            get { return weightWomenCategories; }
        }

        [DataMember]
        public List<decimal> WeightMenCategories
        {
            get { return weightMaleCategories; }
        }

        [DataMember]
        public List<decimal> WeightWomenCategories
        {
            get { return weightFemaleCategories; }
        }

        public ChampionshipDTO()
        {
            Categories = new List<ChampionshipCategoryDTO>();
            TeamCount = 5;

            Reservations = new List<ScheduleEntryReservationDTO>();
            Entries = new List<ChampionshipEntryDTO>();
            Groups = new List<ChampionshipGroupDTO>();
            Customers = new List<ChampionshipCustomerDTO>();
            Results = new List<ChampionshipResultItemDTO>();

            weightMaleCategories = new List<decimal>();
            weightFemaleCategories = new List<decimal>();

            weightMaleCategories.AddRange(weightMenCategories);
            weightFemaleCategories.AddRange(weightWomenCategories);
        }

        static void buildMenWeightCategories()
        {
            weightMenCategories = new List<decimal>();
            weightMenCategories.Add(52);
            weightMenCategories.Add(56);
            weightMenCategories.Add(60);
            weightMenCategories.Add(67.5m);
            weightMenCategories.Add(75);
            weightMenCategories.Add(82.5m);
            weightMenCategories.Add(90);
            weightMenCategories.Add(100);
            weightMenCategories.Add(110);
            weightMenCategories.Add(125);
        }

        static void buildWomenWeightCategories()
        {
            weightWomenCategories = new List<decimal>();
            weightWomenCategories.Add(44);
            weightWomenCategories.Add(48);
            weightWomenCategories.Add(52);
            weightWomenCategories.Add(56);
            weightWomenCategories.Add(60);
            weightWomenCategories.Add(67.5m);
            weightWomenCategories.Add(75);
            weightWomenCategories.Add(82.5m);
            weightWomenCategories.Add(90);
        }

        

        [DataMember]
        [ObjectCollectionValidator(typeof(ChampionshipEntryDTO))]
        public List<ChampionshipEntryDTO> Entries
        {
            get { return _entries; }
            set { _entries = value; }
        }

        [DataMember]
        [ObjectCollectionValidator(typeof(ChampionshipGroupDTO))]
        public List<ChampionshipGroupDTO> Groups
        {
            get { return _groups; }
            set { _groups = value; }
        }

        [DataMember]
        [ObjectCollectionValidator(typeof(ChampionshipCustomerDTO))]
        public List<ChampionshipCustomerDTO> Customers
        {
            get { return _customers; }
            set { _customers = value; }
        }








        private List<ChampionshipCategoryDTO> _categories;



        [DataMember]
        [ObjectCollectionValidator(typeof(ChampionshipCategoryDTO))]
        public List<ChampionshipCategoryDTO> Categories
        {
            get { return _categories; }
            set { _categories = value; }
        }

        [DataMember]
        public ChampionshipType ChampionshipType { get; set; }

        [DataMember]
        [Required()]
        [StringLengthValidator(Constants.NameColumnLength)]
        public string Name { get; set; }

        /// <summary>
        /// określa ile najlepszych miejsc wchodzi do obliczenia punktacji dla druzyny
        /// </summary>
        [DataMember]
        public int TeamCount { get; set; }

        [DataMember]
        [NotNullValidator]
        [ObjectCollectionValidator(typeof(ScheduleEntryReservationDTO))]
        public List<ScheduleEntryReservationDTO> Reservations { get; set; }

        //UTC
        [DataMember]
        public DateTime StartTime { get; set; }

        //UTC
        [DataMember]
        public DateTime EndTime { get; set; }



        [DataMember]
        public Guid? MyPlaceId { get; set; }



        [DataMember]
        [DoNotChecksum]
        [SerializerId]
        public int Version { get; set; }

        [DataMember]
        public decimal Price { get; set; }

        [DataMember]
        public ScheduleEntryState State { get; set; }

        [DataMember]
        public bool IsLocked { get; set; }

        //[DataMember]
        //[ObjectValidator]
        //public ReminderItemDTO Reminder { get; set; }

        [DataMember]
        public TimeSpan? RemindBefore { get; set; }

        [SelfValidation]
        public void Validate(ValidationResults results)
        {
            if (StartTime > EndTime)
            {
                results.AddResult(new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(ValidationStrings.ScheduleEntryDTO_EndLowerStart, this, "EndTime", null, null));
            }

        }
    }


}
