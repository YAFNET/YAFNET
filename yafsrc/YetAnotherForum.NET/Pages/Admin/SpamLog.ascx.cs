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

using FarsiLibrary.Utils;

using Newtonsoft.Json;

using YAF.Core.Utilities.StringUtils;
using YAF.Web.Controls;

/// <summary>
/// The SPAM Event Log Page.
/// </summary>
public partial class SpamLog : AdminPage
{
    private readonly StackTraceBeautify beautify;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpamLog"/> class.
    /// </summary>
    public SpamLog()
        : base("ADMIN_EVENTLOG", ForumPages.Admin_SpamLog)
    {
        this.beautify = new StackTraceBeautify();
    }

    /// <summary>
    /// Delete Selected Event Log Entry
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void DeleteAllClick(object sender, EventArgs e)
    {
        this.GetRepository<Types.Models.EventLog>().DeleteAll();

        // re-bind controls
        this.BindData();
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
    override protected void OnInit(EventArgs e)
    {
        this.List.ItemCommand += this.ListItemCommand;

        base.OnInit(e);
    }

    /// <summary>
    /// The On PreRender event.
    /// </summary>
    /// <param name="e">
    /// the Event Arguments
    /// </param>
    override protected void OnPreRender(EventArgs e)
    {
        this.PageBoardContext.PageElements.RegisterJsBlock(
            "collapseToggleJs",
            JavaScriptBlocks.CollapseToggleJs(
                this.GetText("ADMIN_EVENTLOG", "HIDE"),
                this.GetText("ADMIN_EVENTLOG", "SHOW")));

        base.OnPreRender(e);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        // do it only once, not on post-backs
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

        var ci = this.Get<ILocalization>().Culture;

        if (this.PageBoardContext.BoardSettings.UseFarsiCalender && ci.IsFarsiCulture())
        {
            this.SinceDate.Text = PersianDateConverter.ToPersianDate(PersianDate.MinValue).ToString("d");
            this.ToDate.Text = PersianDateConverter.ToPersianDate(PersianDate.Now).ToString("d");
        }
        else
        {
            this.SinceDate.Text = DateTime.UtcNow.AddDays(-this.PageBoardContext.BoardSettings.EventLogMaxDays).ToString("yyyy-MM-dd");
            this.SinceDate.TextMode = TextBoxMode.Date;

            this.ToDate.Text = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
            this.ToDate.TextMode = TextBoxMode.Date;
        }

        // bind data to controls
        this.BindData();
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot();

        // administration index second
        this.PageBoardContext.PageLinks.AddAdminIndex();

        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_SPAMLOG", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Handles the PageChange event of the PagerTop control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    protected void PagerTopPageChange(object sender, EventArgs e)
    {
        // rebind
        this.BindData();
    }

    /// <summary>
    /// Handles the Click event of the ApplyButton control.
    /// </summary>
    /// <param name="source">The source of the event.</param>
    /// <param name="eventArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void ApplyButtonClick(object source, EventArgs eventArgs)
    {
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
    /// Renders the UserLink
    /// </summary>
    /// <param name="item">
    /// The item.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    protected string UserLink(PagedEventLog item)
    {
        if (item.UserID == 0 || item.UserID == this.PageBoardContext.GuestUserID)
        {
            return string.Empty;
        }

        var userLink = new UserLink
                           {
                               UserID = item.UserID,
                               Suspended = item.Suspended,
                               Style = item.UserStyle,
                               ReplaceName = this.PageBoardContext.BoardSettings.EnableDisplayName ? item.DisplayName : item.Name
                           };

        return userLink.RenderToString();
    }

    /// <summary>
    /// Formats the stack trace.
    /// </summary>
    /// <param name="input">The json input stack trace string</param>
    /// <returns>System.String.</returns>
    protected string FormatStackTrace(string input)
    {
        try
        {
            dynamic json = JsonConvert.DeserializeObject(input);

            try
            {
                var addressLink = string.Format(this.PageBoardContext.BoardSettings.IPInfoPageURL, json.UserIP);

                var exceptionSource = ((string)json.ExceptionSource).IsSet() ?
                    $"""<span class="badge text-light-emphasis bg-light-subtle m-1"><i class="fa-solid fa-code me-1"></i>{json.ExceptionSource}</span>"""
                    : "";

                var url = ((string)json.Url).IsSet()
                              ? $"""<span class="badge text-bg-secondary m-1"><i class="fa-solid fa-globe me-1"></i>{json.Url}</span>"""
                              : "";

                var userIp = ((string)json.UserIP).IsSet()
                                  ? $"""<span class="badge text-bg-info m-1"><i class="fa-solid fa-desktop me-1"></i><a href="{addressLink}" target="_blank">{json.UserIP}</a></span>"""
                                  : "";

                var userAgent = ((string)json.Url).IsSet()
                                     ? $"""<span class="badge text-bg-secondary m-1"><i class="fa-solid fa-computer me-1"></i>{json.UserAgent}</span>"""
                                     : "";

                return $"""
                        <h6 class="card-subtitle">{json.Message}</h6><h5>{userIp}{url}{exceptionSource}{userAgent}</h5><div>{json.ExceptionMessage}</div>
                                                 <div>{this.beautify.Beautify(this.HtmlEncode(json.ExceptionStackTrace.ToString()))}</div>
                        """;
            }
            catch (Exception)
            {
                return this.beautify.Beautify(this.HtmlEncode(input));
            }
        }
        catch (JsonReaderException)
        {
            return this.beautify.Beautify(this.HtmlEncode(input));
        }
    }

    /// <summary>
    /// Populates data source and binds data to controls.
    /// </summary>
    private void BindData()
    {
        var baseSize = this.PageSize.SelectedValue.ToType<int>();
        var currentPageIndex = this.PagerTop.CurrentPageIndex;
        this.PagerTop.PageSize = baseSize;

        var sinceDate = DateTime.UtcNow.AddDays(-this.PageBoardContext.BoardSettings.EventLogMaxDays);
        var toDate = DateTime.UtcNow;

        var ci = this.Get<ILocalization>().Culture;

        if (this.SinceDate.Text.IsSet())
        {
            if (this.PageBoardContext.BoardSettings.UseFarsiCalender && ci.IsFarsiCulture())
            {
                var persianDate = new PersianDate(this.SinceDate.Text.PersianNumberToEnglish());

                sinceDate = PersianDateConverter.ToGregorianDateTime(persianDate);
            }
            else
            {
                DateTime.TryParse(this.SinceDate.Text, ci, DateTimeStyles.None, out sinceDate);
            }
        }

        if (this.ToDate.Text.IsSet())
        {
            if (this.PageBoardContext.BoardSettings.UseFarsiCalender && ci.IsFarsiCulture())
            {
                var persianDate = new PersianDate(this.ToDate.Text.PersianNumberToEnglish());

                toDate = PersianDateConverter.ToGregorianDateTime(persianDate);
            }
            else
            {
                DateTime.TryParse(this.ToDate.Text, ci, DateTimeStyles.None, out toDate);
            }
        }

        // list event for this board
        var list = this.GetRepository<Types.Models.EventLog>()
            .ListPaged(
                this.PageBoardContext.PageBoardID,
                this.PageBoardContext.BoardSettings.EventLogMaxMessages,
                this.PageBoardContext.BoardSettings.EventLogMaxDays,
                currentPageIndex,
                baseSize,
                sinceDate,
                toDate.AddDays(1).AddMinutes(-1),
                null,
                true);

        this.List.DataSource = list;

        this.PagerTop.Count = !list.NullOrEmpty()
                                  ? list.FirstOrDefault().TotalRows
                                  : 0;

        // bind data to controls
        this.DataBind();

        if (this.List.Items.Count == 0)
        {
            this.NoInfo.Visible = true;
        }
    }

    /// <summary>
    /// Handles single record commands in a repeater.
    /// </summary>
    /// <param name="source">The source of the event.</param>
    /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterCommandEventArgs"/> instance containing the event data.</param>
    private void ListItemCommand(object source, RepeaterCommandEventArgs e)
    {
        // what command are we serving?
        switch (e.CommandName)
        {
            // delete log entry
            case "delete":

                // delete just this particular log entry
                this.GetRepository<Types.Models.EventLog>().DeleteById(e.CommandArgument.ToType<int>());

                // re-bind controls
                this.BindData();
                break;
        }
    }
}