﻿/* Yet Another Forum.NET
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

namespace YAF.Core.Migrations;

using System;
using System.Collections.Generic;

using ServiceStack.OrmLite;

using System.Linq;

using Microsoft.AspNet.Identity;

using YAF.Configuration;
using YAF.Core.Context;
using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Types.Extensions;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

using ServiceStack.DataAnnotations;

/// <summary>
/// Version 30 Migrations
/// </summary>
public class Migration30 : IRepositoryMigration, IHaveServiceLocator
{
    /// <summary>
    /// Migrate Repositories (Database).
    /// </summary>
    /// <param name="dbAccess">
    /// The Database access.
    /// </param>
    public void MigrateDatabase(IDbAccess dbAccess)
    {
        if (!Config.IsDotNetNuke)
        {
            // Install Membership Scripts
            dbAccess.Execute(db => db.Connection.CreateTableIfNotExists<AspNetUsers>());
            dbAccess.Execute(db => db.Connection.CreateTableIfNotExists<AspNetRoles>());
            dbAccess.Execute(db => db.Connection.CreateTableIfNotExists<AspNetUserClaims>());
            dbAccess.Execute(db => db.Connection.CreateTableIfNotExists<AspNetUserLogins>());
            dbAccess.Execute(db => db.Connection.CreateTableIfNotExists<AspNetUserRoles>());
        }

        this.MigrateLegacyUsers(dbAccess);
    }

    /// <summary>
    /// Migrate old Users to Identity.
    /// </summary>
    /// <param name="dbAccess">The database access.</param>
    private void MigrateLegacyUsers(IDbAccess dbAccess)
    {
        var guests = this.GetRepository<User>().Get(u => (u.Flags & 4) == 4);

        if (guests.NullOrEmpty())
        {
            var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

            expression.Join<UserGroup>((user, userGroup) => userGroup.UserID == user.ID)
                .Join<UserGroup, Group>((userGroup, group) => group.ID == userGroup.GroupID)
                .Where<Group>(group => (group.Flags & 2) == 2);

            var users = dbAccess.Execute(db => db.Connection.Select(expression));

            users.ForEach(
                user =>
                {
                    var flags = user.UserFlags;

                    flags.IsGuest = true;

                    this.GetRepository<User>().UpdateOnly(
                        () => new User { Flags = flags.BitValue },
                        u => u.ID == user.ID);
                });
        }

        var boards = this.GetRepository<Board>().GetAll();

        boards.ForEach(
            board =>
            {
                // Sync Roles
                this.Get<IAspNetRolesHelper>().SyncRoles(board.ID);

                var users = this.GetRepository<User>().GetByBoardId(board.ID);

                // sync users...
                MigrateUsersFromTable(users);
            });
    }

    /// <summary>
    /// Migrates the users from table User table and import them in to Identity
    /// </summary>
    /// <param name="users">The users.</param>
    private void MigrateUsersFromTable(IList<User> users)
    {
        users.ForEach(
            row =>
            {
                // validate that name and email are available...
                if (row.Name.IsNotSet() || row.Email.IsNotSet())
                {
                    return;
                }

                var name = row.Name.Trim();
                var email = row.Email.ToLower().Trim();

                // clean up the name by removing commas...
                name = name.Replace(",", string.Empty);

                // verify this user & email are not empty
                if (!name.IsSet() || !email.IsSet())
                {
                    return;
                }

                // skip the guest user
                if (!row.UserFlags.IsGuest)
                {
                    var userExist = this.Get<IAspNetUsersHelper>().GetUserByEmail(row.Email);

                    var legacyUserProfile = this.GetRepository<LegacyUser>().GetById(row.ID);

                    if (userExist == null)
                    {
                        var status = MigrateCreateUser(
                            name,
                            email,
                            row.UserFlags.IsApproved,
                            out var aspNetUser);

                        if (!status.Succeeded)
                        {
                            this.Get<ILoggerService>().Log(
                                userId: null,
                                source: "MigrateUsers",
                                description: $"Failed to create user {name}: {status.Errors.FirstOrDefault()}");
                        }
                        else
                        {
                            // update the YAF table with the ProviderKey -- update the provider table if this is the YAF provider...
                            this.GetRepository<User>().UpdateOnly(
                                () => new User { ProviderUserKey = aspNetUser.Id },
                                u => u.ID == row.ID);

                            if (legacyUserProfile != null)
                            {
                                if (legacyUserProfile.RealName.IsSet())
                                {
                                    aspNetUser.Profile_RealName = legacyUserProfile.RealName;
                                }

                                if (legacyUserProfile.Occupation.IsSet())
                                {
                                    aspNetUser.Profile_Occupation = legacyUserProfile.Occupation;
                                }

                                if (legacyUserProfile.Location.IsSet())
                                {
                                    aspNetUser.Profile_Location = legacyUserProfile.Location;
                                }

                                if (legacyUserProfile.Homepage.IsSet())
                                {
                                    aspNetUser.Profile_Homepage = legacyUserProfile.Homepage;
                                }

                                if (legacyUserProfile.Interests.IsSet())
                                {
                                    aspNetUser.Profile_Interests = legacyUserProfile.Interests;
                                }

                                if (legacyUserProfile.Weblog.IsSet())
                                {
                                    aspNetUser.Profile_Blog = legacyUserProfile.Weblog;
                                }

                                aspNetUser.Profile_Gender = legacyUserProfile.Gender;

                                this.Get<IAspNetUsersHelper>().Update(aspNetUser);
                            }

                            // setup roles for this user...
                            var groups = this.GetRepository<UserGroup>().List(row.ID);

                            groups.ForEach(
                                group => this.Get<IAspNetRolesHelper>().AddUserToRole(aspNetUser, group.Name));
                        }
                    }
                    else
                    {
                        // update the YAF table with the ProviderKey -- update the provider table if this is the YAF provider...
                        this.GetRepository<User>().UpdateOnly(
                            () => new User { ProviderUserKey = userExist.Id },
                            u => u.ID == row.ID);

                        // setup roles for this user...
                        var groups = this.GetRepository<UserGroup>().List(row.ID);

                        groups.ForEach(
                            group => this.Get<IAspNetRolesHelper>().AddUserToRole(userExist, group.Name));
                    }
                }
            });
    }

    /// <summary>
    /// Migrates the create user.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="email">The email.</param>
    /// <param name="approved">The approved.</param>
    /// <param name="user">The user.</param>
    /// <returns>
    /// The migrate create user.
    /// </returns>
    private IdentityResult MigrateCreateUser(
        string name,
        string email,
        bool approved,
        out AspNetUsers user)
    {
        var password = PasswordGenerator.GeneratePassword(true, true, true, true, false, 16);

        user = new AspNetUsers
        {
            Id = Guid.NewGuid().ToString(),
            ApplicationId = this.Get<BoardSettings>().ApplicationId,
            UserName = name,
            LoweredUserName = name.ToLower(),
            Email = email,
            IsApproved = approved,
            EmailConfirmed = true,
            Profile_Birthday = null
        };

        return this.Get<IAspNetUsersHelper>().Create(user, password);
    }

    public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;
}

/// <summary>
/// A class which represents the User table.
/// </summary>
[Serializable]
[Alias("User")]
public class LegacyUser : IEntity, IHaveBoardID, IHaveID
{
    #region Properties

    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    [Alias("UserID")]
    [AutoIncrement]
    public int ID { get; set; }

    /// <summary>
    /// Gets or sets the board id.
    /// </summary>
    [References(typeof(Board))]
    [Required]
    [Index]
    public int BoardID { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [Required]
    [Index]
    [StringLength(255)]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets RealName.
    /// </summary>
    public string RealName { get; set; }

    /// <summary>
    /// Gets or sets Occupation.
    /// </summary>
    public string Occupation { get; set; }

    /// <summary>
    /// Gets or sets Location.
    /// </summary>
    public string Location { get; set; }

    /// <summary>
    /// Gets or sets Homepage.
    /// </summary>
    public string Homepage { get; set; }

    /// <summary>
    /// Gets or sets Interests.
    /// </summary>
    public string Interests { get; set; }

    /// <summary>
    /// Gets or sets Blog.
    /// </summary>
    public string Weblog { get; set; }

    /// <summary>
    /// Gets or sets Gender.
    /// </summary>
    public int Gender { get; set; }

    #endregion
}