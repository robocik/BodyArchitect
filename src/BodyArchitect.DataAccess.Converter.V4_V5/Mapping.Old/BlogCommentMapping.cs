using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model.Old;
using FluentNHibernate.Mapping;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings.Old
{
    public class BlogCommentMapping:ClassMap<BlogComment>
    {
        public BlogCommentMapping()
        {
            this.Not.LazyLoad();
            Id(x => x.Id);
            References(x => x.Profile).Not.Nullable();
            References(x => x.BlogEntry).Not.Nullable();
            Map(x => x.Comment).CustomType("StringClob").Not.Nullable();
            Map(x => x.DateTime).Not.Nullable();
            Map(x => x.CommentType).CustomType<ContentType>().Not.Nullable();
        }
    }
}
