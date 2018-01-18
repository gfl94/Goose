using System;
using System.Collections.Generic;
using System.Text;

namespace Goose.Scanner
{
    public interface IConventionStrategySelector: ITargetAssemblySelector
    {
        ISourceAssemblySelector WithDefaultConvention();
        ISourceAssemblySelector WithConvention(Func<Type, Type, bool> predicate);
        ISourceAssemblySelector WithConvention<T>() where T : IConvention, new();
        ISourceAssemblySelector WithConvention(IConvention convention);
    }
}
