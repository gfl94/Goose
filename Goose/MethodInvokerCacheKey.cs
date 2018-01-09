using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Goose
{
    class MethodInvokerCacheKey
    {
        public MethodInfo MethodInfo { get; }
        public GooseOptions Options { get; }
        public MethodInvokerCacheKey(MethodInfo m, GooseOptions options)
        {
            this.MethodInfo = m;
            this.Options = options;
        }

        public override int GetHashCode()
        {
            return this.MethodInfo.GetHashCode() * 3 + this.Options.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is MethodInvokerCacheKey another
                && this.MethodInfo.Equals(another.MethodInfo)
                && this.Options.Equals(another.Options);
        }
    }
}
