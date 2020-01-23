using BodyArchitect.Model;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.NHibernate.Mappings
{
    public class BlogEntryMapping : JoinedSubclassMapping<BlogEntry>
    {
        public BlogEntryMapping()
        {
            Key(x => x.Column("EntryObject_id"));
            
            //Map(x => x.LastCommentDate).Nullable();
            //Map(x => x.AllowComments).Not.Nullable();
            //HasMany(x => x.Comments).Inverse().LazyLoad().Cascade.All().Fetch.Join().BatchSize(20).ReadOnly();
            //Map(x => x.TrainingDayCommentsCount).Formula("(select count(*) from TrainingDayComment where TrainingDayComment.BlogEntry_id = EntryObject_id)");
        }
    }
}
