using System;
using System.Collections.Generic;
using System.Linq;

namespace Goose.Scanner
{
    public class GooseTypePairs
    {
        public static GooseTypePair[] Scan(Action<IScanOptions> builder)
        {
            var options = new ScanOptions();
            builder(options);

            return options.Blocks
                .SelectMany(opt => opt.Source.GetTypes()
                    .Where(type => !type.IsInterface)
                    .SelectMany(source => opt.Target.GetTypes()
                        .Where(target => target.IsInterface && opt.Conventions.Any(c => c.IsValidPair(source, target)))
                        .Select(target => GooseTypePair.Create(source, target))))
                .ToArray();
        }
    }
}
