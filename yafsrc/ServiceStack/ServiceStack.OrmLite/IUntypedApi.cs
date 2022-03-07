// ***********************************************************************
// <copyright file="IUntypedApi.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface IUntypedApi
    /// </summary>
    public interface IUntypedApi
    {
        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        /// <value>The database.</value>
        IDbConnection Db { get; set; }

        /// <summary>
        /// Gets or sets the database command.
        /// </summary>
        /// <value>The database command.</value>
        IDbCommand DbCmd { get; set; }

        /// <summary>
        /// Saves all.
        /// </summary>
        /// <param name="objs">The objs.</param>
        /// <returns>System.Int32.</returns>
        int SaveAll(IEnumerable objs);

        /// <summary>
        /// Saves all asynchronous.
        /// </summary>
        /// <param name="objs">The objs.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        Task<int> SaveAllAsync(IEnumerable objs, CancellationToken token);

        /// <summary>
        /// Saves the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Save(object obj);

        /// <summary>
        /// Saves the asynchronous.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> SaveAsync(object obj, CancellationToken token);

        /// <summary>
        /// Inserts all.
        /// </summary>
        /// <param name="objs">The objs.</param>
        void InsertAll(IEnumerable objs);

        /// <summary>
        /// Inserts all.
        /// </summary>
        /// <param name="objs">The objs.</param>
        /// <param name="commandFilter">The command filter.</param>
        void InsertAll(IEnumerable objs, Action<IDbCommand> commandFilter);

        /// <summary>
        /// Inserts the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="selectIdentity">if set to <c>true</c> [select identity].</param>
        /// <returns>System.Int64.</returns>
        long Insert(object obj, bool selectIdentity = false);

        /// <summary>
        /// Inserts the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <param name="selectIdentity">if set to <c>true</c> [select identity].</param>
        /// <returns>System.Int64.</returns>
        long Insert(object obj, Action<IDbCommand> commandFilter, bool selectIdentity = false);

        /// <summary>
        /// Updates all.
        /// </summary>
        /// <param name="objs">The objs.</param>
        /// <returns>System.Int32.</returns>
        int UpdateAll(IEnumerable objs);

        /// <summary>
        /// Updates all.
        /// </summary>
        /// <param name="objs">The objs.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <returns>System.Int32.</returns>
        int UpdateAll(IEnumerable objs, Action<IDbCommand> commandFilter);

        /// <summary>
        /// Updates the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>System.Int32.</returns>
        int Update(object obj);

        Task<int> UpdateAsync(object obj, CancellationToken token);

        /// <summary>
        /// Deletes all.
        /// </summary>
        /// <returns>System.Int32.</returns>
        int DeleteAll();

        /// <summary>
        /// Deletes the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>System.Int32.</returns>
        int Delete(object obj, object anonType);

        /// <summary>
        /// Deletes the non defaults.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>System.Int32.</returns>
        int DeleteNonDefaults(object obj, object filter);

        /// <summary>
        /// Deletes the by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>System.Int32.</returns>
        int DeleteById(object id);

        /// <summary>
        /// Deletes the by ids.
        /// </summary>
        /// <param name="idValues">The identifier values.</param>
        /// <returns>System.Int32.</returns>
        int DeleteByIds(IEnumerable idValues);

        /// <summary>
        /// Casts the specified results.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <returns>IEnumerable.</returns>
        IEnumerable Cast(IEnumerable results);
    }
}