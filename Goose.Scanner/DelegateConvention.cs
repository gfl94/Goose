using System;
using System.Collections.Generic;
using System.Text;

namespace Goose.Scanner
{
    class DelegateConvention : IConvention
    {
        private readonly Func<Type, Type, bool> _convention;

        public DelegateConvention(Func<Type, Type, bool> convention)
        {
            _convention = convention;
        }

        public bool IsValidPair(Type sourceType, Type targetType)
        {
            return _convention(sourceType, targetType);
        }
    }
}
