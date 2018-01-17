using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Goose.Scanner
{
    class ScanOptionsBlock
    {
        public Assembly Source { get; }
        public Assembly Target { get; }
        public IEnumerable<IConvention> Conventions { get; }

        public ScanOptionsBlock(Assembly source, Assembly target, IEnumerable<IConvention> conventions)
        {
            this.Source = source;
            this.Target = target;
            this.Conventions = conventions;
        }
    }
}
