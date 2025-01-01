/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Core.Extensions;

/// <summary>
/// The db access extensions.
/// </summary>
public static class DbAccessV2Extensions
{
    /// <summary>
    /// The replace command text.
    /// </summary>
    /// <param name="dbCommand">
    /// The db command.
    /// </param>
    public static DbCommand ReplaceCommandText(this DbCommand dbCommand)
    {
        var commandText = dbCommand.CommandText;

        commandText = commandText.Replace("{databaseOwner}", Config.DatabaseOwner);
        commandText = commandText.Replace("{objectQualifier}", Config.DatabaseObjectQualifier);

        dbCommand.CommandText = commandText;

        return dbCommand;
    }

    /// <summary>
    /// Test the DB Connection.
    /// </summary>
    /// <param name="dbAccess">
    /// The db Access.
    /// </param>
    /// <param name="exceptionMessage">
    /// outbound ExceptionMessage
    /// </param>
    /// <returns>
    /// true if successfully connected
    /// </returns>
    public static bool TestConnection(this IDbAccess dbAccess, out string exceptionMessage)
    {
        exceptionMessage = string.Empty;
        var success = false;

        try
        {
            using (var connection = dbAccess.CreateConnectionOpen())
            {
                // we're connected!
                _ = connection;
            }

            // success
            success = true;
        }
        catch (DbException x)
        {
            // unable to connect...
            exceptionMessage = x.Message;
        }

        return success;
    }
}