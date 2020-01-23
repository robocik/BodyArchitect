using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Shared
{
    public interface ISignedData
    {
        byte[] Signature { get; set; }
    }
}
