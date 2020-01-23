using System;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.UI.UserControls
{
    public class MessageEventArgs:EventArgs
    {
        public MessageDTO Message { get; private set; }

        public MessageEventArgs(MessageDTO message)
        {
            Message = message;
        }
    }
}
