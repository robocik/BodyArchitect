using System;


namespace BodyArchitect.Model.Old
{
    public enum DosageType
    {
        Grams,
        Tablets,
        Units,
        Servings
    }

    [Serializable]
    public class SuplementItem : FMObject
    {
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

        public virtual Guid SuplementId { get; set; }

        public virtual DateTime Time { get; set; }

        public virtual double Dosage { get; set; }

        public virtual DosageType DosageType { get; set; }

        public virtual object Clone()
        {
            var newItem = new SuplementItem();
            newItem.SuplementId = SuplementId;
            newItem.Time = Time;
            newItem.Name = Name;
            newItem.Dosage = Dosage;
            newItem.DosageType = DosageType;
            newItem.Comment = Comment;
            return newItem;
        }

    }
}
