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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */


// The tests run on System.Data.SQLite, or on Microsoft.Data.Sqlite with
// dotnet test -p:SqliteProvider=Microsoft.Data.Sqlite (YAFNET.Data.Sqlite.Data, the provider of YAF-SqliteData.csproj).
// The aliases let the tests use the names of System.Data.SQLite for both.
#if MICROSOFT_DATA_SQLITE
global using SQLiteConnection = Microsoft.Data.Sqlite.SqliteConnection;
global using SQLiteException = Microsoft.Data.Sqlite.SqliteException;
global using SQLiteFactory = Microsoft.Data.Sqlite.SqliteFactory;
#else
global using System.Data.SQLite;
#endif

namespace YAF.Tests.TestInfrastructure;

/// <summary>
/// What differs between System.Data.SQLite and Microsoft.Data.Sqlite in the tests.
/// </summary>
public static class SqliteTestProvider
{
    /// <summary>
    /// The connection string of a private in-memory database, which lives as long as a connection to it is open.
    /// Microsoft.Data.Sqlite: the foreign keys are off, as with System.Data.SQLite (and as the forum connects, see
    /// SqliteDbInformation.WithConcurrencySettings). The database is opened by its URI, VACUUM INTO (the backup)
    /// doesn't write a file for a database opened with Mode=Memory.
    /// </summary>
    /// <param name="name">The unique name of the database.</param>
    /// <returns>The connection string.</returns>
    public static string InMemoryConnectionString(string name)
    {
#if MICROSOFT_DATA_SQLITE
        return $"Data Source=file:{name}?mode=memory&cache=shared;Foreign Keys=False";
#else
        return $"FullUri=file:{name}?mode=memory&cache=shared;";
#endif
    }

    /// <summary>
    /// The connection string of a database file which isn't created when it is missing.
    /// </summary>
    /// <param name="file">The database file.</param>
    /// <returns>The connection string.</returns>
    public static string ExistingFileConnectionString(string file)
    {
#if MICROSOFT_DATA_SQLITE
        return $"Data Source={file};Mode=ReadWrite";
#else
        return $"Data Source={file};FailIfMissing=True";
#endif
    }

    /// <summary>
    /// Copies a database with the SQLite backup API.
    /// </summary>
    /// <param name="source">The open source connection.</param>
    /// <param name="destination">The open destination connection.</param>
    public static void CopyDatabase(SQLiteConnection source, SQLiteConnection destination)
    {
#if MICROSOFT_DATA_SQLITE
        source.BackupDatabase(destination);
#else
        source.BackupDatabase(destination, "main", "main", -1, null, 0);
#endif
    }

    /// <summary>
    /// The exception of a locked database (SQLITE_BUSY).
    /// </summary>
    /// <returns>The exception.</returns>
    public static SQLiteException Busy()
    {
#if MICROSOFT_DATA_SQLITE
        return new SQLiteException("database is locked", 5);
#else
        return new SQLiteException(SQLiteErrorCode.Busy, "database is locked");
#endif
    }

    /// <summary>
    /// The exception of a failed constraint (SQLITE_CONSTRAINT).
    /// </summary>
    /// <returns>The exception.</returns>
    public static SQLiteException ConstraintFailed()
    {
#if MICROSOFT_DATA_SQLITE
        return new SQLiteException("constraint failed", 19);
#else
        return new SQLiteException(SQLiteErrorCode.Constraint, "constraint failed");
#endif
    }
}
