using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Goose.Scanner
{
    class TargetAssemblySelector : ITargetAssemblySelector, ISelector
    {
        private ISourceAssemblySelector _inner { get; }
        private IEnumerable<Type> _sources { get; }

        private List<ISelector> _selectors { get; } = new List<ISelector>();

        public TargetAssemblySelector(ISourceAssemblySelector inner, IEnumerable<Type> sources)
        {
            _inner = inner;
            _sources = sources;
        }

        public IConventionStrategySelector ToAssembly(Assembly assembly)
        {
            return AddSelector(assembly.GetTypes());
        }

        public IConventionStrategySelector ToAssemblyOf<T>()
        {
            return this.ToAssembly(typeof(T).Assembly);
        }

        void ISelector.Populate(List<GooseTypePair> pairs)
        {
            if (_selectors.Count == 0)
            {
                Console.WriteLine("no target assembly here");
                return;
            }

            foreach (var selector in _selectors)
            {
                selector.Populate(pairs);
            }
        }

        private IConventionStrategySelector AddSelector(IEnumerable<Type> targets)
        {
            var selector = new ConventionStrategySelector(this, _sources, targets);

            _selectors.Add(selector);

            return selector;
        }

        #region Chain Method
        public ITargetAssemblySelector FromAssembly(Assembly assembly)
        {
            return _inner.FromAssembly(assembly);
        }

        public ITargetAssemblySelector FromAssemblyOf<T>()
        {
            return _inner.FromAssemblyOf<T>();
        }
        #endregion
    }
}
