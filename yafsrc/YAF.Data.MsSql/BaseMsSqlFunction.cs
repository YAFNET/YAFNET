/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Data.MsSql
{
    using System.Collections.Generic;
    using System.Data;

    using System.Data.SqlClient;

    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions.Data;
    using YAF.Types.Interfaces.Data;

    /// <summary>
    /// The base ms sql function.
    /// </summary>
    public abstract class BaseMsSqlFunction : IDbSpecificFunction
    {
        #region Constants and Fields

        /// <summary>
        ///   The _sql messages.
        /// </summary>
        protected List<SqlInfoMessageEventArgs> _sqlMessages = new List<SqlInfoMessageEventArgs>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMsSqlFunction"/> class.
        /// </summary>
        /// <param name="dbAccess">
        /// The db access.
        /// </param>
        protected BaseMsSqlFunction([NotNull] IDbAccess dbAccess)
        {
            this.DbAccess = dbAccess;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets DbAccess.
        /// </summary>
        public IDbAccess DbAccess { get; set; }

        /// <summary>
        ///   Gets ProviderName.
        /// </summary>
        [NotNull]
        public virtual string ProviderName => MsSqlDbAccess.ProviderTypeName;

        /// <summary>
        ///   Gets SortOrder.
        /// </summary>
        public abstract int SortOrder { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="dbfunctionType">
        /// The dbfunction type.
        /// </param>
        /// <param name="operationName">
        /// The operation name.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="transaction"></param>
        /// <returns>
        /// The execute.
        /// </returns>
        public virtual bool Execute(
            DBFunctionType dbfunctionType,
            [NotNull] string operationName,
            [NotNull] IEnumerable<KeyValuePair<string, object>> parameters,
            [CanBeNull] out object result,
            IDbTransaction transaction = null)
        {
            if (this.IsSupportedOperation(operationName))
            {
                this._sqlMessages.Clear();

                var createdTransaction = transaction == null;

                try
                {
                    transaction ??= this.DbAccess.BeginTransaction();

                    if (transaction.Connection is SqlConnection sqlConnection)
                    {
                        sqlConnection.FireInfoMessageEventOnUserErrors = true;
                        sqlConnection.InfoMessage += this.sqlConnection_InfoMessage;

                        var operationSuccessful = this.RunOperation(
                            sqlConnection,
                            transaction,
                            dbfunctionType,
                            operationName,
                            parameters,
                            out result);

                        if (createdTransaction && operationSuccessful)
                        {
                            transaction.Commit();
                        }

                        return operationSuccessful;
                    }
                }
                finally
                {
                    if (createdTransaction)
                    {
                        transaction?.Dispose();
                    }
                }
            }

            result = null;

            return false;
        }

        /// <summary>
        /// The supported operation.
        /// </summary>
        /// <param name="operationName">
        /// The operation name.
        /// </param>
        /// <returns>
        /// True if the operation is supported.
        /// </returns>
        public abstract bool IsSupportedOperation([NotNull] string operationName);

        #endregion

        #region Methods

        /// <summary>
        /// The run operation.
        /// </summary>
        /// <param name="sqlConnection">
        /// The sql connection.
        /// </param>
        /// <param name="dbTransaction">
        /// The unit Of Work.
        /// </param>
        /// <param name="dbfunctionType">
        /// The dbfunction type.
        /// </param>
        /// <param name="operationName">
        /// The operation name.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The run operation.
        /// </returns>
        protected abstract bool RunOperation(
            [NotNull] SqlConnection sqlConnection,
            [NotNull] IDbTransaction dbTransaction,
            DBFunctionType dbfunctionType,
            [NotNull] string operationName,
            [NotNull] IEnumerable<KeyValuePair<string, object>> parameters,
            [CanBeNull] out object result);

        /// <summary>
        /// The sql connection_ info message.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void sqlConnection_InfoMessage([NotNull] object sender, [NotNull] SqlInfoMessageEventArgs e)
        {
            this._sqlMessages.Add(e);
        }

        #endregion
    }
}