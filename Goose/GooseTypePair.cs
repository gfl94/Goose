using System;
using System.Collections.Generic;
using System.Text;

namespace Goose
{
    public class GooseTypePair
    {
        public static GooseTypePair Create<TSource, TTarget>()
        {
            return Create(typeof(TSource), typeof(TTarget));
        }

        public static GooseTypePair Create(Type sourceType, Type targetType)
        {
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));

            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));

            return new GooseTypePair(sourceType, targetType);
        }

        public Type SourceType { get; }

        public Type TargetType { get; }

        public ConvertMethod ConvertMethod { get; }

        private GooseTypePair(Type sourceType, Type targetType)
        {
            this.SourceType = sourceType;
            this.TargetType = targetType;

            if (targetType.IsAssignableFrom(sourceType))
            {
                this.ConvertMethod = ConvertMethod.NoOp;
            }
            else if (targetType.IsInterface && !sourceType.IsInterface)
            {
                this.ConvertMethod = ConvertMethod.Goose;
            }
            else if ((sourceType.IsValueType && sourceType.IsPrimitive || sourceType.IsEnum)
                && (targetType.IsValueType && targetType.IsPrimitive || targetType.IsEnum))
            {
                this.ConvertMethod = ConvertMethod.ValueType;
            }
            else
            {
                throw new ArgumentException($"Can't find a convert method from {sourceType.FullName} to {targetType.FullName}");
            }
        }

        public override int GetHashCode()
        {
            return this.SourceType.GetHashCode() * 5 
                + this.TargetType.GetHashCode() * 3
                + this.ConvertMethod.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is GooseTypePair another
                && this.SourceType.Equals(another.SourceType)
                && this.TargetType.Equals(another.TargetType)
                && this.ConvertMethod.Equals(another.ConvertMethod);
        }

        public override string ToString()
        {
            return $"{{{nameof(SourceType)}={this.SourceType.ToString()}, {nameof(TargetType)}={this.TargetType.ToString()}, {nameof(ConvertMethod)}={this.ConvertMethod.ToString()}}}";
        }
    }
}
