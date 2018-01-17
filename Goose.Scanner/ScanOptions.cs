using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Goose.Scanner.Flow;

namespace Goose.Scanner
{
    class ScanOptions : IScanOptions, ISourceAssemblyRegistered, ITargetAssemblyRegistered, IScanOptionsBlock
    {
        private List<ScanOptionsBlock> _blocks;
        private Assembly _source, _target;
        private List<IConvention> _conventions;

        public ISourceAssemblyRegistered FromAssemblyOf<T>()
        {
            return this.FromAssembly(typeof(T).Assembly);
        }

        public ISourceAssemblyRegistered FromAssembly(Assembly source)
        {
            MakeBlock();
            _source = source;
            return this;
        }

        public ITargetAssemblyRegistered ToAssemblyOf<T>()
        {
            return this.ToAssembly(typeof(T).Assembly);
        }

        public ITargetAssemblyRegistered ToAssembly(Assembly target)
        {
            _target = target;
            return this;
        }

        public IScanOptionsBlock WithDefaultConvention()
        {
            return this.WithConvention(DefaultConvention.Instance);
        }

        public IScanOptionsBlock WithConvention(Func<Type, Type, bool> predicate)
        {
            return this.WithConvention(new DelegateConvention(predicate));
        }

        public IScanOptionsBlock WithConvention<T>() where T : IConvention, new()
        {
            return this.WithConvention(new T());
        }

        public IScanOptionsBlock WithConvention(IConvention convention)
        {
            if (_conventions == null) _conventions = new List<IConvention>();
            _conventions.Add(convention);
            return this;
        }

        private void MakeBlock()
        {
            if (_source != null && _target != null && _conventions != null)
            {
                if (_blocks == null) _blocks = new List<ScanOptionsBlock>();
                _blocks.Add(new ScanOptionsBlock(_source, _target, _conventions));
                _source = null;
                _target = null;
                _conventions = null;
            }
        }

        public IEnumerable<ScanOptionsBlock> Blocks
        {
            get
            {
                MakeBlock();
                return _blocks;
            }
        }
    }
}
