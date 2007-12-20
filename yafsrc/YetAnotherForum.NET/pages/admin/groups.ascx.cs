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
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages.Admin
{
  /// <summary>
  /// Summary description for groups.
  /// </summary>
  public partial class groups : YAF.Classes.Base.AdminPage
  {
		private System.Collections.Specialized.StringCollection _availableRoles = new System.Collections.Specialized.StringCollection();

    protected void Page_Load( object sender, System.EventArgs e )
    {
      if ( !IsPostBack )
      {
        PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
        PageLinks.AddLink( "Administration", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.admin_admin ) );
        PageLinks.AddLink( "Roles", "" );

        // sync roles just in case...
        YAF.Classes.Utils.RoleMembershipHelper.SyncRoles( YAF.Classes.Utils.YafContext.Current.PageBoardID );

        BindData();
      }
    }

    protected void Delete_Load( object sender, System.EventArgs e )
    {
      ( ( LinkButton ) sender ).Attributes ["onclick"] = "return confirm('Delete this Role?')";
    }

    private void BindData()
    {
			DataTable dt = YAF.Classes.Data.DB.group_list( PageContext.PageBoardID, null );
			RoleListYaf.DataSource = dt;			

			_availableRoles.Clear();

			foreach ( string role in Roles.GetAllRoles() )
			{
				string filter = string.Format( "Name='{0}'", role.Replace("'", "''") );
				DataRow [] rows = dt.Select( filter );

				if ( rows.Length == 0 )
				{
					// doesn't exist in the Yaf Groups
					_availableRoles.Add( role );
				}
			}

			if ( _availableRoles.Count > 0 )
			{
				RoleListNet.DataSource = _availableRoles;
			}
			else
			{
				RoleListNet.DataSource = null;
			}

      DataBind();
    }

    protected void RoleListYaf_ItemCommand( object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e )
    {
      switch ( e.CommandName )
      {
        case "edit":
          YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_editgroup, "i={0}", e.CommandArgument );
          break;
        case "delete":
          YAF.Classes.Data.DB.group_delete( e.CommandArgument );
          BindData();
          break;
      }
    }

		protected void RoleListNet_ItemCommand( object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e )
		{
			switch ( e.CommandName )
			{
				case "add":
					long groupID = YAF.Classes.Data.DB.group_save( DBNull.Value, PageContext.PageBoardID, e.CommandArgument.ToString(), false, false, false, false, 1 );
					YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_editgroup, "i={0}", groupID );
					break;
				case "delete":
					System.Web.Security.Roles.DeleteRole( e.CommandArgument.ToString(), false );
					BindData();
					break;
			}
		}

		protected string GetLinkedStatus( System.Data.DataRowView currentRow )
		{
			if ( BitSet( currentRow ["Flags"], 2 ) == true )
			{
				return "Unlinkable";
			}

			return "Linked";
		}

    protected void NewGroup_Click( object sender, System.EventArgs e )
    {
      YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_editgroup );
    }

    protected bool BitSet( object _o, int bitmask )
    {
      int i = ( int ) _o;
      return ( i & bitmask ) != 0;
    }
  }
}
