using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Goose.Scanner
{
    public interface ITargetAssemblySelector: ISourceAssemblySelector
    {
        IConventionStrategySelector ToAssembly(Assembly assembly);
        IConventionStrategySelector ToAssemblyOf<T>();
    }
}
