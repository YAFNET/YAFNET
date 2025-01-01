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

namespace YAF.Pages.Admin.EditUser;

using Microsoft.AspNetCore.Mvc;

using YAF.Core.Context;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

/// <summary>
/// Class UsersPointsModel.
/// Implements the <see cref="YAF.Core.BasePages.AdminPage" />
/// </summary>
/// <seealso cref="YAF.Core.BasePages.AdminPage" />
public class UsersPointsModel : AdminPage
{
    /// <summary>
    /// Gets or sets the User Data.
    /// </summary>
    public User EditUser { get; set; }

    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public UsersPointsInputModel Input { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UsersPointsModel"/> class.
    /// </summary>
    public UsersPointsModel()
        : base("ADMIN_EDITUSER", ForumPages.Admin_EditUser)
    {
    }

    /// <summary>
    /// Called when [get].
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>IActionResult.</returns>
    public IActionResult OnGet(int userId)
    {
        if (!BoardContext.Current.IsAdmin)
        {
            return this.Get<LinkBuilder>().AccessDenied();
        }

        this.Input = new UsersPointsInputModel {
            UserId = userId
        };

        return this.BindData(userId);
    }

    /// <summary>
    /// The add points_ click.
    /// </summary>
    public IActionResult OnPostAddPoints()
    {
        this.GetRepository<User>().AddPoints(this.Input.UserId, null, this.Input.AddPoints);

        return this.Get<LinkBuilder>().Redirect(ForumPages.Admin_EditUser, new {u = this.Input.UserId, tab = "View6"});
    }

    /// <summary>
    /// The remove points_ click.
    /// </summary>
    public IActionResult OnPostRemovePoints()
    {
        this.GetRepository<User>().RemovePoints(this.Input.UserId, null, this.Input.RemovePoints);

        return this.Get<LinkBuilder>().Redirect(ForumPages.Admin_EditUser, new { u = this.Input.UserId, tab = "View6" });
    }

    /// <summary>
    /// The set user points_ click.
    /// </summary>
    public IActionResult OnPostSetUserPoints()
    {
        this.GetRepository<User>().SetPoints(this.Input.UserId, this.Input.UserPoints);

        return this.Get<LinkBuilder>().Redirect(ForumPages.Admin_EditUser, new { u = this.Input.UserId, tab = "View6" });
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private IActionResult BindData(int userId)
    {
        if (this.Get<IDataCache>()[string.Format(Constants.Cache.EditUser, userId)] is not Tuple<User, AspNetUsers, Rank, VAccess> user)
        {
            return this.Get<LinkBuilder>().Redirect(
                ForumPages.Admin_EditUser,
                new {
                    u = userId
                });
        }

        this.EditUser = user.Item1;

        this.Input.UserPoints = this.EditUser.Points;

        return this.Page();
    }
}