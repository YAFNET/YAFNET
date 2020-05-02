/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.UI.WebControls;
    using System.Xml;

    using YAF.Configuration;
    using YAF.Core.BaseControls;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
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
            using (var dv = this.GetRepository<PMessage>().ListAsDataTable(this.PageContext.PageUserID, null, null)
                .DefaultView)
            {
                dv.RowFilter = "IsDeleted = False AND IsArchived = False";

                dv.Cast<DataRowView>().ForEach(
                    item =>
                        {
                            this.GetRepository<PMessage>().ArchiveMessage(item["UserPMessageID"].ToType<int>());
                            archivedCount++;
                        });
            }

            this.BindData();
            this.ClearCache();
            this.PageContext.AddLoadMessage(this.GetTextFormatted("MSG_ARCHIVED+", archivedCount), MessageTypes.success);
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
                            this.GetRepository<PMessage>()
                                .ArchiveMessage(item.FindControlAs<HiddenField>("MessageID").Value);
                            archivedCount++;
                        });

            this.BindData();
            this.ClearCache();

            this.PageContext.AddLoadMessage(
                archivedCount == 1
                    ? this.GetText("MSG_ARCHIVED")
                    : this.GetTextFormatted("MSG_ARCHIVED+", archivedCount),
                MessageTypes.success);
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

            object toUserId = null;
            object fromUserId = null; 
            var isOutbox = false;

            if (this.View == PmView.Outbox)
            {
                fromUserId = this.PageContext.PageUserID;
                isOutbox = true;
            }
            else
            {
                toUserId = this.PageContext.PageUserID;
            }

            using (var dv = this.GetRepository<PMessage>().ListAsDataTable(toUserId, fromUserId, null).DefaultView)
            {
                dv.RowFilter = this.View switch
                    {
                        PmView.Inbox => "IsDeleted = False AND IsArchived = False",
                        PmView.Outbox => "IsInOutbox = True AND IsArchived = False",
                        PmView.Archive => "IsArchived = True",
                        _ => dv.RowFilter
                    };

                dv.Cast<DataRowView>().ForEach(
                    item =>
                        {
                            this.GetRepository<PMessage>().DeleteMessage(item["UserPMessageID"].ToType<int>(), isOutbox);

                            itemCount++;
                        });
            }

            this.BindData();
            this.PageContext.AddLoadMessage(this.GetTextFormatted("msgdeleted2", itemCount), MessageTypes.success);
            this.ClearCache();
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

            this.Messages.Items.Cast<RepeaterItem>().Where(item => item.FindControlAs<CheckBox>("ItemCheck").Checked)
                .ForEach(
                    item =>
                        {
                            this.GetRepository<PMessage>().DeleteMessage(
                                item.FindControlAs<HiddenField>("MessageID").Value.ToType<int>(), this.View == PmView.Outbox);

                            itemCount++;
                        });

            this.BindData();

            this.PageContext.AddLoadMessage(
                itemCount == 1 ? this.GetText("msgdeleted1") : this.GetTextFormatted("msgdeleted2", itemCount),
                MessageTypes.success);
            this.ClearCache();
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
                this.PageContext.AddLoadMessage(this.GetText("NO_MESSAGES"), MessageTypes.warning);
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
            var exportPmIds = this.Messages.Items.Cast<RepeaterItem>()
                .Where(item => item.FindControlAs<CheckBox>("ItemCheck").Checked)
                .Select(item => item.FindControlAs<HiddenField>("MessageID").Value.ToType<int>()).ToList();

            var messageList = this.GetMessagesForExport(exportPmIds);

            // Return if No Message Selected
            if (!messageList.Table.HasRows())
            {
                this.PageContext.AddLoadMessage(this.GetText("MSG_NOSELECTED"), MessageTypes.warning);

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
        /// Get The Icon Image indicating if Unread or Read Message
        /// </summary>
        /// <param name="dataRow">The data row.</param>
        /// <returns>
        /// Returns the Image Url
        /// </returns>
        protected string GetIcon([NotNull] object dataRow)
        {
            var dataRowView = dataRow as DataRowView;
            var isRead = dataRowView["IsRead"].ToType<bool>();

            return $"<i class=\"fa fa-{(isRead ? "envelope-open" : "envelope")} fa-2x text-secondary\"></i>";
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
            return BuildLink.GetLink(
                ForumPages.PrivateMessage,
                "pm={0}&v={1}",
                messageId,
                PmViewConverter.ToQueryStringParam(this.View));
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
            [NotNull] object total,
            [NotNull] object inbox,
            [NotNull] object outbox,
            [NotNull] object archive,
            [NotNull] object limit)
        {
            object percentage = 0;
            if (limit.ToType<int>() != 0)
            {
                percentage = decimal.Round(total.ToType<decimal>() / limit.ToType<decimal>() * 100, 2);
            }

            if (!BoardContext.Current.IsAdmin)
            {
                return this.HtmlEncode(this.GetTextFormatted(text, total, inbox, outbox, archive, limit, percentage));
            }

            limit = "\u221E";
            percentage = 0;

            return this.HtmlEncode(this.GetTextFormatted(text, total, inbox, outbox, archive, limit, percentage));
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
            using (var dv = this.GetRepository<PMessage>().ListAsDataTable(this.PageContext.PageUserID, null, null)
                .DefaultView)
            {
                dv.RowFilter = this.View switch
                    {
                        PmView.Inbox => "IsRead = False AND IsDeleted = False AND IsArchived = False",
                        PmView.Outbox => "IsRead = False AND IsDeleted = False AND IsArchived = False",
                        PmView.Archive => "IsRead = False AND IsArchived = True",
                        _ => dv.RowFilter
                    };

                dv.Cast<DataRowView>().ForEach(
                    item =>
                        {
                            this.GetRepository<UserPMessage>().MarkAsRead(item["UserPMessageID"].ToType<int>());

                            // Clearing cache with old permissions data...
                            this.ClearCache();
                        });
            }

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
            var dt = this.GetRepository<PMessage>().UserMessageCount(this.PageContext.PageUserID);
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
        private DataView GetMessagesForExport([CanBeNull] ICollection<int> exportPmIds)
        {
            var messageList = (DataView)this.Messages.DataSource;

            for (var i = messageList.Table.Rows.Count - 1; i >= 0; i--)
            {
                var row = messageList.Table.Rows[i];

                if (exportPmIds != null && !exportPmIds.Contains(row["PMessageID"].ToType<int>()))
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

            using (var dv = this.GetRepository<PMessage>().ListAsDataTable(toUserId, fromUserId, null).DefaultView)
            {
                dv.RowFilter = this.View switch
                    {
                        PmView.Inbox => "IsDeleted = False AND IsArchived = False",
                        PmView.Outbox => "IsInOutbox = True AND IsArchived = False",
                        PmView.Archive => "IsArchived = True",
                        _ => dv.RowFilter
                    };

                dv.Sort = $"{this.ViewState["SortField"]} {(this.ViewState["SortAsc"].ToType<bool>() ? "asc" : "desc")}";

                var dataRows = dv.Cast<DataRowView>().Skip(this.PagerTop.CurrentPageIndex * this.PagerTop.PageSize)
                    .Take(this.PagerTop.PageSize);

                this.PagerTop.Count = dv.Count;

                if (dv.Count > 0)
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

                this.PagerTop.PageSize = 10;

                this.Messages.DataSource = dataRows;
                this.Messages.DataBind();
            }

            this.Stats_Renew();
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
        private void ExportCsvFile([NotNull] DataView messageList)
        {
            this.Get<HttpResponseBase>().Clear();
            this.Get<HttpResponseBase>().ClearContent();
            this.Get<HttpResponseBase>().ClearHeaders();

            this.Get<HttpResponseBase>().ContentType = "application/vnd.csv";
            this.Get<HttpResponseBase>().AppendHeader(
                "content-disposition",
                $"attachment; filename={HttpUtility.UrlEncode($"Privatemessages-{this.PageContext.PageUserName}-{DateTime.Now:yyyy'-'MM'-'dd'-'HHmm}.csv")}");

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

            messageList.Table.Rows.Cast<DataRow>().ForEach(
                dr =>
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
                    });

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
                $"attachment; filename={HttpUtility.UrlEncode($"Privatemessages-{this.PageContext.PageUserName}-{DateTime.Now:yyyy'-'MM'-'dd'-'HHmm}.txt")}");

            var sw = new StreamWriter(this.Get<HttpResponseBase>().OutputStream);

            sw.Write($"{this.Get<BoardSettings>().Name};{BoardInfo.ForumURL}");
            sw.Write(sw.NewLine);
            sw.Write($"Private Message Dump for User {this.PageContext.PageUserName}; {DateTime.Now}");
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
                $"attachment; filename=PrivateMessages-{this.PageContext.PageUserName}-{HttpUtility.UrlEncode(DateTime.Now.ToString("yyyy'-'MM'-'dd'-'HHmm"))}.xml");

            messageList.Table.TableName = "PrivateMessage";

            var settings = new XmlWriterSettings
                               {
                                   Encoding = Encoding.UTF8,
                                   OmitXmlDeclaration = false,
                                   Indent = true,
                                   NewLineOnAttributes = true
                               };

            var xw = XmlWriter.Create(this.Get<HttpResponseBase>().OutputStream, settings);
            xw.WriteStartDocument();

            messageList.Table.DataSet.DataSetName = "PrivateMessages";

            xw.WriteComment($" {this.Get<BoardSettings>().Name};{BoardInfo.ForumURL} ");
            xw.WriteComment($" Private Message Dump for User {this.PageContext.PageUserName}; {DateTime.Now} ");

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(messageList.Table.DataSet.GetXml());

            xmlDocument.ChildNodes.Cast<XmlNode>().ForEach(node => node.WriteTo(xw));

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