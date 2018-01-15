using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Goose
{
    public class GooseNotImplementedException : NotImplementedException
    {
        public Type Type { get; }
        public MethodInfo Method { get; }

        public GooseNotImplementedException(Type type, MethodInfo method)
            : base($"{type.FullName} doesn't implement \"{method}\"")
        {
            this.Type = type;
            this.Method = method;
        }
    }

    public class GooseAmbiguousMatchException : Exception
    {
        public IEnumerable<MethodInfo> Methods { get; }

        public GooseAmbiguousMatchException(params MethodInfo[] methods)
            : base($"The method call is ambiguous in {string.Join(", ", methods.Select(m => m.ToString()))}")
        {
            this.Methods = methods;
        }
    }
}
