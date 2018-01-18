using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Goose.Scanner
{
    class SourceAssemblySelector : ISourceAssemblySelector, ISelector
    {
        private List<ISelector> _selectors { get; } = new List<ISelector>();

        public ITargetAssemblySelector FromAssembly(Assembly assembly)
        {
            return AddSelector(assembly.GetTypes());
        }

        public ITargetAssemblySelector FromAssemblyOf<T>()
        {
            return this.FromAssembly(typeof(T).Assembly);
        }

        private ITargetAssemblySelector AddSelector(IEnumerable<Type> sources)
        {
            var selector = new TargetAssemblySelector(this, sources);

            _selectors.Add(selector);

            return selector;
        }

        void ISelector.Populate(List<GooseTypePair> pairs)
        {
            if (_selectors.Count == 0)
            {
                Console.WriteLine("no source assembly");
                return;
            }

            foreach (var selector in _selectors)
            {
                selector.Populate(pairs);
            }
        }
    }
}
