using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings
{
    public class RatingUserValueMapping : ClassMapping<RatingUserValue>
    {
        public RatingUserValueMapping()
        {
            Id(x => x.GlobalId, map => map.Generator(Generators.GuidComb));
            Property(b => b.ProfileId, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.RatedObjectId, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.Rating, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.VotedDate, y =>
            {
                y.NotNullable(true);
                
            });
            Property(b => b.ShortComment, y =>
            {
                y.NotNullable(false);
                y.Length(Constants.ShortCommentColumnLength);
            });

            ManyToOne(x => x.LoginData, g =>
            {
                g.NotNullable(false);
                g.Column("LoginData_id");
                g.Cascade(Cascade.None);
            });
        }
    }
}
