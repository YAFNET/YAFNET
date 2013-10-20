/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utilities;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The Host Settings Page.
    /// </summary>
    public partial class hostsettings : AdminPage
    {
        #region Methods

        /// <summary>
        /// Resets the Active Discussions Cache
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ActiveDiscussionsCacheReset_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.RemoveCacheKey(Constants.Cache.ActiveDiscussions);
            this.RemoveCacheKey(Constants.Cache.ForumActiveDiscussions);
        }

        /// <summary>
        /// Resets the Board Categories Cache
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void BoardCategoriesCacheReset_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.RemoveCacheKey(Constants.Cache.ForumCategory);
        }

        /// <summary>
        /// Resets the Board Categories Cache
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void BoardModeratorsCacheReset_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.RemoveCacheKey(Constants.Cache.ForumModerators);
        }

        /// <summary>
        /// Resets the Board User Stats Cache
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void BoardUserStatsCacheReset_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.RemoveCacheKey(Constants.Cache.BoardUserStats);
        }

        /// <summary>
        /// Resets the the User Lazy Data Cache
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void UserLazyDataCacheReset_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // vzrus: remove all users lazy data 
            this.Get<IDataCache>().RemoveOf<object>(
                k => k.Key.StartsWith(Constants.Cache.ActiveUserLazyData.FormatWith(string.Empty)));
            this.CheckCache();
        }

        /// <summary>
        /// Resets the Replace Rules Cache
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ReplaceRulesCacheReset_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.Get<IObjectStore>().RemoveOf<IProcessReplaceRules>();
            this.CheckCache();
        }

        /// <summary>
        /// Resets the Forum Statistics Cache
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ForumStatisticsCacheReset_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.RemoveCacheKey(Constants.Cache.BoardStats);
        }

        /// <summary>
        /// Resets the Complete Cache
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ResetCacheAll_Click([NotNull] object sender, [NotNull] EventArgs e)
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
            YafContext.Current.PageElements.RegisterJQuery();
            YafContext.Current.PageElements.RegisterJQueryUI();

            YafContext.Current.PageElements.RegisterJsBlock(
                "yafTabsJs",
                JavaScriptBlocks.JqueryUITabsLoadJs(this.HostSettingsTabs.ClientID, this.hidLastTab.ClientID, false));

            YafContext.Current.PageElements.RegisterJsBlock("spinnerJs", JavaScriptBlocks.LoadSpinnerWidget());

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
                YafBuildLink.AccessDenied();
            }

            if (!this.IsPostBack)
            {
                this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));
                this.PageLinks.AddLink(
                    this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));
                this.PageLinks.AddLink(this.GetText("ADMIN_HOSTSETTINGS", "TITLE"), string.Empty);

                this.Page.Header.Title = "{0} - {1}".FormatWith(
                    this.GetText("ADMIN_ADMIN", "Administration"), this.GetText("ADMIN_HOSTSETTINGS", "TITLE"));

                this.RenderListItems();

                this.BindData();
            }

            var txtBoxes =
                this.ControlListRecursive(
                    c => (c.GetType() == typeof(TextBox) && ((TextBox)c).TextMode == TextBoxMode.SingleLine)).Cast<TextBox>().ToList();

            // default to 100% width...
            txtBoxes.ForEach(x => x.Width = Unit.Percentage(100));

            // vzrus : 13/5/2010
            this.ServerTimeCorrection.AddStyleAttributeWidth("50px");
            this.ServerTimeCorrection.AddAttributeMaxWidth("4");

            this.ImageAttachmentResizeHeight.AddStyleAttributeWidth("50px");

            this.MaxPostSize.AddStyleAttributeWidth("50px");
            this.UserNameMaxLength.AddAttributeMaxWidth("5");

            this.UserNameMaxLength.AddStyleAttributeWidth("50px");
            this.UserNameMaxLength.AddAttributeMaxWidth("3");

            this.ActiveListTime.AddStyleAttributeWidth("50px");

            this.PictureAttachmentDisplayTreshold.AddStyleAttributeWidth("100px");
            this.PictureAttachmentDisplayTreshold.AddAttributeMaxWidth("11");

            this.ImageAttachmentResizeWidth.AddStyleAttributeWidth("50px");
            this.DisableNoFollowLinksAfterDay.AddStyleAttributeWidth("100px");

            // Ederon : 7/14/2007
            this.UserBox.AddStyleAttributeSize("350px", "100px");

            // CheckCache
            this.CheckCache();
        }

        /// <summary>
        /// Saves the Host Settings
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Save_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // write all the settings back to the settings class

            // load Board Setting collection information...
            var settingCollection = new YafBoardSettingCollection(this.Get<YafBoardSettings>());

            // handle checked fields...
            foreach (string name in settingCollection.SettingsBool.Keys)
            {
                Control control = this.HostSettingsTabs.FindControlRecursive(name);

                if (control is CheckBox && settingCollection.SettingsBool[name].CanWrite)
                {
                    settingCollection.SettingsBool[name].SetValue(
                        this.Get<YafBoardSettings>(), ((CheckBox)control).Checked, null);
                }
            }

            // handle string fields...
            foreach (string name in settingCollection.SettingsString.Keys)
            {
                Control control = this.HostSettingsTabs.FindControlRecursive(name);

                if (control is TextBox && settingCollection.SettingsString[name].CanWrite)
                {
                    settingCollection.SettingsString[name].SetValue(
                        this.Get<YafBoardSettings>(), ((TextBox)control).Text.Trim(), null);
                }
                else if (control is DropDownList && settingCollection.SettingsString[name].CanWrite)
                {
                    settingCollection.SettingsString[name].SetValue(
                        this.Get<YafBoardSettings>(),
                        Convert.ToString(((DropDownList)control).SelectedItem.Value),
                        null);
                }
            }

            // handle int fields...
            foreach (string name in settingCollection.SettingsInt.Keys)
            {
                Control control = this.HostSettingsTabs.FindControlRecursive(name);

                if (control is TextBox && settingCollection.SettingsInt[name].CanWrite)
                {
                    string value = ((TextBox)control).Text.Trim();
                    int i;

                    if (value.IsNotSet())
                    {
                        i = 0;
                    }
                    else
                    {
                        int.TryParse(value, out i);
                    }

                    settingCollection.SettingsInt[name].SetValue(this.Get<YafBoardSettings>(), i, null);
                }
                else if (control is DropDownList && settingCollection.SettingsInt[name].CanWrite)
                {
                    settingCollection.SettingsInt[name].SetValue(
                        this.Get<YafBoardSettings>(), ((DropDownList)control).SelectedItem.Value.ToType<int>(), null);
                }
            }

            // handle double fields...
            foreach (string name in settingCollection.SettingsDouble.Keys)
            {
                Control control = this.HostSettingsTabs.FindControlRecursive(name);

                if (control is TextBox && settingCollection.SettingsDouble[name].CanWrite)
                {
                    string value = ((TextBox)control).Text.Trim();
                    double i;

                    if (value.IsNotSet())
                    {
                        i = 0;
                    }
                    else
                    {
                        double.TryParse(value, out i);
                    }

                    settingCollection.SettingsDouble[name].SetValue(this.Get<YafBoardSettings>(), i, null);
                }
                else if (control is DropDownList && settingCollection.SettingsDouble[name].CanWrite)
                {
                    settingCollection.SettingsDouble[name].SetValue(
                        this.Get<YafBoardSettings>(),
                        Convert.ToDouble(((DropDownList)control).SelectedItem.Value),
                        null);
                }
            }

            // save the settings to the database
            ((YafLoadBoardSettings)this.Get<YafBoardSettings>()).SaveRegistry();

            // reload all settings from the DB
            this.PageContext.BoardSettings = null;

            YafBuildLink.Redirect(ForumPages.admin_admin);
        }

        /// <summary>
        /// Fill Lists with Localized Items
        /// </summary>
        private void RenderListItems()
        {
            var localizations = new[] { "FORBIDDEN", "REG_USERS", "ALL_USERS" };

            var dropDownLists = new[]
                {
                    this.PostsFeedAccess, this.AllowCreateTopicsSameName, this.PostLatestFeedAccess, this.ForumFeedAccess, this.TopicsFeedAccess,
                    this.ActiveTopicFeedAccess, this.FavoriteTopicFeedAccess, this.ReportPostPermissions, this.ProfileViewPermissions,
                    this.MembersListViewPermissions, this.ActiveUsersViewPermissions, this.ExternalSearchPermissions, this.SearchPermissions,
                    this.ShowHelpTo, this.ShowTeamTo, this.ShowRetweetMessageTo, this.ShowShareTopicTo, this.ShoutboxViewPermissions
                };

            foreach (var ddl in dropDownLists)
            {
                ddl.Items.AddRange(
                    localizations.Select((t, i) => new ListItem(this.GetText("ADMIN_HOSTSETTINGS", t), i.ToString())).ToArray());
            }

            this.CaptchaTypeRegister.Items.Add(new ListItem(this.GetText("ADMIN_COMMON", "DISABLED"), "0"));
            this.CaptchaTypeRegister.Items.Add(new ListItem("YafCaptcha", "1"));
            this.CaptchaTypeRegister.Items.Add(new ListItem("ReCaptcha", "2"));

            this.SpamServiceType.Items.Add(new ListItem(this.GetText("ADMIN_COMMON", "DISABLED"), "0"));
            this.SpamServiceType.Items.Add(new ListItem("BlogSpam.NET API", "1"));
            this.SpamServiceType.Items.Add(new ListItem("Akismet API (Needs Registration)", "2"));

            this.BotSpamServiceType.Items.Add(new ListItem(this.GetText("ADMIN_COMMON", "DISABLED"), "0"));
            this.BotSpamServiceType.Items.Add(new ListItem(this.GetText("ADMIN_HOSTSETTINGS", "BOT_CHECK_1"), "1"));
            this.BotSpamServiceType.Items.Add(new ListItem(this.GetText("ADMIN_HOSTSETTINGS", "BOT_CHECK_2"), "2"));
            this.BotSpamServiceType.Items.Add(new ListItem(this.GetText("ADMIN_HOSTSETTINGS", "BOT_CHECK_3"), "3"));
            this.BotSpamServiceType.Items.Add(new ListItem(this.GetText("ADMIN_HOSTSETTINGS", "BOT_CHECK_4"), "4"));

            this.SpamMessageHandling.Items.Add(new ListItem(this.GetText("ADMIN_HOSTSETTINGS", "SPAM_MESSAGE_0"), "0"));
            this.SpamMessageHandling.Items.Add(new ListItem(this.GetText("ADMIN_HOSTSETTINGS", "SPAM_MESSAGE_1"), "1"));
            this.SpamMessageHandling.Items.Add(new ListItem(this.GetText("ADMIN_HOSTSETTINGS", "SPAM_MESSAGE_2"), "2"));

            this.BotHandlingOnRegister.Items.Add(new ListItem(this.GetText("ADMIN_HOSTSETTINGS", "BOT_MESSAGE_0"), "0"));
            this.BotHandlingOnRegister.Items.Add(new ListItem(this.GetText("ADMIN_HOSTSETTINGS", "BOT_MESSAGE_1"), "1"));
            this.BotHandlingOnRegister.Items.Add(new ListItem(this.GetText("ADMIN_HOSTSETTINGS", "BOT_MESSAGE_2"), "2"));

            this.MessageNotificationSystem.Items.Add(new ListItem(this.GetText("ADMIN_HOSTSETTINGS", "MODAL_DIALOG"), "0"));
            this.MessageNotificationSystem.Items.Add(new ListItem(this.GetText("ADMIN_HOSTSETTINGS", "NOTIFICATION_BAR"), "1"));

            this.ShoutboxDefaultState.Items.Add(new ListItem(this.GetText("ADMIN_HOSTSETTINGS", "EXPANDED"), "0")); 
            this.ShoutboxDefaultState.Items.Add(new ListItem(this.GetText("ADMIN_HOSTSETTINGS", "COLLAPSED"), "1"));
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            this.ForumEditor.DataSource = this.Get<IModuleManager<ForumEditor>>().ActiveAsDataTable("Editors");

            // TODO: vzrus: UseFullTextSearch check box is data layer specific and can be hidden by YAF.Classes.Data.LegacyDb.FullTextSupported  property.)
            this.DataBind();

            // load Board Setting collection information...
            var settingCollection = new YafBoardSettingCollection(this.Get<YafBoardSettings>());

            // handle checked fields...
            foreach (string name in settingCollection.SettingsBool.Keys)
            {
                Control control = this.HostSettingsTabs.FindControlRecursive(name);

                if (control is CheckBox && settingCollection.SettingsBool[name].CanRead)
                {
                    // get the value from the property...
                    ((CheckBox)control).Checked =
                        (bool)
                        Convert.ChangeType(
                            settingCollection.SettingsBool[name].GetValue(this.Get<YafBoardSettings>(), null),
                            typeof(bool));
                }
            }

            // handle string fields...
            foreach (string name in settingCollection.SettingsString.Keys)
            {
                Control control = this.HostSettingsTabs.FindControlRecursive(name);

                if (control is TextBox && settingCollection.SettingsString[name].CanRead)
                {
                    // get the value from the property...
                    ((TextBox)control).Text =
                        (string)
                        Convert.ChangeType(
                            settingCollection.SettingsString[name].GetValue(this.Get<YafBoardSettings>(), null),
                            typeof(string));
                }
                else if (control is DropDownList && settingCollection.SettingsString[name].CanRead)
                {
                    ListItem listItem =
                        ((DropDownList)control).Items.FindByValue(
                            settingCollection.SettingsString[name].GetValue(this.Get<YafBoardSettings>(), null).ToString());

                    if (listItem != null)
                    {
                        listItem.Selected = true;
                    }
                }
            }

            // handle int fields...
            foreach (string name in settingCollection.SettingsInt.Keys)
            {
                Control control = this.HostSettingsTabs.FindControlRecursive(name);

                if (control is TextBox && settingCollection.SettingsInt[name].CanRead)
                {
                    ((TextBox)control).CssClass = "Numeric";

                    // get the value from the property...
                    ((TextBox)control).Text =
                        settingCollection.SettingsInt[name].GetValue(this.Get<YafBoardSettings>(), null).ToString();
                }
                else if (control is DropDownList && settingCollection.SettingsInt[name].CanRead)
                {
                    ListItem listItem =
                        ((DropDownList)control).Items.FindByValue(
                            settingCollection.SettingsInt[name].GetValue(this.Get<YafBoardSettings>(), null).ToString());

                    if (listItem != null)
                    {
                        listItem.Selected = true;
                    }
                }
            }

            // handle double fields...
            foreach (string name in settingCollection.SettingsDouble.Keys)
            {
                Control control = this.HostSettingsTabs.FindControlRecursive(name);

                if (control is TextBox && settingCollection.SettingsDouble[name].CanRead)
                {
                    ((TextBox)control).CssClass = "Numeric";

                    // get the value from the property...
                    ((TextBox)control).Text =
                        settingCollection.SettingsDouble[name].GetValue(this.Get<YafBoardSettings>(), null).ToString();
                }
                else if (control is DropDownList && settingCollection.SettingsDouble[name].CanRead)
                {
                    ListItem listItem =
                        ((DropDownList)control).Items.FindByValue(
                            settingCollection.SettingsDouble[name].GetValue(this.Get<YafBoardSettings>(), null).ToString());

                    if (listItem != null)
                    {
                        listItem.Selected = true;
                    }
                }
            }

            // special field handling...
            this.AvatarSize.Text = (this.Get<YafBoardSettings>().AvatarSize != 0)
                                       ? this.Get<YafBoardSettings>().AvatarSize.ToString()
                                       : string.Empty;
            this.MaxFileSize.Text = (this.Get<YafBoardSettings>().MaxFileSize != 0)
                                        ? this.Get<YafBoardSettings>().MaxFileSize.ToString()
                                        : string.Empty;

            this.SQLVersion.Text = this.HtmlEncode(this.Get<YafBoardSettings>().SQLVersion);

            if (General.GetCurrentTrustLevel() <= AspNetHostingPermissionLevel.Medium)
            {
                return;
            }

            this.AppCores.Text = Platform.Processors;
            this.AppMemory.Text = "{0} MB of {1} MB".FormatWith(
                Platform.AllocatedMemory.ToType<int>() / 1000000, Platform.MappedMemory.ToType<int>() / 1000000);
            this.AppOSName.Text = Platform.VersionString;
            this.AppRuntime.Text = "{0} {1}".FormatWith(Platform.RuntimeName, Platform.RuntimeString);
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