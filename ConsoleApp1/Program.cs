using System;
using Goose;

namespace ConsoleApp1
{
    public interface IToString
    {
        void NotExist<U>(out Program p1, ref Program p2);
        string ToString<T>();
    }

    public class Program
    {
        static void Main(string[] args)
        {
            IToString x = 1.Goose<IToString>();
            x.ToString();
            Program p1, p2 = new Program();
            x.NotExist<byte>(out p1, ref p2);

            Console.Read();
        }
    }
}
