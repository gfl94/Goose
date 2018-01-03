using System;
using System.Collections.Generic;
using System.Text;

namespace Goose
{
    public static class Extensions
    {
        public static TTarget Goose<TTarget>(this object source, params GooseTypePair[] knownTypes)
            where TTarget : class
        {
            return (TTarget)Goose(source, typeof(TTarget), knownTypes);
        }

        public static object Goose(this object source, Type targetType, params GooseTypePair[] knownTypes)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));            

            var options = new GooseOptions
            {
                KnownTypes = knownTypes?.Length > 0 ? new HashSet<GooseTypePair>(knownTypes) : new HashSet<GooseTypePair>()
            };

            var selfKnownPair = GooseTypePair.Create(source.GetType(), targetType);
            options.KnownTypes.Add(selfKnownPair);

            return GooseProxy.Build(source, targetType, options);
        }
    }
}
