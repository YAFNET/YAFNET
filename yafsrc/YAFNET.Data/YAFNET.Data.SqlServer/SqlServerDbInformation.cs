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
using YAF.Types.Objects;

namespace YAF.Data.SqlServer;

/// <summary>
/// MySQL DB Information
/// </summary>
public class SqlServerDbInformation : IDbInformation, IHaveServiceLocator
{
    /// <summary>
    /// The YAF Provider Upgrade script list
    /// </summary>
    private readonly static string[] IdentityUpgradeScriptList = ["upgrade.sql"];

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerDbInformation"/> class.
    /// </summary>
    public SqlServerDbInformation()
    {
        this.ConnectionString = () => this.Get<IConfiguration>().GetConnectionString("DefaultConnection");
        this.ProviderName = SqlServerDbAccess.ProviderTypeName;
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
        vaccessGroupSelect.Append("ReadAccess = convert(int,d.Flags & 1),");
        vaccessGroupSelect.Append("PostAccess = convert(int,d.Flags & 2),");
        vaccessGroupSelect.Append("ReplyAccess = convert(int,d.Flags & 4),");
        vaccessGroupSelect.Append("PriorityAccess = convert(int,d.Flags & 8),");
        vaccessGroupSelect.Append("PollAccess = convert(int,d.Flags & 16),");
        vaccessGroupSelect.Append("VoteAccess = convert(int,d.Flags & 32),");
        vaccessGroupSelect.Append("ModeratorAccess = convert(int,d.Flags & 64),");
        vaccessGroupSelect.Append("EditAccess = convert(int,d.Flags & 128),");
        vaccessGroupSelect.Append("DeleteAccess = convert(int,d.Flags & 256),");
        vaccessGroupSelect.Append("AdminGroup = convert(int,e.Flags & 1)");

        vaccessGroupSelect.Append(" from");

        vaccessGroupSelect.AppendFormat(
            " [{0}].[{1}] b",
            this.Get<BoardConfiguration>().DatabaseOwner,
            dbCommand.Connection.GetTableName<UserGroup>());
        vaccessGroupSelect.AppendFormat(
            " INNER JOIN [{0}].[{1}] c on c.GroupID=b.GroupID",
            this.Get<BoardConfiguration>().DatabaseOwner,
            dbCommand.Connection.GetTableName<ForumAccess>());
        vaccessGroupSelect.AppendFormat(
            " INNER JOIN [{0}].[{1}] d on d.AccessMaskID=c.AccessMaskID",
            this.Get<BoardConfiguration>().DatabaseOwner,
            dbCommand.Connection.GetTableName<AccessMask>());
        vaccessGroupSelect.AppendFormat(
            " INNER JOIN [{0}].[{1}] e on e.GroupID=b.GroupID",
            this.Get<BoardConfiguration>().DatabaseOwner,
            dbCommand.Connection.GetTableName<Group>());

        dbCommand.Connection.CreateView<VaccessGroup>(vaccessGroupSelect);

        var vaccessNullSelect = new StringBuilder();

        vaccessNullSelect.Append(" select ");

        vaccessNullSelect.Append("a.UserID,");
        vaccessNullSelect.Append("ForumID = convert(int,0),");
        vaccessNullSelect.Append("GroupID = convert(int,0),");
        vaccessNullSelect.Append("AccessMaskID = convert(int, 0),");
        vaccessNullSelect.Append("ReadAccess = convert(int, 0),");
        vaccessNullSelect.Append("PostAccess = convert(int, 0),");
        vaccessNullSelect.Append("ReplyAccess = convert(int, 0),");
        vaccessNullSelect.Append("PriorityAccess = convert(int, 0),");
        vaccessNullSelect.Append("PollAccess = convert(int, 0),");
        vaccessNullSelect.Append("VoteAccess = convert(int, 0),");
        vaccessNullSelect.Append("ModeratorAccess = convert(int, 0),");
        vaccessNullSelect.Append("EditAccess = convert(int, 0),");
        vaccessNullSelect.Append("DeleteAccess = convert(int, 0),");
        vaccessNullSelect.Append("AdminGroup = convert(int, 0)");

        vaccessNullSelect.Append(" from");

        vaccessNullSelect.AppendFormat(
            " [{0}].[{1}] a",
            this.Get<BoardConfiguration>().DatabaseOwner,
            dbCommand.Connection.GetTableName<User>());

        dbCommand.Connection.CreateView<VaccessNull>(vaccessNullSelect);

        var vaccessUserSelect = new StringBuilder();

        vaccessUserSelect.Append(" select ");

        vaccessUserSelect.Append("b.UserID,");
        vaccessUserSelect.Append("b.ForumID,");
        vaccessUserSelect.Append("c.AccessMaskID,");
        vaccessUserSelect.Append("GroupID = convert(int, 0),");
        vaccessUserSelect.Append("ReadAccess = convert(int, c.Flags & 1),");
        vaccessUserSelect.Append("PostAccess = convert(int, c.Flags & 2),");
        vaccessUserSelect.Append("ReplyAccess = convert(int, c.Flags & 4),");
        vaccessUserSelect.Append("PriorityAccess = convert(int, c.Flags & 8),");
        vaccessUserSelect.Append("PollAccess = convert(int, c.Flags & 16),");
        vaccessUserSelect.Append("VoteAccess = convert(int, c.Flags & 32),");
        vaccessUserSelect.Append("ModeratorAccess = convert(int, c.Flags & 64),");
        vaccessUserSelect.Append("EditAccess = convert(int, c.Flags & 128),");
        vaccessUserSelect.Append("DeleteAccess = convert(int, c.Flags & 256),");
        vaccessUserSelect.Append("AdminGroup = convert(int, 0)");

        vaccessUserSelect.Append(" from");
        vaccessUserSelect.AppendFormat(
            " [{0}].[{1}] b",
            this.Get<BoardConfiguration>().DatabaseOwner,
            dbCommand.Connection.GetTableName<UserForum>());

        vaccessUserSelect.AppendFormat(
            " INNER JOIN [{0}].[{1}] c on c.AccessMaskID=b.AccessMaskID",
            this.Get<BoardConfiguration>().DatabaseOwner,
            dbCommand.Connection.GetTableName<AccessMask>());

        dbCommand.Connection.CreateView<VaccessUser>(vaccessUserSelect);

        var vaccessFullSelect = new StringBuilder();

        vaccessFullSelect.Append(" select ");

        vaccessFullSelect.Append("UserID,ForumID,");
        vaccessFullSelect.Append("MAX(ReadAccess) AS ReadAccess,");
        vaccessFullSelect.Append("MAX(PostAccess) AS PostAccess,");
        vaccessFullSelect.Append("MAX(ReplyAccess) AS ReplyAccess,");
        vaccessFullSelect.Append("MAX(PriorityAccess) AS PriorityAccess,");
        vaccessFullSelect.Append("MAX(PollAccess) AS PollAccess,");
        vaccessFullSelect.Append("MAX(VoteAccess) AS VoteAccess,");
        vaccessFullSelect.Append("MAX(ModeratorAccess) AS ModeratorAccess,");
        vaccessFullSelect.Append("MAX(EditAccess) AS EditAccess,");
        vaccessFullSelect.Append("MAX(DeleteAccess) AS DeleteAccess,");
        vaccessFullSelect.Append("MAX(AdminGroup) as AdminGroup");

        vaccessFullSelect.Append(" FROM ( select");

        vaccessFullSelect.Append(
            " UserID, ForumID, ReadAccess, PostAccess, ReplyAccess, PriorityAccess, PollAccess, VoteAccess, ModeratorAccess,");
        vaccessFullSelect.Append(" EditAccess, DeleteAccess, AdminGroup");

        vaccessFullSelect.Append(" from ");
        vaccessFullSelect.AppendFormat(
            "[{0}].[{1}] b",
            this.Get<BoardConfiguration>().DatabaseOwner,
            dbCommand.Connection.GetTableName<VaccessUser>());

        vaccessFullSelect.Append(" union all select ");

        vaccessFullSelect.Append(
            " UserID, ForumID, ReadAccess, PostAccess, ReplyAccess, PriorityAccess, PollAccess, VoteAccess, ModeratorAccess,");
        vaccessFullSelect.Append(" EditAccess, DeleteAccess, AdminGroup");

        vaccessFullSelect.Append(" from ");
        vaccessFullSelect.AppendFormat(
            "[{0}].[{1}] b",
            this.Get<BoardConfiguration>().DatabaseOwner,
            dbCommand.Connection.GetTableName<VaccessGroup>());

        vaccessFullSelect.Append(" union all select ");

        vaccessFullSelect.Append(
            " UserID, ForumID, ReadAccess, PostAccess, ReplyAccess, PriorityAccess, PollAccess, VoteAccess, ModeratorAccess,");
        vaccessFullSelect.Append(" EditAccess, DeleteAccess, AdminGroup");

        vaccessFullSelect.Append(" from ");
        vaccessFullSelect.AppendFormat(
            "[{0}].[{1}] b",
            this.Get<BoardConfiguration>().DatabaseOwner,
            dbCommand.Connection.GetTableName<VaccessNull>());

        vaccessFullSelect.Append(" ) access GROUP BY UserID,ForumID");

        dbCommand.Connection.CreateView<VAccessFull>(vaccessFullSelect);

        var vaccessSelect = new StringBuilder();

        vaccessSelect.Append(" select ");

        vaccessSelect.Append(" UserID = a.UserID,");
        vaccessSelect.Append("ForumID = x.ForumID,");
        vaccessSelect.Append("IsAdmin = max(convert(int, b.Flags & 1)),");
        vaccessSelect.Append("IsForumModerator = max(convert(int, b.Flags & 8)),");

        vaccessSelect.AppendFormat(
            "IsModerator = (select count(1) from[{0}].[{1}] v1,",
            this.Get<BoardConfiguration>().DatabaseOwner,
            dbCommand.Connection.GetTableName<UserGroup>());
        vaccessSelect.AppendFormat("[{0}].[{1}] w2,", this.Get<BoardConfiguration>().DatabaseOwner, dbCommand.Connection.GetTableName<Group>());
        vaccessSelect.AppendFormat(
            "[{0}].[{1}] x,",
            this.Get<BoardConfiguration>().DatabaseOwner,
            dbCommand.Connection.GetTableName<ForumAccess>());
        vaccessSelect.AppendFormat("[{0}].[{1}] y", this.Get<BoardConfiguration>().DatabaseOwner, dbCommand.Connection.GetTableName<AccessMask>());
        vaccessSelect.Append(" where v1.UserID = a.UserID and w2.GroupID = v1.GroupID and x.GroupID = w2.GroupID");
        vaccessSelect.Append(" and y.AccessMaskID = x.AccessMaskID and (y.Flags & 64) <> 0),");

        vaccessSelect.Append("ReadAccess = max(x.ReadAccess),");
        vaccessSelect.Append("PostAccess = max(x.PostAccess),");
        vaccessSelect.Append("ReplyAccess = max(x.ReplyAccess),");
        vaccessSelect.Append("PriorityAccess = max(x.PriorityAccess),");
        vaccessSelect.Append("PollAccess = max(x.PollAccess),");
        vaccessSelect.Append("VoteAccess = max(x.VoteAccess),");
        vaccessSelect.Append("ModeratorAccess = max(x.ModeratorAccess),");
        vaccessSelect.Append("EditAccess = max(x.EditAccess),");
        vaccessSelect.Append("DeleteAccess = max(x.DeleteAccess)");

        vaccessSelect.Append(" from");

        vaccessSelect.AppendFormat(
            " [{0}].[{1}] as x WITH(NOLOCK)",
            this.Get<BoardConfiguration>().DatabaseOwner,
            dbCommand.Connection.GetTableName<VAccessFull>());
        vaccessSelect.AppendFormat(
            " INNER JOIN [{0}].[{1}] a WITH(NOLOCK) on a.UserID=x.UserID",
            this.Get<BoardConfiguration>().DatabaseOwner,
            dbCommand.Connection.GetTableName<UserGroup>());
        vaccessSelect.AppendFormat(
            " INNER JOIN [{0}].[{1}] b WITH(NOLOCK) on b.GroupID=a.GroupID",
            this.Get<BoardConfiguration>().DatabaseOwner,
            dbCommand.Connection.GetTableName<Group>());

        vaccessSelect.Append(" GROUP BY a.UserID,x.ForumID");

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
        var selectSql = """
                        [UserID] ASC,
                        [ForumID] ASC,
                        [AccessMaskID] ASC,
                        [GroupID] ASC
                        """;

        dbCommand.Connection.CreateViewIndex<VaccessUser>("UserForum_PK", selectSql);
        dbCommand.Connection.CreateViewIndex<VaccessNull>("UserForum_PK", selectSql);
        dbCommand.Connection.CreateViewIndex<VaccessGroup>("UserForum_PK", selectSql);

        return true;
    }
}