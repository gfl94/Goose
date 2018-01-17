using System;
using System.Collections.Generic;
using System.Text;

namespace Goose.Scanner
{
    public interface IConvention
    {
        bool IsValidPair(Type sourceType, Type targetType);
    }
}
