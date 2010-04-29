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
namespace YAF.Controls
{
  #region Using

  using System;
  using System.Data;
  using System.Text;

  using AjaxPro;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;
  using YAF.Utilities;

  #endregion

  /// <summary>
  /// Summary description for DisplayPost.
  /// </summary>
  public partial class DisplayPost : BaseUserControl
  {
    #region Constants and Fields

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
    /// The current Post Data for this post.
    /// </summary>
    private PostDataHelperWrapper _postDataHelperWrapper = null;

    /// <summary>
    /// The _topic flags.
    /// </summary>
    private TopicFlags _topicFlags;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the DataRow.
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
    /// Gets Post Data helper functions.
    /// </summary>
    public PostDataHelperWrapper PostData
    {
      get
      {
        return this._postDataHelperWrapper;
      }
    }

    #endregion

    #region Methods

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
        strID = rawStr.Substring(0, i).Trim();
     
        // Get the username related to this User ID
        strUserName = this.PageContext.UserDisplayName.GetName(Convert.ToInt32(strID));
        rawStr = rawStr.Remove(0, i + 1);

        // If Thanks date is in the data, extract it.
        if (YafContext.Current.BoardSettings.ShowThanksDate)
        {
          i = rawStr.IndexOf(",");
          strDate = rawStr.Substring(0, i);
          rawStr = rawStr.Remove(0, i + 1);
        }

        filler.AppendFormat(
          @"<a id=""Usr{0}"" href=""{1}""><u>{2}</u></a>", 
          String.Format("{0}_{1}", strID, Guid.NewGuid()), 
          YafBuildLink.GetLink(ForumPages.profile, "u={0}", strID), 
          Server.HtmlEncode(strUserName));

        // If showing thanks date is enabled, add it to the formatted string.
        if (YafContext.Current.BoardSettings.ShowThanksDate)
        {
        filler.AppendFormat(
          @" {0}",
          String.Format(YafContext.Current.Localization.GetText("DEFAULT", "ONDATE"),
          YafServices.DateTime.FormatDateShort(Convert.ToDateTime(strDate))));
        }
      }

      return filler.ToString();
    }

    /// <summary>
    /// The get indent cell.
    /// </summary>
    /// <returns>
    /// Returns indent cell.
    /// </returns>
    protected string GetIndentCell()
    {
      if (!this.IsThreaded)
      {
        return string.Empty;
      }

      var indent = (int)this.DataRow["Indent"];
      if (indent > 0)
      {
        return
          string.Format(
            @"<td rowspan=""3"" width=""1%""><img src=""{1}images/spacer.gif"" width=""{0}"" height=""2"" alt=""""/></td>", 
            indent * 32, 
            YafForumInfo.ForumClientFileRoot);
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
      if (!this.IsThreaded || (int)this.DataRow["Indent"] == 0)
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
      if (this.IsAlt)
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
      if (this.PostData.PostDeleted)
      {
        return "0";
      }

      return "100";
    }

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit(EventArgs e)
    {
      this.PreRender += new EventHandler(this.DisplayPost_PreRender);
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
      Utility.RegisterTypeForAjax(typeof(ThankYou));

      string addThankBoxHTML =
        "'<a class=\"yaflittlebutton\" href=\"javascript:addThanks(' + res.value.MessageID + ');\" onclick=\"this.blur();\" title=' + res.value.Title + '><span>' + res.value.Text + '</span></a>'";

      string removeThankBoxHTML =
        "'<a class=\"yaflittlebutton\" href=\"javascript:removeThanks(' + res.value.MessageID + ');\" onclick=\"this.blur();\" title=' + res.value.Title + '><span>' + res.value.Text + '</span></a>'";

      YafContext.Current.PageElements.RegisterJsBlockStartup(
        "addThanksJs", JavaScriptBlocks.addThanksJs(removeThankBoxHTML));
      YafContext.Current.PageElements.RegisterJsBlockStartup(
        "removeThanksJs", JavaScriptBlocks.removeThanksJs(addThankBoxHTML));
      YafContext.Current.PageElements.RegisterJsBlockStartup("asynchCallFailedJs", JavaScriptBlocks.asynchCallFailedJs);

      this.PopMenu1.Visible = !this.IsGuest;
      if (this.PopMenu1.Visible)
      {
        this.PopMenu1.ItemClick += new PopEventHandler(this.PopMenu1_ItemClick);
        this.PopMenu1.AddPostBackItem("userprofile", this.PageContext.Localization.GetText("POSTS", "USERPROFILE"));

        if (YafContext.Current.BoardSettings.EnableThanksMod)
        {
          this.PopMenu1.AddPostBackItem("viewthanks", this.PageContext.Localization.GetText("VIEWTHANKS", "TITLE"));
        }

        if (this.PageContext.IsAdmin)
        {
          this.PopMenu1.AddPostBackItem("edituser", "Edit User (Admin)");
        }

        if (!this.PageContext.IsGuest)
        {
          if (YafServices.UserIgnored.IsIgnored(this.PostData.UserId))
          {
            this.PopMenu1.AddPostBackItem(
              "toggleuserposts_show", this.PageContext.Localization.GetText("POSTS", "TOGGLEUSERPOSTS_SHOW"));
          }
          else
          {
            this.PopMenu1.AddPostBackItem(
              "toggleuserposts_hide", this.PageContext.Localization.GetText("POSTS", "TOGGLEUSERPOSTS_HIDE"));
          }
        }

        if (YafContext.Current.BoardSettings.EnableBuddyList &&
            this.PageContext.PageUserID != (int)this.DataRow["UserID"])
        {
          // Should we add the "Add Buddy" item?
          if (!YafBuddies.IsBuddy((int)this.DataRow["UserID"], false) && !this.PageContext.IsGuest)
          {
            this.PopMenu1.AddPostBackItem("addbuddy", this.PageContext.Localization.GetText("BUDDY", "ADDBUDDY"));
          }
            
          // Are the users approved buddies? Add the "Remove buddy" item.
          else if (YafBuddies.IsBuddy((int)this.DataRow["UserID"], true) && !this.PageContext.IsGuest)
          {
            this.PopMenu1.AddClientScriptItemWithPostback(
              this.PageContext.Localization.GetText("BUDDY", "REMOVEBUDDY"), 
              "removebuddy", 
              "if (confirm('Remove Buddy?')) {postbackcode}");
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

      this.NameCell.ColSpan = int.Parse(this.GetIndentSpan());
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
      if (this.PageContext.IsGuest)
      {
        this.PostFooter.TogglePost.Visible = false;
      }
      else if (YafServices.UserIgnored.IsIgnored(this.PostData.UserId))
      {
        this.panMessage.Attributes["style"] = "display:none";
        this.PostFooter.TogglePost.Visible = true;
        this.PostFooter.TogglePost.Attributes["onclick"] = string.Format(
          "toggleMessage('{0}'); return false;", this.panMessage.ClientID);
      }
      else if (!YafServices.UserIgnored.IsIgnored(this.PostData.UserId))
      {
        this.panMessage.Attributes["style"] = "display:block";
        this.panMessage.Visible = true;
      }

      this.Attach.Visible = !this.PostData.PostDeleted && this.PostData.CanAttach && !this.PostData.IsLocked;
      this.Attach.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.attachments, "m={0}", this.PostData.MessageId);
      this.Edit.Visible = !this.PostData.PostDeleted && this.PostData.CanEditPost && !this.PostData.IsLocked;
      this.Edit.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.postmessage, "m={0}", this.PostData.MessageId);
      this.MovePost.Visible = this.PageContext.ForumModeratorAccess && !this.PostData.IsLocked;
      this.MovePost.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
        ForumPages.movemessage, "m={0}", this.PostData.MessageId);
      this.Delete.Visible = !this.PostData.PostDeleted && this.PostData.CanDeletePost && !this.PostData.IsLocked;
      this.Delete.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
        ForumPages.deletemessage, "m={0}&action=delete", this.PostData.MessageId);
      this.UnDelete.Visible = this.PostData.CanUnDeletePost && !this.PostData.IsLocked;
      this.UnDelete.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
        ForumPages.deletemessage, "m={0}&action=undelete", this.PostData.MessageId);
      this.Quote.Visible = !this.PostData.PostDeleted && this.PostData.CanReply && !this.PostData.IsLocked;
      this.Quote.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
        ForumPages.postmessage, 
        "t={0}&f={1}&q={2}", 
        this.PageContext.PageTopicID, 
        this.PageContext.PageForumID, 
        this.PostData.MessageId);

      this.Thank.Visible = this.PostData.CanThankPost && !this.PageContext.IsGuest &&
                           YafContext.Current.BoardSettings.EnableThanksMod;
      if (Convert.ToBoolean(this.DataRow["IsThankedByUser"]) == true)
      {
        this.Thank.NavigateUrl = "javascript:removeThanks(" + this.DataRow["MessageID"] + ");";
        this.Thank.TextLocalizedTag = "BUTTON_THANKSDELETE";
        this.Thank.TitleLocalizedTag = "BUTTON_THANKSDELETE_TT";
      }
      else
      {
        this.Thank.NavigateUrl = "javascript:addThanks(" + this.DataRow["MessageID"] + ");";
        this.Thank.TextLocalizedTag = "BUTTON_THANKS";
        this.Thank.TitleLocalizedTag = "BUTTON_THANKS_TT";
      }

      int thanksNumber = Convert.ToInt32(this.DataRow["MessageThanksNumber"]);
      if (thanksNumber != 0)
      {
        this.Literal2.Text = this.FormatThanksInfo(this.DataRow["ThanksInfo"].ToString());
        if (thanksNumber == 1)
        {
          this.Literal1.Text = String.Format(
            this.PageContext.Localization.GetText("THANKSINFOSINGLE"), Server.HtmlEncode(this.PostData.UserProfile.UserName));
        }
        else
        {
          this.Literal1.Text = String.Format(
            this.PageContext.Localization.GetText("THANKSINFO"), thanksNumber, Server.HtmlEncode(this.PostData.UserProfile.UserName));
        }
      }
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
          YafBuildLink.Redirect(ForumPages.profile, "u={0}", this.PostData.UserId);
          break;
        case "addbuddy":
          var strBuddyRequest = new string[2];
          this.PopMenu1.RemovePostBackItem("addbuddy");
          strBuddyRequest = YafBuddies.AddBuddyRequest(this.PostData.UserId);
          if (Convert.ToBoolean(strBuddyRequest[1]))
          {
            this.PageContext.AddLoadMessage(
              string.Format(
                this.PageContext.Localization.GetText("NOTIFICATION_BUDDYAPPROVED_MUTUAL"), strBuddyRequest[0]));
            this.PopMenu1.AddClientScriptItemWithPostback(
              this.PageContext.Localization.GetText("BUDDY", "REMOVEBUDDY"), 
              "removebuddy", 
              "if (confirm('Remove Buddy?')) {postbackcode}");
          }
          else
          {
            this.PageContext.AddLoadMessage(this.PageContext.Localization.GetText("NOTIFICATION_BUDDYREQUEST"));
          }

          break;
        case "removebuddy":
          {
            this.PopMenu1.RemovePostBackItem("removebuddy");
            this.PopMenu1.AddPostBackItem("addbuddy", this.PageContext.Localization.GetText("BUDDY", "ADDBUDDY"));
            this.PageContext.AddLoadMessage(
              string.Format(
                this.PageContext.Localization.GetText("REMOVEBUDDY_NOTIFICATION"), 
                YafBuddies.RemoveBuddy(this.PostData.UserId)));
            break;
          }

        case "edituser":
          YafBuildLink.Redirect(ForumPages.admin_edituser, "u={0}", this.PostData.UserId);
          break;
        case "toggleuserposts_show":
          YafServices.UserIgnored.RemoveIgnored(this.PostData.UserId);
          this.Response.Redirect(this.Request.RawUrl);
          break;
        case "toggleuserposts_hide":
          YafServices.UserIgnored.AddIgnored(this.PostData.UserId);
          this.Response.Redirect(this.Request.RawUrl);
          break;
        case "viewthanks":
          YafBuildLink.Redirect(ForumPages.viewthanks, "u={0}", this.PostData.UserId);
          break;
      }
    }

    #endregion
  }
}