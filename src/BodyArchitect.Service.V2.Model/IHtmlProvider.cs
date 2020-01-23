using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Service.V2.Model
{
    public interface IHtmlProvider
    {
        string GetHtml();

        string Title { get; }

        Guid GlobalId { get; }

        string Language { get; }
    }
}
