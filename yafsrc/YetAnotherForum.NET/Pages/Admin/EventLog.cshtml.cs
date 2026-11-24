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

using StackTraceBeautify;

using System.Collections.Generic;
using System.Globalization;

using FarsiLibrary.Core.Utils;

using Microsoft.AspNetCore.Mvc.Rendering;

using Newtonsoft.Json;

using YAF.Core.Context;
using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Types.Extensions;
using YAF.Types.Models;
using YAF.Types.Objects.Model;

/// <summary>
/// The Admin Event Log Page.
/// </summary>
public class EventLogModel : AdminPage
{
    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public EventLogInputModel Input { get; set; }

    /// <summary>
    /// Gets or sets the list.
    /// </summary>
    /// <value>The list.</value>
    [BindProperty]
    public List<PagedEventLog> List { get; set; }

    /// <summary>
    /// Gets or sets the number of the entries of each type in the period of the filter.
    /// </summary>
    public Dictionary<int, int> TypeCounts { get; set; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether the filter is not the default filter (it stays open on small screens).
    /// </summary>
    public bool FilterActive { get; set; }

    private readonly StackTraceBeautify beautify;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventLogModel"/> class.
    /// </summary>
    public EventLogModel()
        : base("ADMIN_EVENTLOG", ForumPages.Admin_EventLog)
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

        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_EVENTLOG", "TITLE"), string.Empty);
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
                var addressLink = this.HtmlEncode(
                    string.Format(this.PageBoardContext.BoardSettings.IPInfoPageURL, (string)json.UserIP));

                var textBgColor = this.PageBoardContext.PageUser.DarkMode ? "text-bg-dark" : "text-bg-light";

                var exceptionSource = ((string)json.ExceptionSource).IsSet() ?
                                          $"""<span class="badge {textBgColor} m-1"><i class="fa-solid fa-code me-1"></i>{this.HtmlEncode((string)json.ExceptionSource)}</span>"""
                                          : "";

                // the url of the request (path, query string) can contain any text
                var url = ((string)json.Url).IsSet()
                              ? $"""<span class="badge text-bg-secondary m-1"><i class="fa-solid fa-globe me-1"></i>{this.HtmlEncode((string)json.Url)}</span>"""
                              : "";

                var userIp = ((string)json.UserIP).IsSet()
                                 ? $"""<span class="badge text-bg-info m-1"><i class="fa-solid fa-desktop me-1"></i><a href="{addressLink}" target="_blank">{this.HtmlEncode((string)json.UserIP)}</a></span>"""
                                 : "";

                var userAgent = ((string)json.UserAgent).IsSet()
                                    ? $"""<span class="badge text-bg-secondary m-1"><i class="fa-solid fa-computer me-1"></i>{this.HtmlEncode((string)json.UserAgent)}</span>"""
                                    : "";

                return $"""
                        <h6 class="card-subtitle">{this.HtmlEncode((string)json.Message)}</h6><h5>{userIp}{url}{exceptionSource}{userAgent}</h5><div>{this.HtmlEncode((string)json.ExceptionMessage)}</div>
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
    public async Task OnPostDeleteAllAsync()
    {
        // only the entries of this board (like they are listed)
        await this.GetRepository<EventLog>().DeleteBoardEntriesAsync(this.PageBoardContext.PageBoardID);

        // re-bind controls
        this.BindData();
    }

    /// <summary>
    /// Gets the icon and the color of an event type.
    /// </summary>
    /// <param name="type">The event type.</param>
    /// <returns>The icon name and the color (a theme color like danger).</returns>
    public static (string Icon, string Color) GetTypeIcon(int type)
    {
        EventLogTypes eventType;

        try
        {
            // find out of what type event log entry is
            eventType = type.ToEnum<EventLogTypes>();
        }
        catch (Exception)
        {
            eventType = EventLogTypes.Information;
        }

        return eventType switch
        {
            EventLogTypes.Error => ("radiation", "danger"),
            EventLogTypes.Debug or EventLogTypes.Warning or EventLogTypes.Trace or EventLogTypes.SqlError => (
                "exclamation-triangle", "warning"),
            EventLogTypes.Information => ("exclamation", "info"),
            EventLogTypes.UserSuspended => ("user-clock", "warning"),
            EventLogTypes.UserUnsuspended => ("user-check", "info"),
            EventLogTypes.LoginFailure => ("user-injured", "warning"),
            EventLogTypes.UserDeleted => ("user-alt-slash", "danger"),
            EventLogTypes.IpBanSet or EventLogTypes.IpBanDetected => ("hand-paper", "warning"),
            EventLogTypes.IpBanLifted => ("slash", "success"),
            EventLogTypes.SpamBotReported => ("user-ninja", "warning"),
            EventLogTypes.SpamBotDetected => ("user-lock", "warning"),
            EventLogTypes.SpamMessageReported => ("flag", "success"),
            EventLogTypes.SpamMessageDetected => ("shield-alt", "warning"),
            _ => ("exclamation-circle", "primary")
        };
    }

    /// <summary>
    /// Gets the name of an event type (the words of the name, e.g. "Login Failure").
    /// </summary>
    /// <param name="type">The event type.</param>
    /// <returns>The name.</returns>
    public static string GetTypeName(int type)
    {
        // an unknown type shows its number
        if (!Enum.IsDefined(typeof(EventLogTypes), type))
        {
            return type.ToString();
        }

        // the translated name (the english name of the enum was shown in every language)
        return BoardContext.Current.Get<ILocalization>().GetText(
            "ADMIN_EVENTLOG",
            $"TYPE_{((EventLogTypes)type).ToString().ToUpperInvariant()}");
    }

    /// <summary>
    /// Gets the message of a log entry for the list (the details show the full entry).
    /// </summary>
    /// <param name="item">The log entry.</param>
    /// <returns>The message.</returns>
    public static string GetSummary(PagedEventLog item)
    {
        var text = item.Description ?? string.Empty;

        try
        {
            dynamic json = JsonConvert.DeserializeObject(text);

            string message = json?.Message;
            string exceptionMessage = json?.ExceptionMessage;

            text = message.IsSet() ? message : exceptionMessage ?? text;
        }
        catch (Exception)
        {
            // no json, the text is shown
        }

        text = text.Trim();

        return text.Length > 200 ? $"{text[..200]}…" : text;
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public void OnGet()
    {
        var (sinceDate, toDate) = this.GetDefaultDates();

        this.Input = new EventLogInputModel { SinceDate = sinceDate, ToDate = toDate };

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
    /// A chip of the types filters the list by its type.
    /// </summary>
    public void OnPostTypeFilter()
    {
        this.BindData();
    }

    /// <summary>
    /// delete just this particular log entry
    /// </summary>
    /// <param name="id">The identifier.</param>
    public async Task OnPostDeleteAsync(int id)
    {
        // delete just this particular log entry (of this board, the id is sent by the client)
        await this.GetRepository<EventLog>().DeleteBoardEntriesAsync(this.PageBoardContext.PageBoardID, id);

        // re-bind controls
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
        this.PageSizeList = new SelectList(StaticDataHelper.PageEntries(), nameof(SelectListItem.Value), nameof(SelectListItem.Text));

        var currentPageIndex = this.PageBoardContext.PageIndex;

        var sinceDate = DateTime.UtcNow.AddDays(-this.PageBoardContext.BoardSettings.EventLogMaxDays);
        var toDate = DateTime.UtcNow;

        var ci = this.Get<ILocalization>().Culture;

        if (this.Input.SinceDate.IsSet())
        {
            if (this.PageBoardContext.BoardSettings.UseFarsiCalender && ci.IsFarsiCulture())
            {
                // a value that is no date keeps the default
                try
                {
                    sinceDate = PersianDateConverter.ToGregorianDateTime(
                        new PersianDate(this.Input.SinceDate.PersianNumberToEnglish()));
                }
                catch (Exception)
                {
                    // the default
                }
            }
            else
            {
                if (DateTime.TryParse(this.Input.SinceDate, ci, DateTimeStyles.None, out var parsedSinceDate))
                {
                    sinceDate = parsedSinceDate;
                }
            }
        }

        if (this.Input.ToDate.IsSet())
        {
            if (this.PageBoardContext.BoardSettings.UseFarsiCalender && ci.IsFarsiCulture())
            {
                // a value that is no date keeps the default
                try
                {
                    toDate = PersianDateConverter.ToGregorianDateTime(
                        new PersianDate(this.Input.ToDate.PersianNumberToEnglish()));
                }
                catch (Exception)
                {
                    // the default
                }
            }
            else
            {
                if (DateTime.TryParse(this.Input.ToDate, ci, DateTimeStyles.None, out var parsedToDate))
                {
                    toDate = parsedToDate;
                }
            }
        }

        var (defaultSinceDate, defaultToDate) = this.GetDefaultDates();

        this.FilterActive = this.Input.Type != -1 || this.Input.SinceDate != defaultSinceDate
                                                  || this.Input.ToDate != defaultToDate;

        // the chips of the types count the entries of the period
        this.TypeCounts = this.GetRepository<EventLog>().CountByType(
            this.PageBoardContext.PageBoardID,
            sinceDate,
            toDate.AddDays(1).AddMinutes(-1));

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
                this.Input.Type.Equals(-1) ? null : this.Input.Type);
    }

    /// <summary>
    /// Gets the dates of the default filter (the days that are kept).
    /// </summary>
    /// <returns>The since date and the to date as they are shown in the fields.</returns>
    private (string SinceDate, string ToDate) GetDefaultDates()
    {
        var ci = this.Get<ILocalization>().Culture;

        if (this.PageBoardContext.BoardSettings.UseFarsiCalender && ci.IsFarsiCulture())
        {
            // the smallest persian date (1/01/02) was read as 1401/01/02 (2022)
            return (
                PersianDateConverter
                    .ToPersianDate(DateTime.UtcNow.AddDays(-this.PageBoardContext.BoardSettings.EventLogMaxDays))
                    .ToString("d"),
                PersianDateConverter.ToPersianDate(PersianDate.Now).ToString("d"));
        }

        return (
            DateTime.UtcNow.AddDays(-this.PageBoardContext.BoardSettings.EventLogMaxDays).ToString("yyyy-MM-dd"),
            DateTime.UtcNow.Date.ToString("yyyy-MM-dd"));
    }
}
