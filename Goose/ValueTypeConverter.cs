using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goose
{
    class ValueTypeConverter
    {
        public static object Convert(object o, Type targetType)
        {
            if (o.GetType().IsEnum)
            {
                o = System.Convert.ChangeType(o, Enum.GetUnderlyingType(o.GetType()));
            }

            if (targetType.IsEnum)
            {
                o = System.Convert.ChangeType(o, Enum.GetUnderlyingType(targetType));

                if (Enum.IsDefined(targetType, o))
                {
                    return Enum.ToObject(targetType, o);
                }
                else
                {
                    throw new InvalidCastException($"Can't cast {o.ToString()} to {targetType.FullName}");
                }
            }
            else
            {
                return System.Convert.ChangeType(o, targetType);
            }
        }

    }
}
