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

        public interface IPoison { }

        class Poison {}

        public interface IPerson
        {
            void Eat(IFood food);
            void Drink(IPoison poison);
        }

        class Person
        {
            public void Eat(Food food) => throw new FoodExpiredException(food);
            public void Drink(Poison poison) => throw new Exception("I can not drink this");
        }

        interface IException { }

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

        [Fact(Skip = "System.Exception class not works well")]
        public void Interchangable_For_System_Exception_class_Registered()
        {
            Person person = new Person();
            Poison poison = new Poison();
            IPoison poisonTarget = poison.As<IPoison>();
            IPerson personTarget = person.As<IPerson>(
                GooseTypePair.Create<Poison, IPoison>(),
                GooseTypePair.Create<Exception, IException>());

            var ex = Assert.Throws<WrappedException<IException>>(() => personTarget.Drink(poisonTarget));
            Assert.NotNull(ex.Exception);
            Assert.Equal(typeof(Exception), ex.Exception.GetSource().GetType());
        }
    }
}
