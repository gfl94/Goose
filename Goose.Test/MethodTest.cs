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

    class Vegetable
    {
        public int Calories { get; set; }
    }

    class Person
    {
        public int TotalCalories { get; set; }
        public void Eat(Food food) { TotalCalories += food.Calories; }
        public void Eat(Food food, Vegetable veg) { TotalCalories += (food.Calories + veg.Calories); }
        public Food CookFoodWithVegetable(Vegetable veg) { return new Food { Calories = veg.Calories * 2 }; }
        public Vegetable PlantVege() { return new Vegetable() { Calories = TotalCalories }; }

        public virtual void Walk() { TotalCalories -= 100; }
    }

    class FatPerson : Person
    {
        public new void Eat(Food food) { TotalCalories += 2 * food.Calories; }
        public override void Walk() { TotalCalories -= 50; }
    }

    interface IPerson
    {
        void Eat(IFood food);
        void Walk();
    }

    interface IFood
    {
        int Calories { get; }
    }

    interface IEatFoodVege
    {
        void Eat(IFood food, IVegetable veg);
    }

    interface IVegetable
    {
        int Calories { get; }
    }

    interface ICooker
    {
        IFood CookFoodWithVegetable(IVegetable veg);
    }

    interface IFarmer
    {
        IVegetable PlantVege();
    }

    public class MethodTest
    {
        Random _random = new Random();

        [Fact]
        public void Goose_Single_Parameter_Test()
        {
            int food_cal = _random.Next(100, 200);
            var food = new Food() { Calories = food_cal };
            var source = new Person();

            var ifood = food.As<IFood>();
            var iperson = source.As<IPerson>(GooseTypePair.Create<Food, IFood>());
            iperson.Eat(ifood);

            var person = iperson.GetSource<Person>();
            Assert.Equal(food.Calories, person.TotalCalories);
        }

        [Fact]
        public void Goose_Inproper_Construction()
        {
            int food_cal = _random.Next(100, 200);
            var food = new Food() { Calories = food_cal };
            var source = new Person();

            var ifood = food.As<IFood>();
            Assert.Throws<ArgumentException>(() =>
            {
                var iperson = source.As<IPerson>(GooseTypePair.Create<IFood, Food>());
                iperson.Eat(ifood);
            });
        }

        [Fact]
        public void Goose_Multiple_Parameter_Throw_Test()
        {
            int food_cal = _random.Next(100, 200);
            int vege_cal = _random.Next(20, 50);
            Food food = new Food() { Calories = food_cal };
            Vegetable vege = new Vegetable() { Calories = vege_cal };
            Person source = new Person();

            IFood ifood = food.As<IFood>();
            IVegetable ivege = vege.As<IVegetable>();


            IEatFoodVege ip1 = source.As<IEatFoodVege>();
            IEatFoodVege ip2 = source.As<IEatFoodVege>(
                    GooseTypePair.Create<Food, IFood>());
            IEatFoodVege ip3 = source.As<IEatFoodVege>(
                    GooseTypePair.Create<Vegetable, IVegetable>());

            Assert.Throws<GooseNotImplementedException>(() => ip1.Eat(ifood, ivege));
            Assert.Throws<GooseNotImplementedException>(() => ip2.Eat(ifood, ivege));
            Assert.Throws<GooseNotImplementedException>(() => ip3.Eat(ifood, ivege));
        }

        [Fact]
        public void Goose_Multiple_Parameter_Test()
        {
            int food_cal = _random.Next(100, 200);
            int vege_cal = _random.Next(20, 50);
            Food food = new Food() { Calories = food_cal };
            Vegetable vege = new Vegetable() { Calories = vege_cal };
            Person source = new Person();

            IFood ifood = food.As<IFood>();
            IVegetable ivege = vege.As<IVegetable>();
            IEatFoodVege ip = source.As<IEatFoodVege>(
                    GooseTypePair.Create<Food, IFood>(),
                    GooseTypePair.Create<Vegetable, IVegetable>());
            ip.Eat(ifood, ivege);

            Person person = ip.GetSource<Person>();

            Person p0 = new Person();
            p0.Eat(food, vege);

            Assert.Equal(p0.TotalCalories, person.TotalCalories);
        }

        [Fact]
        public void Goose_Return_Type_Test()
        {
            int food_cal = _random.Next(200, 500);
            var source = new Person() { TotalCalories = food_cal };

            var ifarmer = source.As<IFarmer>(GooseTypePair.Create<Vegetable, IVegetable>());
            var ivege = ifarmer.PlantVege();

            Vegetable vege = ivege.GetSource<Vegetable>();
            Assert.Equal(
                new Person() { TotalCalories = food_cal }.PlantVege().Calories,
                vege.Calories);
        }

        [Fact]
        public void Goose_Return_Type_And_Parameter_Test()
        {
            int vege_cal = _random.Next(100, 200);
            var vege = new Vegetable() { Calories = vege_cal };
            var source = new Person();

            var ivege = vege.As<IVegetable>();
            var icooker = source.As<ICooker>(GooseTypePair.Create<Vegetable, IVegetable>(),
                GooseTypePair.Create<Food, IFood>());
            var ifood = icooker.CookFoodWithVegetable(ivege);

            var food = ifood.GetSource<Food>();

            var expected_cal = new Person().CookFoodWithVegetable(
                    new Vegetable() { Calories = vege_cal }).Calories;

            Assert.Equal(expected_cal, food.Calories);
        }

        [Fact]
        public void Polymorphism_Test()
        {
            int food_cal = _random.Next(100, 200);
            Food food = new Food() { Calories = food_cal };

            Person source = new FatPerson();

            IFood ifood = food.As<IFood>();
            IPerson target = source.As<IPerson>(GooseTypePair.Create<Food, IFood>());

            Assert.Throws<GooseAmbiguousMatchException>(() => target.Eat(ifood));
            target.Walk();

            Person pp = target.GetSource<Person>();

            // expected answer
            FatPerson fat = new FatPerson();
            fat.Walk();

            Assert.Equal(fat.TotalCalories, pp.TotalCalories);
        }
    }
}
