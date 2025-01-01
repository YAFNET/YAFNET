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

using System.IO;

using YAF.Types.Models;

/// <summary>
/// The Admin Banned IP Page.
/// </summary>
public partial class BannedIps : AdminPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BannedIps"/> class. 
    /// </summary>
    public BannedIps()
        : base("ADMIN_BANNEDIP", ForumPages.Admin_BannedIps)
    {
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.IsPostBack)
        {
            return;
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
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot();
        this.PageBoardContext.PageLinks.AddAdminIndex();

        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_BANNEDIP", "TITLE"), string.Empty);
    }

    /// <summary>
    /// The list_ item command.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterCommandEventArgs"/> instance containing the event data.</param>
    protected void List_ItemCommand(object sender, RepeaterCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "add":
                this.EditDialog.BindData(null);

                this.PageBoardContext.PageElements.RegisterJsBlockStartup(
                    "openModalJs",
                    JavaScriptBlocks.OpenModalJs("EditDialog"));
                break;
            case "edit":
                this.EditDialog.BindData(e.CommandArgument.ToType<int>());

                this.PageBoardContext.PageElements.RegisterJsBlockStartup(
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

                    this.PageBoardContext.Notify(
                        this.GetTextFormatted("MSG_REMOVEBAN_IP", ipAddress), MessageTypes.success);

                    this.SearchInput.Text = string.Empty;

                    this.BindData();

                    if (this.PageBoardContext.BoardSettings.LogBannedIP)
                    {
                        this.Get<ILoggerService>()
                            .Log(
                                this.PageBoardContext.PageUserID,
                                " YAF.Pages.Admin.bannedip",
                                $"IP or mask {ipAddress} was deleted by {this.PageBoardContext.PageUser.DisplayOrUserName()}.",
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
    protected void PagerTop_PageChange(object sender, EventArgs e)
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
    /// Clear Search
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void ClearClick(object sender, EventArgs e)
    {
        this.SearchInput.Text = string.Empty;

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
        this.PagerTop.PageSize = this.PageSize.SelectedValue.ToType<int>();

        var searchText = this.SearchInput.Text.Trim();

        List<BannedIP> bannedList;

        if (searchText.IsSet())
        {
            bannedList = this.GetRepository<BannedIP>().GetPaged(
                x => x.BoardID == this.PageBoardContext.PageBoardID && x.Mask == searchText,
                this.PagerTop.CurrentPageIndex,
                this.PagerTop.PageSize);
        }
        else
        {
            bannedList = this.GetRepository<BannedIP>().GetPaged(
                x => x.BoardID == this.PageBoardContext.PageBoardID,
                this.PagerTop.CurrentPageIndex,
                this.PagerTop.PageSize);
        }

        this.list.DataSource = bannedList;

        this.PagerTop.Count = !bannedList.NullOrEmpty()
                                  ? this.GetRepository<BannedIP>()
                                      .Count(x => x.BoardID == this.PageBoardContext.PageBoardID).ToType<int>()
                                  : 0;

        this.DataBind();

        if (this.list.Items.Count == 0)
        {
            this.EmptyState.Visible = true;
        }
    }
}