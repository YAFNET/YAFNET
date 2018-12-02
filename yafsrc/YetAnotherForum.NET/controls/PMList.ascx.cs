/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

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
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.UI.WebControls;
    using System.Xml;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

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

            set
            {
                this.ViewState["View"] = value;
            }
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
            using (var dv = LegacyDb.pmessage_list(this.PageContext.PageUserID, null, null).DefaultView)
            {
                dv.RowFilter = "IsDeleted = False AND IsArchived = False";

                foreach (DataRowView item in dv)
                {
                    LegacyDb.pmessage_archive(item["UserPMessageID"]);
                    archivedCount++;
                }
            }

            this.BindData();
            this.ClearCache();
            this.PageContext.AddLoadMessage(this.GetText("MSG_ARCHIVED+").FormatWith(archivedCount));
        }

        /// <summary>
        /// Handles the Load event of the ArchiveAll control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void ArchiveAll_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            ((ThemeButton)sender).Attributes["onclick"] =
                "return confirm('{0}')".FormatWith(this.GetText("CONFIRM_ARCHIVEALL"));
        }

        /// <summary>
        /// Handles the Load event of the ExportAll control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void ExportAll_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            ((ThemeButton)sender).Attributes["onclick"] =
                "this.form.onsubmit = function() {return true;}";
        }

        /// <summary>
        /// The archive selected_ click.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void ArchiveSelected_Click([NotNull] object source, [NotNull] EventArgs e)
        {
            long archivedCount = 0;

            foreach (var item in
                this.MessagesView.Rows.Cast<GridViewRow>().Where(
                    item => ((CheckBox)item.FindControl("ItemCheck")).Checked))
            {
                LegacyDb.pmessage_archive(this.MessagesView.DataKeys[item.RowIndex].Value);
                archivedCount++;
            }

            this.BindData();
            this.ClearCache();
            this.PageContext.AddLoadMessage(
                archivedCount == 1
                    ? this.GetText("MSG_ARCHIVED")
                    : this.GetText("MSG_ARCHIVED+").FormatWith(archivedCount));
        }

        /// <summary>
        /// Sort By Date
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void DateLink_Click([NotNull] object sender, [NotNull] EventArgs e)
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

            object toUserId = null;
            object fromUserId = null;
            var isoutbox = false;

            if (this.View == PmView.Outbox)
            {
                fromUserId = this.PageContext.PageUserID;
                isoutbox = true;
            }
            else
            {
                toUserId = this.PageContext.PageUserID;
            }

            using (var dv = LegacyDb.pmessage_list(toUserId, fromUserId, null).DefaultView)
            {
                switch (this.View)
                {
                    case PmView.Inbox:
                        dv.RowFilter = "IsDeleted = False AND IsArchived = False";
                        break;
                    case PmView.Outbox:
                        dv.RowFilter = "IsInOutbox = True AND IsArchived = False";
                        break;
                    case PmView.Archive:
                        dv.RowFilter = "IsArchived = True";
                        break;
                }

                foreach (DataRowView item in dv)
                {
                    if (isoutbox)
                    {
                        LegacyDb.pmessage_delete(item["UserPMessageID"], true);
                    }
                    else
                    {
                        LegacyDb.pmessage_delete(item["UserPMessageID"]);
                    }

                    itemCount++;
                }
            }

            this.BindData();
            this.PageContext.AddLoadMessage(this.GetTextFormatted("msgdeleted2", itemCount));
            this.ClearCache();
        }

        /// <summary>
        /// The delete all_ load.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void DeleteAll_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            ((ThemeButton)sender).Attributes["onclick"] =
                "return confirm('{0}')".FormatWith(this.GetText("CONFIRM_DELETEALL"));
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

            foreach (var item in
                this.MessagesView.Rows.Cast<GridViewRow>().Where(
                    item => ((CheckBox)item.FindControl("ItemCheck")).Checked))
            {
                switch (this.View)
                {
                    case PmView.Outbox:
                        LegacyDb.pmessage_delete(this.MessagesView.DataKeys[item.RowIndex].Value, true);
                        break;
                    default:
                        LegacyDb.pmessage_delete(this.MessagesView.DataKeys[item.RowIndex].Value);
                        break;
                }

                itemCount++;
            }

            this.BindData();

            this.PageContext.AddLoadMessage(
                itemCount == 1 ? this.GetText("msgdeleted1") : this.GetTextFormatted("msgdeleted2", itemCount));
            this.ClearCache();
        }

        /// <summary>
        /// The delete selected_ load.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void DeleteSelected_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            ((ThemeButton)sender).Attributes["onclick"] =
                "return confirm('{0}')".FormatWith(this.GetText("CONFIRM_DELETE"));
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

            // Return if No Messages are Available to Export
            if (!messageList.Table.HasRows())
            {
                this.PageContext.AddLoadMessage(this.GetText("NO_MESSAGES"));
                return;
            }

            if (this.ExportType.SelectedItem.Value.Equals("xml"))
            {
                this.ExportXmlFile(messageList);
            }
            else if (this.ExportType.SelectedItem.Value.Equals("csv"))
            {
                this.ExportCsvFile(messageList);
            }
            else if (this.ExportType.SelectedItem.Value.Equals("txt"))
            {
                this.ExportTextFile(messageList);
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
            var exportPmIds =
                this.MessagesView.Rows.Cast<GridViewRow>()
                    .Where(item => ((CheckBox)item.FindControl("ItemCheck")).Checked)
                    .Select(item => (int)this.MessagesView.DataKeys[item.RowIndex].Value)
                    .ToList();

            var messageList = this.GetMessagesForExport(exportPmIds);
            
            // Return if No Message Selected
            if (!messageList.Table.HasRows())
            {
                this.PageContext.AddLoadMessage(this.GetText("MSG_NOSELECTED"));

                this.BindData();

                return;
            }

            if (this.ExportType.SelectedItem.Value.Equals("xml"))
            {
                this.ExportXmlFile(messageList);
            }
            else if (this.ExportType.SelectedItem.Value.Equals("csv"))
            {
                this.ExportCsvFile(messageList);
            }
            else if (this.ExportType.SelectedItem.Value.Equals("txt"))
            {
                this.ExportTextFile(messageList);
            }

            this.BindData();
        }

        /// <summary>
        /// Formats the body.
        /// </summary>
        /// <param name="dataRowView">The data row view.</param>
        /// <returns>
        /// The format body.
        /// </returns>
        protected string FormatBody([NotNull] object dataRowView)
        {
            var row = (DataRowView)dataRowView;
            return (string)row["Body"];
        }

        /// <summary>
        /// The from link_ click.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void FromLink_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.SetSort(this.View == PmView.Outbox ? "ToUser" : "FromUser", true);

            this.BindData();
        }

        /// <summary>
        /// Get The Icon Image indicating if Unread or Read Message
        /// </summary>
        /// <param name="dataRow">The data row.</param>
        /// <returns>
        /// Returns the Image Url
        /// </returns>
        protected string GetImage([NotNull] object dataRow)
        {
            var dataRowView = dataRow as DataRowView;
            var isRead = dataRowView["IsRead"].ToType<bool>();

            return dataRowView["IsReply"].ToType<bool>()
                       ? this.Get<ITheme>().GetItem("ICONS", isRead ? "PM_READ_REPLY" : "PM_NEW_REPLY")
                       : this.Get<ITheme>().GetItem("ICONS", isRead ? "PM_READ" : "PM_NEW");
        }

        /// <summary>
        /// Gets the localized <paramref name="text"/>.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="page">The resource page.</param>
        /// <returns>
        /// The get localized text.
        /// </returns>
        protected string GetLocalizedText([NotNull] string text, string page)
        {
            return this.HtmlEncode(!string.IsNullOrEmpty(page) ? this.GetText(page, text) : this.GetText(text));
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
            return YafBuildLink.GetLink(
                ForumPages.cp_message, "pm={0}&v={1}", messageId, PmViewConverter.ToQueryStringParam(this.View));
        }

        /// <summary>
        /// Gets the message user header.
        /// </summary>
        /// <returns>
        /// The get message user header.
        /// </returns>
        protected string GetMessageUserHeader()
        {
            return this.GetLocalizedText(this.View == PmView.Outbox ? "to" : "from", "CP_PM");
        }

        /// <summary>
        /// Gets the message text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="_total">The _total.</param>
        /// <param name="_inbox">The _inbox.</param>
        /// <param name="_outbox">The _outbox.</param>
        /// <param name="_archive">The _archive.</param>
        /// <param name="_limit">The _limit.</param>
        /// <returns>Returns the Message Text</returns>
        protected string GetPMessageText(
            [NotNull] string text,
            [NotNull] object _total,
            [NotNull] object _inbox,
            [NotNull] object _outbox,
            [NotNull] object _archive,
            [NotNull] object _limit)
        {
            object percentage = 0;
            if (_limit.ToType<int>() != 0)
            {
                percentage = decimal.Round((_total.ToType<decimal>() / _limit.ToType<decimal>()) * 100, 2);
            }

            if (YafContext.Current.IsAdmin)
            {
                _limit = "\u221E";
                percentage = 0;
            }

            return this.HtmlEncode(this.GetTextFormatted(text, _total, _inbox, _outbox, _archive, _limit, percentage));
        }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <returns>
        /// The get title.
        /// </returns>
        protected string GetTitle()
        {
            switch (this.View)
            {
                case PmView.Outbox:
                    return this.GetLocalizedText("SENTITEMS", null);
                case PmView.Inbox:
                    return this.GetLocalizedText("INBOX", null);
                default:
                    return this.GetLocalizedText("ARCHIVE", null);
            }
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
            using (var dv = LegacyDb.pmessage_list(this.PageContext.PageUserID, null, null).DefaultView)
            {
                switch (this.View)
                {
                    case PmView.Inbox:
                        dv.RowFilter = "IsRead = False AND IsDeleted = False AND IsArchived = False";
                        break;
                    case PmView.Outbox:
                        dv.RowFilter = "IsRead = False AND IsDeleted = False AND IsArchived = False";
                        break;
                    case PmView.Archive:
                        dv.RowFilter = "IsRead = False AND IsArchived = True";
                        break;
                }

                foreach (DataRowView item in dv)
                {
                    LegacyDb.pmessage_markread(item["UserPMessageID"]);

                    // Clearing cache with old permissions data...
                    this.ClearCache();
                }
            }

            this.BindData();
        }

        /// <summary>
        /// The messages view_ row created.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        /// </param>
        protected void MessagesView_RowCreated([NotNull] object sender, [NotNull] GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                case DataControlRowType.Header:
                    {
                        var gridView = (GridView)sender;
                        var gridViewRow = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);

                        var tableCell = new TableCell { Text = this.GetTitle(), CssClass = "header1", ColumnSpan = 5 };

                        // Add Header to top with column span of 5... no need for two tables.
                        gridViewRow.Cells.Add(tableCell);
                        gridView.Controls[0].Controls.AddAt(0, gridViewRow);

                        var sortFrom = (Image)e.Row.FindControl("SortFrom");
                        var sortSubject = (Image)e.Row.FindControl("SortSubject");
                        var sortDate = (Image)e.Row.FindControl("SortDate");

                        sortFrom.Visible = (this.View == PmView.Outbox)
                                               ? (string)this.ViewState["SortField"] == "ToUser"
                                               : (string)this.ViewState["SortField"] == "FromUser";
                        sortFrom.ImageUrl = this.Get<ITheme>().GetItem(
                            "SORT", (bool)this.ViewState["SortAsc"] ? "ASCENDING" : "DESCENDING");

                        sortSubject.Visible = (string)this.ViewState["SortField"] == "Subject";
                        sortSubject.ImageUrl = this.Get<ITheme>().GetItem(
                            "SORT", (bool)this.ViewState["SortAsc"] ? "ASCENDING" : "DESCENDING");

                        sortDate.Visible = (string)this.ViewState["SortField"] == "Created";
                        sortDate.ImageUrl = this.Get<ITheme>().GetItem(
                            "SORT", (bool)this.ViewState["SortAsc"] ? "ASCENDING" : "DESCENDING");
                    }

                    break;
                case DataControlRowType.Footer:
                    {
                        var rolCount = e.Row.Cells.Count;

                        for (var i = rolCount - 1; i >= 1; i--)
                        {
                            e.Row.Cells.RemoveAt(i);
                        }

                        e.Row.Cells[0].ColumnSpan = rolCount;
                    }

                    break;
            }
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

            if (!this.IsPostBack)
            {
                // setup pager...
                this.MessagesView.AllowPaging = true;
                this.MessagesView.PagerSettings.Visible = false;
                this.MessagesView.AllowSorting = true;

                this.PagerTop.PageSize = 10;
                this.MessagesView.PageSize = 10;
            }
            else
            {
                // make sure addLoadMessage is empty...
                this.PageContext.LoadMessage.Clear();
            }

            this.lblExportType.Text = this.GetText("EXPORTFORMAT");

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
        /// The stats_ renew.
        /// </summary>
        protected void Stats_Renew()
        {
            // Renew PM Statistics
            var dt = LegacyDb.user_pmcount(this.PageContext.PageUserID);
            if (dt.HasRows())
            {
                this.PMInfoLink.Text = this.GetPMessageText(
                    "PMLIMIT_ALL",
                    dt.Rows[0]["NumberTotal"],
                    dt.Rows[0]["NumberIn"],
                    dt.Rows[0]["NumberOut"],
                    dt.Rows[0]["NumberArchived"],
                    dt.Rows[0]["NumberAllowed"]);
            }
        }

        /// <summary>
        /// The subject link_ click.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void SubjectLink_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.SetSort("Subject", true);
            this.BindData();
        }

        /// <summary>
        /// Gets the messages for export.
        /// </summary>
        /// <param name="exportPmIds">The export pm ids.</param>
        /// <returns>
        /// Returns the filtered Messages
        /// </returns>
        private DataView GetMessagesForExport([CanBeNull] List<int> exportPmIds)
        {
            var messageList = (DataView)this.MessagesView.DataSource;

            for (var i = messageList.Table.Rows.Count - 1; i >= 0; i--)
            {
                var row = messageList.Table.Rows[i];

                if (exportPmIds != null  && !exportPmIds.Contains(row["PMessageID"].ToType<int>()))
                {
                    messageList.Table.Rows.RemoveAt(i);
                    continue;
                }

                if (row["IsDeleted"].ToType<bool>())
                {
                    messageList.Table.Rows.RemoveAt(i);
                }
                else
                {
                    switch (this.View)
                    {
                        case PmView.Inbox:
                            {
                                if (row["IsArchived"].ToType<bool>())
                                {
                                    messageList.Table.Rows.RemoveAt(i);
                                }
                            }

                            break;
                        case PmView.Outbox:
                            {
                                if (!row["IsInOutbox"].ToType<bool>())
                                {
                                    messageList.Table.Rows.RemoveAt(i);
                                }
                            }

                            break;
                        case PmView.Archive:
                            {
                                if (!row["IsArchived"].ToType<bool>())
                                {
                                    messageList.Table.Rows.RemoveAt(i);
                                }
                            }

                            break;
                    }
                }
            }

            // Remove Columns that are not needed
            messageList.Table.Columns.Remove("IsDeleted");
            messageList.Table.Columns.Remove("IsArchived");
            messageList.Table.Columns.Remove("IsInOutbox");
            messageList.Table.Columns.Remove("Flags");
            
            return messageList;
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            object toUserId = null;
            object fromUserId = null;

            switch (this.View)
            {
                case PmView.Outbox:
                    fromUserId = this.PageContext.PageUserID;
                    break;
                case PmView.Archive:
                    fromUserId = this.PageContext.PageUserID;
                    toUserId = this.PageContext.PageUserID;
                    break;
                default:
                    toUserId = this.PageContext.PageUserID;
                    break;
            }

            using (var dv = LegacyDb.pmessage_list(toUserId, fromUserId, null).DefaultView)
            {
                switch (this.View)
                {
                    case PmView.Inbox:
                        dv.RowFilter = "IsDeleted = False AND IsArchived = False";
                        break;
                    case PmView.Outbox:
                        dv.RowFilter = "IsInOutbox = True AND IsArchived = False";
                        break;
                    case PmView.Archive:
                        dv.RowFilter = "IsArchived = True";
                        break;
                }

                dv.Sort = "{0} {1}".FormatWith(
                    this.ViewState["SortField"], (bool)this.ViewState["SortAsc"] ? "asc" : "desc");
                this.PagerTop.Count = dv.Count;

                if (dv.Count > 0)
                {
                    this.lblExportType.Visible = true;
                    this.ExportType.Visible = true;
                }
                else
                {
                    this.lblExportType.Visible = false;
                    this.ExportType.Visible = false;
                }

                this.MessagesView.PageIndex = this.PagerTop.CurrentPageIndex;
                this.MessagesView.DataSource = dv;
                this.MessagesView.DataBind();
            }

            this.Stats_Renew();
        }

        /// <summary>
        /// The clear cache.
        /// </summary>
        private void ClearCache()
        {
            this.Get<IDataCache>().Remove(Constants.Cache.ActiveUserLazyData.FormatWith(this.PageContext.PageUserID));
        }

        /// <summary>
        /// Export the Private Messages in message List as CSV File
        /// </summary>
        /// <param name="messageList">
        /// DataView that Contains the Private Messages
        /// </param>
        private void ExportCsvFile([NotNull] DataView messageList)
        {
            this.Get<HttpResponseBase>().Clear();
            this.Get<HttpResponseBase>().ClearContent();
            this.Get<HttpResponseBase>().ClearHeaders();

            this.Get<HttpResponseBase>().ContentType = "application/vnd.csv";
            this.Response.AppendHeader(
                "content-disposition",
                "attachment; filename=" +
                HttpUtility.UrlEncode(
                    "Privatemessages-{0}-{1}.csv".FormatWith(
                        this.PageContext.PageUserName, DateTime.Now.ToString("yyyy'-'MM'-'dd'-'HHmm"))));

            var sw = new StreamWriter(this.Get<HttpResponseBase>().OutputStream);

            var columnsCount = messageList.Table.Columns.Count;

            for (var i = 0; i < columnsCount; i++)
            {
                sw.Write(messageList.Table.Columns[i]);

                if (i < columnsCount - 1)
                {
                    sw.Write(",");
                }
            }

            sw.Write(sw.NewLine);

            foreach (DataRow dr in messageList.Table.Rows)
            {
                for (var i = 0; i < columnsCount; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        sw.Write(dr[i].ToString());
                    }

                    if (i < columnsCount - 1)
                    {
                        sw.Write(",");
                    }
                }

                sw.Write(sw.NewLine);
            }

            sw.Close();

            this.Get<HttpResponseBase>().Flush();
            this.Get<HttpResponseBase>().End();
        }

        /// <summary>
        /// Export the Private Messages in message List as Text File
        /// </summary>
        /// <param name="messageList">
        /// DataView that Contains the Private Messages
        /// </param>
        private void ExportTextFile([NotNull] DataView messageList)
        {
            this.Get<HttpResponseBase>().Clear();
            this.Get<HttpResponseBase>().ClearContent();
            this.Get<HttpResponseBase>().ClearHeaders();

            this.Get<HttpResponseBase>().ContentType = "application/vnd.text";
            this.Get<HttpResponseBase>().AppendHeader(
                "content-disposition",
                "attachment; filename=" +
                HttpUtility.UrlEncode(
                    "Privatemessages-{0}-{1}.txt".FormatWith(
                        this.PageContext.PageUserName, DateTime.Now.ToString("yyyy'-'MM'-'dd'-'HHmm"))));

            var sw = new StreamWriter(this.Get<HttpResponseBase>().OutputStream);

            sw.Write("{0};{1}".FormatWith(this.Get<YafBoardSettings>().Name, YafForumInfo.ForumURL));
            sw.Write(sw.NewLine);
            sw.Write("Private Message Dump for User {0}; {1}".FormatWith(this.PageContext.PageUserName, DateTime.Now));
            sw.Write(sw.NewLine);

            for (var i = 0; i <= messageList.Table.DataSet.Tables[0].Rows.Count - 1; i++)
            {
                for (var j = 0; j <= messageList.Table.DataSet.Tables[0].Columns.Count - 1; j++)
                {
                    sw.Write(
                        "{0}: {1}",
                        messageList.Table.DataSet.Tables[0].Columns[j],
                        messageList.Table.DataSet.Tables[0].Rows[i][j]);
                    sw.Write(sw.NewLine);
                }
            }

            sw.Close();

            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// Export the Private Messages in message List as Xml File
        /// </summary>
        /// <param name="messageList">
        /// DataView that Contains the Private Messages
        /// </param>
        private void ExportXmlFile([NotNull] DataView messageList)
        {
            this.Get<HttpResponseBase>().Clear();
            this.Get<HttpResponseBase>().ClearContent();
            this.Get<HttpResponseBase>().ClearHeaders();

            this.Get<HttpResponseBase>().ContentType = "text/xml";
            this.Get<HttpResponseBase>().AppendHeader(
                "content-disposition",
                "attachment; filename=PrivateMessages-{0}-{1}.xml".FormatWith(
                    this.PageContext.PageUserName,
                    HttpUtility.UrlEncode(DateTime.Now.ToString("yyyy'-'MM'-'dd'-'HHmm"))));

            messageList.Table.TableName = "PrivateMessage";

            var settings = new XmlWriterSettings
                {
                   Encoding = Encoding.UTF8, OmitXmlDeclaration = false, Indent = true, NewLineOnAttributes = true 
                };

            var xw = XmlWriter.Create(this.Get<HttpResponseBase>().OutputStream, settings);
            xw.WriteStartDocument();

            messageList.Table.DataSet.DataSetName = "PrivateMessages";

            xw.WriteComment(" {0};{1} ".FormatWith(this.Get<YafBoardSettings>().Name, YafForumInfo.ForumURL));
            xw.WriteComment(
                " Private Message Dump for User {0}; {1} ".FormatWith(this.PageContext.PageUserName, DateTime.Now));

            var xd = new XmlDataDocument(messageList.Table.DataSet);

            foreach (XmlNode node in xd.ChildNodes)
            {
                // nItemCount = node.ChildNodes.Count;
                node.WriteTo(xw);
            }

            xw.WriteEndDocument();

            xw.Close();

            this.Get<HttpResponseBase>().Flush();
            this.Get<HttpResponseBase>().End();
        }

        /// <summary>
        /// The set sort.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <param name="asc">
        /// The asc.
        /// </param>
        private void SetSort([NotNull] string field, bool asc)
        {
            if (this.ViewState["SortField"] != null && (string)this.ViewState["SortField"] == field)
            {
                this.ViewState["SortAsc"] = !(bool)this.ViewState["SortAsc"];
            }
            else
            {
                this.ViewState["SortField"] = field;
                this.ViewState["SortAsc"] = asc;
            }
        }

        #endregion
    }
}