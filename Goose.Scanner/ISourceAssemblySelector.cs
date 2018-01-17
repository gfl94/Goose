using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Goose.Scanner
{
    public interface ISourceAssemblySelector
    {
        ITargetAssemblySelector FromAssembly(Assembly assembly);
    }
}
