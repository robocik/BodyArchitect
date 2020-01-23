using System;
using BodyArchitect.Shared;

namespace BodyArchitect.Model
{
    public enum DosageType
    {
        Grams,
        Tablets,
        Units,
        Servings,
        MiliGrams
    }

    [Serializable]
    public class SuplementItem : FMGlobalObject, ICommentable
    {
        public SuplementItem()
        {
            Time=new BATime();
        }

        public virtual string Comment
        {
            get;
            set;
        }

        public virtual string Name
        {
            get;
            set;
        }

        public virtual SuplementsEntry SuplementsEntry { get; set; }

        public virtual Suplement Suplement { get; set; }

        public virtual BATime Time { get; set; }

        public virtual decimal Dosage { get; set; }

        public virtual DosageType DosageType { get; set; }

        public virtual int Position { get; set; }
    }
}
