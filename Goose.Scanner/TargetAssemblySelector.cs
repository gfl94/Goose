using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Goose.Scanner
{
    class TargetAssemblySelector : ITargetAssemblySelector, ISelector
    {
        private ISourceAssemblySelector Inner { get; }
        private IEnumerable<Type> Sources { get; }

        private List<ISelector> Selectors { get; } = new List<ISelector>();

        public TargetAssemblySelector(ISourceAssemblySelector inner, IEnumerable<Type> sources)
        {
            Inner = inner;
            Sources = sources;
        }

        public IConventionStrategySelector ToAssembly(Assembly assembly)
        {
            return AddSelector(assembly.GetTypes());
        }

        public IConventionStrategySelector ToAssemblyOf<T>()
        {
            return ToAssembly(typeof(T).Assembly);
        }

        void ISelector.Populate(List<GooseTypePair> pairs)
        {
            if (Selectors.Count == 0)
            {
                Console.WriteLine("no target assembly here");
                return;
            }

            foreach (var selector in Selectors)
            {
                selector.Populate(pairs);
            }
        }

        private IConventionStrategySelector AddSelector(IEnumerable<Type> targets)
        {
            var selector = new ConventionStrategySelector(this, Sources, targets);

            Selectors.Add(selector);

            return selector;
        }

        #region Chain Method
        public ITargetAssemblySelector FromAssembly(Assembly assembly)
        {
            return Inner.FromAssembly(assembly);
        }

        public ITargetAssemblySelector FromAssemblyOf<T>()
        {
            return Inner.FromAssemblyOf<T>();
        }
        #endregion
    }
}
