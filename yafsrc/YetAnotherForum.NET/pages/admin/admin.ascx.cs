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

namespace YAF.Pages.Admin
{
  using System;
  using System.Data;
  using System.Web.UI.WebControls;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// Summary description for main.
  /// </summary>
  public partial class admin : AdminPage
  {
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
      if (!IsPostBack)
      {
        this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink("Administration", string.Empty);

        // bind data
        BindBoardsList();
        BindData();

        // TODO UpgradeNotice.Visible = install._default.GetCurrentVersion() < Data.AppVersion;
      }
    }

    /// <summary>
    /// The delete_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Delete_Load(object sender, EventArgs e)
    {
      ((LinkButton) sender).Attributes["onclick"] = "return confirm('Delete this User?')";
    }

    /// <summary>
    /// The approve_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Approve_Load(object sender, EventArgs e)
    {
      ((LinkButton) sender).Attributes["onclick"] = "return confirm('Approve this User?')";
    }

    /// <summary>
    /// The delete all_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void DeleteAll_Load(object sender, EventArgs e)
    {
      ((Button) sender).Attributes["onclick"] = "return confirm('Delete all Unapproved Users more than 14 days old?')";
    }

    /// <summary>
    /// The approve all_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ApproveAll_Load(object sender, EventArgs e)
    {
      ((Button) sender).Attributes["onclick"] = "return confirm('Approve all Users?')";
    }

    /// <summary>
    /// Bind list of boards to dropdown
    /// </summary>
    private void BindBoardsList()
    {
      // only if user is hostadmin, otherwise boards' list is hidden
      if (PageContext.IsHostAdmin)
      {
        DataTable dt = DB.board_list(null);

        // add row for "all boards" (null value)
        DataRow r = dt.NewRow();

        r["BoardID"] = -1;
        r["Name"] = " - All Boards -";

        dt.Rows.InsertAt(r, 0);

        // set datasource
        this.BoardStatsSelect.DataSource = dt;
        this.BoardStatsSelect.DataBind();

        // select current board as default
        this.BoardStatsSelect.SelectedIndex = this.BoardStatsSelect.Items.IndexOf(this.BoardStatsSelect.Items.FindByValue(PageContext.PageBoardID.ToString()));
      }
    }


    /// <summary>
    /// Gets board ID for which to show statistics.
    /// </summary>
    /// <returns>
    /// Returns ID of selected board (for host admin), ID of current board (for admin), null if all boards is selected.
    /// </returns>
    private object GetSelectedBoardID()
    {
      // check dropdown only if user is hostadmin
      if (PageContext.IsHostAdmin)
      {
        // -1 means all boards are selected
        if (this.BoardStatsSelect.SelectedValue == "-1")
        {
          return null;
        }
        else
        {
          return this.BoardStatsSelect.SelectedValue;
        }
      }
        
        // for non host admin user, return board he's logged on
      else
      {
        return PageContext.PageBoardID;
      }
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      
        DataTable activeList =  DB.active_list(
        PageContext.PageBoardID, true, PageContext.BoardSettings.ActiveListTime, PageContext.BoardSettings.UseStyledNicks);
        // Set colorOnly parameter to false, as we get active users style from database        
        if (PageContext.BoardSettings.UseStyledNicks)
        {
            new StyleTransform(PageContext.Theme).DecodeStyleByTable(ref activeList,false);
        }
        this.ActiveList.DataSource = activeList;
        this.UserList.DataSource = DB.user_list(PageContext.PageBoardID, null, false);
        DataBind();

      // get stats for current board, selected board or all boards (see function)
      DataRow row = DB.board_stats(GetSelectedBoardID());

      this.NumPosts.Text = String.Format("{0:N0}", row["NumPosts"]);
      this.NumTopics.Text = String.Format("{0:N0}", row["NumTopics"]);
      this.NumUsers.Text = String.Format("{0:N0}", row["NumUsers"]);

      TimeSpan span = DateTime.Now - (DateTime) row["BoardStart"];
      double days = span.Days;

      this.BoardStart.Text = String.Format("{0:d} ({1:N0} days ago)", row["BoardStart"], days);

      if (days < 1)
      {
        days = 1;
      }

      this.DayPosts.Text = String.Format("{0:N2}", SqlDataLayerConverter.VerifyInt32(row["NumPosts"]) / days);
      this.DayTopics.Text = String.Format("{0:N2}", SqlDataLayerConverter.VerifyInt32(row["NumTopics"]) / days);
      this.DayUsers.Text = String.Format("{0:N2}", SqlDataLayerConverter.VerifyInt32(row["NumUsers"]) / days);

      this.DBSize.Text = String.Format("{0} MB", DB.DBSize);
    }

    /// <summary>
    /// The user list_ item command.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    public void UserList_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
      switch (e.CommandName)
      {
        case "edit":
          YafBuildLink.Redirect(ForumPages.admin_edituser, "u={0}", e.CommandArgument);
          break;
        case "delete":
          UserMembershipHelper.DeleteUser(Convert.ToInt32(e.CommandArgument));
          BindData();
          break;
        case "approve":
          UserMembershipHelper.ApproveUser(Convert.ToInt32(e.CommandArgument));
          BindData();
          break;
        case "deleteall":
          UserMembershipHelper.DeleteAllUnapproved(DateTime.Now.AddDays(-14));

          // YAF.Classes.Data.DB.user_deleteold( PageContext.PageBoardID );
          BindData();
          break;
        case "approveall":
          UserMembershipHelper.ApproveAll();

          // YAF.Classes.Data.DB.user_approveall( PageContext.PageBoardID );
          BindData();
          break;
      }
    }

    /// <summary>
    /// The board stats select_ changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    public void BoardStatsSelect_Changed(object sender, EventArgs e)
    {
      // re-bind data
      BindData();
    }

    /// <summary>
    /// The format forum link.
    /// </summary>
    /// <param name="ForumID">
    /// The forum id.
    /// </param>
    /// <param name="ForumName">
    /// The forum name.
    /// </param>
    /// <returns>
    /// The format forum link.
    /// </returns>
    protected string FormatForumLink(object ForumID, object ForumName)
    {
      if (ForumID.ToString() == string.Empty || ForumName.ToString() == string.Empty)
      {
        return string.Empty;
      }

      return String.Format("<a target=\"_top\" href=\"{0}\">{1}</a>", YafBuildLink.GetLink(ForumPages.topics, "f={0}", ForumID), ForumName);
    }

    /// <summary>
    /// The format topic link.
    /// </summary>
    /// <param name="TopicID">
    /// The topic id.
    /// </param>
    /// <param name="TopicName">
    /// The topic name.
    /// </param>
    /// <returns>
    /// The format topic link.
    /// </returns>
    protected string FormatTopicLink(object TopicID, object TopicName)
    {
      if (TopicID.ToString() == string.Empty || TopicName.ToString() == string.Empty)
      {
        return string.Empty;
      }

      return String.Format("<a target=\"_top\" href=\"{0}\">{1}</a>", YafBuildLink.GetLink(ForumPages.posts, "t={0}", TopicID), TopicName);
    }
  }
}