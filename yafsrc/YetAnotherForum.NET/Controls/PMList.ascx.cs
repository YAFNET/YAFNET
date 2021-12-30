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

namespace YAF.Controls
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.UI.WebControls;
    using System.Xml.Linq;

    using ServiceStack.Text;

    using YAF.Core.BaseControls;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Types.Objects.Model;

    #endregion

    /// <summary>
    /// The pm list.
    /// </summary>
    public partial class PMList : BaseUserControl
    {
        #region Properties

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

        #endregion

        #region Methods

        /// <summary>
        /// Archive All messages
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void ArchiveAll_Click([NotNull] object source, [NotNull] EventArgs e)
        {
            long archivedCount = 0;
            var messages = this.GetRepository<UserPMessage>().Get(
                p => p.UserID == this.PageContext.PageUserID && (p.Flags & 8) != 8 && (p.Flags & 4) != 4);

            messages.ForEach(
                item =>
                {
                    this.GetRepository<UserPMessage>().Archive(item.ID, new PMessageFlags(item.Flags));
                    archivedCount++;
                });

            this.ClearCache();

            this.PageContext.LoadMessage.AddSession(
                this.GetTextFormatted("MSG_ARCHIVED+", archivedCount),
                MessageTypes.success);

            this.Get<LinkBuilder>().Redirect(ForumPages.MyMessages, "#View{0}", this.View.ToInt());
        }

        /// <summary>
        /// The archive selected_ click.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void ArchiveSelected_Click([NotNull] object source, [NotNull] EventArgs e)
        {
            long archivedCount = 0;

            this.Messages.Items.Cast<RepeaterItem>().Where(item => item.FindControlAs<CheckBox>("ItemCheck").Checked)
                .ForEach(
                    item =>
                    {
                        var messageId = item.FindControlAs<HiddenField>("MessageID").Value.ToType<int>();

                        var message = this.GetRepository<UserPMessage>().GetById(messageId);

                        this.GetRepository<UserPMessage>().Archive(message.ID, new PMessageFlags(message.Flags));

                        archivedCount++;
                    });

            this.ClearCache();

            this.PageContext.LoadMessage.AddSession(
                archivedCount == 1
                    ? this.GetText("MSG_ARCHIVED")
                    : this.GetTextFormatted("MSG_ARCHIVED+", archivedCount),
                MessageTypes.success);

            this.Get<LinkBuilder>().Redirect(ForumPages.MyMessages, "#View{0}", this.View.ToInt());
        }

        /// <summary>
        /// Sort By Date
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void DateLinkAsc_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.SetSort("Created", true);
            this.BindData();
        }

        /// <summary>
        /// Sort By Date descending
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void DateLinkDesc_Click([NotNull] object sender, [NotNull] EventArgs e)
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
        protected void DeleteAll_Click([NotNull] object source, [NotNull] EventArgs e)
        {
            long itemCount = 0;

            switch (this.View)
            {
                case PmView.Inbox:
                    {
                        var messages = this.GetRepository<UserPMessage>().Get(
                            p => p.UserID == this.PageContext.PageUserID && (p.Flags & 8) != 8 && (p.Flags & 4) != 4);

                        messages.ForEach(
                            item =>
                            {
                                this.GetRepository<UserPMessage>().Delete(item.ID, false);

                                itemCount++;
                            });
                        break;
                    }
                case PmView.Outbox:
                    {
                        var messages = this.GetRepository<PMessage>().Get(
                            p => p.FromUserID == this.PageContext.PageUserID && p.PMessageFlags.IsInOutbox && p.PMessageFlags.IsArchived == false);

                        messages.ForEach(
                            item =>
                            {
                                this.GetRepository<UserPMessage>().Delete(item.ID, true);

                                itemCount++;
                            });
                        break;
                    }
                case PmView.Archive:
                    {
                        var messages = this.GetRepository<UserPMessage>().Get(
                            p => p.UserID == this.PageContext.PageUserID && (p.Flags & 4) == 4);

                        messages.ForEach(
                            item =>
                            {
                                this.GetRepository<UserPMessage>().Delete(item.ID, false);

                                itemCount++;
                            });
                        break;
                    }
            }

            this.ClearCache();

            this.PageContext.LoadMessage.AddSession(
                this.GetTextFormatted("msgdeleted2", itemCount),
                MessageTypes.success);


            this.Get<LinkBuilder>().Redirect(ForumPages.MyMessages, "#View{0}", this.View.ToInt());
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
        protected void DeleteSelected_Click([NotNull] object source, [NotNull] EventArgs e)
        {
            long itemCount = 0;

            var selectedMessages = this.Messages.Items.Cast<RepeaterItem>()
                .Where(item => item.FindControlAs<CheckBox>("ItemCheck").Checked);

            selectedMessages.ForEach(
                item =>
                {
                    this.GetRepository<UserPMessage>().Delete(
                        item.FindControlAs<HiddenField>("MessageID").Value.ToType<int>(),
                        this.View == PmView.Outbox);

                    itemCount++;
                });

            this.ClearCache();

            this.PageContext.LoadMessage.AddSession(
                itemCount == 1 ? this.GetText("msgdeleted1") : this.GetTextFormatted("msgdeleted2", itemCount),
                MessageTypes.success);


            this.Get<LinkBuilder>().Redirect(ForumPages.MyMessages, "#View{0}", this.View.ToInt());
        }

        /// <summary>
        /// Export All Messages
        /// </summary>
        /// <param name="source">
        /// The source of the event.
        /// </param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void ExportAll_Click([NotNull] object source, [NotNull] EventArgs e)
        {
            var messageList = this.GetMessagesForExport(null);

            //Return if No Messages are Available to Export
            if (!messageList.Any())
            {
                this.PageContext.AddLoadMessage(this.GetText("NO_MESSAGES"), MessageTypes.warning);
                return;
            }

            switch (this.ExportType.SelectedItem.Value)
            {
                case "xml":
                    this.ExportXmlFile(messageList);
                    break;
                case "csv":
                    this.ExportCsvFile(messageList);
                    break;
            }
        }

        /// <summary>
        /// Export Selected Messages
        /// </summary>
        /// <param name="source">
        /// The source of the event.
        /// </param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void ExportSelected_Click([NotNull] object source, [NotNull] EventArgs e)
        {
            var exportPmIds = this.Messages.Items.Cast<RepeaterItem>()
                .Where(item => item.FindControlAs<CheckBox>("ItemCheck").Checked)
                .Select(item => item.FindControlAs<HiddenField>("MessageID").Value.ToType<int>()).ToList();

            //Return if No Message Selected
            if (!exportPmIds.Any())
            {
                this.PageContext.AddLoadMessage(this.GetText("MSG_NOSELECTED"), MessageTypes.warning);

                this.BindData();

                return;
            }

            var messageList = this.GetMessagesForExport(exportPmIds);


            switch (this.ExportType.SelectedItem.Value)
            {
                case "xml":
                    this.ExportXmlFile(messageList);
                    break;
                case "csv":
                    this.ExportCsvFile(messageList);
                    break;
            }

            this.BindData();
        }

        /// <summary>
        /// sort by name ascending
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void FromLinkAsc_Click([NotNull] object sender, [NotNull] EventArgs e)
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
        protected void FromLinkDesc_Click([NotNull] object sender, [NotNull] EventArgs e)
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
        protected string GetMessageLink([NotNull] object messageId)
        {
            return this.Get<LinkBuilder>().GetLink(
                ForumPages.PrivateMessage,
                "pm={0}&v={1}",
                messageId,
                PmViewConverter.ToQueryStringParam(this.View));
        }

        /// <summary>
        /// The mark as read_ click.
        /// </summary>
        /// <param name="source">
        /// The source of the event.
        /// </param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void MarkAsRead_Click([NotNull] object source, [NotNull] EventArgs e)
        {
            var messages = this.GetRepository<UserPMessage>().List(this.PageContext.PageUserID, this.View);

            messages.ForEach(
                    item =>
                        {
                            this.GetRepository<UserPMessage>().MarkAsRead(item.ID, new PMessageFlags(item.Flags));

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
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.ViewState["SortField"] == null)
            {
                this.SetSort("Created", false);
            }

            if (this.IsPostBack)
            {
                // make sure addLoadMessage is empty...
                this.PageContext.LoadMessage.Clear();
            }

            this.lblExportType.Text = this.GetText("EXPORTFORMAT");

            this.PageSize.DataSource = StaticDataHelper.PageEntries();
            this.PageSize.DataTextField = "Name";
            this.PageSize.DataValueField = "Value";
            this.PageSize.DataBind();

            try
            {
                this.PageSize.SelectedValue = this.PageContext.User.PageSize.ToString();
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
                PmView.Archive => this.GetText("PM", "ARCHIV"),
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
        protected void PagerTop_PageChange([NotNull] object sender, [NotNull] EventArgs e)
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
        protected void SubjectLinkAsc_Click([NotNull] object sender, [NotNull] EventArgs e)
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
        protected void SubjectLinkDesc_Click([NotNull] object sender, [NotNull] EventArgs e)
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
            var list = (List<PagedPm>)this.Messages.DataSource;

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
                                if (!item.IsArchived)
                                {
                                    messageList.Add(item);
                                }
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
                        case PmView.Archive:
                            {
                                if (item.IsArchived)
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

            var messages = this.GetRepository<PMessage>().List(
                this.PageContext.PageUserID,
                this.View,
                this.ViewState["SortField"].ToString(),
                this.ViewState["SortAsc"].ToType<bool>());

            var messagesPaged = messages.GetPaged(this.PagerTop);

            this.PagerTop.Count = messages.Count;

            if (messages.Count > 0)
            {
                this.lblExportType.Visible = true;
                this.ExportType.Visible = true;

                this.NoMessage.Visible = false;

                this.Sort.Visible = true;
                this.upPanExport.Visible = true;
            }
            else
            {
                this.lblExportType.Visible = false;
                this.ExportType.Visible = false;

                this.NoMessage.Visible = true;

                this.Sort.Visible = false;
                this.upPanExport.Visible = false;
            }

            this.PagerTop.PageSize = this.PageSize.SelectedValue.ToType<int>();

            this.Messages.DataSource = messagesPaged;
            this.Messages.DataBind();
        }

        /// <summary>
        /// The clear cache.
        /// </summary>
        private void ClearCache()
        {
            this.Get<IDataCache>()
                .Remove(string.Format(Constants.Cache.ActiveUserLazyData, this.PageContext.PageUserID));
        }

        /// <summary>
        /// Export the Private Messages in message List as CSV File
        /// </summary>
        /// <param name="messageList">
        /// DataView that Contains the Private Messages
        /// </param>
        private void ExportCsvFile([NotNull] IEnumerable<PagedPm> messageList)
        {
            this.Get<HttpResponseBase>().Clear();
            this.Get<HttpResponseBase>().ClearContent();
            this.Get<HttpResponseBase>().ClearHeaders();

            this.Get<HttpResponseBase>().ContentType = "application/vnd.csv";
            this.Get<HttpResponseBase>().AppendHeader(
                "content-disposition",
                $"attachment; filename={HttpUtility.UrlEncode($"Privatemessages-{this.PageContext.User.DisplayOrUserName()}-{DateTime.Now:yyyy'-'MM'-'dd'-'HHmm}.csv")}");

            var sw = new StreamWriter(this.Get<HttpResponseBase>().OutputStream);

            var list = messageList.Select(
                message => new {
                    message.FromUser,
                    message.ToUser,
                    message.Created,
                    message.Subject,
                    message.Body,
                    MessageID = message.PMessageID
                });

            sw.Write(list.ToCsv());
            sw.Close();

            this.Get<HttpResponseBase>().Flush();
            this.Get<HttpResponseBase>().End();
        }

        /// <summary>
        /// Export the Private Messages in message List as Xml File
        /// </summary>
        /// <param name="messageList">
        /// DataView that Contains the Private Messages
        /// </param>
        private void ExportXmlFile([NotNull] IEnumerable<PagedPm> messageList)
        {
            this.Get<HttpResponseBase>().Clear();
            this.Get<HttpResponseBase>().ClearContent();
            this.Get<HttpResponseBase>().ClearHeaders();

            this.Get<HttpResponseBase>().ContentType = "text/xml";
            this.Get<HttpResponseBase>().AppendHeader(
                "content-disposition",
                $"attachment; filename=PrivateMessages-{this.PageContext.User.DisplayOrUserName()}-{HttpUtility.UrlEncode(DateTime.Now.ToString("yyyy'-'MM'-'dd'-'HHmm"))}.xml");

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
        private void SetSort([NotNull] string field, bool ascending)
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

        #endregion
    }
}