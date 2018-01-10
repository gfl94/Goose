using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Goose.Test.DuckTyping
{
    public class Inheritance
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
            public virtual Task<int> WalkAsync() => Task.FromResult(1);
            public Task<string> SwimAsync() => Task.FromResult("Duck.SwimAsync");
        }

        class ExecitedDuck : Duck
        {
            public new string Quack() => "Quack Quack";
            public override Task<int> WalkAsync() => Task.FromResult(2);
        }

        [Fact]
        public void Hide_Base_Using_New()
        {
            ExecitedDuck source = new ExecitedDuck();
            IDuck target = source.As<IDuck>();
            Assert.Equal(source.Quack(), target.Quack());
            Assert.Same(source, target.GetSource<Duck>());
            Assert.Same(source, target.GetSource<ExecitedDuck>());
        }

        [Fact]
        public async Task Override_Virtual_Method()
        {
            ExecitedDuck source = new ExecitedDuck();
            IDuck target = source.As<IDuck>();
            Assert.Equal(await source.WalkAsync(), await target.WalkAsync());
            Assert.Same(source, target.GetSource<ExecitedDuck>());
        }
    }
}
