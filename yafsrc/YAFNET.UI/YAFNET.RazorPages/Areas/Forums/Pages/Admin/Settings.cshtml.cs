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
using System.IO;
using System.Linq;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Types.Extensions;
using YAF.Types.Objects;
using YAF.Types.Models;

using Microsoft.AspNetCore.Mvc.Rendering;

/// <summary>
/// The Board Settings Admin Page.
/// </summary>
public class SettingsModel : AdminPage
{
    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public SettingsInputModel Input { get; set; }

    /// <summary>
    /// Gets or sets the logo images.
    /// </summary>
    /// <value>The logo images.</value>
    public List<SelectListItem> LogoImages { get; set; }

    /// <summary>
    /// Gets or sets the themes.
    /// </summary>
    /// <value>The themes.</value>
    public IReadOnlyCollection<SelectListItem> Themes { get; set; }

    /// <summary>
    /// Gets or sets the cultures.
    /// </summary>
    /// <value>The cultures.</value>
    public SelectList Cultures { get; set; }

    /// <summary>
    /// Gets or sets the show topics.
    /// </summary>
    /// <value>The show topics.</value>
    public SelectList ShowTopics { get; set; }

    /// <summary>
    /// Gets or sets the forum default access masks.
    /// </summary>
    /// <value>The forum default access masks.</value>
    public SelectList ForumDefaultAccessMasks { get; set; }

    /// <summary>
    /// Gets or sets the default notification settings.
    /// </summary>
    /// <value>The default notification settings.</value>
    public List<SelectListItem> DefaultNotificationSettings { get; set; }

    /// <summary>
    /// Gets or sets the default collapsible panel states.
    /// </summary>
    /// <value>The default collapsible panel states.</value>
    public List<SelectListItem> DefaultCollapsiblePanelStates { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsModel"/> class.
    /// </summary>
    public SettingsModel()
        : base("ADMIN_BOARDSETTINGS", ForumPages.Admin_Settings)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_BOARDSETTINGS", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public Task OnGetAsync()
    {
        this.Input = new SettingsInputModel();

        return this.BindDataAsync();
    }

    /// <summary>
    /// Save the Board Settings
    /// </summary>
    public IActionResult OnPostSave()
    {
        var languageFile = "english.json";

        var cultures = StaticDataHelper.Cultures().Where(c => c.CultureTag.Equals(this.Input.Culture)).ToList();

        if (cultures.Count != 0)
        {
            languageFile = cultures.FirstOrDefault()!.CultureFile;
        }

        this.GetRepository<Board>().Save(
            this.PageBoardContext.PageBoardID,
            this.Input.Name,
            this.Input.Description,
            languageFile,
            this.Input.Culture);

        // save poll group
        var boardSettings = this.Get<BoardSettingsService>().LoadBoardSettings(this.PageBoardContext.PageBoardID, null);

        boardSettings.Language = languageFile;
        boardSettings.Culture = this.Input.Culture;
        boardSettings.Theme = this.Input.Theme;
        boardSettings.ForumDefaultAccessMask = this.Input.ForumDefaultAccessMask;
        boardSettings.ShowTopicsDefault = this.Input.ShowTopic;
        boardSettings.NotificationOnUserRegisterEmailList = this.Input.NotificationOnUserRegisterEmailList;
        boardSettings.EmailModeratorsOnModeratedPost = this.Input.EmailModeratorsOnModeratedPost;
        boardSettings.EmailModeratorsOnReportedPost = this.Input.EmailModeratorsOnReportedPost;
        boardSettings.AllowDigestEmail = this.Input.AllowDigestEmail;
        boardSettings.DefaultSendDigestEmail = this.Input.DefaultSendDigestEmail;
        boardSettings.DefaultNotificationSetting =
            this.Input.DefaultNotificationSetting.ToEnum<UserNotificationSetting>();
        boardSettings.DefaultCollapsiblePanelState =
            this.Input.DefaultCollapsiblePanelState.ToEnum<CollapsiblePanelState>();
        boardSettings.ForumEmail = this.Input.ForumEmail;
        boardSettings.DigestSendEveryXHours = this.Input.DigestSendEveryXHours;
        boardSettings.PageSizeDefault = this.Input.DefaultPageSize;

        if (this.Input.BoardLogo.IsSet())
        {
            boardSettings.ForumLogo = this.Input.BoardLogo;
        }

        // save the settings to the database
        this.Get<BoardSettingsService>().SaveRegistry(boardSettings);

        // Clearing cache with old users permissions data to get new default styles...
        this.Get<IDataCache>().Remove(x => x.StartsWith(Constants.Cache.ActiveUserLazyData));

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_Admin);
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private async Task BindDataAsync()
    {
        this.CreateLogosList();

        var board = await this.GetRepository<Board>().GetByIdAsync(this.PageBoardContext.PageBoardID);

        var boardSettings = this.PageBoardContext.BoardSettings;

        this.Input.Name = board.Name;
        this.Input.Description = board.Description;

        this.Themes = StaticDataHelper.Themes();

        this.Cultures = new SelectList(
            StaticDataHelper.Cultures().OrderBy(x => x.CultureNativeName),
            nameof(Culture.CultureTag),
            nameof(Culture.CultureNativeName));

        this.ShowTopics = new SelectList(
            StaticDataHelper.TopicTimes(),
            nameof(SelectListItem.Value),
            nameof(SelectListItem.Text));

        // population default notification setting options...
        var items = EnumHelper.EnumToDictionary<UserNotificationSetting>();

        this.DefaultNotificationSettings = [];

        items.ForEach(
            x => this.DefaultNotificationSettings.Add(
                new SelectListItem(HtmlTagHelper.StripHtml(this.GetText("SUBSCRIPTIONS", x.Value)), x.Key.ToString())));

        this.DefaultCollapsiblePanelStates = [
            new SelectListItem(
                this.GetText(
                    "ADMIN_BOARDSETTINGS",
                    "EXPANDED"),
                "0"),

            new SelectListItem(
                this.GetText(
                    "ADMIN_BOARDSETTINGS",
                    "COLLAPSED"),
                "1")
        ];

        this.ForumDefaultAccessMasks = new SelectList(
            await this.GetRepository<AccessMask>().GetByBoardIdAsync(),
            nameof(AccessMask.ID),
            nameof(AccessMask.Name));

        this.Input.ForumDefaultAccessMask = boardSettings.ForumDefaultAccessMask;

        this.Input.Theme = boardSettings.Theme.Contains(".xml") ? "yaf" : boardSettings.Theme;

        var langFileCulture = StaticDataHelper.CultureDefaultFromFile(boardSettings.Language) ?? "en-US";

        this.Input.Culture = boardSettings.Culture;

        if (this.Input.Culture.IsNotSet())
        {
            // If 2-letter language code is the same we return Culture, else we return  a default full culture from language file
            this.Input.Culture =
                langFileCulture[..2] == boardSettings.Culture ? boardSettings.Culture : langFileCulture;
        }

        this.Input.ShowTopic = boardSettings.ShowTopicsDefault;

        this.Input.DefaultNotificationSetting = boardSettings.DefaultNotificationSetting.ToInt();
        this.Input.DefaultCollapsiblePanelState = boardSettings.DefaultCollapsiblePanelState.ToInt();
        this.Input.DefaultPageSize = boardSettings.PageSizeDefault;

        this.Input.NotificationOnUserRegisterEmailList = boardSettings.NotificationOnUserRegisterEmailList;
        this.Input.EmailModeratorsOnModeratedPost = boardSettings.EmailModeratorsOnModeratedPost;
        this.Input.EmailModeratorsOnReportedPost = boardSettings.EmailModeratorsOnReportedPost;
        this.Input.AllowDigestEmail = boardSettings.AllowDigestEmail;
        this.Input.DefaultSendDigestEmail = boardSettings.DefaultSendDigestEmail;
        this.Input.ForumEmail = boardSettings.ForumEmail;

        this.Input.BoardLogo = boardSettings.ForumLogo;

        this.Input.DigestSendEveryXHours = boardSettings.DigestSendEveryXHours;
    }

    /// <summary>
    /// create images List.
    /// </summary>
    protected void CreateLogosList()
    {
        var list = new List<SelectListItem> {new(this.GetText("BOARD_LOGO_SELECT"), string.Empty)};

        var dir = new DirectoryInfo(
            Path.Combine(this.Get<BoardInfo>().WebRootPath, this.Get<BoardFolders>().Logos));

        if (dir.Exists)
        {
            var files = dir.GetFiles("*.*").ToList();

            list.AddImageFiles(files, this.Get<BoardFolders>().Logos);
        }

        this.LogoImages = list;
    }
}