using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Goose
{
    class MethodCompatibility
    {
        public MethodInfo Method { get; }
        public TypeCompatibility[] ArgumentCompatibilities { get; }
        public TypeCompatibility ReturnCompatibility { get; }

        public MethodCompatibility(MethodInfo method, 
            TypeCompatibility[] argumentCompatibilities,
            TypeCompatibility returnCompatibility)
        {
            this.Method = method;
            this.ArgumentCompatibilities = argumentCompatibilities;
            this.ReturnCompatibility = returnCompatibility;
        }

        public int Score => this.ArgumentCompatibilities.Sum(a => GetScore(a)) + GetScore(this.ReturnCompatibility);

        private int GetScore(TypeCompatibility compatibility)
        {
            switch (compatibility)
            {
                case TypeCompatibility.NoOp: return 100;
                case TypeCompatibility.ValueType: return 90;
                case TypeCompatibility.ToGoose: return 10;
                case TypeCompatibility.FromGoose: return 1;
                default: return 0;
            }
        }
    }
}
