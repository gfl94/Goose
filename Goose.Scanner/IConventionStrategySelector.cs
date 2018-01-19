using System;
using System.Collections.Generic;
using System.Text;

namespace Goose.Scanner
{
    public interface IConventionStrategySelector: ITargetAssemblySelector
    {
        IConventionStrategySelector WithDefaultConvention();
        IConventionStrategySelector WithConvention(Func<Type, Type, bool> predicate);
        IConventionStrategySelector WithConvention<T>() where T : IConvention, new();
        IConventionStrategySelector WithConvention(IConvention convention);
    }
}
