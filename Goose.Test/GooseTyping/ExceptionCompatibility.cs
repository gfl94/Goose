using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Goose.Test.GooseTyping
{
    public class ExceptionCompatibility
    {
        public interface IFood
        {
            int FoodCalories { get; }
        }

        class Food
        {
            public int FoodCalories { get; set; }
        }

        public interface IPerson
        {
            void Eat(IFood food);
        }

        class Person
        {
            public void Eat(Food food) => throw new FoodExpiredException(food);
        }

        public interface IFoodExpiredException
        {
            IFood ExpiredFood { get; }
        }

        class FoodExpiredException : Exception
        {
            public Food ExpiredFood { get; }

            public FoodExpiredException(Food food)
            {
                this.ExpiredFood = food;
            }
        }

        [Fact]
        public void Not_Wrapped_If_Exception_Not_Registered()
        {
            Person person = new Person();
            Food food = new Food { FoodCalories = 100 };

            IFood foodTarget = food.As<IFood>();
            IPerson personTarget = person.As<IPerson>(GooseTypePair.Create<Food, IFood>());

            Assert.Throws<FoodExpiredException>(() => personTarget.Eat(foodTarget));
        }

        [Fact]
        public void Interchangable_If_Exception_Registered()
        {
            Person person = new Person();
            Food food = new Food { FoodCalories = 100 };

            IFood foodTarget = food.As<IFood>();
            IPerson personTarget = person.As<IPerson>(
                GooseTypePair.Create<Food, IFood>(),
                GooseTypePair.Create<FoodExpiredException, IFoodExpiredException>());

            var ex = Assert.Throws<WrappedException<IFoodExpiredException>>(() => personTarget.Eat(foodTarget));
            Assert.NotNull(ex.Exception);
            Assert.Same(food, ex.Exception.ExpiredFood.GetSource());
            Assert.Equal(typeof(FoodExpiredException), ex.Exception.GetSource().GetType());            
        }
    }
}
