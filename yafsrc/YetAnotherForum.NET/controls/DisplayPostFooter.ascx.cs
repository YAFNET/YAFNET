namespace YAF.Controls
{
  #region Using

  using System;
  using System.Data;
  using System.Text;
  using System.Web;
  using System.Web.UI.WebControls;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.UI;
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// The display post footer.
  /// </summary>
  public partial class DisplayPostFooter : BaseUserControl
  {
    #region Constants and Fields

    /// <summary>
    /// The current Post Data for this post.
    /// </summary>
    private PostDataHelperWrapper _postDataHelperWrapper = null;

    #endregion

    #region Properties

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
        this._postDataHelperWrapper = new PostDataHelperWrapper(value);
      }
    }

    /// <summary>
    /// Gets a value indicating whether IsGuest.
    /// </summary>
    public bool IsGuest
    {
      get
      {
        if (this.PostData != null)
        {
          return UserMembershipHelper.IsGuestUser(this.PostData.UserId);
        }
        else
        {
          return true;
        }
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
    /// Provides access to the Toggle Post button.
    /// </summary>
    public ThemeButton TogglePost
    {
      get
      {
        return this.btnTogglePost;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The create message details.
    /// </summary>
    protected void CreateMessageDetails()
    {
      var sb = new StringBuilder();

      if (!this.PostData.PostDeleted)
      {
        if (Convert.ToDateTime(this.DataRow["Edited"]) >
            Convert.ToDateTime(this.DataRow["Posted"]).AddSeconds(this.PageContext.BoardSettings.EditTimeOut))
        {
          string editedText = YafServices.DateTime.FormatDateTimeShort(Convert.ToDateTime(this.DataRow["Edited"]));

          // vzrus: Guests doesn't have right to view change history
          this.MessageHistoryHolder.Visible = true;

          if (HttpContext.Current.Server.HtmlDecode(Convert.ToString(this.DataRow["EditReason"])) != string.Empty)
          {
            // reason was specified
           
              this.messageHistoryLink.Title += String.Format(
                 " | {0}: {1}",
                 this.PageContext.Localization.GetText("EDIT_REASON"),
                 FormatMsg.RepairHtml((string)this.DataRow["EditReason"], true));
          }
          else
          {
              this.messageHistoryLink.Title += String.Format(
               " {0}: {1}",
               this.PageContext.Localization.GetText("EDIT_REASON"),
               this.PageContext.Localization.GetText("EDIT_REASON_NA"));
          }

          // message has been edited
          // show, why the post was edited or deleted?
          string whoChanged = Convert.ToBoolean(this.DataRow["IsModeratorChanged"])
                                ? this.PageContext.Localization.GetText("EDITED_BY_MOD")
                                : this.PageContext.Localization.GetText("EDITED_BY_USER");

          this.messageHistoryLink.InnerHtml = string.Format(
             @"<span class=""editedinfo"" title=""{2}"">{0} {1}</span>",
             this.PageContext.Localization.GetText("EDITED"),
             whoChanged,
             editedText + this.messageHistoryLink.Title);
          this.messageHistoryLink.HRef = YafBuildLink.GetLinkNotEscaped(ForumPages.messagehistory, "m={0}", DataRow["MessageID"]);

        }
      }
      else
      {
        string deleteText = string.Empty;

        if (HttpContext.Current.Server.HtmlDecode(Convert.ToString(this.DataRow["DeleteReason"])) != String.Empty)
        {
          // reason was specified
          deleteText = FormatMsg.RepairHtml((string)this.DataRow["DeleteReason"], true);
        }
        else
        {
          // reason was not specified
          deleteText = this.PageContext.Localization.GetText("EDIT_REASON_NA");
        }

        sb.AppendFormat(
          @" | <span class=""editedinfo"" title=""{1}"">{0}</span>", 
          this.PageContext.Localization.GetText("EDIT_REASON"), 
          deleteText);
      }

      // display admin only info
      if (this.PageContext.IsAdmin ||
          (this.PageContext.BoardSettings.AllowModeratorsViewIPs && this.PageContext.IsModerator))
      {
         
          // We should show IP
          this.IPSpan1.Visible = true;
          this.IPLink1.HRef = string.Format(this.PageContext.BoardSettings.IPInfoPageURL, this.DataRow["IP"].ToString());
          this.IPLink1.Title= this.PageContext.Localization.GetText("COMMON","TT_IPDETAILS");
          this.IPLink1.InnerText = HttpContext.Current.Server.HtmlEncode(this.DataRow["IP"].ToString());

          sb.Append(' ');
        
      } 

      if (sb.Length > 0)
      {
        this.MessageDetails.Visible = true;
        this.MessageDetails.Text = @"<span class=""MessageDetails"">" + sb.ToString() + @"</span>";
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
      this.PreRender += new EventHandler(this.DisplayPostFooter_PreRender);
      base.OnInit(e);
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
        if (this.PageContext.BoardSettings.AllowReportPosts && !this.IsGuest)
        {
            this.reportPostLink.Visible = true;
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
        if (this.PageContext.BoardSettings.UseNoFollowLinks)
        {
          thisButton.Attributes.Add("rel", "nofollow");
        }
      }
      else
      {
        thisButton.NavigateUrl = string.Empty;
      }
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
        if (!this.PageContext.BoardSettings.AllowGuestToReportPost && this.PageContext.CurrentUserData.IsGuest)
        {
            this.reportPostLink.Visible = true;
        }           
        
      // vzrus Addition 
      this.reportPostLink.InnerText = this.reportPostLink.Title = this.PageContext.Localization.GetText("REPORTPOST"); 

      this.reportPostLink.HRef = YafBuildLink.GetLinkNotEscaped(ForumPages.reportpost, "m={0}", this.PostData.MessageId);
      
      // private messages
      this.Pm.Visible = !this.IsGuest && !this.PostData.PostDeleted && this.PageContext.User != null &&
                        this.PageContext.BoardSettings.AllowPrivateMessages && !this.PostData.IsSponserMessage;
      this.Pm.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.pmessage, "u={0}", this.PostData.UserId);

      // emailing
      this.Email.Visible = !this.IsGuest && !this.PostData.PostDeleted && this.PageContext.User != null &&
                           this.PageContext.BoardSettings.AllowEmailSending && !this.PostData.IsSponserMessage;
      this.Email.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_email, "u={0}", this.PostData.UserId);

      // home page
      this.Home.Visible = !this.PostData.PostDeleted && !String.IsNullOrEmpty(this.PostData.UserProfile.Homepage);
      this.SetupThemeButtonWithLink(this.Home, this.PostData.UserProfile.Homepage);

      // blog page
      this.Blog.Visible = !this.PostData.PostDeleted && !String.IsNullOrEmpty(this.PostData.UserProfile.Blog);
      this.SetupThemeButtonWithLink(this.Blog, this.PostData.UserProfile.Blog);

      if (!this.PostData.PostDeleted && this.PageContext.User != null)
      {
        // MSN
        this.Msn.Visible = !String.IsNullOrEmpty(this.PostData.UserProfile.MSN);
        this.Msn.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_msn, "u={0}", this.PostData.UserId);

        // Yahoo IM
        this.Yim.Visible = !String.IsNullOrEmpty(this.PostData.UserProfile.YIM);
        this.Yim.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_yim, "u={0}", this.PostData.UserId);

        // AOL IM
        this.Aim.Visible = !String.IsNullOrEmpty(this.PostData.UserProfile.AIM);
        this.Aim.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_aim, "u={0}", this.PostData.UserId);

        // ICQ
        this.Icq.Visible = !String.IsNullOrEmpty(this.PostData.UserProfile.ICQ);
        this.Icq.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_icq, "u={0}", this.PostData.UserId);

        // XMPP
        this.Xmpp.Visible = !String.IsNullOrEmpty(this.PostData.UserProfile.XMPP);
        this.Xmpp.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_xmpp, "u={0}", this.PostData.UserId);

        // Skype
        this.Skype.Visible = !String.IsNullOrEmpty(this.PostData.UserProfile.Skype);
        this.Skype.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_skype, "u={0}", this.PostData.UserId);
      }

      this.CreateMessageDetails();
    }

    #endregion
  }
}