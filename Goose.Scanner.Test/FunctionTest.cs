using System;
using System.Collections.Generic;
using System.Text;
using DuckLib;
using Xunit;
using System.Linq;

namespace Goose.Scanner.Test
{
    public class FunctionTest
    {
        [Fact]
        public void Convention_Test()
        {
            var pairs = GooseTypePairs.Scan(options =>
            {
                options.FromAssemblyOf<Duck>().ToAssemblyOf<Duck>()
                    .WithConvention((sourceType, targetType) => targetType.Name == "IStandard" + sourceType.Name);
            });

            Assert.True(CheckPairs(pairs, new GooseTypePair[] { GooseTypePair.Create<Fish, IStandardFish>() }));
        }

        [Fact]
        public void Default_Convention_Test()
        {
            var pairs = GooseTypePairs.Scan(options =>
            {
                options.FromAssemblyOf<Duck>().ToAssemblyOf<Duck>().WithDefaultConvention();
            });

            Assert.True(CheckPairs(pairs, new GooseTypePair[] { GooseTypePair.Create<Duck, IDuck>() }));
        }

        [Fact]
        public void Chain_Convention_Test()
        {
            var pairs = GooseTypePairs.Scan(options =>
            {
                options.FromAssemblyOf<Duck>().ToAssemblyOf<Duck>().WithConvention((sourceType, targetType)
                    => targetType.Name == "IStandard" + sourceType.Name).WithDefaultConvention();
            });

            Assert.True(CheckPairs(pairs, new GooseTypePair[] { GooseTypePair.Create<Duck, IDuck>(),
                GooseTypePair.Create<Fish, IStandardFish>() }));
        }

        bool CheckPairs(GooseTypePair[] expected, GooseTypePair[] actual)
        {
            return expected.Length == actual.Length && expected.Intersect(actual).Count() == expected.Length;
        }
    }
}
