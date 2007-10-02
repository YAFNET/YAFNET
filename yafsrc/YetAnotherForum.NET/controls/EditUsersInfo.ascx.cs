/* Yet Another Forum.NET
 * Copyright (C) 2006-2007 Jaben Cargman
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
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Controls
{
  public partial class EditUsersInfo : YAF.Classes.Base.BaseUserControl
  {
    protected void Page_Load( object sender, EventArgs e )
    {
      IsHostAdminRow.Visible = PageContext.IsHostAdmin;

      if ( !IsPostBack )
      {
        BindData();
        using ( DataTable dt = YAF.Classes.Data.DB.user_list( PageContext.PageBoardID, Request.QueryString ["u"], null ) )
        {
          DataRow row = dt.Rows [0];
          Name.Text = ( string ) row ["Name"];
          Email.Text = row ["Email"].ToString();

		  IsHostAdminX.Checked = General.BinaryAnd(row["Flags"], UserFlags.IsHostAdmin);
		  IsGuestX.Checked = General.BinaryAnd(row["Flags"], UserFlags.IsGuest);
          Joined.Text = row ["Joined"].ToString();

          LastVisit.Text = row ["LastVisit"].ToString();

          ListItem item = RankID.Items.FindByValue( row ["RankID"].ToString() );

          if ( item != null )
            item.Selected = true;
        }
      }
    }

    private void BindData()
    {
      RankID.DataSource = YAF.Classes.Data.DB.rank_list( PageContext.PageBoardID, null );
      RankID.DataValueField = "RankID";
      RankID.DataTextField = "Name";
      DataBind();
    }

    protected void Cancel_Click( object sender, System.EventArgs e )
    {
      YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_users );
    }

    protected void Save_Click( object sender, System.EventArgs e )
    {
      // update the membership too...
      if ( !IsGuestX.Checked )
      {
        MembershipUser user = Membership.GetUser( Name.Text );

        if ( Email.Text.Trim() != user.Email )
        {
          // update the e-mail here too...
          user.Email = Email.Text.Trim();
          System.Web.Security.Membership.UpdateUser( user );
        }
      }

      YAF.Classes.Data.DB.user_adminsave( PageContext.PageBoardID, Request.QueryString ["u"], Name.Text, Email.Text, IsHostAdminX.Checked, IsGuestX.Checked, RankID.SelectedValue );
      YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_users );
    }
  }
}