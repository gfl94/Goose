using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Goose.Scanner
{
    class ConventionStrategySelector : IConventionStrategySelector, ISelector
    {
        private ITargetAssemblySelector _inner { get; }
        private IEnumerable<Type> _sources { get; }
        private IEnumerable<Type> _targets { get; }
        private IConvention _convention { get; set; }
        
        public ConventionStrategySelector(ITargetAssemblySelector inner, IEnumerable<Type> sources, IEnumerable<Type> targets)
        {
            _inner = inner;
            _sources = sources;
            _targets = targets;
        }

        public ISourceAssemblySelector WithConvention(Func<Type, Type, bool> predicate)
        {
            return this.WithConvention(new DelegateConvention(predicate));
        }

        public ISourceAssemblySelector WithDefaultConvention()
        {
            return this.WithConvention(DefaultConvention.Instance);
        }

        public ISourceAssemblySelector WithConvention<T>() where T : IConvention, new()
        {
            return this.WithConvention(new T());
        }

        public ISourceAssemblySelector WithConvention(IConvention convention)
        {
            _convention = convention;
            return this;
        }

        void ISelector.Populate(List<GooseTypePair> pairs)
        {
            foreach (var target in _targets)
            {
                foreach (var source in _sources)
                {
                    if (_convention.IsValidPair(source, target))
                    {
                        pairs.Add(GooseTypePair.Create(source, target));
                    }
                };
            };
        }

        #region Chain Methods
        public ITargetAssemblySelector FromAssembly(Assembly assembly)
        {
            return _inner.FromAssembly(assembly);
        }

        public IConventionStrategySelector ToAssembly(Assembly assembly)
        {
            return _inner.ToAssembly(assembly);
        }

        public IConventionStrategySelector ToAssemblyOf<T>()
        {
            return _inner.ToAssemblyOf<T>();
        }

        public ITargetAssemblySelector FromAssemblyOf<T>()
        {
            return _inner.FromAssemblyOf<T>();
        }
        #endregion
    }
}
