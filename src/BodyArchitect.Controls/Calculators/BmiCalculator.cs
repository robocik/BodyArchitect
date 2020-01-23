using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Controls.Calculators
{
    public enum BmiResult
    {
        Incorrect,
        Wyglodzenie,
        Wychudzenie,
        Niedowaga,
        Normal,
        PrzedOtyloscia,
        IStopienOtylosci,
        IIStopienOtylosci,
        IIIStopienOtylosci

    }

    public class BmiCalculator
    {
        public static double Calculate(double height,double weight)
        {
            height = height/100;
            double bmi = weight/(height*height);
            return bmi;
        }

        public static BmiResult GetResult(double bmi,bool male)
        {
            if(bmi<15)
            {
                return BmiResult.Wyglodzenie;
            }
            else if(bmi>=15 && bmi<=16.6)
            {
                return BmiResult.Wychudzenie;
            }
            else if(bmi>=16.7 && bmi<=18.5)
            {
                return BmiResult.Niedowaga;
            }
            else if(bmi>=18.6 && bmi<=24.9)
            {
                return BmiResult.Normal;
            }
            else if(bmi>=25 && bmi<=29.9)
            {
                return BmiResult.PrzedOtyloscia;
            }
            else if(bmi>30 && bmi<34.9)
            {
                return BmiResult.IStopienOtylosci;
            }
            else if(bmi >=35 && bmi<=39.9)
            {
                return BmiResult.IIStopienOtylosci;
            }
            else if (bmi > 39.9)
            {
                return BmiResult.IIIStopienOtylosci;
            }
            return BmiResult.Incorrect;
        }
    }
}
