/* Yet Another Forum.NET
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
using YAF.Controls;

namespace YAF.Pages.moderate
{
	/// <summary>
	/// Summary description for _default.
	/// </summary>
	public partial class reportedspam : YAF.Classes.Base.ForumPage
	{
		#region Constructors & Overriden Methods

		/// <summary>
		/// Default constructor.
		/// </summary>
		public reportedspam() : base( "MODERATE_FORUM" ) { }


		/// <summary>
		/// Creates page links for this page.
		/// </summary>
		protected override void CreatePageLinks()
		{
			// forum index
			PageLinks.AddLink( PageContext.BoardSettings.Name, YafBuildLink.GetLink( ForumPages.forum ) );
			// moderation index
			PageLinks.AddLink( GetText( "MODERATE_DEFAULT", "TITLE" ), YafBuildLink.GetLink( ForumPages.moderate_index ) );
			// current page
			PageLinks.AddLink( PageContext.PageForumName );
		}

		#endregion


		#region Event Handlers

		/// <summary>
		/// Handles page load event.
		/// </summary>
		protected void Page_Load( object sender, System.EventArgs e )
		{
			// only forum moderators are allowed here
			if ( !PageContext.IsModerator || !PageContext.ForumModeratorAccess ) YafBuildLink.AccessDenied();

			// do this just on page load, not postbacks
			if ( !IsPostBack )
			{
				// create page links
				CreatePageLinks();

				// bind data
				BindData();
			}
		}

		/// <summary>
		/// Handles load event for delete button, adds confirmation dialog.
		/// </summary>
		protected void Delete_Load( object sender, System.EventArgs e )
		{
			ThemeButton button = sender as ThemeButton;
			if ( button != null ) button.Attributes ["onclick"] = String.Format( "return confirm('{0}');", GetText( "ASK_DELETE" ) );
		}

		/// <summary>
		/// Handles post moderation events/buttons.
		/// </summary>
		private void List_ItemCommand( object sender, RepeaterCommandEventArgs e )
		{
			// which command are we handling
			switch ( e.CommandName.ToLower() )
			{
				case "delete":
					// delete message
					DB.message_delete( e.CommandArgument, true, "", 1, true );
					// re-bind data
					BindData();
					// tell user message was deleted
					PageContext.AddLoadMessage( GetText( "DELETED" ) );
					break;
				case "view":
					// go to the message
					YafBuildLink.Redirect( ForumPages.posts, "m={0}", e.CommandArgument );
					break;
				case "copyover":
					// re-bind data
					BindData();
					// update message text
					DB.message_reportcopyover( e.CommandArgument );
					break;
				case "resolved":
					// mark message as resolved
					DB.message_reportresolve( 8, e.CommandArgument, PageContext.PageUserID );
					// re-bind data
					BindData();
					// tell user message was flagged as resolved
					PageContext.AddLoadMessage( GetText( "RESOLVEDFEEDBACK" ) );
					break;
			}

			// see if there are any items left...
			DataTable dt = DB.message_listreported( 8, PageContext.PageForumID );

			if ( dt.Rows.Count == 0 )
			{
				// nope -- redirect back to the moderate main...
				YafBuildLink.Redirect( ForumPages.moderate_index );
			}
		}
		#endregion


		#region Data Binding & Formatting

		/// <summary>
		/// Bind data for this control.
		/// </summary>
		private void BindData()
		{
			// get reported posts for this forum
			List.DataSource = DB.message_listreported( 8, PageContext.PageForumID );

			// bind data to controls
			DataBind();
		}

		#endregion


		#region Web Form Designer generated code
		override protected void OnInit( EventArgs e )
		{
			List.ItemCommand += new RepeaterCommandEventHandler( List_ItemCommand );
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
		}
		#endregion
	}
}
