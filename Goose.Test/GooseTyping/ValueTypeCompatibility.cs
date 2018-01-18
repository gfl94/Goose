using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Goose.Test.GooseTyping
{
    public class ValueTypeCompatibility
    {
        public enum Choice
        {
            A = 0,
            B = 1,
            C = 2
        }

        public enum MyChoice
        {
            B = 1,
            D = 3
        }

        public class Person
        {
            public Choice MakeChoice(Choice c)
            {
                return c;
            }
        }

        public interface IMe
        {
            MyChoice MakeChoice(MyChoice c);
        }

        public interface IMe2
        {
            MyChoice MakeChoice(int c);
        }

        [Fact]
        public void Enum_Should_Be_Compatible_If_Registered()
        {
            Person person = new Person();
            IMe me = person.As<IMe>(GooseTypePair.Create<MyChoice, Choice>(), GooseTypePair.Create<Choice, MyChoice>());
            Assert.True((int)person.MakeChoice(Choice.B) == (int)me.MakeChoice(MyChoice.B));
        }

        [Fact]
        public void Enum_Should_Not_Compatible_If_Not_Registered()
        {
            Person person = new Person();
            IMe me = person.As<IMe>(GooseTypePair.Create<MyChoice, Choice>());
            Assert.Throws<GooseNotImplementedException>(() => me.MakeChoice(MyChoice.B));
        }

        [Fact]
        public void Enum_Should_Throw_InvalidCastException_If_Not_Defined()
        {
            Person person = new Person();
            IMe me = person.As<IMe>(GooseTypePair.Create<MyChoice, Choice>(), GooseTypePair.Create<Choice, MyChoice>());
            Assert.Throws<InvalidCastException>(() => me.MakeChoice(MyChoice.D));
        }

        [Fact]
        public void Enum_Should_Be_Compatible_With_Underlying_Type_If_Registered()
        {
            Person person = new Person();
            IMe2 me = person.As<IMe2>(GooseTypePair.Create<int, Choice>(), GooseTypePair.Create<Choice, MyChoice>());
            Assert.Throws<InvalidCastException>(() => me.MakeChoice(100));
            Assert.Equal(1, (int)me.MakeChoice(1));
        }

        public class Calculator
        {
            public long Add(long a, long b) => a + b;
        }

        public interface ICalculator
        {
            int Add(int a, int b);
        }

        [Fact]
        public void Primitive_Types_Compatible_If_Registered()
        {
            Calculator source = new Calculator();
            ICalculator target = source.As<ICalculator>(GooseTypePair.Create<int, long>(), GooseTypePair.Create<long, int>());

            int a = DateTime.Now.Millisecond;
            int b = DateTime.Now.Second;
            Assert.Equal(source.Add(a, b), target.Add(a, b));
        }

    }
}
