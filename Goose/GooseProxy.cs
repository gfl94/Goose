using System;
using System.Collections.Generic;
using System.Text;
using Castle.DynamicProxy;

namespace Goose
{
    class GooseProxy
    {
        static IProxyGenerator ProxyGenerator = new ProxyGenerator();

        public static TInterface Build<TInterface>(object instance, GooseOption option)
            where TInterface : class
        {
            var interceptor = new GooseInterceptor(instance, option);
            return ProxyGenerator.CreateInterfaceProxyWithoutTarget<TInterface>(interceptor);
        }
    }
}
