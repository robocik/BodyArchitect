using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using BodyArchitect.DataAccess.Converter.V4_V5;
using NUnit.Framework;

namespace BodyArchitect.UnitTests.DbConverter.V4V5
{
    [TestFixture]
    public class MyTest
    {
        [Test]
        public void Test()
        {
            
            float fl = (float)7.77778E+31;
            if(fl>(double) decimal.MaxValue || fl<(double) decimal.MinValue)
            {
                fl = 0;
            }
            fl=(float) Math.Round(fl, 0);
            fl = (float) Math.Ceiling(fl);
            decimal ttt = Convert.ToDecimal(fl);
        }

        [Test]
        public void TestDecimalPlaces()
        {
            decimal val1 = 32.05m;
            decimal val2 = 23.000003m;
            decimal val3 = 34.82310534m;
            decimal val4 = 100.4545403m;
            decimal val5 = 34m;
            decimal val6 = 100.25m;


            Assert.IsTrue(val1.IsInDbFormat());
            Assert.IsFalse(val2.IsInDbFormat());
            Assert.IsFalse(val3.IsInDbFormat());
            Assert.IsFalse(val4.IsInDbFormat());
            Assert.IsTrue(val5.IsInDbFormat());
            Assert.IsTrue(val6.IsInDbFormat());

        }
    }
}
