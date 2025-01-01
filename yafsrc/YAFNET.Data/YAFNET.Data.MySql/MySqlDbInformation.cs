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

using Microsoft.Extensions.Configuration;

using YAF.Core.Context;
using YAF.Types.Interfaces;

namespace YAF.Data.MySql;

/// <summary>
/// MySQL DB Information
/// </summary>
public class MySqlDbInformation : IDbInformation, IHaveServiceLocator
{
    /// <summary>
    /// The YAF Provider Upgrade script list
    /// </summary>
    private readonly static string[] IdentityUpgradeScriptList = [
        "upgrade.sql"
    ];

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlDbInformation"/> class.
    /// </summary>
    public MySqlDbInformation()
    {
        this.ConnectionString = () => this.Get<IConfiguration>().GetConnectionString("DefaultConnection");
        this.ProviderName = MySqlDbAccess.ProviderTypeName;
    }

    /// <summary>
    ///   Gets the ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

    /// <summary>
    /// Gets or sets the DB Connection String
    /// </summary>
    public Func<string> ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the DB Provider Name
    /// </summary>
    public string ProviderName { get; protected set; }

    /// <summary>
    /// Gets the YAF Provider Upgrade Script List.
    /// </summary>
    public IEnumerable<string> IdentityUpgradeScripts => IdentityUpgradeScriptList;

    /// <summary>
    /// Create Table Views
    /// </summary>
    /// <param name="dbAccess">
    /// The database access.
    /// </param>
    /// <param name="dbCommand">
    /// The database command.
    /// </param>
    public bool CreateViews(IDbAccess dbAccess, IDbCommand dbCommand)
    {
        var vaccessGroupSelect = new StringBuilder();

        vaccessGroupSelect.Append(" select ");

        vaccessGroupSelect.Append("e.BoardID,");
        vaccessGroupSelect.Append("b.UserID,");
        vaccessGroupSelect.Append("c.ForumID,");
        vaccessGroupSelect.Append("d.AccessMaskID,");
        vaccessGroupSelect.Append("b.GroupID,");
        vaccessGroupSelect.Append("d.Flags & 1 AS ReadAccess,");
        vaccessGroupSelect.Append("d.Flags & 2 AS PostAccess,");
        vaccessGroupSelect.Append("d.Flags & 4 AS ReplyAccess,");
        vaccessGroupSelect.Append("d.Flags & 8 AS PriorityAccess,");
        vaccessGroupSelect.Append("d.Flags & 16 AS PollAccess,");
        vaccessGroupSelect.Append("d.Flags & 32 AS VoteAccess,");
        vaccessGroupSelect.Append("d.Flags & 64 AS ModeratorAccess,");
        vaccessGroupSelect.Append("d.Flags & 128 AS EditAccess,");
        vaccessGroupSelect.Append("d.Flags & 256 AS DeleteAccess,");
        vaccessGroupSelect.Append("e.Flags & 1 AS AdminGroup");

        vaccessGroupSelect.Append(" from");

        vaccessGroupSelect.AppendFormat(
            " {0}.{1} AS b",
            dbCommand.Connection.Database,
            dbCommand.Connection.GetTableName<UserGroup>());
        vaccessGroupSelect.AppendFormat(
            " INNER JOIN {0}.{1} AS c on c.GroupID=b.GroupID",
            dbCommand.Connection.Database,
            dbCommand.Connection.GetTableName<ForumAccess>());
        vaccessGroupSelect.AppendFormat(
            " INNER JOIN {0}.{1} AS d on d.AccessMaskID=c.AccessMaskID",
            dbCommand.Connection.Database,
            dbCommand.Connection.GetTableName<AccessMask>());
        vaccessGroupSelect.AppendFormat(
            " INNER JOIN {0}.{1} AS e on e.GroupID=b.GroupID",
            dbCommand.Connection.Database,
            dbCommand.Connection.GetTableName<Group>());

        dbCommand.Connection.CreateView<VaccessGroup>(vaccessGroupSelect);

        var vaccessNullSelect = new StringBuilder();

        vaccessNullSelect.Append(" select ");

        vaccessNullSelect.Append("a.UserID,");
        vaccessNullSelect.Append("0 AS ForumID,");
        vaccessNullSelect.Append("0 AS ReadAccess,");
        vaccessNullSelect.Append("0 AS PostAccess,");
        vaccessNullSelect.Append("0 AS ReplyAccess,");
        vaccessNullSelect.Append("0 AS PriorityAccess,");
        vaccessNullSelect.Append("0 AS PollAccess,");
        vaccessNullSelect.Append("0 AS VoteAccess,");
        vaccessNullSelect.Append("0 AS ModeratorAccess,");
        vaccessNullSelect.Append("0 AS EditAccess,");
        vaccessNullSelect.Append("0 AS DeleteAccess,");
        vaccessNullSelect.Append("0 AS AdminGroup ");

        vaccessNullSelect.Append(" from");

        vaccessNullSelect.AppendFormat(" {0}.{1} AS a", dbCommand.Connection.Database, dbCommand.Connection.GetTableName<User>());

        dbCommand.Connection.CreateView<VaccessNull>(vaccessNullSelect);

        var vaccessUserSelect = new StringBuilder();

        vaccessUserSelect.Append(" select ");

        vaccessUserSelect.Append("b.UserID,");
        vaccessUserSelect.Append("b.ForumID,");
        vaccessUserSelect.Append("c.AccessMaskID,");
        vaccessNullSelect.Append("0 AS GroupID,");
        vaccessUserSelect.Append("c.Flags & 1 AS ReadAccess,");
        vaccessUserSelect.Append("c.Flags & 2 AS PostAccess,");
        vaccessUserSelect.Append("c.Flags & 4 AS ReplyAccess,");
        vaccessUserSelect.Append("c.Flags & 8 AS PriorityAccess,");
        vaccessUserSelect.Append("c.Flags & 16 AS PollAccess,");
        vaccessUserSelect.Append("c.Flags & 32 AS VoteAccess,");
        vaccessUserSelect.Append("c.Flags & 64 AS ModeratorAccess,");
        vaccessUserSelect.Append("c.Flags & 128 AS EditAccess,");
        vaccessUserSelect.Append("c.Flags & 256 AS DeleteAccess");

        vaccessUserSelect.Append(" from");
        vaccessUserSelect.AppendFormat(
            " {0}.{1} AS b",
            dbCommand.Connection.Database,
            dbCommand.Connection.GetTableName<UserForum>());
        vaccessUserSelect.AppendFormat(
            " INNER JOIN {0}.{1} AS c on c.AccessMaskID=b.AccessMaskID",
            dbCommand.Connection.Database,
            dbCommand.Connection.GetTableName<AccessMask>());

        dbCommand.Connection.CreateView<VaccessUser>(vaccessUserSelect);

        var vaccessFullSelect = new StringBuilder();

        vaccessFullSelect.Append(" select ");

        vaccessFullSelect.Append("b.UserID,");
        vaccessFullSelect.Append("b.ForumID,");
        vaccessFullSelect.Append("c.Flags & 1 AS ReadAccess,");
        vaccessFullSelect.Append(" c.Flags & 2 AS PostAccess,");
        vaccessFullSelect.Append("c.Flags & 4 AS ReplyAccess,");
        vaccessFullSelect.Append("c.Flags & 8 AS PriorityAccess,");
        vaccessFullSelect.Append("c.Flags & 16 AS PollAccess,");
        vaccessFullSelect.Append("c.Flags & 32 AS VoteAccess,");
        vaccessFullSelect.Append("c.Flags & 64 AS ModeratorAccess,");
        vaccessFullSelect.Append("c.Flags & 128 AS EditAccess,");
        vaccessFullSelect.Append("c.Flags & 256 AS DeleteAccess,");
        vaccessFullSelect.Append("0 AS AdminGroup ");

        vaccessFullSelect.AppendFormat("FROM {0}.{1} AS b ",
            dbCommand.Connection.Database,
            dbCommand.Connection.GetTableName<UserForum>());

        vaccessFullSelect.AppendFormat("INNER JOIN {0}.{1} AS c ",
            dbCommand.Connection.Database,
            dbCommand.Connection.GetTableName<AccessMask>());

        vaccessFullSelect.Append("ON c.AccessMaskID = b.AccessMaskID ");

        vaccessFullSelect.Append("UNION ALL ");
        vaccessFullSelect.Append(" SELECT ");

        vaccessFullSelect.Append("b.UserID,");
        vaccessFullSelect.Append("c.ForumID,");
        vaccessFullSelect.Append("d.Flags & 1 AS ReadAccess,");
        vaccessFullSelect.Append("d.Flags & 2 AS PostAccess,");
        vaccessFullSelect.Append("d.Flags & 4 AS ReplyAccess,");
        vaccessFullSelect.Append("d.Flags & 8 AS PriorityAccess,");
        vaccessFullSelect.Append("d.Flags & 16 AS PollAccess,");
        vaccessFullSelect.Append("d.Flags & 32 AS VoteAccess,");
        vaccessFullSelect.Append("d.Flags & 64 AS ModeratorAccess,");
        vaccessFullSelect.Append("d.Flags & 128 AS EditAccess,");
        vaccessFullSelect.Append("d.Flags & 256 AS DeleteAccess,");
        vaccessFullSelect.Append("e.Flags & 1 AS AdminGroup");

        vaccessFullSelect.AppendFormat(" FROM {0}.{1} AS b",
            dbCommand.Connection.Database,
            dbCommand.Connection.GetTableName<UserGroup>());

        vaccessFullSelect.AppendFormat(" INNER JOIN {0}.{1} AS c ON c.GroupID = b.GroupID ",
            dbCommand.Connection.Database,
            dbCommand.Connection.GetTableName<ForumAccess>());

        vaccessFullSelect.AppendFormat(" INNER JOIN {0}.{1} AS d ON d.AccessMaskID = c.AccessMaskID ",
            dbCommand.Connection.Database,
            dbCommand.Connection.GetTableName<AccessMask>());

        vaccessFullSelect.AppendFormat(" INNER JOIN {0}.{1} e ON e.GroupID = b.GroupID ",
            dbCommand.Connection.Database,
            dbCommand.Connection.GetTableName<Group>());

        vaccessFullSelect.Append(" UNION ALL ");
        vaccessFullSelect.Append(" SELECT ");

        vaccessFullSelect.Append("UserID,");
        vaccessFullSelect.Append("0 AS ForumID,");
        vaccessFullSelect.Append("0 AS ReadAccess,");
        vaccessFullSelect.Append("0 AS PostAccess,");
        vaccessFullSelect.Append("0 AS ReplyAccess,");
        vaccessFullSelect.Append("0 AS PriorityAccess,");
        vaccessFullSelect.Append("0 AS PollAccess,");
        vaccessFullSelect.Append("0 AS VoteAccess,");
        vaccessFullSelect.Append("0 AS ModeratorAccess,");
        vaccessFullSelect.Append("0 AS EditAccess,");
        vaccessFullSelect.Append("0 AS DeleteAccess,");
        vaccessFullSelect.Append("0 AS AdminGroup ");

        vaccessFullSelect.AppendFormat("FROM {0}.{1} AS a",
            dbCommand.Connection.Database,
            dbCommand.Connection.GetTableName<User>());

        dbCommand.Connection.CreateView<VAccessFull>(vaccessFullSelect);

        var vaccessSelect = new StringBuilder();

        vaccessSelect.Append(" select ");

        vaccessSelect.Append("a.UserID,");
        vaccessSelect.Append("x_1.ForumID,");
        vaccessSelect.Append("MAX(b.Flags & 1) AS IsAdmin,");
        vaccessSelect.Append("MAX(b.Flags & 2) AS IsGuest,");
        vaccessSelect.Append("MAX(b.Flags & 8) AS IsForumModerator,");
        vaccessSelect.Append("(SELECT COUNT(1) AS Expr1 ");

        vaccessSelect.AppendFormat("FROM {0}.{1} AS v",
            dbCommand.Connection.Database,
            dbCommand.Connection.GetTableName<UserGroup>());
        vaccessSelect.AppendFormat(" INNER JOIN {0}.{1} AS w ON v.GroupID = w.GroupID",
            dbCommand.Connection.Database,
            dbCommand.Connection.GetTableName<Group>());

        vaccessSelect.AppendFormat(" CROSS JOIN  {0}.{1} AS x",
            dbCommand.Connection.Database,
            dbCommand.Connection.GetTableName<ForumAccess>());

        vaccessSelect.AppendFormat(" CROSS JOIN  {0}.{1} AS y",
            dbCommand.Connection.Database,
            dbCommand.Connection.GetTableName<AccessMask>());

        vaccessSelect.Append(" WHERE(v.UserID = a.UserID)");
        vaccessSelect.Append(" AND(x.GroupID = w.GroupID)");
        vaccessSelect.Append(" AND(y.AccessMaskID = x.AccessMaskID)");

        vaccessSelect.Append(" AND(y.Flags & 64 <> 0)) AS IsModerator,");
        vaccessSelect.Append("MAX(x_1.ReadAccess) AS ReadAccess,");
        vaccessSelect.Append("MAX(x_1.PostAccess) AS PostAccess,");
        vaccessSelect.Append("MAX(x_1.ReplyAccess) AS ReplyAccess,");
        vaccessSelect.Append("MAX(x_1.PriorityAccess) AS PriorityAccess,");
        vaccessSelect.Append("MAX(x_1.PollAccess) AS PollAccess,");
        vaccessSelect.Append("MAX(x_1.VoteAccess) AS VoteAccess,");
        vaccessSelect.Append("MAX(x_1.ModeratorAccess) AS ModeratorAccess,");
        vaccessSelect.Append("MAX(x_1.EditAccess) AS EditAccess,");
        vaccessSelect.Append("MAX(x_1.DeleteAccess) AS DeleteAccess");

        vaccessSelect.AppendFormat(" FROM {0}.{1} x_1 ",
            dbCommand.Connection.Database,
            dbCommand.Connection.GetTableName<VAccessFull>());

        vaccessSelect.AppendFormat(" INNER JOIN  {0}.{1} AS a ON a.UserID = x_1.UserID ",
            dbCommand.Connection.Database,
            dbCommand.Connection.GetTableName<UserGroup>());

        vaccessSelect.AppendFormat(" INNER JOIN {0}.{1} AS b ON b.GroupID = a.GroupID ",
            dbCommand.Connection.Database,
            dbCommand.Connection.GetTableName<Group>());

        vaccessSelect.Append(" GROUP BY a.UserID, x_1.ForumID");

        dbCommand.Connection.CreateView<VAccess>(vaccessSelect);

        return true;
    }

    /// <summary>
    /// Create Indexes on Table Views
    /// </summary>
    /// <param name="dbAccess">
    /// The database access.
    /// </param>
    /// <param name="dbCommand">
    /// The database command.
    /// </param>
    public bool CreateIndexViews(IDbAccess dbAccess, IDbCommand dbCommand)
    {
        return true;
    }
}