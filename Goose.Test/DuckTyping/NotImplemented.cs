using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Goose.Test.DuckTyping
{
    public class NotImplemented
    {
        public interface IDuck
        {
            string Quack();
            Task<int> WalkAsync();
        }

        class Duck
        {
            internal string Quack() => "Quack";
        }

        static Duck _source = new Duck();
        static IDuck _target = _source.As<IDuck>();

        [Fact]
        public void Not_Implemented_Method_Should_Throw_Exeption()
        {
            Assert.ThrowsAsync<GooseNotImplementedException>(() => _target.WalkAsync());
        }

        [Fact]
        public void Non_Public_Implemented_Method_Should_Throw_Exeption()
        {
            Assert.Throws<GooseNotImplementedException>(() => _target.Quack());
        }

        [Fact]
        public void Get_Source_Reference_Check()
        {
            Assert.Same(_source, _target.GetSource());
            Assert.Same(_source, _target.GetSource<Duck>());
        }
    }
}
