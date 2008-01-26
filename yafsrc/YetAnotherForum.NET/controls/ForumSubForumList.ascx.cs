/* Yet Another Forum.NET
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
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;

namespace YAF.Controls
{
	public partial class ForumSubForumList : YAF.Classes.Base.BaseUserControl
	{
		public ForumSubForumList()
			: base()
		{

		}

		public System.Collections.IEnumerable DataSource
		{
			set
			{
				SubforumList.DataSource = value;
			}
		}

		protected void SubforumList_ItemCreated( object sender, RepeaterItemEventArgs e )
		{
			if ( e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem )
			{
				DataRow row = ( DataRow )e.Item.DataItem;
				DateTime lastRead = Mession.GetForumRead( ( int )row ["ForumID"] );
				DateTime lastPosted = row ["LastPosted"] != DBNull.Value ? ( DateTime )row ["LastPosted"] : lastRead;

				ThemeImage subForumIcon = e.Item.FindControl( "ThemeSubforumIcon" ) as ThemeImage;

				subForumIcon.ThemeTag = "SUBFORUM";
				subForumIcon.LocalizedTitlePage = "ICONLEGEND";
				subForumIcon.LocalizedTitleTag = "NO_NEW_POSTS";

				try
				{
					if ( lastPosted > lastRead )
					{
						subForumIcon.ThemeTag = "SUBFORUM_NEW";
						subForumIcon.LocalizedTitlePage = "ICONLEGEND";
						subForumIcon.LocalizedTitleTag = "NEW_POSTS";
					}
				}
				catch
				{

				}
			}
		}

		/// <summary>
		/// Provides the "Forum Link Text" for the ForumList control.
		/// Automatically disables the link if the current user doesn't
		/// have proper permissions.
		/// </summary>
		/// <param name="row">Current data row</param>
		/// <returns>Forum link text</returns>
		public string GetForumLink( System.Data.DataRow row )
		{
			string output = "";
			int forumID = Convert.ToInt32( row ["ForumID"] );

			// get the Forum Description
			output = Convert.ToString( row ["Forum"] );

			if ( int.Parse( row ["ReadAccess"].ToString() ) > 0 )
			{
				output = String.Format( "<a href=\"{0}\">{1}</a>",
					YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.topics, "f={0}", forumID ),
					output
					);
			}
			else
			{
				// no access to this forum
				output = String.Format( "{0} {1}", output, PageContext.Localization.GetText( "NO_FORUM_ACCESS" ) );
			}

			return output;
		}
	}
}