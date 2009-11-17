namespace YAF.Controls
{
  using System;
  using System.Data;
  using System.Web;
  using System.Web.UI.WebControls;
  using Classes.Core;
  using Classes.UI;
  using YAF.Classes;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// The display post footer.
  /// </summary>
  public partial class DisplayPostFooter : BaseUserControl
  {
    /// <summary>
    /// The current Post Data for this post.
    /// </summary>
    private PostDataHelperWrapper _postDataHelperWrapper = null;

    /// <summary>
    /// Provides access to the Toggle Post button.
    /// </summary>
    public ThemeButton TogglePost
    {
      get
      {
        return btnTogglePost;
      }
    }

    /// <summary>
    /// Gets and Sets the DataRow.
    /// </summary>
    public DataRowView DataRow
    {
      get
      {
        if (this._postDataHelperWrapper.DataRow != null)
        {
          return this._postDataHelperWrapper.DataRow;
        }

        return null;
      }
      set
      {
        _postDataHelperWrapper = new PostDataHelperWrapper(value);
      }
    }

    /// <summary>
    /// Gets access Post Data helper functions.
    /// </summary>
    public PostDataHelperWrapper PostData
    {
      get
      {
        return this._postDataHelperWrapper;
      }
    }

    /// <summary>
    /// Gets a value indicating whether IsGuest.
    /// </summary>
    public bool IsGuest
    {
      get
      {
        if (PostData != null)
        {
          return UserMembershipHelper.IsGuestUser(PostData.UserId);
        }
        else
        {
          return true;
        }
      }
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
      if (!PageContext.BoardSettings.AllowGuestToReportPost && PageContext.CurrentUserData.IsGuest)
      {
        this.ReportButtons.Visible = false;
      }
    }

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit(EventArgs e)
    {
      this.ReportPostLinkButton.Command += new CommandEventHandler(Report_Command);
      this.ReportAbuseLinkButton.Command += new CommandEventHandler(Report_Command);
      this.ReportSpamButton.Command += new CommandEventHandler(Report_Command);
      PreRender += new EventHandler(DisplayPostFooter_PreRender);
      base.OnInit(e);
    }

    /// <summary>
    /// The display post footer_ pre render.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void DisplayPostFooter_PreRender(object sender, EventArgs e)
    {
      // report posts
      this.ReportPostLinkButton.Visible = PageContext.BoardSettings.AllowReportAbuse && !IsGuest; // vzrus Addition 08/18/2007
      this.ReportPostLinkButton.Text = PageContext.Localization.GetText("REPORTPOST"); // Mek Addition 08/18/2007
      this.ReportPostLinkButton.Attributes.Add("onclick", String.Format("return confirm('{0}');", PageContext.Localization.GetText("CONFIRM_REPORTPOST")));

      // report abuse posts
      this.ReportAbuseLinkButton.Visible = PageContext.BoardSettings.AllowReportAbuse && !IsGuest; // Mek Addition 08/18/2007
      this.ReportAbuseLinkButton.Text = PageContext.Localization.GetText("REPORTABUSE"); // Mek Addition 08/18/2007
      this.ReportAbuseLinkButton.Attributes.Add("onclick", String.Format("return confirm('{0}');", PageContext.Localization.GetText("CONFIRM_REPORTABUSE")));

      // report spam
      this.ReportSpamButton.Visible = PageContext.BoardSettings.AllowReportSpam && !IsGuest; // Mek Addition 08/18/2007
      this.ReportSpamButton.Text = PageContext.Localization.GetText("REPORTSPAM"); // Mek Addition 08/18/2007
      this.ReportSpamButton.Attributes.Add("onclick", String.Format("return confirm('{0}');", PageContext.Localization.GetText("CONFIRM_REPORTSPAM")));

      // private messages
      this.Pm.Visible = !IsGuest && !PostData.PostDeleted && PageContext.User != null && PageContext.BoardSettings.AllowPrivateMessages && !PostData.IsSponserMessage;
      this.Pm.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.pmessage, "u={0}", PostData.UserId);

      // emailing
      this.Email.Visible = !IsGuest && !PostData.PostDeleted && PageContext.User != null && PageContext.BoardSettings.AllowEmailSending && !PostData.IsSponserMessage;
      this.Email.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_email, "u={0}", PostData.UserId);

      // home page
      this.Home.Visible = !PostData.PostDeleted && !String.IsNullOrEmpty(PostData.UserProfile.Homepage);
      SetupThemeButtonWithLink(this.Home, PostData.UserProfile.Homepage);

      // blog page
      this.Blog.Visible = !PostData.PostDeleted && !String.IsNullOrEmpty(PostData.UserProfile.Blog);
      SetupThemeButtonWithLink(this.Blog, PostData.UserProfile.Blog);

      // MSN
      this.Msn.Visible = !PostData.PostDeleted && PageContext.User != null && !String.IsNullOrEmpty(PostData.UserProfile.MSN);
      this.Msn.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_msn, "u={0}", PostData.UserId);

      // Yahoo IM
      this.Yim.Visible = !PostData.PostDeleted && PageContext.User != null && !String.IsNullOrEmpty(PostData.UserProfile.YIM);
      this.Yim.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_yim, "u={0}", PostData.UserId);

      // AOL IM
      this.Aim.Visible = !PostData.PostDeleted && PageContext.User != null && !String.IsNullOrEmpty(PostData.UserProfile.AIM);
      this.Aim.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_aim, "u={0}", PostData.UserId);

      // ICQ
      this.Icq.Visible = !PostData.PostDeleted && PageContext.User != null && !String.IsNullOrEmpty(PostData.UserProfile.ICQ);
      this.Icq.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_icq, "u={0}", PostData.UserId);

      // Skype
      this.Skype.Visible = !PostData.PostDeleted && PageContext.User != null && !String.IsNullOrEmpty(PostData.UserProfile.Skype);
      this.Skype.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_skype, "u={0}", PostData.UserId);

      if (!PostData.PostDeleted)
      {
        this.AdminInformation.InnerHtml = @"<span class=""MessageDetails"">";

        if (Convert.ToDateTime(DataRow["Edited"]) > Convert.ToDateTime(DataRow["Posted"]).AddSeconds(PageContext.BoardSettings.EditTimeOut))
        {
          string editedText = YafServices.DateTime.FormatDateTimeShort(Convert.ToDateTime(DataRow["Edited"]));

          if (HttpContext.Current.Server.HtmlDecode(Convert.ToString(DataRow["EditReason"])) != string.Empty)
          {
            // reason was specified
            editedText += String.Format(
              " | {0}: {1}", PageContext.Localization.GetText("EDIT_REASON"), FormatMsg.RepairHtml((string)DataRow["EditReason"], true));
          }
          else
          {
            // reason was not specified
            editedText += String.Format(
              " | {0}: {1}", PageContext.Localization.GetText("EDIT_REASON"), PageContext.Localization.GetText("EDIT_REASON_NA"));
          }

          // message has been edited
          // show, why the post was edited or deleted?
          string whoChanged = Convert.ToBoolean(DataRow["IsModeratorChanged"])
                                ? PageContext.Localization.GetText("EDITED_BY_MOD")
                                : PageContext.Localization.GetText("EDITED_BY_USER");
          this.AdminInformation.InnerHtml += String.Format(
            @"| <span class=""editedinfo"" title=""{2}"">{0} {1}</span>",
            PageContext.Localization.GetText("EDITED"),
            whoChanged, editedText);
        }
      }
      else
      {
        this.AdminInformation.InnerHtml = @"<span class=""MessageDetails"">";

        string deleteText = string.Empty;

        if (HttpContext.Current.Server.HtmlDecode(Convert.ToString(DataRow["DeleteReason"])) != String.Empty)
        {
          // reason was specified
          deleteText = FormatMsg.RepairHtml((string) DataRow["DeleteReason"], true);
        }
        else
        {
          // reason was not specified
          deleteText = PageContext.Localization.GetText("EDIT_REASON_NA");
        }

        this.AdminInformation.InnerHtml += String.Format(@" | <span class=""editedinfo"" title=""{1}"">{0}</span>", PageContext.Localization.GetText("EDIT_REASON"), deleteText);
      }

      // display admin only info
      if (PageContext.IsAdmin)
      {
        this.AdminInformation.InnerHtml += String.Format(@" | <span class=""ipinfo"" title=""{1}"">{0}: {1}</span>", PageContext.Localization.GetText("IP"), DataRow["IP"].ToString());
      }

      this.AdminInformation.InnerHtml += "</span>";
    }

    /// <summary>
    /// Command Button - Report post as Abusive/Spam
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Report_Command(object sender, CommandEventArgs e)
    {
      int reportFlag = 0;

      switch (e.CommandName)
      {
        case "ReportAbuse":
          reportFlag = 7;
          break;
        case "ReportSpam":
          reportFlag = 8;
          break;
        case "ReportPost":
          reportFlag = 9;
          break;
      }

      string reportMessage;
      switch (reportFlag)
      {
        case 7:
          reportMessage = PageContext.CurrentForumPage.GetText("REPORTED");
          break;
        case 8:
          reportMessage = PageContext.CurrentForumPage.GetText("REPORTEDSPAM");
          break;
        default:

          // TODO: vzrus: required a window to enter custom report text with Report and Cancel buttons 
          // Not sure how to implement it YAF-like ;) 
          reportMessage = "Message reported!";
          break;
      }

      if (reportFlag < 9)
      {
        DB.message_report(reportFlag, e.CommandArgument.ToString(), PageContext.PageUserID, DateTime.Today, reportMessage);
      }
      else
      {
        HttpContext.Current.Response.Redirect(YafBuildLink.GetLinkNotEscaped(ForumPages.reportpost, "m={0}", e.CommandArgument.ToString()));
      }

      // Response.Redirect(YAF.Classes.Utils.YafBuildLink.GetLink(ForumPages.posts, "m={0}#post{0}", DataRow["MessageID"]));
      PageContext.AddLoadMessage(PageContext.Localization.GetText("REPORTEDFEEDBACK"));
    }

    /// <summary>
    /// The setup theme button with link.
    /// </summary>
    /// <param name="thisButton">
    /// The this button.
    /// </param>
    /// <param name="linkUrl">
    /// The link url.
    /// </param>
    protected void SetupThemeButtonWithLink(ThemeButton thisButton, string linkUrl)
    {
      if (!String.IsNullOrEmpty(linkUrl))
      {
        string link = linkUrl.Replace("\"", string.Empty);
        if (!link.ToLower().StartsWith("http"))
        {
          link = "http://" + link;
        }

        thisButton.NavigateUrl = link;
        thisButton.Attributes.Add("target", "_blank");
        if (PageContext.BoardSettings.UseNoFollowLinks)
        {
          thisButton.Attributes.Add("rel", "nofollow");
        }
      }
      else
      {
        thisButton.NavigateUrl = string.Empty;
      }
    }
  }
}