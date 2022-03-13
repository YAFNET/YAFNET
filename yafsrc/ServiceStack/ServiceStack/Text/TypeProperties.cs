// ***********************************************************************
// <copyright file="TypeProperties.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.Text
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    /// <summary>
    /// Class PropertyAccessor.
    /// </summary>
    public class PropertyAccessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyAccessor"/> class.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="publicGetter">The public getter.</param>
        /// <param name="publicSetter">The public setter.</param>
        public PropertyAccessor(
            PropertyInfo propertyInfo,
            GetMemberDelegate publicGetter,
            SetMemberDelegate publicSetter)
        {
            PropertyInfo = propertyInfo;
            PublicGetter = publicGetter;
            PublicSetter = publicSetter;
        }

        /// <summary>
        /// Gets the property information.
        /// </summary>
        /// <value>The property information.</value>
        public PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// Gets the public getter.
        /// </summary>
        /// <value>The public getter.</value>
        public GetMemberDelegate PublicGetter { get; }

        /// <summary>
        /// Gets the public setter.
        /// </summary>
        /// <value>The public setter.</value>
        public SetMemberDelegate PublicSetter { get; }
    }

    /// <summary>
    /// Class TypeProperties.
    /// Implements the <see cref="TypeProperties" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="TypeProperties" />
    public class TypeProperties<T> : TypeProperties
    {
        /// <summary>
        /// The instance
        /// </summary>
        public static readonly TypeProperties<T> Instance = new();

        /// <summary>
        /// Initializes static members of the <see cref="TypeProperties{T}"/> class.
        /// </summary>
        static TypeProperties()
        {
            Instance.Type = typeof(T);
            Instance.PublicPropertyInfos = typeof(T).GetPublicProperties();
            foreach (var pi in Instance.PublicPropertyInfos)
            {
                try
                {
                    Instance.PropertyMap[pi.Name] = new PropertyAccessor(
                        pi,
                        ReflectionOptimizer.Instance.CreateGetter(pi),
                        ReflectionOptimizer.Instance.CreateSetter(pi)
                    );
                }
                catch (Exception ex)
                {
                    Tracer.Instance.WriteError(ex);
                }
            }
        }

        /// <summary>
        /// Gets the accessor.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>PropertyAccessor.</returns>
        public new static PropertyAccessor GetAccessor(string propertyName)
        {
            return Instance.PropertyMap.TryGetValue(propertyName, out PropertyAccessor info)
                ? info
                : null;
        }
    }

    /// <summary>
    /// Class TypeProperties.
    /// Implements the <see cref="TypeProperties" />
    /// </summary>
    /// <seealso cref="TypeProperties" />
    public abstract class TypeProperties
    {
        /// <summary>
        /// The cache map
        /// </summary>
        static Dictionary<Type, TypeProperties> CacheMap = new();

        /// <summary>
        /// The factory type
        /// </summary>
        public static readonly Type FactoryType = typeof(TypeProperties<>);

        /// <summary>
        /// Gets the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>TypeProperties.</returns>
        public static TypeProperties Get(Type type)
        {
            if (CacheMap.TryGetValue(type, out TypeProperties value))
                return value;

            var genericType = FactoryType.MakeGenericType(type);
            var instanceFi = genericType.GetPublicStaticField("Instance");
            var instance = (TypeProperties)instanceFi.GetValue(null);

            Dictionary<Type, TypeProperties> snapshot, newCache;
            do
            {
                snapshot = CacheMap;
                newCache = new Dictionary<Type, TypeProperties>(CacheMap)
                {
                    [type] = instance
                };
            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref CacheMap, newCache, snapshot), snapshot));

            return instance;
        }

        /// <summary>
        /// Gets the accessor.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>PropertyAccessor.</returns>
        public PropertyAccessor GetAccessor(string propertyName)
        {
            return PropertyMap.TryGetValue(propertyName, out PropertyAccessor info)
                ? info
                : null;
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public Type Type { get; protected set; }

        /// <summary>
        /// The property map
        /// </summary>
        public Dictionary<string, PropertyAccessor> PropertyMap = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets or sets the public property infos.
        /// </summary>
        /// <value>The public property infos.</value>
        public PropertyInfo[] PublicPropertyInfos { get; protected set; }

        /// <summary>
        /// Gets the public property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>PropertyInfo.</returns>
        public PropertyInfo GetPublicProperty(string name)
        {
            return this.PublicPropertyInfos.FirstOrDefault(pi => pi.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets the public getter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>GetMemberDelegate.</returns>
        public GetMemberDelegate GetPublicGetter(string name)
        {
            if (name == null)
                return null;

            return PropertyMap.TryGetValue(name, out PropertyAccessor info)
                ? info.PublicGetter
                : null;
        }

        /// <summary>
        /// Gets the public setter.
        /// </summary>
        /// <param name="pi">The pi.</param>
        /// <returns>SetMemberDelegate.</returns>
        public SetMemberDelegate GetPublicSetter(PropertyInfo pi) => GetPublicSetter(pi?.Name);

        /// <summary>
        /// Gets the public setter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>SetMemberDelegate.</returns>
        public SetMemberDelegate GetPublicSetter(string name)
        {
            if (name == null)
                return null;

            return PropertyMap.TryGetValue(name, out PropertyAccessor info)
                ? info.PublicSetter
                : null;
        }
    }

    /// <summary>
    /// Class PropertyInvoker.
    /// </summary>
    public static class PropertyInvoker
    {
        /// <summary>
        /// Creates the getter.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <returns>GetMemberDelegate.</returns>
        public static GetMemberDelegate CreateGetter(this PropertyInfo propertyInfo) =>
            ReflectionOptimizer.Instance.CreateGetter(propertyInfo);

        /// <summary>
        /// Creates the getter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyInfo">The property information.</param>
        /// <returns>GetMemberDelegate&lt;T&gt;.</returns>
        public static GetMemberDelegate<T> CreateGetter<T>(this PropertyInfo propertyInfo) =>
            ReflectionOptimizer.Instance.CreateGetter<T>(propertyInfo);

        /// <summary>
        /// Creates the setter.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <returns>SetMemberDelegate.</returns>
        public static SetMemberDelegate CreateSetter(this PropertyInfo propertyInfo) =>
            ReflectionOptimizer.Instance.CreateSetter(propertyInfo);

        /// <summary>
        /// Creates the setter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyInfo">The property information.</param>
        /// <returns>SetMemberDelegate&lt;T&gt;.</returns>
        public static SetMemberDelegate<T> CreateSetter<T>(this PropertyInfo propertyInfo) =>
            ReflectionOptimizer.Instance.CreateSetter<T>(propertyInfo);
    }
}

