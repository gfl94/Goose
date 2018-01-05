using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Goose.Test
{
    class Duck
    {
        public int QuackCount { get; protected set; }

        public void Quack()
        {
            this.QuackCount++;
        }
    }
    class NamedDuck: Duck
    {
        public string Name { get; set; }
    }

    interface IDuck
    {
        void Quack();
    }

    interface IBigDuck: IDuck
    {
        void Bark();
    }

    class BigDuck: Duck, IBigDuck
    {
        public new void Quack() { }
        public void Bark() { QuackCount -= 10; }
    }

    interface INamedDuck
    {
        void Quack();
        string Name { get; }
    }

    public class DuckTypingBasicTest
    {
        Random _random = new Random();

        [Fact]
        public void BigDuck_Test()
        {
            var count = _random.Next(1, 100);

            var source = new Duck();
            var target = source.Goose<IBigDuck>();

            for (var i = 0; i < count; i++)
            {
                target.Quack();
            }
            Assert.Throws<GooseNotImplementedException>(() => target.Bark());

            var duck = target.GetSource<Duck>();
            Assert.NotNull(duck);
            Assert.Equal(count, duck.QuackCount);
        }

        [Fact]
        public void BigDuck_Test2()
        {

            var source = new BigDuck();
            var target = source.Goose<IBigDuck>();

            Assert.Throws<GooseAmbiguousMatchException>(() => target.Quack());
            target.Bark();

            var duck = target.GetSource<BigDuck>();
            Assert.NotNull(duck);
            Assert.Equal(-10, duck.QuackCount);
        }

        [Fact]
        public void Exactly_Match()
        {
            var count = _random.Next(1, 100);

            var source = new Duck();
            var target = source.Goose<IDuck>();

            for (var i = 0; i < count; i++)
            {
                target.Quack();
            }

            var duck = target.GetSource<Duck>();
            Assert.NotNull(duck);
            Assert.Equal(count, duck.QuackCount);
        }

        [Fact]
        public void Not_Implemented_Methods()
        {
            var source = new Duck();
            var target = source.Goose<INamedDuck>();
            target.Quack();
            Assert.Throws<GooseNotImplementedException>(() => target.Name);

            var duck = target.GetSource<Duck>();
            Assert.NotNull(duck);
            Assert.Equal(1, duck.QuackCount);
        }

        [Fact] 
        public void Unrelated_Class_Func_Test()
        {
            var source = 1;
            var target = source.Goose<INamedDuck>();
            Assert.Throws<GooseNotImplementedException>(() => target.Quack());
            Assert.Throws<GooseNotImplementedException>(() => target.Name);

            var source32 = target.GetSource<Int32>();
            Assert.Equal(1, source32);
        }
    }
}

