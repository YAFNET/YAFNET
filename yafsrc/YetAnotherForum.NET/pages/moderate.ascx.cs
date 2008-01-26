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

namespace YAF.Pages // YAF.Pages
{
	/// <summary>
	/// Summary description for moderate.
	/// </summary>
	public partial class moderate0 : YAF.Classes.Base.ForumPage
	{

		public moderate0()
			: base( "MODERATE" )
		{
		}

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( !PageContext.ForumModeratorAccess )
				YafBuildLink.AccessDenied();

			if ( !IsPostBack )
			{
				AddUser.Text = GetText( "INVITE" );

				if ( PageContext.Settings.LockedForum == 0 )
				{
					PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
					PageLinks.AddLink( PageContext.PageCategoryName, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum, "c={0}", PageContext.PageCategoryID ) );
				}
				PageLinks.AddForumLinks( PageContext.PageForumID );
				PageLinks.AddLink( GetText( "TITLE" ), "" );
			}
			BindData();
		}

		private void AddUser_Click( object sender, System.EventArgs e )
		{
			YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.mod_forumuser, "f={0}", PageContext.PageForumID );
		}

		protected void Delete_Load( object sender, System.EventArgs e )
		{
			( ( ThemeButton )sender ).Attributes ["onclick"] = String.Format( "return confirm('{0}')", GetText( "confirm_delete" ) );
		}

		protected void DeleteUser_Load( object sender, System.EventArgs e )
		{
			( ( LinkButton )sender ).Attributes ["onclick"] = String.Format( "return confirm('{0}')", "Remove this user from this forum?" );
		}

		private void BindData()
		{
			topiclist.DataSource = YAF.Classes.Data.DB.topic_list( PageContext.PageForumID, -1, null, 0, 999999 );
			UserList.DataSource = YAF.Classes.Data.DB.userforum_list( null, PageContext.PageForumID );
			DataBind();
		}

		private void topiclist_ItemCommand( object sender, RepeaterCommandEventArgs e )
		{
			if ( e.CommandName == "delete" )
			{
				YAF.Classes.Data.DB.topic_delete( e.CommandArgument );
				PageContext.AddLoadMessage( GetText( "deleted" ) );
				BindData();
			}
		}

		private void UserList_ItemCommand( object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e )
		{
			switch ( e.CommandName )
			{
				case "edit":
					YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.mod_forumuser, "f={0}&u={1}", PageContext.PageForumID, e.CommandArgument );
					break;
				case "remove":
					YAF.Classes.Data.DB.userforum_delete( e.CommandArgument, PageContext.PageForumID );
					BindData();
					// clear moderatorss cache
					YafCache.Current.Remove(YafCache.GetBoardCacheKey(Constants.Cache.ForumModerators));
					break;
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit( EventArgs e )
		{
			topiclist.ItemCommand += new RepeaterCommandEventHandler( topiclist_ItemCommand );
			UserList.ItemCommand += new System.Web.UI.WebControls.RepeaterCommandEventHandler( this.UserList_ItemCommand );
			AddUser.Click += new EventHandler( AddUser_Click );
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
