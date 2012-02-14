/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
 * Copyright (C) 2006-2012 Jaben Cargman
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
  using System.Collections.Generic;
  using System.Data;
  using System.Linq;
  using System.Web.UI;
  using System.Web.UI.WebControls;

  using YAF.Classes;
  using YAF.Core;
  using YAF.Core.BBCode;
  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Attributes;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using YAF.Utilities;
  using YAF.Utils;
  using YAF.Utils.Helpers;

  #endregion

  /// <summary>
  /// Summary description for settings.
  /// </summary>
  public partial class hostsettings : AdminPage
  {
    #region Methods

    /// <summary>
    /// The active discussions cache reset_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ActiveDiscussionsCacheReset_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.RemoveCacheKey(Constants.Cache.ActiveDiscussions);
      this.RemoveCacheKey(Constants.Cache.ForumActiveDiscussions);
    }

    /// <summary>
    /// The board categories cache reset_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void BoardCategoriesCacheReset_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.RemoveCacheKey(Constants.Cache.ForumCategory);
    }

    /// <summary>
    /// The board moderators cache reset_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void BoardModeratorsCacheReset_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.RemoveCacheKey(Constants.Cache.ForumModerators);
    }

    /// <summary>
    /// The board user statistics cache reset_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void BoardUserStatsCacheReset_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.RemoveCacheKey(Constants.Cache.BoardUserStats);
    }

    /// <summary>
    /// The forum statistics cache reset_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ForumStatisticsCacheReset_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.RemoveCacheKey(Constants.Cache.BoardStats);
    }

    /// <summary>
    /// The On PreRender event.
    /// </summary>
    /// <param name="e">
    /// the Event Arguments
    /// </param>
    protected override void OnPreRender([NotNull] EventArgs e)
    {
      // setup jQuery and YAF JS...
      YafContext.Current.PageElements.RegisterJQuery();
      YafContext.Current.PageElements.RegisterJQueryUI();

      YafContext.Current.PageElements.RegisterJsBlock(
        "yafTabsJs", 
        JavaScriptBlocks.JqueryUITabsLoadJs(this.HostSettingsTabs.ClientID, this.hidLastTab.ClientID, false));
    
      base.OnPreRender(e);
    }

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (!this.PageContext.IsHostAdmin)
      {
        YafBuildLink.AccessDenied();
      }

      if (!this.IsPostBack)
      {
        this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));
        this.PageLinks.AddLink(this.GetText("ADMIN_HOSTSETTINGS", "TITLE"), string.Empty);

        this.Page.Header.Title = "{0} - {1}".FormatWith(
       this.GetText("ADMIN_ADMIN", "Administration"),
       this.GetText("ADMIN_HOSTSETTINGS", "TITLE"));

        this.RenderListItems();

        this.BindData();
      }

      var txtBoxes =
        this.ControlListRecursive(
          c => (c.GetType() == typeof(TextBox) && ((TextBox)c).TextMode == TextBoxMode.SingleLine)).Cast<TextBox>().
          ToList();

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
      /// Fill Lists with Localized Items
      /// </summary>
      private void RenderListItems()
      {
        var localizations = new[] { "FORBIDDEN", "REG_USERS", "ALL_USERS" };

        var dropDownLists = new[]
          {
            PostsFeedAccess,
            AllowCreateTopicsSameName,
            PostLatestFeedAccess,
            ForumFeedAccess,
            TopicsFeedAccess,
            ActiveTopicFeedAccess,
            FavoriteTopicFeedAccess,
            ReportPostPermissions,
            ProfileViewPermissions,
            MembersListViewPermissions,
            ActiveUsersViewPermissions,
            ExternalSearchPermissions,
            SearchPermissions,
            ShowHelpTo,
            ShowTeamTo,
            ShowRetweetMessageTo,
            ShowShareTopicTo
          };

        foreach (var ddl in dropDownLists)
        {
          ddl.Items.AddRange(localizations.Select((t, i) => new ListItem(this.GetText("ADMIN_HOSTSETTINGS", t), i.ToString())).ToArray());
        }

        CaptchaTypeRegister.Items.Add(new ListItem(this.GetText("ADMIN_COMMON", "DISABLED"), "0"));
          CaptchaTypeRegister.Items.Add(new ListItem("YafCaptcha", "1"));
          CaptchaTypeRegister.Items.Add(new ListItem("ReCaptcha", "2"));

          SpamServiceType.Items.Add(new ListItem(this.GetText("ADMIN_COMMON", "DISABLED"), "0"));
          SpamServiceType.Items.Add(new ListItem("BlogSpam.NET API", "1"));
          SpamServiceType.Items.Add(new ListItem("Akismet API (Needs Registration)", "2"));

          SpamMessageHandling.Items.Add(new ListItem(this.GetText("ADMIN_HOSTSETTINGS", "SPAM_MESSAGE_0"), "0"));
          SpamMessageHandling.Items.Add(new ListItem(this.GetText("ADMIN_HOSTSETTINGS", "SPAM_MESSAGE_1"), "1"));
          SpamMessageHandling.Items.Add(new ListItem(this.GetText("ADMIN_HOSTSETTINGS", "SPAM_MESSAGE_2"), "2"));
      }

      /// <summary>
    /// The replace rules cache reset_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ReplaceRulesCacheReset_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.Get<IObjectStore>().RemoveOf<IProcessReplaceRules>();
      this.CheckCache();
    }

    /// <summary>
    /// The reset cache all_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ResetCacheAll_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      // clear all cache keys
      this.Get<IObjectStore>().Clear();
      this.Get<IDataCache>().Clear();

      this.CheckCache();
    }

    /// <summary>
    /// The save_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Save_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      // write all the settings back to the settings class

      // load Board Setting collection information...
      var settingCollection = new YafBoardSettingCollection(this.PageContext.BoardSettings);

      // handle checked fields...
      foreach (string name in settingCollection.SettingsBool.Keys)
      {
        Control control = this.HostSettingsTabs.FindControlRecursive(name);

        if (control != null && control is CheckBox && settingCollection.SettingsBool[name].CanWrite)
        {
          settingCollection.SettingsBool[name].SetValue(
            this.PageContext.BoardSettings, ((CheckBox)control).Checked, null);
        }
      }

      // handle string fields...
      foreach (string name in settingCollection.SettingsString.Keys)
      {
        Control control = this.HostSettingsTabs.FindControlRecursive(name);

        if (control != null && control is TextBox && settingCollection.SettingsString[name].CanWrite)
        {
          settingCollection.SettingsString[name].SetValue(
            this.PageContext.BoardSettings, ((TextBox)control).Text.Trim(), null);
        }
        else if (control != null && control is DropDownList && settingCollection.SettingsString[name].CanWrite)
        {
          settingCollection.SettingsString[name].SetValue(
            this.PageContext.BoardSettings, Convert.ToString(((DropDownList)control).SelectedItem.Value), null);
        }
      }

      // handle int fields...
      foreach (string name in settingCollection.SettingsInt.Keys)
      {
        Control control = this.HostSettingsTabs.FindControlRecursive(name);

        if (control != null && control is TextBox && settingCollection.SettingsInt[name].CanWrite)
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

          settingCollection.SettingsInt[name].SetValue(this.PageContext.BoardSettings, i, null);
        }
        else if (control != null && control is DropDownList && settingCollection.SettingsInt[name].CanWrite)
        {
          settingCollection.SettingsInt[name].SetValue(
            this.PageContext.BoardSettings, ((DropDownList)control).SelectedItem.Value.ToType<int>(), null);
        }
      }

      // handle double fields...
      foreach (string name in settingCollection.SettingsDouble.Keys)
      {
        Control control = this.HostSettingsTabs.FindControlRecursive(name);

        if (control != null && control is TextBox && settingCollection.SettingsDouble[name].CanWrite)
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

          settingCollection.SettingsDouble[name].SetValue(this.PageContext.BoardSettings, i, null);
        }
        else if (control != null && control is DropDownList && settingCollection.SettingsDouble[name].CanWrite)
        {
          settingCollection.SettingsDouble[name].SetValue(
            this.PageContext.BoardSettings, Convert.ToDouble(((DropDownList)control).SelectedItem.Value), null);
        }
      }

      // save the settings to the database
      ((YafLoadBoardSettings)this.PageContext.BoardSettings).SaveRegistry();

      // reload all settings from the DB
      this.PageContext.BoardSettings = null;

      YafBuildLink.Redirect(ForumPages.admin_admin);
    }

    /// <summary>
    /// The user lazy data cache reset_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void UserLazyDataCacheReset_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      // vzrus: remove all users lazy data 
      this.Get<IDataCache>().RemoveOf<object>(k => k.Key.StartsWith(Constants.Cache.ActiveUserLazyData.FormatWith(String.Empty)));
      this.CheckCache();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.ForumEditor.DataSource = this.Get<IModuleManager<ForumEditor>>().ActiveAsDataTable("Editors");

      // TODO: vzrus: UseFullTextSearch check box is data layer specific and can be hidden by YAF.Classes.Data.LegacyDb.FullTextSupported  property.)
      this.DataBind();

      // load Board Setting collection information...
      var settingCollection = new YafBoardSettingCollection(this.PageContext.BoardSettings);

      // handle checked fields...
      foreach (string name in settingCollection.SettingsBool.Keys)
      {
        Control control = this.HostSettingsTabs.FindControlRecursive(name);

        if (control != null && control is CheckBox && settingCollection.SettingsBool[name].CanRead)
        {
          // get the value from the property...
          ((CheckBox)control).Checked =
            (bool)
            Convert.ChangeType(
              settingCollection.SettingsBool[name].GetValue(this.PageContext.BoardSettings, null), typeof(bool));
        }
      }

      // handle string fields...
      foreach (string name in settingCollection.SettingsString.Keys)
      {
        Control control = this.HostSettingsTabs.FindControlRecursive(name);

        if (control != null && control is TextBox && settingCollection.SettingsString[name].CanRead)
        {
          // get the value from the property...
          ((TextBox)control).Text =
            (string)
            Convert.ChangeType(
              settingCollection.SettingsString[name].GetValue(this.PageContext.BoardSettings, null), typeof(string));
        }
        else if (control != null && control is DropDownList && settingCollection.SettingsString[name].CanRead)
        {
          ListItem listItem =
            ((DropDownList)control).Items.FindByValue(
              settingCollection.SettingsString[name].GetValue(this.PageContext.BoardSettings, null).ToString());

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

        if (control != null && control is TextBox && settingCollection.SettingsInt[name].CanRead)
        {
          // get the value from the property...
          ((TextBox)control).Text =
            settingCollection.SettingsInt[name].GetValue(this.PageContext.BoardSettings, null).ToString();
        }
        else if (control != null && control is DropDownList && settingCollection.SettingsInt[name].CanRead)
        {
          ListItem listItem =
            ((DropDownList)control).Items.FindByValue(
              settingCollection.SettingsInt[name].GetValue(this.PageContext.BoardSettings, null).ToString());

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

        if (control != null && control is TextBox && settingCollection.SettingsDouble[name].CanRead)
        {
          // get the value from the property...
          ((TextBox)control).Text =
            settingCollection.SettingsDouble[name].GetValue(this.PageContext.BoardSettings, null).ToString();
        }
        else if (control != null && control is DropDownList && settingCollection.SettingsDouble[name].CanRead)
        {
          ListItem listItem =
            ((DropDownList)control).Items.FindByValue(
              settingCollection.SettingsDouble[name].GetValue(this.PageContext.BoardSettings, null).ToString());

          if (listItem != null)
          {
            listItem.Selected = true;
          }
        }
      }

      // special field handling...
      this.AvatarSize.Text = (this.PageContext.BoardSettings.AvatarSize != 0)
                               ? this.PageContext.BoardSettings.AvatarSize.ToString()
                               : string.Empty;
      this.MaxFileSize.Text = (this.PageContext.BoardSettings.MaxFileSize != 0)
                                ? this.PageContext.BoardSettings.MaxFileSize.ToString()
                                : string.Empty;

      this.SQLVersion.Text = this.HtmlEncode(this.PageContext.BoardSettings.SQLVersion);
    }

    /// <summary>
    /// The check cache.
    /// </summary>
    private void CheckCache()
    {
      this.ForumStatisticsCacheReset.Enabled = this.CheckCacheKey(Constants.Cache.BoardStats);
      this.BoardUserStatsCacheReset.Enabled = this.CheckCacheKey(Constants.Cache.BoardUserStats);
      this.ActiveDiscussionsCacheReset.Enabled = this.CheckCacheKey(Constants.Cache.ActiveDiscussions) ||
                                                 this.CheckCacheKey(Constants.Cache.ForumActiveDiscussions);
      this.BoardModeratorsCacheReset.Enabled = this.CheckCacheKey(Constants.Cache.ForumModerators);
      this.BoardCategoriesCacheReset.Enabled = this.CheckCacheKey(Constants.Cache.ForumCategory);
      this.ActiveUserLazyDataCacheReset.Enabled = this.CheckCacheKey(Constants.Cache.ActiveUserLazyData);
      this.ResetCacheAll.Enabled = this.Get<IDataCache>().Count() > 0;
    }

    /// <summary>
    /// The check cache key.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <returns>
    /// The check cache key.
    /// </returns>
    private bool CheckCacheKey([NotNull] string key)
    {
      return this.Get<IDataCache>()[key] != null;
    }

    /// <summary>
    /// The remove cache key.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    private void RemoveCacheKey([NotNull] string key)
    {
      this.Get<IDataCache>().Remove(key);
      this.CheckCache();
    }

    #endregion
  }
}