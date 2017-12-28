using System;
using System.Collections.Generic;
using System.Text;
using Castle.DynamicProxy;

namespace Goose
{
    class GooseInterceptor : IInterceptor
    {
        private readonly object _instance;
        private readonly GooseOption _option;

        public GooseInterceptor(object instance, GooseOption option)
        {
            _instance = instance;
            _option = option;
        }

        public void Intercept(IInvocation invocation)
        {
            throw new NotImplementedException($"{_instance.GetType().Name} doesn't implement method {invocation.Method.GetSignatureText()}");
        }
    }
}
