using BodyArchitect.Model;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.NHibernate.Mappings
{
    public class WP7TrialMapping : ClassMapping<WP7Trial>
    {
        public WP7TrialMapping()
        {
            Id(x => x.DeviceId, map =>
            {
                map.Generator(Generators.Assigned);
                map.Length(200);
            });

            Property(b => b.TrialStartedDate, y =>
            {
                y.NotNullable(true);
                
            });
        }
    }
}
