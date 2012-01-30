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
  using System.Data;
  using System.Web.UI;

  using YAF.Core; using YAF.Types.Interfaces; using YAF.Types.Constants;
  using YAF.Classes.Data;
  using YAF.Utils;
  using YAF.Types;
  using YAF.Types.Constants;

  #endregion

  /// <summary>
  /// Summary description for PageLinks.
  /// </summary>
  public class PageLinks : BaseControl
  {
    #region Properties

    /// <summary>
    ///   Gets or sets LinkedPageLinkID.
    /// </summary>
    [CanBeNull]
    public string LinkedPageLinkID
    {
      get
      {
        if (this.ViewState["LinkedPageLinkID"] != null)
        {
          return this.ViewState["LinkedPageLinkID"].ToString();
        }

        return null;
      }

      set
      {
        this.ViewState["LinkedPageLinkID"] = value;
      }
    }

    /// <summary>
    ///   Gets or sets PageLinkDT.
    /// </summary>
    [CanBeNull]
    protected DataTable PageLinkDT
    {
      get
      {
        if (this.ViewState["PageLinkDT"] != null)
        {
          return this.ViewState["PageLinkDT"] as DataTable;
        }

        return null;
      }

      set
      {
        this.ViewState["PageLinkDT"] = value;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The add forum links.
    /// </summary>
    /// <param name="forumID">
    /// The forum id.
    /// </param>
    public void AddForumLinks(int forumID)
    {
      this.AddForumLinks(forumID, false);
    }

    /// <summary>
    /// The add forum links.
    /// </summary>
    /// <param name="forumID">
    /// The forum id.
    /// </param>
    /// <param name="noForumLink">
    /// The no forum link.
    /// </param>
    public void AddForumLinks(int forumID, bool noForumLink)
    {
      using (DataTable dtLinks = LegacyDb.forum_listpath(forumID))
      {
        foreach (DataRow row in dtLinks.Rows)
        {
          if (noForumLink && Convert.ToInt32(row["ForumID"]) == forumID)
          {
            this.AddLink(row["Name"].ToString(), string.Empty);
          }
          else
          {
            this.AddLink(row["Name"].ToString(), YafBuildLink.GetLink(ForumPages.topics, "f={0}", row["ForumID"]));
          }
        }
      }
    }

    /// <summary>
    /// The add link.
    /// </summary>
    /// <param name="title">
    /// The title.
    /// </param>
    public void AddLink([NotNull] string title)
    {
      this.AddLink(title, string.Empty);
    }

    /// <summary>
    /// The add link.
    /// </summary>
    /// <param name="title">
    /// The title.
    /// </param>
    /// <param name="url">
    /// The url.
    /// </param>
    public void AddLink([NotNull] string title, [NotNull] string url)
    {
      DataTable dt = this.PageLinkDT;

      if (dt == null)
      {
        dt = new DataTable();
        dt.Columns.Add("Title", typeof(string));
        dt.Columns.Add("URL", typeof(string));
        this.PageLinkDT = dt;
      }

      DataRow dr = dt.NewRow();
      dr["Title"] = title;
      dr["URL"] = url;
      dt.Rows.Add(dr);
    }

    /// <summary>
    /// Clear all Links
    /// </summary>
    public void Clear()
    {
      if (this.PageLinkDT != null)
      {
        this.PageLinkDT = null;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render([NotNull] HtmlTextWriter writer)
    {
      DataTable linkDataTable = null;

      if (this.LinkedPageLinkID.IsSet())
      {
        // attempt to get access to the other control...
        var plControl = this.Parent.FindControl(this.LinkedPageLinkID) as PageLinks;

        if (plControl != null)
        {
          // use the other data stream...
          linkDataTable = plControl.PageLinkDT;
        }
      }
      else
      {
        // use the data table from this control...
        linkDataTable = this.PageLinkDT;
      }

      if (linkDataTable == null || linkDataTable.Rows.Count == 0)
      {
        return;
      }

      writer.WriteLine(@"<div id=""{0}"" class=""yafPageLink"">".FormatWith(this.ClientID));

      bool bFirst = true;
      foreach (DataRow row in linkDataTable.Rows)
      {
        if (!bFirst)
        {
          writer.WriteLine(@"<span class=""linkSeperator"">&nbsp;&#187;&nbsp;</span>");
        }
        else
        {
          bFirst = false;
        }

        string title = this.HtmlEncode(row["Title"].ToString().Trim());
        string url = row["URL"].ToString().Trim();

        if (url.IsNotSet())
        {
          writer.WriteLine(@"<span class=""currentPageLink"">{0}</span>".FormatWith(title));
        }
        else
        {
          writer.WriteLine(@"<a href=""{0}"">{1}</a>".FormatWith(url, title));
        }
      }

      writer.WriteLine("</div>");
    }

    #endregion
  }
}