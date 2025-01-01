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

namespace YAF.Controls;

using System.ComponentModel;
using System.Xml.Linq;

using YAF.Types.Models;

/// <summary>
/// The pm list.
/// </summary>
public partial class PMList : BaseUserControl
{
    /// <summary>
    ///   Gets or sets the current view for the user's private messages.
    /// </summary>
    [Category("Behavior")]
    [Description("Gets or sets the current view for the user's private messages.")]
    public PmView View
    {
        get
        {
            if (this.ViewState["View"] != null)
            {
                return (PmView)this.ViewState["View"];
            }

            return PmView.Inbox;
        }

        set => this.ViewState["View"] = value;
    }

    /// <summary>
    /// Sort By Date
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    protected void DateLinkAsc_Click(object sender, EventArgs e)
    {
        this.SetSort("Created", true);
        this.BindData();
    }

    /// <summary>
    /// Sort By Date descending
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    protected void DateLinkDesc_Click(object sender, EventArgs e)
    {
        this.SetSort("Created", false);
        this.BindData();
    }

    /// <summary>
    /// Delete All Messages.
    /// </summary>
    /// <param name="source">
    /// The source of the event.
    /// </param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    protected void DeleteAll_Click(object source, EventArgs e)
    {
        var messages = this.GetRepository<UserPMessage>().List(this.PageBoardContext.PageUserID, this.View);

        messages.ForEach(
            item =>
            {
                this.GetRepository<UserPMessage>().Delete(item, this.View == PmView.Outbox);
            });

        this.ClearCache();

        this.PageBoardContext.LoadMessage.AddSession(
            this.GetTextFormatted("msgdeleted2", this.PagerTop.Count),
            MessageTypes.success);

        this.Get<LinkBuilder>().Redirect(
            ForumPages.MyMessages,
            new {v = PmViewConverter.ToQueryStringParam(this.View)});
    }

    /// <summary>
    /// The delete selected_ click.
    /// </summary>
    /// <param name="source">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    /// </param>
    protected void DeleteSelected_Click(object source, EventArgs e)
    {
        var selectedMessages = this.Messages.Items.Cast<RepeaterItem>()
            .Where(item => item.FindControlAs<CheckBox>("ItemCheck").Checked);

        long itemCount = selectedMessages.Count();

        selectedMessages.ForEach(
            item =>
                {
                    this.GetRepository<UserPMessage>().Delete(
                        item.FindControlAs<HiddenField>("MessageID").Value.ToType<int>(), this.View == PmView.Outbox);
                });

        this.ClearCache();

        this.PageBoardContext.LoadMessage.AddSession(
            itemCount == 1 ? this.GetText("msgdeleted1") : this.GetTextFormatted("msgdeleted2", itemCount),
            MessageTypes.success);

        this.Get<LinkBuilder>().Redirect(
            ForumPages.MyMessages,
            new {v = PmViewConverter.ToQueryStringParam(this.View)});
    }

    /// <summary>
    /// Export All Messages
    /// </summary>
    /// <param name="source">
    /// The source of the event.
    /// </param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    protected void ExportAll_Click(object source, EventArgs e)
    {
        var messageList = this.GetMessagesForExport(null);

        //Return if No Messages are Available to Export
        if (!messageList.Any())
        {
            this.PageBoardContext.Notify(this.GetText("NO_MESSAGES"), MessageTypes.warning);
            return;
        }

        this.ExportXmlFile(messageList);
    }

    /// <summary>
    /// Export Selected Messages
    /// </summary>
    /// <param name="source">
    /// The source of the event.
    /// </param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    protected void ExportSelected_Click(object source, EventArgs e)
    {
        var exportPmIds = this.Messages.Items.Cast<RepeaterItem>()
            .Where(item => item.FindControlAs<CheckBox>("ItemCheck").Checked)
            .Select(item => item.FindControlAs<HiddenField>("MessageID").Value.ToType<int>()).ToList();

        //Return if No Message Selected
        if (!exportPmIds.Any())
        {
            this.PageBoardContext.Notify(this.GetText("MSG_NOSELECTED"), MessageTypes.warning);

            this.BindData();

            return;
        }

        var messageList = this.GetMessagesForExport(exportPmIds);

        this.ExportXmlFile(messageList);

        this.BindData();
    }

    /// <summary>
    /// sort by name ascending
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    protected void FromLinkAsc_Click(object sender, EventArgs e)
    {
        this.SetSort(this.View == PmView.Outbox ? "ToUser" : "FromUser", true);

        this.BindData();
    }

    /// <summary>
    /// sort by name descending
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void FromLinkDesc_Click(object sender, EventArgs e)
    {
        this.SetSort(this.View == PmView.Outbox ? "ToUser" : "FromUser", false);

        this.BindData();
    }

    /// <summary>
    /// Gets the message link.
    /// </summary>
    /// <param name="messageId">The message id.</param>
    /// <returns>
    /// The get message link.
    /// </returns>
    protected string GetMessageLink(object messageId)
    {
        return this.Get<LinkBuilder>().GetLink(
            ForumPages.PrivateMessage,
            new { pm = messageId, v = PmViewConverter.ToQueryStringParam(this.View) });
    }

    /// <summary>
    /// The mark as read_ click.
    /// </summary>
    /// <param name="source">
    /// The source of the event.
    /// </param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    protected void MarkAsRead_Click(object source, EventArgs e)
    {
        var messages = this.GetRepository<UserPMessage>().List(this.PageBoardContext.PageUserID, this.View);

        messages.ForEach(
            item =>
                {
                    this.GetRepository<UserPMessage>().MarkAsRead(item);

                    // Clearing cache with old permissions data...
                    this.ClearCache();
                });
        this.BindData();
    }

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.ViewState["SortField"] == null)
        {
            this.SetSort("Created", false);
        }

        if (this.IsPostBack)
        {
            // make sure addLoadMessage is empty...
            this.PageBoardContext.LoadMessage.Clear();
        }

        this.PageSize.DataSource = StaticDataHelper.PageEntries();
        this.PageSize.DataTextField = "Name";
        this.PageSize.DataValueField = "Value";
        this.PageSize.DataBind();

        try
        {
            this.PageSize.SelectedValue = this.PageBoardContext.PageUser.PageSize.ToString();
        }
        catch (Exception)
        {
            this.PageSize.SelectedValue = "5";
        }

        this.BindData();
    }

    /// <summary>
    /// Renders the Icon Header Text
    /// </summary>
    protected string GetHeaderText()
    {
        return this.View switch
            {
                PmView.Inbox => this.GetText("PM", "INBOX"),
                PmView.Outbox => this.GetText("PM", "SENTITEMS"),
                _ => this.GetText("PM", "INBOX")
            };
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
    /// The pager top_ page change.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    protected void PagerTop_PageChange(object sender, EventArgs e)
    {
        // rebind
        this.BindData();
    }

    /// <summary>
    /// The subject link_ click.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    protected void SubjectLinkAsc_Click(object sender, EventArgs e)
    {
        this.SetSort("Subject", true);
        this.BindData();
    }

    /// <summary>
    /// The subject link_ click.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    protected void SubjectLinkDesc_Click(object sender, EventArgs e)
    {
        this.SetSort("Subject", false);
        this.BindData();
    }

    /// <summary>
    /// Gets the messages for export.
    /// </summary>
    /// <param name="exportPmIds">The export pm ids.</param>
    /// <returns>
    /// Returns the filtered Messages
    /// </returns>
    private List<PagedPm> GetMessagesForExport(ICollection<int> exportPmIds)
    {
        var list = (List<PagedPm>)this.Messages.DataSource;

        list = !exportPmIds.NullOrEmpty()
                   ? list.Where(x => !x.IsDeleted && exportPmIds.Contains(x.PMessageID)).ToList()
                   : list.Where(x => !x.IsDeleted).ToList();

        var messageList = new List<PagedPm>();

        list.ForEach(
            item =>
                {
                    switch (this.View)
                    {
                        case PmView.Inbox:
                            {
                                messageList.Add(item);
                            }

                            break;
                        case PmView.Outbox:
                            {
                                if (item.IsInOutbox)
                                {
                                    messageList.Add(item);
                                }
                            }

                            break;
                    }
                });

        return messageList;
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
        this.IconHeader.Text = this.GetHeaderText();

        this.SortFromAsc.Text = this.GetText(this.View == PmView.Outbox ? "TO_ASC" : "FROM_ASC");
        this.SortFromDesc.Text = this.GetText(this.View == PmView.Outbox ? "TO_DESC" : "FROM_DESC");

        this.PagerTop.PageSize = this.PageSize.SelectedValue.ToType<int>();

        var messages = this.GetRepository<PMessage>().List(
            this.PageBoardContext.PageUserID,
            this.View,
            this.ViewState["SortField"].ToString(),
            this.ViewState["SortAsc"].ToType<bool>());

        var messagesPaged = messages.GetPaged(this.PagerTop);

        this.PagerTop.Count = messages.Count;

        if (messages.Count > 0)
        {
            this.NoMessage.Visible = false;

            this.Sort.Visible = true;
            this.Tools.Visible = true;
        }
        else
        {
            this.NoMessage.Visible = true;

            this.Sort.Visible = false;
            this.Tools.Visible = false;
        }

        this.Messages.DataSource = messagesPaged;
        this.Messages.DataBind();
    }

    /// <summary>
    /// The clear cache.
    /// </summary>
    private void ClearCache()
    {
        this.Get<IDataCache>()
            .Remove(string.Format(Constants.Cache.ActiveUserLazyData, this.PageBoardContext.PageUserID));
    }

    /// <summary>
    /// Export the Private Messages in message List as Xml File
    /// </summary>
    /// <param name="messageList">
    /// DataView that Contains the Private Messages
    /// </param>
    private void ExportXmlFile(IEnumerable<PagedPm> messageList)
    {
        this.Get<HttpResponseBase>().Clear();
        this.Get<HttpResponseBase>().ClearContent();
        this.Get<HttpResponseBase>().ClearHeaders();

        this.Get<HttpResponseBase>().ContentType = "text/xml";
        this.Get<HttpResponseBase>().AppendHeader(
            "content-disposition",
            $"attachment; filename=PrivateMessages-{this.PageBoardContext.PageUser.DisplayOrUserName()}-{HttpUtility.UrlEncode(DateTime.Now.ToString("yyyy'-'MM'-'dd'-'HHmm"))}.xml");

        var element = new XElement(
            "PrivateMessages",
            from message in messageList
            select new XElement(
                "Message",
                new XElement("FromUser", message.FromUser),
                new XElement("ToUser", message.ToUser),
                new XElement("Created", message.Created),
                new XElement("Subject", message.Subject),
                new XElement("Body", message.Body),
                new XElement("MessageID", message.PMessageID)));

        element.Save(this.Get<HttpResponseBase>().OutputStream);

        this.Get<HttpResponseBase>().Flush();
        this.Get<HttpResponseBase>().End();
    }

    /// <summary>
    /// The set sort.
    /// </summary>
    /// <param name="field">
    /// The field.
    /// </param>
    /// <param name="ascending">
    /// The ascending.
    /// </param>
    private void SetSort(string field, bool ascending)
    {
        if (this.ViewState["SortField"] != null && this.ViewState["SortField"].ToString() == field)
        {
            this.ViewState["SortAsc"] = !this.ViewState["SortAsc"].ToType<bool>();
        }
        else
        {
            this.ViewState["SortField"] = field;
            this.ViewState["SortAsc"] = ascending;
        }

        switch (field)
        {
            case "Subject":
                {
                    if (ascending)
                    {
                        this.SortSubjectAsc.Icon = "check-square";
                        this.SortSubjectDesc.Icon = "sort-alpha-down-alt";
                    }
                    else
                    {
                        this.SortSubjectDesc.Icon = "check-square";
                        this.SortSubjectAsc.Icon = "sort-alpha-down";
                    }

                    this.SortDatedAsc.Icon = "sort-alpha-down";
                    this.SortDateDesc.Icon = "sort-alpha-down-alt";

                    this.SortFromAsc.Icon = "sort-alpha-down";
                    this.SortFromDesc.Icon = "sort-alpha-down-alt";
                }

                break;
            case "Created":
                {
                    if (ascending)
                    {
                        this.SortDatedAsc.Icon = "check-square";
                        this.SortDateDesc.Icon = "sort-alpha-down-alt";
                    }
                    else
                    {
                        this.SortDateDesc.Icon = "check-square";
                        this.SortDatedAsc.Icon = "sort-alpha-down";
                    }

                    this.SortFromAsc.Icon = "sort-alpha-down";
                    this.SortFromDesc.Icon = "sort-alpha-down-alt";

                    this.SortSubjectAsc.Icon = "sort-alpha-down";
                    this.SortSubjectDesc.Icon = "sort-alpha-down-alt";
                }

                break;
            case "ToUser":
                {
                    if (ascending)
                    {
                        this.SortFromAsc.Icon = "check-square";
                        this.SortFromDesc.Icon = "sort-alpha-down-alt";
                    }
                    else
                    {
                        this.SortFromDesc.Icon = "check-square";
                        this.SortFromAsc.Icon = "sort-alpha-down";
                    }

                    this.SortDatedAsc.Icon = "sort-alpha-down";
                    this.SortDateDesc.Icon = "sort-alpha-down-alt";

                    this.SortSubjectAsc.Icon = "sort-alpha-down";
                    this.SortSubjectDesc.Icon = "sort-alpha-down-alt";
                }

                break;
            case "FromUser":
                {
                    if (ascending)
                    {
                        this.SortFromAsc.Icon = "check-square";
                        this.SortFromDesc.Icon = "sort-alpha-down-alt";
                    }
                    else
                    {
                        this.SortFromDesc.Icon = "check-square";
                        this.SortFromAsc.Icon = "sort-alpha-down";
                    }

                    this.SortDatedAsc.Icon = "sort-alpha-down";
                    this.SortDateDesc.Icon = "sort-alpha-down-alt";

                    this.SortFromAsc.Icon = "sort-alpha-down";
                    this.SortFromDesc.Icon = "sort-alpha-down-alt";

                    this.SortSubjectAsc.Icon = "sort-alpha-down";
                    this.SortSubjectDesc.Icon = "sort-alpha-down-alt";
                }

                break;
        }
    }
}