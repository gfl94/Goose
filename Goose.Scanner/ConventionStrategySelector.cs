using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Goose.Scanner
{
    class ConventionStrategySelector : IConventionStrategySelector, ISelector
    {
        private ITargetAssemblySelector Inner { get; }
        private IEnumerable<Type> Sources { get; }
        private IEnumerable<Type> Targets { get; }
        private IConvention Convention { get; set; }
        
        public ConventionStrategySelector(ITargetAssemblySelector inner, IEnumerable<Type> sources, IEnumerable<Type> targets)
        {
            Inner = inner;
            Sources = sources;
            Targets = targets;
        }

        public ISourceAssemblySelector WithConvention(Func<Type, Type, bool> predicate)
        {
            return WithConvention(new DelegateConvention(predicate));
        }

        public ISourceAssemblySelector WithDefaultConvention()
        {
            return WithConvention(DefaultConvention.Instance);
        }

        public ISourceAssemblySelector WithConvention<T>() where T : IConvention, new()
        {
            return WithConvention(new T());
        }

        public ISourceAssemblySelector WithConvention(IConvention convention)
        {
            Convention = convention;
            return this;
        }

        void ISelector.Populate(List<GooseTypePair> pairs)
        {
            foreach (var target in Targets)
            {
                foreach (var source in Sources)
                {
                    if (Convention.IsValidPair(source, target))
                    {
                        pairs.Add(GooseTypePair.Create(source, target));
                    }
                };
            };
        }

        #region Chain Methods
        public ITargetAssemblySelector FromAssembly(Assembly assembly)
        {
            return Inner.FromAssembly(assembly);
        }

        public IConventionStrategySelector ToAssembly(Assembly assembly)
        {
            return Inner.ToAssembly(assembly);
        }

        public IConventionStrategySelector ToAssemblyOf<T>()
        {
            return Inner.ToAssemblyOf<T>();
        }

        public ITargetAssemblySelector FromAssemblyOf<T>()
        {
            return Inner.FromAssemblyOf<T>();
        }
        #endregion
    }
}
