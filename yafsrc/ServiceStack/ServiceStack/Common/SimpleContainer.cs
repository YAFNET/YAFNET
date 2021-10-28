// ***********************************************************************
// <copyright file="SimpleContainer.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ServiceStack.Configuration;

namespace ServiceStack
{
    using ServiceStack.Text;

    /// <summary>
    /// Class SimpleContainer.
    /// Implements the <see cref="ServiceStack.IContainer" />
    /// Implements the <see cref="ServiceStack.Configuration.IResolver" />
    /// </summary>
    /// <seealso cref="ServiceStack.IContainer" />
    /// <seealso cref="ServiceStack.Configuration.IResolver" />
    public class SimpleContainer : IContainer, IResolver
    {
        /// <summary>
        /// Gets the ignore types named.
        /// </summary>
        /// <value>The ignore types named.</value>
        public HashSet<string> IgnoreTypesNamed { get; } = new();

        /// <summary>
        /// The instance cache
        /// </summary>
        protected readonly ConcurrentDictionary<Type, object> InstanceCache = new();

        /// <summary>
        /// The factory
        /// </summary>
        protected readonly ConcurrentDictionary<Type, Func<object>> Factory = new();

        /// <summary>
        /// Resolves the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>object.</returns>
        public object Resolve(Type type)
        {
            Factory.TryGetValue(type, out Func<object> fn);
            return fn?.Invoke();
        }

        /// <summary>
        /// Existses the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>bool.</returns>
        public bool Exists(Type type) => Factory.ContainsKey(type);

        /// <summary>
        /// Requireds the resolve.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="ownerType">Type of the owner.</param>
        /// <returns>object.</returns>
        /// <exception cref="Exception">$"Required Type of '{type.Name}' in '{ownerType.Name}' constructor was not registered in '{GetType().Name}'</exception>
        public object RequiredResolve(Type type, Type ownerType)
        {
            var instance = Resolve(type);
            if (instance == null)
                throw new Exception($"Required Type of '{type.Name}' in '{ownerType.Name}' constructor was not registered in '{GetType().Name}'");

            return instance;
        }

        /// <summary>
        /// Adds the singleton.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="factory">The factory.</param>
        /// <returns>ServiceStack.IContainer.</returns>
        public IContainer AddSingleton(Type type, Func<object> factory)
        {
            Factory[type] = () => InstanceCache.GetOrAdd(type, factory());
            return this;
        }

        /// <summary>
        /// Adds the transient.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="factory">The factory.</param>
        /// <returns>ServiceStack.IContainer.</returns>
        public IContainer AddTransient(Type type, Func<object> factory)
        {
            Factory[type] = factory;
            return this;
        }

        /// <summary>
        /// Tries the resolve.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>T.</returns>
        public T TryResolve<T>() => (T)Resolve(typeof(T));

        /// <summary>
        /// Includes the property.
        /// </summary>
        /// <param name="pi">The pi.</param>
        /// <returns>bool.</returns>
        protected virtual bool IncludeProperty(PropertyInfo pi)
        {
            return pi.CanWrite
                   && !pi.PropertyType.IsValueType
                   && pi.PropertyType != typeof(string)
                   && pi.PropertyType != typeof(object)
                   && !IgnoreTypesNamed.Contains(pi.PropertyType.FullName);
        }

        /// <summary>
        /// Resolves the best constructor.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>System.Reflection.ConstructorInfo.</returns>
        protected virtual ConstructorInfo ResolveBestConstructor(Type type)
        {
            return type.GetConstructors()
                .OrderByDescending(x => x.GetParameters().Length) //choose constructor with most params
                .FirstOrDefault(ctor => !ctor.IsStatic);
        }

        /// <summary>
        /// Creates the factory.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>System.Func&lt;object&gt;.</returns>
        /// <exception cref="Exception">$"Constructor not found for Type '{type.Name}</exception>
        public Func<object> CreateFactory(Type type)
        {
            var containerParam = Expression.Constant(this);
            var memberBindings = type.GetPublicProperties()
                .Where(IncludeProperty)
                .Select(x =>
                    Expression.Bind
                    (
                        x,
                        Expression.TypeAs(Expression.Call(containerParam, GetType().GetMethod(nameof(Resolve)), Expression.Constant(x.PropertyType)), x.PropertyType)
                    )
                ).ToArray();

            var ctorWithMostParameters = ResolveBestConstructor(type);
            if (ctorWithMostParameters == null)
                throw new Exception($"Constructor not found for Type '{type.Name}");

            var constructorParameterInfos = ctorWithMostParameters.GetParameters();
            var regParams = constructorParameterInfos
                .Select(x =>
                    Expression.TypeAs(Expression.Call(containerParam, GetType().GetMethod(nameof(RequiredResolve)), Expression.Constant(x.ParameterType), Expression.Constant(type)), x.ParameterType)
                );

            return Expression.Lambda<Func<object>>
            (
                Expression.TypeAs(Expression.MemberInit
                (
                    Expression.New(ctorWithMostParameters, regParams.ToArray()),
                    memberBindings
                ), typeof(object))
            ).Compile();
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            var hold = InstanceCache;
            InstanceCache.Clear();
            foreach (var instance in hold)
            {
                try
                {
                    using (instance.Value as IDisposable) { }
                }
                catch { /* ignored */ }
            }
        }
    }

    /// <summary>
    /// Class ContainerExtensions.
    /// </summary>
    public static class ContainerExtensions
    {
        /// <summary>
        /// Resolves the specified container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container">The container.</param>
        /// <returns>T.</returns>
        /// <exception cref="Exception">$"Error trying to resolve Service '{typeof(T).Name}' or one of its autowired dependencies.</exception>
        public static T Resolve<T>(this IResolver container)
        {
            var ret = container.TryResolve<T>();
            if (ret == null)
                throw new Exception($"Error trying to resolve Service '{typeof(T).Name}' or one of its autowired dependencies.");

            return ret;
        }

        /// <summary>
        /// Resolves the specified container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container">The container.</param>
        /// <returns>T.</returns>
        public static T Resolve<T>(this IContainer container) =>
            (T)container.Resolve(typeof(T));

        /// <summary>
        /// Existses the specified container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container">The container.</param>
        /// <returns>bool.</returns>
        public static bool Exists<T>(this IContainer container) => container.Exists(typeof(T));

        /// <summary>
        /// Adds the transient.
        /// </summary>
        /// <typeparam name="TService">The type of the t service.</typeparam>
        /// <param name="container">The container.</param>
        /// <returns>ServiceStack.IContainer.</returns>
        public static IContainer AddTransient<TService>(this IContainer container) =>
            container.AddTransient(typeof(TService), container.CreateFactory(typeof(TService)));

        /// <summary>
        /// Adds the transient.
        /// </summary>
        /// <typeparam name="TService">The type of the t service.</typeparam>
        /// <param name="container">The container.</param>
        /// <param name="factory">The factory.</param>
        /// <returns>ServiceStack.IContainer.</returns>
        public static IContainer AddTransient<TService>(this IContainer container, Func<TService> factory) =>
            container.AddTransient(typeof(TService), () => factory());

        /// <summary>
        /// Adds the transient.
        /// </summary>
        /// <typeparam name="TService">The type of the t service.</typeparam>
        /// <typeparam name="TImpl">The type of the t implementation.</typeparam>
        /// <param name="container">The container.</param>
        /// <returns>ServiceStack.IContainer.</returns>
        public static IContainer AddTransient<TService, TImpl>(this IContainer container) where TImpl : TService =>
            container.AddTransient(typeof(TService), container.CreateFactory(typeof(TImpl)));

        /// <summary>
        /// Adds the transient.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="type">The type.</param>
        /// <returns>ServiceStack.IContainer.</returns>
        public static IContainer AddTransient(this IContainer container, Type type) =>
            container.AddTransient(type, container.CreateFactory(type));

        /// <summary>
        /// Adds the singleton.
        /// </summary>
        /// <typeparam name="TService">The type of the t service.</typeparam>
        /// <param name="container">The container.</param>
        /// <returns>ServiceStack.IContainer.</returns>
        public static IContainer AddSingleton<TService>(this IContainer container) =>
            container.AddSingleton(typeof(TService), container.CreateFactory(typeof(TService)));

        /// <summary>
        /// Adds the singleton.
        /// </summary>
        /// <typeparam name="TService">The type of the t service.</typeparam>
        /// <param name="container">The container.</param>
        /// <param name="factory">The factory.</param>
        /// <returns>ServiceStack.IContainer.</returns>
        public static IContainer AddSingleton<TService>(this IContainer container, Func<TService> factory) =>
            container.AddSingleton(typeof(TService), () => factory());

        /// <summary>
        /// Adds the singleton.
        /// </summary>
        /// <typeparam name="TService">The type of the t service.</typeparam>
        /// <typeparam name="TImpl">The type of the t implementation.</typeparam>
        /// <param name="container">The container.</param>
        /// <returns>ServiceStack.IContainer.</returns>
        public static IContainer AddSingleton<TService, TImpl>(this IContainer container) where TImpl : TService =>
            container.AddSingleton(typeof(TService), container.CreateFactory(typeof(TImpl)));

        /// <summary>
        /// Adds the singleton.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="type">The type.</param>
        /// <returns>ServiceStack.IContainer.</returns>
        public static IContainer AddSingleton(this IContainer container, Type type) =>
            container.AddSingleton(type, container.CreateFactory(type));
    }
}