using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Service.Model
{
    [Serializable]
    public struct A6WDay
    {
        int day;
        int setNumber;
        int repetitionNumber;
        
        public A6WDay(int dayNumber,int setNumber, int repetitionNumber)
        {
            this.day = dayNumber;
            this.setNumber = setNumber;
            this.repetitionNumber = repetitionNumber;
        }

        public int SetNumber
        {
            get
            {
                return setNumber;
            }
        }

        public int DayNumber
        {
            get
            {
                return day;
            }
        }

        public int RepetitionNumber
        {
            get
            {
                return repetitionNumber;
            }
        }

        public override string ToString()
        {
            return day.ToString();
        }
    }
}
