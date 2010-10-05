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

namespace YAF.Pages
{
  // YAF.Pages
  #region Using

  using System;
  using System.Data;
  using System.Linq;
  using System.Web.UI.WebControls;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// Summary description for cp_subscriptions.
  /// </summary>
  public partial class cp_subscriptions : ForumPageRegistered
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "cp_subscriptions" /> class.
    /// </summary>
    public cp_subscriptions()
      : base("CP_SUBSCRIPTIONS")
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// The format forum replies.
    /// </summary>
    /// <param name="o">
    /// The o.
    /// </param>
    /// <returns>
    /// The format forum replies.
    /// </returns>
    protected string FormatForumReplies(object o)
    {
      var row = (DataRowView)o;
      return "{0}".FormatWith((int)row["Messages"] - (int)row["Topics"]);
    }

    /// <summary>
    /// The format last posted.
    /// </summary>
    /// <param name="o">
    /// The o.
    /// </param>
    /// <returns>
    /// The format last posted.
    /// </returns>
    protected string FormatLastPosted(object o)
    {
      var row = (DataRowView)o;

      if (row["LastPosted"].ToString().Length == 0)
      {
        return "&nbsp;";
      }

      string link =
        "<a href=\"{0}\">{1}</a>".FormatWith(
          YafBuildLink.GetLink(ForumPages.profile, "u={0}", row["LastUserID"]), row["LastUserName"]);
      string by = this.GetTextFormatted(
        "lastpostlink", this.Get<YafDateTime>().FormatDateTime((DateTime)row["LastPosted"]), link);

      string html = "{0} <a href=\"{1}\"><img src=\"{2}\"'></a>".FormatWith(
        by, 
        YafBuildLink.GetLink(ForumPages.posts, "m={0}#post{0}", row["LastMessageID"]), 
        this.GetThemeContents("ICONS", "ICON_LATEST"));

      return html;
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
    protected void Page_Load(object sender, EventArgs e)
    {
      if (!this.IsPostBack)
      {
        this.BindData();

        this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(this.PageContext.PageUserName, YafBuildLink.GetLink(ForumPages.cp_profile));
        this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);

        this.UnsubscribeForums.Text = this.GetText("unsubscribe");
        this.UnsubscribeTopics.Text = this.GetText("unsubscribe");
        this.SaveUser.Text = this.GetText("Save");

        this.DailyDigestRow.Visible = this.PageContext.BoardSettings.AllowDigestEmail;
        this.PMNotificationRow.Visible = this.PageContext.BoardSettings.AllowPMEmailNotification;

        var items = EnumHelper.EnumToDictionary<UserNotificationSetting>();

        if (!this.PageContext.BoardSettings.AllowNotificationAllPostsAllTopics)
        {
          // remove it...
          items.Remove(UserNotificationSetting.AllTopics.ToInt());
        }

        this.rblNotificationType.Items.AddRange(
          items.Select(x => new ListItem(this.GetText(x.Value), x.Key.ToString())).ToArray());

        var setting =
          this.rblNotificationType.Items.FindByValue(
            this.PageContext.CurrentUserData.NotificationSetting.ToInt().ToString());

        if (setting == null)
        {
          setting = this.rblNotificationType.Items.FindByValue(0.ToString());
        }

        if (setting != null)
        {
          setting.Selected = true;
        }

        // update the ui...
        this.UpdateSubscribeUI(this.PageContext.CurrentUserData.NotificationSetting);
      }
    }

    /// <summary>
    /// The save user_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void SaveUser_Click(object sender, EventArgs e)
    {
      if (Page.IsValid)
      {
        bool autoWatchTopicsEnabled = false;

        var value = this.rblNotificationType.SelectedValue.ToEnum<UserNotificationSetting>();

        if (value == UserNotificationSetting.TopicsIPostToOrSubscribeTo)
        {
          autoWatchTopicsEnabled = true;
        }

        // save the settings...
        DB.user_savenotification(
          this.PageContext.PageUserID,
          this.PMNotificationEnabled.Checked,
          autoWatchTopicsEnabled,
          this.rblNotificationType.SelectedValue,
          this.DailyDigestEnabled.Checked);

        // clear the cache for this user...
        UserMembershipHelper.ClearCacheForUserId(this.PageContext.PageUserID);

        // Clearing cache with old Active User Lazy Data ...
        this.PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.ActiveUserLazyData.FormatWith(this.PageContext.PageUserID)));

        this.PageContext.AddLoadMessage(this.GetText("SAVED_NOTIFICATION_SETTING"));
      }
    }

    /// <summary>
    /// The unsubscribe forums_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void UnsubscribeForums_Click(object sender, EventArgs e)
    {
      bool noneChecked = true;

      for (int i = 0; i < this.ForumList.Items.Count; i++)
      {
        var ctrl = (CheckBox)this.ForumList.Items[i].FindControl("unsubf");
        var lbl = (Label)this.ForumList.Items[i].FindControl("tfid");
        if (ctrl.Checked)
        {
          DB.watchforum_delete(lbl.Text);
          noneChecked = false;
        }
      }

      if (noneChecked)
      {
        this.PageContext.AddLoadMessage(this.GetText("WARN_SELECTFORUMS"));
      }
      else
      {
        this.BindData();
      }
    }

    /// <summary>
    /// The unsubscribe topics_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void UnsubscribeTopics_Click(object sender, EventArgs e)
    {
      bool noneChecked = true;

      for (int i = 0; i < this.TopicList.Items.Count; i++)
      {
        var ctrl = (CheckBox)this.TopicList.Items[i].FindControl("unsubx");
        var lbl = (Label)this.TopicList.Items[i].FindControl("ttid");
        if (ctrl.Checked)
        {
          DB.watchtopic_delete(lbl.Text);
          noneChecked = false;
        }
      }

      if (noneChecked)
      {
        this.PageContext.AddLoadMessage(this.GetText("WARN_SELECTTOPICS"));
      }
      else
      {
        this.BindData();
      }
    }

    /// <summary>
    /// The rbl notification type_ selection changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void rblNotificationType_SelectionChanged(object sender, EventArgs e)
    {
      var selectedValue = this.rblNotificationType.SelectedItem.Value.ToEnum<UserNotificationSetting>();

      this.UpdateSubscribeUI(selectedValue);
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.ForumList.DataSource = DB.watchforum_list(this.PageContext.PageUserID);
      this.TopicList.DataSource = DB.watchtopic_list(this.PageContext.PageUserID);

      this.PMNotificationEnabled.Checked = this.PageContext.CurrentUserData.PMNotification;
      this.DailyDigestEnabled.Checked = this.PageContext.CurrentUserData.DailyDigest;

      this.DataBind();
    }

    /// <summary>
    /// The update subscribe ui.
    /// </summary>
    /// <param name="selectedValue">
    /// The selected value.
    /// </param>
    private void UpdateSubscribeUI(UserNotificationSetting selectedValue)
    {
      bool showSubscribe =
        !(selectedValue == UserNotificationSetting.AllTopics || selectedValue == UserNotificationSetting.NoNotification);

      this.SubscribeHolder.Visible = showSubscribe;
    }

    #endregion
  }
}