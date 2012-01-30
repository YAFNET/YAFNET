/* Yet Another Forum.NET
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
namespace YAF.Controls
{
  #region Using

  using System;
  using System.Collections.Specialized;
  using System.Data;
  using System.Web.UI;

  using YAF.Core; using YAF.Types.Interfaces; using YAF.Types.Constants;
  using YAF.Core.Services;
  using YAF.Classes.Data;
  using YAF.Utils;
  using YAF.Types;
  using YAF.Types.Constants;

  #endregion

  /// <summary>
  /// Summary description for ForumJump.
  /// </summary>
  public class ForumJump : BaseControl, IPostBackDataHandler
  {
    #region Properties

    /// <summary>
    ///   Gets or sets ForumID.
    /// </summary>
    private int ForumID
    {
      get
      {
        return (int)this.ViewState["ForumID"];
      }

      set
      {
        this.ViewState["ForumID"] = value;
      }
    }

    #endregion

    #region Implemented Interfaces

    #region IPostBackDataHandler

    /// <summary>
    /// The load post data.
    /// </summary>
    /// <param name="postDataKey">
    /// The post data key.
    /// </param>
    /// <param name="postCollection">
    /// The post collection.
    /// </param>
    /// <returns>
    /// The load post data.
    /// </returns>
    public virtual bool LoadPostData([NotNull] string postDataKey, [NotNull] NameValueCollection postCollection)
    {
      int forumID;
      if (int.TryParse(postCollection[postDataKey], out forumID) && forumID != this.ForumID)
      {
        this.ForumID = forumID;
        return true;
      }

      return false;
    }

    /// <summary>
    /// The raise post data changed event.
    /// </summary>
    public virtual void RaisePostDataChangedEvent()
    {
      // Ederon : 9/4/2007
      if (this.ForumID > 0)
      {
        YafBuildLink.Redirect(ForumPages.topics, "f={0}", this.ForumID);
      }
      else
      {
        YafBuildLink.Redirect(ForumPages.forum, "c={0}", -this.ForumID);
      }
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit([NotNull] EventArgs e)
    {
      this.Load += this.Page_Load;
      base.OnInit(e);
    }

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render([NotNull] HtmlTextWriter writer)
    {
      var forumJump =
        this.Get<IDataCache>().GetOrSet(
          Constants.Cache.ForumJump.FormatWith(
            this.PageContext.User != null ? this.PageContext.PageUserID.ToString() : "Guest"),
          () => LegacyDb.forum_listall_sorted(this.PageContext.PageBoardID, this.PageContext.PageUserID),
          TimeSpan.FromMinutes(5));

      writer.WriteLine(
        @"<select name=""{0}"" onchange=""{1}"" id=""{2}"">".FormatWith(
          this.UniqueID, this.Page.ClientScript.GetPostBackClientHyperlink(this, this.ID), this.ClientID));

      int forumID = this.PageContext.PageForumID;
      if (forumID <= 0)
      {
        writer.WriteLine("<option/>");
      }

      foreach (DataRow row in forumJump.Rows)
      {
        writer.WriteLine(
          @"<option {2}value=""{0}"">{1}</option>".FormatWith(
            row["ForumID"], 
            this.HtmlEncode(row["Title"]), 
            Convert.ToString(row["ForumID"]) == forumID.ToString() ? @"selected=""selected"" " : string.Empty));
      }

      writer.WriteLine("</select>");
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
    private void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (!this.Page.IsPostBack)
      {
        this.ForumID = this.PageContext.PageForumID;
      }
    }

    #endregion
  }
}