/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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
using YAF.Classes;
using YAF.Classes.Utils;


namespace YAF.Pages
{
	/// <summary>
	/// Summary description for movetopic.
	/// </summary>
	public partial class movemessage : YAF.Classes.Core.ForumPage
	{
		public movemessage()
			: base( "MOVEMESSAGE" )
		{
		}

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( Request.QueryString ["m"] == null || !PageContext.ForumModeratorAccess )
				YafBuildLink.AccessDenied();

			if ( !IsPostBack )
			{
				PageLinks.AddLink( PageContext.BoardSettings.Name, YafBuildLink.GetLink( ForumPages.forum ) );
				PageLinks.AddLink( PageContext.PageCategoryName, YafBuildLink.GetLink( ForumPages.forum, "c={0}", PageContext.PageCategoryID ) );
				PageLinks.AddForumLinks( PageContext.PageForumID );
				PageLinks.AddLink( PageContext.PageTopicName, YafBuildLink.GetLink( ForumPages.posts, "t={0}", PageContext.PageTopicID ) );

				Move.Text = GetText( "MOVE_MESSAGE" );
				CreateAndMove.Text = GetText( "CREATE_TOPIC" );

				ForumList.DataSource = YAF.Classes.Data.DB.forum_listall_sorted( PageContext.PageBoardID, PageContext.PageUserID );
				ForumList.DataTextField = "Title";
				ForumList.DataValueField = "ForumID";
				DataBind();
				ForumList.Items.FindByValue( PageContext.PageForumID.ToString() ).Selected = true;
				ForumList_SelectedIndexChanged( this.ForumList, e );
			}
		}

		protected void Move_Click( object sender, System.EventArgs e )
		{
			if ( Convert.ToInt32( TopicsList.SelectedValue ) != PageContext.PageTopicID )
			{
				YAF.Classes.Data.DB.message_move( Request.QueryString ["m"], TopicsList.SelectedValue, true );
			}
			YafBuildLink.Redirect( ForumPages.topics, "f={0}", PageContext.PageForumID );
		}

		protected void ForumList_SelectedIndexChanged( object sender, System.EventArgs e )
		{
			TopicsList.DataSource = YAF.Classes.Data.DB.topic_list( ForumList.SelectedValue, null, 0, null, 0, 32762 );
			TopicsList.DataTextField = "Subject";
			TopicsList.DataValueField = "TopicID";
			TopicsList.DataBind();
			TopicsList_SelectedIndexChanged( this.ForumList, e );
			CreateAndMove.Enabled = Convert.ToInt32( ForumList.SelectedValue ) <= 0 ? false : true;
		}

		protected void TopicsList_SelectedIndexChanged( object sender, System.EventArgs e )
		{
			if ( TopicsList.SelectedValue == "" )
				Move.Enabled = false;
			else
				Move.Enabled = true;
		}

		protected void CreateAndMove_Click( object sender, System.EventArgs e )
		{
			if ( TopicSubject.Text != "" )
			{
				long nTopicId =
						YAF.Classes.Data.DB.topic_create_by_message( Request.QueryString ["m"], ForumList.SelectedValue, TopicSubject.Text );
				YAF.Classes.Data.DB.message_move( Request.QueryString ["m"], nTopicId, true );
				YafBuildLink.Redirect( ForumPages.topics, "f={0}", PageContext.PageForumID );
			}
			else
				PageContext.AddLoadMessage( GetText( "EmptyTopic" ) );
		}
	}
}