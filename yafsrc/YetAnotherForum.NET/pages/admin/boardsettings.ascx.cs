/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2010 Jaben Cargman
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
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Pattern;
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// Summary description for settings.
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
      if (!this.IsPostBack)
      {
        this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink("Administration", YafBuildLink.GetLink(ForumPages.admin_admin));
        this.PageLinks.AddLink("Board Settings", string.Empty);

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
          this.MobileTheme.DataSource = mobileThemeData.CopyToDataTable();
          this.MobileTheme.DataTextField = "Theme";
          this.MobileTheme.DataValueField = "FileName";
        }

        this.Culture.DataSource =
          StaticDataHelper.Cultures().AsEnumerable().OrderBy(x => x.Field<string>("CultureNativeName")).CopyToDataTable(
            );
        this.Culture.DataTextField = "CultureNativeName";
        this.Culture.DataValueField = "CultureTag";

        this.ShowTopic.DataSource = StaticDataHelper.TopicTimes();
        this.ShowTopic.DataTextField = "TopicText";
        this.ShowTopic.DataValueField = "TopicValue";

        this.FileExtensionAllow.DataSource = StaticDataHelper.AllowDisallow();
        this.FileExtensionAllow.DataTextField = "Text";
        this.FileExtensionAllow.DataValueField = "Value";

        this.BindData();

        // bind poll group list
        var pollGroup =
          DB.PollGroupList(this.PageContext.PageUserID, null, this.PageContext.PageBoardID).Distinct(
            new AreEqualFunc<TypedPollGroup>((v1, v2) => v1.PollGroupID == v2.PollGroupID)).ToList();

        pollGroup.Insert(0, new TypedPollGroup(String.Empty, -1));

        // TODO: vzrus needs some work, will be in polls only until feature is debugged there.
        this.PollGroupListDropDown.Items.AddRange(
          pollGroup.Select(x => new ListItem(x.Question, x.PollGroupID.ToString())).ToArray());

        // population default notification setting options...
        var items = EnumHelper.EnumToDictionary<UserNotificationSetting>();

        if (!this.PageContext.BoardSettings.AllowNotificationAllPostsAllTopics)
        {
          // remove it...
          items.Remove(UserNotificationSetting.AllTopics.ToInt());
        }

        var notificationItems =
          items.Select(
            x =>
            new ListItem(
              HtmlHelper.StripHtml(this.PageContext.Localization.GetText("CP_SUBSCRIPTIONS", x.Value)), x.Key.ToString()))
            .ToArray();

        this.DefaultNotificationSetting.Items.AddRange(notificationItems);

        // Get first default full culture from a language file tag.
        string langFileCulture = StaticDataHelper.CultureDefaultFromFile(this.PageContext.BoardSettings.Language) ??
                                 "en";

        SetSelectedOnList(ref this.Theme, this.PageContext.BoardSettings.Theme);
        SetSelectedOnList(ref this.MobileTheme, this.PageContext.BoardSettings.MobileTheme);

        // If 2-letter language code is the same we return Culture, else we return  a default full culture from language file
        SetSelectedOnList(
          ref this.Culture, 
          langFileCulture.Substring(0, 2) == this.PageContext.BoardSettings.Culture
            ? this.PageContext.BoardSettings.Culture
            : langFileCulture);

        SetSelectedOnList(ref this.ShowTopic, this.PageContext.BoardSettings.ShowTopicsDefault.ToString());
        SetSelectedOnList(
          ref this.FileExtensionAllow, this.PageContext.BoardSettings.FileExtensionAreAllowed ? "0" : "1");

        SetSelectedOnList(
          ref this.DefaultNotificationSetting, 
          this.PageContext.BoardSettings.DefaultNotificationSetting.ToInt().ToString());

        this.NotificationOnUserRegisterEmailList.Text =
          this.PageContext.BoardSettings.NotificationOnUserRegisterEmailList;
        this.AllowThemedLogo.Checked = this.PageContext.BoardSettings.AllowThemedLogo;
        this.EmailModeratorsOnModeratedPost.Checked = this.PageContext.BoardSettings.EmailModeratorsOnModeratedPost;
        this.AllowDigestEmail.Checked = this.PageContext.BoardSettings.AllowDigestEmail;
        this.DefaultSendDigestEmail.Checked = this.PageContext.BoardSettings.DefaultSendDigestEmail;

        if (this.PageContext.BoardSettings.BoardPollID > 0)
        {
          this.PollGroupListDropDown.SelectedValue = this.PageContext.BoardSettings.BoardPollID.ToString();
        }
        else
        {
          this.PollGroupListDropDown.SelectedIndex = 0;
        }

        this.PollGroupList.Visible = true;
      }
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

      DB.board_save(
        this.PageContext.PageBoardID, 
        languageFile, 
        this.Culture.SelectedValue, 
        this.Name.Text, 
        this.AllowThreaded.Checked);

      // save poll group
      this.PageContext.BoardSettings.BoardPollID = this.PollGroupListDropDown.SelectedIndex.ToType<int>() > 0
                                                     ? this.PollGroupListDropDown.SelectedValue.ToType<int>()
                                                     : 0;

      this.PageContext.BoardSettings.Language = languageFile;
      this.PageContext.BoardSettings.Culture = this.Culture.SelectedValue;
      this.PageContext.BoardSettings.Theme = this.Theme.SelectedValue;

      // allow null/empty as a mobile theme many not be desired.
      this.PageContext.BoardSettings.MobileTheme = this.MobileTheme.SelectedValue ?? String.Empty;
      
      this.PageContext.BoardSettings.ShowTopicsDefault = this.ShowTopic.SelectedValue.ToType<int>();
      this.PageContext.BoardSettings.AllowThemedLogo = this.AllowThemedLogo.Checked;
      this.PageContext.BoardSettings.FileExtensionAreAllowed = this.FileExtensionAllow.SelectedValue.ToType<int>() == 0
                                                                 ? true
                                                                 : false;
      this.PageContext.BoardSettings.NotificationOnUserRegisterEmailList =
        this.NotificationOnUserRegisterEmailList.Text.Trim();

      this.PageContext.BoardSettings.EmailModeratorsOnModeratedPost = this.EmailModeratorsOnModeratedPost.Checked;
      this.PageContext.BoardSettings.AllowDigestEmail = this.AllowDigestEmail.Checked;
      this.PageContext.BoardSettings.DefaultSendDigestEmail = this.DefaultSendDigestEmail.Checked;
      this.PageContext.BoardSettings.DefaultNotificationSetting =
        this.DefaultNotificationSetting.SelectedValue.ToEnum<UserNotificationSetting>();

      // save the settings to the database
      ((YafLoadBoardSettings)this.PageContext.BoardSettings).SaveRegistry();

      // Reload forum settings
      this.PageContext.BoardSettings = null;

      // Clearing cache with old users permissions data to get new default styles...
      this.PageContext.Cache.Remove((x) => x.StartsWith(YafCache.GetBoardCacheKey(Constants.Cache.ActiveUserLazyData)));
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
      using (DataTable dt = DB.board_list(this.PageContext.PageBoardID))
      {
        row = dt.Rows[0];
      }

      this.DataBind();
      this.Name.Text = row["Name"].ToString();
      this.AllowThreaded.Checked = SqlDataLayerConverter.VerifyBool(row["AllowThreaded"]);
    }

    #endregion
  }
}