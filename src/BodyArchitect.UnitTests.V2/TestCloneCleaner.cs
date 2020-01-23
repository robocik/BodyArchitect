using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common;
using NUnit.Framework;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class CloneCleanerTests
    {
        internal class ExampleClass
        {
            [NotCloneable]
            public int Integer { get; set; }
            public int IntegerCloneable { get; set; }

            [NotCloneable]
            public string Strign { get; set; }
            public string StrignCloneable { get; set; }

            [NotCloneable]
            public double Double { get; set; }
            public double DoubleCloneable { get; set; }
        }

        [Test]
        public void Clean_should_clean_attributes_marked_with_NotCloneable()
        {
            var objectToClear = new ExampleClass()
            {
                Integer = 5,
                IntegerCloneable = 5,
                Strign = "text",
                StrignCloneable = "text",
                Double = 1.2,
                DoubleCloneable = 1.2
            };

            CloneCleaner.Clean(objectToClear);

            Assert.That(objectToClear.Integer, Is.EqualTo(0));
            Assert.That(objectToClear.IntegerCloneable, Is.EqualTo(5));
            Assert.That(objectToClear.Strign, Is.Null);
            Assert.That(objectToClear.StrignCloneable, Is.EqualTo("text"));
            Assert.That(objectToClear.Double, Is.EqualTo(0));
            Assert.That(objectToClear.DoubleCloneable, Is.EqualTo(1.2));
        }


        internal class ExampleClass2
        {
            [NotCloneable]
            public int? Integer { get; set; }
            public int? IntegerCloneable { get; set; }
        }

        [Test]
        public void Clean_should_set_null_for_nullable_type()
        {
            var objectToClear = new ExampleClass2()
            {
                Integer = 5,
                IntegerCloneable = 5
            };

            CloneCleaner.Clean(objectToClear);

            Assert.That(objectToClear.Integer, Is.Null);
            Assert.That(objectToClear.IntegerCloneable, Is.EqualTo(5));
        }

        internal class ExampleClass3
        {
            [NotCloneable]
            public Guid Guid { get; set; }
            public Guid GuidCloneable { get; set; }
        }

        [Test]
        public void Clean_should_set_empty_guid()
        {
            var guid = Guid.NewGuid();

            var objectToClear = new ExampleClass3()
            {
                Guid = guid,
                GuidCloneable = guid
            };

            CloneCleaner.Clean(objectToClear);

            Assert.That(objectToClear.Guid, Is.EqualTo(Guid.Empty));
            Assert.That(objectToClear.GuidCloneable, Is.EqualTo(guid));
        }

        internal class ExampleClass4
        {
            [NotCloneable]
            public Guid Guid { get; set; }
            public Guid GuidCloneable { get; set; }

            public ExampleClass ExampleClass { get; set; }
            public ExampleClass2 ExampleClass2 { get; set; }
            public ExampleClass3 ExampleClass3 { get; set; }
        }

        [Test]
        public void Clean_should_clean_complex_classes()
        {
            var guid = Guid.NewGuid();

            var objectToClear1 = new ExampleClass()
            {
                Integer = 5,
                IntegerCloneable = 5,
                Strign = "text",
                StrignCloneable = "text",
                Double = 1.2,
                DoubleCloneable = 1.2
            };

            var objectToClear2 = new ExampleClass2()
            {
                Integer = 5,
                IntegerCloneable = 5
            };

            var objectToClear3 = new ExampleClass3()
            {
                Guid = guid,
                GuidCloneable = guid
            };

            var objectToClear = new ExampleClass4()
            {
                Guid = guid,
                GuidCloneable = guid,

                ExampleClass = objectToClear1,
                ExampleClass2 = objectToClear2,
                ExampleClass3 = objectToClear3

            };

            CloneCleaner.Clean(objectToClear);

            Assert.That(objectToClear.Guid, Is.EqualTo(Guid.Empty));
            Assert.That(objectToClear.GuidCloneable, Is.EqualTo(guid));

            Assert.That(objectToClear.ExampleClass.Integer, Is.EqualTo(0));
            Assert.That(objectToClear.ExampleClass.IntegerCloneable, Is.EqualTo(5));
            Assert.That(objectToClear.ExampleClass.Strign, Is.Null);
            Assert.That(objectToClear.ExampleClass.StrignCloneable, Is.EqualTo("text"));
            Assert.That(objectToClear.ExampleClass.Double, Is.EqualTo(0));
            Assert.That(objectToClear.ExampleClass.DoubleCloneable, Is.EqualTo(1.2));

            Assert.That(objectToClear.ExampleClass2.Integer, Is.Null);
            Assert.That(objectToClear.ExampleClass2.IntegerCloneable, Is.EqualTo(5));

            Assert.That(objectToClear.ExampleClass3.Guid, Is.EqualTo(Guid.Empty));
            Assert.That(objectToClear.ExampleClass3.GuidCloneable, Is.EqualTo(guid));
        }

        internal class ExampleClass5
        {
            [NotCloneable]
            public Guid Guid
            {
                get { return Guid.Empty; }
            }

            [NotCloneable]
            public float Float { get; set; }
            public float FloatCloneable { get; set; }
        }

        [Test]
        public void Clean_should_skip_readonly_getters()
        {
            var objectToClear = new ExampleClass5()
            {
                Float = 1.3f,
                FloatCloneable = 1.3f
            };

            CloneCleaner.Clean(objectToClear);

            Assert.That(objectToClear.Float, Is.EqualTo(0));
            Assert.That(objectToClear.FloatCloneable, Is.EqualTo(1.3f));
        }

        internal class ExampleClass6
        {
            private Guid _guid;

            [NotCloneable]
            public Guid Guid
            {
                set { _guid = value; }
            }

            public Guid ReadGuid()
            {
                return _guid;
            }
        }

        [Test]
        public void Clean_should_work_with_setter_only()
        {
            var guid = Guid.NewGuid();

            var objectToClear = new ExampleClass6()
            {
                Guid = guid
            };

            CloneCleaner.Clean(objectToClear);

            Assert.That(objectToClear.ReadGuid(), Is.EqualTo(Guid.Empty));
        }

        internal class ExampleClass7
        {
            public List<ExampleClass> List { get; set; }
        }

        [Test]
        public void Clean_should_work_enumerable_types()
        {

            var objectToClear = new ExampleClass7();
            objectToClear.List = new List<ExampleClass>();

            for (int i = 0; i < 10; i++)
            {
                var item = new ExampleClass()
                {
                    Integer = 5,
                    IntegerCloneable = 5,
                    Strign = "text",
                    StrignCloneable = "text",
                    Double = 1.2,
                    DoubleCloneable = 1.2
                };

                objectToClear.List.Add(item);
            }


            CloneCleaner.Clean(objectToClear);

            foreach (var item in objectToClear.List)
            {
                Assert.That(item.Integer, Is.EqualTo(0));
                Assert.That(item.IntegerCloneable, Is.EqualTo(5));
                Assert.That(item.Strign, Is.Null);
                Assert.That(item.StrignCloneable, Is.EqualTo("text"));
                Assert.That(item.Double, Is.EqualTo(0));
                Assert.That(item.DoubleCloneable, Is.EqualTo(1.2));
            }
        }

        internal class ExampleClass8
        {
            public Guid Guid { get; set; }

            [NotCloneable]
            public ExampleClass ExampleClass { get; set; }
        }

        [Test]
        public void Clean_should_set_null_for_reference_type()
        {
            var guid = Guid.NewGuid();

            var objectToClear = new ExampleClass8()
            {
                Guid = guid,
                ExampleClass = new ExampleClass()
            };

            CloneCleaner.Clean(objectToClear);

            Assert.That(objectToClear.Guid, Is.EqualTo(guid));
            Assert.That(objectToClear.ExampleClass, Is.Null);
        }

        internal class ExampleClass9
        {
            public ExampleClass[] List { get; set; }
        }

        [Test]
        public void Clean_should_work_array()
        {
            var objectToClear = new ExampleClass9();
            objectToClear.List = new ExampleClass[10];

            for (int i = 0; i < 10; i++)
            {
                var item = new ExampleClass()
                {
                    Integer = 5,
                    IntegerCloneable = 5,
                    Strign = "text",
                    StrignCloneable = "text",
                    Double = 1.2,
                    DoubleCloneable = 1.2
                };

                objectToClear.List[i] = item;
            }

            CloneCleaner.Clean(objectToClear);

            foreach (var item in objectToClear.List)
            {
                Assert.That(item.Integer, Is.EqualTo(0));
                Assert.That(item.IntegerCloneable, Is.EqualTo(5));
                Assert.That(item.Strign, Is.Null);
                Assert.That(item.StrignCloneable, Is.EqualTo("text"));
                Assert.That(item.Double, Is.EqualTo(0));
                Assert.That(item.DoubleCloneable, Is.EqualTo(1.2));
            }
        }

        internal class ExampleClass10
        {
            public Guid Guid { get; set; }

            public object Self
            {
                get { return this; }
            }
        }

        [Test]
        public void Clear_should_skip_recursion()
        {
            var guid = Guid.NewGuid();

            var objectToClear = new ExampleClass10()
            {
                Guid = guid,
            };

            Assert.DoesNotThrow(() => CloneCleaner.Clean(objectToClear));
        }

        internal class ExampleClass11
        {
            private int _setterInvoked;

            [SkipCloneableAttribute]
            public object ToSkip
            {
                get
                {
                    _setterInvoked++;

                    return Guid.NewGuid();
                }
                set { }
            }

            public int GetSetterInvoked()
            {
                return _setterInvoked;
            }
        }

        [Test]
        public void Clear_should_skip_property_marked_with_SkipCloneableAttribute()
        {
            var objectToClear = new ExampleClass11();

            Assert.DoesNotThrow(() => CloneCleaner.Clean(objectToClear));

            Assert.That(objectToClear.GetSetterInvoked(), Is.EqualTo(0));
        }
    }
}
