// ***********************************************************************
// <copyright file="IdUtils.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Linq;
using System.Reflection;
using ServiceStack.Model;
using ServiceStack.OrmLite.Base.Common;
using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack;

/// <summary>
/// Class IdUtils.
/// </summary>
/// <typeparam name="T"></typeparam>
public static class IdUtils<T>
{
    /// <summary>
    /// The can get identifier
    /// </summary>
    static internal GetMemberDelegate<T> CanGetId;

    /// <summary>
    /// Initializes static members of the <see cref="IdUtils{T}" /> class.
    /// </summary>
    static IdUtils()
    {
        var hasIdInterfaces = typeof(T).GetTypeInfo().ImplementedInterfaces.Where(t => t.GetTypeInfo().IsGenericType
            && t.GetTypeInfo().GetGenericTypeDefinition() == typeof(IHasId<>)).ToArray();

        if (hasIdInterfaces.Length > 0)
        {
            CanGetId = HasId<T>.GetId;
            return;
        }

        if (typeof(T).IsClass || typeof(T).IsInterface)
        {
            foreach (var pi in typeof(T).GetPublicProperties()
                         .Where(pi => pi.AllAttributes<Attribute>()
                             .Exists(attr => attr.GetType().Name == "PrimaryKeyAttribute")))
            {
                CanGetId = pi.CreateGetter<T>();
                return;
            }

            var piId = typeof(T).GetIdProperty();
            if (piId?.GetGetMethod(nonPublic: true) != null)
            {
                CanGetId = HasPropertyId<T>.GetId;
                return;
            }
        }

        if (typeof(T) == typeof(object))
        {
            CanGetId = x =>
                {
                    var piId = x.GetType().GetIdProperty();
                    if (piId?.GetGetMethod(nonPublic: true) != null)
                    {
                        return x.GetObjectId();
                    }

                    return x.GetHashCode();
                };
            return;
        }

        CanGetId = x => x.GetHashCode();
    }

    /// <summary>
    /// Gets the identifier.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>System.Object.</returns>
    public static object GetId(T entity)
    {
        return CanGetId(entity);
    }
}

/// <summary>
/// Class HasPropertyId.
/// </summary>
/// <typeparam name="TEntity">The type of the t entity.</typeparam>
static internal class HasPropertyId<TEntity>
{
    /// <summary>
    /// The get identifier function
    /// </summary>
    private readonly static GetMemberDelegate<TEntity> GetIdFn;

    /// <summary>
    /// Initializes static members of the <see cref="HasPropertyId{TEntity}" /> class.
    /// </summary>
    static HasPropertyId()
    {
        var pi = typeof(TEntity).GetIdProperty();
        GetIdFn = pi.CreateGetter<TEntity>();
    }

    /// <summary>
    /// Gets the identifier.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>System.Object.</returns>
    public static object GetId(TEntity entity)
    {
        return GetIdFn(entity);
    }
}

/// <summary>
/// Class HasId.
/// </summary>
/// <typeparam name="TEntity">The type of the t entity.</typeparam>
static internal class HasId<TEntity>
{
    /// <summary>
    /// The get identifier function
    /// </summary>
    private readonly static Func<TEntity, object> GetIdFn;

    /// <summary>
    /// Initializes static members of the <see cref="HasId{TEntity}" /> class.
    /// </summary>
    static HasId()
    {
        var hasIdInterfaces = typeof(TEntity).GetTypeInfo().ImplementedInterfaces.Where(t => t.GetTypeInfo().IsGenericType
            && t.GetTypeInfo().GetGenericTypeDefinition() == typeof(IHasId<>)).ToArray();
        var genericArg = hasIdInterfaces[0].GetGenericArguments()[0];
        var genericType = typeof(HasIdGetter<,>).MakeGenericType(typeof(TEntity), genericArg);

        var oInstanceParam = System.Linq.Expressions.Expression.Parameter(typeof(TEntity), "oInstanceParam");
        var exprCallStaticMethod = System.Linq.Expressions.Expression.Call
        (
            genericType,
            "GetId",
            TypeConstants.EmptyTypeArray,
            oInstanceParam
        );
        GetIdFn = System.Linq.Expressions.Expression.Lambda<Func<TEntity, object>>
        (
            exprCallStaticMethod,
            oInstanceParam
        ).Compile();
    }

    /// <summary>
    /// Gets the identifier.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>System.Object.</returns>
    public static object GetId(TEntity entity)
    {
        return GetIdFn(entity);
    }
}

/// <summary>
/// Class HasIdGetter.
/// </summary>
/// <typeparam name="TEntity">The type of the t entity.</typeparam>
/// <typeparam name="TId">The type of the t identifier.</typeparam>
internal class HasIdGetter<TEntity, TId>
    where TEntity : IHasId<TId>
{
    /// <summary>
    /// Gets the identifier.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>System.Object.</returns>
    public static object GetId(TEntity entity)
    {
        return entity.Id;
    }
}

/// <summary>
/// Class IdUtils.
/// </summary>
public static class IdUtils
{
    /// <summary>
    /// The identifier field
    /// </summary>
    public const string IdField = "Id";

    /// <summary>
    /// Gets the object identifier.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>System.Object.</returns>
    public static object GetObjectId(this object entity)
    {
        return entity.GetType().GetIdProperty().GetGetMethod(nonPublic: true).Invoke(entity, TypeConstants.EmptyObjectArray);
    }

    /// <param name="entity">The entity.</param>
    /// <typeparam name="T"></typeparam>
    extension<T>(T entity)
    {
        /// <summary>
        /// Converts to id.
        /// </summary>
        /// <returns>System.Object.</returns>
        public object ToId()
        {
            return entity.GetId();
        }

        /// <summary>
        /// Converts to urn.
        /// </summary>
        /// <returns>System.String.</returns>
        public string ToUrn()
        {
            return entity.CreateUrn();
        }
    }

    /// <summary>
    /// Converts to urn.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="id">The identifier.</param>
    /// <returns>System.String.</returns>
    public static string ToUrn<T>(this object id)
    {
        return CreateUrn<T>(id);
    }

    /// <summary>
    /// Gets the identifier.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity">The entity.</param>
    /// <returns>System.Object.</returns>
    public static object GetId<T>(this T entity)
    {
        return IdUtils<T>.GetId(entity);
    }

    /// <summary>
    /// Creates the urn.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="id">The identifier.</param>
    /// <returns>System.String.</returns>
    public static string CreateUrn<T>(object id)
    {
        return $"urn:{typeof(T).Name.ToLowerInvariant()}:{id}";
    }

    /// <summary>
    /// Creates the urn.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="id">The identifier.</param>
    /// <returns>System.String.</returns>
    public static string CreateUrn(Type type, object id)
    {
        return $"urn:{type.Name.ToLowerInvariant()}:{id}";
    }

    /// <summary>
    /// Creates the urn.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="id">The identifier.</param>
    /// <returns>System.String.</returns>
    public static string CreateUrn(string type, object id)
    {
        return $"urn:{type.ToLowerInvariant()}:{id}";
    }

    /// <summary>
    /// Creates the urn.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity">The entity.</param>
    /// <returns>System.String.</returns>
    public static string CreateUrn<T>(this T entity)
    {
        var id = GetId(entity);
        return $"urn:{typeof(T).Name.ToLowerInvariant()}:{id}";
    }

    /// <summary>
    /// Gets the identifier property.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>PropertyInfo.</returns>
    public static PropertyInfo GetIdProperty(this Type type)
    {
        return Array.Find(type.GetProperties(),pi => string.Equals(IdField, pi.Name, StringComparison.OrdinalIgnoreCase));
    }

}