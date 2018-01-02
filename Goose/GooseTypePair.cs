using System;
using System.Collections.Generic;
using System.Text;

namespace Goose
{
    public class GooseTypePair
    {
        public static GooseTypePair Create<TSource, TTarget>() where TTarget : class
        {
            return Create(typeof(TSource), typeof(TTarget));
        }

        public static GooseTypePair Create(Type sourceType, Type targetType)
        {
            Guard.RequireNotNull(sourceType, nameof(sourceType));
            Guard.RequireNotNull(targetType, nameof(targetType));
            Guard.RequirePublicInterface(targetType);

            return new GooseTypePair(sourceType, targetType);
        }


        public Type SourceType { get; }

        public Type TargetType { get; }

        private GooseTypePair(Type sourceType, Type targetType)
        {
            this.SourceType = sourceType;
            this.TargetType = targetType;
        }

        public override int GetHashCode()
        {
            return this.SourceType.GetHashCode() * 3 + this.TargetType.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is GooseTypePair another
                && this.SourceType.Equals(another.SourceType)
                && this.TargetType.Equals(another.TargetType);
        }

        public override string ToString()
        {
            return $"{{{nameof(SourceType)}={this.SourceType.ToString()}, {nameof(TargetType)}={this.TargetType.ToString()}}}";
        }
    }
}
