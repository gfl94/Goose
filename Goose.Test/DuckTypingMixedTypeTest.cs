using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Goose.Test
{
    public class DuckTypingMixedTypeTest
    {
        public class A
        {
            public int MACallTimeInA { get; private set; }
            public virtual void MA() { MACallTimeInA++; }
        }

        public class BA : A
        {
            public int MACallTimeInBA { get; private set; }
            public int MBCallTimeInBA { get; private set; }
            public new void MA() { MACallTimeInBA++; }
            public void MB() { MBCallTimeInBA++; }
        }

        public class C
        {
            public void MC() { }
        }

        public interface IGooseA
        {
            void MA();
        }

        public interface IGooseB
        {
            void MB();
        }

        public interface IGooseC
        {
            void MC();
        }

        public class MixedClass : C, IGooseA, IGooseB, IGooseC
        {
            public BA _BA {get; set; }
            public void MA() { _BA.MA(); }
            public void MB() { _BA.MB(); }
        }

        public interface IGooseABC
        {
            void MA();
            void MB();
            void MC();
        }

        Random _random = new Random();

        [Fact]
        public void GooseMixToATest()
        {
            var source = new MixedClass()
            {
                _BA = new BA()
            };
            var target = source.Goose<IGooseA>();
            var count = _random.Next(20, 200);
            for (var i = 0; i < count; ++i) { target.MA(); }
            var mixed = target.GetSource<MixedClass>();
            Assert.Equal(count, mixed._BA.MACallTimeInBA);
        }

        [Fact]
        public void GooseMixToBTest()
        {
            var source = new MixedClass()
            {
                _BA = new BA()
            };
            var target = source.Goose<IGooseB>();
            var count = _random.Next(20, 200);
            for (var i = 0; i < count; ++i) { target.MB(); }
            var mixed = target.GetSource<MixedClass>();
            Assert.Equal(count, mixed._BA.MBCallTimeInBA);
        }

        [Fact]
        public void GooseMixToCTest()
        {
            var source = new MixedClass()
            {
                _BA = new BA()
            };
            var target = source.Goose<IGooseC>();
            target.MC();
        }

        [Fact]
        public void GooseMixToABCTest()
        {
            var source = new MixedClass()
            {
                _BA = new BA()
            };
            var target = source.Goose<IGooseABC>();
            var count1 = _random.Next(20, 200);
            var count2 = _random.Next(20, 200);
            for (var i = 0; i < count1; ++i) { target.MB(); }
            for (var i = 0; i < count2; ++i) { target.MA(); }
            var mixed = target.GetSource<MixedClass>();
            Assert.Equal(count1, mixed._BA.MBCallTimeInBA);
            Assert.Equal(count2, mixed._BA.MACallTimeInBA);
        }
    }
}
