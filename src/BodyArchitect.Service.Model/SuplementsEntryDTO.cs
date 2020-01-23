using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Portable;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.Model
{
    [Serializable]
    [EntryObjectInstance(EntryObjectInstance.Single)]
    [EntryObjectOperationsAttribute(EntryObjectOperation.Copy | EntryObjectOperation.Move)]
    [DataContract(Namespace = Const.ServiceNamespace,IsReference = true)]
    public class SuplementsEntryDTO : SpecificEntryObjectDTO, ICloneable,IMovable
    {
        public SuplementsEntryDTO()
        {
            Items = new List<SuplementItemDTO>();
        }

        public override bool IsEmpty
        {
            get { return Items.Count == 0; }
        }

        [NotNullValidator]
        [ObjectCollectionValidator(typeof(SuplementItemDTO))]
        [DataMember]
        public List<SuplementItemDTO> Items { get; private set; }

        public void AddItem(SuplementItemDTO item)
        {
            item.SuplementsEntry = this;
            Items.Add(item);
        }

        public void RemoveItem(SuplementItemDTO item)
        {
            Items.Remove(item);
            item.SuplementsEntry = null;
        }

        #region ICloneable Members

        public object Clone()
        {
            var entry = new SuplementsEntryDTO();
            entry.ReportStatus = ReportStatus;
            entry.Comment = Comment;
            entry.Name = Name;
            foreach (var suplementItem in Items)
            {
                entry.AddItem((SuplementItemDTO)suplementItem.Clone());
            }
            return entry;
        }

        #endregion

        public void Move(DateTime newDateTime)
        {
            foreach (var item in Items)
            {
                item.Time = DateHelper.MoveToNewDate(item.Time, newDateTime);
            }
        }
    }
}
