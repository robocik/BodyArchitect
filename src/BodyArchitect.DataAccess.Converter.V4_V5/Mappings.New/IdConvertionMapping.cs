using BodyArchitect.Model;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings
{
    //TODO:Remove or not?
    //public class IdConvertionMapping : ClassMapping<IdConvertion>
    //{
    //    public IdConvertionMapping()
    //    {
    //        Mutable(true);
    //        ComposedId(key =>
    //        {
    //            key.Property(a => a.IntId);
    //            key.Property(a => a.Type);
    //        });
            
    //        Property(b => b.Type, map =>
    //        {
    //            map.NotNullable(true);
    //            map.UniqueKey("int_type_idx");
    //        });
    //        Property(b => b.IntId, map =>
    //        {
    //            map.NotNullable(true);
    //            map.UniqueKey("int_type_idx");
    //        });
    //        Property(b => b.GuidId, map =>
    //        {
    //            map.NotNullable(true);
    //        });
    //    }
    //}
}
