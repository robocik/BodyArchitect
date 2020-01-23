using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Service.V2.Model
{
    public partial class SuplementsEntryDTO
    {
        public SuplementsEntryDTO()
        {
            Items = new List<SuplementItemDTO>();
        }

        public SuplementItemDTO GetItem(Guid instanceId)
        {
            return (from e in Items where e.InstanceId == instanceId select e).Single();
        }
    }
}
