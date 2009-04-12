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
	/// <summary>
	/// Renders the "Last Post" part of the Forum Topics
	/// </summary>
	public partial class ForumLastPost : YAF.Classes.Base.BaseUserControl
	{
		public ForumLastPost()
		{
			this.PreRender += new EventHandler( ForumLastPost_PreRender );
		}

		void ForumLastPost_PreRender( object sender, EventArgs e )
		{
			if ( DataRow != null )
			{
				if ( int.Parse( DataRow ["ReadAccess"].ToString() ) == 0 )
				{
					TopicInPlaceHolder.Visible = false;
				}

				if ( DataRow ["LastPosted"] != DBNull.Value )
				{
					LastPosted.Text = YafDateTime.FormatDateTimeTopic( DataRow ["LastPosted"] );
					topicLink.NavigateUrl = YAF.Classes.Utils.YafBuildLink.GetLinkNotEscaped( YAF.Classes.Utils.ForumPages.posts, "t={0}", DataRow ["LastTopicID"] );
					topicLink.Text = General.Truncate( General.BadWordReplace( DataRow ["LastTopicName"].ToString() ), 50 );
					ProfileUserLink.UserID = Convert.ToInt32( DataRow ["LastUserID"] );
					ProfileUserLink.UserName = DataRow ["LastUser"].ToString();

					LastTopicImgLink.ToolTip = PageContext.Localization.GetText( "GO_LAST_POST" );
					LastTopicImgLink.NavigateUrl = YAF.Classes.Utils.YafBuildLink.GetLinkNotEscaped( YAF.Classes.Utils.ForumPages.posts, "m={0}#post{0}", DataRow ["LastMessageID"] );
					Icon.ThemeTag = ( DateTime.Parse( Convert.ToString( DataRow ["LastPosted"] ) ) > Mession.GetTopicRead( ( int ) DataRow ["LastTopicID"] ) ) ? "ICON_NEWEST" : "ICON_LATEST";

					LastPostedHolder.Visible = true;
					NoPostsLabel.Visible = false;
				}
				else
				{
					// show "no posts"
					LastPostedHolder.Visible = false;
					NoPostsLabel.Visible = true;
				}
			}
		}

		private DataRow _dataRow = null;
		public DataRow DataRow
		{
			get
			{
				return _dataRow;
			}
			set
			{
				_dataRow = value;
			}
		}
	}
}
