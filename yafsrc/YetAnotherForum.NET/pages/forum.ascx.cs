/* Yet Another Forum.NET
 * Copyright (C) 2006-2009 Jaben Cargman
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

namespace YAF.Pages
{
	/// <summary>
	/// Summary description for _default.
	/// </summary>
	public partial class forum : YAF.Classes.Base.ForumPage
	{

		public forum()
			: base( "DEFAULT" )
		{
		}

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( !IsPostBack )
			{
				ForumStats.Visible = PageContext.BoardSettings.ShowForumStatistics;
				ActiveDiscussions.Visible = PageContext.BoardSettings.ShowActiveDiscussions;

				if ( PageContext.IsPrivate && User == null )
				{
					YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.login, "ReturnUrl={0}", Request.RawUrl );
				}

				if ( PageContext.Settings.LockedForum == 0 )
				{
					PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
					if ( PageContext.PageCategoryID != 0 )
					{
						PageLinks.AddLink( PageContext.PageCategoryName, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum, "c={0}", PageContext.PageCategoryID ) );
						Welcome.Visible = false;
					}
				}

				BindData();
			}
		}


		protected void MarkAll_Click( object sender, System.EventArgs e )
		{
			Mession.LastVisit = DateTime.Now;
			BindData();
		}

		private void BindData()
		{
			DataSet ds = YAF.Classes.Utils.DBBroker.board_layout( PageContext.PageBoardID, PageContext.PageUserID, PageContext.PageCategoryID, null );
			CategoryList.DataSource = ds.Tables [DBAccess.GetObjectName("Category")];

			DataBind();
		}

		protected string GetViewing( object o )
		{
			DataRow row = ( DataRow ) o;
			int nViewing = ( int ) row ["Viewing"];
			if ( nViewing > 0 )
				return "&nbsp;" + String.Format( GetText( "VIEWING" ), nViewing );
			else
				return "";
		}

		protected string GetForumIcon( object o )
		{
			DataRow row = ( DataRow ) o;
			bool locked = ( bool ) row ["Locked"];
			DateTime lastRead = Mession.GetForumRead( ( int ) row ["ForumID"] );
			DateTime lastPosted = row ["LastPosted"] != DBNull.Value ? ( DateTime ) row ["LastPosted"] : lastRead;

			string img, imgTitle;

			try
			{
				if ( locked )
				{
					img = GetThemeContents( "ICONS", "FORUM_LOCKED" );
					imgTitle = GetText( "ICONLEGEND", "Forum_Locked" );
				}
				else if ( lastPosted > lastRead )
				{
					img = GetThemeContents( "ICONS", "FORUM_NEW" );
					imgTitle = GetText( "ICONLEGEND", "New_Posts" );
				}
				else
				{
					img = GetThemeContents( "ICONS", "FORUM" );
					imgTitle = GetText( "ICONLEGEND", "No_New_Posts" );
				}
			}
			catch ( Exception )
			{
				img = GetThemeContents( "ICONS", "FORUM" );
				imgTitle = GetText( "ICONLEGEND", "No_New_Posts" );
			}

			return String.Format( "<img src=\"{0}\" alt=\"{1}\" title=\"{1}\"/>", img, imgTitle );
		}

		protected void ModeratorList_ItemCommand( object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e )
		{
			//PageContext.AddLoadMessage("TODO: Fix this");
			//TODO: Show moderators
		}

		protected void OnNeedDataBind( object sender, EventArgs e )
		{
			BindData();
		}

		protected void CollapsibleImage_OnClick( object sender, ImageClickEventArgs e )
		{
			BindData();
		}
	}
}
