using System;
using System.Collections.Generic;
using System.Text;

namespace Goose
{
    public static class Extensions
    {
        public static TInterface Goose<TInterface>(this object instance)
            where TInterface : class
        {
            return Goose<TInterface>(instance, GooseOption.Default);
        }

        public static TInterface Goose<TInterface>(this object instance, GooseOption option)
            where TInterface : class
        {
            Guard.RequireNotNull(instance);
            //Guard.RequireClass(instance.GetType());
            Guard.RequirePublicInterface(typeof(TInterface));

            return GooseProxy.Build<TInterface>(instance, option);
        }
    }
}
