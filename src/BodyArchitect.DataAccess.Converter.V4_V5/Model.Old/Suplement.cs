using System;



namespace BodyArchitect.Model.Old
{
    [Serializable]
    public class Suplement : FMObject
    {
        public readonly static Suplement Removed;

        static Suplement()
        {
            Removed = new Suplement();
            Removed.Name = "";
        }

        //[Property(NotNull = true,Unique=true)]
        //[ValidateIsUnique]
        public virtual Guid SuplementId { get; set; }

        public virtual int? ProfileId { get; set; }
        //[Property(NotNull = true)]
        //[ValidateIsUnique(ResourceType = typeof(SuplementsEntryStrings), ErrorMessageKey = "Suplement_Name_Unique")]
        //[ValidateNonEmpty(ResourceType = typeof(SuplementsEntryStrings), ErrorMessageKey = "Suplement_Name_Empty")]
        //[ValidateLength(1, Constants.NameColumnLength, ResourceType = typeof(SuplementsEntryStrings), ErrorMessageKey = "Suplement_Name_Length")]
        public virtual string Name { get; set; }

        //[Property(NotNull = false, SqlType = "ntext", ColumnType = "StringClob")]
        public virtual string Comment { get; set; }

        public virtual string Url { get; set; }
        //public static IList<Suplement> GetAll()
        //{
        //    return FindAll(typeof(Suplement)).Cast<Suplement>().ToList();
        //}

    }
}
