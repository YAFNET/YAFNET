// ***********************************************************************
// <copyright file="DelegateFactory.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ServiceStack.Reflection
{
    /// <summary>
    /// Class DelegateFactory.
    /// </summary>
    public static class DelegateFactory
    {
        /*
         *	MethodInfo method = typeof(String).GetMethod("StartsWith", new[] { typeof(string) });  
            LateBoundMethod callback = DelegateFactory.Create(method);  
  
            string foo = "this is a test";  
            bool result = (bool) callback(foo, new[] { "this" });  
  
            result.ShouldBeTrue();  
         */
        /// <summary>
        /// Delegate LateBoundMethod
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>System.Object.</returns>
        public delegate object LateBoundMethod(object target, object[] arguments);

        /// <summary>
        /// Creates the specified method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>LateBoundMethod.</returns>
        public static LateBoundMethod Create(MethodInfo method)
        {
            ParameterExpression instanceParameter = Expression.Parameter(typeof(object), "target");
            ParameterExpression argumentsParameter = Expression.Parameter(typeof(object[]), "arguments");

            MethodCallExpression call = Expression.Call(
                Expression.Convert(instanceParameter, method.DeclaringType),
                method,
                CreateParameterExpressions(method, argumentsParameter));

            Expression<LateBoundMethod> lambda = Expression.Lambda<LateBoundMethod>(
                Expression.Convert(call, typeof(object)),
                instanceParameter,
                argumentsParameter);

            return lambda.Compile();
        }

        /// <summary>
        /// Creates the parameter expressions.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="argumentsParameter">The arguments parameter.</param>
        /// <returns>Expression[].</returns>
        private static Expression[] CreateParameterExpressions(MethodInfo method, Expression argumentsParameter)
        {
            return method.GetParameters().Select((parameter, index) =>
                Expression.Convert(
                    Expression.ArrayIndex(argumentsParameter, Expression.Constant(index)),
                    parameter.ParameterType)).ToArray();
        }


        /// <summary>
        /// Delegate LateBoundVoid
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="arguments">The arguments.</param>
        public delegate void LateBoundVoid(object target, object[] arguments);

        /// <summary>
        /// Creates the void.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>LateBoundVoid.</returns>
        public static LateBoundVoid CreateVoid(MethodInfo method)
        {
            ParameterExpression instanceParameter = Expression.Parameter(typeof(object), "target");
            ParameterExpression argumentsParameter = Expression.Parameter(typeof(object[]), "arguments");

            MethodCallExpression call = Expression.Call(
                Expression.Convert(instanceParameter, method.DeclaringType),
                method,
                CreateParameterExpressions(method, argumentsParameter));

            var lambda = Expression.Lambda<LateBoundVoid>(
                Expression.Convert(call, method.ReturnParameter.ParameterType),
                instanceParameter,
                argumentsParameter);

            return lambda.Compile();
        }
    }
}