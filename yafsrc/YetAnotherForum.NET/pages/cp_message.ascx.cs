/* Yet Another Forum.net
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2008 Jaben Cargman
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
using System.Web.UI.WebControls;
using YAF.Classes.Base;
using YAF.Classes.Data;
using YAF.Classes.UI;
using YAF.Classes.Utils;

namespace YAF.Pages // YAF.Pages
{
	/// <summary>
	/// Summary description for inbox.
	/// </summary>
	public partial class cp_message : ForumPage
	{
		public cp_message()
			: base( "CP_MESSAGE" )
		{
		}

		protected void Page_Load( object sender, EventArgs e )
		{
			if ( User == null )
				YafBuildLink.Redirect( ForumPages.login, "ReturnUrl={0}", General.GetSafeRawUrl() );

			// check if this feature is disabled
			if ( !PageContext.BoardSettings.AllowPrivateMessages )
				YafBuildLink.Redirect( ForumPages.info, "i=5" );

			if ( !IsPostBack )
			{
				if ( String.IsNullOrEmpty( Request.QueryString ["pm"] ) )
					YafBuildLink.AccessDenied();

				PageLinks.AddLink( PageContext.BoardSettings.Name, YafBuildLink.GetLink( ForumPages.forum ) );
				PageLinks.AddLink( PageContext.PageUserName, YafBuildLink.GetLink( ForumPages.cp_profile ) );

				// handle custom BBCode javascript or CSS...
				BBCode.RegisterCustomBBCodePageElements( Page, this.GetType() );

				BindData();
			}
		}

		protected bool IsOutbox
		{
			get
			{
				if ( ViewState ["IsOutbox"] == null )
					return false;
				else
					return ( bool )ViewState ["IsOutbox"];
			}
			set { ViewState ["IsOutbox"] = value; }
		}

		protected bool IsArchived
		{
			get
			{
				if ( ViewState ["IsArchived"] == null )
					return false;
				else
					return ( bool )ViewState ["IsArchived"];
			}
			set { ViewState ["IsArchived"] = value; }
		}

		/// <summary>
		/// Sets the IsOutbox property as appropriate for this private message.
		/// </summary>
		/// <remarks>User id parameters are downcast to object to allow for potential future use of non-integer user id's</remarks>
		/// <param name="fromUserId">User id of the message sender</param>
		/// <param name="toUserId">User id of the message receiver</param>
		/// <param name="messageIsInOutbox">Bool indicating whether the message is in the sender's outbox</param>
		private void SetMessageView( object fromUserID, object toUserID, bool messageIsInOutbox, bool messageIsArchived )
		{
			bool isCurrentUserFrom = fromUserID.Equals( PageContext.PageUserID );
			bool isCurrentUserTo = toUserID.Equals( PageContext.PageUserID );

			// check if it's the same user...
			if ( isCurrentUserFrom && isCurrentUserTo )
			{
				// it is... handle the view based on the query string passed
				IsOutbox = Request.QueryString ["v"] == "out";
				IsArchived = Request.QueryString ["v"] == "arch";

				// see if the message got deleted, if so, redirect to their outbox/archive
				if ( IsOutbox && !messageIsInOutbox ) YafBuildLink.Redirect( ForumPages.cp_pm, "v=out" );
				else if ( IsArchived && !messageIsArchived ) YafBuildLink.Redirect( ForumPages.cp_pm, "v=arch" );
			}
			else if ( isCurrentUserFrom )
			{
				// see if it's been deleted by the from user...
				if ( !messageIsInOutbox )
				{
					// deleted for this user, redirect...
					YafBuildLink.Redirect( ForumPages.cp_pm, "v=out" );
				}
				else
				{
					// nope
					IsOutbox = true;
				}
			}
			else if ( isCurrentUserTo )
			{
				// get the status for the receiver
				IsArchived = messageIsArchived;
				IsOutbox = false;
			}
		}

		private void BindData()
		{
			using ( DataTable dt = DB.pmessage_list( Security.StringToLongOrRedirect( Request.QueryString ["pm"] ) ) )
			{
				if ( dt.Rows.Count > 0 )
				{
					DataRow row = dt.Rows [0];

					// if the pm isn't from or two the current user--then it's access denied
					if ( ( int )row ["ToUserID"] != PageContext.PageUserID &&
							( int )row ["FromUserID"] != PageContext.PageUserID )
						YafBuildLink.AccessDenied();

					SetMessageView( row ["FromUserID"], row ["ToUserID"], Convert.ToBoolean( row ["IsInOutbox"] ), Convert.ToBoolean( row ["IsArchived"] ) );

					// get the return link to the pm listing
					if ( IsOutbox )
						PageLinks.AddLink( GetText( "SENTITEMS" ), YafBuildLink.GetLink( ForumPages.cp_pm, "v=out" ) );
					else if ( IsArchived )
						PageLinks.AddLink( GetText( "ARCHIVE" ), YafBuildLink.GetLink( ForumPages.cp_pm, "v=arch" ) );
					else
						PageLinks.AddLink( GetText( "INBOX" ), YafBuildLink.GetLink( ForumPages.cp_pm ) );

					PageLinks.AddLink( row ["Subject"].ToString() );

					Inbox.DataSource = dt;
				}
				else
					YafBuildLink.Redirect( ForumPages.cp_pm );
			}

			DataBind();

			if ( !IsOutbox )
				DB.pmessage_markread( Request.QueryString ["pm"] );
		}

		protected void Inbox_ItemCommand( object source, RepeaterCommandEventArgs e )
		{
			if ( e.CommandName == "delete" )
			{
				if ( IsOutbox )
					DB.pmessage_delete( e.CommandArgument, true );
				else
					DB.pmessage_delete( e.CommandArgument );

				BindData();
				PageContext.AddLoadMessage( GetText( "msg_deleted" ) );
			}
			else if ( e.CommandName == "reply" )
			{
				YafBuildLink.Redirect( ForumPages.pmessage, "p={0}&q=0", e.CommandArgument );
			}
			else if ( e.CommandName == "quote" )
			{
				YafBuildLink.Redirect( ForumPages.pmessage, "p={0}&q=1", e.CommandArgument );
			}
		}

		protected void ThemeButtonDelete_Load( object sender, EventArgs e )
		{
			YAF.Controls.ThemeButton themeButton = (YAF.Controls.ThemeButton) sender;
			themeButton.Attributes ["onclick"] = String.Format( "return confirm('{0}')", GetText( "confirm_deletemessage" ) );
		}
	}
}