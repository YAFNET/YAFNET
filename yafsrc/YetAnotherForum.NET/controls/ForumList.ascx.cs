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
  using System.Web.UI.HtmlControls;
  using System.Web.UI.WebControls;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// 		Summary description for ForumList.
  /// </summary>
  public partial class ForumList : BaseUserControl
  {

    /// <summary>
    /// The Go to last post Image ToolTip.
    /// </summary>
    private string _altLastPost = null;

    /// <summary>
    /// Gets or sets Alt.
    /// </summary>
    public string AltLastPost
    {
        get
        {
            if (string.IsNullOrEmpty(this._altLastPost))
            {
                return string.Empty;
            }
            return this._altLastPost;
        }

        set
        {
            this._altLastPost = value;
        }
    }

    /// <summary>
    /// Sets DataSource.
    /// </summary>
    public IEnumerable DataSource
    {
      set
      {
        this.ForumList1.DataSource = value;
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
        this.AltLastPost = PageContext.Localization.GetText("DEFAULT", "GO_LAST_POST");
    }

    /// <summary>
    /// The forum list 1_ item created.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ForumList1_ItemCreated(object sender, RepeaterItemEventArgs e)
    {
      if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
      {
        var row = (DataRow) e.Item.DataItem;
        var flags = new ForumFlags(row["Flags"]);
        var imageLastPostTT = this.AltLastPost;
        DateTime lastRead = Mession.GetForumRead((int) row["ForumID"]);
        DateTime lastPosted = row["LastPosted"] != DBNull.Value ? (DateTime) row["LastPosted"] : lastRead;
        
          if (string.IsNullOrEmpty(row["ImageUrl"].ToString()))
        {
            var forumIcon = e.Item.FindControl("ThemeForumIcon") as ThemeImage;            
            forumIcon.ThemeTag = "FORUM";
            forumIcon.LocalizedTitlePage = "ICONLEGEND";
            forumIcon.LocalizedTitleTag = "NO_NEW_POSTS";
            forumIcon.Visible = true;
        

        try
        {
          if (flags.IsLocked)
          {
            forumIcon.ThemeTag = "FORUM_LOCKED";
            forumIcon.LocalizedTitlePage = "ICONLEGEND";
            forumIcon.LocalizedTitleTag = "FORUM_LOCKED";
          }
          else if (lastPosted > lastRead)
          {
            forumIcon.ThemeTag = "FORUM_NEW";
            forumIcon.LocalizedTitlePage = "ICONLEGEND";
            forumIcon.LocalizedTitleTag = "NEW_POSTS";
          }
          else
          {
            forumIcon.ThemeTag = "FORUM";
            forumIcon.LocalizedTitlePage = "ICONLEGEND";
            forumIcon.LocalizedTitleTag = "NO_NEW_POSTS";
          }
        }
        catch
        {
        }
        }
          else
          {
              System.Web.UI.HtmlControls.HtmlImage forumImage = e.Item.FindControl("ForumImage1") as System.Web.UI.HtmlControls.HtmlImage;
              forumImage.Src = String.Format("{0}{1}/{2}", YafForumInfo.ForumServerFileRoot, YafBoardFolders.Current.Forums, row["ImageUrl"].ToString());
              
              // TODO: vzrus: needs to be moved to css and converted to a more light control in the future.
              // Highlight custom icon images and add tool tips to them. 
              try
              {
                  forumImage.Attributes.Clear();          

                  if (flags.IsLocked)
                  {                      
                      forumImage.Attributes.Add("class", "forum_customimage_locked");                      
                      forumImage.Attributes.Add("alt", PageContext.Localization.GetText("ICONLEGEND", "FORUM_LOCKED"));
                      forumImage.Attributes.Add("title", PageContext.Localization.GetText("ICONLEGEND", "FORUM_LOCKED"));
                      forumImage.Attributes.Add("src", String.Format("{0}{1}/{2}", YafForumInfo.ForumServerFileRoot, YafBoardFolders.Current.Forums, row["ImageUrl"].ToString()));
                    
                  }
                  else if (lastPosted > lastRead)
                  {                      
                      forumImage.Attributes.Add("class", "forum_customimage_newposts");                    
                      forumImage.Attributes.Add("alt", PageContext.Localization.GetText("ICONLEGEND", "NEW_POSTS"));
                      forumImage.Attributes.Add("title", PageContext.Localization.GetText("ICONLEGEND", "NEW_POSTS"));
                      forumImage.Attributes.Add("src", String.Format("{0}{1}/{2}", YafForumInfo.ForumServerFileRoot, YafBoardFolders.Current.Forums, row["ImageUrl"].ToString()));
                     
                  }
                  else
                  {
                      forumImage.Attributes.Add("class", "forum_customimage_nonewposts"); 
                      forumImage.Attributes.Add("src", String.Format("{0}{1}/{2}", YafForumInfo.ForumServerFileRoot, YafBoardFolders.Current.Forums, row["ImageUrl"].ToString()));                    
                      forumImage.Attributes.Add("alt", PageContext.Localization.GetText("ICONLEGEND", "NO_NEW_POSTS"));
                      forumImage.Attributes.Add("title", PageContext.Localization.GetText("ICONLEGEND", "NO_NEW_POSTS"));                    
                     
                  }

                  forumImage.Visible = true;
                  
                 
              }
              catch
              {
              }      

              forumImage.Visible = true;
          }

        if (!PageContext.BoardSettings.ShowModeratorList)
        {
          // hide moderator list...
          var moderatorColumn = e.Item.FindControl("ModeratorListTD") as HtmlTableCell;
          var modList = e.Item.FindControl("ModeratorList") as ForumModeratorList;

          // set them as invisible...
          moderatorColumn.Visible = false;
          modList.Visible = false;
        }
      }
    }

    // Suppress rendering of footer if there is one or more 
    /// <summary>
    /// The get moderators footer.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <returns>
    /// The get moderators footer.
    /// </returns>
    protected string GetModeratorsFooter(Repeater sender)
    {
      if (sender.DataSource != null && sender.DataSource is DataRow[] && ((DataRow[]) sender.DataSource).Length < 1)
      {
        return "-";
      }
      else
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// The get moderator link.
    /// </summary>
    /// <param name="row">
    /// The row.
    /// </param>
    /// <returns>
    /// The get moderator link.
    /// </returns>
    protected string GetModeratorLink(DataRow row)
    {
      string output;

      if ((int) row["IsGroup"] == 0)
      {
        output = String.Format("<a href=\"{0}\">{1}</a>", YafBuildLink.GetLink(ForumPages.profile, "u={0}", row["ModeratorID"]), row["ModeratorName"]);
      }
      else
      {
        // TODO : group link should point to group info page (yet unavailable)
        /*output = String.Format(
						"<strong><a href=\"{0}\">{1}</a></strong>",
						YafBuildLink.GetLink(ForumPages.forum, "g={0}", row["ModeratorID"]),
						row["ModeratorName"]
						);*/
        output = String.Format("<strong>{0}</strong>", row["ModeratorName"]);
      }

      return output;
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
        output = String.Format("<a href=\"{0}\">{1}</a>", YafBuildLink.GetLink(ForumPages.topics, "f={0}", forumID), output);
      }
      else
      {
        // no access to this forum
        output = String.Format("{0} {1}", output, PageContext.Localization.GetText("NO_FORUM_ACCESS"));
      }

      return output;
    }

    /// <summary>
    /// The topics.
    /// </summary>
    /// <param name="_o">
    /// The _o.
    /// </param>
    /// <returns>
    /// The topics.
    /// </returns>
    protected string Topics(object _o)
    {
      var row = (DataRow) _o;
      if (row["RemoteURL"] == DBNull.Value)
      {
        return string.Format("{0:N0}", row["Topics"]);
      }
      else
      {
        return "-";
      }
    }

    /// <summary>
    /// The posts.
    /// </summary>
    /// <param name="_o">
    /// The _o.
    /// </param>
    /// <returns>
    /// The posts.
    /// </returns>
    protected string Posts(object _o)
    {
      var row = (DataRow) _o;
      if (row["RemoteURL"] == DBNull.Value)
      {
        return string.Format("{0:N0}", row["Posts"]);
      }
      else
      {
        return "-";
      }
    }

    /// <summary>
    /// The get viewing.
    /// </summary>
    /// <param name="o">
    /// The o.
    /// </param>
    /// <returns>
    /// The get viewing.
    /// </returns>
    protected string GetViewing(object o)
    {
      var row = (DataRow) o;
      int nViewing = SqlDataLayerConverter.VerifyInt32(row["Viewing"]);
      if (nViewing > 0)
      {
        return "&nbsp;" + PageContext.Localization.GetTextFormatted("VIEWING", nViewing);
      }
      else
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// The get moderated.
    /// </summary>
    /// <param name="o">
    /// The o.
    /// </param>
    /// <returns>
    /// The get moderated.
    /// </returns>
    protected bool GetModerated(object o)
    {
      return ((DataRow) o)["Flags"].BinaryAnd(ForumFlags.Flags.IsModerated);
    }

    // Ederon : 08/27/2007
    /// <summary>
    /// The has subforums.
    /// </summary>
    /// <param name="row">
    /// The row.
    /// </param>
    /// <returns>
    /// The has subforums.
    /// </returns>
    protected bool HasSubforums(DataRow row)
    {
      return (int) row["Subforums"] > 0;
    }

    /// <summary>
    /// The get subforums.
    /// </summary>
    /// <param name="row">
    /// The row.
    /// </param>
    /// <returns>
    /// </returns>
    protected IEnumerable GetSubforums(DataRow row)
    {
      if (HasSubforums(row))
      {
        return DB.forum_listread(PageContext.PageBoardID, PageContext.PageUserID, row["CategoryID"], row["ForumID"]).Rows;
      }

      return null;
    }
  }
}