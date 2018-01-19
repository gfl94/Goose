using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace Goose.Scanner
{
    class ConventionStrategySelector : IConventionStrategySelector, ISelector
    {
        private ITargetAssemblySelector _inner { get; }
        private IEnumerable<Type> _sources { get; }
        private IEnumerable<Type> _targets { get; }
        private List<IConvention> _convention { get; } = new List<IConvention>();
        
        public ConventionStrategySelector(ITargetAssemblySelector inner, IEnumerable<Type> sources, IEnumerable<Type> targets)
        {
            _inner = inner;
            _sources = sources;
            _targets = targets;
        }

        public IConventionStrategySelector WithConvention(Func<Type, Type, bool> predicate)
        {
            return this.WithConvention(new DelegateConvention(predicate));
        }

        public IConventionStrategySelector WithDefaultConvention()
        {
            return this.WithConvention(DefaultConvention.Instance);
        }

        public IConventionStrategySelector WithConvention<T>() where T : IConvention, new()
        {
            return this.WithConvention(new T());
        }

        public IConventionStrategySelector WithConvention(IConvention convention)
        {
            _convention.Add(convention);
            return this;
        }

        void ISelector.Populate(List<GooseTypePair> pairs)
        {
            pairs.AddRange(_targets.SelectMany(target => _sources.Where(source =>
                    _convention.Any(conv => conv.IsValidPair(source, target))
                ).Select(source => GooseTypePair.Create(source, target))
            ));
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
