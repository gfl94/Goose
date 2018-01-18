using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Goose.Test.DuckTyping
{
    public class ObjectMethods
    {
        public interface IDuck
        {
            string Quack();
        }

        public interface IDuckFull
        {
            string Quack();
            int GetHashCode();
            bool Equals(object o);
            string ToString();
        }

        class Duck
        {
            public string Quack() => "Quack";

            public override int GetHashCode()
            {
                return typeof(Duck).GetHashCode();
            }
        }

        [Fact]
        public void Object_Methods_Should_Not_Invoke_Source_If_Not_Defined_In_Interface()
        {
            Duck source = new Duck();
            IDuck target = source.As<IDuck>();
            Assert.NotEqual(source.GetHashCode(), target.GetHashCode());
            Assert.NotEqual(source.ToString(), target.ToString());
            Assert.NotEqual(source.Equals(source), target.Equals(source));
        }

        [Fact]
        public void Object_Methods_Should_Not_Invoke_Source_If_Defined_In_Interface()
        {
            Duck source = new Duck();
            IDuckFull target = source.As<IDuckFull>();
            Assert.Equal(source.GetHashCode(), target.GetHashCode());
            Assert.Equal(source.ToString(), target.ToString());
            Assert.Equal(source.Equals(source), target.Equals(source));
        }
    }
}
