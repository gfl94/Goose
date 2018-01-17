using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Goose.Scanner
{
    class SourceAssemblySelector : ISourceAssemblySelector, ISelector
    {
        private List<ISelector> Selectors { get; } = new List<ISelector>();

        public ITargetAssemblySelector FromAssembly(Assembly assembly)
        {
            return AddSelector(assembly.GetTypes());
        }

        private ITargetAssemblySelector AddSelector(IEnumerable<Type> sources)
        {
            var selector = new TargetAssemblySelector(this, sources);

            Selectors.Add(selector);

            return selector;
        }

        void ISelector.Populate(List<GooseTypePair> pairs)
        {
            if (Selectors.Count == 0)
            {
                Console.WriteLine("no from assembly");
                return;
            }

            foreach (var selector in Selectors)
            {
                selector.Populate(pairs);
            }
        }
    }
}
