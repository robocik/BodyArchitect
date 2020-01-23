using BodyArchitect.Model;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings
{
    public class MessageMapping : ClassMapping<Message>
    {
        public MessageMapping()
        {
            Id(x => x.GlobalId, map =>
            {
                map.Generator(Generators.GuidComb);
            });

            ManyToOne(x => x.Sender, map =>
            {
                map.Cascade(Cascade.None);
                map.NotNullable(false);
                map.Column("Sender_id");
                map.Fetch(FetchKind.Join);
            });
            ManyToOne(x => x.Receiver, map =>
            {
                map.Cascade(Cascade.None);
                map.NotNullable(false);
                map.Column("Receiver_id");
                map.Fetch(FetchKind.Join);
            });
            Property(b => b.Topic, y =>
            {
                y.NotNullable(false);
                y.Length(100);
            });
            Property(b => b.Priority, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.Content, y =>
            {
                y.Length(2000);
            });
            Property(b => b.CreatedDate, y =>
            {
                y.NotNullable(true);
                
            });
            //References(x => x.Sender).Cascade.None().Fetch.Join();
            //References(x => x.Receiver).Cascade.None().Fetch.Join();
            //Map(x => x.MessageType).CustomType<MessageType>().Not.Nullable();
            //Map(x => x.Topic).Length(100);
            //Map(x => x.Priority).CustomType<MessagePriority>().Not.Nullable();
            //Map(x => x.ContentType).CustomType<ContentType>().Not.Nullable();
            //Map(x => x.Content).Length(2000);
            //Map(x => x.CreationDate).Not.Nullable();
        }
    }
}
