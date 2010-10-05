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
namespace YAF.Controls
{
  using System;
  using System.Collections;
  using System.Data;
  using System.Web.UI.WebControls;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Utils;

  /// <summary>
  /// The forum sub forum list.
  /// </summary>
  public partial class ForumSubForumList : BaseUserControl
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ForumSubForumList"/> class.
    /// </summary>
    public ForumSubForumList()
      : base()
    {
    }

    /// <summary>
    /// Sets DataSource.
    /// </summary>
    public IEnumerable DataSource
    {
      set
      {
        this.SubforumList.DataSource = value;
      }
    }

    /// <summary>
    /// The subforum list_ item created.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void SubforumList_ItemCreated(object sender, RepeaterItemEventArgs e)
    {
      if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
      {
        var row = (DataRow) e.Item.DataItem;
        DateTime lastRead = YafContext.Current.Get<YafSession>().GetForumRead((int) row["ForumID"]);
        DateTime lastPosted = row["LastPosted"] != DBNull.Value ? (DateTime) row["LastPosted"] : lastRead;

        var subForumIcon = e.Item.FindControl("ThemeSubforumIcon") as ThemeImage;

        if (subForumIcon != null)
        {
          subForumIcon.ThemeTag = "SUBFORUM";
          subForumIcon.LocalizedTitlePage = "ICONLEGEND";
          subForumIcon.LocalizedTitleTag = "NO_NEW_POSTS";

          try
          {
            if (lastPosted > lastRead)
            {
              subForumIcon.ThemeTag = "SUBFORUM_NEW";
              subForumIcon.LocalizedTitlePage = "ICONLEGEND";
              subForumIcon.LocalizedTitleTag = "NEW_POSTS";
            }
          }
          catch
          {
          }
        }
      }
    }

    /// <summary>
    /// Provides the "Forum Link Text" for the ForumList control.
    /// Automatically disables the link if the current user doesn't
    /// have proper permissions.
    /// </summary>
    /// <param name="row">
    /// Current data row
    /// </param>
    /// <returns>
    /// Forum link text
    /// </returns>
    public string GetForumLink(DataRow row)
    {
      string output = string.Empty;
      int forumID = Convert.ToInt32(row["ForumID"]);

      // get the Forum Description
      output = Convert.ToString(row["Forum"]);

      if (int.Parse(row["ReadAccess"].ToString()) > 0)
      {
        output = "<a href=\"{0}\">{1}</a>".FormatWith(YafBuildLink.GetLink(ForumPages.topics, "f={0}", forumID), output);
      }
      else
      {
        // no access to this forum
        output = "{0} {1}".FormatWith(output, this.PageContext.Localization.GetText("NO_FORUM_ACCESS"));
      }

      return output;
    }
  }
}