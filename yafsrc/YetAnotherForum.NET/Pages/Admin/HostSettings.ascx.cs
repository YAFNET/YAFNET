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
    using System.Linq;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core;
    using YAF.Core.BasePages;
    using YAF.Core.Context;
    using YAF.Core.Helpers;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// The Host Settings Page.
    /// </summary>
    public partial class HostSettings : AdminPage
    {
        #region Methods

        /// <summary>
        /// Updates the Search Index
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void IndexSearch_OnClick(object sender, EventArgs e)
        {
            this.Get<BoardSettings>().ForceUpdateSearchIndex = true;
            ((LoadBoardSettings)BoardContext.Current.BoardSettings).SaveRegistry();

            this.PageContext.AddLoadMessage(this.GetText("FORCE_SEARCHINDED"), MessageTypes.info);
        }

        /// <summary>
        /// Resets the Active Discussions Cache
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ActiveDiscussionsCacheResetClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.RemoveCacheKey(Constants.Cache.ActiveDiscussions);
            this.RemoveCacheKey(Constants.Cache.ForumActiveDiscussions);
        }

        /// <summary>
        /// Resets the Board Categories Cache
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void BoardCategoriesCacheResetClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.RemoveCacheKey(Constants.Cache.ForumCategory);
        }

        /// <summary>
        /// Resets the Board Categories Cache
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void BoardModeratorsCacheResetClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.RemoveCacheKey(Constants.Cache.ForumModerators);
        }

        /// <summary>
        /// Resets the Board User Stats Cache
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void BoardUserStatsCacheResetClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.RemoveCacheKey(Constants.Cache.BoardUserStats);
        }

        /// <summary>
        /// Resets the the User Lazy Data Cache
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void UserLazyDataCacheResetClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            // vzrus: remove all users lazy data
            this.Get<IDataCache>().RemoveOf<object>(
                k => k.Key.StartsWith(string.Format(Constants.Cache.ActiveUserLazyData, string.Empty)));
            this.CheckCache();
        }

        /// <summary>
        /// Resets the Forum Statistics Cache
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ForumStatisticsCacheResetClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.RemoveCacheKey(Constants.Cache.BoardStats);
        }

        /// <summary>
        /// Resets the Complete Cache
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ResetCacheAllClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            // clear all cache keys
            this.Get<IObjectStore>().Clear();
            this.Get<IDataCache>().Clear();

            this.CheckCache();
        }

        /// <summary>
        /// Registers the needed Java Scripts
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            // setup jQuery and YAF JS...
            BoardContext.Current.PageElements.RegisterJsBlock(
                "yafTabsJs",
                JavaScriptBlocks.BootstrapTabLoadJs("v-pills-tab", this.hidLastTab.ClientID));

            base.OnPreRender(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.PageContext.IsHostAdmin)
            {
                BuildLink.AccessDenied();
            }

            if (!this.IsPostBack)
            {
                this.RenderListItems();

                this.BindData();
            }

            this.ServerTimeCorrection.MaxLength = 4;

            this.UserNameMaxLength.MaxLength = 5;

            this.UserNameMaxLength.MaxLength = 3;

            this.PictureAttachmentDisplayTreshold.MaxLength = 11;

            // CheckCache
            this.CheckCache();

            // Hide Some settings if yaf is inside dnn
            this.AvatarSettingsHolder.Visible = !Config.IsDotNetNuke;
            this.SSLSettings.Visible = !Config.IsDotNetNuke;
            this.BotRegisterCheck.Visible = !Config.IsDotNetNuke;
            this.LoginSettings.Visible = !Config.IsDotNetNuke;
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
            this.PageLinks.AddLink(this.GetText("ADMIN_HOSTSETTINGS", "TITLE"), string.Empty);

            this.Page.Header.Title =
                $"{this.GetText("ADMIN_ADMIN", "Administration")} - {this.GetText("ADMIN_HOSTSETTINGS", "TITLE")}";
        }

        /// <summary>
        /// Saves the Host Settings
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void SaveClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            // write all the settings back to the settings class

            // load Board Setting collection information...
            var settingCollection = new BoardSettingCollection(this.Get<BoardSettings>());

            // handle checked fields...
            settingCollection.SettingsBool.Keys.ForEach(
                name =>
                    {
                        var control = this.HostSettingsTabs.FindControlRecursive(name);

                        if (control is CheckBox box && settingCollection.SettingsBool[name].CanWrite)
                        {
                            settingCollection.SettingsBool[name].SetValue(
                                this.Get<BoardSettings>(),
                                box.Checked,
                                null);
                        }
                    });

            // handle string fields...
            settingCollection.SettingsString.Keys.ForEach(
                name =>
                    {
                        var control = this.HostSettingsTabs.FindControlRecursive(name);

                        switch (control)
                        {
                            case TextBox box when settingCollection.SettingsString[name].CanWrite:
                                settingCollection.SettingsString[name].SetValue(
                                    this.Get<BoardSettings>(),
                                    box.Text.Trim(),
                                    null);
                                break;
                            case DropDownList list when settingCollection.SettingsString[name].CanWrite:
                                settingCollection.SettingsString[name].SetValue(
                                    this.Get<BoardSettings>(),
                                    Convert.ToString(list.SelectedItem.Value),
                                    null);
                                break;
                        }
                    });

            // handle int fields...
            settingCollection.SettingsInt.Keys.ForEach(
                name =>
                    {
                        var control = this.HostSettingsTabs.FindControlRecursive(name);

                        switch (control)
                        {
                            case TextBox box when settingCollection.SettingsInt[name].CanWrite:
                                {
                                    var value = box.Text.Trim();
                                    int i;

                                    if (value.IsNotSet())
                                    {
                                        i = 0;
                                    }
                                    else
                                    {
                                        int.TryParse(value, out i);
                                    }

                                    settingCollection.SettingsInt[name].SetValue(this.Get<BoardSettings>(), i, null);
                                    break;
                                }

                            case DropDownList list when settingCollection.SettingsInt[name].CanWrite:
                                settingCollection.SettingsInt[name].SetValue(
                                    this.Get<BoardSettings>(),
                                    list.SelectedItem.Value.ToType<int>(),
                                    null);
                                break;
                        }
                    });

            // handle double fields...
            settingCollection.SettingsDouble.Keys.ForEach(
                name =>
                    {
                        var control = this.HostSettingsTabs.FindControlRecursive(name);

                        switch (control)
                        {
                            case TextBox box when settingCollection.SettingsDouble[name].CanWrite:
                                {
                                    var value = box.Text.Trim();
                                    double i;

                                    if (value.IsNotSet())
                                    {
                                        i = 0;
                                    }
                                    else
                                    {
                                        double.TryParse(value, out i);
                                    }

                                    settingCollection.SettingsDouble[name].SetValue(
                                        this.Get<BoardSettings>(),
                                        i,
                                        null);
                                    break;
                                }

                            case DropDownList list when settingCollection.SettingsDouble[name].CanWrite:
                                settingCollection.SettingsDouble[name].SetValue(
                                    this.Get<BoardSettings>(),
                                    Convert.ToDouble(list.SelectedItem.Value),
                                    null);
                                break;
                        }
                    });

            // save the settings to the database
            ((LoadBoardSettings)this.Get<BoardSettings>()).SaveRegistry();

            // reload all settings from the DB
            this.PageContext.BoardSettings = null;

            BuildLink.Redirect(ForumPages.Admin_Admin);
        }

        /// <summary>
        /// Fill Lists with Localized Items
        /// </summary>
        private void RenderListItems()
        {
            var localizations = new[] { "FORBIDDEN", "REG_USERS", "ALL_USERS" };

            var dropDownLists = new[]
                                    {
                                        this.PostsFeedAccess, this.AllowCreateTopicsSameName, this.PostLatestFeedAccess,
                                        this.ForumFeedAccess, this.TopicsFeedAccess, this.ActiveTopicFeedAccess,
                                        this.FavoriteTopicFeedAccess, this.ReportPostPermissions,
                                        this.ProfileViewPermissions, this.MembersListViewPermissions,
                                        this.ActiveUsersViewPermissions, this.SearchPermissions, this.ShowHelpTo,
                                        this.ShowTeamTo, this.ShowShareTopicTo
                                    };

            dropDownLists.ForEach(
                ddl => ddl.Items.AddRange(
                    localizations.Select((t, i) => new ListItem(this.GetText("ADMIN_HOSTSETTINGS", t), i.ToString()))
                        .ToArray()));

            this.CaptchaTypeRegister.Items.Add(new ListItem(this.GetText("ADMIN_COMMON", "DISABLED"), "0"));
            this.CaptchaTypeRegister.Items.Add(new ListItem("YafCaptcha", "1"));
            this.CaptchaTypeRegister.Items.Add(new ListItem("ReCaptcha", "2"));

            this.SpamServiceType.Items.Add(new ListItem(this.GetText("ADMIN_COMMON", "DISABLED"), "0"));
            this.SpamServiceType.Items.Add(new ListItem(this.GetText("ADMIN_HOSTSETTINGS", "SPAM_SERVICE_TYP_2"), "2"));
            this.SpamServiceType.Items.Add(new ListItem(this.GetText("ADMIN_HOSTSETTINGS", "SPAM_SERVICE_TYP_3"), "3"));

            this.BotSpamServiceType.Items.Add(new ListItem(this.GetText("ADMIN_COMMON", "DISABLED"), "0"));
            this.BotSpamServiceType.Items.Add(new ListItem(this.GetText("ADMIN_HOSTSETTINGS", "BOT_CHECK_1"), "1"));
            this.BotSpamServiceType.Items.Add(new ListItem(this.GetText("ADMIN_HOSTSETTINGS", "BOT_CHECK_2"), "2"));
            this.BotSpamServiceType.Items.Add(new ListItem(this.GetText("ADMIN_HOSTSETTINGS", "BOT_CHECK_3"), "3"));
            this.BotSpamServiceType.Items.Add(new ListItem(this.GetText("ADMIN_HOSTSETTINGS", "BOT_CHECK_4"), "4"));

            this.SpamMessageHandling.Items.Add(new ListItem(this.GetText("ADMIN_HOSTSETTINGS", "SPAM_MESSAGE_0"), "0"));
            this.SpamMessageHandling.Items.Add(new ListItem(this.GetText("ADMIN_HOSTSETTINGS", "SPAM_MESSAGE_1"), "1"));
            this.SpamMessageHandling.Items.Add(new ListItem(this.GetText("ADMIN_HOSTSETTINGS", "SPAM_MESSAGE_2"), "2"));
            this.SpamMessageHandling.Items.Add(new ListItem(this.GetText("ADMIN_HOSTSETTINGS", "SPAM_MESSAGE_3"), "3"));

            this.BotHandlingOnRegister.Items.Add(
                new ListItem(this.GetText("ADMIN_HOSTSETTINGS", "BOT_MESSAGE_0"), "0"));
            this.BotHandlingOnRegister.Items.Add(
                new ListItem(this.GetText("ADMIN_HOSTSETTINGS", "BOT_MESSAGE_1"), "1"));
            this.BotHandlingOnRegister.Items.Add(
                new ListItem(this.GetText("ADMIN_HOSTSETTINGS", "BOT_MESSAGE_2"), "2"));

            this.SendWelcomeNotificationAfterRegister.Items.Add(
                new ListItem(this.GetText("ADMIN_HOSTSETTINGS", "WELCOME_NOTIFICATION_0"), "0"));
            this.SendWelcomeNotificationAfterRegister.Items.Add(
                new ListItem(this.GetText("ADMIN_HOSTSETTINGS", "WELCOME_NOTIFICATION_1"), "1"));
            this.SendWelcomeNotificationAfterRegister.Items.Add(
                new ListItem(this.GetText("ADMIN_HOSTSETTINGS", "WELCOME_NOTIFICATION_2"), "2"));
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            this.DataBind();

            // load Board Setting collection information...
            var settingCollection = new BoardSettingCollection(this.Get<BoardSettings>());

            // handle checked fields...
            settingCollection.SettingsBool.Keys.ForEach(
                name =>
                    {
                        var control = this.HostSettingsTabs.FindControlRecursive(name);

                        if (control is CheckBox box && settingCollection.SettingsBool[name].CanRead)
                        {
                            // get the value from the property...
                            box.Checked = (bool)Convert.ChangeType(
                                settingCollection.SettingsBool[name].GetValue(this.Get<BoardSettings>(), null),
                                typeof(bool));
                        }
                    });

            // handle string fields...
            settingCollection.SettingsString.Keys.ForEach(
                name =>
                    {
                        var control = this.HostSettingsTabs.FindControlRecursive(name);

                        switch (control)
                        {
                            case TextBox box when settingCollection.SettingsString[name].CanRead:
                                // get the value from the property...
                                box.Text = (string)Convert.ChangeType(
                                    settingCollection.SettingsString[name].GetValue(this.Get<BoardSettings>(), null),
                                    typeof(string));
                                break;
                            case DropDownList list when settingCollection.SettingsString[name].CanRead:
                                {
                                    var listItem = list.Items.FindByValue(
                                        settingCollection.SettingsString[name].GetValue(
                                            this.Get<BoardSettings>(),
                                            null).ToString());

                                    if (listItem != null)
                                    {
                                        listItem.Selected = true;
                                    }

                                    break;
                                }
                        }
                    });

            // handle int fields...
            settingCollection.SettingsInt.Keys.ForEach(
                name =>
                    {
                        var control = this.HostSettingsTabs.FindControlRecursive(name);

                        switch (control)
                        {
                            case TextBox box when settingCollection.SettingsInt[name].CanRead:
                                {
                                    if (!name.Equals("ServerTimeCorrection"))
                                    {
                                        box.TextMode = TextBoxMode.Number;
                                    }

                                    // get the value from the property...
                                    box.Text = settingCollection.SettingsInt[name]
                                        .GetValue(this.Get<BoardSettings>(), null).ToString();
                                    break;
                                }

                            case DropDownList list when settingCollection.SettingsInt[name].CanRead:
                                {
                                    var listItem = list.Items.FindByValue(
                                        settingCollection.SettingsInt[name].GetValue(this.Get<BoardSettings>(), null)
                                            .ToString());

                                    if (listItem != null)
                                    {
                                        listItem.Selected = true;
                                    }

                                    break;
                                }
                        }
                    });

            // handle double fields...
            settingCollection.SettingsDouble.Keys.ForEach(
                name =>
                    {
                        var control = this.HostSettingsTabs.FindControlRecursive(name);

                        switch (control)
                        {
                            case TextBox box when settingCollection.SettingsDouble[name].CanRead:
                                box.CssClass = "form-control";

                                // get the value from the property...
                                box.Text = settingCollection.SettingsDouble[name]
                                    .GetValue(this.Get<BoardSettings>(), null).ToString();
                                break;

                            case DropDownList list when settingCollection.SettingsDouble[name].CanRead:
                                {
                                    var listItem = list.Items.FindByValue(
                                        settingCollection.SettingsDouble[name].GetValue(
                                            this.Get<BoardSettings>(),
                                            null).ToString());

                                    if (listItem != null)
                                    {
                                        listItem.Selected = true;
                                    }

                                    break;
                                }
                        }
                    });

            // special field handling...
            this.AvatarSize.Text = this.Get<BoardSettings>().AvatarSize != 0
                                       ? this.Get<BoardSettings>().AvatarSize.ToString()
                                       : string.Empty;
            this.MaxFileSize.Text = this.Get<BoardSettings>().MaxFileSize != 0
                                        ? this.Get<BoardSettings>().MaxFileSize.ToString()
                                        : string.Empty;

            this.AlbumImagesSizeMax.Text = this.Get<BoardSettings>().AlbumImagesSizeMax != 0
                                               ? this.Get<BoardSettings>().AlbumImagesSizeMax.ToString()
                                               : string.Empty;

            this.SQLVersion.Text = this.HtmlEncode(this.Get<IDbFunction>().GetSQLVersion());

            this.AppCores.Text = SystemInfo.Processors;
            this.AppMemory.Text =
                $"{SystemInfo.AllocatedMemory.ToType<long>() / 1000000} MB of {SystemInfo.MappedMemory.ToType<long>() / 1000000} MB";
            this.AppOSName.Text = SystemInfo.VersionString;
            this.AppRuntime.Text = $"{SystemInfo.RuntimeName} {SystemInfo.RuntimeString}";
        }

        /// <summary>
        /// Checks the cache.
        /// </summary>
        private void CheckCache()
        {
            this.ForumStatisticsCacheReset.Enabled = this.CheckCacheKey(Constants.Cache.BoardStats);
            this.BoardUserStatsCacheReset.Enabled = this.CheckCacheKey(Constants.Cache.BoardUserStats);
            this.ActiveDiscussionsCacheReset.Enabled = this.CheckCacheKey(Constants.Cache.ActiveDiscussions)
                                                       || this.CheckCacheKey(Constants.Cache.ForumActiveDiscussions);
            this.BoardModeratorsCacheReset.Enabled = this.CheckCacheKey(Constants.Cache.ForumModerators);
            this.BoardCategoriesCacheReset.Enabled = this.CheckCacheKey(Constants.Cache.ForumCategory);
            this.ActiveUserLazyDataCacheReset.Enabled = this.CheckCacheKey(Constants.Cache.ActiveUserLazyData);
            this.ResetCacheAll.Enabled = this.Get<IDataCache>().Count() > 0;
        }

        /// <summary>
        /// Checks the cache key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// The check cache key.
        /// </returns>
        private bool CheckCacheKey([NotNull] string key)
        {
            return this.Get<IDataCache>()[key] != null;
        }

        /// <summary>
        /// Removes the cache key.
        /// </summary>
        /// <param name="key">The key.</param>
        private void RemoveCacheKey([NotNull] string key)
        {
            this.Get<IDataCache>().Remove(key);
            this.CheckCache();
        }

        #endregion
    }
}