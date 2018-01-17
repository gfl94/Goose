using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Goose.Scanner.Flow
{
    public interface ISourceAssemblyRegistered
    {
        ITargetAssemblyRegistered ToAssemblyOf<T>();
        ITargetAssemblyRegistered ToAssembly(Assembly target);
    }
}
