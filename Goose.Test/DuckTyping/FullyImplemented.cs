using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Goose.Test.DuckTyping
{
    public class FullyImplemented
    {
        public interface IDuck
        {
            string Quack();
            Task<int> WalkAsync();
            Task<string> SwimAsync();
        }

        class Duck
        {
            public string Quack() => "Quack";
            public Task<int> WalkAsync() => Task.FromResult(DateTime.Now.Second);
            public async Task<string> SwimAsync() => await Task.FromResult("far away");
        }

        static Duck _source = new Duck();
        static IDuck _target = _source.As<IDuck>();

        [Fact]
        public void Simple_Method()
        {
            Assert.Equal(_source.Quack(), _target.Quack());
        }

        [Fact]
        public async Task Async_Method_Without_Await()
        {
            Assert.Equal(await _source.WalkAsync(), await _target.WalkAsync());
        }

        [Fact]
        public async Task Async_Method_With_Await()
        {
            Assert.Equal(await _source.SwimAsync(), await _target.SwimAsync());
        }

        [Fact]
        public void Get_Source_Reference_Check()
        {
            Assert.Same(_source, _target.GetSource());
            Assert.Same(_source, _target.GetSource<Duck>());
        }
    }
}
