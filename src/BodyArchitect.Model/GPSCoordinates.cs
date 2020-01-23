using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model
{
    [Serializable]
    public class GPSCoordinates : FMGlobalObject
    {
        public virtual byte[] Content { get; set; }
    }
}
