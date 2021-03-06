﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Goose
{
    public static class Extensions
    {
        public static TTarget As<TTarget>(this object source, params GooseTypePair[] knownTypes)
            where TTarget : class
        {
            return (TTarget)As(source, typeof(TTarget), knownTypes);
        }

        public static object As(this object source, Type targetType, params GooseTypePair[] knownTypes)
        {
            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));

            if (source != null && targetType.IsAssignableFrom(source.GetType()))
            {
                return source;
            }

            var options = new GooseOptions
            {
                KnownTypes = knownTypes?.Length > 0 
                    ? new HashSet<GooseTypePair>(knownTypes) 
                    : new HashSet<GooseTypePair>()
            };

            if (source != null)
            {
                var selfKnownPair = GooseTypePair.Create(source.GetType(), targetType);
                options.KnownTypes.Add(selfKnownPair);
            }

            return GooseProxy.Build(source, targetType, options);
        }

        public static T GetSource<T>(this object target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (target is IGooseTyped goosed)
            {
                return (T)goosed.Source;
            }
            else if (typeof(T).IsAssignableFrom(target.GetType()))
            {
                return (T)target;
            }
            else
            {
                throw new ArgumentException($"{nameof(target)} is not goose-typed");
            }
        }
    }
}
