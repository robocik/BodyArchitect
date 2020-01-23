using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace BodyArchitect.Service.V2.Model
{
    public class A6WManager
    {
        static List<A6WDay> data = new List<A6WDay>() {new A6WDay(1,1,6),
                                                new A6WDay(2,2,6),
                                                new A6WDay(3,2,6),
                                                new A6WDay(4,3,6),
                                                new A6WDay(5,3,6),
                                                new A6WDay(6,3,6),
                                                new A6WDay(7,3,8),
                                                new A6WDay(8,3,8),
                                                new A6WDay(9,3,8),
                                                new A6WDay(10,3,8),
                                                new A6WDay(11,3,10),
                                                new A6WDay(12,3,10),
                                                new A6WDay(13,3,10),
                                                new A6WDay(14,3,10),
                                                new A6WDay(15,3,12),
                                                new A6WDay(16,3,12),
                                                new A6WDay(17,3,12),
                                                new A6WDay(18,3,12),
                                                new A6WDay(19,3,14),
                                                new A6WDay(20,3,14),
                                                new A6WDay(21,3,14),
                                                new A6WDay(22,3,14),
                                                new A6WDay(23,3,16),
                                                new A6WDay(24,3,16),
                                                new A6WDay(25,3,16),
                                                new A6WDay(26,3,16),
                                                new A6WDay(27,3,18),
                                                new A6WDay(28,3,18),
                                                new A6WDay(29,3,18),
                                                new A6WDay(30,3,18),
                                                new A6WDay(31,3,20),
                                                new A6WDay(32,3,20),
                                                new A6WDay(33,3,20),
                                                new A6WDay(34,3,20),
                                                new A6WDay(35,3,22),
                                                new A6WDay(36,3,22),
                                                new A6WDay(37,3,22),
                                                new A6WDay(38,3,22),
                                                new A6WDay(39,3,24),
                                                new A6WDay(40,3,24),
                                                new A6WDay(41,3,24),
                                                new A6WDay(42,3,24)};


        public static ReadOnlyCollection<A6WDay> Days
        {
            get
            {
                return data.AsReadOnly();
            }
        }

        public static  A6WDay LastDay
        {
         get { return data[data.Count-1]; }
        }
    }
}
