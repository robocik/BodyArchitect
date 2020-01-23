using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model
{
    
    public class ChampionshipGroup:FMGlobalObject
    {
        public ChampionshipGroup()
        {
            Members = new HashSet<ChampionshipCustomer>();
        }

        public virtual string Name { get; set; }

        public virtual ICollection<ChampionshipCustomer> Members { get; set; }
    }

    

    public class ChampionshipCategory:FMGlobalObject
    {
        public ChampionshipCategory()
        {
            IsAgeStrict = true;
        }

        /// <summary>
        /// Jeśli true to miejsca wyznaczane są tylko dla zawodników z ChampionshipCustomerType.Normal. Jesli false to dla wszystkich. W przypadku IsOfficial nieklasyfikowani zawodnicy też mogą miec result itemy ale wtedy Position = -1
        /// </summary>
        public virtual bool IsOfficial { get; set; }
        /// <summary>
        /// jeśli true to do tej kategorii wchodzą tylko te osoby które mają dokładnie taki wiek jak jest w przedziale (np tylko weteranki 40-49 lat). Gdy jest false to wchodzą osoby od 40 lat w zwyż (do następnej kategorii wiekowej)
        /// </summary>
        public virtual bool IsAgeStrict { get; set; }

        public virtual ChampionshipCategoryType Type { get; set; }

        public virtual ChampionshipWinningCategories Category { get; set; }

        public virtual Gender Gender { get; set; }

        public virtual bool IsPostCategory
        {
            get
            {
                return Category == ChampionshipWinningCategories.MistrzMistrzow || Category == ChampionshipWinningCategories.Druzynowa;
            }
        }
    }


    [Serializable]
    [DebuggerDisplay("{Customer.Customer.LastName}:{Position}")]
    public class ChampionshipResultItem : FMGlobalObject
    {
        public virtual ChampionshipCategory Category { get; set; }

        public virtual ChampionshipCustomer Customer { get; set; }

        public virtual int Position { get; set; }

        public virtual ChampionshipGroup Group { get; set; }

        public virtual decimal Value { get; set; }
    }

    [Serializable]
    public class Championship : ScheduleEntryBase
    {
        [NonSerialized]
        private ICollection<ChampionshipEntry> _entries;
        [NonSerialized]
        private ICollection<ChampionshipGroup> _groups;
        [NonSerialized]
        private ICollection<ChampionshipCustomer> _customers;

        private ICollection<ChampionshipResultItem> _results;
        private ICollection<ChampionshipCategory> _categories;

        public Championship()
        {
            Entries = new HashSet<ChampionshipEntry>();
            Groups = new HashSet<ChampionshipGroup>();
            Customers = new HashSet<ChampionshipCustomer>();
            Results = new HashSet<ChampionshipResultItem>();
            Categories = new HashSet<ChampionshipCategory>();
            TeamCount = 5;
        }

        public virtual ICollection<ChampionshipResultItem> Results
        {
            get { return _results; }
            set { _results = value; }
        }

        public virtual ICollection<ChampionshipCategory> Categories
        {
            get { return _categories; }
            set { _categories = value; }
        }


        public virtual ChampionshipType ChampionshipType { get; set; }

        public virtual string Name { get; set; }

        public virtual string Comment { get; set; }

        /// <summary>
        /// określa ile najlepszych miejsc wchodzi do obliczenia punktacji dla druzyny
        /// </summary>
        public virtual int TeamCount { get; set; }

        public virtual ICollection<ChampionshipEntry> Entries
        {
            get { return _entries; }
            set { _entries = value; }
        }

        public virtual ICollection<ChampionshipGroup> Groups
        {
            get { return _groups; }
            set { _groups = value; }
        }

        public virtual ICollection<ChampionshipCustomer> Customers
        {
            get { return _customers; }
            set { _customers = value; }
        }

        public override bool IsLocked
        {
            get { return State != ScheduleEntryState.Planned; }
        }
    }
}
