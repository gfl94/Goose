using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Goose.Test
{
    public class MethodBasicTest
    {
        public class A
        {
            public int CA { get; set; }
            public void MA() { }
        }

        public class B
        {
            public int CB { get; set; }
            public void MB() { }
        }

        public class C
        {
            public int CC { get; set; }
            public void MC_A(A a)
            {
                CC += a.CA;
                a.CA *= 2;
            }
            public void MC_AB(A a, B b)
            {
                CC = a.CA * b.CB;
                a.CA += CC;
                b.CB += CC;
            }
            public A MGetA()
            {
                CC /= 2;
                return new A { CA = CC };
            }
            public B MGetBWithA(A a)
            {
                return new B { CB = a.CA };
            }
        }

        public interface IA
        {
            void MA();
        }

        public interface IB
        {
            void MB();
        }

        public interface IC_A
        {
            void MC_A(IA a);
        }

        public interface IC_AB
        {
            void MC_AB(IA a, IB b);
        }

        public interface IAGenerator
        {
            IA MGetA();
        }

        public interface IBGenWithA
        {
            IB MGetBWithA(IA a);
        }

        Random _random = new Random();

        [Fact]
        public void GooseInproperConstruction()
        {
            var originalCA = _random.Next(10, 200);
            var a = new A() { CA = originalCA };
            var c = new C();
            var ia = a.Goose<IA>();
            var ic = c.Goose<IC_A>(GooseTypePair.Create<IA, A>()); // with the wrong order
            Assert.Throws<GooseNotImplementedException>(() => ic.MC_A(ia)); 
        }

        [Fact]
        public void GooseAsSingleParameter()
        {
            var originalCA = _random.Next(10, 200); ;
            var a = new A() { CA = originalCA };
            var c = new C();

            var ia = a.Goose<IA>();

            var _ic = c.Goose<IC_A>();
            Assert.Throws<GooseNotImplementedException>(() => _ic.MC_A(ia));

            var ic = c.Goose<IC_A>(GooseTypePair.Create<A, IA>());
            
            ic.MC_A(ia);

            var aa = ia.GetSource<A>();
            var cc = ic.GetSource<C>();

            Assert.Equal(originalCA * 2, aa.CA);
            Assert.Equal(originalCA, cc.CC);
        }

        [Fact]
        public void GooseAsMulitpleParaThrows()
        {
            var originalCA = _random.Next(10, 200);
            var originalCB = _random.Next(10, 200);
            var a = new A() { CA = originalCA };
            var b = new B() { CB = originalCB };
            var c = new C();

            var ia = a.Goose<IA>();
            var ib = b.Goose<IB>();

            var _ic1 = c.Goose<IC_AB>();
            var _ic2 = c.Goose<IC_AB>(GooseTypePair.Create<B, IB>());
            var _ic3 = c.Goose<IC_AB>(GooseTypePair.Create<A, IA>());

            Assert.Throws<GooseNotImplementedException>(() => _ic1.MC_AB(ia, ib));
            Assert.Throws<GooseNotImplementedException>(() => _ic2.MC_AB(ia, ib));
            Assert.Throws<GooseNotImplementedException>(() => _ic3.MC_AB(ia, ib));
        }

        [Fact]
        public void GooseAsMulitpleParameter()
        {
            var originalCA = _random.Next(10, 200);
            var originalCB = _random.Next(10, 200);
            var a = new A() { CA = originalCA };
            var b = new B() { CB = originalCB };
            var c = new C();

            var ia = a.Goose<IA>();
            var ib = b.Goose<IB>();

            var ic = c.Goose<IC_AB>(GooseTypePair.Create<A, IA>(), GooseTypePair.Create<B, IB>());

            ic.MC_AB(ia, ib);

            var aa = ia.GetSource<A>();
            var bb = ib.GetSource<B>();
            var cc = ic.GetSource<C>();

            // Expected answer
            var a0 = new A() { CA = originalCA };
            var b0 = new B() { CB = originalCB };
            var c0 = new C();
            c0.MC_AB(a0, b0);
            // Expected answer end

            Assert.Equal(a0.CA, aa.CA);
            Assert.Equal(b0.CB, bb.CB);
            Assert.Equal(c0.CC, cc.CC);
        }
        
        [Fact]
        public void GooseAsReturnType()
        {
            var originalCC = _random.Next(10, 200);
            var c = new C() { CC = originalCC };
            
            var ic = c.Goose<IAGenerator>(GooseTypePair.Create<A, IA>());

            IA ia = ic.MGetA();

            var aa = ia.GetSource<A>();
            var cc = ic.GetSource<C>();

            // expected answer 
            var c0 = new C() { CC = originalCC };
            var a0 = c0.MGetA();

            Assert.Equal(a0.CA, aa.CA);
            Assert.Equal(c0.CC, cc.CC);
        }

        [Fact]
        public void GooseAsParamAndReturnType()
        {
            var originalCA = _random.Next(10, 200);
            var originalCC = _random.Next(10, 200);

            var a = new A { CA = originalCA };
            var c = new C { CC = originalCC };

            var ia = a.Goose<IA>();
            var ic = c.Goose<IBGenWithA>(GooseTypePair.Create<A, IA>(), GooseTypePair.Create<B, IB>());

            IB ib = ic.MGetBWithA(ia);
            var aa = ia.GetSource<A>();
            var bb = ib.GetSource<B>();
            var cc = ic.GetSource<C>();

            // expected answer
            var a0 = new A { CA = originalCA };
            var c0 = new C { CC = originalCC };
            var b0 = c0.MGetBWithA(a0);

            Assert.Equal(a0.CA, aa.CA);
            Assert.Equal(b0.CB, bb.CB);
            Assert.Equal(c0.CC, cc.CC);
        }
    }
}
