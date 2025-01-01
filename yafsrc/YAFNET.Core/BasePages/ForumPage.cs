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

namespace YAF.Core.BasePages;

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

using YAF.Core.Filters;
using YAF.Core.Handlers;
using YAF.Types.Attributes;

/// <summary>
/// The class that all YAF forum pages are derived from.
/// </summary>
[EnableRateLimiting("fixed")]
[PageSecurityCheck]
[UserSuspendCheck]
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
    /// Gets or sets a value indicating whether no data base, Should only be set by the page that initialized the database.
    /// </summary>
    protected bool NoDataBase { get; set; }

    /// <summary>
    /// Called before the handler method executes, after model binding is complete.
    /// </summary>
    /// <param name="context">The <see cref="PageHandlerExecutingContext"/>.</param>
    public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        this.Get<IRaiseEvent>().Raise(new ForumPageInitEvent());

        if (this.NoDataBase)
        {
            return;
        }

        // fire pre-load event...
        this.Get<IRaiseEvent>().Raise(new ForumPagePreLoadEvent());

        this.Get<IDataCache>().Remove("TopicID");
    }

    /// <summary>
    /// Called after the handler method executes, before the action result executes.
    /// </summary>
    /// <param name="context">The <see cref="PageHandlerExecutedContext"/>.</param>
    public override void OnPageHandlerExecuted(PageHandlerExecutedContext context)
    {
        // fire pre-load event...
        this.Get<IRaiseEvent>().Raise(new ForumPagePostLoadEvent());
    }

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