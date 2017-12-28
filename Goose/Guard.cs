using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Goose
{
    class Guard
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RequirePublicInterface(Type type)
        {
            if (!type.IsInterface || !type.IsPublic)
            {
                throw new ArgumentException($"{type.FullName} is not a public interface");
            }
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static void RequireClass(Type type)
        //{
        //    if (!type.IsClass)
        //    {
        //        throw new ArgumentException($"{type.FullName} is not an interface");
        //    }
        //}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RequireNotNull(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException();
            }
        }
    }
}
