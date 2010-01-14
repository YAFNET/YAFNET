/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages // YAF.Pages
{
	/// <summary>
	/// Summary description for approve.
	/// </summary>
	public partial class approve : YAF.Classes.Core.ForumPage
	{

		public approve()
			: base( "APPROVE" )
		{
		}

		protected void Page_Load( object sender, System.EventArgs e )
		{
			// if already logged in..
			if ( User != null )
			{
				// redirect to front page...
				YafBuildLink.Redirect( ForumPages.forum );
			}

			if ( !IsPostBack )
			{
				PageLinks.AddLink( PageContext.BoardSettings.Name, YafBuildLink.GetLink( ForumPages.forum ) );
				PageLinks.AddLink( GetText( "TITLE" ), "" );

				ValidateKey.Text = GetText( "validate" );
				if ( Request.QueryString ["k"] != null )
				{
					key.Text = Request.QueryString ["k"];
					ValidateKey_Click( sender, e );
				}
				else
				{
					approved.Visible = false;
					error.Visible = !approved.Visible;
				}
			}
		}

		public void ValidateKey_Click( object sender, System.EventArgs e )
		{
      DataTable dt = YAF.Classes.Data.DB.checkemail_update( key.Text );
      DataRow row = dt.Rows [0];
      string dbEmail = row ["Email"].ToString();

      bool keyVerified = ( row["ProviderUserKey"] == DBNull.Value ) ? false : true;

 			approved.Visible = keyVerified;
			error.Visible = !keyVerified;

			if ( keyVerified )
			{
        // approve and update e-mail in the membership as well...
        System.Web.Security.MembershipUser user = UserMembershipHelper.GetMembershipUserByKey( row ["ProviderUserKey"] );
        if (!user.IsApproved) user.IsApproved = true;
        // update the email if anything was returned...
        if (user.Email != dbEmail && dbEmail != "" ) user.Email = dbEmail;
        // tell the provider to update...
				PageContext.CurrentMembership.UpdateUser( user );

        // now redirect to login...
				PageContext.LoadMessage.AddSession( GetText( "EMAIL_VERIFIED" ) );

        YafBuildLink.Redirect( ForumPages.login );
			}
		}

		public override bool IsProtected
		{
			get { return false; }
		}
	}
}
