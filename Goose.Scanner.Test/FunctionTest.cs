using System;
using System.Collections.Generic;
using System.Text;
using DuckLib;
using Xunit;

namespace Goose.Scanner.Test
{
    public class FunctionTest
    {
        [Fact]
        public void Convention_Test()
        {
            var pairs = GooseTypePairs.Build(options =>
            {
                options.FromAssembly(typeof(Duck).Module.Assembly).ToAssembly(typeof(Duck).Module.Assembly)
                    .WithConvention((sourceType, targetType) => targetType.Name == "IStandard" + sourceType.Name);
            });

            Assert.True(CheckPairs(pairs, new GooseTypePair[] { GooseTypePair.Create<Fish, IStandardFish>() }));
        }

        [Fact]
        public void Default_Convention_Test()
        {
            var pairs = GooseTypePairs.Build(options =>
            {
                options.FromAssembly(typeof(Duck).Module.Assembly).ToAssembly(typeof(Duck).Module.Assembly)
                    .WithDefaultConventions();
            });

            Assert.True(CheckPairs(pairs, new GooseTypePair[] { GooseTypePair.Create<Duck, IDuck>() }));
        }

        bool CheckPairs(GooseTypePair[] expected, GooseTypePair[] actual)
        {
            if (expected == null && actual == null) return true;
            if (expected == null || actual == null) return false;
            if (expected.Length != actual.Length) return false;

            Dictionary<Type, Type> expected_pairs = new Dictionary<Type, Type>();
            for (var i = 0; i < expected.Length; ++i)
            {
                expected_pairs[expected[i].SourceType] = expected[i].TargetType;
            }

            for (var i = 0; i < expected.Length; ++i)
            {
                Type source = actual[i].SourceType;
                if (!expected_pairs.ContainsKey(source) || expected_pairs[source] != actual[i].TargetType)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
