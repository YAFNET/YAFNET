/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Classes.Data
{
    #region Using

    using System.Data;
    using System.Data.SqlClient;
    using YAF.Types;
    using YAF.Types.Handlers;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;

    #endregion

    /// <summary>
    /// Provides open/close management for DB Connections
    /// </summary>
    public class MsSqlDbConnectionManager : IDbConnectionManager
    {
        #region Constants and Fields

        /// <summary>
        ///   The _connection.
        /// </summary>
        protected SqlConnection _connection;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "MsSqlDbConnectionManager" /> class.
        /// </summary>
        public MsSqlDbConnectionManager()
        {
            // just initalize it (not open)
            this.InitConnection();
        }

        #endregion

        #region Events

        /// <summary>
        ///   The info message.
        /// </summary>
        public event YafDBConnInfoMessageEventHandler InfoMessage;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets ConnectionString.
        /// </summary>
        public virtual string ConnectionString
        {
            get { return Config.ConnectionString; }
        }

        /// <summary>
        ///   Gets the current DB Connection in any state.
        /// </summary>
        public SqlConnection DBConnection
        {
            get
            {
                this.InitConnection();
                return this._connection;
            }
        }

        /// <summary>
        ///   Gets an open connection to the DB. Can be called any number of times.
        /// </summary>
        public SqlConnection OpenDBConnection
        {
            get
            {
                this.InitConnection();

                if (this._connection.State != ConnectionState.Open)
                {
                    // open it up...
                    this._connection.Open();
                }

                return this._connection;
            }
        }

        /// <summary>
        /// Gets DBConnection.
        /// </summary>
        IDbConnection IDbConnectionManager.DBConnection
        {
            get { return this.DBConnection; }
        }

        /// <summary>
        /// Gets OpenDBConnection.
        /// </summary>
        IDbConnection IDbConnectionManager.OpenDBConnection
        {
            get { return this.OpenDBConnection; }
        }

        #endregion

        #region Implemented Interfaces

        #region IDbConnectionManager

        /// <summary>
        /// The close connection.
        /// </summary>
        public void CloseConnection()
        {
            if (this._connection != null && this._connection.State != ConnectionState.Closed)
            {
                this._connection.Close();
            }
        }

        /// <summary>
        /// The init connection.
        /// </summary>
        public void InitConnection()
        {
            if (this._connection == null)
            {
                // create the connection
                this._connection = new SqlConnection();
                this._connection.InfoMessage += this.Connection_InfoMessage;
                this._connection.ConnectionString = this.ConnectionString;
            }
            else if (this._connection.State != ConnectionState.Open)
            {
                // verify the connection string is in there...
                this._connection.ConnectionString = this.ConnectionString;
            }
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// The dispose.
        /// </summary>
        public virtual void Dispose()
        {
            // close and delete connection
            this.CloseConnection();
            this._connection = null;
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// The connection_ info message.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Connection_InfoMessage([NotNull] object sender, [NotNull] SqlInfoMessageEventArgs e)
        {
            if (this.InfoMessage != null)
            {
                this.InfoMessage(this, new YafDBConnInfoMessageEventArgs(e.Message));
            }
        }

        #endregion
    }
}