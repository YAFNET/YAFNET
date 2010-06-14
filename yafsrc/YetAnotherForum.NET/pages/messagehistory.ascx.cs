/* Yet Another Forum.NET
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
  #region Using

  using System;
  using System.Data;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// Summary description for members.
  /// </summary>
  public partial class messagehistory : ForumPage
  {
    #region Constants and Fields

    /// <summary>
    ///   To save forumid value.
    /// </summary>
    private int forumID;

    /// <summary>
    ///   To save messageid value.
    /// </summary>
    private int messageID;

    /// <summary>
    ///   To save originalRow value.
    /// </summary>
    private DataTable originalRow;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="messagehistory"/> class. 
    ///   Initializes a new instance of the <see cref="members"/> class.
    /// </summary>
    public messagehistory()
      : base("MESSAGEHISTORY")
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Called when the page loads
    /// </summary>
    /// <param name="sender">
    /// </param>
    /// <param name="e">
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
      if (!String.IsNullOrEmpty(this.Request.QueryString["m"]))
      {
        if (!Int32.TryParse(this.Request.QueryString["m"], out this.messageID))
        {
          this.Response.Redirect(YafBuildLink.GetLink(ForumPages.error, "Incorrect message value: {0}", this.messageID));
        }

        this.ReturnBtn.Visible = true;
      }

      if (!String.IsNullOrEmpty(this.Request.QueryString["f"]))
      {
        // We check here if the user have access to the option
        if (this.PageContext.IsGuest)
        {
          this.Response.Redirect(YafBuildLink.GetLinkNotEscaped(ForumPages.info, "i=4"));
        }

        if (!Int32.TryParse(this.Request.QueryString["f"], out this.forumID))
        {
          this.Response.Redirect(YafBuildLink.GetLink(ForumPages.error, "Incorrect forum value: {0}", this.forumID));
        }

        this.ReturnModBtn.Visible = true;
      }

      this.originalRow = DB.message_secdata(this.messageID, this.PageContext.PageUserID);

      if (this.originalRow.Rows.Count <= 0)
      {
        this.Response.Redirect(YafBuildLink.GetLink(ForumPages.error, "Incorrect message value: {0}", this.messageID));
      }

      if (!this.IsPostBack)
      {
        this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);

        this.BindData();
      }
    }

    /// <summary>
    /// The return btn_ on click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ReturnBtn_OnClick(object sender, EventArgs e)
    {
      // Redirect to the changed post
      this.Response.Redirect(YafBuildLink.GetLinkNotEscaped(ForumPages.posts, "m={0}#post{0}", this.messageID));
    }

    /// <summary>
    /// The return mod btn_ on click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ReturnModBtn_OnClick(object sender, EventArgs e)
    {
      // Redirect to the changed post
      this.Response.Redirect(YafBuildLink.GetLinkNotEscaped(ForumPages.moderate_reportedposts, "f={0}", this.forumID));
    }

    /// <summary>
    /// Binds data to data source
    /// </summary>
    private void BindData()
    {
      // Fill revisions list repeater. We set clean-up period to 365 days.
      this.RevisionsList.DataSource =
        DB.messagehistory_list(this.messageID, this.PageContext.BoardSettings.MessageHistoryDaysToLog, true).
          AsEnumerable();

      // Fill current message repeater
      this.CurrentMessageRpt.Visible = true;
      this.CurrentMessageRpt.DataSource = DB.message_secdata(this.messageID, this.PageContext.PageUserID).AsEnumerable();

      this.DataBind();
    }

    #endregion
  }
}