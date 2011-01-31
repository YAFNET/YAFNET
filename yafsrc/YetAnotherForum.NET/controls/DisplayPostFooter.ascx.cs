namespace YAF.Controls
{
  #region Using

  using System;
  using System.Data;
  using System.Text;
  using System.Web;

  using YAF.Core;
  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// The display post footer.
  /// </summary>
  public partial class DisplayPostFooter : BaseUserControl
  {
    #region Constants and Fields

    /// <summary>
    ///   The current Post Data for this post.
    /// </summary>
    private PostDataHelperWrapper _postDataHelperWrapper;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets and Sets the DataRow.
    /// </summary>
    [CanBeNull]
    public DataRow DataRow
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
    ///   Gets a value indicating whether IsGuest.
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
    ///   Gets access Post Data helper functions.
    /// </summary>
    public PostDataHelperWrapper PostData
    {
      get
      {
        return this._postDataHelperWrapper;
      }
    }

    /// <summary>
    ///   Provides access to the Toggle Post button.
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
          string editedText = this.Get<IDateTime>().FormatDateTimeShort(Convert.ToDateTime(this.DataRow["Edited"]));

          // vzrus: Guests doesn't have right to view change history
          this.MessageHistoryHolder.Visible = true;

          if (HttpContext.Current.Server.HtmlDecode(Convert.ToString(this.DataRow["EditReason"])) != string.Empty)
          {
            // reason was specified
            this.messageHistoryLink.Title +=
              " | {0}: {1}".FormatWith(
                this.GetText("EDIT_REASON"), 
                this.Get<IFormatMessage>().RepairHtml((string)this.DataRow["EditReason"], true));
          }
          else
          {
            this.messageHistoryLink.Title += " {0}: {1}".FormatWith(
              this.GetText("EDIT_REASON"), 
              this.GetText("EDIT_REASON_NA"));
          }

          // message has been edited
          // show, why the post was edited or deleted?
          string whoChanged = Convert.ToBoolean(this.DataRow["IsModeratorChanged"])
                                ? this.GetText("EDITED_BY_MOD")
                                : this.GetText("EDITED_BY_USER");

          this.messageHistoryLink.InnerHtml =
            @"<span class=""editedinfo"" title=""{2}"">{0} {1}</span>".FormatWith(
              this.GetText("EDITED"), whoChanged, editedText + this.messageHistoryLink.Title);
          this.messageHistoryLink.HRef = YafBuildLink.GetLink(
            ForumPages.messagehistory, "m={0}", this.DataRow["MessageID"]);
        }
      }
      else
      {
        string deleteText = string.Empty;

        if (HttpContext.Current.Server.HtmlDecode(Convert.ToString(this.DataRow["DeleteReason"])) != String.Empty)
        {
          // reason was specified
          deleteText = this.Get<IFormatMessage>().RepairHtml((string)this.DataRow["DeleteReason"], true);
        }
        else
        {
          // reason was not specified
          deleteText = this.GetText("EDIT_REASON_NA");
        }

        sb.AppendFormat(
          @" | <span class=""editedinfo"" title=""{1}"">{0}</span>", 
          this.GetText("EDIT_REASON"), 
          deleteText);
      }

      // display admin only info
      if (this.PageContext.IsAdmin ||
          (this.PageContext.BoardSettings.AllowModeratorsViewIPs && this.PageContext.IsModerator))
      {
        // We should show IP
        this.IPSpan1.Visible = true;
        this.IPLink1.HRef = this.PageContext.BoardSettings.IPInfoPageURL.FormatWith(this.DataRow["IP"].ToString());
        this.IPLink1.Title = this.GetText("COMMON", "TT_IPDETAILS");
        this.IPLink1.InnerText = this.HtmlEncode(this.DataRow["IP"].ToString());

        sb.Append(' ');
      }

      if (sb.Length > 0)
      {
        this.MessageDetails.Visible = true;
        this.MessageDetails.Text = @"<span class=""MessageDetails"">" + sb + @"</span>";
      }
    }

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit([NotNull] EventArgs e)
    {
      this.PreRender += this.DisplayPostFooter_PreRender;
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
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
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
    protected void SetupThemeButtonWithLink([NotNull] ThemeButton thisButton, [NotNull] string linkUrl)
    {
      if (linkUrl.IsSet())
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
    private void DisplayPostFooter_PreRender([NotNull] object sender, [NotNull] EventArgs e)
    {
      // report posts
      if (this.Get<IPermissions>().Check(this.PageContext.BoardSettings.ReportPostPermissions) &&
          !this.PostData.PostDeleted)
      {
        if (this.PageContext.IsGuest || (!this.PageContext.IsGuest && this.PageContext.User != null))
        {
          this.reportPostLink.Visible = true;

          // vzrus Addition 
          this.reportPostLink.InnerText =
            this.reportPostLink.Title = this.GetText("REPORTPOST");

          this.reportPostLink.HRef = YafBuildLink.GetLink(ForumPages.reportpost, "m={0}", this.PostData.MessageId);
        }
      }

      // private messages
      this.Pm.Visible = this.PostData.UserId != this.PageContext.PageUserID && !this.IsGuest &&
                        !this.PostData.PostDeleted && this.PageContext.User != null &&
                        this.PageContext.BoardSettings.AllowPrivateMessages && !this.PostData.IsSponserMessage;
      this.Pm.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.pmessage, "u={0}", this.PostData.UserId);

      // emailing
      this.Email.Visible = this.PostData.UserId != this.PageContext.PageUserID && !this.IsGuest &&
                           !this.PostData.PostDeleted && this.PageContext.User != null &&
                           this.PageContext.BoardSettings.AllowEmailSending && !this.PostData.IsSponserMessage;
      this.Email.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_email, "u={0}", this.PostData.UserId);

      // home page
      this.Home.Visible = !this.PostData.PostDeleted && this.PostData.UserProfile.Homepage.IsSet();
      this.SetupThemeButtonWithLink(this.Home, this.PostData.UserProfile.Homepage);

      // blog page
      this.Blog.Visible = !this.PostData.PostDeleted && this.PostData.UserProfile.Blog.IsSet();
      this.SetupThemeButtonWithLink(this.Blog, this.PostData.UserProfile.Blog);

      if (!this.PostData.PostDeleted && this.PageContext.User != null &&
          (this.PostData.UserId != this.PageContext.PageUserID))
      {
        // MSN
        this.Msn.Visible = this.PostData.UserProfile.MSN.IsSet();
        this.Msn.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_msn, "u={0}", this.PostData.UserId);

        // Yahoo IM
        this.Yim.Visible = this.PostData.UserProfile.YIM.IsSet();
        this.Yim.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_yim, "u={0}", this.PostData.UserId);

        // AOL IM
        this.Aim.Visible = this.PostData.UserProfile.AIM.IsSet();
        this.Aim.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_aim, "u={0}", this.PostData.UserId);

        // ICQ
        this.Icq.Visible = this.PostData.UserProfile.ICQ.IsSet();
        this.Icq.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_icq, "u={0}", this.PostData.UserId);

        // XMPP
        this.Xmpp.Visible = this.PostData.UserProfile.XMPP.IsSet();
        this.Xmpp.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_xmpp, "u={0}", this.PostData.UserId);

        // Skype
        this.Skype.Visible = this.PostData.UserProfile.Skype.IsSet();
        this.Skype.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_skype, "u={0}", this.PostData.UserId);
      }

      this.CreateMessageDetails();
    }

    #endregion
  }
}