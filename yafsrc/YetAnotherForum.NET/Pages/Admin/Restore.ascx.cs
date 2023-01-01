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

namespace YAF.Pages.Admin;

using YAF.Types.Models;

/// <summary>
/// The Admin Restore Topics Page
/// </summary>
public partial class Restore : AdminPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Restore"/> class. 
    /// </summary>
    public Restore()
        : base("ADMIN_RESTORE", ForumPages.Admin_Restore)
    {
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (this.IsPostBack)
        {
            return;
        }

        this.PageSize.DataSource = StaticDataHelper.PageEntries();
        this.PageSize.DataTextField = "Name";
        this.PageSize.DataValueField = "Value";
        this.PageSize.DataBind();

        this.PageSizeMessages.DataSource = StaticDataHelper.PageEntries();
        this.PageSizeMessages.DataTextField = "Name";
        this.PageSizeMessages.DataValueField = "Value";
        this.PageSizeMessages.DataBind();

        try
        {
            this.PageSize.SelectedValue =
                this.PageSizeMessages.SelectedValue = this.PageBoardContext.PageUser.PageSize.ToString();
        }
        catch (Exception)
        {
            this.PageSize.SelectedValue = this.PageSizeMessages.SelectedValue = "5";
        }

        this.BindData();
    }

    /// <summary>
    /// The page size on selected index changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void PageSizeSelectedIndexChanged(object sender, EventArgs e)
    {
        this.BindData();
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot();
        this.PageBoardContext.PageLinks.AddAdminIndex();
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_RESTORE", "TITLE"), string.Empty);
    }

    /// <summary>
    /// The list_ item command.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void List_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int? forumId = null;
        int? topicId = null;

        if (e.CommandArgument != null)
        {
            var commandArgs = e.CommandArgument.ToString().Split(';');

            topicId = commandArgs[0].ToType<int>();
            forumId = commandArgs[1].ToType<int>();
        }

        switch (e.CommandName)
        {
            case "delete":
                {
                    this.GetRepository<Topic>().Delete(forumId.Value, topicId.Value, true);

                    this.PageBoardContext.Notify(this.GetText("MSG_DELETED"), MessageTypes.success);

                    this.BindData();
                }

                break;
            case "restore":
                {
                    var getFirstMessage = this.GetRepository<Message>()
                        .GetSingle(m => m.TopicID == topicId && m.Position == 0);

                    if (getFirstMessage != null)
                    {
                        this.GetRepository<Message>().Restore(
                            forumId.Value,
                            topicId.Value,
                            getFirstMessage);
                    }

                    var topic = this.GetRepository<Topic>().GetById(topicId.Value);

                    var flags = topic.TopicFlags;

                    flags.IsDeleted = false;

                    this.GetRepository<Topic>().UpdateOnly(
                        () => new Topic { Flags = flags.BitValue },
                        t => t.ID == topicId);

                    this.PageBoardContext.Notify(this.GetText("MSG_RESTORED"), MessageTypes.success);

                    this.BindData();
                }

                break;
            case "delete_all":
                {
                    var deletedTopics = this.GetRepository<Topic>()
                        .GetDeletedTopics(this.PageBoardContext.PageBoardID, this.Filter.Text);

                    deletedTopics.ForEach(
                        x => this.GetRepository<Topic>().Delete(x.Item2.ForumID, x.Item2.ID, true));

                    this.PageBoardContext.Notify(this.GetText("MSG_DELETED"), MessageTypes.success);

                    this.PagerTop.CurrentPageIndex--;

                    this.BindData();
                }

                break;
            case "delete_zero":
                {
                    var deletedTopics = this.GetRepository<Topic>().Get(t => (t.Flags & 8) == 8 && t.NumPosts.Equals(0));

                    deletedTopics.ForEach(
                        topic => this.GetRepository<Topic>().Delete(topic.ForumID, topic.ID, true));

                    this.PageBoardContext.Notify(this.GetText("MSG_DELETED"), MessageTypes.success);

                    this.BindData();
                }

                break;
        }
    }

    /// <summary>
    /// The messages_ item command.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Messages_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int? forumId = null;
        int? topicId = null;
        Message message = null;

        if (e.CommandArgument != null)
        {
            var commandArgs = e.CommandArgument.ToString().Split(';');

            int? messageId = commandArgs[0].ToType<int>();
            forumId = commandArgs[1].ToType<int>();
            topicId = commandArgs[2].ToType<int>();

            message = this.GetRepository<Message>().GetById(messageId.Value);
        }
        
        switch (e.CommandName)
        {
            case "delete":
                {
                    // delete message
                    this.GetRepository<Message>().Delete(
                        forumId.Value,
                        topicId.Value,
                        message,
                        true,
                        string.Empty,
                        true,
                        true);

                    this.PageBoardContext.Notify(this.GetText("MSG_DELETED"), MessageTypes.success);

                    this.BindData();
                }

                break;
            case "restore":
                {
                    this.GetRepository<Message>().Restore(
                        forumId.Value,
                        topicId.Value,
                        message);

                    this.PageBoardContext.Notify(this.GetText("MSG_RESTORED"), MessageTypes.success);

                    this.BindData();
                }

                break;
            case "delete_all":
                {
                    var messages = this.GetRepository<Message>()
                        .GetDeletedMessages(this.PageBoardContext.PageBoardID);

                    messages.ForEach(
                        x => this.GetRepository<Message>().Delete(
                            x.Item1.ID,
                            x.Item3.TopicID,
                            x.Item3,
                            true,
                            string.Empty,
                            true,
                            true));

                    this.PageBoardContext.Notify(this.GetText("MSG_DELETED"), MessageTypes.success);

                    this.PagerTop.CurrentPageIndex--;

                    this.BindData();
                }

                break;
        }
    }

    /// <summary>
    /// The pager top_ page change.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void PagerTop_PageChange([NotNull] object sender, [NotNull] EventArgs e)
    {
        // rebind
        this.BindData();
    }

    /// <summary>
    /// The refresh click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void RefreshClick(object sender, EventArgs e)
    {
        this.BindData();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
        this.PagerTop.PageSize = this.PageSize.SelectedValue.ToType<int>();
        this.PagerMessages.PageSize = this.PageSizeMessages.SelectedValue.ToType<int>();

        var deletedTopics = this.GetRepository<Topic>()
            .GetDeletedTopics(this.PageBoardContext.PageBoardID, this.Filter.Text);

        var count = deletedTopics.Count;

        var deletedTopicPaged = deletedTopics.GetPaged(this.PagerTop);

        this.DeletedTopics.DataSource = deletedTopicPaged;
        this.DeletedTopics.DataBind();

        this.PagerTop.Count = deletedTopicPaged.Any()
                                  ? count
                                  : 0;

        var deletedMessages = this.GetRepository<Message>()
            .GetDeletedMessages(this.PageBoardContext.PageBoardID);

        count = deletedMessages.Count;

        var deletedMessagesPaged = deletedMessages.GetPaged(this.PagerMessages);

        this.DeletedMessages.DataSource = deletedMessagesPaged;
        this.DeletedMessages.DataBind();

        this.PagerMessages.Count = deletedMessagesPaged.Any()
                                       ? count
                                       : 0;
    }
}