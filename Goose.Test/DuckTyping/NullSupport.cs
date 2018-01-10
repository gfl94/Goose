using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Goose.Test.DuckTyping
{
    public class NullSupport
    {
        public interface IDuck
        {
            string Quack();
        }

        class Duck
        {
            public string Quack() => "Quack";
        }

        [Fact]
        public void Null_Can_Be_Goosed()
        {
            Duck source = null;
            IDuck target = source.As<IDuck>();
            Assert.NotNull(target);
            Assert.Null(target.GetSource<Duck>());
        }

        [Fact]
        public void NullReferenceException_When_Invoking()
        {
            Duck source = null;
            IDuck target = source.As<IDuck>();
            Assert.NotNull(target);
            Assert.Throws<NullReferenceException>(() => target.Quack());
        }
    }
}
