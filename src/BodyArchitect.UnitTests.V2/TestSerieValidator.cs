using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Validators;
using NUnit.Framework;

namespace BodyArchitect.UnitTests.V2
{
    

    [TestFixture]
    public class TestSerieValidator
    {
        [TestFixtureSetUp]
        public void TestSetup()
        {
            CultureInfo pl = new CultureInfo("pl-PL");//set polish culture because in set values I use , as a decimal separator
            Thread.CurrentThread.CurrentCulture = pl;
        }



        #region Wrong
        [Test]
        public void Wrong_1()
        {
            var item = createStrengthTrainingItem(ExerciseType.Klatka, "15x25", "15x325", "12x30", "12x30");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(1, wrongSets.Count);
            Assert.AreEqual(325, wrongSets[0].Weight);
        }

        [Test]
        public void Wrong_2()
        {
            var item = createStrengthTrainingItem(ExerciseType.Klatka, "5x20", "5x20", "5x28", "3x42", "2x5456", "5x70", "5x70", "5x70");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(1, wrongSets.Count);
            Assert.AreEqual(5456, wrongSets[0].Weight);
        }

        [Test]
        public void Wrong_3()
        {
            var item = createStrengthTrainingItem(ExerciseType.Klatka, "8x95105", "8x105", "8x115");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(1, wrongSets.Count);
            Assert.AreEqual(95105, wrongSets[0].Weight);
        }

        [Test]
        public void Wrong_4()
        {
            var item = createStrengthTrainingItem(ExerciseType.Klatka, "12x20", "8x225", "8x20");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(1, wrongSets.Count);
            Assert.AreEqual(225, wrongSets[0].Weight);
        }

        [Test]
        public void Wrong_5()
        {
            var item = createStrengthTrainingItem(ExerciseType.Klatka, "12x1580", "10x17,5", "8x20");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(1, wrongSets.Count);
            Assert.AreEqual(1580, wrongSets[0].Weight);
        }

        [Test]
        public void Wrong_6()
        {
            var item = createStrengthTrainingItem(ExerciseType.Klatka, "8x42,5", "8x42,5", "8x42,5", "8x42,5", "4x130");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(1, wrongSets.Count);
            Assert.AreEqual(130, wrongSets[0].Weight);
        }

        [Test]
        public void Wrong_7()
        {
            var item = createStrengthTrainingItem(ExerciseType.Klatka, "8x300", "8x30", "8x30", "8x30");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(1, wrongSets.Count);
            Assert.AreEqual(300, wrongSets[0].Weight);
        }

        [Test]
        public void Wrong_8()
        {
            var item = createStrengthTrainingItem(ExerciseType.Klatka, "15x720", "10x30", "6x40");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(1, wrongSets.Count);
            Assert.AreEqual(720, wrongSets[0].Weight);
        }

        [Test]
        public void Wrong_9()
        {
            var item = createStrengthTrainingItem(ExerciseType.Klatka, "12x60", "12x800", "10x100", "8x100", "5x120");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(1, wrongSets.Count);
            Assert.AreEqual(800, wrongSets[0].Weight);
        }

        [Test]
        public void Wrong_10()
        {
            var item = createStrengthTrainingItem(ExerciseType.Klatka, "14x2018", "14x20", "14x20");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(1, wrongSets.Count);
            Assert.AreEqual(2018, wrongSets[0].Weight);
        }

        [Test]
        public void Wrong_11()
        {
            var item = createStrengthTrainingItem(ExerciseType.Klatka, "12x60", "12x800", "10x100", "8x100", "5x120");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(1, wrongSets.Count);
            Assert.AreEqual(800, wrongSets[0].Weight);
        }

        [Test]
        public void Wrong_12()
        {
            var item = createStrengthTrainingItem(ExerciseType.Klatka, "14x2018", "14x20", "14x20");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(1, wrongSets.Count);
            Assert.AreEqual(2018, wrongSets[0].Weight);
        }

        [Test]
        public void Wrong_13()
        {
            var item = createStrengthTrainingItem(ExerciseType.Klatka, "15x50", "15x50", "15x550", "15x50");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(1, wrongSets.Count);
            Assert.AreEqual(550, wrongSets[0].Weight);
        }

        [Test]
        public void Wrong_14()
        {
            var item = createStrengthTrainingItem(ExerciseType.Klatka, "10x20", "12x370", "15x60", "15x70");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(1, wrongSets.Count);
            Assert.AreEqual(370, wrongSets[0].Weight);
        }


        [Test]
        public void Wrong_15()
        {
            var item = createStrengthTrainingItem(ExerciseType.Klatka, "15x40", "15x60", "15x270", "15x80");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(1, wrongSets.Count);
            Assert.AreEqual(270, wrongSets[0].Weight);
        }
        #endregion

        #region Good

        [Test]
        public void Good_1()
        {
            var item = createStrengthTrainingItem(ExerciseType.Klatka, "15x25", "15x25", "12x30", "12x30");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(0, wrongSets.Count);
        }

        [Test]
        public void Good_2()
        {
            var item = createStrengthTrainingItem(ExerciseType.Klatka, "5x20", "5x20", "5x28", "3x42", "2x54", "4x56", "5x70", "5x70", "5x70");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(0, wrongSets.Count);
        }

        [Test]
        public void Good_3()
        {
            var item = createStrengthTrainingItem(ExerciseType.Klatka, "8x95", "7x105", "8x105", "8x115");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(0, wrongSets.Count);
        }

        [Test]
        public void Good_4()
        {
            var item = createStrengthTrainingItem(ExerciseType.Klatka, "12x20", "8x22", "8x20");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(0, wrongSets.Count);
        }

        [Test]
        public void Good_5()
        {
            var item = createStrengthTrainingItem(ExerciseType.Klatka, "12x15", "10x17,5", "8x20");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(0, wrongSets.Count);
        }

        [Test]
        public void Good_6()
        {
            var item = createStrengthTrainingItem(ExerciseType.Klatka, "8x42,5", "8x42,5", "8x42,5", "8x42,5");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(0, wrongSets.Count);
        }

        [Test]
        public void Good_7()
        {
            var item = createStrengthTrainingItem(ExerciseType.Klatka, "8x30", "8x30", "8x30", "8x30");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(0, wrongSets.Count);
        }

        [Test]
        public void Good_8()
        {
            var item = createStrengthTrainingItem(ExerciseType.Klatka, "15x20", "10x30", "6x40");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(0, wrongSets.Count);
        }

        [Test]
        public void Good_9()
        {
            var item = createStrengthTrainingItem(ExerciseType.Klatka, "12x60", "12x80", "10x100", "8x100", "5x120");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(0, wrongSets.Count);
        }

        [Test]
        public void Good_10()
        {
            var item = createStrengthTrainingItem(ExerciseType.Klatka, "14x18", "14x20", "14x20");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(0, wrongSets.Count);
        }

        [Test]
        public void Good_11()
        {
            var item = createStrengthTrainingItem(ExerciseType.Klatka, "12x60", "12x80", "10x100", "8x100", "5x120");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(0, wrongSets.Count);
        }

        [Test]
        public void Good_12()
        {
            var item = createStrengthTrainingItem(ExerciseType.Klatka, "14x18", "14x20", "14x20");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(0, wrongSets.Count);
        }

        [Test]
        public void Good_13()
        {
            var item = createStrengthTrainingItem(ExerciseType.Klatka, "15x50", "15x50", "15x55", "15x50");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(0, wrongSets.Count);
        }

        [Test]
        public void Good_14()
        {
            var item = createStrengthTrainingItem(ExerciseType.Nogi, "15x50", "15x50", "15x105", "15x50");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(0, wrongSets.Count);
        }

        [Test]
        public void Good_15()
        {
            var item = createStrengthTrainingItem(ExerciseType.Biceps, "15x20", "15x20", "15x20", "15x40");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(0, wrongSets.Count);
        }

        [Test]
        public void Good_16()
        {
            var item = createStrengthTrainingItem(ExerciseType.Barki, "12x40", "10x50", "7x60", "7x60", "7x60","6x62,5");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(0, wrongSets.Count);
        }


        [Test]
        public void Good_17()
        {
            var item = createStrengthTrainingItem(ExerciseType.Nogi, "15x140", "15x190", "15x240", "15x240", "15x240", "15x240", "15x240");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(0, wrongSets.Count);
        }

        [Test]
        public void Good_18()
        {
            var item = createStrengthTrainingItem(ExerciseType.Nogi, "15x140", "15x", "15x240", "15x240", "15x240", "15x240", "15x240");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(0, wrongSets.Count);
        }

        [Test]
        public void Good_19()
        {
            var item = createStrengthTrainingItem(ExerciseType.Klatka, "15x30", "15x50", "15x70", "15x120");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(0, wrongSets.Count);
        }
        [Test]
        public void Good_20()
        {
            var item = createStrengthTrainingItem(ExerciseType.Klatka, "8x165", "8x181", "8x197", "4x");
            SerieValidator validator = new SerieValidator();
            var wrongSets = validator.GetIncorrectSets(item);
            Assert.AreEqual(0, wrongSets.Count);
        }
        #endregion

        StrengthTrainingItemDTO createStrengthTrainingItem(ExerciseType exerciseType,params string[] sets)
        {
            ExerciseDTO exercise = new ExerciseDTO();
            exercise.ExerciseType = exerciseType;
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise;

            foreach (var serie in sets)
            {
                SerieDTO set = new SerieDTO(serie);
                item.AddSerie(set);
            }

            return item;
        }
    }
}
