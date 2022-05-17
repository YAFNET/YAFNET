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

namespace YAF.Pages;

using YAF.Types.Models;

/// <summary>
/// The Private Message Page
/// </summary>
public partial class MyMessages : ForumPageRegistered
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "MyMessages" /> class.
    /// </summary>
    public MyMessages()
        : base("PM", ForumPages.MyMessages)
    {
    }

    /// <summary>
    ///   Gets View.
    /// </summary>
    protected PmView View { get; private set; }

    /// <summary>
    /// The On PreRender event.
    /// </summary>
    /// <param name="e">
    /// the Event Arguments
    /// </param>
    protected override void OnPreRender([NotNull] EventArgs e)
    {
        // setup jQuery and Jquery Ui Tabs.
        this.PageBoardContext.PageElements.RegisterJsBlock(
            "yafPmTabsJs",
            JavaScriptBlocks.BootstrapTabsLoadJs(this.PmTabs.ClientID, this.hidLastTab.ClientID));

        base.OnPreRender(e);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        // check if this feature is disabled
        if (!this.PageBoardContext.BoardSettings.AllowPrivateMessages)
        {
            this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Disabled);
        }

        if (this.IsPostBack)
        {
            return;
        }

        if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("v").IsSet())
        {
            this.View = PmViewConverter.FromQueryString(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("v"));

            this.hidLastTab.Value = $"View{(int)this.View}";
        }

        this.NewPM.NavigateUrl = this.Get<LinkBuilder>().GetLink(ForumPages.PostPrivateMessage);
        this.NewPM2.NavigateUrl = this.NewPM.NavigateUrl;

        // Renew PM Statistics
        var count = this.GetRepository<PMessage>().UserMessageCount(this.PageBoardContext.PageUserID);

        if (count != null)
        {
            this.InfoInbox.Text = this.InfoArchive.Text = this.InfoOutbox.Text = this.GetPMessageText(
                                                              "PMLIMIT_ALL",
                                                              count.NumberTotal,
                                                              count.InboxCount,
                                                              count.OutBoxCount,
                                                              count.ArchivedCount,
                                                              count.Allowed);
        }
    }

    /// <summary>
    /// Gets the message text.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="total">The total.</param>
    /// <param name="inbox">The inbox.</param>
    /// <param name="outbox">The outbox.</param>
    /// <param name="archive">The archive.</param>
    /// <param name="limit">The limit.</param>
    /// <returns>Returns the Message Text</returns>
    protected string GetPMessageText(
        [NotNull] string text,
        [NotNull] int total,
        [NotNull] int inbox,
        [NotNull] int outbox,
        [NotNull] int archive,
        [NotNull] int limit)
    {
        decimal percentage = 0;

        if (limit != 0)
        {
            percentage = decimal.Round(total / limit * 100, 2);
        }

        if (!this.PageBoardContext.IsAdmin)
        {
            return this.HtmlEncode(this.GetTextFormatted(text, total, inbox, outbox, archive, limit, percentage));
        }

        percentage = 0;

        return this.HtmlEncode(this.GetTextFormatted(text, total, inbox, outbox, archive, "\u221E", percentage));
    }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    protected override void CreatePageLinks()
    {
        this.PageLinks.AddRoot();
        this.PageLinks.AddLink(this.PageBoardContext.PageUser.DisplayOrUserName(), this.Get<LinkBuilder>().GetLink(ForumPages.MyAccount));
        this.PageLinks.AddLink(this.GetText("TITLE"));
    }
}