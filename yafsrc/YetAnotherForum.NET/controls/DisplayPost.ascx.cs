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
    /// The current Post Data for this post.
    /// </summary>
    private PostDataHelperWrapper _postDataHelperWrapper = null;

    /// <summary>
    /// The _topic flags.
    /// </summary>
    private TopicFlags _topicFlags;

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
    /// Gets Post Data helper functions.
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
      Utility.RegisterTypeForAjax(typeof(ThankYou));

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
          if (YafServices.UserIgnored.IsIgnored(PostData.UserId))
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
      if (PageContext.IsGuest)
      {
        this.PostFooter.TogglePost.Visible = false;
      }
      else if (YafServices.UserIgnored.IsIgnored(PostData.UserId))
      {
        this.PostFooter.TogglePost.Visible = true;
        this.PostFooter.TogglePost.Attributes["onclick"] = string.Format("toggleMessage('{0}'); return false;", this.panMessage.ClientID);
      }

      this.Attach.Visible = !PostData.PostDeleted && PostData.CanAttach && !PostData.IsLocked;
      this.Attach.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.attachments, "m={0}", PostData.MessageId);
      this.Edit.Visible = !PostData.PostDeleted && PostData.CanEditPost && !PostData.IsLocked;
      this.Edit.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.postmessage, "m={0}", PostData.MessageId);
      this.MovePost.Visible = PageContext.ForumModeratorAccess && !PostData.IsLocked;
      this.MovePost.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.movemessage, "m={0}", PostData.MessageId);
      this.Delete.Visible = !PostData.PostDeleted && PostData.CanDeletePost && !PostData.IsLocked;
      this.Delete.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.deletemessage, "m={0}&action=delete", PostData.MessageId);
      this.UnDelete.Visible = PostData.CanUnDeletePost && !PostData.IsLocked;
      this.UnDelete.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.deletemessage, "m={0}&action=undelete", PostData.MessageId);
      this.Quote.Visible = !PostData.PostDeleted && PostData.CanReply && !PostData.IsLocked;
      this.Quote.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
        ForumPages.postmessage, "t={0}&f={1}&q={2}", PageContext.PageTopicID, PageContext.PageForumID, PostData.MessageId);

      this.Thank.Visible = PostData.CanThankPost && !PageContext.IsGuest && YafContext.Current.BoardSettings.EnableThanksMod;
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
          this.Literal1.Text = String.Format(PageContext.Localization.GetText("THANKSINFOSINGLE"), PostData.UserProfile.UserName);
        }
        else
        {
          this.Literal1.Text = String.Format(PageContext.Localization.GetText("THANKSINFO"), thanksNumber, PostData.UserProfile.UserName);
        }
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
      PreRender += new EventHandler(DisplayPost_PreRender);
      base.OnInit(e);
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
      if (PostData.PostDeleted)
      {
        return "0";
      }

      return "100";
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
          YafBuildLink.Redirect(ForumPages.profile, "u={0}", PostData.UserId);
          break;
        case "edituser":
          YafBuildLink.Redirect(ForumPages.admin_edituser, "u={0}", PostData.UserId);
          break;
        case "toggleuserposts_show":
          YafServices.UserIgnored.RemoveIgnored(PostData.UserId);
          Response.Redirect(Request.RawUrl);
          break;
        case "toggleuserposts_hide":
          YafServices.UserIgnored.AddIgnored(PostData.UserId);
          Response.Redirect(Request.RawUrl);
          break;
      }
    }
  }
}