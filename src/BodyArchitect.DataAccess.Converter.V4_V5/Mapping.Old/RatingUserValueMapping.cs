using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model.Old;
using BodyArchitect.Shared;
using FluentNHibernate.Mapping;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings.Old
{
    public class RatingUserValueMapping:ClassMap<RatingUserValue>
    {
        public RatingUserValueMapping()
        {
            this.Not.LazyLoad();
            Id(x => x.Id);
            Map(x => x.ProfileId).Not.Nullable();
            Map(x => x.RatedObjectId).Not.Nullable();
            Map(x => x.Rating).Not.Nullable();
            Map(x => x.VotedDate).Not.Nullable();
            Map(x => x.ShortComment).Length(Constants.ShortCommentColumnLength).Nullable();
        }
    }
}
