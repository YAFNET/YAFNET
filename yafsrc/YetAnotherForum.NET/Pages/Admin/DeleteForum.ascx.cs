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

namespace YAF.Pages.Admin;

using YAF.Core.Tasks;
using YAF.Types.Models;

/// <summary>
/// Administrative Page for the deleting of forum properties.
/// </summary>
public partial class DeleteForum : AdminPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteForum"/> class. 
    /// </summary>
    public DeleteForum()
        : base("ADMIN_DELETEFORUM", ForumPages.Admin_DeleteForum)
    {
    }

    /// <summary>
    /// Registers the needed Java Scripts
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    override protected void OnPreRender(EventArgs e)
    {
       this.PageBoardContext.PageElements.RegisterJsBlockStartup(
            nameof(JavaScriptBlocks.SelectForumsLoadJs),
            JavaScriptBlocks.SelectForumsLoadJs(
                "ForumList",
                this.GetText("ADMIN_DELETEFORUM", "NEW_FORUM"),
                false,
                false,
                this.ForumListSelected.ClientID));

        base.OnPreRender(e);
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    override protected void OnInit(EventArgs e)
    {
        this.Delete.Click += this.SaveClick;
        this.Cancel.Click += this.Cancel_Click;

        base.OnInit(e);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.IsPostBack)
        {
            return;
        }

        var forumId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAsInt("fa");

        if (!this.Get<HttpRequestBase>().QueryString.Exists("fa") && forumId.HasValue)
        {
            this.Get<LinkBuilder>().Redirect(ForumPages.Admin_Forums);
        }
        else
        {
            var forum = this.GetRepository<Forum>().GetById(forumId.Value);

            this.IconHeader.Text = $"{this.GetText("HEADER1")}: <strong>{forum.Name}</strong>";
        }
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot();
        this.PageBoardContext.PageLinks.AddAdminIndex();

        this.PageBoardContext.PageLinks.AddLink(this.GetText("TEAM", "FORUMS"), this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Forums));
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_DELETEFORUM", "TITLE"), string.Empty);
    }

    /// <summary>
    /// The update status timer_ tick.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void UpdateStatusTimerTick(object sender, EventArgs e)
    {
        // see if the migration is done....
        if (this.Get<ITaskModuleManager>().TryGetTask(ForumDeleteTask.TaskName, out var task) && task.IsRunning)
        {
            // continue...
            return;
        }

        this.UpdateStatusTimer.Enabled = false;

        this.Get<LinkBuilder>().Redirect(ForumPages.Admin_Forums);
    }

    /// <summary>
    /// Cancel Deleting and Redirecting back to The Admin Forums Page.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void Cancel_Click(object sender, EventArgs e)
    {
        this.Get<LinkBuilder>().Redirect(ForumPages.Admin_Forums);
    }

    /// <summary>
    /// Delete The Forum
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void SaveClick(object sender, EventArgs e)
    {
        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
            "BlockUiFunctionJs",
            JavaScriptBlocks.BlockUiFunctionJs("DeleteForumMessage"));

        string errorMessage;

        var newForumId = this.ForumListSelected.Value.ToType<int>();
        var oldForumId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAs<int>("fa");

        if (this.MoveTopics.Checked && newForumId != oldForumId)
        {
            // schedule...
            ForumDeleteTask.Start(
                this.PageBoardContext.PageBoardID,
                oldForumId,
                newForumId,
                out errorMessage);

            // enable timer...
            this.UpdateStatusTimer.Enabled = true;

            this.LocalizedLabel6.LocalizedTag = "DELETE_MOVE_TITLE";
        }
        else
        {
            // schedule...
            ForumDeleteTask.Start(this.PageBoardContext.PageBoardID, oldForumId, out errorMessage);

            // enable timer...
            this.UpdateStatusTimer.Enabled = true;

            this.LocalizedLabel6.LocalizedTag = "DELETE_TITLE";
        }

        if (errorMessage.IsSet())
        {
            this.PageBoardContext.Notify(errorMessage, MessageTypes.danger);
        }
    }
}