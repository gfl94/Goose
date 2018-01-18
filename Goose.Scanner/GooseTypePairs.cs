using System;
using System.Collections.Generic;
using System.Linq;

namespace Goose.Scanner
{
    public class GooseTypePairs
    {
        public static GooseTypePair[] Scan(Action<ISourceAssemblySelector> options)
        {
            var selector = new SourceAssemblySelector();
            options(selector);

            List<GooseTypePair> pairs = new List<GooseTypePair>();
            return Populate(pairs, selector).ToArray();
        }

        private static List<GooseTypePair> Populate(List<GooseTypePair> pairs, ISelector selector)
        {
            selector.Populate(pairs);
            return pairs;
        }
    }
}
