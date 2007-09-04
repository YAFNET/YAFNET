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
  /// Summary description for members.
  /// </summary>
  public partial class users : YAF.Classes.Base.AdminPage
  {

    protected void Page_Load( object sender, System.EventArgs e )
    {
      if ( !IsPostBack )
      {
        PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
        PageLinks.AddLink( "Administration", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.admin_admin ) );
        PageLinks.AddLink( "Users", "" );

        using ( DataTable dt = YAF.Classes.Data.DB.group_list( PageContext.PageBoardID, null ) )
        {
          DataRow newRow = dt.NewRow();
          newRow ["Name"] = string.Empty;
          newRow ["GroupID"] = DBNull.Value;
          dt.Rows.InsertAt( newRow, 0 );

          group.DataSource = dt;
          group.DataTextField = "Name";
          group.DataValueField = "GroupID";
          group.DataBind();
        }

        using ( DataTable dt = YAF.Classes.Data.DB.rank_list( PageContext.PageBoardID, null ) )
        {
          DataRow newRow = dt.NewRow();
          newRow ["Name"] = string.Empty;
          newRow ["RankID"] = DBNull.Value;
          dt.Rows.InsertAt( newRow, 0 );

          rank.DataSource = dt;
          rank.DataTextField = "Name";
          rank.DataValueField = "RankID";
          rank.DataBind();
        }
      }
    }

    private void BindData()
    {
      using ( DataTable dt =
            YAF.Classes.Data.DB.user_list( PageContext.PageBoardID, null, null,
            group.SelectedIndex <= 0 ? null : group.SelectedValue,
            rank.SelectedIndex <= 0 ? null : rank.SelectedValue
            ) )
      {
        using ( DataView dv = dt.DefaultView )
        {
          if ( name.Text.Trim().Length > 0 || ( Email.Text.Trim().Length > 0 ) )
            dv.RowFilter = string.Format( "Name like '%{0}%' and Email like '%{1}%'", name.Text.Trim(), Email.Text.Trim() );
          UserList.DataSource = dv;
          UserList.DataBind();
        }
      }
    }

    public void Delete_Load( object sender, System.EventArgs e )
    {
      ( ( LinkButton ) sender ).Attributes ["onclick"] = "return confirm('Delete this user?')";
    }

    public void UserList_ItemCommand( object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e )
    {
      switch ( e.CommandName )
      {
        case "edit":
          YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_edituser, "u={0}", e.CommandArgument );
          break;
        case "delete":
          if ( PageContext.PageUserID == int.Parse( e.CommandArgument.ToString() ) )
          {
            PageContext.AddLoadMessage( "You can't delete yourself." );
            return;
          }
          string userName = string.Empty;
          using ( DataTable dt = YAF.Classes.Data.DB.user_list( PageContext.PageBoardID, e.CommandArgument, DBNull.Value ) )
          {
            foreach ( DataRow row in dt.Rows )
            {
              userName = ( string ) row ["Name"];
              if ( ( int ) row ["IsGuest"] > 0 )
              {
                PageContext.AddLoadMessage( "You can't delete the Guest." );
                return;
              }
              if ( ( int ) row ["IsAdmin"] > 0 || ( int ) row ["IsHostAdmin"] > 0 )
              {
                PageContext.AddLoadMessage( "You can't delete the Admin." );
                return;
              }
            }
          }
          UserMembershipHelper.DeleteUser( Convert.ToInt32(e.CommandArgument) );
          BindData();
          break;
      }
    }

    public void NewUser_Click( object sender, System.EventArgs e )
    {
      YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_reguser );
    }


    public void search_Click( object sender, EventArgs e )
    {
      BindData();
    }

    protected bool BitSet( object _o, int bitmask )
    {
      int i = ( int ) _o;
      return ( i & bitmask ) != 0;
    }
  }
}
