using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Goose.Test.DuckTyping
{
    public class PrimitiveType
    {
        public interface IDuck
        {
            void Quack();
        }

        public interface IComparableInt
        {
            int CompareTo(int another);
        }

        [Fact]
        public void Primitive_Type_As_Unrelated_Interface()
        {
            int source = 1;
            IDuck target = source.As<IDuck>();
            Assert.NotNull(target);
            Assert.Equal(source, target.GetSource<int>());
            Assert.Throws<InvalidCastException>(() => target.GetSource<long>());
        }

        [Fact]
        public void Primitive_Type_With_Implemented_Method()
        {
            int source = 5;
            IComparableInt target = source.As<IComparableInt>();
            int comparand = DateTime.Now.Second;
            Assert.Equal(source.CompareTo(comparand), target.CompareTo(comparand));
        }
    }
}
