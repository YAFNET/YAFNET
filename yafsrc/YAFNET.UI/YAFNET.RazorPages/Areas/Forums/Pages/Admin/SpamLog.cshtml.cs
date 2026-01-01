
/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

using System.Threading.Tasks;

namespace YAF.Pages.Admin;

using System.Collections.Generic;
using System.Globalization;

using FarsiLibrary.Core.Utils;

using Microsoft.AspNetCore.Mvc.Rendering;

using Newtonsoft.Json;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Core.Utilities.StringUtils;
using YAF.Types.Extensions;
using YAF.Types.Models;
using YAF.Types.Objects.Model;

/// <summary>
/// The SPAM Event Log Page.
/// </summary>
public class SpamLogModel : AdminPage
{
    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public SpamLogInputModel Input { get; set; }

    /// <summary>
    /// Gets or sets the list.
    /// </summary>
    /// <value>The list.</value>
    [BindProperty]
    public List<PagedEventLog> List { get; set; }

    private readonly StackTraceBeautify beautify;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpamLogModel"/> class.
    /// </summary>
    public SpamLogModel()
        : base("ADMIN_SPAMLOG", ForumPages.Admin_SpamLog)
    {
        this.beautify = new StackTraceBeautify();
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        // administration index second
        this.PageBoardContext.PageLinks.AddAdminIndex();

        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_SPAMLOG", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Formats the stack trace.
    /// </summary>
    /// <param name="input">The JSON input stack trace.</param>
    /// <returns>System.String.</returns>
    public string FormatStackTrace(string input)
    {
        try
        {
            dynamic json = JsonConvert.DeserializeObject(input);

            try
            {
                var addressLink = string.Format(this.PageBoardContext.BoardSettings.IPInfoPageURL, json.UserIP);

                var exceptionSource = ((string)json.ExceptionSource).IsSet()
                    ? $"""<span class="badge text-light-emphasis bg-light-subtle m-1"><i class="fa-solid fa-code me-1"></i>{json.ExceptionSource}</span>"""
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
    /// Delete Selected Event Log Entry
    /// </summary>
    public void OnPostDeleteAll()
    {
        this.GetRepository<EventLog>().DeleteAll();

        // re-bind controls
        this.BindData();
    }

    /// <summary>
    /// delete just this particular log entry
    /// </summary>
    /// <param name="id">The identifier.</param>
    public async Task OnPostDeleteAsync(int id)
    {
        // delete just this particular log entry
        await this.GetRepository<EventLog>().DeleteByIdAsync(id);

        // re-bind controls
        this.BindData();
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public void OnGet()
    {
        this.Input = new SpamLogInputModel();

        var ci = this.Get<ILocalization>().Culture;

        if (this.PageBoardContext.BoardSettings.UseFarsiCalender && ci.IsFarsiCulture())
        {
            this.Input.SinceDate = PersianDateConverter.ToPersianDate(PersianDate.MinValue).ToString("d");
            this.Input.ToDate = PersianDateConverter.ToPersianDate(PersianDate.Now).ToString("d");
        }
        else
        {
            this.Input.SinceDate = DateTime.UtcNow.AddDays(-this.PageBoardContext.BoardSettings.EventLogMaxDays)
                .ToString("yyyy-MM-dd");

            this.Input.ToDate = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
        }

        // bind data to controls
        this.BindData();
    }

    /// <summary>
    /// Handles the Click event of the ApplyButton control.
    /// </summary>
    public void OnPostApplyButton()
    {
        this.BindData();
    }

    /// <summary>
    /// The page size on selected index changed.
    /// </summary>
    public void OnPost()
    {
        this.BindData();
    }

    /// <summary>
    /// Populates data source and binds data to controls.
    /// </summary>
    private void BindData()
    {
        this.PageSizeList = new SelectList(StaticDataHelper.PageEntries(), nameof(SelectListItem.Value),
            nameof(SelectListItem.Text));

        var currentPageIndex = this.PageBoardContext.PageIndex;

        var sinceDate = DateTime.UtcNow.AddDays(-this.PageBoardContext.BoardSettings.EventLogMaxDays);
        var toDate = DateTime.UtcNow;

        var ci = this.Get<ILocalization>().Culture;

        if (this.Input.SinceDate.IsSet())
        {
            if (this.PageBoardContext.BoardSettings.UseFarsiCalender && ci.IsFarsiCulture())
            {
                var persianDate = new PersianDate(this.Input.SinceDate.PersianNumberToEnglish());

                sinceDate = PersianDateConverter.ToGregorianDateTime(persianDate);
            }
            else
            {
                DateTime.TryParse(this.Input.SinceDate, ci, DateTimeStyles.None, out sinceDate);
            }
        }

        if (this.Input.ToDate.IsSet())
        {
            if (this.PageBoardContext.BoardSettings.UseFarsiCalender && ci.IsFarsiCulture())
            {
                var persianDate = new PersianDate(this.Input.ToDate.PersianNumberToEnglish());

                toDate = PersianDateConverter.ToGregorianDateTime(persianDate);
            }
            else
            {
                DateTime.TryParse(this.Input.ToDate, ci, DateTimeStyles.None, out toDate);
            }
        }

        // list event for this board
        this.List = this.GetRepository<EventLog>()
            .ListPaged(
                this.PageBoardContext.PageBoardID,
                this.PageBoardContext.BoardSettings.EventLogMaxMessages,
                this.PageBoardContext.BoardSettings.EventLogMaxDays,
                currentPageIndex,
                this.Size,
                sinceDate,
                toDate.AddDays(1).AddMinutes(-1),
                null,
                true);
    }
}