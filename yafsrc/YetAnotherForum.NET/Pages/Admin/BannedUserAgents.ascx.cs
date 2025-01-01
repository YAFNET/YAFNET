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

/// <summary>
/// The Admin Banned User Agents Page.
/// </summary>
public partial class BannedUserAgents : AdminPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BannedUserAgents"/> class. 
    /// </summary>
    public BannedUserAgents()
        : base("ADMIN_BANNED_USERAGENTS", ForumPages.Admin_BannedUserAgents)
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

        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_BANNED_USERAGENTS", "TITLE"), string.Empty);
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
                    var bannedUserAgents = this.GetRepository<Types.Models.BannedUserAgent>().Get(x => x.BoardID == this.PageBoardContext.PageBoardID);

                    this.Get<HttpResponseBase>().Clear();
                    this.Get<HttpResponseBase>().ClearContent();
                    this.Get<HttpResponseBase>().ClearHeaders();

                    this.Get<HttpResponseBase>().ContentType = "application/vnd.text";
                    this.Get<HttpResponseBase>()
                        .AppendHeader("content-disposition", "attachment; filename=BannedEmailsExport.txt");

                    var streamWriter = new StreamWriter(this.Get<HttpResponseBase>().OutputStream);

                    bannedUserAgents.ForEach(
                        name =>
                            {
                                streamWriter.Write(name.UserAgent);
                                streamWriter.Write(streamWriter.NewLine);
                            });

                    streamWriter.Close();

                    this.Get<HttpResponseBase>().End();
                }

                break;
            case "delete":
                {
                    this.GetRepository<Types.Models.BannedUserAgent>().DeleteById(e.CommandArgument.ToType<int>());

                    this.PageBoardContext.Notify(
                        this.GetText("ADMIN_BANNED_USERAGENTS", "MSG_REMOVEBAN_NAME"),
                        MessageTypes.success);

                    this.SearchInput.Text = string.Empty;

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
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        this.PagerTop.PageSize = this.PageSize.SelectedValue.ToType<int>();

        var searchText = this.SearchInput.Text.Trim();

        List<Types.Models.BannedUserAgent> bannedList;

        if (searchText.IsSet())
        {
            bannedList = this.GetRepository<Types.Models.BannedUserAgent>().GetPaged(
                x => x.BoardID == this.PageBoardContext.PageBoardID && x.UserAgent == searchText,
                this.PagerTop.CurrentPageIndex,
                this.PagerTop.PageSize);
        }
        else
        {
            bannedList = this.GetRepository<Types.Models.BannedUserAgent>().GetPaged(
                x => x.BoardID == this.PageBoardContext.PageBoardID,
                this.PagerTop.CurrentPageIndex,
                this.PagerTop.PageSize);
        }

        this.list.DataSource = bannedList;

        this.PagerTop.Count = !bannedList.NullOrEmpty()
                                  ? this.GetRepository<Types.Models.BannedUserAgent>()
                                      .Count(x => x.BoardID == this.PageBoardContext.PageBoardID).ToType<int>()
                                  : 0;

        this.DataBind();

        if (this.list.Items.Count == 0)
        {
            this.EmptyState.Visible = true;
        }
    }
}