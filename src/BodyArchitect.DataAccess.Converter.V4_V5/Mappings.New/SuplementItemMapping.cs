using BodyArchitect.Model;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings
{
    public class SuplementItemMapping : ClassMapping<SuplementItem>
    {
        public SuplementItemMapping()
        {
            Id(x => x.GlobalId, map => map.Generator(Generators.GuidComb));
            Property(b => b.DosageType, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.Dosage, y =>
            {
                y.NotNullable(true);
            });

            //TODO:Change UTC
            Component(b => b.Time);

            ManyToOne(x => x.Suplement, map =>
            {
                map.Cascade(Cascade.None);
                map.NotNullable(true);
                map.Column("Suplement_id");
            });

            ManyToOne(x => x.SuplementsEntry, map =>
            {
                map.Cascade(Cascade.None);
                map.NotNullable(true);
                map.Column("SuplementsEntry_id");
            });
            Property(b => b.Name, y =>
            {
                y.NotNullable(false);
                y.Length(Constants.NameColumnLength);
            });
            Property(b => b.Position, y =>
            {
                y.NotNullable(true);
            });
            
            Property(b => b.Comment, y =>
            {
                y.NotNullable(false);
                y.Type(NHibernateUtil.StringClob);
            });


        }

    }
}
