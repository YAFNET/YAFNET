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

namespace YAF.Core.Migrations;

using System;
using System.Collections.Generic;

using ServiceStack.OrmLite;

using System.Linq;
using System.Threading.Tasks;

using YAF.Configuration;
using YAF.Core.Context;
using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Types.Extensions;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

using ServiceStack.DataAnnotations;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

/// <summary>
/// Version 30 Migrations
/// </summary>
public class Migration30 : IRepositoryMigration, IHaveServiceLocator
{
    /// <summary>
    /// Migrate Repositories (Database).
    /// </summary>
    /// <param name="dbAccess">
    ///     The Database access.
    /// </param>
    public async Task MigrateDatabaseAsync(IDbAccess dbAccess)
    {
        // Install Membership Scripts
        dbAccess.Execute(db => db.Connection.CreateTableIfNotExists<AspNetUsers>());
        dbAccess.Execute(db => db.Connection.CreateTableIfNotExists<AspNetRoles>());
        dbAccess.Execute(db => db.Connection.CreateTableIfNotExists<AspNetUserClaims>());
        dbAccess.Execute(db => db.Connection.CreateTableIfNotExists<AspNetUserLogins>());
        dbAccess.Execute(db => db.Connection.CreateTableIfNotExists<AspNetUserRoles>());

        await this.MigrateLegacyUsersAsync(dbAccess);
    }

    /// <summary>
    /// Migrate old Users to Identity.
    /// </summary>
    /// <param name="dbAccess">The database access.</param>
    private async Task MigrateLegacyUsersAsync(IDbAccess dbAccess)
    {
        var guests = await this.GetRepository<User>().GetAsync(u => (u.Flags & 4) == 4);

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
                        () => new User {Flags = flags.BitValue},
                        u => u.ID == user.ID);
                });
        }

        var boards = this.GetRepository<Board>().GetAll();

        foreach (var boardId in boards.Select(board => board.ID))
        {
            // Sync Roles
            await this.Get<IAspNetRolesHelper>().SyncRolesAsync(boardId);

            var users = await this.GetRepository<User>().GetByBoardIdAsync(boardId);

            // sync users...
            await this.MigrateUsersFromTableAsync(users);
        }
    }

    /// <summary>
    /// Migrates the users from table PageUser table and import them in to Identity
    /// </summary>
    /// <param name="users">The users.</param>
    private async Task MigrateUsersFromTableAsync(IList<User> users)
    {
        foreach (var row in users)
        {
            // validate that name and email are available...
            if (row.Name.IsNotSet() || row.Email.IsNotSet())
            {
                continue;
            }

            var name = row.Name.Trim();
            var email = row.Email.ToLower().Trim();

            // clean up the name by removing commas...
            name = name.Replace(",", string.Empty);

            // verify this user & email are not empty
            if (!name.IsSet() || !email.IsSet())
            {
                continue;
            }

            // skip the guest user
            if (!row.UserFlags.IsGuest)
            {
                var userExist = await this.Get<IAspNetUsersHelper>().GetUserByEmailAsync(row.Email);

                var legacyUserProfile = await this.GetRepository<LegacyUser>().GetByIdAsync(row.ID);

                if (userExist == null)
                {
                    var result = await this.MigrateCreateUserAsync(
                        name,
                        email,
                        row.UserFlags.IsApproved);

                    var aspNetUser = result.User;

                    if (!result.Result.Succeeded)
                    {
                        this.Get<ILogger<Migration30>>().Log(
                            userId: null,
                            source: "MigrateUsers",
                            description: $"Failed to create user {name}: {result.Result.Errors.FirstOrDefault()}");
                    }
                    else
                    {
                        // update the YAF table with the ProviderKey -- update the provider table if this is the YAF provider...
                        await this.GetRepository<User>().UpdateOnlyAsync(
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

                            await this.Get<IAspNetUsersHelper>().UpdateUserAsync(aspNetUser);
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
                    await this.GetRepository<User>().UpdateOnlyAsync(
                        () => new User { ProviderUserKey = userExist.Id },
                        u => u.ID == row.ID);

                    // setup roles for this user...
                    var groups = this.GetRepository<UserGroup>().List(row.ID);

                    groups.ForEach(
                        group => this.Get<IAspNetRolesHelper>().AddUserToRole(userExist, group.Name));
                }
            }
        }
    }

    /// <summary>
    /// Migrates the create user.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="email">The email.</param>
    /// <param name="approved">The approved.</param>
    /// <returns>
    /// The migrate create user.
    /// </returns>
    private async Task<(IdentityResult Result, AspNetUsers User)> MigrateCreateUserAsync(
        string name,
        string email,
        bool approved)
    {
        var password = PasswordGenerator.GeneratePassword(true, true, true, true, false, 16);

        var user = new AspNetUsers
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

        return (await this.Get<IAspNetUsersHelper>().CreateUserAsync(user, password), user);
    }

    /// <summary>
    /// Gets ServiceLocator.
    /// </summary>
    /// <value>The service locator.</value>
    public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;
}

/// <summary>
/// A class which represents the PageUser table.
/// </summary>
[Serializable]
[Alias("PageUser")]
public class LegacyUser : IEntity, IHaveBoardID, IHaveID
{
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
}