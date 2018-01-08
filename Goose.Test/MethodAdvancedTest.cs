using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Goose.Test
{
    public class MethodAdvancedTest
    {
        public class A
        {
            public int MCallsInA { get; protected set; }
            public virtual void M() { MCallsInA++; }
        }

        public class B : A
        {
            public new void M() { MCallsInA--; }
        }

        public class C
        {
            public void M_A(A a) { a.M(); }
        }

        public interface IA { }
        public interface IC
        {
            void M_A(IA a);
            void M_A(A a);
        }

        Random _random = new Random();

        [Fact]
        public void PolymorphismTest()
        {
            int count = _random.Next(20, 200);
            B sourceB = new B();
            C sourceC = new C();

            IA ia = sourceB.Goose<IA>();
            IC targetC = sourceC.Goose<IC>(GooseTypePair.Create<A, IA>());

            for (var i = 0; i < count; ++i)
            {
                targetC.M_A(ia);
            }

            Assert.Equal(-count, sourceB.MCallsInA);
        }
        
        [Fact]
        public void MethodNoCopyTest()
        {
            int count = _random.Next(20, 200);

            A sourceA = new A();
            C sourceC = new C();
            IC targetC = sourceC.Goose<IC>(GooseTypePair.Create<A, IA>());
            var ia = sourceA.Goose<IA>();

            for (var i = 0; i < count; ++i)
            {
                targetC.M_A(ia);
            }

            for (var i = 0; i < count; ++i)
            {
                targetC.M_A(sourceA);
            }

            var a = ia.GetSource<A>();
            Assert.Equal(2 * count, a.MCallsInA);
            Assert.Equal(2 * count, sourceA.MCallsInA);
        }
    }
}
