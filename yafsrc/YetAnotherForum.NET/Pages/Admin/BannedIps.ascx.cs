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

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    
    using YAF.Core.BasePages;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// The Admin Banned IP Page.
    /// </summary>
    public partial class BannedIps : AdminPage
    {
        #region Methods

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

            this.BindData();
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"),
                BuildLink.GetLink(ForumPages.Admin_Admin));

            this.PageLinks.AddLink(this.GetText("ADMIN_BANNEDIP", "TITLE"), string.Empty);

            this.Page.Header.Title =
                $"{this.GetText("ADMIN_ADMIN", "Administration")} - {this.GetText("ADMIN_BANNEDIP", "TITLE")}";
        }

        /// <summary>
        /// The list_ item command.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterCommandEventArgs"/> instance containing the event data.</param>
        protected void List_ItemCommand([NotNull] object sender, [NotNull] RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "add":
                    this.EditDialog.BindData(null);

                    BoardContext.Current.PageElements.RegisterJsBlockStartup(
                        "openModalJs",
                        JavaScriptBlocks.OpenModalJs("EditDialog"));
                    break;
                case "edit":
                    this.EditDialog.BindData(e.CommandArgument.ToType<int>());

                    BoardContext.Current.PageElements.RegisterJsBlockStartup(
                        "openModalJs",
                        JavaScriptBlocks.OpenModalJs("EditDialog"));
                    break;
                case "export":
                    {
                        var bannedIps = this.GetRepository<BannedIP>().GetByBoardId();

                        this.Get<HttpResponseBase>().Clear();
                        this.Get<HttpResponseBase>().ClearContent();
                        this.Get<HttpResponseBase>().ClearHeaders();

                        this.Get<HttpResponseBase>().ContentType = "application/vnd.text";
                        this.Get<HttpResponseBase>()
                            .AppendHeader("content-disposition", "attachment; filename=BannedIpsExport.txt");

                        var streamWriter = new StreamWriter(this.Get<HttpResponseBase>().OutputStream);

                        bannedIps.ForEach(
                            ip =>
                                {
                                    streamWriter.Write(ip.Mask);
                                    streamWriter.Write(streamWriter.NewLine);
                                });

                        streamWriter.Close();

                        this.Get<HttpResponseBase>().End();
                    }

                    break;
                case "delete":
                    {
                        var id = e.CommandArgument.ToType<int>();
                        var ipAddress = this.GetIPFromID(id);

                        this.GetRepository<BannedIP>().DeleteById(id);

                        this.PageContext.AddLoadMessage(
                            this.GetTextFormatted("MSG_REMOVEBAN_IP", ipAddress), MessageTypes.success);

                        this.BindData();

                        if (BoardContext.Current.Get<BoardSettings>().LogBannedIP)
                        {
                            this.Get<ILogger>()
                                .Log(
                                    this.PageContext.PageUserID,
                                    " YAF.Pages.Admin.bannedip",
                                    $"IP or mask {ipAddress} was deleted by {(this.Get<BoardSettings>().EnableDisplayName ? this.PageContext.CurrentUserData.DisplayName : this.PageContext.CurrentUserData.UserName)}.",
                                    EventLogTypes.IpBanLifted);
                        }
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
        /// Handles the Click event of the Search control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Search_Click(object sender, EventArgs e)
        {
            this.BindData();
        }

        /// <summary>
        /// Helper to get mask from ID.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <returns>
        /// Returns the IP
        /// </returns>
        private string GetIPFromID(int id)
        {
            return this.GetRepository<BannedIP>().GetById(id).Mask;
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            this.PagerTop.PageSize = this.Get<BoardSettings>().MemberListPageSize;

            var searchText = this.SearchInput.Text.Trim();

            List<BannedIP> bannedList;

            if (searchText.IsSet())
            {
                bannedList = this.GetRepository<BannedIP>().GetPaged(
                    x => x.BoardID == this.PageContext.PageBoardID && x.Mask == searchText,
                    this.PagerTop.CurrentPageIndex,
                    this.PagerTop.PageSize);
            }
            else
            {
                bannedList = this.GetRepository<BannedIP>().GetPaged(
                    x => x.BoardID == this.PageContext.PageBoardID,
                    this.PagerTop.CurrentPageIndex,
                    this.PagerTop.PageSize);
            }

            this.list.DataSource = bannedList;

            this.PagerTop.Count = bannedList != null && bannedList.Any()
                                      ? this.GetRepository<BannedIP>()
                                          .Count(x => x.BoardID == this.PageContext.PageBoardID).ToType<int>()
                                      : 0;

            this.DataBind();
        }

        #endregion
    }
}