/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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
using YAF.Types.Attributes;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

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
    public InputModel Input { get; set; }

    public UsersPointsModel()
        : base("ADMIN_EDITUSER", ForumPages.Admin_EditUser)
    {
    }

    public IActionResult OnGet(int userId)
    {
        if (!BoardContext.Current.IsAdmin)
        {
            return this.Get<LinkBuilder>().AccessDenied();
        }

        this.Input = new InputModel
                     {
                         UserId = userId
                     };

        this.BindData(userId);

        return this.Page();
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
    private void BindData(int userId)
    {
        var user =
            this.Get<IDataCache>()[string.Format(Constants.Cache.EditUser, userId)] as
                Tuple<User, AspNetUsers, Rank, VAccess>;

        this.EditUser = user.Item1;

        this.Input.UserPoints = this.EditUser.Points;
    }

    /// <summary>
    /// The input model.
    /// </summary>
    public class InputModel
    {
        public int UserId { get; set; }

        public int RemovePoints { get; set; }

        public int AddPoints { get; set; }

        public int UserPoints { get; set; }
    }
}