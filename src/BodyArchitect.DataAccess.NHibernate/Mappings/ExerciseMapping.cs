using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.NHibernate.Mappings
{
    public class ExerciseMapping : ClassMapping<Exercise>
    {
        public ExerciseMapping()
        {
            Id(x => x.GlobalId, map =>
            {
                map.Generator(Generators.Assigned);
            });

            Property(b => b.CreationDate, y =>
            {
                y.NotNullable(true);
            });

            Version(x => x.Version, map => map.Generated(VersionGeneration.Never));
            Property(b => b.Name, y =>
            {
                y.NotNullable(true);
                y.Length(Constants.NameColumnLength);
            });
            Property(b => b.Description, y =>
            {
                y.NotNullable(false);
                y.Type(NHibernateUtil.StringClob);
            });
            Property(b => b.Url, y =>
            {
                y.NotNullable(false);
                y.Length(Constants.UrlLength);
            });
            Property(b => b.IsDeleted, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.UseInRecords, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.Shortcut, y =>
            {
                y.NotNullable(true);
                y.Length(20);
            });
            Property(b => b.Met, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.Rating, y =>
            {
                y.NotNullable(true);
                //y.Formula("(select avg(RatingUserValue.Rating) from RatingUserValue where RatingUserValue.RatedObjectId=GlobalId)");
            });

            ManyToOne(x => x.Profile, g =>
            {
                g.NotNullable(false);
                g.Column("Profile_id");
                g.Lazy(LazyRelation.Proxy);
                g.Cascade(Cascade.None);
            });
            Property(b => b.MechanicsType, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.ExerciseForceType, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.ExerciseType, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.Difficult, y =>
            {
                y.NotNullable(true);
            });
        }
    }
}
