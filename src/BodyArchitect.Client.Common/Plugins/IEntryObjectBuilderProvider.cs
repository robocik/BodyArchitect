using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Common.Plugins
{
    public interface IEntryObjectBuilderProvider
    {
        void EntryObjectCreated(EntryObjectDTO entryObject);
    }
}
