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
  using System.Data;
  using System.Linq;
  using System.Web.UI.WebControls;

  using YAF.Classes;
  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using YAF.Types.Objects;
  using YAF.Utils;
  using YAF.Utils.Helpers;

  #endregion

  /// <summary>
  /// The Board Settings Admin Page.
  /// </summary>
  public partial class boardsettings : AdminPage
  {
    #region Methods

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
        if (this.IsPostBack)
        {
            return;
        }

        this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));
        this.PageLinks.AddLink(this.GetText("ADMIN_BOARDSETTINGS", "TITLE"), string.Empty);

        this.Page.Header.Title = "{0} - {1}".FormatWith(
              this.GetText("ADMIN_ADMIN", "Administration"),
              this.GetText("ADMIN_BOARDSETTINGS", "TITLE"));

        this.Save.Text = this.GetText("COMMON", "SAVE");

        // create list boxes by populating datasources from Data class
        var themeData = StaticDataHelper.Themes().AsEnumerable().Where(x => !x.Field<bool>("IsMobile"));

        if (themeData.Any())
        {
            this.Theme.DataSource = themeData.CopyToDataTable();
            this.Theme.DataTextField = "Theme";
            this.Theme.DataValueField = "FileName";
        }

        var mobileThemeData = StaticDataHelper.Themes().AsEnumerable().Where(x => x.Field<bool>("IsMobile"));

        if (mobileThemeData.Any())
        {
            var mobileThemes = mobileThemeData.CopyToDataTable();

            // Add Dummy Disabled Mobile Theme Item to allow disabling the Mobile Theme
            DataRow dr = mobileThemes.NewRow();
            dr["Theme"] = "[ {0} ]".FormatWith(this.GetText("ADMIN_COMMON", "DISABLED"));

            dr["FileName"] = string.Empty;
            dr["IsMobile"] = false;

            mobileThemes.Rows.InsertAt(dr, 0);

            this.MobileTheme.DataSource = mobileThemes;
            this.MobileTheme.DataTextField = "Theme";
            this.MobileTheme.DataValueField = "FileName";
        }

        this.Culture.DataSource =
            StaticDataHelper.Cultures().AsEnumerable().OrderBy(x => x.Field<string>("CultureNativeName")).CopyToDataTable();

        this.Culture.DataTextField = "CultureNativeName";
        this.Culture.DataValueField = "CultureTag";

        this.ShowTopic.DataSource = StaticDataHelper.TopicTimes();
        this.ShowTopic.DataTextField = "TopicText";
        this.ShowTopic.DataValueField = "TopicValue";

        this.FileExtensionAllow.DataSource = StaticDataHelper.AllowDisallow();
        this.FileExtensionAllow.DataTextField = "Text";
        this.FileExtensionAllow.DataValueField = "Value";

        this.JqueryUITheme.DataSource = StaticDataHelper.JqueryUIThemes();
        this.JqueryUITheme.DataTextField = "Theme";
        this.JqueryUITheme.DataValueField = "Theme";

        this.BindData();

        // bind poll group list
        var pollGroup =
            LegacyDb.PollGroupList(this.PageContext.PageUserID, null, this.PageContext.PageBoardID).Distinct(
                new AreEqualFunc<TypedPollGroup>((v1, v2) => v1.PollGroupID == v2.PollGroupID)).ToList();

        pollGroup.Insert(0, new TypedPollGroup(string.Empty, -1));

        // TODO: vzrus needs some work, will be in polls only until feature is debugged there.
        this.PollGroupListDropDown.Items.AddRange(
            pollGroup.Select(x => new ListItem(x.Question, x.PollGroupID.ToString())).ToArray());

        // population default notification setting options...
        var items = EnumHelper.EnumToDictionary<UserNotificationSetting>();

        if (!this.Get<YafBoardSettings>().AllowNotificationAllPostsAllTopics)
        {
            // remove it...
            items.Remove(UserNotificationSetting.AllTopics.ToInt());
        }

        var notificationItems =
            items.Select(
                x =>
                new ListItem(
                    HtmlHelper.StripHtml(this.GetText("CP_SUBSCRIPTIONS", x.Value)), x.Key.ToString()))
                .ToArray();

        this.DefaultNotificationSetting.Items.AddRange(notificationItems);

        SetSelectedOnList(ref this.JqueryUITheme, this.Get<YafBoardSettings>().JqueryUITheme);

        // Get first default full culture from a language file tag.
        string langFileCulture = StaticDataHelper.CultureDefaultFromFile(this.Get<YafBoardSettings>().Language) ?? "en-US";

        SetSelectedOnList(ref this.Theme, this.Get<YafBoardSettings>().Theme);
        SetSelectedOnList(ref this.MobileTheme, this.Get<YafBoardSettings>().MobileTheme);

        // If 2-letter language code is the same we return Culture, else we return  a default full culture from language file
        /* SetSelectedOnList(
          ref this.Culture, 
          langFileCulture.Substring(0, 2) == this.Get<YafBoardSettings>().Culture
            ? this.Get<YafBoardSettings>().Culture
            : langFileCulture);*/
        SetSelectedOnList(ref this.Culture, this.Get<YafBoardSettings>().Culture);
        if (this.Culture.SelectedIndex == 0)
        {
          // If 2-letter language code is the same we return Culture, else we return  a default full culture from language file
          SetSelectedOnList(
            ref this.Culture,
            langFileCulture.Substring(0, 2) == this.Get<YafBoardSettings>().Culture
              ? this.Get<YafBoardSettings>().Culture
              : langFileCulture);
        }

        SetSelectedOnList(ref this.ShowTopic, this.Get<YafBoardSettings>().ShowTopicsDefault.ToString());
        SetSelectedOnList(
            ref this.FileExtensionAllow, this.Get<YafBoardSettings>().FileExtensionAreAllowed ? "0" : "1");

        SetSelectedOnList(
            ref this.DefaultNotificationSetting, 
            this.Get<YafBoardSettings>().DefaultNotificationSetting.ToInt().ToString());

        this.NotificationOnUserRegisterEmailList.Text =
            this.Get<YafBoardSettings>().NotificationOnUserRegisterEmailList;
        this.AllowThemedLogo.Checked = this.Get<YafBoardSettings>().AllowThemedLogo;
        this.EmailModeratorsOnModeratedPost.Checked = this.Get<YafBoardSettings>().EmailModeratorsOnModeratedPost;
        this.EmailModeratorsOnReportedPost.Checked = this.Get<YafBoardSettings>().EmailModeratorsOnReportedPost;
        this.AllowDigestEmail.Checked = this.Get<YafBoardSettings>().AllowDigestEmail;
        this.DefaultSendDigestEmail.Checked = this.Get<YafBoardSettings>().DefaultSendDigestEmail;
        this.JqueryUIThemeCDNHosted.Checked = this.Get<YafBoardSettings>().JqueryUIThemeCDNHosted;

      this.CopyrightRemovalKey.Text = this.Get<YafBoardSettings>().CopyrightRemovalDomainKey;

      this.DigestSendEveryXHours.Text = this.Get<YafBoardSettings>().DigestSendEveryXHours.ToString();

        if (this.Get<YafBoardSettings>().BoardPollID > 0)
        {
            this.PollGroupListDropDown.SelectedValue = this.Get<YafBoardSettings>().BoardPollID.ToString();
        }
        else
        {
            this.PollGroupListDropDown.SelectedIndex = 0;
        }

        this.PollGroupList.Visible = true;

			// Copyright Linkback Algorithm
			// Please keep if you haven't purchased a removal or commercial license.

    	CopyrightHolder.Visible = true;
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
      string languageFile = "english.xml";

      var cultures =
        StaticDataHelper.Cultures().AsEnumerable().Where(
          c => c.Field<string>("CultureTag").Equals(this.Culture.SelectedValue));

      if (cultures.Any())
      {
        languageFile = cultures.First().Field<string>("CultureFile");
      }

      LegacyDb.board_save(
        this.PageContext.PageBoardID, 
        languageFile, 
        this.Culture.SelectedValue, 
        this.Name.Text, 
        this.AllowThreaded.Checked);

      // save poll group
      this.Get<YafBoardSettings>().BoardPollID = this.PollGroupListDropDown.SelectedIndex.ToType<int>() > 0
                                                     ? this.PollGroupListDropDown.SelectedValue.ToType<int>()
                                                     : 0;

      this.Get<YafBoardSettings>().Language = languageFile;
      this.Get<YafBoardSettings>().Culture = this.Culture.SelectedValue;
      this.Get<YafBoardSettings>().Theme = this.Theme.SelectedValue;

      // allow null/empty as a mobile theme many not be desired.
      this.Get<YafBoardSettings>().MobileTheme = this.MobileTheme.SelectedValue ?? string.Empty;

      this.Get<YafBoardSettings>().ShowTopicsDefault = this.ShowTopic.SelectedValue.ToType<int>();
      this.Get<YafBoardSettings>().AllowThemedLogo = this.AllowThemedLogo.Checked;
      this.Get<YafBoardSettings>().FileExtensionAreAllowed = this.FileExtensionAllow.SelectedValue.ToType<int>() == 0
                                                                 ? true
                                                                 : false;
      this.Get<YafBoardSettings>().NotificationOnUserRegisterEmailList =
        this.NotificationOnUserRegisterEmailList.Text.Trim();

      this.Get<YafBoardSettings>().EmailModeratorsOnModeratedPost = this.EmailModeratorsOnModeratedPost.Checked;
      this.Get<YafBoardSettings>().EmailModeratorsOnReportedPost = this.EmailModeratorsOnReportedPost.Checked;
      this.Get<YafBoardSettings>().AllowDigestEmail = this.AllowDigestEmail.Checked;
      this.Get<YafBoardSettings>().DefaultSendDigestEmail = this.DefaultSendDigestEmail.Checked;
      this.Get<YafBoardSettings>().DefaultNotificationSetting =
        this.DefaultNotificationSetting.SelectedValue.ToEnum<UserNotificationSetting>();

      this.Get<YafBoardSettings>().CopyrightRemovalDomainKey = this.CopyrightRemovalKey.Text.Trim();
      this.Get<YafBoardSettings>().JqueryUITheme = this.JqueryUITheme.SelectedValue;
      this.Get<YafBoardSettings>().JqueryUIThemeCDNHosted = this.JqueryUIThemeCDNHosted.Checked;

      int hours;

      if (!int.TryParse(this.DigestSendEveryXHours.Text, out hours))
      {
        hours = 24;
      }

      this.Get<YafBoardSettings>().DigestSendEveryXHours = hours;

      // save the settings to the database
      ((YafLoadBoardSettings)this.Get<YafBoardSettings>()).SaveRegistry();

      // Reload forum settings
      this.PageContext.BoardSettings = null;

      // Clearing cache with old users permissions data to get new default styles...
      this.Get<IDataCache>().Remove(x => x.StartsWith(Constants.Cache.ActiveUserLazyData));
      YafBuildLink.Redirect(ForumPages.admin_admin);
    }

    /// <summary>
    /// The set selected on list.
    /// </summary>
    /// <param name="list">
    /// The list.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    private static void SetSelectedOnList([NotNull] ref DropDownList list, [NotNull] string value)
    {
      ListItem selItem = list.Items.FindByValue(value);

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
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      DataRow row;
      using (DataTable dt = LegacyDb.board_list(this.PageContext.PageBoardID))
      {
        row = dt.Rows[0];
      }

      this.DataBind();
      this.Name.Text = row["Name"].ToString();
      this.AllowThreaded.Checked = Convert.ToBoolean(row["AllowThreaded"]);
    }

    #endregion
  }
}