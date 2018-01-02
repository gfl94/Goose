using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Goose
{
    class Guard
    {
        public static void RequirePublicInterface(Type type)
        {
            if (!type.IsInterface || !type.IsPublic)
            {
                throw new ArgumentException($"{type.FullName} is not a public interface");
            }
        }

        public static void RequireNotNull(object arg, string argName)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(argName);
            }
        }
    }
}
