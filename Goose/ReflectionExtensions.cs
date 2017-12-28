using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Goose
{
    static class ReflectionExtensions
    {
        public static string GetSignatureText(this MethodInfo method)
        {
            return $"{method.Name}{GetGenericSignatureText(method)}{GetParameterSignatureText(method)}";
        }

        private static string GetGenericSignatureText(MethodInfo method)
        {
            if (!method.IsGenericMethod) return string.Empty;
            return $"<{string.Join(", ", method.GetGenericArguments().Select(a => a.FullName))}>";
        }

        private static string GetParameterSignatureText(MethodInfo method)
        {
            var parameters = method.GetParameters();
            if (parameters.Length == 0) return "()";
            return $"({string.Join(", ", parameters.Select(p => GetParameterText(p)))})";
        }

        private static string GetParameterText(ParameterInfo p)
        {
            var parameterTypeName = p.ParameterType.FullName;
            if (!p.ParameterType.IsByRef) return parameterTypeName;
            if (p.IsOut) return "out " + parameterTypeName;
            return "ref " + parameterTypeName;
        }
    }
}
