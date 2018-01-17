using System;
using System.Collections.Generic;
using System.Text;

namespace Goose.Scanner
{
    public interface IConventionStrategySelector: ITargetAssemblySelector
    {
        ISourceAssemblySelector WithDefaultConventions();
        ISourceAssemblySelector WithConvention(Func<Type, Type, bool> predicate);
    }
}
