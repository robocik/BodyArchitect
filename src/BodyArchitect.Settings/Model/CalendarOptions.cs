using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Settings.Model
{
    

    [Serializable]
    public class CalendarOptionsItem
    {
        public Guid ModuleId { get; set; }

        public bool? IsDefault { get; set; }

        public int Order { get; set; }

        public CalendarOptionsItem()
        {
            Order = int.MaxValue;
        }

        public CalendarOptionsItem(Guid moduleId,bool isDefault)
        {
            ModuleId = moduleId;
            IsDefault = isDefault;
            Order = int.MaxValue;
        }
    }
    [Serializable]
    public class CalendarOptions
    {
        private List<CalendarOptionsItem> defaultEntries = new List<CalendarOptionsItem>();

        public CalendarOptions()
        {
            defaultEntries.Add(new CalendarOptionsItem(new Guid("F69099BE-C42A-4EC5-B582-5EFF57758503"),true){Order = 0});//strength training
            defaultEntries.Add(new CalendarOptionsItem(new Guid("083F5171-0D6C-46B4-B72A-155DB3C0270A"), true) { Order = 1 });//sizes
            defaultEntries.Add(new CalendarOptionsItem(new Guid("7240E588-75C5-4FE0-8688-5ED4D9532DD6"), false) { Order = 2 });//open cardio
            defaultEntries.Add(new CalendarOptionsItem(new Guid("BD58BEAE-7E5F-424F-92A7-75CA132508FB"), false) { Order = 3 });//supplements
            defaultEntries.Add(new CalendarOptionsItem(new Guid("29E5BFEB-D8D0-434C-BF37-B22F18E23E9A"), false) { Order = 4 });//blog
            defaultEntries.Add(new CalendarOptionsItem(new Guid("A7B7503A-FF26-414D-A8A5-242C137B426C"), false) { Order = 5 });//a6w
            defaultEntries.Add(new CalendarOptionsItem(new Guid("A18353CA-5767-46E5-AEC9-3E5C193E6E15"), false) { Order = 6 });//summary
        }

        public List<CalendarOptionsItem> DefaultEntries
        {
            get{ 
                if(defaultEntries==null)
                {
                    defaultEntries = new List<CalendarOptionsItem>();
                }
                return defaultEntries;
            }
        }

        public bool GetDefaultEntry(Guid id)
        {
            var module = DefaultEntries.Where(x => x.ModuleId == id).SingleOrDefault();
            var isDefault =module!=null && module.IsDefault==true;
            return isDefault;
        }

    }
}
