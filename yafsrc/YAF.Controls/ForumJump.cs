/* Yet Another Forum.NET
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
using System;
using System.Collections.Specialized;
using System.Data;
using System.Web.UI;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Data;
using YAF.Classes.Utils;

namespace YAF.Controls
{
  /// <summary>
  /// Summary description for ForumJump.
  /// </summary>
  public class ForumJump : BaseControl, IPostBackDataHandler
  {
    /// <summary>
    /// Gets or sets ForumID.
    /// </summary>
    private int ForumID
    {
      get
      {
        return (int) ViewState["ForumID"];
      }

      set
      {
        ViewState["ForumID"] = value;
      }
    }

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
    public virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection)
    {
      int forumID;
      if (int.TryParse(postCollection[postDataKey], out forumID) && forumID != ForumID)
      {
        ForumID = forumID;
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
      if (ForumID > 0)
      {
        YafBuildLink.Redirect(ForumPages.topics, "f={0}", ForumID);
      }
      else
      {
        YafBuildLink.Redirect(ForumPages.forum, "c={0}", -ForumID);
      }
    }

    #endregion

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void Page_Load(object sender, EventArgs e)
    {
      if (!Page.IsPostBack)
      {
        ForumID = PageContext.PageForumID;
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
      Load += new EventHandler(Page_Load);
      base.OnInit(e);
    }

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render(HtmlTextWriter writer)
    {
      string cacheKey =
        YafCache.GetBoardCacheKey(String.Format(Constants.Cache.ForumJump, PageContext.User != null ? PageContext.PageUserID.ToString() : "Guest"));
      DataTable dataTable;
      if (PageContext.Cache[cacheKey] != null)
      {
        dataTable = (DataTable) PageContext.Cache[cacheKey];
      }
      else
      {
        dataTable = DB.forum_listall_sorted(PageContext.PageBoardID, PageContext.PageUserID);
        PageContext.Cache.Insert(cacheKey, dataTable, null, DateTime.Now.AddMinutes(5), TimeSpan.Zero);
      }

      writer.WriteLine(
        String.Format(@"<select name=""{0}"" onchange=""{1}"" id=""{2}"">", UniqueID, Page.ClientScript.GetPostBackClientHyperlink(this, ID), ClientID));

      int forumID = PageContext.PageForumID;
      if (forumID <= 0)
      {
        writer.WriteLine("<option/>");
      }

      foreach (DataRow row in dataTable.Rows)
      {
        writer.WriteLine(
          string.Format(
            @"<option {2}value=""{0}"">{1}</option>", 
            row["ForumID"], 
            HtmlEncode(row["Title"]), 
            Convert.ToString(row["ForumID"]) == forumID.ToString() ? @"selected=""selected"" " : string.Empty));
      }

      writer.WriteLine("</select>");
    }
  }
}