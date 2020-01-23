using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Portable;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.V2.Model
{
    [Serializable]
    [EntryObjectInstance(EntryObjectInstance.Multiple)]
    [EntryObjectOperationsAttribute(EntryObjectOperation.Copy | EntryObjectOperation.Move)]
    [DataContract(Namespace = Const.ServiceNamespace,IsReference = true)]
    public class SuplementsEntryDTO : SpecificEntryObjectDTO, ICloneable,IMovable
    {
        public static readonly Guid EntryTypeId = new Guid("53AF5DC7-5D67-4D95-B571-6762BBA3BCE9");

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
        public List<SuplementItemDTO> Items { get; set; }

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
            entry.RemindBefore = RemindBefore;
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
                item.Time.DateTime = DateHelper.MoveToNewDate(item.Time.DateTime, newDateTime);
            }
        }
    }
}
