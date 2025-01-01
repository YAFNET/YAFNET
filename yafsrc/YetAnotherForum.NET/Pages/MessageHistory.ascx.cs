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

namespace YAF.Pages;

using YAF.Core.Extensions;
using YAF.Core.Utilities.StringUtils;
using YAF.Types.Models;

/// <summary>
/// Post Message History Page.
/// </summary>
public partial class MessageHistory : ForumPage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "MessageHistory" /> class.
    /// </summary>
    public MessageHistory()
        : base("MESSAGEHISTORY", ForumPages.MessageHistory)
    {
    }

    /// <summary>
    /// Gets or sets the revisions count.
    /// </summary>
    /// <value>
    /// The revisions count.
    /// </value>
    protected int RevisionsCount { get; set; }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    override protected void OnInit(EventArgs e)
    {
        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
            this,
            nameof(JavaScriptBlocks.ToggleDiffSelectionJs),
            JavaScriptBlocks.ToggleDiffSelectionJs(this.GetText("SELECT_TWO")));

        base.OnInit(e);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.PageBoardContext.IsGuest)
        {
            this.Get<LinkBuilder>().AccessDenied();
        }

        if (this.PageBoardContext.PageMessage != null)
        {
            this.ReturnBtn.Visible = true;
        }

        if (this.Get<HttpRequestBase>().QueryString.Exists("f"))
        {
            // We check here if the user have access to the option
            if (this.PageBoardContext.IsGuest)
            {
                this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.AccessDenied);
            }

            this.ReturnModBtn.Visible = true;
        }

        var originalMessage = this.GetRepository<Message>().GetMessageWithAccess(this.PageBoardContext.PageMessage.ID, this.PageBoardContext.PageUserID);

        if (originalMessage == null)
        {
            this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        if (this.IsPostBack)
        {
            return;
        }

        this.PageBoardContext.PageLinks.AddForum(this.PageBoardContext.PageForum);
        this.PageBoardContext.PageLinks.AddTopic(this.PageBoardContext.PageTopic);

        this.PageBoardContext.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);

        this.BindData();
    }

    /// <summary>
    /// The create page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot();
    }

    /// <summary>
    /// Redirect to the changed post
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void ReturnBtn_OnClick(object sender, EventArgs e)
    {
        this.Get<LinkBuilder>().Redirect(
            ForumPages.Posts,
            new { m = this.PageBoardContext.PageMessage.ID, name = this.PageBoardContext.PageTopic.TopicName });
    }

    /// <summary>
    /// Redirect to the changed post
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void ReturnModBtn_OnClick(object sender, EventArgs e)
    {
        this.Get<LinkBuilder>().Redirect(ForumPages.Moderate_ReportedPosts, new { f = this.PageBoardContext.PageForumID });
    }

    /// <summary>
    /// Handle Commands for restoring an old Message Version
    /// </summary>
    /// <param name="source">The source of the event.</param>
    /// <param name="e">The <see cref="RepeaterCommandEventArgs"/> instance containing the event data.</param>
    protected void RevisionsList_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "restore":
                var edited = e.CommandArgument.ToType<DateTime>();

                var messageToRestore = this.GetRepository<Types.Models.MessageHistory>().GetSingle(
                    m => m.MessageID == this.PageBoardContext.PageMessage.ID && m.Edited == edited);

                if (messageToRestore != null)
                {
                    var messageUser = this.GetRepository<User>().GetById(this.PageBoardContext.PageMessage.UserID);

                    this.GetRepository<Message>().Update(
                        null,
                        messageToRestore.Message,
                        null,
                        null,
                        null,
                        null,
                        messageToRestore.EditReason,
                        this.PageBoardContext.PageUserID != this.PageBoardContext.PageMessage.UserID,
                        this.PageBoardContext.IsAdmin || this.PageBoardContext.ForumModeratorAccess,
                        this.PageBoardContext.PageTopic,
                        this.PageBoardContext.PageMessage,
                        this.PageBoardContext.PageForum,
                        messageUser,
                        this.PageBoardContext.PageUserID);

                    this.PageBoardContext.Notify(this.GetText("MESSAGE_RESTORED"), MessageTypes.success);

                    this.BindData();
                }

                break;
        }
    }

    /// <summary>
    /// The get IP address.
    /// </summary>
    /// <param name="dataItem">
    /// The data item.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    protected string GetIpAddress(MessageHistoryTopic dataItem)
    {
        var ip = IPHelper.GetIpAddressAsString(dataItem.IP);

        return ip.IsSet() ? ip : IPHelper.GetIpAddressAsString(dataItem.MessageIP);
    }

    /// <summary>
    /// Render Diff Message
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ShowDiffClick(object sender, EventArgs e)
    {
        var dmp = new DiffMatchPatch();

        var messages = this.RevisionsList.Items.Cast<RepeaterItem>()
            .Where(item => item.FindControlAs<CheckBox>("Compare").Checked)
            .Select(item => item.FindControlAs<HiddenField>("MessageField").Value);

        if (messages.NullOrEmpty())
        {
            this.PageBoardContext.Notify(this.GetText("MESSAGEHISTORY", "NOTHING_SELECTED"), MessageTypes.warning);
            return;
        }

        if (messages.Count() == 1)
        {
            this.PageBoardContext.Notify(this.GetText("MESSAGEHISTORY", "SELECT_BOTH"), MessageTypes.warning);
            return;
        }

        var text1 = messages.ElementAt(0);
        var text2 = messages.ElementAt(1);

        var diff = dmp.DiffMain(text1, text2, true);

        dmp.CleanupSemantic(diff);

        this.DiffView.Text = dmp.PrettyHtml(diff);

        this.InfoSelect.Visible = false;
    }

    /// <summary>
    /// Binds data to data source
    /// </summary>
    private void BindData()
    {
        // Fill revisions list repeater.
        var revisionsTable = this.GetRepository<Types.Models.MessageHistory>().List(
            this.PageBoardContext.PageMessage.ID,
            this.PageBoardContext.BoardSettings.MessageHistoryDaysToLog);

        this.RevisionsCount = revisionsTable.Count;

        this.RevisionsList.DataSource = revisionsTable;

        this.DataBind();
    }
}