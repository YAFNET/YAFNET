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

namespace YAF.Core.Filters;

using System;
using System.Threading.Tasks;

using YAF.Core.Model;
using YAF.Types.Models;

/// <summary>
/// Page Security Check
/// Implements the <see cref="ResultFilterAttribute" />
/// Implements the <see cref="IHaveServiceLocator" />
/// </summary>
/// <seealso cref="ResultFilterAttribute" />
/// <seealso cref="IHaveServiceLocator" />
[AttributeUsage(AttributeTargets.Class)]
public class PageSecurityCheckAttribute : ResultFilterAttribute, IHaveServiceLocator
{
    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

    /// <summary>
    /// On result execution as an asynchronous operation.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="next">The next.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    /// <inheritdoc />
    public override Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        // no security features for login/logout pages
        if (!BoardContext.Current.CurrentForumPage.IsAccountPage)
        {
            // check if login is required
            if (BoardContext.Current.BoardSettings.RequireLogin && BoardContext.Current.IsGuest &&
                BoardContext.Current.CurrentForumPage.IsProtected)
            {
                // redirect to login page if login is required
                var result = this.Get<IPermissions>().HandleRequest(ViewPermissions.RegisteredUsers);

                if (result != null)
                {
                    context.Result = result;
                }
            }

            // check if it's a "registered user only page" and check permissions.
            if (BoardContext.Current.CurrentForumPage.IsRegisteredPage &&
                BoardContext.Current.CurrentForumPage.AspNetUser == null)
            {
                var result = this.Get<IPermissions>().HandleRequest(ViewPermissions.RegisteredUsers);

                if (result != null)
                {
                    context.Result = result;
                }
            }

            // Handle admin pages
            if (BoardContext.Current.CurrentForumPage.IsAdminPage)
            {
                if (!BoardContext.Current.IsAdmin)
                {
                    context.Result = this.Get<LinkBuilder>().AccessDenied();
                }
                else
                {
                    // Load the page access list.
                    var hasAccess = this.GetRepository<AdminPageUserAccess>().HasAccess(
                        BoardContext.Current.PageUserID,
                        BoardContext.Current.CurrentForumPage.PageName.ToString());

                    // Check access rights to the page.
                    if (!BoardContext.Current.PageUser.UserFlags.IsHostAdmin &&
                        (!BoardContext.Current.CurrentForumPage.PageName.ToString().IsSet() || !hasAccess))
                    {
                        context.Result = this.Get<LinkBuilder>()
                            .RedirectInfoPage(InfoMessage.HostAdminPermissionsAreRequired);
                    }
                }
            }

            // handle security features...
            if (BoardContext.Current.CurrentForumPage.PageName == ForumPages.Account_Register &&
                BoardContext.Current.BoardSettings.DisableRegistrations)
            {
                context.Result = this.Get<LinkBuilder>().AccessDenied();
            }

            // check access permissions for specific pages...
            var resultPermission = BoardContext.Current.CurrentForumPage.PageName switch
            {
                ForumPages.ActiveUsers => this.Get<IPermissions>()
                    .HandleRequest((ViewPermissions)BoardContext.Current.BoardSettings.ActiveUsersViewPermissions),
                ForumPages.Members => this.Get<IPermissions>()
                    .HandleRequest((ViewPermissions)BoardContext.Current.BoardSettings.MembersListViewPermissions),
                ForumPages.UserProfile or ForumPages.Albums or ForumPages.Album => this.Get<IPermissions>()
                    .HandleRequest((ViewPermissions)BoardContext.Current.BoardSettings.ProfileViewPermissions),
                ForumPages.Search => this.Get<IPermissions>()
                    .HandleRequest((ViewPermissions)BoardContext.Current.BoardSettings.SearchPermissions),
                _ => null
            };

            if (resultPermission != null)
            {
                context.Result = resultPermission;
            }
        }

        return next.Invoke();
    }
}