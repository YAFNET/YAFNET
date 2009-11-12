/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2009 Jaben Cargman
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
namespace YAF.Controls
{
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Text;
  using System.Web.UI;
  using System.Web.UI.WebControls;
  using AjaxPro;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.UI;
  using YAF.Classes.Utils;
  using YAF.Utilities;

  /// <summary>
  /// Summary description for DisplayPost.
  /// </summary>
  public partial class DisplayPost : BaseUserControl
  {
    /// <summary>
    /// The _forum flags.
    /// </summary>
    private ForumFlags _forumFlags;

    /// <summary>
    /// The _is alt.
    /// </summary>
    private bool _isAlt = false;

    /// <summary>
    /// The _is threaded.
    /// </summary>
    private bool _isThreaded = false;

    /// <summary>
    /// The _message flags.
    /// </summary>
    private MessageFlags _messageFlags;

    /// <summary>
    /// The _parent page.
    /// </summary>
    private ForumPage _parentPage;

    /// <summary>
    /// The current data row for this post.
    /// </summary>
    private DataRowView _row = null;

    /// <summary>
    /// The _topic flags.
    /// </summary>
    private TopicFlags _topicFlags;

    /// <summary>
    /// The _user ignore list.
    /// </summary>
    private List<int> _userIgnoreList = null;

    /// <summary>
    /// The _user profile.
    /// </summary>
    private YafUserProfile _userProfile = null;

    #region Helper Properties

    /// <summary>
    /// Gets UserId.
    /// </summary>
    protected int UserId
    {
      get
      {
        if (DataRow != null)
        {
          return Convert.ToInt32(DataRow["UserID"]);
        }

        return 0;
      }
    }

    /// <summary>
    /// Gets MessageId.
    /// </summary>
    protected int MessageId
    {
      get
      {
        if (DataRow != null)
        {
          return Convert.ToInt32(DataRow["MessageID"]);
        }

        return 0;
      }
    }

    #endregion

    /// <summary>
    /// Gets ParentPage.
    /// </summary>
    public ForumPage ParentPage
    {
      get
      {
        return this._parentPage;
      }
    }


    /// <summary>
    /// Gets or sets DataRow.
    /// </summary>
    public DataRowView DataRow
    {
      get
      {
        return this._row;
      }

      set
      {
        this._row = value;

        // get all flags for forum, topic and message
        if (this._row != null)
        {
          this._forumFlags = new ForumFlags(this._row["ForumFlags"]);
          this._topicFlags = new TopicFlags(this._row["TopicFlags"]);
          this._messageFlags = new MessageFlags(this._row["Flags"]);
        }
        else
        {
          this._forumFlags = new ForumFlags(0);
          this._topicFlags = new TopicFlags(0);
          this._messageFlags = new MessageFlags(0);
        }
      }
    }

    /// <summary>
    /// Gets a value indicating whether IsGuest.
    /// </summary>
    public bool IsGuest
    {
      get
      {
        if (DataRow != null)
        {
          return UserMembershipHelper.IsGuestUser(UserId);
        }
        else
        {
          return true;
        }
      }
    }

    /// <summary>
    /// IsLocked flag should only be used for "ghost" posts such as the
    /// Sponser post that isn't really there.
    /// </summary>
    public bool IsLocked
    {
      get
      {
        if (this._messageFlags != null)
        {
          return this._messageFlags.IsLocked;
        }

        return false;
      }
    }

    /// <summary>
    /// Gets UserProfile.
    /// </summary>
    public YafUserProfile UserProfile
    {
      get
      {
        if (this._userProfile == null)
        {
          // setup instance of the user profile...
          this._userProfile = YafUserProfile.GetProfile(UserMembershipHelper.GetUserNameFromID(UserId));
        }

        return this._userProfile;
      }
    }

    /*private MessageFlags PostMessageFlags
		{
			get
			{
				return new MessageFlags( Convert.ToInt32( DataRow ["Flags"] ) );
			}
		}*/

    /// <summary>
    /// Gets a value indicating whether IsSponserMessage.
    /// </summary>
    protected bool IsSponserMessage
    {
      get
      {
        return DataRow["IP"].ToString() == "none";
      }
    }

    /// <summary>
    /// Gets a value indicating whether CanThankPost.
    /// </summary>
    protected bool CanThankPost
    {
      get
      {
        return (int) DataRow["UserID"] != PageContext.PageUserID;
      }
    }

    /// <summary>
    /// Gets a value indicating whether CanEditPost.
    /// </summary>
    protected bool CanEditPost
    {
      get
      {
        // Ederon : 9/9/2007 - moderaotrs can edit locked posts
        // Ederon : 12/5/2007 - new flags implementation
        return ((!PostLocked && !this._forumFlags.IsLocked && !this._topicFlags.IsLocked && UserId == PageContext.PageUserID) ||
                PageContext.ForumModeratorAccess) && PageContext.ForumEditAccess;
      }
    }

    /// <summary>
    /// Gets a value indicating whether PostLocked.
    /// </summary>
    private bool PostLocked
    {
      get
      {
        // post is explicitly locked
        if (this._messageFlags.IsLocked)
        {
          return true;
        }

        // there is auto-lock period defined
        if (!PageContext.IsAdmin && PageContext.BoardSettings.LockPosts > 0)
        {
          var edited = (DateTime) DataRow["Edited"];

          // check if post is locked according to this rule
          if (edited.AddDays(PageContext.BoardSettings.LockPosts) < DateTime.Now)
          {
            return true;
          }
        }

        return false;
      }
    }

    /// <summary>
    /// Gets a value indicating whether PostDeleted.
    /// </summary>
    private bool PostDeleted
    {
      get
      {
        return this._messageFlags.IsDeleted;
      }
    }

    /// <summary>
    /// Gets a value indicating whether CanAttach.
    /// </summary>
    protected bool CanAttach
    {
      get
      {
        // Ederon : 9/9/2007 - moderaotrs can attack to locked posts
        return ((!PostLocked && !this._forumFlags.IsLocked && !this._topicFlags.IsLocked && UserId == PageContext.PageUserID) ||
                PageContext.ForumModeratorAccess) && PageContext.ForumUploadAccess;
      }
    }

    /// <summary>
    /// Gets a value indicating whether CanDeletePost.
    /// </summary>
    protected bool CanDeletePost
    {
      get
      {
        // Ederon : 9/9/2007 - moderaotrs can delete in locked posts
        return ((!PostLocked && !this._forumFlags.IsLocked && !this._topicFlags.IsLocked && UserId == PageContext.PageUserID) ||
                PageContext.ForumModeratorAccess) && PageContext.ForumDeleteAccess;
      }
    }

    /// <summary>
    /// Gets a value indicating whether CanUnDeletePost.
    /// </summary>
    public bool CanUnDeletePost
    {
      get
      {
        return PostDeleted && CanDeletePost;
      }
    }

    /// <summary>
    /// Gets a value indicating whether CanReply.
    /// </summary>
    protected bool CanReply
    {
      get
      {
        // Ederon : 9/9/2007 - moderaotrs can reply in locked posts
        return ((!this._messageFlags.IsLocked && !this._forumFlags.IsLocked && !this._topicFlags.IsLocked) || PageContext.ForumModeratorAccess) &&
               PageContext.ForumReplyAccess;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether IsAlt.
    /// </summary>
    public bool IsAlt
    {
      get
      {
        return this._isAlt;
      }

      set
      {
        this._isAlt = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether IsThreaded.
    /// </summary>
    public bool IsThreaded
    {
      get
      {
        return this._isThreaded;
      }

      set
      {
        this._isThreaded = value;
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
            ReportButtons.Visible = false;
       
      Utility.RegisterTypeForAjax(typeof (ThankYou));

      string AddThankBoxHTML =
        "'<a class=\"yaflittlebutton\" href=\"javascript:addThanks(' + res.value.MessageID + ');\" onclick=\"this.blur();\" title=' + res.value.Title + '><span>' + res.value.Text + '</span></a>'";

      string RemoveThankBoxHTML =
        "'<a class=\"yaflittlebutton\" href=\"javascript:removeThanks(' + res.value.MessageID + ');\" onclick=\"this.blur();\" title=' + res.value.Title + '><span>' + res.value.Text + '</span></a>'";

      YafContext.Current.PageElements.RegisterJsBlockStartup("addThanksJs", JavaScriptBlocks.addThanksJs(RemoveThankBoxHTML));
      YafContext.Current.PageElements.RegisterJsBlockStartup("removeThanksJs", JavaScriptBlocks.removeThanksJs(AddThankBoxHTML));
      YafContext.Current.PageElements.RegisterJsBlockStartup("asynchCallFailedJs", JavaScriptBlocks.asynchCallFailedJs);

      this.PopMenu1.Visible = !IsGuest;
      if (this.PopMenu1.Visible)
      {
        this.PopMenu1.ItemClick += new PopEventHandler(PopMenu1_ItemClick);
        this.PopMenu1.AddPostBackItem("userprofile", PageContext.Localization.GetText("POSTS", "USERPROFILE"));

        if (PageContext.IsAdmin)
        {
          this.PopMenu1.AddPostBackItem("edituser", "Edit User (Admin)");
        }

        if (!PageContext.IsGuest)
        {
          if (IsIgnored(UserId))
          {
            this.PopMenu1.AddPostBackItem("toggleuserposts_show", PageContext.Localization.GetText("POSTS", "TOGGLEUSERPOSTS_SHOW"));
          }
          else
          {
            this.PopMenu1.AddPostBackItem("toggleuserposts_hide", PageContext.Localization.GetText("POSTS", "TOGGLEUSERPOSTS_HIDE"));
          }
        }

        this.PopMenu1.Attach(this.UserProfileLink);
      }

      // setup jQuery, LightBox and YAF JS...
      YafContext.Current.PageElements.RegisterJQuery();
      YafContext.Current.PageElements.RegisterJsResourceInclude("yafjs", "js/yaf.js");
      YafContext.Current.PageElements.RegisterJsBlock("toggleMessageJs", JavaScriptBlocks.ToggleMessageJs);

      // lightbox only need if the browser is not IE6...
      if (!UserAgentHelper.IsBrowserIE6())
      {
        YafContext.Current.PageElements.RegisterJsResourceInclude("lightboxjs", "js/jquery.lightbox.min.js");
        YafContext.Current.PageElements.RegisterCssIncludeResource("css/jquery.lightbox.css");
        YafContext.Current.PageElements.RegisterJsBlock("lightboxloadjs", JavaScriptBlocks.LightBoxLoadJs);
      }

      this.NameCell.ColSpan = int.Parse(GetIndentSpan());

      if (PageContext.IsGuest)
      {
        this.btnTogglePost.Visible = false;
      }
      else if (IsIgnored(UserId))
      {
        this.btnTogglePost.Visible = true;
        this.btnTogglePost.Attributes["onclick"] = string.Format("toggleMessage('{0}'); return false;", this.panMessage.ClientID);
        this.panMessage.Style["display"] = "none";
      }     
    }

    /// <summary>
    /// The display post_ pre render.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void DisplayPost_PreRender(object sender, EventArgs e)
    {
      this.Attach.Visible = !PostDeleted && CanAttach && !IsLocked;

      this.Attach.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.attachments, "m={0}", MessageId);
      this.Edit.Visible = !PostDeleted && CanEditPost && !IsLocked;
      this.Edit.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.postmessage, "m={0}", MessageId);
      this.MovePost.Visible = PageContext.ForumModeratorAccess && !IsLocked;
      this.MovePost.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.movemessage, "m={0}", MessageId);
      this.Delete.Visible = !PostDeleted && CanDeletePost && !IsLocked;
      this.Delete.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.deletemessage, "m={0}&action=delete", MessageId);
      this.UnDelete.Visible = CanUnDeletePost && !IsLocked;
      this.UnDelete.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.deletemessage, "m={0}&action=undelete", MessageId);
      this.Quote.Visible = !PostDeleted && CanReply && !IsLocked;
      this.Quote.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
        ForumPages.postmessage, "t={0}&f={1}&q={2}", PageContext.PageTopicID, PageContext.PageForumID, MessageId);

      this.Thank.Visible = CanThankPost && !PageContext.IsGuest && YafContext.Current.BoardSettings.EnableThanksMod;
      if (Convert.ToBoolean(DataRow["IsThankedByUser"]) == true)
      {
        this.Thank.NavigateUrl = "javascript:removeThanks(" + DataRow["MessageID"] + ");";
        this.Thank.TextLocalizedTag = "BUTTON_THANKSDELETE";
        this.Thank.TitleLocalizedTag = "BUTTON_THANKSDELETE_TT";
      }
      else
      {
        this.Thank.NavigateUrl = "javascript:addThanks(" + DataRow["MessageID"] + ");";
        this.Thank.TextLocalizedTag = "BUTTON_THANKS";
        this.Thank.TitleLocalizedTag = "BUTTON_THANKS_TT";
      }

      int thanksNumber = Convert.ToInt32(DataRow["MessageThanksNumber"]);
      if (thanksNumber != 0)
      {
        this.Literal2.Text = FormatThanksInfo(DataRow["ThanksInfo"].ToString());
        if (thanksNumber == 1)
        {
          this.Literal1.Text = String.Format(PageContext.Localization.GetText("THANKSINFOSINGLE"), UserProfile.UserName);
        }
        else
        {
          this.Literal1.Text = String.Format(PageContext.Localization.GetText("THANKSINFO"), thanksNumber, UserProfile.UserName);
        }
      }

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
      this.Pm.Visible = !IsGuest && !PostDeleted && PageContext.User != null && PageContext.BoardSettings.AllowPrivateMessages && !IsSponserMessage;
      this.Pm.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.pmessage, "u={0}", UserId);

      // emailing
      this.Email.Visible = !IsGuest && !PostDeleted && PageContext.User != null && PageContext.BoardSettings.AllowEmailSending && !IsSponserMessage;
      this.Email.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_email, "u={0}", UserId);

      // home page
      this.Home.Visible = !PostDeleted && !String.IsNullOrEmpty(UserProfile.Homepage);
      SetupThemeButtonWithLink(this.Home, UserProfile.Homepage);

      // blog page
      this.Blog.Visible = !PostDeleted && !String.IsNullOrEmpty(UserProfile.Blog);
      SetupThemeButtonWithLink(this.Blog, UserProfile.Blog);

      // MSN
      this.Msn.Visible = !PostDeleted && PageContext.User != null && !String.IsNullOrEmpty(UserProfile.MSN);
      this.Msn.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_msn, "u={0}", UserId);

      // Yahoo IM
      this.Yim.Visible = !PostDeleted && PageContext.User != null && !String.IsNullOrEmpty(UserProfile.YIM);
      this.Yim.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_yim, "u={0}", UserId);

      // AOL IM
      this.Aim.Visible = !PostDeleted && PageContext.User != null && !String.IsNullOrEmpty(UserProfile.AIM);
      this.Aim.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_aim, "u={0}", UserId);

      // ICQ
      this.Icq.Visible = !PostDeleted && PageContext.User != null && !String.IsNullOrEmpty(UserProfile.ICQ);
      this.Icq.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_icq, "u={0}", UserId);

      // Skype
      this.Skype.Visible = !PostDeleted && PageContext.User != null && !String.IsNullOrEmpty(UserProfile.Skype);
      this.Skype.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_skype, "u={0}", UserId);

      if (!PostDeleted)
      {
        this.AdminInformation.InnerHtml = @"<span class=""smallfont"">";
        if (Convert.ToDateTime(DataRow["Edited"]) > Convert.ToDateTime(DataRow["Posted"]).AddSeconds(PageContext.BoardSettings.EditTimeOut))
        {
          // message has been edited
          // show, why the post was edited or deleted?
          string whoChanged = Convert.ToBoolean(DataRow["IsModeratorChanged"])
                                ? PageContext.Localization.GetText("EDITED_BY_MOD")
                                : PageContext.Localization.GetText("EDITED_BY_USER");
          this.AdminInformation.InnerHtml += String.Format(
            @"| <span class=""editedinfo"">{0} {1}:</span> {2}", 
            PageContext.Localization.GetText("EDITED"), 
            whoChanged, 
            YafServices.DateTime.FormatDateTimeShort(Convert.ToDateTime(DataRow["Edited"])));
          if (Server.HtmlDecode(Convert.ToString(DataRow["EditReason"])) != string.Empty)
          {
            // reason was specified
            this.AdminInformation.InnerHtml += String.Format(
              " |<b> {0}:</b> {1}", PageContext.Localization.GetText("EDIT_REASON"), FormatMsg.RepairHtml((string) DataRow["EditReason"], true));
          }
          else
          {
            // reason was not specified
            this.AdminInformation.InnerHtml += String.Format(
              " |<b> {0}:</b> {1}", PageContext.Localization.GetText("EDIT_REASON"), PageContext.Localization.GetText("EDIT_REASON_NA"));
          }
        }
      }
      else
      {
        this.AdminInformation.InnerHtml = @"<span class=""smallfont"">";
        if (Server.HtmlDecode(Convert.ToString(DataRow["DeleteReason"])) != String.Empty)
        {
          // reason was specified
          this.AdminInformation.InnerHtml += String.Format(
            " |<b> {0}:</b> {1}", PageContext.Localization.GetText("EDIT_REASON"), FormatMsg.RepairHtml((string) DataRow["DeleteReason"], true));
        }
        else
        {
          // reason was not specified
          this.AdminInformation.InnerHtml += String.Format(
            " |<b> {0}:</b> {1}", PageContext.Localization.GetText("EDIT_REASON"), PageContext.Localization.GetText("EDIT_REASON_NA"));
        }
      }

      // display admin only info
      if (PageContext.IsAdmin)
      {
        this.AdminInformation.InnerHtml += String.Format(" |<b> {0}:</b> {1}", PageContext.Localization.GetText("IP"), DataRow["IP"].ToString());
      }

      this.AdminInformation.InnerHtml += "</span>";
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
      PreRender += new EventHandler(DisplayPost_PreRender);
      Init += new EventHandler(DisplayPost_Init);
      base.OnInit(e);
    }

    /// <summary>
    /// The display post_ init.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void DisplayPost_Init(object sender, EventArgs e)
    {
      // retrieves parent page (ForumPage)
      RetrieveParentPage();
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

    /// <summary>
    /// Retrieve parent page (ForumPage) if it exists.
    /// </summary>
    protected void RetrieveParentPage()
    {
      // get parent page (ForumPage type), if applicable
      Control parent = Parent;

      // cycle until no there is no parent 
      while (parent != null)
      {
        // is parent control of desired type?
        if (parent is ForumPage)
        {
          this._parentPage = (ForumPage) parent;
          break;
        }
        else
        {
          // go one step up in hierarchy
          parent = parent.Parent;

          // are we topmost?
          if (parent == null)
          {
            this._parentPage = null;
            break;
          }
        }
      }
    }

    /// <summary>
    /// Formats the dvThanksInfo section.
    /// </summary>
    /// <param name="rawStr">
    /// The raw Str.
    /// </param>
    /// <returns>
    /// The format thanks info.
    /// </returns>
    protected string FormatThanksInfo(string rawStr)
    {
      var filler = new StringBuilder();
      string strID, strUserName;
      string strDate = string.Empty;

      // Extract all user IDs, usernames and (If enabled thanks dates) related to this message.
      while (rawStr != string.Empty)
      {
        if (filler.Length > 0)
        {
          filler.Append(",&nbsp;");
        }

        int i = rawStr.IndexOf(",");

        // Extract UserID
        strID = rawStr.Substring(0, i);

        // Get the username related to this User ID
        strUserName = UserMembershipHelper.GetUserNameFromID(Convert.ToInt32(strID));
        rawStr = rawStr.Remove(0, i + 1);

        // If Thanks date is in the data, extract it.
        if (YafContext.Current.BoardSettings.ShowThanksDate)
        {
          i = rawStr.IndexOf(",");
          strDate = rawStr.Substring(0, i);
          rawStr = rawStr.Remove(0, i + 1);
        }

        filler.AppendFormat(@"<a id=""{0}"" href=""{1}""><u>{2}</u></a>", strID, YafBuildLink.GetLink(ForumPages.profile, "u={0}", strID), strUserName);

        // If showing thanks date is enabled, add it to the formatted string.
        if (YafContext.Current.BoardSettings.ShowThanksDate)
        {
          filler.AppendFormat(
            @" {0}", 
            String.Format(YafContext.Current.Localization.GetText("DEFAULT", "ONDATE"), YafServices.DateTime.FormatDateShort(Convert.ToDateTime(strDate))));
        }
      }

      return filler.ToString();
    }

    /// <summary>
    /// The get indent cell.
    /// </summary>
    /// <returns>
    /// The get indent cell.
    /// </returns>
    protected string GetIndentCell()
    {
      if (!IsThreaded)
      {
        return string.Empty;
      }

      var iIndent = (int) DataRow["Indent"];
      if (iIndent > 0)
      {
        return string.Format(
          @"<td rowspan=""3"" width=""1%""><img src=""{1}images/spacer.gif"" width=""{0}"" height=""2"" alt=""""/></td>", iIndent * 32, YafForumInfo.ForumRoot);
      }
      else
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// The get indent span.
    /// </summary>
    /// <returns>
    /// The get indent span.
    /// </returns>
    protected string GetIndentSpan()
    {
      if (!IsThreaded || (int) DataRow["Indent"] == 0)
      {
        return "2";
      }
      else
      {
        return "1";
      }
    }

    /// <summary>
    /// The get post class.
    /// </summary>
    /// <returns>
    /// The get post class.
    /// </returns>
    protected string GetPostClass()
    {
      if (IsAlt)
      {
        return "post_alt";
      }
      else
      {
        return "post";
      }
    }

    // Prevents a high user box when displaying a deleted post.

    /// <summary>
    /// The get user box height.
    /// </summary>
    /// <returns>
    /// The get user box height.
    /// </returns>
    protected string GetUserBoxHeight()
    {
      if (PostDeleted)
      {
        return "0";
      }

      return "100";
    }

    /// <summary>
    /// The is ignored.
    /// </summary>
    /// <param name="ignoredUserId">
    /// The ignored user id.
    /// </param>
    /// <returns>
    /// The is ignored.
    /// </returns>
    private bool IsIgnored(int ignoredUserId)
    {
      if (this._userIgnoreList == null)
      {
        this._userIgnoreList = YafServices.DBBroker.UserIgnoredList(PageContext.PageUserID);
      }

      if (this._userIgnoreList.Count > 0)
      {
        return this._userIgnoreList.Contains(ignoredUserId);
      }

      return false;
    }

    /// <summary>
    /// The clear ignore cache.
    /// </summary>
    private void ClearIgnoreCache()
    {
      // clear for the session
      string key = YafCache.GetBoardCacheKey(String.Format(Constants.Cache.UserIgnoreList, PageContext.PageUserID));
      Session.Remove(key);
    }

    /// <summary>
    /// The add ignored.
    /// </summary>
    /// <param name="ignoredUserId">
    /// The ignored user id.
    /// </param>
    private void AddIgnored(int ignoredUserId)
    {
      DB.user_addignoreduser(PageContext.PageUserID, ignoredUserId);
      ClearIgnoreCache();
    }

    /// <summary>
    /// The remove ignored.
    /// </summary>
    /// <param name="ignoredUserId">
    /// The ignored user id.
    /// </param>
    private void RemoveIgnored(int ignoredUserId)
    {
      DB.user_removeignoreduser(PageContext.PageUserID, ignoredUserId);
      ClearIgnoreCache();
    }

    /// <summary>
    /// The pop menu 1_ item click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void PopMenu1_ItemClick(object sender, PopEventArgs e)
    {
      switch (e.Item)
      {
        case "userprofile":
          YafBuildLink.Redirect(ForumPages.profile, "u={0}", UserId);
          break;
        case "edituser":
          YafBuildLink.Redirect(ForumPages.admin_edituser, "u={0}", UserId);
          break;
        case "toggleuserposts_show":
          RemoveIgnored(UserId);
          Response.Redirect(Request.RawUrl);
          break;
        case "toggleuserposts_hide":
          AddIgnored(UserId);
          Response.Redirect(Request.RawUrl);
          break;
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
          DB.message_report(reportFlag, e.CommandArgument.ToString(), PageContext.PageUserID, DateTime.Today, reportMessage);
      else
          Response.Redirect(YAF.Classes.Utils.YafBuildLink.GetLinkNotEscaped(ForumPages.reportpost, "m={0}", e.CommandArgument.ToString()));
     
         // Response.Redirect(YAF.Classes.Utils.YafBuildLink.GetLink(ForumPages.posts, "m={0}#post{0}", DataRow["MessageID"]));
      PageContext.AddLoadMessage(PageContext.Localization.GetText("REPORTEDFEEDBACK"));
    }
  }
}