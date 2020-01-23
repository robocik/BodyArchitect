using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.DataAccess.Converter.V4_V5;

namespace BodyArchitect.UnitTests.DbConverter.V4V5
{
    class BACallbackMock : IBADatabaseCallback
    {


        public void ConvertProgressChanged(BADatabaseCallbackParam param)
        {
        }
    }
}
