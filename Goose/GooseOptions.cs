using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Goose
{
    class GooseOptions
    {
        public HashSet<GooseTypePair> KnownTypes { get; set; }

        public override int GetHashCode()
        {
            if (this.KnownTypes.Count == 0) return 0;
            return this.KnownTypes.Count 
                * this.KnownTypes.Sum(p => p.SourceType.GetHashCode() + p.TargetType.GetHashCode() * 3);
        }

        public override bool Equals(object obj)
        {
            return obj is GooseOptions another
                && this.KnownTypes.Count == another.KnownTypes.Count
                && this.KnownTypes.OrderBy(t => t.ToString()).SequenceEqual(another.KnownTypes.OrderBy(t => t.ToString()));
        }
    }
}
