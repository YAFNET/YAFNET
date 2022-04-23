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

namespace YAF.Pages.Admin;

#region Using

using YAF.Core.Tasks;
using YAF.Types.Models;

#endregion

/// <summary>
/// Administrative Page for the deleting of forum properties.
/// </summary>
public partial class DeleteForum : AdminPage
{
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteForum"/> class. 
    /// </summary>
    public DeleteForum()
        : base("ADMIN_DELETEFORUM", ForumPages.Admin_DeleteForum)
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Registers the needed Java Scripts
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    protected override void OnPreRender([NotNull] EventArgs e)
    {
        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
            "BlockUiFunctionJs",
            JavaScriptBlocks.BlockUiFunctionJs("DeleteForumMessage"));
            
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
    protected override void OnInit([NotNull] EventArgs e)
    {
        this.Delete.Click += this.SaveClick;
        this.Cancel.Click += this.Cancel_Click;

        this.Delete.ReturnConfirmText = this.GetText("ADMIN_FORUMS", "CONFIRM_DELETE");
        this.Delete.ReturnConfirmEvent = "blockUIMessage";

        base.OnInit(e);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (this.IsPostBack)
        {
            return;
        }

        if (!this.Get<HttpRequestBase>().QueryString.Exists("fa"))
        {
            this.Get<LinkBuilder>().Redirect(ForumPages.Admin_Forums);
        }

        var forumId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAsInt("fa");

        var forum = this.GetRepository<Forum>().GetById(forumId.Value);

        if (forum == null)
        {
            this.Get<LinkBuilder>().Redirect(ForumPages.Admin_Forums);
        }
        else
        {
            this.IconHeader.Text = $"{this.GetText("HEADER1")}: <strong>{forum.Name}</strong>";
        }
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    protected override void CreatePageLinks()
    {
        this.PageLinks.AddRoot();
        this.PageLinks.AddAdminIndex();

        this.PageLinks.AddLink(this.GetText("TEAM", "FORUMS"), this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Forums));
        this.PageLinks.AddLink(this.GetText("ADMIN_DELETEFORUM", "TITLE"), string.Empty);
    }

    /// <summary>
    /// The update status timer_ tick.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void UpdateStatusTimerTick([NotNull] object sender, [NotNull] EventArgs e)
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
    private void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
        this.Get<LinkBuilder>().Redirect(ForumPages.Admin_Forums);
    }

    /// <summary>
    /// Delete The Forum
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void SaveClick([NotNull] object sender, [NotNull] EventArgs e)
    {
        string errorMessage;

        var forumId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAsInt("fa");

        var newForumId = this.ForumListSelected.Value.ToType<int>();

        if (this.MoveTopics.Checked && newForumId != forumId.Value)
        {
            // schedule...
            ForumDeleteTask.Start(
                this.PageBoardContext.PageBoardID,
                forumId.Value,
                newForumId,
                out errorMessage);

            // enable timer...
            this.UpdateStatusTimer.Enabled = true;

            this.LocalizedLabel6.LocalizedTag = "DELETE_MOVE_TITLE";
        }
        else
        {
            // schedule...
            ForumDeleteTask.Start(this.PageBoardContext.PageBoardID, forumId.Value, out errorMessage);

            // enable timer...
            this.UpdateStatusTimer.Enabled = true;

            this.LocalizedLabel6.LocalizedTag = "DELETE_TITLE";
        }

        if (errorMessage.IsSet())
        {
            this.PageBoardContext.Notify(errorMessage, MessageTypes.danger);
        }
    }

    #endregion
}