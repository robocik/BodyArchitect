using System;
using System.Drawing;
using System.Windows.Media;
using BodyArchitect.Service.V2.Model;
using Microsoft.Windows.Controls.Ribbon;

namespace BodyArchitect.Client.Common.Plugins
{
    public class ShareSocialContent
    {
        public string Name { get; set; }

        public string Message { get; set; }

        public string Caption { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }
    }

    public interface IEntryObjectProvider
    {
        Guid GlobalId { get; }

        Type EntryObjectControl { get; }

        ImageSource ModuleImage
        {
            get;
        }

        Type EntryObjectType
        {
            get;
        }


        ShareSocialContent ShareToSocial(EntryObjectDTO entryObj);
    }
}
