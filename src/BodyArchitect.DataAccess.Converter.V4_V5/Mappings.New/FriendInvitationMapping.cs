using BodyArchitect.Model;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings
{
    public class FriendInvitationMapping : ClassMapping<FriendInvitation>
    {
        public FriendInvitationMapping()
        {
            this.ComposedId(x=>
                                {
                                    x.ManyToOne(c => c.Inviter);
                                    x.ManyToOne(c=>c.Invited);
                                });
            //CompositeId().KeyReference(x => x.Inviter).KeyReference(x => x.Invited);
            Property(b => b.CreateDate, y =>
            {
                y.NotNullable(true);
                
            });
            Property(b => b.InvitationType, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.Message, y =>
            {
                y.NotNullable(false);
                y.Length(500);
            });
        }
    }
}
