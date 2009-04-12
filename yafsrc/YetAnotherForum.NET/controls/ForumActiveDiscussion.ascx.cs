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
	public partial class ForumActiveDiscussion : YAF.Classes.Base.BaseUserControl
	{
		public ForumActiveDiscussion()
			: base()
		{

		}

		public override void DataBind()
		{
			BindData();
			base.DataBind();
		}

		protected void BindData()
		{
			// Latest forum posts
			// Shows the latest n number of posts on the main forum list page

			string cacheKey = YafCache.GetBoardCacheKey( Constants.Cache.ForumActiveDiscussions );
			DataTable activeTopics = null;

			if ( PageContext.IsGuest )
			{
				// allow caching since this is a guest...				
				activeTopics = YafCache.Current [cacheKey] as DataTable;
			}

			if ( activeTopics == null )
			{
				activeTopics = YAF.Classes.Data.DB.topic_latest( PageContext.PageBoardID, PageContext.BoardSettings.ActiveDiscussionsCount, PageContext.PageUserID );
				if ( PageContext.IsGuest ) YafCache.Current.Insert( cacheKey, activeTopics, null, DateTime.Now.AddMinutes( PageContext.BoardSettings.ActiveDiscussionsCacheTimeout ), TimeSpan.Zero );
			}

			LatestPosts.DataSource = activeTopics;
		}

		protected void LatestPosts_ItemDataBound( object sender, RepeaterItemEventArgs e )
		{
			// populate the controls here...
			if ( e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem )
			{
				DataRowView currentRow = ( DataRowView ) e.Item.DataItem;
				// make message url...
				string messageUrl = YAF.Classes.Utils.YafBuildLink.GetLinkNotEscaped( YAF.Classes.Utils.ForumPages.posts, "m={0}#post{0}", currentRow ["LastMessageID"] );
				// get the controls
				HyperLink textMessageLink = (HyperLink)e.Item.FindControl( "TextMessageLink" );
				HyperLink imageMessageLink = (HyperLink)e.Item.FindControl( "ImageMessageLink" );
				ThemeImage lastPostedImage = (ThemeImage)e.Item.FindControl( "LastPostedImage" );
				UserLink lastUserLink = ( UserLink ) e.Item.FindControl( "LastUserLink" );
				Label lastPostedDateLabel = ( Label ) e.Item.FindControl( "LastPostedDateLabel" );
				HyperLink forumLink = ( HyperLink ) e.Item.FindControl( "ForumLink" );

				// populate them...
				textMessageLink.Text = YAF.Classes.Utils.General.BadWordReplace( currentRow["Topic"].ToString() );
				textMessageLink.NavigateUrl = messageUrl;
				imageMessageLink.NavigateUrl = messageUrl;
				// Just in case...
				if ( currentRow ["LastUserID"] != DBNull.Value )
				{
					lastUserLink.UserID = Convert.ToInt32( currentRow ["LastUserID"] );
					lastUserLink.UserName = currentRow ["LastUserName"].ToString();
				}
				if ( currentRow ["LastPosted"] != DBNull.Value )
				{
					lastPostedDateLabel.Text = YafDateTime.FormatDateTimeTopic( currentRow ["LastPosted"] );
					lastPostedImage.ThemeTag = ( DateTime.Parse( currentRow ["LastPosted"].ToString() ) > YAF.Classes.Utils.Mession.GetTopicRead( Convert.ToInt32( currentRow ["TopicID"] ) ) ) ? "ICON_NEWEST" : "ICON_LATEST";
				}
				forumLink.Text = currentRow["Forum"].ToString();
				forumLink.NavigateUrl = YAF.Classes.Utils.YafBuildLink.GetLinkNotEscaped( YAF.Classes.Utils.ForumPages.topics, "f={0}", currentRow ["ForumID"] );				
			}
		}

		public event EventHandler<EventArgs> NeedDataBind;

		protected void CollapsibleImage_OnClick( object sender, ImageClickEventArgs e )
		{
			if ( NeedDataBind != null ) NeedDataBind( this, new EventArgs() );
		}
	}
}
