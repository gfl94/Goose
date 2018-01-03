using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Goose.Test
{
    class Food
    {
        public int Calories { get; set; }
    }

    interface IFood
    {
        int Calories { get; }
    }

    class Person
    {
        public void Eat(Food food) { }
    }

    interface IPerson
    {
        void Eat(IFood food);
        void Walk();
    }

    public class MethodTest
    {
        [Fact]
        public void M()
        {
            var food = new Food();
            var person = new Person();

            var ifood = food.Goose<IFood>();
            var iperson = person.Goose<IPerson>(GooseTypePair.Create<Food, IFood>());
            iperson.Eat(ifood);
        }
    }
}
