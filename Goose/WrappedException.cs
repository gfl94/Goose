using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Goose
{
    public sealed class WrappedException<T> : Exception
        where T : class
    {
        public T Exception { get; }
        private readonly Exception _origin;

        internal WrappedException(T exception)
        {
            this.Exception = exception;
            _origin = (exception as IGooseTyped).Source as Exception;
        }

        public override string Message => _origin.Message;
        public override IDictionary Data => _origin.Data;        
        public override string Source { get => _origin.Source; set => _origin.Source = value; }
        public override string StackTrace => _origin.StackTrace;
        public override Exception GetBaseException() => _origin.GetBaseException();
    }
}
