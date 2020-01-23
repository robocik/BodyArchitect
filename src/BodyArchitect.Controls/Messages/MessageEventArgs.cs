using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Service.Model;

namespace BodyArchitect.Controls
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
