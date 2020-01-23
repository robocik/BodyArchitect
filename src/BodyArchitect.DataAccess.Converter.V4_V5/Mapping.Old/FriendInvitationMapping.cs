using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model.Old;
using FluentNHibernate.Mapping;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings.Old
{
    public class FriendInvitationMapping:ClassMap<FriendInvitation>
    {
        public FriendInvitationMapping()
        {
            this.Not.LazyLoad();
            CompositeId().KeyReference(x => x.Inviter).KeyReference(x => x.Invited);
            Map(x => x.CreateDate).Not.Nullable();
            Map(x => x.InvitationType).Not.Nullable();
            Map(x => x.Message).Nullable().Length(500);
        }
    }
}
