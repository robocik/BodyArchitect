using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model.Old;
using FluentNHibernate.Mapping;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings.Old
{
    public class MessageMapping: ClassMap<Message>
    {
        public MessageMapping()
        {
            this.Not.LazyLoad();
            Id(x => x.Id);
            References(x => x.Sender).Cascade.None().Fetch.Join();
            References(x => x.Receiver).Cascade.None().Fetch.Join();
            Map(x => x.MessageType).CustomType<MessageType>().Not.Nullable();
            Map(x => x.Topic).Length(100);
            Map(x => x.Priority).CustomType<MessagePriority>().Not.Nullable();
            Map(x => x.ContentType).CustomType<ContentType>().Not.Nullable();
            Map(x => x.Content).Length(2000);
            Map(x => x.CreatedDate).Not.Nullable();
        }
    }
}
