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
            = typeof(IGooseTyped).GetProperty(nameof(IGooseTyped.Source)).GetGetMethod();

        public static readonly MethodInfo GooseExtensionMethod
            = typeof(Extensions).GetMethod(nameof(Extensions.As), new[] { typeof(object), typeof(Type), typeof(GooseTypePair[]) });

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
            if (invocation.Method == GetGooseSourceMethod)
            {
                invocation.ReturnValue = _source;
                return;
            }

            if (_source == null)
                throw new NullReferenceException($"{nameof(invocation.Method)}: Object reference not set to an instance of an object.");

            var sourceType = _source.GetType();

            if (_blacklist.Contains(invocation.Method))
                throw new GooseNotImplementedException(sourceType, invocation.Method);

            if (_knownHandlers.ContainsKey(invocation.Method))
            {
                invocation.ReturnValue = _knownHandlers[invocation.Method](invocation.Arguments);
                return;
            }

            var sourceCandidateMethods = sourceType
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.Name.Equals(invocation.Method.Name));

            var invocationParameters = invocation.Method.GetParameters();
            var candidates = new List<MethodCompatibility>();

            foreach (var sourceMethod in sourceCandidateMethods)
            {
                var sourceParameters = sourceMethod.GetParameters();
                if (sourceParameters.Length != invocationParameters.Length) continue;

                var argumentCompatibilities = new TypeCompatibility[invocation.Arguments.Length];
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

                var returnTypeCompatibility = GetCompatibility(sourceMethod.ReturnType, invocation.Method.ReturnType);
                if (returnTypeCompatibility == TypeCompatibility.Incompatible) continue;

                candidates.Add(new MethodCompatibility(sourceMethod, argumentCompatibilities, returnTypeCompatibility));
            }

            MethodCompatibility candidate = null;

            if (candidates.Count > 1)
            {
                var highestScoredCandidates = candidates
                    .OrderByDescending(c => c.Score)
                    .TakeWhile((c, i) => i == 0 || candidates[i].Score == candidates[i - 1].Score)
                    .ToList();

                if (highestScoredCandidates.Count == 1)
                {
                    candidate = highestScoredCandidates.Single();
                }
                else
                {
                    var orderByDeclaringTypes = highestScoredCandidates
                        .Select(c => new { Candidate = c, Inheritance = GetInheritanceString(c.Method.DeclaringType) })
                        .OrderByDescending(x => x.Inheritance)
                        .ToList();

                    if (orderByDeclaringTypes[0].Inheritance.Equals(orderByDeclaringTypes[1].Inheritance))
                    {
                        throw new GooseAmbiguousMatchException(orderByDeclaringTypes[0].Candidate.Method, orderByDeclaringTypes[1].Candidate.Method);
                    }

                    candidate = orderByDeclaringTypes[0].Candidate;
                }
            }
            else if (candidates.Count == 1)
            {
                candidate = candidates.Single();
            }
            else if (candidates.Count == 0)
            {
                _blacklist.Add(invocation.Method);
                throw new GooseNotImplementedException(sourceType, invocation.Method);
            }

            var handler = MakeHandler(invocation, candidate);
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

        private Func<object[], object> MakeHandler(IInvocation invocation, MethodCompatibility candidate)
        {
            var parameter = Expression.Parameter(typeof(object[]));
            var variables = new List<ParameterExpression>();
            var beforeInvoke = new List<Expression>();
            var invoke = new List<Expression>();
            var afterInvoke = new List<Expression>();

            var arguments = new List<Expression>();
            var methodParameters = candidate.Method.GetParameters();
            for (var i = 0; i < candidate.ArgumentCompatibilities.Length; i++)
            {
                var arrayIndex = Expression.ArrayIndex(parameter, Expression.Constant(i));
                Expression argument = null;
                if (candidate.ArgumentCompatibilities[i] == TypeCompatibility.Same)
                {
                    argument = arrayIndex;
                }
                else if (candidate.ArgumentCompatibilities[i] == TypeCompatibility.FromGoose)
                {
                    var asIGooseTarget = Expression.Convert(arrayIndex, typeof(IGooseTyped));
                    var getSource = Expression.Property(asIGooseTarget, nameof(IGooseTyped.Source));
                    argument = getSource;
                }
                else if (candidate.ArgumentCompatibilities[i] == TypeCompatibility.ToGoose)
                {
                    var gooseType = Expression.Constant(methodParameters[i].ParameterType);
                    var callGooseExtension = Expression.Call(GooseExtensionMethod, arrayIndex, gooseType, Expression.Constant(_options.KnownTypes.ToArray()));
                    argument = callGooseExtension;
                }

                if (methodParameters[i].IsOut)
                {
                    var local = Expression.Variable(methodParameters[i].ParameterType.GetElementType());
                    variables.Add(local);
                    var assignLocal = Expression.Assign(local, Expression.Convert(argument, methodParameters[i].ParameterType.GetElementType()));
                    beforeInvoke.Add(assignLocal);

                    var arrayAccess = Expression.ArrayAccess(parameter, Expression.Constant(i));
                    var assignBack = Expression.Assign(arrayAccess, Expression.Convert(local, typeof(object)));
                    afterInvoke.Add(assignBack);
                    arguments.Add(local);
                }
                else
                {
                    arguments.Add(Expression.Convert(argument, methodParameters[i].ParameterType));
                }
            }

            LabelTarget returnTarget = Expression.Label(typeof(object));
            ParameterExpression returnVariable = Expression.Variable(typeof(object));
            variables.Add(returnVariable);
            LabelExpression returnLabel = Expression.Label(returnTarget, returnVariable);
            afterInvoke.Add(Expression.Return(returnTarget, returnVariable));
            afterInvoke.Add(returnLabel);

            Expression invokeSource = Expression.Call(Expression.Constant(_source), candidate.Method, arguments);
            if (candidate.ReturnCompatibility == TypeCompatibility.FromGoose)
            {
                var asIGooseTarget = Expression.Convert(invokeSource, typeof(IGooseTyped));
                var getSource = Expression.Property(asIGooseTarget, nameof(IGooseTyped.Source));
                invokeSource = getSource;
            }
            else if (candidate.ReturnCompatibility == TypeCompatibility.ToGoose)
            {
                var gooseType = Expression.Constant(invocation.Method.ReturnType);
                invokeSource = Expression.Call(GooseExtensionMethod, invokeSource, gooseType, Expression.Constant(_options.KnownTypes.ToArray()));
            }

            invoke.Add(returnVariable);
            if (candidate.Method.ReturnType == typeof(void))
            {
                invoke.Add(invokeSource);
                invoke.Add(Expression.Assign(returnVariable, Expression.Constant(null)));
            }
            else
            {
                invoke.Add(Expression.Assign(returnVariable, Expression.Convert(invokeSource, typeof(object))));
            }


            Expression body = Expression.Block(variables, beforeInvoke.Concat(invoke).Concat(afterInvoke));

            var exceptions = _options.KnownTypes
                .Where(t => t.SourceType.IsSubclassOf(typeof(Exception)))
                .ToList();

            if (exceptions.Count > 0)
            {
                var catches = exceptions
                    .OrderByDescending(e => GetInheritanceString(e.SourceType))
                    .Select(e =>
                    {
                        var p = Expression.Parameter(e.SourceType);
                        var gooseType = Expression.Constant(e.TargetType);
                        var callGooseExtension = Expression.Call(GooseExtensionMethod, p, gooseType, Expression.Constant(_options.KnownTypes.ToArray()));

                        var ctor = typeof(WrappedException<>).MakeGenericType(e.TargetType).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Single();
                        var throwWrap = Expression.Throw(Expression.New(ctor, Expression.Convert(callGooseExtension, e.TargetType)));
                        return Expression.Catch(p, Expression.Block(throwWrap, Expression.Constant(null)));
                    });

                body = Expression.TryCatch(body, catches.ToArray());
            }

            var lambda = Expression.Lambda<Func<object[], object>>(body, parameter);
            return lambda.Compile();
        }

        private string GetInheritanceString(Type type)
        {
            var typeNames = new Stack<string>();
            while (!type.Equals(typeof(object)))
            {
                typeNames.Push(type.Name);
                type = type.BaseType;
            }
            return string.Join("->", typeNames.Reverse());
        }

        private int GetScore(TypeCompatibility compatibility)
        {
            switch (compatibility)
            {
                case TypeCompatibility.Same: return 100;
                case TypeCompatibility.ToGoose: return 10;
                case TypeCompatibility.FromGoose: return 1;
                default: return 0;
            }
        }
    }
}
