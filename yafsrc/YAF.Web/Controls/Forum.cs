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

namespace YAF.Web.Controls;

using YAF.Core.BasePages;

/// <summary>
/// The YAF Forum Control
/// </summary>
[ToolboxData("<{0}:Forum runat=\"server\"></{0}:Forum>")]
public class Forum : UserControl, IHaveServiceLocator
{
    /// <summary>
    ///   The current forum page.
    /// </summary>
    private ForumPage currentForumPage;

    /// <summary>
    ///   The page.
    /// </summary>
    private ILocatablePage page;

    /// <summary>
    ///   Initializes a new instance of the <see cref="Forum" /> class.
    /// </summary>
    public Forum()
    {
        // init the modules and run them immediately...
        var baseModules = this.Get<IModuleManager<IBaseForumModule>>();

        baseModules.GetAll().ForEach(
            module =>
                {
                    module.ForumControlObj = this;
                    module.Init();
                });
    }

    /// <summary>
    ///   The after forum page load.
    /// </summary>
    public event EventHandler<AfterForumPageLoad> AfterForumPageLoad;

    /// <summary>
    ///   The before forum page load.
    /// </summary>
    public event EventHandler<BeforeForumPageLoad> BeforeForumPageLoad;

    /// <summary>
    /// Occurs when [initialize forum page].
    /// </summary>
    public event EventHandler<InitForumPage> InitForumPage;

    /// <summary>
    ///   The page title set.
    /// </summary>
    public event EventHandler<ForumPageTitleArgs> PageTitleSet;

    /// <summary>
    ///   Gets or sets the Board ID for this instance of the forum control, overriding the value defined in app.config.
    /// </summary>
    public int BoardID
    {
        get => this.Get<ControlSettings>().BoardID;

        set => this.Get<ControlSettings>().BoardID = value;
    }

    /// <summary>
    ///   Gets or sets the CategoryID for this instance of the forum control
    /// </summary>
    public int CategoryID
    {
        get => this.Get<ControlSettings>().CategoryID;

        set => this.Get<ControlSettings>().CategoryID = value;
    }

    /// <summary>
    ///   Gets or sets The forum footer control
    /// </summary>
    public Control Footer { get; set; }

    /// <summary>
    ///   Gets or sets The forum header control
    /// </summary>
    public Control Header { get; set; }

    /// <summary>
    ///   Gets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

    /// <summary>
    /// Called when the forum control sets it's Page Title
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    public void FirePageTitleSet(object sender, ForumPageTitleArgs e)
    {
        this.PageTitleSet?.Invoke(this, e);
    }

    /// <summary>
    /// The on unload.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    override protected void OnUnload(EventArgs e)
    {
        base.OnUnload(e);

        // make sure the BoardContext is disposed of...
        BoardContext.Current.Dispose();
    }

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    override protected void Render(HtmlTextWriter writer)
    {
        // wrap the forum in one main div and then a page div for better CSS selection
        writer.WriteLine();

        writer.Write("""<div id="{0}" class="yafnet">""", this.ClientID);

        writer.Write("""<div class="page-{0}">""", this.page.PageName.ToLower().Replace("_", "-"));

        // render the forum
        base.Render(writer);

        writer.WriteLine("</div></div>");
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    override protected void OnInit(EventArgs e)
    {
        // handle script manager first...
        var yafScriptManager = ScriptManager.GetCurrent(this.Page);

        if (yafScriptManager != null)
        {
            yafScriptManager.EnableCdn = this.Get<BoardSettings>().ScriptManagerScriptsCDNHosted;
            yafScriptManager.EnableScriptLocalization = !this.Get<BoardSettings>().ScriptManagerScriptsCDNHosted;
            yafScriptManager.EnableCdnFallback = this.Get<BoardSettings>().ScriptManagerScriptsCDNHosted;

            return;
        }

        // add a script manager since one doesn't exist...
        yafScriptManager = new ScriptManager
                               {
                                   ID = "YafScriptManager",
                                   EnablePartialRendering = true,
                                   EnableCdn = this.Get<BoardSettings>().ScriptManagerScriptsCDNHosted,
                                   EnableCdnFallback = this.Get<BoardSettings>().ScriptManagerScriptsCDNHosted,
                                   EnableScriptLocalization =
                                       !this.Get<BoardSettings>().ScriptManagerScriptsCDNHosted
                               };

        this.Controls.Add(yafScriptManager);

        this.InitForumPage?.Invoke(this, new InitForumPage());

        base.OnInit(e);
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
    /// </summary>
    /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    override protected void OnLoad(EventArgs e)
    {
        // context is ready to be loaded, call the before page load event...
        this.BeforeForumPageLoad?.Invoke(this, new BeforeForumPageLoad());

        // add the forum header control...
        var topControl = new PlaceHolder();
        this.Controls.AddAt(0, topControl);

        // get the current page...
        var src = this.GetPageSource();

        try
        {
            this.currentForumPage = (ForumPage)this.LoadControl(src);

            this.Header = this.LoadControl($"{BoardInfo.ForumServerFileRoot}controls/Header.ascx");

            this.Footer = new Footer();
        }
        catch (FileNotFoundException)
        {
            throw new ArgumentNullException($"Failed to load {src}.");
        }

        this.currentForumPage.ForumTopControl = topControl;
        this.currentForumPage.ForumFooter = this.Footer;

        this.currentForumPage.ForumHeader = this.Header;

        // only show header if show toolbar is enabled
        this.currentForumPage.ForumHeader.Visible = this.currentForumPage.ShowToolBar;

        // set the BoardContext ForumPage...
        BoardContext.Current.CurrentForumPage = this.currentForumPage;

        // add the header control before the page rendering...
        this.Controls.AddAt(1, this.Header);

        // Add the LoginBox to Control, if used and User is Guest
        if (BoardContext.Current.IsGuest && !Config.IsAnyPortal && Config.AllowLoginAndLogoff && !this.page.IsAccountPage)
        {
            this.Controls.Add(this.LoadControl($"{BoardInfo.ForumServerFileRoot}Dialogs/LoginBox.ascx"));
        }

        this.Controls.Add(this.currentForumPage);

        // add the footer control after the page...
        this.Controls.Add(this.Footer);

        const string CookieName = "YAF-AcceptCookies";

        if (BoardContext.Current.Get<HttpRequestBase>().Cookies[CookieName] == null
            && this.Get<BoardSettings>().ShowCookieConsent && !Config.IsAnyPortal)
        {
            // Add cookie consent
            this.Controls.Add(this.LoadControl($"{BoardInfo.ForumServerFileRoot}controls/CookieConsent.ascx"));
        }

        if (this.Get<BoardSettings>().ShowScrollBackToTopButton)
        {
            // Add Scroll top button
            this.Controls.Add(this.LoadControl($"{BoardInfo.ForumServerFileRoot}controls/ScrollTop.ascx"));
        }

        // load plugins/functionality modules
        this.AfterForumPageLoad?.Invoke(this, new AfterForumPageLoad());

        base.OnLoad(e);
    }

    /// <summary>
    /// Gets the page source.
    /// </summary>
    /// <returns>
    /// The get page source.
    /// </returns>
    private string GetPageSource()
    {
        var pages = this.Get<IEnumerable<ILocatablePage>>().ToList();

        if (this.Get<HttpRequestBase>().QueryString.Exists("g"))
        {
            var pageQuery = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("g");

            this.page = pages.GetPage(pageQuery);
        }

        this.page ??= pages.GetPage("Board");

        var src = $"{BoardInfo.ForumServerFileRoot}pages/{this.page.PageName}.ascx";

        var replacementPaths = new List<string> { "Profile", "Account", "moderate", "admin", "help" };

        replacementPaths.Where(path => src.IndexOf($"/{path}_", StringComparison.Ordinal) >= 0)
            .ForEach(path => src = src.Replace($"/{path}_", $"/{path}/"));

        return src;
    }
}