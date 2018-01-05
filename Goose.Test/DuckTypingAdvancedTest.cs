using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Goose.Test
{

    public class DuckTypingAdvancedTest
    {

        public class A
        {
            public int MACallTimeInA { get; private set; }
            public virtual void MA() { MACallTimeInA++; }
        }

        public class BA : A
        {
            public int MACallTimeInBA { get; private set; }
            public new void MA() { MACallTimeInBA++; }
            public void MB() { }
        }

        public interface IGooseA
        {
            void MA();
        }

        public interface IGooseBA
        {
            void MA();
            void MB();
        }

        public interface IFeatureX
        {
            void MBuild();
            void MX();
        }

        public class C : IFeatureX
        {
            public int Count { get; private set; }
            public void MX() { }
            public void MBuild() { Count += 1000; }
            public void MOwnMethodC() { Count += 1; }
        }

        public interface IGooseC
        {
            void MOwnMethodC();
        }

        public interface IGooseBuild
        {
            void MBuild();
        }

        Random _random = new Random();

        [Fact]
        public void GooseToClassTest()
        {
            A source = new A();
            Assert.Throws<ArgumentException>(() => source.Goose<BA>());
        }

        [Fact]
        public void GooseExactMatchTest()
        {
            A source = new A();
            var target = source.Goose<IGooseA>();
            var count = _random.Next(20, 200);
            for (var i = 0; i < count; ++i) { target.MA(); }
            var A1 = target.GetSource<A>();
            Assert.Equal(count, A1.MACallTimeInA);
        }

        [Fact]
        public void GooseNormalInhertTest()
        {
            A source = new A();
            var target = source.Goose<IGooseBA>();
            Assert.Throws<GooseNotImplementedException>(() => target.MB());
            var count = _random.Next(20, 200);
            for (var i = 0; i < count; ++i) { target.MA(); }
            var A1 = target.GetSource<A>();
            Assert.Equal(count, A1.MACallTimeInA);
        }
        
        [Fact]
        public void GooseRevertInhertTest()
        {
            BA source = new BA();
            var target = source.Goose<IGooseA>();

            Assert.Throws<GooseAmbiguousMatchException>(() => target.MA());
            var ba = target.GetSource<BA>();
            ba.MA();
            ba.MB();
        }

        [Fact]
        public void GoosePolymorphismTest()
        {
            A source = new BA();
            var target = source.Goose<IGooseBA>();
            Assert.Throws<GooseAmbiguousMatchException>(() => target.MA());
            target.MB();

            var a = target.GetSource<A>();
            a.MA();
        }

        [Fact]
        public void GooseNoCopyTest()
        {
            var _count1 = _random.Next(20, 200);
            var _count2 = _random.Next(20, 200);

            var source = new C();
            var gooseC = source.Goose<IGooseC>();
            for (var i = 0; i < _count1; ++i)
            {
                gooseC.MOwnMethodC();
            }

            var gooseBuild = source.Goose<IGooseBuild>();
            for (var i = 0; i < _count2; ++i)
            {
                gooseBuild.MBuild();
            }

            //expected answer
            var c0 = new C();
            for (var i = 0; i < _count1; ++i)
            {
                c0.MOwnMethodC();
            }
            for (var i = 0; i < _count2; ++i)
            {
                c0.MBuild();
            }

            var c = gooseC.GetSource<C>();
            Assert.Equal(c0.Count, c.Count);

            var cc = gooseBuild.GetSource<C>();
            Assert.Equal(c0.Count, c.Count);
        }

        [Fact]
        public void GooseTwoStepTest()
        {
            var source = new C();
            var gooseC = source.Goose<IGooseC>();
            gooseC.MOwnMethodC();
            var c = gooseC.GetSource<C>();
            c.MOwnMethodC();
            c.MBuild();
            var gooseBuild = gooseC.Goose<IGooseBuild>();
            Assert.Throws<GooseNotImplementedException>(() => gooseBuild.MBuild());
            var cc = gooseBuild.GetSource<IGooseC>().GetSource<C>();
            cc.MOwnMethodC();
            cc.MBuild();
        }
    }
}
