/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

#region Using

using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;

using YAF.Configuration;
using YAF.Core.Context;
using YAF.Core.Extensions;
using YAF.Core.Handlers;
using YAF.Core.Helpers;
using YAF.Core.Utilities.StringUtils;
using YAF.Types;
using YAF.Types.Attributes;
using YAF.Types.Constants;
using YAF.Types.EventProxies;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Events;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models.Identity;

#endregion

/// <summary>
/// The class that all YAF forum pages are derived from.
/// </summary>
public abstract class ForumPage : UserControl,
                                  IRaiseControlLifeCycles,
                                  IHaveServiceLocator,
                                  ILocatablePage,
                                  IHaveLocalization
{
    #region Constants and Fields

    /// <summary>
    ///   The trans page.
    /// </summary>
    private readonly string transPage;

    /// <summary>
    /// The Unicode Encoder
    /// </summary>
    private readonly UnicodeEncoder unicodeEncoder;

    /// <summary>
    ///   The top page control.
    /// </summary>
    private Control topPageControl;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "ForumPage" /> class.
    /// </summary>
    protected ForumPage()
        : this(string.Empty, ForumPages.Board)
    {
        this.unicodeEncoder = new UnicodeEncoder();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForumPage"/> class.
    /// </summary>
    /// <param name="transPage">
    /// The trans page.
    /// </param>
    /// <param name="pageType">
    /// The page Type.
    /// </param>
    protected ForumPage([CanBeNull] string transPage, ForumPages pageType)
    {
        this.Get<IInjectServices>().Inject(this);

        BoardContext.Current.CurrentForumPage = this;

        // create empty hashtable for cache entries));
        this.PageCache = new Hashtable();

        this.PageType = pageType;
        this.transPage = transPage;
        this.Init += this.ForumPage_Init;
        this.Load += this.ForumPage_Load;
        this.Unload += this.ForumPage_Unload;
        this.PreRender += this.ForumPage_PreRender;

        this.unicodeEncoder = new UnicodeEncoder();
    }

    #endregion

    #region Events

    /// <summary>
    ///   The forum page rendered.
    /// </summary>
    public event EventHandler<ForumPageRenderedArgs> ForumPageRendered;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets DataCache.
    /// </summary>
    [Inject]
    public IDataCache DataCache { get; set; }

    /// <summary>
    ///   Gets or sets ForumFooter.
    /// </summary>
    public Control ForumFooter { get; set; }

    /// <summary>
    ///   Gets or sets ForumHeader.
    /// </summary>
    public Control ForumHeader { get; set; }

    /// <summary>
    ///   Gets or sets ForumTopControl.
    /// </summary>
    public PlaceHolder ForumTopControl { get; set; }

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
    public virtual bool IsHostAdminOnly => false;

    /// <summary>
    ///   Gets or sets a value indicating whether Is Protected.
    /// </summary>
    public virtual bool IsProtected { get; protected set; } = true;

    /// <summary>
    ///   Gets or sets a value indicating whether IsRegisteredPage.
    /// </summary>
    public bool IsRegisteredPage { get; protected set; }

    /// <summary>
    ///   Gets the Localization.
    /// </summary>
    public ILocalization Localization => this.Get<ILocalization>();

    /// <summary>
    ///   Gets or sets Logger.
    /// </summary>
    [Inject]
    public ILoggerService Logger { get; set; }

    /// <summary>
    ///   Gets cache associated with this page.
    /// </summary>
    public Hashtable PageCache { get; }

    /// <summary>
    ///   Gets the current forum Context (helper reference)
    /// </summary>
    public BoardContext PageBoardContext => this.PageBoardContext();

    /// <summary>
    ///   Gets PageName.
    /// </summary>
    [NotNull]
    public virtual string PageName => this.GetType().Name.Replace("aspx", string.Empty);

    /// <summary>
    /// Gets or sets the page type.
    /// </summary>
    public ForumPages PageType { get; set; }

    /// <summary>
    ///   Gets or sets RefreshTime.
    /// </summary>
    public int RefreshTime { get; set; }

    /// <summary>
    ///   Gets or sets Adds a message that is displayed to the user when the page is loaded.
    /// </summary>
    public string RefreshURL { get; set; }

    /// <summary>
    ///   Gets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator => this.PageBoardContext().ServiceLocator;

    /// <summary>
    ///   Gets or sets a value indicating whether ShowFooter.
    /// </summary>
    public bool ShowFooter { get; protected set; } = Config.ShowFooter;

    /// <summary>
    ///   Gets or sets a value indicating whether
    ///   if you don't want the menus at top and bottom.
    ///   Only admin pages will set this to false
    /// </summary>
    public bool ShowToolBar { get; protected set; } = Config.ShowToolBar;

    /// <summary>
    ///   Gets TopPageControl.
    /// </summary>
    public Control TopPageControl
    {
        get
        {
            if (this.topPageControl != null)
            {
                return this.topPageControl;
            }

            if (this.Page?.Header != null)
            {
                this.topPageControl = this.Page.Header;
            }
            else
            {
                this.topPageControl = this.FindControlRecursiveBoth("YafHead") ?? this.ForumTopControl;
            }

            return this.topPageControl;
        }
    }

    /// <summary>
    ///   Gets the current user.
    /// </summary>
    public AspNetUsers User => this.PageBoardContext.MembershipUser;

    /// <summary>
    /// Gets or sets a value indicating whether no data base, Should only be set by the page that initialized the database.
    /// </summary>
    protected bool NoDataBase { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Encodes the HTML
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>Returns the Encoded String</returns>
    [CanBeNull]
    public string HtmlEncode([NotNull] object data)
    {
        return data is not string ? null : this.unicodeEncoder.XSSEncode(data.ToString());
    }

    /// <summary>
    /// Redirects if no access.
    /// </summary>
    public void RedirectNoAccess()
    {
        this.Get<IPermissions>().HandleRequest(ViewPermissions.RegisteredUsers);
    }

    #endregion

    #region Implemented Interfaces

    #region IRaiseControlLifeCycles

    /// <summary>
    /// Raise init.
    /// </summary>
    void IRaiseControlLifeCycles.RaiseInit()
    {
        this.OnInit(EventArgs.Empty);
    }

    /// <summary>
    /// The raise load.
    /// </summary>
    void IRaiseControlLifeCycles.RaiseLoad()
    {
        this.OnLoad(EventArgs.Empty);
    }

    /// <summary>
    /// The raise pre render.
    /// </summary>
    void IRaiseControlLifeCycles.RaisePreRender()
    {
        this.OnPreRender(EventArgs.Empty);
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// Creates the page links.
    /// </summary>
    protected virtual void CreatePageLinks()
    {
        // Page link creation goes to this method (overloads in descendant classes)
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    protected override void OnPreRender([NotNull] EventArgs e)
    {
        base.OnPreRender(e);
    }

    /// <summary>
    /// Writes the document
    /// </summary>
    /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the server control content.</param>
    protected override void Render([NotNull] HtmlTextWriter writer)
    {
        base.Render(writer);

        // handle additional rendering if desired...
        this.ForumPageRendered?.Invoke(this, new ForumPageRenderedArgs(writer));
    }

    /// <summary>
    /// Called first to initialize the context
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void ForumPage_Init([NotNull] object sender, [NotNull] EventArgs e)
    {
        this.Get<IRaiseEvent>().Raise(new ForumPageInitEvent());

        if (this.NoDataBase)
        {
            return;
        }

        // set the current translation page...
        this.Get<LocalizationProvider>().TranslationPage = this.transPage;

        // fire pre-load event...
        this.Get<IRaiseEvent>().Raise(new ForumPagePreLoadEvent());
    }

    /// <summary>
    /// Called when page is loaded
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void ForumPage_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (!this.IsPostBack)
        {
            this.CreatePageLinks();
        }

        // fire pre-load event...
        this.Get<IRaiseEvent>().Raise(new ForumPagePostLoadEvent());
    }

    /// <summary>
    /// Called when the page is pre rendered
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void ForumPage_PreRender([NotNull] object sender, [NotNull] EventArgs e)
    {
        this.Get<IRaiseEvent>().Raise(new ForumPagePreRenderEvent());

        this.ForumFooter.Visible = this.ShowFooter;
    }

    /// <summary>
    /// Called when the page is unloaded
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void ForumPage_Unload([NotNull] object sender, [NotNull] EventArgs e)
    {
        this.Get<IRaiseEvent>().Raise(new ForumPageUnloadEvent());

        // release cache
        this.PageCache?.Clear();
    }

    #endregion
}