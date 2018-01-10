using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Goose.Test.DuckTyping
{
    public class PartiallyImplemented
    {
        public interface IDuck
        {
            string Quack();
            Task<int> WalkAsync();
            Task<string> SwimAsync();
        }

        class Duck
        {
            public Task<int> WalkAsync() => Task.FromResult(DateTime.Now.Second);
            private Task<string> SwimAsync() => Task.FromResult("private");
        }

        static Duck _source = new Duck();
        static IDuck _target = _source.As<IDuck>();

        [Fact]
        public void Not_Implemented_Method_Should_Throw_Exeption()
        {
            Assert.Throws<GooseNotImplementedException>(() => _target.Quack());
        }

        [Fact]
        public void Non_Public_Implemented_Method_Should_Throw_Exeption()
        {
            Assert.ThrowsAsync<GooseNotImplementedException>(() => _target.SwimAsync());
        }

        [Fact]
        public async Task Implemented_Method_Should_Work()
        {
            Assert.Equal(await _source.WalkAsync(), await _target.WalkAsync());
        }

        [Fact]
        public async Task Async_Method_Without_Await()
        {
            Assert.Equal(await _source.WalkAsync(), await _target.WalkAsync());
        }

        [Fact]
        public void Get_Source_Reference_Check()
        {
            Assert.Same(_source, _target.GetSource());
            Assert.Same(_source, _target.GetSource<Duck>());
        }
    }
}
