using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model.Old;
using FluentNHibernate.Mapping;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings.Old
{
    public class BlogEntryMapping: SubclassMap<BlogEntry>
    {
        public BlogEntryMapping()
        {
            this.Not.LazyLoad();
            Map(x => x.LastCommentDate).Nullable();
            Map(x => x.AllowComments).Not.Nullable();
            HasMany(x => x.Comments).Inverse().LazyLoad().Cascade.All().Fetch.Join().ReadOnly();
            Map(x => x.BlogCommentsCount).Formula("(select count(*) from BlogComment where BlogComment.BlogEntry_id = EntryObject_id)");
        }
    }
}
