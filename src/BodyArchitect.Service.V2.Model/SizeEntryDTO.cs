using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using BodyArchitect.Portable;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;


namespace BodyArchitect.Service.V2.Model
{
    [EntryObjectInstance(EntryObjectInstance.Multiple)]
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
            get { return Status==EntryObjectStatus.Done && (Wymiary == null || Wymiary.IsEmpty); }
        }

        public virtual object Clone()
        {
            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Comment = Comment;
            sizeEntry.ReportStatus = ReportStatus;
            sizeEntry.Name = Name;
            sizeEntry.RemindBefore = RemindBefore;
            sizeEntry.Status = Status;
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
                Wymiary.Time.DateTime = DateHelper.MoveToNewDate(Wymiary.Time.DateTime, newDateTime);
            }
        }
        #endregion
    }
}
