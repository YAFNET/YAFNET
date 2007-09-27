using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;
using YAF.Classes.UI;
using YAF.Classes.Data;

namespace YAF.Controls
{
	/// <summary>
	///		Summary description for ForumList.
	/// </summary>
	public partial class ForumList : YAF.Classes.Base.BaseUserControl
	{

		protected void Page_Load( object sender, System.EventArgs e )
		{
		}

		protected string GetSubForumIcon( object o )
		{
			DataRow row = ( DataRow ) o;
			DateTime lastRead = Mession.GetForumRead( ( int ) row ["ForumID"] );
			DateTime lastPosted = row ["LastPosted"] != DBNull.Value ? ( DateTime ) row ["LastPosted"] : lastRead;

			string img, imgTitle;

			try
			{
				if ( lastPosted > lastRead )
				{
					img = PageContext.Theme.GetItem( "ICONS", "SUBFORUM_NEW" );
					imgTitle = PageContext.Localization.GetText( "ICONLEGEND", "New_Posts" );
				}
				else
				{
					img = PageContext.Theme.GetItem( "ICONS", "SUBFORUM" );
					imgTitle = PageContext.Localization.GetText( "ICONLEGEND", "No_New_Posts" );
				}
			}
			catch ( Exception )
			{
				img = PageContext.Theme.GetItem( "ICONS", "SUBFORUM" );
				imgTitle = PageContext.Localization.GetText( "ICONLEGEND", "No_New_Posts" );
			}

			return String.Format( "<img src=\"{0}\" title=\"{1}\"/>", img, imgTitle );
		}

		protected string GetForumIcon( object o )
		{
			DataRow row = ( DataRow ) o;
			bool locked = General.BinaryAnd( row ["Flags"], ForumFlags.Locked );
			DateTime lastRead = Mession.GetForumRead( ( int ) row ["ForumID"] );
			DateTime lastPosted = row ["LastPosted"] != DBNull.Value ? ( DateTime ) row ["LastPosted"] : lastRead;

			string img, imgTitle;

			try
			{
				if ( locked )
				{
					img = PageContext.Theme.GetItem( "ICONS", "FORUM_LOCKED" );
					imgTitle = PageContext.Localization.GetText( "ICONLEGEND", "Forum_Locked" );
				}
				else if ( lastPosted > lastRead )
				{
					img = PageContext.Theme.GetItem( "ICONS", "FORUM_NEW" );
					imgTitle = PageContext.Localization.GetText( "ICONLEGEND", "New_Posts" );
				}
				else
				{
					img = PageContext.Theme.GetItem( "ICONS", "FORUM" );
					imgTitle = PageContext.Localization.GetText( "ICONLEGEND", "No_New_Posts" );
				}
			}
			catch ( Exception )
			{
				img = PageContext.Theme.GetItem( "ICONS", "FORUM" );
				imgTitle = PageContext.Localization.GetText( "ICONLEGEND", "No_New_Posts" );
			}

			return String.Format( "<img src=\"{0}\" title=\"{1}\"/>", img, imgTitle );
		}

		// Suppress rendering of footer if there is one or more 
		protected string GetModeratorsFooter( Repeater sender )
		{
			if ( sender.DataSource != null && sender.DataSource is DataRow [] && ( ( DataRow [] ) sender.DataSource ).Length < 1 )
			{
				return "-";
			}
			else return "";
		}

		protected string GetModeratorLink( System.Data.DataRow row )
		{
			string output;

			if ( ( int ) row ["IsGroup"] == 0 )
			{
				output = String.Format(
						"<a href=\"{0}\">{1}</a>",
						YafBuildLink.GetLink( ForumPages.profile, "u={0}", row ["ModeratorID"] ),
						row ["ModeratorName"]
						);
			}
			else
			{
				// TODO : group link should point to group info page (yet unavailable)
				/*output = String.Format(
						"<b><a href=\"{0}\">{1}</a></b>",
						YafBuildLink.GetLink(ForumPages.forum, "g={0}", row["ModeratorID"]),
						row["ModeratorName"]
						);*/
				output = String.Format(
						"<b>{0}</b>",
						row ["ModeratorName"]
						);
			}

			return output;
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

		protected string Topics( object _o )
		{
			DataRow row = ( DataRow ) _o;
			if ( row ["RemoteURL"] == DBNull.Value )
				return string.Format( "{0:N0}", row ["Topics"] );
			else
				return "-";
		}
		protected string Posts( object _o )
		{
			DataRow row = ( DataRow ) _o;
			if ( row ["RemoteURL"] == DBNull.Value )
				return string.Format( "{0:N0}", row ["Posts"] );
			else
				return "-";
		}
		protected string GetViewing( object o )
		{
			DataRow row = ( DataRow ) o;
			int nViewing = ( int ) row ["Viewing"];
			if ( nViewing > 0 )
				return "&nbsp;" + String.Format( PageContext.Localization.GetText( "VIEWING" ), nViewing );
			else
				return "";
		}

		public System.Collections.IEnumerable DataSource
		{
			set
			{
				forumList.DataSource = value;
			}
		}

		protected bool GetModerated( object o )
		{
			return General.BinaryAnd( ( ( DataRow ) o ) ["Flags"], ForumFlags.Moderated );
		}

		// Ederon : 08/27/2007
		protected bool HasSubforums( System.Data.DataRow row )
		{
			return ( ( int ) row ["Subforums"] > 0 );
		}

		// Ederon : 08/27/2007
		protected System.Collections.IEnumerable GetSubforums( System.Data.DataRow row )
		{
			return YAF.Classes.Data.DB.forum_listread(
				PageContext.PageBoardID, PageContext.PageUserID, row ["CategoryID"], row ["ForumID"] ).Rows;
		}
	}
}
