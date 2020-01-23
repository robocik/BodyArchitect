using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace BodyArchitect.DataAccess.NHibernate
{
    public class CustomDebugWriter : TextWriter
    {
        public override void WriteLine(string value)
        {
            Debug.WriteLine(value);
            base.WriteLine(value);
        }

        public override void Write(string value)
        {
            Debug.Write(value);
            base.Write(value);
        }
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}
