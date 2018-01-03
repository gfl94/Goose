using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Goose.Test
{
    class Duck
    {
        public int QuackCount { get; private set; }

        public void Quack()
        {
            this.QuackCount++;
        }
    }

    interface IDuck
    {
        void Quack();
    }

    interface INamedDuck
    {
        void Quack();
        string Name { get; }
    }

    public class DuckTypingTest
    {
        Random _random = new Random();

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
    }
}
