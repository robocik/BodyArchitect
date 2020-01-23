using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using BodyArchitect.Service.V2.Model.Localization;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;


namespace BodyArchitect.Service.V2.Model
{
    [Serializable]
    [EntryObjectInstance(EntryObjectInstance.None)]
    [EntryObjectOperationsAttribute(EntryObjectOperation.None)]
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class A6WEntryDTO : SpecificEntryObjectDTO
    {
        public static readonly Guid EntryTypeId = new Guid("461C9162-E81F-4635-A618-077E6EAAEBB1");

        #region Persistent properties

        [RangeValidator(1, RangeBoundaryType.Inclusive, 42, RangeBoundaryType.Inclusive, MessageTemplateResourceName = "A6WEntryDTO_DayNumber_Range", MessageTemplateResourceType = typeof(ValidationStrings))]
        [DataMember]
        [DoNotChecksum]
        public A6WDay Day
        {
            get; set;
        }

        [DataMember]
        public bool Completed
        {
            get; set;
        }

        [DataMember]
        public int? Set1
        {
            get; set;
        }

        [DataMember]
        public int? Set2
        {
            get; set;
        }

        [DataMember]
        public int? Set3
        {
            get; set;
        }


        #endregion

        #region Methods

        //public Dictionary<int, A6WEntryDTO> GetA6WTrainingEntries()
        //{
        //    Dictionary<int, A6WEntryDTO> res = new Dictionary<int, A6WEntryDTO>();
        //    if (MyTraining.Id != Constants.UnsavedObjectId)
        //    {
        //        var entries = MyTraining.EntryObjects;
        //        foreach (A6WEntryDTO a6WEntry in entries)
        //        {
        //            if (!res.ContainsKey(a6WEntry.DayNumber))
        //            {
        //                res.Add(a6WEntry.DayNumber, a6WEntry);
        //            }
        //        }
        //    }
        //    return res;
        //}


        public int GetNextDayNumber(IList<EntryObjectDTO> entries)
        {
            var a6wList = entries.Cast<A6WEntryDTO>();
            var res = from e in a6wList
                      orderby e.Day.DayNumber descending
                      select e.Day.DayNumber;
            int dayNumber = res.FirstOrDefault();
            return dayNumber + 1;

        }

        #endregion
    }
}
