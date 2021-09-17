// ***********************************************************************
// <copyright file="TypeFields.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using ServiceStack.Text;

namespace ServiceStack
{
    /// <summary>
    /// Class FieldAccessor.
    /// </summary>
    public class FieldAccessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldAccessor"/> class.
        /// </summary>
        /// <param name="fieldInfo">The field information.</param>
        /// <param name="publicGetter">The public getter.</param>
        /// <param name="publicSetter">The public setter.</param>
        /// <param name="publicSetterRef">The public setter reference.</param>
        public FieldAccessor(
            FieldInfo fieldInfo,
            GetMemberDelegate publicGetter,
            SetMemberDelegate publicSetter,
            SetMemberRefDelegate publicSetterRef)
        {
            FieldInfo = fieldInfo;
            PublicGetter = publicGetter;
            PublicSetter = publicSetter;
            PublicSetterRef = publicSetterRef;
        }

        /// <summary>
        /// Gets the field information.
        /// </summary>
        /// <value>The field information.</value>
        public FieldInfo FieldInfo { get; }

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

        /// <summary>
        /// Gets the public setter reference.
        /// </summary>
        /// <value>The public setter reference.</value>
        public SetMemberRefDelegate PublicSetterRef { get; }
    }

    /// <summary>
    /// Class TypeFields.
    /// Implements the <see cref="ServiceStack.TypeFields" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="ServiceStack.TypeFields" />
    public class TypeFields<T> : TypeFields
    {
        /// <summary>
        /// The instance
        /// </summary>
        public static readonly TypeFields<T> Instance = new();

        /// <summary>
        /// Initializes static members of the <see cref="TypeFields{T}"/> class.
        /// </summary>
        static TypeFields()
        {
            Instance.Type = typeof(T);
            Instance.PublicFieldInfos = typeof(T).GetPublicFields();
            foreach (var fi in Instance.PublicFieldInfos)
            {
                try
                {
                    var fnRef = fi.SetExpressionRef<T>();
                    Instance.FieldsMap[fi.Name] = new FieldAccessor(
                        fi,
                        ReflectionOptimizer.Instance.CreateGetter(fi),
                        ReflectionOptimizer.Instance.CreateSetter(fi),
                        delegate (ref object instance, object arg)
                        {
                            var valueInstance = (T)instance;
                            fnRef(ref valueInstance, arg);
                            instance = valueInstance;
                        });
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
        /// <returns>FieldAccessor.</returns>
        public new static FieldAccessor GetAccessor(string propertyName)
        {
            return Instance.FieldsMap.TryGetValue(propertyName, out FieldAccessor info)
                ? info
                : null;
        }
    }

    /// <summary>
    /// Class TypeFields.
    /// Implements the <see cref="ServiceStack.TypeFields" />
    /// </summary>
    /// <seealso cref="ServiceStack.TypeFields" />
    public abstract class TypeFields
    {
        /// <summary>
        /// The cache map
        /// </summary>
        static Dictionary<Type, TypeFields> CacheMap = new();

        /// <summary>
        /// The factory type
        /// </summary>
        public static Type FactoryType = typeof(TypeFields<>);

        /// <summary>
        /// Gets the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>TypeFields.</returns>
        public static TypeFields Get(Type type)
        {
            if (CacheMap.TryGetValue(type, out TypeFields value))
                return value;

            var genericType = FactoryType.MakeGenericType(type);
            var instanceFi = genericType.GetPublicStaticField("Instance");
            var instance = (TypeFields)instanceFi.GetValue(null);

            Dictionary<Type, TypeFields> snapshot, newCache;
            do
            {
                snapshot = CacheMap;
                newCache = new Dictionary<Type, TypeFields>(CacheMap)
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
        /// <returns>FieldAccessor.</returns>
        public FieldAccessor GetAccessor(string propertyName)
        {
            return FieldsMap.TryGetValue(propertyName, out FieldAccessor info)
                ? info
                : null;
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public Type Type { get; protected set; }

        /// <summary>
        /// The fields map
        /// </summary>
        public readonly Dictionary<string, FieldAccessor> FieldsMap =
            new(PclExport.Instance.InvariantComparerIgnoreCase);

        /// <summary>
        /// Gets or sets the public field infos.
        /// </summary>
        /// <value>The public field infos.</value>
        public FieldInfo[] PublicFieldInfos { get; protected set; }

        /// <summary>
        /// Gets the public field.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>FieldInfo.</returns>
        public virtual FieldInfo GetPublicField(string name)
        {
            foreach (var fi in PublicFieldInfos)
            {
                if (fi.Name == name)
                    return fi;
            }
            return null;
        }

        /// <summary>
        /// Gets the public getter.
        /// </summary>
        /// <param name="fi">The fi.</param>
        /// <returns>GetMemberDelegate.</returns>
        public virtual GetMemberDelegate GetPublicGetter(FieldInfo fi) => GetPublicGetter(fi?.Name);

        /// <summary>
        /// Gets the public getter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>GetMemberDelegate.</returns>
        public virtual GetMemberDelegate GetPublicGetter(string name)
        {
            if (name == null)
                return null;

            return FieldsMap.TryGetValue(name, out FieldAccessor info)
                ? info.PublicGetter
                : null;
        }

        /// <summary>
        /// Gets the public setter.
        /// </summary>
        /// <param name="fi">The fi.</param>
        /// <returns>SetMemberDelegate.</returns>
        public virtual SetMemberDelegate GetPublicSetter(FieldInfo fi) => GetPublicSetter(fi?.Name);

        /// <summary>
        /// Gets the public setter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>SetMemberDelegate.</returns>
        public virtual SetMemberDelegate GetPublicSetter(string name)
        {
            if (name == null)
                return null;

            return FieldsMap.TryGetValue(name, out FieldAccessor info)
                ? info.PublicSetter
                : null;
        }

        /// <summary>
        /// Gets the public setter reference.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>SetMemberRefDelegate.</returns>
        public virtual SetMemberRefDelegate GetPublicSetterRef(string name)
        {
            if (name == null)
                return null;

            return FieldsMap.TryGetValue(name, out FieldAccessor info)
                ? info.PublicSetterRef
                : null;
        }
    }

    /// <summary>
    /// Class FieldInvoker.
    /// </summary>
    public static class FieldInvoker
    {
        /// <summary>
        /// Creates the getter.
        /// </summary>
        /// <param name="fieldInfo">The field information.</param>
        /// <returns>GetMemberDelegate.</returns>
        public static GetMemberDelegate CreateGetter(this FieldInfo fieldInfo) =>
            ReflectionOptimizer.Instance.CreateGetter(fieldInfo);

        /// <summary>
        /// Creates the getter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldInfo">The field information.</param>
        /// <returns>GetMemberDelegate&lt;T&gt;.</returns>
        public static GetMemberDelegate<T> CreateGetter<T>(this FieldInfo fieldInfo) =>
            ReflectionOptimizer.Instance.CreateGetter<T>(fieldInfo);

        /// <summary>
        /// Creates the setter.
        /// </summary>
        /// <param name="fieldInfo">The field information.</param>
        /// <returns>SetMemberDelegate.</returns>
        public static SetMemberDelegate CreateSetter(this FieldInfo fieldInfo) =>
            ReflectionOptimizer.Instance.CreateSetter(fieldInfo);

        /// <summary>
        /// Creates the setter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldInfo">The field information.</param>
        /// <returns>SetMemberDelegate&lt;T&gt;.</returns>
        public static SetMemberDelegate<T> CreateSetter<T>(this FieldInfo fieldInfo) =>
            ReflectionOptimizer.Instance.CreateSetter<T>(fieldInfo);

        /// <summary>
        /// Sets the expression reference.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldInfo">The field information.</param>
        /// <returns>SetMemberRefDelegate&lt;T&gt;.</returns>
        public static SetMemberRefDelegate<T> SetExpressionRef<T>(this FieldInfo fieldInfo) =>
            ReflectionOptimizer.Instance.CreateSetterRef<T>(fieldInfo);
    }
}
