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

namespace YAF.Pages
{
  // YAF.Pages
  #region Using

  using System;
  using System.Data;
  using System.Linq;
  using System.Web.UI.WebControls;

  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.EventProxies;
  using YAF.Types.Interfaces;
  using YAF.Utils;

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
    protected string FormatForumReplies([NotNull] object o)
    {
      var row = o as DataRow;

      if (row != null)
      {
        return "{0}".FormatWith((int)row["Messages"] - (int)row["Topics"]);
      }

      return string.Empty;
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
    protected string FormatLastPosted([NotNull] object o)
    {
      var row = o as DataRow;

      if (row != null)
      {
        if (row["LastPosted"].ToString().Length == 0)
        {
          return "&nbsp;";
        }

        string link =
          "<a href=\"{0}\">{1}</a>".FormatWith(
            YafBuildLink.GetLink(ForumPages.profile, "u={0}", row["LastUserID"]), row["LastUserName"]);
        string by = this.GetTextFormatted(
          "lastpostlink", this.Get<IDateTime>().FormatDateTime((DateTime)row["LastPosted"]), link);

        string html = @"{0} <a href=""{1}""><img src=""{2}"" alt="""" /></a>".FormatWith(
          by,
          YafBuildLink.GetLink(ForumPages.posts, "m={0}&find=lastpost", row["LastMessageID"]), 
          this.GetThemeContents("ICONS", "ICON_LATEST"));

        return html;
      }

      return string.Empty;
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
    /// The pager top_ page change.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void PagerTop_PageChange([NotNull] object sender, [NotNull] EventArgs e)
    {
      // rebind
      this.BindData();
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
    protected void SaveUser_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (this.Page.IsValid)
      {
        bool autoWatchTopicsEnabled = false;

        var value = this.rblNotificationType.SelectedValue.ToEnum<UserNotificationSetting>();

        if (value == UserNotificationSetting.TopicsIPostToOrSubscribeTo)
        {
          autoWatchTopicsEnabled = true;
        }

        // save the settings...
        LegacyDb.user_savenotification(
          this.PageContext.PageUserID, 
          this.PMNotificationEnabled.Checked, 
          autoWatchTopicsEnabled, 
          this.rblNotificationType.SelectedValue, 
          this.DailyDigestEnabled.Checked);

        this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.PageContext.PageUserID));

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
    protected void UnsubscribeForums_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      bool noneChecked = true;

      for (int i = 0; i < this.ForumList.Items.Count; i++)
      {
        var ctrl = (CheckBox)this.ForumList.Items[i].FindControl("unsubf");
        var lbl = (Label)this.ForumList.Items[i].FindControl("tfid");
        if (ctrl.Checked)
        {
          LegacyDb.watchforum_delete(lbl.Text);
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
    protected void UnsubscribeTopics_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      bool noneChecked = true;

      for (int i = 0; i < this.TopicList.Items.Count; i++)
      {
        var ctrl = (CheckBox)this.TopicList.Items[i].FindControl("unsubx");
        var lbl = (Label)this.TopicList.Items[i].FindControl("ttid");
        if (ctrl.Checked)
        {
          LegacyDb.watchtopic_delete(lbl.Text);
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
    protected void rblNotificationType_SelectionChanged([NotNull] object sender, [NotNull] EventArgs e)
    {
      var selectedValue = this.rblNotificationType.SelectedItem.Value.ToEnum<UserNotificationSetting>();

      this.UpdateSubscribeUI(selectedValue);
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.ForumList.DataSource = LegacyDb.watchforum_list(this.PageContext.PageUserID).AsEnumerable();

      // we are going to page results
      var dt = LegacyDb.watchtopic_list(this.PageContext.PageUserID);

      // set pager and datasource
      this.PagerTop.Count = dt.Rows.Count;

      // page to render
      var currentPageIndex = this.PagerTop.CurrentPageIndex;

      var pageCount = this.PagerTop.Count / this.PagerTop.PageSize;

      // if we are above total number of pages, select last
      if (currentPageIndex >= pageCount)
      {
        currentPageIndex = pageCount - 1;
      }

      // bind list
      this.TopicList.DataSource =
        dt.AsEnumerable().Skip(currentPageIndex * this.PagerTop.PageSize).Take(this.PagerTop.PageSize);

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