using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Goose.Test.GooseTyping
{
    public class MethodSignatureCompatibility
    {
        public interface IFood
        {
            int FoodCalories { get; }
        }

        class Food
        {
            public int FoodCalories { get; set; }
        }

        public interface IRemain
        {
            int RemainCalories { get; }
        }

        class Remain
        {
            public int RemainCalories { get; set; }
        }

        public interface IPerson
        {
            int CaloriesTaken { get; }
            IRemain Eat(IFood food, int percentage, out bool enough);
        }

        class Person
        {
            public int CaloriesTaken { get; private set; }
            
            public Remain Eat(Food food, int percentage, out bool enough)
            {
                var random = new Random();
                var calories = food.FoodCalories * percentage / 100;
                this.CaloriesTaken += calories;
                enough = false;
                return new Remain { RemainCalories = food.FoodCalories - calories };
            }
        }

        [Fact]
        public void Not_Implemented_If_Not_Registered()
        {
            Person person = new Person();
            Food food = new Food { FoodCalories = 100 };

            IFood foodTarget = food.As<IFood>();
            IPerson personTarget = person.As<IPerson>();
            Assert.Throws<GooseNotImplementedException>(() => personTarget.Eat(foodTarget, 90, out var enough));

            // <Remain, IRemain> is not registered
            IPerson personTarget2 = person.As<IPerson>(GooseTypePair.Create<Food, IFood>());
            Assert.Throws<GooseNotImplementedException>(() => personTarget.Eat(foodTarget, 90, out var enough));
        }

        [Fact]
        public void Interchangable_If_Registered()
        {
            Person person = new Person();
            Food food = new Food { FoodCalories = 100 };

            IFood foodTarget = food.As<IFood>();
            IPerson personTarget = person.As<IPerson>(
                GooseTypePair.Create<Food, IFood>(),
                GooseTypePair.Create<Remain, IRemain>());

            IRemain remain = personTarget.Eat(foodTarget, 90, out var enough);
            Assert.Equal(food.FoodCalories, person.CaloriesTaken + remain.RemainCalories);
        }
    }
}
