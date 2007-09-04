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

namespace YAF.Pages // YAF.Pages
{
	/// <summary>
	/// Summary description for active.
	/// </summary>
	public partial class im_email : YAF.Classes.Base.ForumPage
	{

		public im_email()
			: base( "IM_EMAIL" )
		{
		}

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( User == null || !PageContext.BoardSettings.AllowEmailSending )
				YafBuildLink.AccessDenied();

			if ( !IsPostBack )
			{
				using ( DataTable dt = YAF.Classes.Data.DB.user_list( PageContext.PageBoardID, Request.QueryString ["u"], null ) )
				{
					foreach ( DataRow row in dt.Rows )
					{
						PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
						PageLinks.AddLink( row ["Name"].ToString(), YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.profile, "u={0}", row ["UserID"] ) );
						PageLinks.AddLink( GetText( "TITLE" ), "" );
						break;
					}
				}
				Send.Text = GetText( "SEND" );
			}
		}

		private void Send_Click( object sender, EventArgs e )
		{
			try
			{
				string from = string.Empty, to = string.Empty;
				string fromName = string.Empty, toName = string.Empty;
				using ( DataTable dt = YAF.Classes.Data.DB.user_list( PageContext.PageBoardID, Request.QueryString ["u"], null ) )
				{
					foreach ( DataRow row in dt.Rows )
					{
						to = row ["Email"].ToString();
						toName = row ["Name"].ToString();
						break;
					}
				}
				using ( DataTable dt = YAF.Classes.Data.DB.user_list( PageContext.PageBoardID, PageContext.PageUserID, null ) )
				{
					foreach ( DataRow row in dt.Rows )
					{
						from = row ["Email"].ToString();
						fromName = row ["Name"].ToString();
						break;
					}
				}
				General.SendMail( from, fromName, to, toName, Subject.Text, Body.Text );
				YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.profile, "u={0}", Request.QueryString ["u"] );
			}
			catch ( Exception x )
			{
				YAF.Classes.Data.DB.eventlog_create( PageContext.PageUserID, this, x );
				if ( PageContext.IsAdmin )
					PageContext.AddLoadMessage( x.Message );
				else
					PageContext.AddLoadMessage( GetText( "ERROR" ) );
			}
		}

		override protected void OnInit( EventArgs e )
		{
			base.OnInit( e );
		}
	}
}
