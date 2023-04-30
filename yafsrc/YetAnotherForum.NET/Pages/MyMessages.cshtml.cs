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

namespace YAF.Pages;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;

using Microsoft.AspNetCore.Mvc.Rendering;

using ServiceStack.Text;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Types.Attributes;
using YAF.Types.Extensions;
using YAF.Types.Models;
using YAF.Types.Objects;
using YAF.Types.Objects.Model;

/// <summary>
/// The Private Message Page
/// </summary>
public class MyMessagesModel : ForumPageRegistered
{
    /// <summary>
    /// Gets or sets the information.
    /// </summary>
    /// <value>The information.</value>
    [TempData]
    public string Info { get; set; }

    [BindProperty]
    public int ViewIndex { get; set; }

    [BindProperty]
    public string SortField { get; set; }

    [BindProperty]
    public bool SortAsc { get; set; }

    [BindProperty]
    public IList<PagedPm> Messages { get; set; }

    [BindProperty]
    public int MessagesCount { get; set; }

    [BindProperty]
    public string SortSubjectAscIcon { get; set; }

    [BindProperty]
    public string SortSubjectDescIcon { get; set; }

    [BindProperty]
    public string SortDateAscIcon { get; set; }

    [BindProperty]
    public string SortDateDescIcon { get; set; }

    [BindProperty]
    public string SortFromAscIcon { get; set; }

    [BindProperty]
    public string SortFromDescIcon { get; set; }

    [BindProperty]
    public string ExportType { get; set; }

    /// <summary>
    ///   Gets View.
    /// </summary>
    public PmView View { get; private set; }

    public SelectList PmViews { get; set; }

    /// <summary>
    ///   Initializes a new instance of the <see cref = "MyMessagesModel" /> class.
    /// </summary>
    public MyMessagesModel()
        : base("PM", ForumPages.MyMessages)
    {
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public IActionResult OnGet()

    {
        // check if this feature is disabled
        if (!this.PageBoardContext.BoardSettings.AllowPrivateMessages)
        {
            return this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Disabled);
        }

        if (this.SortField.IsNotSet())
        {
            this.SetSort("Created", false);
        }

        this.BindData();

        return this.Page();
    }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddLink(this.PageBoardContext.PageUser.DisplayOrUserName(), this.Get<LinkBuilder>().GetLink(ForumPages.MyAccount));
        this.PageBoardContext.PageLinks.AddLink(this.GetText("PM","TITLE"));
    }

    public void OnPost()
    {
        if (this.SortField.IsNotSet())
        {
            this.SetSort("Created", false);
        }

        this.BindData();
    }

    /// <summary>
    /// Sort By Date
    /// </summary>
    public void OnPostDateLinkAsc()
    {
        this.SetSort("Created", true);
        this.BindData();
    }

    /// <summary>
    /// Sort By Date descending
    /// </summary>
    public void OnPostDateLinkDesc()
    {
        this.SetSort("Created", false);
        this.BindData();
    }

    /// <summary>
    /// Delete All Messages.
    /// </summary>
    public IActionResult OnPostDeleteAll()
    {
        var messages = this.GetRepository<UserPMessage>().List(this.PageBoardContext.PageUserID, this.View);

        var deleteCount = 0;

        messages.ForEach(
            item =>
            {
                deleteCount += this.GetRepository<UserPMessage>().Delete(item, this.View == PmView.Outbox);
            });

        this.ClearCache();

        this.PageBoardContext.SessionNotify(
            this.GetTextFormatted("msgdeleted2", this.View == PmView.Outbox ? deleteCount : messages.Count),
            MessageTypes.success); 

        return this.Get<LinkBuilder>().Redirect(
            ForumPages.MyMessages,
            new {v = PmViewConverter.ToQueryStringParam(this.View)});
    }

    /// <summary>
    /// The delete selected_ click.
    /// </summary>
    public IActionResult OnPostDeleteSelected()
    {
        var selectedMessages = this.Messages.Where(x => x.Selected).ToList();

        long itemCount = selectedMessages.Count;

        selectedMessages.ForEach(
            item =>
            {
                this.GetRepository<UserPMessage>().Delete(
                    item.UserPMessageID, this.View == PmView.Outbox);
            });

        this.ClearCache();

        this.PageBoardContext.SessionNotify(
            itemCount == 1 ? this.GetText("msgdeleted1") : this.GetTextFormatted("msgdeleted2", itemCount),
            MessageTypes.success);

        return this.Get<LinkBuilder>().Redirect(
            ForumPages.MyMessages,
           new {v = PmViewConverter.ToQueryStringParam(this.View)});
    }

    /// <summary>
    /// Export All Messages
    /// </summary>
    public IActionResult OnPostExportAll()
    {
        var messageList = this.GetMessagesForExport(null);

        return this.ExportType == "xml" ? this.ExportXmlFile(messageList) : this.ExportCsvFile(messageList);
    }

    /// <summary>
    /// Export Selected Messages
    /// </summary>
    public IActionResult OnPostExportSelected()
    {
        var messageList = this.Messages.Where(x => x.Selected).ToList();

        if (messageList.Any())
        {
            return this.ExportType == "xml" ? this.ExportXmlFile(messageList) : this.ExportCsvFile(messageList);
        }

        return this.PageBoardContext.Notify(this.GetText("MSG_NOSELECTED"), MessageTypes.warning);
    }

    /// <summary>
    /// sort by name ascending
    /// </summary>
    public void OnPostFromLinkAsc()
    {
        this.SetSort(this.View == PmView.Outbox ? "ToUser" : "FromUser", true);

        this.BindData();
    }

    /// <summary>
    /// sort by name descending
    /// </summary>
    public void OnPostFromLinkDesc()
    {
        this.SetSort(this.View == PmView.Outbox ? "ToUser" : "FromUser", false);

        this.BindData();
    }

    /// <summary>
    /// The mark as read_ click.
    /// </summary>
    public void OnPostMarkAsRead()
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
    /// The subject link_ click.
    /// </summary>
    public void OnPostSubjectLinkAsc()
    {
        this.SetSort("Subject", true);
        this.BindData();
    }

    /// <summary>
    /// The subject link_ click.
    /// </summary>
    public void OnPostSubjectLinkDesc()
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
    private List<PagedPm> GetMessagesForExport([CanBeNull] ICollection<int> exportPmIds)
    {
        var list = this.Messages;

        list = !exportPmIds.NullOrEmpty()
                   ? list.Where(x => x.IsDeleted == false && exportPmIds.Contains(x.PMessageID)).ToList()
                   : list.Where(x => x.IsDeleted == false).ToList();

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
        this.PmViews = new SelectList(
            StaticDataHelper.PmViews(),
            nameof(SelectListItem.Value),
            nameof(SelectListItem.Text));
        this.PageSizeList = new SelectList(
            StaticDataHelper.PageEntries(),
            nameof(SelectListItem.Value),
            nameof(SelectListItem.Text));

        this.View = this.ViewIndex.ToEnum<PmView>();

        // Renew PM Statistics
        var count = this.GetRepository<PMessage>().UserMessageCount(this.PageBoardContext.PageUserID);

        if (count != null)
        {
            this.Info = this.GetPMessageText(
                "PMLIMIT_ALL",
                count.NumberTotal,
                count.InboxCount,
                count.OutBoxCount,
                count.Allowed);
        }

        var messages = this.GetRepository<PMessage>().List(
            this.PageBoardContext.PageUserID,
            this.View,
            this.SortField,
            this.SortAsc);

        this.MessagesCount = messages.Count;

        var pager = new Paging {CurrentPageIndex = this.PageBoardContext.PageIndex, PageSize = this.Size};

        var messagesPaged = messages.GetPaged(pager);

        this.Messages = messagesPaged;
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
    /// Export the Private Messages in message List as CSV File
    /// </summary>
    /// <param name="messageList">
    /// DataView that Contains the Private Messages
    /// </param>
    private FileContentResult ExportCsvFile([NotNull] IEnumerable<PagedPm> messageList)
    {
        var list = messageList.Select(
        message => new {
        message.FromUser,
        message.ToUser,
        message.Created,
        message.Subject,
        message.Body,
        MessageID = message.PMessageID
        });

        var stream = new MemoryStream();

        CsvSerializer.SerializeToStream(list, stream);

        var fileName = $"PrivateMessages-{this.PageBoardContext.PageUser.DisplayOrUserName()}-{HttpUtility.UrlEncode(DateTime.Now.ToString("yyyy'-'MM'-'dd'-'HHmm"))}.csv";

        return this.File(stream.ToArray(), "application/vnd.csv", fileName);
    }

    /// <summary>
    /// Export the Private Messages in message List as Xml File
    /// </summary>
    /// <param name="messageList">
    /// DataView that Contains the Private Messages
    /// </param>
    private FileContentResult ExportXmlFile([NotNull] IEnumerable<PagedPm> messageList)
    {
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

        var writer = new System.Xml.Serialization.XmlSerializer(element.GetType());
        var stream = new MemoryStream();
        writer.Serialize(stream, element);

        var fileName = $"PrivateMessages-{this.PageBoardContext.PageUser.DisplayOrUserName()}-{HttpUtility.UrlEncode(DateTime.Now.ToString("yyyy'-'MM'-'dd'-'HHmm"))}.xml";

        return this.File(stream.ToArray(), "application/xml", fileName);
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
    private void SetSort([NotNull] string field, bool ascending)
    {
        if (this.SortField.IsSet() && this.SortField == field)
        {
            this.SortAsc = !this.SortAsc;
        }
        else
        {
            this.SortField = field;
            this.SortAsc = ascending;
        }

        switch (field)
        {
            case "Subject":
            {
                if (ascending)
                {
                    this.SortSubjectAscIcon = "check-square";
                    this.SortSubjectDescIcon = "sort-alpha-down-alt";
                }
                else
                {
                    this.SortSubjectDescIcon = "check-square";
                    this.SortSubjectAscIcon = "sort-alpha-down";
                }

                this.SortDateAscIcon = "sort-alpha-down";
                this.SortDateDescIcon = "sort-alpha-down-alt";

                this.SortFromAscIcon = "sort-alpha-down";
                this.SortFromDescIcon = "sort-alpha-down-alt";
            }

                break;
            case "Created":
            {
                if (ascending)
                {
                    this.SortDateAscIcon = "check-square";
                    this.SortDateDescIcon = "sort-alpha-down-alt";
                }
                else
                {
                    this.SortDateDescIcon = "check-square";
                    this.SortDateAscIcon = "sort-alpha-down";
                }

                this.SortFromAscIcon = "sort-alpha-down";
                this.SortFromDescIcon = "sort-alpha-down-alt";

                this.SortSubjectAscIcon = "sort-alpha-down";
                this.SortSubjectDescIcon = "sort-alpha-down-alt";
            }

                break;
            case "ToUser":
            {
                if (ascending)
                {
                    this.SortFromAscIcon = "check-square";
                    this.SortFromDescIcon = "sort-alpha-down-alt";
                }
                else
                {
                    this.SortFromDescIcon = "check-square";
                    this.SortFromAscIcon = "sort-alpha-down";
                }

                this.SortDateAscIcon = "sort-alpha-down";
                this.SortDateDescIcon = "sort-alpha-down-alt";

                this.SortSubjectAscIcon = "sort-alpha-down";
                this.SortSubjectDescIcon = "sort-alpha-down-alt";
            }

                break;
            case "FromUser":
            {
                if (ascending)
                {
                    this.SortFromAscIcon = "check-square";
                    this.SortFromDescIcon = "sort-alpha-down-alt";
                }
                else
                {
                    this.SortFromDescIcon = "check-square";
                    this.SortFromAscIcon = "sort-alpha-down";
                }

                this.SortDateAscIcon = "sort-alpha-down";
                this.SortDateDescIcon = "sort-alpha-down-alt";

                this.SortSubjectAscIcon = "sort-alpha-down";
                this.SortSubjectDescIcon = "sort-alpha-down-alt";
            }

                break;
        }
    }

    /// <summary>
    /// Gets the message text.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="total">The total.</param>
    /// <param name="inbox">The inbox.</param>
    /// <param name="outbox">The outbox.</param>
    /// <param name="limit">The limit.</param>
    /// <returns>Returns the Message Text</returns>
    protected string GetPMessageText(
        [NotNull] string text,
        [NotNull] int total,
        [NotNull] int inbox,
        [NotNull] int outbox,
        [NotNull] int limit)
    {
        decimal percentage = 0;

        if (limit != 0)
        {
            percentage = decimal.Round(total / limit * 100, 2);
        }

        if (!this.PageBoardContext.IsAdmin)
        {
            return this.HtmlEncode(this.GetTextFormatted(text, total, inbox, outbox, limit, percentage));
        }

        percentage = 0;

        return this.HtmlEncode(this.GetTextFormatted(text, total, inbox, outbox, "\u221E", percentage));
    }
}