/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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
namespace YAF.Data.Sqlite;

/// <summary>
/// The Sqlite Database access.
/// </summary>
public class SqliteDbAccess : DbAccessBase
{
    /// <summary>
    /// The provider type name.
    /// </summary>
    public const string ProviderTypeName = "System.Data.SQLite";

    /// <summary>
    /// Initializes a new instance of the <see cref="SqliteDbAccess"/> class.
    /// </summary>
    /// <param name="dbProviderFactory">
    /// The database provider factory.
    /// </param>
    public SqliteDbAccess(Func<string, DbProviderFactory> dbProviderFactory)
        : base(dbProviderFactory, new SqliteDbInformation())
    {
    }

    /// <summary>
    /// The waits before the retries of an operation that failed with "database is locked" (in milliseconds).
    /// </summary>
    private static readonly int[] BusyRetryDelays = [100, 300, 600];

    /// <summary>
    /// Executes the function, an operation that fails with "database is locked" (another connection writes at the
    /// same time, longer than the busy timeout or in a way SQLite doesn't wait for) is tried again.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="execFunc">The function.</param>
    /// <param name="cmd">The command.</param>
    /// <param name="dbTransaction">The transaction.</param>
    /// <returns>The result of the function.</returns>
    public override T Execute<T>(Func<IDbCommand, T> execFunc, IDbCommand cmd = null, IDbTransaction dbTransaction = null)
    {
        // inside a transaction the whole transaction would have to be repeated
        if (dbTransaction != null)
        {
            return base.Execute(execFunc, cmd, dbTransaction);
        }

        for (var attempt = 0; ; attempt++)
        {
            try
            {
                return base.Execute(execFunc, cmd, dbTransaction);
            }
            catch (SQLiteException ex) when (IsBusy(ex) && attempt < BusyRetryDelays.Length)
            {
                System.Threading.Thread.Sleep(BusyRetryDelays[attempt]);
            }
        }
    }

    /// <summary>
    /// Checks if the database was locked by another connection.
    /// </summary>
    /// <param name="ex">The exception.</param>
    /// <returns>Returns if the operation can be tried again.</returns>
    private static bool IsBusy(SQLiteException ex)
    {
        return ex.ResultCode is SQLiteErrorCode.Busy or SQLiteErrorCode.Locked
               || (ex.ResultCode & (SQLiteErrorCode)0xFF) is SQLiteErrorCode.Busy or SQLiteErrorCode.Locked;
    }
}