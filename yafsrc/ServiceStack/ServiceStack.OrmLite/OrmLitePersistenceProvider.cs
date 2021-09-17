// ***********************************************************************
// <copyright file="OrmLitePersistenceProvider.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Data;
using ServiceStack.Data;

namespace ServiceStack.OrmLite
{
    /// <summary>
    /// Allow for code-sharing between OrmLite, IPersistenceProvider and ICacheClient
    /// </summary>
    public class OrmLitePersistenceProvider
        : IEntityStore
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        protected string ConnectionString { get; set; }
        /// <summary>
        /// The dispose connection
        /// </summary>
        protected bool DisposeConnection = true;

        /// <summary>
        /// The connection
        /// </summary>
        protected IDbConnection connection;
        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        public IDbConnection Connection
        {
            get
            {
                if (connection == null)
                {
                    var connStr = this.ConnectionString;
                    connection = connStr.OpenDbConnection();
                }
                return connection;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrmLitePersistenceProvider"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public OrmLitePersistenceProvider(string connectionString)
        {
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrmLitePersistenceProvider"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public OrmLitePersistenceProvider(IDbConnection connection)
        {
            this.connection = connection;
            this.DisposeConnection = false;
        }

        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <returns>IDbCommand.</returns>
        private IDbCommand CreateCommand()
        {
            var cmd = this.Connection.CreateCommand();
            cmd.CommandTimeout = OrmLiteConfig.CommandTimeout;
            return cmd;
        }

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The identifier.</param>
        /// <returns>T.</returns>
        public T GetById<T>(object id)
        {
            return this.Connection.SingleById<T>(id);
        }

        /// <summary>
        /// Gets the by ids.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids">The ids.</param>
        /// <returns>IList&lt;T&gt;.</returns>
        public IList<T> GetByIds<T>(ICollection ids)
        {
            return this.Connection.SelectByIds<T>(ids);
        }

        /// <summary>
        /// Stores the specified entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>T.</returns>
        public T Store<T>(T entity)
        {
            this.Connection.Save(entity);
            return entity;
        }

        /// <summary>
        /// Stores all.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="entities">The entities.</param>
        public void StoreAll<TEntity>(IEnumerable<TEntity> entities)
        {
            this.Connection.SaveAll(entities);
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        public void Delete<T>(T entity)
        {
            this.Connection.DeleteById<T>(entity.GetId());
        }

        /// <summary>
        /// Deletes the by identifier.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The identifier.</param>
        public void DeleteById<T>(object id)
        {
            this.Connection.DeleteById<T>(id);
        }

        /// <summary>
        /// Deletes the by ids.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids">The ids.</param>
        public void DeleteByIds<T>(ICollection ids)
        {
            this.Connection.DeleteByIds<T>(ids);
        }

        /// <summary>
        /// Deletes all.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        public void DeleteAll<TEntity>()
        {
            this.Connection.DeleteAll<TEntity>();
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            if (!DisposeConnection) return;
            if (this.connection == null) return;

            this.connection.Dispose();
            this.connection = null;
        }
    }
}