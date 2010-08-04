/* Yet Another Forum.net
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
  using System.Web.UI.WebControls;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Core.BBCode;
  using YAF.Classes.Data;
  using YAF.Classes.UI;
  using YAF.Classes.Utils;
  using YAF.Controls;

  #endregion

  /// <summary>
  /// The cp_message.
  /// </summary>
  public partial class cp_message : ForumPageRegistered
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="cp_message"/> class.
    /// </summary>
    public cp_message()
      : base("CP_MESSAGE")
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets a value indicating whether IsArchived.
    /// </summary>
    protected bool IsArchived
    {
      get
      {
        if (this.ViewState["IsArchived"] == null)
        {
          return false;
        }
        else
        {
          return (bool)this.ViewState["IsArchived"];
        }
      }

      set
      {
        this.ViewState["IsArchived"] = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether IsOutbox.
    /// </summary>
    protected bool IsOutbox
    {
      get
      {
        if (this.ViewState["IsOutbox"] == null)
        {
          return false;
        }
        else
        {
          return (bool)this.ViewState["IsOutbox"];
        }
      }

      set
      {
        this.ViewState["IsOutbox"] = value;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The inbox_ item command.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Inbox_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
      if (e.CommandName == "delete")
      {
        if (this.IsOutbox)
        {
          DB.pmessage_delete(e.CommandArgument, true);
        }
        else
        {
          DB.pmessage_delete(e.CommandArgument);
        }

        this.BindData();
        this.PageContext.AddLoadMessage(this.GetText("msg_deleted"));
      }
      else if (e.CommandName == "reply")
      {
        YafBuildLink.Redirect(ForumPages.pmessage, "p={0}&q=0", e.CommandArgument);
      }
      else if (e.CommandName == "quote")
      {
        YafBuildLink.Redirect(ForumPages.pmessage, "p={0}&q=1", e.CommandArgument);
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
      // check if this feature is disabled
      if (!this.PageContext.BoardSettings.AllowPrivateMessages)
      {
        YafBuildLink.RedirectInfoPage(InfoMessage.Disabled);
      }

      if (!this.IsPostBack)
      {
        if (String.IsNullOrEmpty(this.Request.QueryString.GetFirstOrDefault("pm")))
        {
          YafBuildLink.AccessDenied();
        }

        this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(this.PageContext.PageUserName, YafBuildLink.GetLink(ForumPages.cp_profile));

        // handle custom YafBBCode javascript or CSS...
        YafBBCode.RegisterCustomBBCodePageElements(this.Page, this.GetType());

        this.BindData();
      }
    }

    /// <summary>
    /// The theme button delete_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ThemeButtonDelete_Load(object sender, EventArgs e)
    {
      var themeButton = (ThemeButton)sender;
      themeButton.Attributes["onclick"] = String.Format("return confirm('{0}')", this.GetText("confirm_deletemessage"));
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      using (DataTable dt = DB.pmessage_list(Security.StringToLongOrRedirect(this.Request.QueryString.GetFirstOrDefault("pm"))))
      {
        if (dt.Rows.Count > 0)
        {
          DataRow row = dt.Rows[0];

          // if the pm isn't from or two the current user--then it's access denied
          if ((int)row["ToUserID"] != this.PageContext.PageUserID &&
              (int)row["FromUserID"] != this.PageContext.PageUserID)
          {
            YafBuildLink.AccessDenied();
          }

          this.SetMessageView(
            row["FromUserID"], 
            row["ToUserID"], 
            Convert.ToBoolean(row["IsInOutbox"]), 
            Convert.ToBoolean(row["IsArchived"]));

          // get the return link to the pm listing
          if (this.IsOutbox)
          {
            this.PageLinks.AddLink(this.GetText("SENTITEMS"), YafBuildLink.GetLink(ForumPages.cp_pm, "v=out"));
          }
          else if (this.IsArchived)
          {
            this.PageLinks.AddLink(this.GetText("ARCHIVE"), YafBuildLink.GetLink(ForumPages.cp_pm, "v=arch"));
          }
          else
          {
            this.PageLinks.AddLink(this.GetText("INBOX"), YafBuildLink.GetLink(ForumPages.cp_pm));
          }

          this.PageLinks.AddLink(row["Subject"].ToString());

          this.Inbox.DataSource = dt;
        }
        else
        {
          YafBuildLink.Redirect(ForumPages.cp_pm);
        }
      }

      this.DataBind();

      if (!this.IsOutbox)
      {
        DB.pmessage_markread(this.Request.QueryString.GetFirstOrDefault("pm"));

        // Clearing cache with old permissions data...
        this.PageContext.Cache.Remove(
          YafCache.GetBoardCacheKey(String.Format(Constants.Cache.ActiveUserLazyData, this.PageContext.PageUserID)));
      }
    }

    /// <summary>
    /// Sets the IsOutbox property as appropriate for this private message.
    /// </summary>
    /// <remarks>
    /// User id parameters are downcast to object to allow for potential future use of non-integer user id's
    /// </remarks>
    /// <param name="fromUserID">
    /// The from User ID.
    /// </param>
    /// <param name="toUserID">
    /// The to User ID.
    /// </param>
    /// <param name="messageIsInOutbox">
    /// Bool indicating whether the message is in the sender's outbox
    /// </param>
    /// <param name="messageIsArchived">
    /// The message Is Archived.
    /// </param>
    private void SetMessageView(object fromUserID, object toUserID, bool messageIsInOutbox, bool messageIsArchived)
    {
      bool isCurrentUserFrom = fromUserID.Equals(this.PageContext.PageUserID);
      bool isCurrentUserTo = toUserID.Equals(this.PageContext.PageUserID);

      // check if it's the same user...
      if (isCurrentUserFrom && isCurrentUserTo)
      {
        // it is... handle the view based on the query string passed
        this.IsOutbox = this.Request.QueryString.GetFirstOrDefault("v") == "out";
        this.IsArchived = this.Request.QueryString.GetFirstOrDefault("v") == "arch";

        // see if the message got deleted, if so, redirect to their outbox/archive
        if (this.IsOutbox && !messageIsInOutbox)
        {
          YafBuildLink.Redirect(ForumPages.cp_pm, "v=out");
        }
        else if (this.IsArchived && !messageIsArchived)
        {
          YafBuildLink.Redirect(ForumPages.cp_pm, "v=arch");
        }
      }
      else if (isCurrentUserFrom)
      {
        // see if it's been deleted by the from user...
        if (!messageIsInOutbox)
        {
          // deleted for this user, redirect...
          YafBuildLink.Redirect(ForumPages.cp_pm, "v=out");
        }
        else
        {
          // nope
          this.IsOutbox = true;
        }
      }
      else if (isCurrentUserTo)
      {
        // get the status for the receiver
        this.IsArchived = messageIsArchived;
        this.IsOutbox = false;
      }
    }

    #endregion
  }
}