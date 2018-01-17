using System;
using System.Collections.Generic;
using System.Text;

namespace Goose.Scanner
{
    class DefaultConvention : IConvention
    {
        public static readonly DefaultConvention Instance = new DefaultConvention();

        public bool IsValidPair(Type sourceType, Type targetType)
        {
            throw new NotImplementedException();
        }
    }
}
