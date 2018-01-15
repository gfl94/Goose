using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Goose.Test.DuckTyping
{
    public class InterfaceImplemented
    {
        public interface IDuck { }
        class Duck : IDuck { }
        class ExecitedDuck : Duck { }

        [Fact]
        public void Should_Use_Original_Object()
        {
            Duck source = new Duck();
            IDuck target = source.As<IDuck>();
            Assert.Same(source, target);
            Assert.Same(source, target.GetSource<Duck>());
            Assert.Throws<ArgumentException>(() => target.GetSource<ExecitedDuck>());
        }
    }
}
