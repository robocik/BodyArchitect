using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using ImageTools;

namespace BodyArchitect.WP7.Controls.Cache
{
    public class DeleteMessageServiceCommand: IServiceCommand
    {
        private Guid messageId;

        public DeleteMessageServiceCommand(Guid messageId)
        {
            this.messageId = messageId;
        }

        public void Execute()
        {
            try
            {
                var m = new ServiceManager<AsyncCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<AsyncCompletedEventArgs> operationCompleted)
                {
                    MessageOperationParam param = new MessageOperationParam();
                    param.MessageId = messageId;
                    param.Operation = MessageOperationType.Delete;
                    client1.MessageOperationAsync(ApplicationState.Current.SessionData.Token, param);
                });

                m.Run();

            }
            catch (Exception)
            {
                
            }


        }
    }
}
