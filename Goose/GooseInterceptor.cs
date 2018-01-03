using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Castle.DynamicProxy;

namespace Goose
{
    class GooseInterceptor : IInterceptor
    {
        private readonly object _source;
        private readonly Type _targetType;
        private readonly GooseOptions _options;
        private readonly Dictionary<MethodInfo, Func<object[], object>> _knownHandlers;
        private readonly HashSet<MethodInfo> _blacklist;

        public static readonly MethodInfo GetGooseSourceMethod
            = typeof(IGooseTarget).GetProperty(nameof(IGooseTarget.Source)).GetGetMethod();

        public static readonly MethodInfo GooseExtensionMethod
            = typeof(Extensions).GetMethod(nameof(Extensions.Goose), new[] { typeof(object), typeof(Type), typeof(GooseTypePair[]) });

        public GooseInterceptor(object source, Type targetType, GooseOptions options)
        {
            _source = source;
            _targetType = targetType;
            _options = options;
            _knownHandlers = new Dictionary<MethodInfo, Func<object[], object>>();
            _blacklist = new HashSet<MethodInfo>();
        }

        public void Intercept(IInvocation invocation)
        {
            var sourceType = _source.GetType();

            if (_blacklist.Contains(invocation.Method))
                throw new GooseNotImplementedException(sourceType, invocation.Method);

            if (invocation.Method == GetGooseSourceMethod)
            {
                invocation.ReturnValue = _source;
                return;
            }

            if (_knownHandlers.ContainsKey(invocation.Method))
            {
                invocation.ReturnValue = _knownHandlers[invocation.Method](invocation.Arguments);
                return;
            }

            var sourceCandidateMethods = sourceType
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.Name.Equals(invocation.Method.Name));


            var invocationParameters = invocation.Method.GetParameters();
            var argumentCompatibilities = new TypeCompatibility[invocation.Arguments.Length];
            var returnTypeCompatibility = TypeCompatibility.Incompatible;
            var validSourceMethods = new List<MethodInfo>();

            foreach (var sourceMethod in sourceCandidateMethods)
            {
                var sourceParameters = sourceMethod.GetParameters();
                if (sourceParameters.Length != invocationParameters.Length) continue;

                var parameterCompatible = true;
                for (var i = 0; i < sourceParameters.Length; i++)
                {
                    argumentCompatibilities[i] = GetCompatibility(invocationParameters[i].ParameterType, sourceParameters[i].ParameterType);
                    if (argumentCompatibilities[i] == TypeCompatibility.Incompatible)
                    {
                        parameterCompatible = false;
                        break;
                    }
                }

                if (!parameterCompatible) continue;

                returnTypeCompatibility = GetCompatibility(sourceMethod.ReturnType, invocation.Method.ReturnType);
                if (returnTypeCompatibility == TypeCompatibility.Incompatible) continue;

                validSourceMethods.Add(sourceMethod);
            }

            if (validSourceMethods.Count > 1)
                throw new GooseAmbiguousMatchException(validSourceMethods);

            if (validSourceMethods.Count == 0)
            {
                _blacklist.Add(invocation.Method);
                throw new GooseNotImplementedException(sourceType, invocation.Method);
            }

            var handler = MakeHandler(invocation, validSourceMethods.Single(), argumentCompatibilities, returnTypeCompatibility);
            invocation.ReturnValue = handler(invocation.Arguments);
            _knownHandlers.Add(invocation.Method, handler);
        }

        private TypeCompatibility GetCompatibility(Type fromType, Type toType)
        {
            if (fromType.Equals(toType)) return TypeCompatibility.Same;

            foreach (var pair in _options.KnownTypes)
            {
                if (pair.SourceType.Equals(fromType) && pair.TargetType.Equals(toType))
                {
                    return TypeCompatibility.ToGoose;
                }

                if (pair.SourceType.Equals(toType) && pair.TargetType.Equals(fromType))
                {
                    return TypeCompatibility.FromGoose;
                }
            }

            return TypeCompatibility.Incompatible;
        }

        private Func<object[], object> MakeHandler(IInvocation invocation, MethodInfo method, TypeCompatibility[] argumentCompatibilities, TypeCompatibility returnTypeCompatibility)
        {
            var parameter = Expression.Parameter(typeof(object[]));

            var arguments = new List<Expression>();
            var methodParameters = method.GetParameters();
            for (var i = 0; i < argumentCompatibilities.Length; i++)
            {
                var accessArgument = Expression.ArrayIndex(parameter, Expression.Constant(i));
                Expression argument = null;
                if (argumentCompatibilities[i] == TypeCompatibility.Same)
                {
                    argument = accessArgument;
                }
                else if (argumentCompatibilities[i] == TypeCompatibility.FromGoose)
                {
                    var asIGooseTarget = Expression.Convert(accessArgument, typeof(IGooseTarget));
                    var getSource = Expression.Property(asIGooseTarget, nameof(IGooseTarget.Source));
                    argument = getSource;
                }
                else if (argumentCompatibilities[i] == TypeCompatibility.ToGoose)
                {
                    var gooseType = Expression.Constant(methodParameters[i].ParameterType);
                    var callGooseExtension = Expression.Call(GooseExtensionMethod, accessArgument, gooseType, Expression.Constant(_options.KnownTypes.ToArray()));
                    argument = callGooseExtension;
                }
                arguments.Add(Expression.Convert(argument, methodParameters[i].ParameterType));
            }

            Expression invoke = Expression.Call(Expression.Constant(_source), method, arguments);
            if (returnTypeCompatibility == TypeCompatibility.FromGoose)
            {
                var asIGooseTarget = Expression.Convert(invoke, typeof(IGooseTarget));
                var getSource = Expression.Property(asIGooseTarget, nameof(IGooseTarget.Source));
                invoke = getSource;
            }
            else if (returnTypeCompatibility == TypeCompatibility.ToGoose)
            {
                var gooseType = Expression.Constant(invocation.Method.ReturnType);
                invoke = Expression.Call(GooseExtensionMethod, invoke, gooseType, Expression.Constant(_options.KnownTypes.ToArray()));
            }

            Expression body;
            if (method.ReturnType == typeof(void))
            {
                body = Expression.Block(invoke, Expression.Constant(null));
            }
            else
            {
                body = Expression.Convert(invoke, typeof(object));
            }

            return Expression.Lambda<Func<object[], object>>(body, parameter).Compile();
        }
    }
}
