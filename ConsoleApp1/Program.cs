using System;
using System.Collections.Generic;
using Goose;

namespace ConsoleApp1
{
    class Client
    {
        public User GetUser()
        {
            return new User() { Name = "danny" };
        }

        public User ChangeName(User user)
        {
            user.Name += "_changed";
            return user;
        }
    }

    class User
    {
        public string Name { get; set; }
    }
    
    public interface IUser
    {
        string Name { get; }
        int Age { get; }
    }

    public interface IClient
    {
        IUser GetUser();

        IUser ChangeName(IUser user);
    }

    public class Program
    {
        static void Main(string[] args)
        {
            var client = new Client().Goose<IClient>(GooseTypePair.Create<User, IUser>());
            var user = client.GetUser();
            Console.WriteLine(user.Name);

            user = client.ChangeName(user);
            Console.WriteLine(user.Name);

            Console.Read();
        }
    }
}
