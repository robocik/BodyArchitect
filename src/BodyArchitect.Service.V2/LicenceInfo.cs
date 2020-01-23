using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Service.V2
{
    public enum LicenceType
    {
        Professional,
        Instructor
    }

    public class LicenceInfo
    {
        public int BAPoints { get; set; }

        public Guid Id { get; set; }
    }
}
