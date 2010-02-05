namespace YAF.Controls
{
  using System;
  using System.Data;
  using System.Text;
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
      this.MessageHistoryLBtn.Command += new CommandEventHandler(History_Command); 
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
      this.ReportPostLinkButton.Visible = PageContext.BoardSettings.AllowReportPosts && !IsGuest; // vzrus Addition 08/18/2007
      this.ReportPostLinkButton.Text = PageContext.Localization.GetText("REPORTPOST"); // Mek Addition 08/18/2007
      this.ReportPostLinkButton.Attributes.Add("onclick", String.Format("return confirm('{0}');", PageContext.Localization.GetText("CONFIRM_REPORTPOST")));
      
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
      
      // XMPP
      this.Xmpp.Visible = !PostData.PostDeleted && PageContext.User != null && !String.IsNullOrEmpty(PostData.UserProfile.XMPP);
      this.Xmpp.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_xmpp, "u={0}", PostData.UserId);

      // Skype
      this.Skype.Visible = !PostData.PostDeleted && PageContext.User != null && !String.IsNullOrEmpty(PostData.UserProfile.Skype);
      this.Skype.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_skype, "u={0}", PostData.UserId);

      CreateMessageDetails();
    }

    protected void CreateMessageDetails()
    {
      StringBuilder sb = new StringBuilder();

      if (!this.PostData.PostDeleted)
      {
        if (Convert.ToDateTime(this.DataRow["Edited"]) > Convert.ToDateTime(this.DataRow["Posted"]).AddSeconds(this.PageContext.BoardSettings.EditTimeOut))
        {
          string editedText = YafServices.DateTime.FormatDateTimeShort(Convert.ToDateTime(this.DataRow["Edited"]));
          // vzrus: Guests doesn't have right to view change history
        
              MessageHistoryLBtn.Visible = true;
         
          if (HttpContext.Current.Server.HtmlDecode(Convert.ToString(this.DataRow["EditReason"])) != string.Empty)
          {
            // reason was specified
            /*
              " | {0}: {1}", this.PageContext.Localization.GetText("EDIT_REASON"), FormatMsg.RepairHtml((string)this.DataRow["EditReason"], true)); */
            MessageHistoryLBtn.ToolTip = String.Format(
                "{0}: {1}", this.PageContext.Localization.GetText("EDIT_REASON"), FormatMsg.RepairHtml((string)this.DataRow["EditReason"], true));
          }
          else
          {
            // reason was not specified
           /* editedText += String.Format(
              " | {0}: {1}", this.PageContext.Localization.GetText("EDIT_REASON"), this.PageContext.Localization.GetText("EDIT_REASON_NA")); */
          MessageHistoryLBtn.ToolTip = String.Format("{0}: {1}", this.PageContext.Localization.GetText("EDIT_REASON"), this.PageContext.Localization.GetText("EDIT_REASON_NA"));
          }

          // message has been edited
          // show, why the post was edited or deleted?
          string whoChanged = Convert.ToBoolean(this.DataRow["IsModeratorChanged"])
                                ? this.PageContext.Localization.GetText("EDITED_BY_MOD")
                                : this.PageContext.Localization.GetText("EDITED_BY_USER");

         /* sb.AppendFormat(@"| <span class=""editedinfo"" title=""{2}"">{0} {1}</span>", this.PageContext.Localization.GetText("EDITED"), whoChanged, editedText); */
          this.MessageHistoryLBtn.Text = string.Format(@"| <span class=""editedinfo"" title=""{2}"">{0} {1}</span>", this.PageContext.Localization.GetText("EDITED"), whoChanged, editedText);
        }
      }
      else
      {
        string deleteText = string.Empty;

        if (HttpContext.Current.Server.HtmlDecode(Convert.ToString(this.DataRow["DeleteReason"])) != String.Empty)
        {
          // reason was specified
          deleteText = FormatMsg.RepairHtml((string) this.DataRow["DeleteReason"], true);
        }
        else
        {
          // reason was not specified
          deleteText = this.PageContext.Localization.GetText("EDIT_REASON_NA");
        }

        sb.AppendFormat(@" | <span class=""editedinfo"" title=""{1}"">{0}</span>", this.PageContext.Localization.GetText("EDIT_REASON"), deleteText);
      }

      // display admin only info
      if (PageContext.IsAdmin)
      {
        sb.AppendFormat(@" | <span class=""ipinfo"" title=""{1}"">{0}: {1}</span>", PageContext.Localization.GetText("IP"), DataRow["IP"].ToString());
      }

      if (sb.Length > 0)
      {
        this.MessageDetails.Visible = true;
        this.MessageDetails.Text = @"<span class=""MessageDetails"">" + sb.ToString() + @"</span>";
      }
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

    if  ( e.CommandName == "ReportPost" ) 
      { 
   
      HttpContext.Current.Response.Redirect(YafBuildLink.GetLinkNotEscaped(ForumPages.reportpost, "m={0}", e.CommandArgument.ToString()));

      // Response.Redirect(YAF.Classes.Utils.YafBuildLink.GetLink(ForumPages.posts, "m={0}#post{0}", DataRow["MessageID"]));
      // PageContext.AddLoadMessage(PageContext.Localization.GetText("REPORTEDFEEDBACK"));
      }
    }

    protected void History_Command(object sender, CommandEventArgs e)
    {

        if (e.CommandName == "ShowHistory")
        {

            HttpContext.Current.Response.Redirect(YafBuildLink.GetLinkNotEscaped(ForumPages.messagehistory, "m={0}", e.CommandArgument.ToString()));
                 
            // PageContext.AddLoadMessage(PageContext.Localization.GetText("REPORTEDFEEDBACK"));
        }
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