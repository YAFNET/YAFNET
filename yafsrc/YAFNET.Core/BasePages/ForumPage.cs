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

using YAF.Core.Model;

namespace YAF.Core.BasePages;

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

using YAF.Core.Handlers;
using YAF.Types.Attributes;
using YAF.Types.Models;

/// <summary>
/// The class that all YAF forum pages are derived from.
/// </summary>
[EnableRateLimiting("fixed")]
public abstract class ForumPage : PageModel,
                                  IHaveServiceLocator,
                                  ILocatablePage,
                                  IHaveLocalization
{
    /// <summary>
    /// The Unicode Encoder
    /// </summary>
    private readonly UnicodeEncoder unicodeEncoder;

    /// <summary>
    /// Called asynchronously before the handler method is invoked, after model binding is complete.
    /// </summary>
    /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.Filters.PageHandlerExecutingContext" />.</param>
    /// <param name="next">The <see cref="T:Microsoft.AspNetCore.Mvc.Filters.PageHandlerExecutionDelegate" />. Invoked to execute the next page filter or the handler method itself.</param>
    public async override Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
    {
        this.Get<IRaiseEvent>().Raise(new ForumPageInitEvent());

        if (this.NoDataBase)
        {
            return;
        }

        // fire preload event...
        this.Get<IRaiseEvent>().Raise(new ForumPagePreLoadEvent());

        this.Get<IDataCache>().Remove("TopicID");

        // no security features for login/logout pages
        if (BoardContext.Current.CurrentForumPage.IsAccountPage)
        {
            await next.Invoke();
        }

        // check if login is required
        if (BoardContext.Current.BoardSettings.RequireLogin && BoardContext.Current.IsGuest &&
            BoardContext.Current.CurrentForumPage.IsProtected)
        {
            // redirect to login page if login is required
            var result = this.Get<IPermissions>().HandleRequest(ViewPermissions.RegisteredUsers);

            if (result != null)
            {
                context.Result = result;
                return;
            }
        }

        // check for suspension if enabled...
        if (BoardContext.Current.Globals.IsSuspendCheckEnabled && BoardContext.Current.IsSuspended)
        {
            if (this.Get<IDateTimeService>().GetUserDateTime(BoardContext.Current.SuspendedUntil)
                <= this.Get<IDateTimeService>().GetUserDateTime(DateTime.UtcNow))
            {
                await this.GetRepository<User>().SuspendAsync(BoardContext.Current.PageUserID);

                await this.Get<ISendNotification>().SendUserSuspensionEndedNotificationAsync(
                    BoardContext.Current.PageUser.Email,
                    BoardContext.Current.PageUser.DisplayOrUserName());

                this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(BoardContext.Current.PageUserID));
            }
            else
            {
                context.Result = this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Suspended);
                return;
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

                return;
            }
        }

        // Handle admin pages
        if (BoardContext.Current.CurrentForumPage.IsAdminPage)
        {
            if (!BoardContext.Current.IsAdmin)
            {
                context.Result = this.Get<ILinkBuilder>().AccessDenied();
                return;
            }

            // Load the page access list.
            var hasAccess = this.GetRepository<AdminPageUserAccess>().HasAccess(
                BoardContext.Current.PageUserID,
                BoardContext.Current.CurrentForumPage.PageName.ToString());

            // Check access rights to the page.
            if (!BoardContext.Current.PageUser.UserFlags.IsHostAdmin &&
                (!BoardContext.Current.CurrentForumPage.PageName.ToString().IsSet() || !hasAccess))
            {
                context.Result = this.Get<ILinkBuilder>()
                    .RedirectInfoPage(InfoMessage.HostAdminPermissionsAreRequired);

                return;
            }
        }

        // handle security features...
        if (BoardContext.Current.CurrentForumPage.PageName == ForumPages.Account_Register &&
            BoardContext.Current.BoardSettings.DisableRegistrations)
        {
            context.Result = this.Get<ILinkBuilder>().AccessDenied();

            return;
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

            return;
        }

        // fire preload event...
        this.Get<IRaiseEvent>().Raise(new ForumPagePostLoadEvent());

        await next.Invoke();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForumPage"/> class.
    /// </summary>
    /// <param name="transPage">
    /// The trans page.
    /// </param>
    /// <param name="page">
    /// The page.
    /// </param>
    protected ForumPage(string transPage, ForumPages page)
    {
        this.PageName = page;

        this.Get<IInjectServices>().Inject(this);

        // set the BoardContext ForumPage...
        BoardContext.Current.CurrentForumPage = this;

        // set the current translation page...
        this.Get<LocalizationProvider>().TranslationPage = transPage;

        this.Size = BoardContext.Current.PageUser.PageSize;

        this.PageTitle = this.Localization.GetText(transPage, "TITLE");

        this.unicodeEncoder = new UnicodeEncoder();

        this.PageBoardContext.PageLinks.AddRoot();

        this.CreatePageLinks();
    }

    /// <summary>
    ///   Gets or sets DataCache.
    /// </summary>
    [Inject]
    public IDataCache DataCache { get; set; }

    /// <summary>
    /// Creates the page links.
    /// </summary>
    public virtual void CreatePageLinks()
    {
        // Page link creation goes to this method (overloads in descendant classes)
    }

    /// <summary>
    /// Gets or sets a value indicating whether is account page.
    /// </summary>
    public virtual bool IsAccountPage { get; protected set; }

    /// <summary>
    ///   Gets or sets a value indicating whether Is Admin Page.
    /// </summary>
    public virtual bool IsAdminPage { get; protected set; }

    /// <summary>
    ///   Gets a value indicating whether Is Host Admin Only.
    /// </summary>
    public virtual bool IsHostAdminOnly { get; protected set; }

    /// <summary>
    ///   Gets or sets a value indicating whether Is Protected.
    /// </summary>
    public virtual bool IsProtected { get; protected set; } = true;

    /// <summary>
    ///   Gets or sets a value indicating whether IsRegisteredPage.
    /// </summary>
    public bool IsRegisteredPage { get; protected set; }

    /// <summary>
    ///   Gets or sets the PageName.
    /// </summary>
    public ForumPages PageName { get; set; }

    /// <summary>
    /// Gets or sets the Page Size.
    /// </summary>
    [BindProperty]
    public int Size { get; set; }

    /// <summary>
    /// Gets or sets the pageSize List.
    /// </summary>
    public SelectList PageSizeList { get; set; }

    /// <summary>
    ///   Gets the Localization.
    /// </summary>
    public ILocalization Localization => this.Get<ILocalization>();

    /// <summary>
    ///   Gets the current forum Context (helper reference)
    /// </summary>
    public BoardContext PageBoardContext => BoardContext.Current;

    /// <summary>
    /// Gets or sets the page title.
    /// </summary>
    /// <value>The page title.</value>
    public string PageTitle { get; set; }

    /// <summary>
    ///   Gets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

    /// <summary>
    ///   Gets the current user.
    /// </summary>
    public AspNetUsers AspNetUser => this.PageBoardContext.MembershipUser;

    /// <summary>
    /// Gets or sets a value indicating whether no database, Should only be set by the page that initialized the database.
    /// </summary>
    protected bool NoDataBase { get; set; }

    /// <summary>
    /// Encodes the HTML
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>Returns the Encoded String</returns>
    public string HtmlEncode(object data)
    {
        return data is not string ? null : this.unicodeEncoder.XSSEncode(data.ToString());
    }
}