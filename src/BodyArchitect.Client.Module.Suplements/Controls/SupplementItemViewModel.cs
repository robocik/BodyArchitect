using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.Suplements.Controls
{
    public class SupplementItemViewModel : ViewModelBase, IDataErrorInfo
    {
        private SuplementItemDTO item;
        private bool readOnly;

        public SupplementItemViewModel()
        {
        }

        public SupplementItemViewModel(SuplementItemDTO item, bool readOnly)
        {
            this.item = item;
            this.readOnly = readOnly;
        }

        public string Comment
        {
            get { return item.Comment; }
            set
            {
                item.Comment = value;
                NotifyOfPropertyChange(() => Comment);
            }
        }

        public SuplementDTO Supplement
        {
            get { return item.Suplement; }
            set
            {
                item.Suplement = value;

                NotifyOfPropertyChange(() => Supplement);
                NotifyOfPropertyChange(() => IsNew);
            }
        }

        public string Name
        {
            get { return item.Name; }
            set
            {
                item.Name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

  

        public DateTime Time
        {
            get { return item.Time.DateTime; }
            set
            {
                item.Time.DateTime = value;
                NotifyOfPropertyChange(() => Time);
            }
        }

        public bool HasTime
        {
            get { return Time.TimeOfDay != TimeSpan.Zero; }
        }


        public decimal Dosage
        {
            get { return item.Dosage; }
            set
            {
                item.Dosage = value;
                NotifyOfPropertyChange(() => Dosage);
            }
        }

        public TimeType TimeType
        {
            get { return item.Time.TimeType; }
            set { item.Time.TimeType = value; }
        }

        public DosageType DosageType
        {
            get { return item.DosageType; }
            set { item.DosageType = value; }
        }

        public string this[string columnName]
        {
            get
            {
                // apply property level validation rules
                if (columnName == "SuplementId")
                {
                    if (Supplement.IsEmpty())
                    {
                        return EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Suplements:SuplementsEntryStrings:SupplementItemViewModel_columnName_EmptyId");
                    }

                }

                return EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Suplements:SuplementsEntryStrings:SupplementItemViewModel_EmptyString");
            }
        }

        public string Error
        {
            get
            {
                StringBuilder error = new StringBuilder();

                // iterate over all of the properties
                // of this object - aggregating any validation errors
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(this);
                foreach (PropertyDescriptor prop in props)
                {
                    string propertyError = this[prop.Name];
                    if (propertyError != string.Empty)
                    {
                        error.Append((error.Length != 0 ? ", " : "") + propertyError);
                    }
                }

                return error.ToString();
            }
        }

        public bool IsNew
        {
            get { return item.Suplement.IsEmpty(); }
        }

        public bool CanDelete
        {
            get { return !readOnly && !IsNew; }
        }

        
        public SuplementItemDTO GetItem()
        {
            if (item == null)
            {
                item = new SuplementItemDTO();
            }
            item.Comment = Comment;
            item.Name = Name;
            item.Suplement = Supplement;
            item.Time.DateTime = Time;
            item.Dosage = Dosage;
            item.DosageType = DosageType;
            return item;
        }
    }
}
