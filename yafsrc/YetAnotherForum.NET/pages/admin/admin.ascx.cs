/* Yet Another Forum.net
 * Copyright (C) 2003 Bjørnar Henden
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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages.Admin
{
  /// <summary>
  /// Summary description for main.
  /// </summary>
  public partial class admin : YAF.Classes.Base.AdminPage
  {

    protected void Page_Load( object sender, System.EventArgs e )
    {
      if ( !IsPostBack )
      {
        PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
        PageLinks.AddLink( "Administration", "" );
        BindData();
        //TODO UpgradeNotice.Visible = install._default.GetCurrentVersion() < Data.AppVersion;
      }
    }

    protected void Delete_Load( object sender, System.EventArgs e )
    {
      ( ( LinkButton ) sender ).Attributes ["onclick"] = "return confirm('Delete this user?')";
    }

    protected void Approve_Load( object sender, System.EventArgs e )
    {
      ( ( LinkButton ) sender ).Attributes ["onclick"] = "return confirm('Approve this user?')";
    }

    private void BindData()
    {
      ActiveList.DataSource = YAF.Classes.Data.DB.active_list( PageContext.PageBoardID, true );
      UserList.DataSource = YAF.Classes.Data.DB.user_list( PageContext.PageBoardID, null, false );
      DataBind();

      DataRow row = YAF.Classes.Data.DB.board_stats();
      NumPosts.Text = String.Format( "{0:N0}", row ["NumPosts"] );
      NumTopics.Text = String.Format( "{0:N0}", row ["NumTopics"] );
      NumUsers.Text = String.Format( "{0:N0}", row ["NumUsers"] );

      TimeSpan span = DateTime.Now - ( DateTime ) row ["BoardStart"];
      double days = span.Days;

      BoardStart.Text = String.Format( "{0:d} ({1:N0} days ago)", row ["BoardStart"], days );

      if ( days < 1 ) days = 1;
      DayPosts.Text = String.Format( "{0:N2}", ( int ) row ["NumPosts"] / days );
      DayTopics.Text = String.Format( "{0:N2}", ( int ) row ["NumTopics"] / days );
      DayUsers.Text = String.Format( "{0:N2}", ( int ) row ["NumUsers"] / days );

      DBSize.Text = String.Format( "{0} MB", YAF.Classes.Data.DB.DBSize() );
    }

    public void UserList_ItemCommand( object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e )
    {
      switch ( e.CommandName )
      {
        case "edit":
          YAF.Classes.Utils.yaf_BuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_edituser, "u={0}", e.CommandArgument );
          break;
        case "delete":
          UserMembershipHelper.DeleteUser( Convert.ToInt32( e.CommandArgument ));
          BindData();
          break;
        case "approve":
          UserMembershipHelper.ApproveUser( Convert.ToInt32( e.CommandArgument ));
          BindData();
          break;
        case "deleteall":
          UserMembershipHelper.DeleteAllUnapproved();
          //YAF.Classes.Data.DB.user_deleteold( PageContext.PageBoardID );
          BindData();
          break;
        case "approveall":
          UserMembershipHelper.ApproveAll();
          //YAF.Classes.Data.DB.user_approveall( PageContext.PageBoardID );
          BindData();
          break;
      }
    }

    protected string FormatForumLink( object ForumID, object ForumName )
    {
      if ( ForumID.ToString() == "" || ForumName.ToString() == "" )
        return "";

      return String.Format( "<a target=\"_top\" href=\"{0}\">{1}</a>", YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.topics, "f={0}", ForumID ), ForumName );
    }

    protected string FormatTopicLink( object TopicID, object TopicName )
    {
      if ( TopicID.ToString() == "" || TopicName.ToString() == "" )
        return "";

      return String.Format( "<a target=\"_top\" href=\"{0}\">{1}</a>", YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.posts, "t={0}", TopicID ), TopicName );
    }
  }
}
