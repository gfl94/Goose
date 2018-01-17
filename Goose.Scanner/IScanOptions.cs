using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Goose.Scanner.Flow;

namespace Goose.Scanner
{
    public interface IScanOptions
    {        
        ISourceAssemblyRegistered FromAssemblyOf<T>();
        ISourceAssemblyRegistered FromAssembly(Assembly source);
    }
}
