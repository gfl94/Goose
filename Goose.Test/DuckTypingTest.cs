using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Goose.Test
{
    class Duck
    {
        public int QuackCount { get; protected set; }

        public int SwimDistance { get; protected set; }

        public void Quack()
        {
            this.QuackCount++;
        }

        public virtual void Swim() { SwimDistance += 1; }
    }
    class NamedDuck: Duck
    {
        public string Name { get; set; }
    }

    interface IDuck
    {
        void Quack();
        void Swim();
    }

    interface INamedDuck
    {
        void Quack();
        string Name { get; }
    }

    interface ISwimmable
    {
        void Swim();
    }

    interface IQuackable
    {
        void Quack();
    }

    class SlientDuck : Duck
    {
        public new void Quack() { }
    }

    class QuickSwimDuck: Duck
    {
        public override void Swim() { SwimDistance += 10; }
    }

    public class DuckTypingTest
    {
        Random _random = new Random();

        [Fact]
        public void Goose_To_Class_Test()
        {
            Duck source = new Duck();
            // interface needed here
            Assert.Throws<ArgumentException>(() => source.Goose<Duck>());
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

        [Fact]
        public void Overwrite_Method_Test()
        {
            SlientDuck source = new SlientDuck();
            IDuck target = source.Goose<IDuck>();

            Assert.Throws<GooseAmbiguousMatchException>(() => target.Quack());

            SlientDuck duck = target.GetSource<SlientDuck>();
            Assert.NotNull(duck);
            Assert.Equal(0, duck.QuackCount);
        }

        [Fact]
        public void Override_Method_Test()
        {
            var count1 = _random.Next(1, 100);
            var count2 = _random.Next(1, 100);

            QuickSwimDuck source = new QuickSwimDuck();
            IDuck target = source.Goose<IDuck>();

            for (var i = 0; i < count1; i++)
            {
                target.Swim();
            }

            for (var i = 0; i < count2; ++i)
            {
                target.Quack();
            }

            QuickSwimDuck qsd = target.GetSource<QuickSwimDuck>();

            //expected answer
            QuickSwimDuck expectedQuickSwimDuck = new QuickSwimDuck();

            for (var i = 0; i < count1; i++)
            {
                expectedQuickSwimDuck.Swim();
            }

            for (var i = 0; i < count2; ++i)
            {
                expectedQuickSwimDuck.Quack();
            }
            // expected answer end

            Assert.Equal(expectedQuickSwimDuck.QuackCount, qsd.QuackCount);
            Assert.Equal(expectedQuickSwimDuck.SwimDistance, qsd.SwimDistance);
        }

        [Fact]
        public void Object_No_Copy_Test()
        {
            var count1 = _random.Next(1, 100);
            var count2 = _random.Next(1, 100);

            QuickSwimDuck source = new QuickSwimDuck();

            ISwimmable swimmable = source.Goose<ISwimmable>();
            for (var i = 0; i < count1; i++)
            {
                swimmable.Swim();
            }

            IQuackable quackable = source.Goose<IQuackable>();
            for (var i = 0; i < count2; ++i)
            {
                quackable.Quack();
            }

            QuickSwimDuck qsd1 = swimmable.GetSource<QuickSwimDuck>();
            QuickSwimDuck qsd2 = quackable.GetSource<QuickSwimDuck>();

            //expected answer
            QuickSwimDuck expectedQuickSwimDuck = new QuickSwimDuck();

            for (var i = 0; i < count1; i++)
            {
                expectedQuickSwimDuck.Swim();
            }

            for (var i = 0; i < count2; ++i)
            {
                expectedQuickSwimDuck.Quack();
            }
            // expected answer end

            Assert.Equal(expectedQuickSwimDuck.QuackCount, qsd1.QuackCount);
            Assert.Equal(expectedQuickSwimDuck.SwimDistance, qsd1.SwimDistance);
            Assert.Equal(expectedQuickSwimDuck.QuackCount, qsd2.QuackCount);
            Assert.Equal(expectedQuickSwimDuck.SwimDistance, qsd2.SwimDistance);
        }

        [Fact]
        public void Two_Step_Goose_Test()
        {
            var count1 = _random.Next(1, 100);
            var count2 = _random.Next(1, 100);

            QuickSwimDuck source = new QuickSwimDuck();

            ISwimmable swimmable = source.Goose<ISwimmable>();
            swimmable.Swim();
            IQuackable quackable = swimmable.Goose<IQuackable>();

            Assert.Throws<GooseNotImplementedException>(() => quackable.Quack()); // hide method by default
        }
    }
}

