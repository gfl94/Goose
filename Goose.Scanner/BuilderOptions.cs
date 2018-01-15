using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Goose.Scanner
{
    public class BuilderOptions
    {
        public BuilderOptions FromAssembly(Assembly assembly)
        {
            throw new NotImplementedException();
        }

        public BuilderOptions ToAssembly(Assembly assembly)
        {
            throw new NotImplementedException();
        }

        public BuilderOptions WithDefaultConventions()
        {
            throw new NotImplementedException();
        }

        public BuilderOptions WithConvention(Func<Type, Type, bool> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
