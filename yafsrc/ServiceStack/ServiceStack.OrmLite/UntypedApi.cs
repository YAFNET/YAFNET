// ***********************************************************************
// <copyright file="UntypedApi.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using ServiceStack.Text;

    /// <summary>
    /// Class UntypedApiExtensions.
    /// </summary>
    public static class UntypedApiExtensions
    {
        /// <summary>
        /// The untyped API map
        /// </summary>
        static readonly ConcurrentDictionary<Type, Type> untypedApiMap =
            new();

        /// <summary>
        /// Creates the typed API.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <param name="forType">For type.</param>
        /// <returns>IUntypedApi.</returns>
        public static IUntypedApi CreateTypedApi(this IDbConnection db, Type forType)
        {
            var genericType = untypedApiMap.GetOrAdd(forType, key => typeof(UntypedApi<>).GetCachedGenericType(key));
            var unTypedApi = genericType.CreateInstance<IUntypedApi>();
            unTypedApi.Db = db;
            return unTypedApi;
        }

        /// <summary>
        /// Creates the typed API.
        /// </summary>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="forType">For type.</param>
        /// <returns>IUntypedApi.</returns>
        public static IUntypedApi CreateTypedApi(this IDbCommand dbCmd, Type forType)
        {
            var genericType = untypedApiMap.GetOrAdd(forType, key => typeof(UntypedApi<>).GetCachedGenericType(key));
            var unTypedApi = genericType.CreateInstance<IUntypedApi>();
            unTypedApi.DbCmd = dbCmd;
            return unTypedApi;
        }

        /// <summary>
        /// Creates the typed API.
        /// </summary>
        /// <param name="forType">For type.</param>
        /// <returns>IUntypedApi.</returns>
        public static IUntypedApi CreateTypedApi(this Type forType)
        {
            var genericType = untypedApiMap.GetOrAdd(forType, key => typeof(UntypedApi<>).GetCachedGenericType(key));
            var unTypedApi = genericType.CreateInstance<IUntypedApi>();
            return unTypedApi;
        }
    }

    /// <summary>
    /// Class UntypedApi.
    /// Implements the <see cref="ServiceStack.OrmLite.IUntypedApi" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="ServiceStack.OrmLite.IUntypedApi" />
    public class UntypedApi<T> : IUntypedApi
    {
        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        /// <value>The database.</value>
        public IDbConnection Db { get; set; }
        /// <summary>
        /// Gets or sets the database command.
        /// </summary>
        /// <value>The database command.</value>
        public IDbCommand DbCmd { get; set; }

        /// <summary>
        /// Executes the specified filter.
        /// </summary>
        /// <typeparam name="TReturn">The type of the t return.</typeparam>
        /// <param name="filter">The filter.</param>
        /// <returns>TReturn.</returns>
        public TReturn Exec<TReturn>(Func<IDbCommand, TReturn> filter)
        {
            return DbCmd != null ? filter(DbCmd) : Db.Exec(filter);
        }

        public Task<TReturn> Exec<TReturn>(Func<IDbCommand, Task<TReturn>> filter)
        {
            return DbCmd != null ? filter(DbCmd) : Db.Exec(filter);
        }

        /// <summary>
        /// Executes the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public void Exec(Action<IDbCommand> filter)
        {
            if (DbCmd != null)
                filter(DbCmd);
            else
                Db.Exec(filter);
        }

        /// <summary>
        /// Saves all.
        /// </summary>
        /// <param name="objs">The objs.</param>
        /// <returns>System.Int32.</returns>
        public int SaveAll(IEnumerable objs)
        {
            return Exec(dbCmd => dbCmd.SaveAll((IEnumerable<T>)objs));
        }

        /// <summary>
        /// Saves the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Save(object obj)
        {
            return Exec(dbCmd => dbCmd.Save((T)obj));
        }

#if ASYNC
        /// <summary>
        /// Saves all asynchronous.
        /// </summary>
        /// <param name="objs">The objs.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> SaveAllAsync(IEnumerable objs, CancellationToken token)
        {
            return Exec(dbCmd => dbCmd.SaveAllAsync((IEnumerable<T>)objs, token));
        }

        /// <summary>
        /// Saves the asynchronous.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> SaveAsync(object obj, CancellationToken token)
        {
            return Exec(dbCmd => dbCmd.SaveAsync((T)obj, token));
        }
#else
        public Task<int> SaveAllAsync(IEnumerable objs, CancellationToken token)
        {
            throw new NotImplementedException(OrmLiteUtils.AsyncRequiresNet45Error);
        }

        public Task<bool> SaveAsync(object obj, CancellationToken token)
        {
            throw new NotImplementedException(OrmLiteUtils.AsyncRequiresNet45Error);
        }
#endif

        /// <summary>
        /// Inserts all.
        /// </summary>
        /// <param name="objs">The objs.</param>
        public void InsertAll(IEnumerable objs)
        {
            Exec(dbCmd => dbCmd.InsertAll((IEnumerable<T>)objs, commandFilter: null));
        }

        /// <summary>
        /// Inserts all.
        /// </summary>
        /// <param name="objs">The objs.</param>
        /// <param name="commandFilter">The command filter.</param>
        public void InsertAll(IEnumerable objs, Action<IDbCommand> commandFilter)
        {
            Exec(dbCmd => dbCmd.InsertAll((IEnumerable<T>)objs, commandFilter: commandFilter));
        }

        /// <summary>
        /// Inserts the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="selectIdentity">if set to <c>true</c> [select identity].</param>
        /// <returns>System.Int64.</returns>
        public long Insert(object obj, bool selectIdentity = false)
        {
            return Exec(dbCmd => dbCmd.Insert((T)obj, commandFilter: null, selectIdentity: selectIdentity));
        }

        /// <summary>
        /// Inserts the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="selectIdentity">if set to <c>true</c> [select identity].</param>
        /// <returns>System.Int64.</returns>
        public long Insert(object obj, Action<IDbCommand> commandFilter, bool selectIdentity = false)
        {
            return Exec(dbCmd => dbCmd.Insert((T)obj, commandFilter: commandFilter, selectIdentity: selectIdentity));
        }

        /// <summary>
        /// Updates all.
        /// </summary>
        /// <param name="objs">The objs.</param>
        /// <returns>System.Int32.</returns>
        public int UpdateAll(IEnumerable objs)
        {
            return Exec(dbCmd => dbCmd.UpdateAll((IEnumerable<T>)objs));
        }

        /// <summary>
        /// Updates all.
        /// </summary>
        /// <param name="objs">The objs.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <returns>System.Int32.</returns>
        public int UpdateAll(IEnumerable objs, Action<IDbCommand> commandFilter)
        {
            return Exec(dbCmd => dbCmd.UpdateAll((IEnumerable<T>)objs, commandFilter: commandFilter));
        }

        /// <summary>
        /// Updates the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>System.Int32.</returns>
        public int Update(object obj)
        {
            return Exec(dbCmd => dbCmd.Update((T)obj));
        }

        /// <summary>
        /// Updates the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <returns>System.Int32.</returns>
        public int Update(object obj, Action<IDbCommand> commandFilter)
        {
            return Exec(dbCmd => dbCmd.Update((T)obj, commandFilter: commandFilter));
        }

        /// <summary>
        /// Deletes all.
        /// </summary>
        /// <returns>System.Int32.</returns>
        public int DeleteAll()
        {
            return Exec(dbCmd => dbCmd.DeleteAll<T>());
        }

        /// <summary>
        /// Deletes the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>System.Int32.</returns>
        public int Delete(object obj, object anonType)
        {
            return Exec(dbCmd => dbCmd.Delete<T>(anonType));
        }

        /// <summary>
        /// Deletes the non defaults.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>System.Int32.</returns>
        public int DeleteNonDefaults(object obj, object filter)
        {
            return Exec(dbCmd => dbCmd.DeleteNonDefaults((T)filter));
        }

        /// <summary>
        /// Deletes the by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>System.Int32.</returns>
        public int DeleteById(object id)
        {
            return Exec(dbCmd => dbCmd.DeleteById<T>(id));
        }

        /// <summary>
        /// Deletes the by ids.
        /// </summary>
        /// <param name="idValues">The identifier values.</param>
        /// <returns>System.Int32.</returns>
        public int DeleteByIds(IEnumerable idValues)
        {
            return Exec(dbCmd => dbCmd.DeleteByIds<T>(idValues));
        }

        /// <summary>
        /// Casts the specified results.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <returns>IEnumerable.</returns>
        public IEnumerable Cast(IEnumerable results)
        {
            return (from object result in results select (T)result).ToList();
        }
    }
}