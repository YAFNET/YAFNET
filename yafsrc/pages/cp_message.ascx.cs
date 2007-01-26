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
using YAF.Classes.UI;

namespace YAF.Pages // YAF.Pages
{
	/// <summary>
	/// Summary description for inbox.
	/// </summary>
	public partial class cp_message : YAF.Classes.Base.ForumPage
	{

		public cp_message()
			: base( "CP_MESSAGE" )
		{
		}

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if(User==null)
			{
				if(CanLogin)
					YAF.Classes.Utils.yaf_BuildLink.Redirect( YAF.Classes.Utils.ForumPages.login, "ReturnUrl={0}", General.GetSafeRawUrl() );
				else
					YAF.Classes.Utils.yaf_BuildLink.Redirect( YAF.Classes.Utils.ForumPages.forum );
			}

			if ( !IsPostBack )
				BindData();
		}

		private void BindData()
		{
			if ( Request.QueryString ["pm"] == null )
			{
				yaf_BuildLink.AccessDenied();
				return;
			}

			using ( DataTable dt = YAF.Classes.Data.DB.userpmessage_list( Request.QueryString ["pm"] ) )
			{
				foreach ( DataRow row in dt.Rows )
				{
					if ( ( int ) row ["ToUserID"] != PageContext.PageUserID && ( int ) row ["FromUserID"] != PageContext.PageUserID )
						yaf_BuildLink.AccessDenied();

					PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
					PageLinks.AddLink( PageContext.PageUserName, YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.cp_profile ) );
					if ( ( int ) row ["ToUserID"] == PageContext.PageUserID )
						PageLinks.AddLink( GetText( "INBOX" ), YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.cp_inbox ) );
					else
						PageLinks.AddLink( GetText( "SENTITEMS" ), YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.cp_inbox, "sent=1" ) );
					PageLinks.AddLink( HtmlEncode( row ["Subject"] ), "" );
				}
				Inbox.DataSource = dt;
			}
			DataBind();
			YAF.Classes.Data.DB.pmessage_markread( Request.QueryString ["pm"] );
		}

		protected string FormatBody( object o )
		{
			DataRowView row = ( DataRowView ) o;
			return FormatMsg.FormatMessage( row ["Body"].ToString(), Convert.ToInt32( row ["Flags"] ) );
		}

		private void Inbox_ItemCommand( object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e )
		{
			if ( e.CommandName == "delete" )
			{
				YAF.Classes.Data.DB.userpmessage_delete( e.CommandArgument );
				BindData();
				PageContext.AddLoadMessage( GetText( "msg_deleted" ) );
			}
			else if ( e.CommandName == "reply" )
			{
				YAF.Classes.Utils.yaf_BuildLink.Redirect( YAF.Classes.Utils.ForumPages.pmessage, "p={0}&q=0", e.CommandArgument );
			}
			else if ( e.CommandName == "quote" )
			{
				YAF.Classes.Utils.yaf_BuildLink.Redirect( YAF.Classes.Utils.ForumPages.pmessage, "p={0}&q=1", e.CommandArgument );
			}
		}

		protected void DeleteMessage_Load( object sender, System.EventArgs e )
		{
			( ( LinkButton ) sender ).Attributes ["onclick"] = String.Format( "return confirm('{0}')", GetText( "confirm_deletemessage" ) );
		}

		#region Web Form Designer generated code
		override protected void OnInit( EventArgs e )
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit( e );
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Inbox.ItemCommand += new System.Web.UI.WebControls.RepeaterCommandEventHandler( this.Inbox_ItemCommand );
		}
		#endregion
	}
}
