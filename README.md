
Goose
=====
An unconstrained duck typing library.

[Duck Typing](https://en.wikipedia.org/wiki/Duck_typing)
-----------
    class Duck
    {
	    public void Quack() { }
    }

    interface IDuck
    {
        void Quack();
    }
	
	var duck = new Duck();
	var duckTyped = duck.Goose<IDuck>();
	duckTyped.Quack();      //calls Duck.Quack()

Goose Typing
------------
More than duck typing, the interface can have more methods than the class.
	

	class Duck
    {
	    public void Quack() { }
    }
    
    interface INamedDuck
    {
	    string Name { get; }
        void Quack();
    }
	
	var duck = new Duck();
	var duckTyped = duck.Goose<INamedDuck>();
	duckTyped.Quack();          //calls Duck.Quack()
	var name = duckTyped.Name;  //throws NotImplementedException

Goose Typing Methods
--------------------
More than simple goose typing, you can make classes and interfaces compatible in method calls.
	

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

    var food = new Food();
    var person = new Person();
    var ifood = food.Goose<IFood>();
    var iperson = person.Goose<IPerson>(GooseTypePair.Create<Food, IFood>());
    iperson.Eat(ifood);