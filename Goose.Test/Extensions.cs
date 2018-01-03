using System;
using System.Collections.Generic;
using System.Text;

namespace Goose.Test
{
    static class Extensions
    {
        public static TSource GetSource<TSource>(this object target)
        {
            return (TSource)(target as IGooseTarget)?.Source;
        }
    }
}
