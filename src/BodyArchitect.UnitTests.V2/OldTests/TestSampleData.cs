//using System;
//using System.Collections.Generic;
//using BodyArchitect.Module.StrengthTraining.Model;
//using BodyArchitect.Module.Suplements.Model;
//using NUnit.Framework;

//namespace BodyArchitect.UnitTests.V2
//{
//    [TestFixture]
//    public class TestSampleData
//    {
//        [Test]
//        public void TestUniquenessOfGuids_Suplements_PL()
//        {
//            SuplementsBuilder builder = new BodyArchitect.Module.Suplements.Model.SuplementsBuilder();
//            var list = builder.Create();
//            Dictionary<Guid, Suplement> suplements = new Dictionary<Guid, Suplement>();
//            foreach (var suplement in list)
//            {
//                suplements.Add(suplement.SuplementId, suplement);
//            }
//        }

//        [Test]
//        public void TestUniquenessOfGuids_Exercises_PL()
//        {
//            ExercisesBuilderPL builder = new ExercisesBuilderPL();
//            var list = builder.Create();
//            Dictionary<Guid, Exercise> exercisesByGuid = new Dictionary<Guid, Exercise>();
//            foreach (Exercise exercise in list)
//            {
//                exercisesByGuid.Add(exercise.GlobalId, exercise);
//            }
//        }

//        [Test]
//        public void TestUniquenessOfShortcuts_Exercises_PL()
//        {
//            ExercisesBuilderPL builder = new ExercisesBuilderPL();
//            var list = builder.Create();
//            Dictionary<string, Exercise> exercisesByShortcuts = new Dictionary<string, Exercise>();
//            foreach (Exercise exercise in list)
//            {
//                exercisesByShortcuts.Add(exercise.Shortcut, exercise);
//            }
//        }

//        [Test]
//        public void TestUniquenessOfGuids_Exercises_EN()
//        {
//            ExercisesBuilderEN builder = new ExercisesBuilderEN();
//            var list = builder.Create();
//            Dictionary<Guid, Exercise> exercisesByGuid = new Dictionary<Guid, Exercise>();
//            foreach (Exercise exercise in list)
//            {
//                Console.WriteLine(exercise.GlobalId);
//                exercisesByGuid.Add(exercise.GlobalId, exercise);
//            }
//        }

//        [Test]
//        public void TestUniquenessOfShortcuts_Exercises_EN()
//        {
//            ExercisesBuilderEN builder = new ExercisesBuilderEN();
//            var list = builder.Create();
//            Dictionary<string, Exercise> exercisesByShortcuts = new Dictionary<string, Exercise>();
//            foreach (Exercise exercise in list)
//            {
//                Console.WriteLine(exercise.GlobalId);
//                exercisesByShortcuts.Add(exercise.Shortcut, exercise);
//            }
//        }
//    }
//}
