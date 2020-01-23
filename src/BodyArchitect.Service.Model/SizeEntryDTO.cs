using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using BodyArchitect.Portable;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;


namespace BodyArchitect.Service.Model
{
    [EntryObjectInstance(EntryObjectInstance.Single)]
    [EntryObjectOperationsAttribute(EntryObjectOperation.Copy | EntryObjectOperation.Move)]
    [DataContract(Namespace = Const.ServiceNamespace)]
    [Serializable]
    public class SizeEntryDTO : SpecificEntryObjectDTO, IMovable, ICloneable
    {
        public SizeEntryDTO()
        {
            Wymiary = new WymiaryDTO();
        }

        [DataMember]
        [ObjectValidator]
        public WymiaryDTO Wymiary
        {
            get;
            set;
        }

        #region Methods

        public override bool IsEmpty
        {
            get { return Wymiary == null || Wymiary.IsEmpty; }
        }

        public virtual object Clone()
        {
            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Comment = Comment;
            sizeEntry.ReportStatus = ReportStatus;
            sizeEntry.Name = Name;
            if (Wymiary != null)
            {
                sizeEntry.Wymiary = Wymiary.Clone(false);
            }
            return sizeEntry;

        }

        public virtual void Move(DateTime newDateTime)
        {
            if (Wymiary != null)
            {
                Wymiary.DateTime = DateHelper.MoveToNewDate(Wymiary.DateTime, newDateTime);
            }
        }
        #endregion
    }
}
