using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Goose.Scanner
{
    class DefaultConvention : IConvention
    {
        public static readonly DefaultConvention Instance = new DefaultConvention();

        public bool IsValidPair(Type sourceType, Type targetType)
        {
            return IsDefaultClassInterfacePair(sourceType, targetType) || IsDefaultEnumPair(sourceType, targetType);
        }

        private bool IsDefaultClassInterfacePair(Type source, Type target)
        {
            return (!source.IsInterface && target.IsInterface && target.Name[0] == 'I'
                && target.Name.Skip(1).SequenceEqual(source.Name));
        }

        private bool IsDefaultEnumPair(Type source, Type target)
        {
            return (source.IsEnum && target.IsEnum && source.Name == target.Name);
        }
    }
}
