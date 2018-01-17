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
        private Func<Type, Type, bool> Predicate { get; set;}

        public static bool DefaultConvent(Type source, Type target) => target.Name.StartsWith("I")
                && target.Name.Substring(1).CompareTo(source.Name) == 0;
        
        public ConventionStrategySelector(ITargetAssemblySelector inner, IEnumerable<Type> sources, IEnumerable<Type> targets)
        {
            Inner = inner;
            Sources = sources;
            Targets = targets;
        }

        public ISourceAssemblySelector WithConvention(Func<Type, Type, bool> predicate)
        {
            Predicate = predicate;
            return this;
        }

        public ISourceAssemblySelector WithDefaultConventions()
        {
            return WithConvention(DefaultConvent);
        }

        void ISelector.Populate(List<GooseTypePair> pairs)
        {
            foreach (var target in Targets)
            {
                foreach (var source in Sources)
                {
                    if (Predicate(source, target))
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
        #endregion
    }
}
