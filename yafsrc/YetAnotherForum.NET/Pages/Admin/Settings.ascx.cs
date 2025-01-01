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

using YAF.Types.Objects;
using YAF.Types.Models;

using ListItem = ListItem;

/// <summary>
/// The Board Settings Admin Page.
/// </summary>
public partial class Settings : AdminPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Settings"/> class.
    /// </summary>
    public Settings()
        : base("ADMIN_BOARDSETTINGS", ForumPages.Admin_Settings)
    {
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
            nameof(JavaScriptBlocks.FormValidatorJs),
            JavaScriptBlocks.FormValidatorJs(this.Save.ClientID));

        if (this.IsPostBack)
        {
            return;
        }

        this.BindData();
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddLink(this.PageBoardContext.BoardSettings.Name, this.Get<LinkBuilder>().GetLink(ForumPages.Board));
        this.PageBoardContext.PageLinks.AddAdminIndex();
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_BOARDSETTINGS", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Save the Board Settings
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void SaveClick(object sender, EventArgs e)
    {
        var languageFile = "english.json";

        var cultures = StaticDataHelper.Cultures()
            .Where(c => c.CultureTag.Equals(this.Culture.SelectedValue)).ToList();

        if (cultures.Any())
        {
            languageFile = cultures.FirstOrDefault().CultureFile;
        }

        this.GetRepository<Board>().Save(
            this.PageBoardContext.PageBoardID,
            this.Name.Text,
            languageFile,
            this.Culture.SelectedValue);

        // save poll group
        var boardSettings = this.Get<BoardSettingsService>().LoadBoardSettings(this.PageBoardContext.PageBoardID, null);

        boardSettings.Language = languageFile;
        boardSettings.Culture = this.Culture.SelectedValue;
        boardSettings.Theme = this.Theme.SelectedValue;
        boardSettings.ForumDefaultAccessMask = this.ForumDefaultAccessMask.SelectedValue.ToType<int>();
        boardSettings.ShowTopicsDefault = this.ShowTopic.SelectedValue.ToType<int>();
        boardSettings.NotificationOnUserRegisterEmailList = this.NotificationOnUserRegisterEmailList.Text.Trim();
        boardSettings.EmailModeratorsOnModeratedPost = this.EmailModeratorsOnModeratedPost.Checked;
        boardSettings.EmailModeratorsOnReportedPost = this.EmailModeratorsOnReportedPost.Checked;
        boardSettings.AllowDigestEmail = this.AllowDigestEmail.Checked;
        boardSettings.DefaultSendDigestEmail = this.DefaultSendDigestEmail.Checked;
        boardSettings.DefaultNotificationSetting = this.DefaultNotificationSetting.SelectedValue.ToEnum<UserNotificationSetting>();
        boardSettings.DefaultCollapsiblePanelState = this.DefaultCollapsiblePanelState.SelectedValue.ToEnum<CollapsiblePanelState>();
        boardSettings.PageSizeDefault = this.DefaultPageSize.Text.ToType<int>();
        boardSettings.BaseUrlMask = this.ForumBaseUrlMask.Text;
        boardSettings.ForumEmail = this.ForumEmail.Text;
        boardSettings.DigestSendEveryXHours = this.DigestSendEveryXHours.Text.ToType<int>();

        if (this.BoardLogo.SelectedIndex > 0)
        {
            boardSettings.ForumLogo = this.BoardLogo.SelectedItem.Text;
        }

        // save the settings to the database
        this.Get<BoardSettingsService>().SaveRegistry(boardSettings);

        // Clearing cache with old users permissions data to get new default styles...
        this.Get<IDataCache>().Remove(x => x.StartsWith(Constants.Cache.ActiveUserLazyData));

        this.Get<LinkBuilder>().Redirect(ForumPages.Admin_Admin);
    }

    /// <summary>
    /// Increases the CDV version on click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void IncreaseVersionOnClick(object sender, EventArgs e)
    {
        this.PageBoardContext.BoardSettings.CdvVersion++;
        this.Get<BoardSettingsService>().SaveRegistry(this.PageBoardContext.BoardSettings);

        this.CdvVersion.Text = this.PageBoardContext.BoardSettings.CdvVersion.ToString();
    }

    /// <summary>
    /// The set selected on list.
    /// </summary>
    /// <param name="list">The list.</param>
    /// <param name="value">The value.</param>
    private static void SetSelectedOnList(ref DropDownList list, string value)
    {
        var selItem = list.Items.FindByValue(value);

        list.ClearSelection();

        if (selItem != null)
        {
            selItem.Selected = true;
        }
        else if (list.Items.Count > 0)
        {
            // select the first...
            list.SelectedIndex = 0;
        }
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        var board = this.GetRepository<Board>().GetById(this.PageBoardContext.PageBoardID);

        var logos = new List<NamedParameter> {
                                                     new(this.GetText("BOARD_LOGO_SELECT"), string.Empty)
                                                 };

        var dir = new DirectoryInfo(
            this.Get<HttpRequestBase>().MapPath($"{BoardInfo.ForumServerFileRoot}{this.Get<BoardFolders>().Logos}"));

        var files = dir.GetFiles("*.*").ToList();

        logos.AddImageFiles(files, this.Get<BoardFolders>().Logos);

        this.BoardLogo.PlaceHolder = this.GetText("BOARD_LOGO_SELECT");

        this.BoardLogo.DataSource = logos;
        this.BoardLogo.DataValueField = "Value";
        this.BoardLogo.DataTextField = "Name";
        this.BoardLogo.DataBind();

        this.Name.Text = board.Name;

        var boardSettings = this.PageBoardContext.BoardSettings;

        this.CdvVersion.Text = boardSettings.CdvVersion.ToString();

        // create list boxes by populating data sources from Data class
        var themeData = StaticDataHelper.Themes();

        if (themeData.Any())
        {
            this.Theme.DataSource = themeData;
            this.Theme.DataBind();
        }

        this.Culture.DataSource = StaticDataHelper.Cultures().OrderBy(x => x.CultureNativeName);

        this.Culture.DataTextField = "CultureNativeName";
        this.Culture.DataValueField = "CultureTag";
        this.Culture.DataBind();

        this.ShowTopic.DataSource = StaticDataHelper.TopicTimes();
        this.ShowTopic.DataTextField = "Name";
        this.ShowTopic.DataValueField = "Value";
        this.ShowTopic.DataBind();

        // population default notification setting options...
        var items = EnumHelper.EnumToDictionary<UserNotificationSetting>();

        var notificationItems = items.Select(
                x => new ListItem(HtmlTagHelper.StripHtml(this.GetText("SUBSCRIPTIONS", x.Value)), x.Key.ToString()))
            .ToArray();

        this.DefaultNotificationSetting.Items.AddRange(notificationItems);

        this.DefaultCollapsiblePanelState.Items.Add(
            new ListItem {Text = this.GetText("ADMIN_BOARDSETTINGS", "EXPANDED"), Value = "0"});
        this.DefaultCollapsiblePanelState.Items.Add(
            new ListItem {Text = this.GetText("ADMIN_BOARDSETTINGS", "COLLAPSED"), Value = "1"});

        // Get first default full culture from a language file tag.
        var langFileCulture = StaticDataHelper.CultureDefaultFromFile(boardSettings.Language) ?? "en-US";

        this.ForumDefaultAccessMask.DataSource = this.GetRepository<AccessMask>().GetByBoardId();
        this.ForumDefaultAccessMask.DataValueField = "ID";
        this.ForumDefaultAccessMask.DataTextField = "Name";
        this.ForumDefaultAccessMask.DataBind();

        SetSelectedOnList(ref this.ForumDefaultAccessMask, boardSettings.ForumDefaultAccessMask.ToString());

        if (boardSettings.Theme.Contains(".xml"))
        {
            SetSelectedOnList(ref this.Theme, "yaf");
        }
        else
        {
            SetSelectedOnList(ref this.Theme, boardSettings.Theme);
        }

        SetSelectedOnList(ref this.Culture, boardSettings.Culture);
        if (this.Culture.SelectedIndex == 0)
        {
            // If 2-letter language code is the same we return Culture, else we return  a default full culture from language file
            SetSelectedOnList(
                ref this.Culture,
                langFileCulture.Substring(0, 2) == boardSettings.Culture ? boardSettings.Culture : langFileCulture);
        }

        SetSelectedOnList(ref this.ShowTopic, boardSettings.ShowTopicsDefault.ToString());
        SetSelectedOnList(
            ref this.DefaultNotificationSetting,
            boardSettings.DefaultNotificationSetting.ToInt().ToString());
        SetSelectedOnList(
            ref this.DefaultCollapsiblePanelState,
            boardSettings.DefaultCollapsiblePanelState.ToInt().ToString());

        this.NotificationOnUserRegisterEmailList.Text = boardSettings.NotificationOnUserRegisterEmailList;
        this.EmailModeratorsOnModeratedPost.Checked = boardSettings.EmailModeratorsOnModeratedPost;
        this.EmailModeratorsOnReportedPost.Checked = boardSettings.EmailModeratorsOnReportedPost;
        this.AllowDigestEmail.Checked = boardSettings.AllowDigestEmail;
        this.DefaultSendDigestEmail.Checked = boardSettings.DefaultSendDigestEmail;
        this.ForumEmail.Text = boardSettings.ForumEmail;
        this.ForumBaseUrlMask.Text = boardSettings.BaseUrlMask;
        this.DefaultPageSize.Text = boardSettings.PageSizeDefault.ToString();

        var item = this.BoardLogo.Items.FindByText(boardSettings.ForumLogo);

        if (item != null)
        {
            item.Selected = true;
        }

        this.DigestSendEveryXHours.Text = boardSettings.DigestSendEveryXHours.ToString();
    }
}