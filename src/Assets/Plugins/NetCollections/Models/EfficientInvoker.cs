using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace OSK.Unity3D.NetCollections.Assets.Plugins.NetCollections.Models
{
    internal sealed class EfficientInvoker
    {
        #region Variables

        private static readonly ConcurrentDictionary<MemberKey, EfficientInvoker> MemberToWrapperMap
            = new ConcurrentDictionary<MemberKey, EfficientInvoker>(MemberKeyComparer.Instance);

        public Type[] ParameterTypes { get; }
        private readonly Func<object, object[], object> _func;

        #endregion

        #region Constructors

        private EfficientInvoker(Func<object, object[], object> func, MemberKey memberKey)
        {
            _func = func;
            ParameterTypes = memberKey.ParameterTypes;
        }

        #endregion

        #region EfficientInvoker

        public static EfficientInvoker FromMethod(Type serviceType, MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            var key = new MemberKey(serviceType, method.Name, method.GetParameters().Select(p => p.ParameterType).ToArray());
            return MemberToWrapperMap.GetOrAdd(key, k =>
            {
                var wrapper = CreateMethodWrapper(k.Type, method, false);
                return new EfficientInvoker(wrapper, k);
            });
        }

        public static EfficientInvoker ForProperty(Type type, string propertyName)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            var key = new MemberKey(type, propertyName, null);
            return MemberToWrapperMap.GetOrAdd(key, k =>
            {
                var wrapper = CreatePropertyWrapper(type, propertyName);
                return new EfficientInvoker(wrapper, k);
            });
        }

        public object Invoke(object target, params object[] args)
        {
            return _func(target, args);
        }

        #endregion

        #region Helpers

        private static Func<object, object[], object> CreateMethodWrapper(Type type, MethodInfo method, bool isDelegate)
        {
            CreateParamsExpressions(method, out ParameterExpression argsExp, out Expression[] paramsExps);

            var targetExp = Expression.Parameter(typeof(object), "target");
            var castTargetExp = Expression.Convert(targetExp, type);
            var invokeExp = isDelegate
                ? (Expression)Expression.Invoke(castTargetExp, paramsExps)
                : Expression.Call(castTargetExp, method, paramsExps);

            LambdaExpression lambdaExp;

            if (method.ReturnType != typeof(void))
            {
                var resultExp = Expression.Convert(invokeExp, typeof(object));
                lambdaExp = Expression.Lambda(resultExp, targetExp, argsExp);
            }
            else
            {
                var constExp = Expression.Constant(null, typeof(object));
                var blockExp = Expression.Block(invokeExp, constExp);
                lambdaExp = Expression.Lambda(blockExp, targetExp, argsExp);
            }

            var lambda = lambdaExp.Compile();
            return (Func<object, object[], object>)lambda;
        }

        private static void CreateParamsExpressions(MethodBase method, out ParameterExpression argsExp, out Expression[] paramsExps)
        {
            var parameters = method.GetParameters().Select(parameter => parameter.ParameterType).ToList();

            argsExp = Expression.Parameter(typeof(object[]), "args");
            paramsExps = new Expression[parameters.Count];

            for (var i = 0; i < parameters.Count; i++)
            {
                var constExp = Expression.Constant(i, typeof(int));
                var argExp = Expression.ArrayIndex(argsExp, constExp);
                paramsExps[i] = Expression.Convert(argExp, parameters[i]);
            }
        }

        private static Func<object, object[], object> CreatePropertyWrapper(Type type, string propertyName)
        {
            var property = type.GetRuntimeProperty(propertyName);
            var targetExp = Expression.Parameter(typeof(object), "target");
            var argsExp = Expression.Parameter(typeof(object[]), "args");
            var castArgExp = Expression.Convert(targetExp, type);
            var propExp = Expression.Property(castArgExp, property);
            var castPropExp = Expression.Convert(propExp, typeof(object));
            var lambdaExp = Expression.Lambda(castPropExp, targetExp, argsExp);
            var lambda = lambdaExp.Compile();
            return (Func<object, object[], object>)lambda;
        }

        #endregion
    }
}
