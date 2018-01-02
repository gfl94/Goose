using System;
using System.Collections.Generic;
using System.Text;
using Castle.DynamicProxy;

namespace Goose
{
    class GooseProxy
    {
        static IProxyGenerator ProxyGenerator = new ProxyGenerator();

        public static object Build(object source, Type targetType, GooseOptions options)
        {
            var interceptor = new GooseInterceptor(source, targetType, options);
            return ProxyGenerator.CreateInterfaceProxyWithoutTarget(targetType, new[] { typeof(IGooseTarget) }, interceptor);
        }
    }
}
