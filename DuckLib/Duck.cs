using System;

namespace DuckLib
{
    public class Duck
    {
        void Eat(Fish fish) { }
    }

    public interface IDuck { }

    public class Fish { }

    public interface IStandardFish { }
}
