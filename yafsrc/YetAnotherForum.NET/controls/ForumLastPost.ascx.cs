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
					topicLink.NavigateUrl = YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.posts, "t={0}", DataRow ["LastTopicID"] );
					topicLink.Text = General.Truncate( General.BadWordReplace( DataRow ["LastTopicName"].ToString() ), 50 );
					ProfileUserLink.UserID = Convert.ToInt32( DataRow ["LastUserID"] );
					ProfileUserLink.UserName = DataRow ["LastUser"].ToString();

					LastTopicImgLink.ToolTip = PageContext.Localization.GetText( "GO_LAST_POST" );
					LastTopicImgLink.NavigateUrl = YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.posts, "m={0}#{0}", DataRow ["LastMessageID"] );
					Icon.ImageUrl = PageContext.Theme.GetItem( "ICONS", ( DateTime.Parse( Convert.ToString( DataRow ["LastPosted"] ) ) > Mession.GetTopicRead( ( int ) DataRow ["LastTopicID"] ) ) ? "ICON_NEWEST" : "ICON_LATEST" );

					LastPostedHolder.Visible = true;
					NoPostsHolder.Visible = false;
				}
				else
				{
					// show "no posts"
					LastPostedHolder.Visible = false;
					NoPostsHolder.Visible = true;
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
