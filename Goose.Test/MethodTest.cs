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
        public int TotalCalories { get; set; }
        public void Eat(Food food) { TotalCalories += food.Calories; }
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
            var food = new Food()
            {
                Calories = 100
            };
            var source = new Person();

            var ifood = food.Goose<IFood>();
            var iperson = source.Goose<IPerson>(GooseTypePair.Create<Food, IFood>());
            iperson.Eat(ifood);

            var person = iperson.GetSource<Person>();
            Assert.Equal(food.Calories, person.TotalCalories);
        }
    }
}
