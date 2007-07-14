using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;
using YAF.Classes.UI;

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

		#region Web Form Designer generated code
		override protected void OnInit( EventArgs e )
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit( e );
		}

		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion

		protected string GetForumIcon( object o )
		{
			DataRow row = ( DataRow ) o;
			bool locked = ( ( int ) row ["Flags"] & ( int ) YAF.Classes.Data.ForumFlags.Locked ) == ( int ) YAF.Classes.Data.ForumFlags.Locked;
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

		protected void ModeratorList_ItemCommand( object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e )
		{
			//PageContext.AddLoadMessage("TODO: Fix this");
			//TODO: Show moderators
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
			string strReturn = "";
			int ForumID = Convert.ToInt32( row ["ForumID"] );

			// get the Forum Description
			strReturn = Convert.ToString( row ["Forum"] );

			if ( int.Parse( row ["ReadAccess"].ToString() ) > 0 )
			{
				strReturn = String.Format( "<a href=\"{0}\">{1}</a>",
					YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.topics, "f={0}", ForumID ),
					strReturn
					);
			}
			else
			{
				// no access to this forum
				strReturn = String.Format( "{0} {1}", strReturn, PageContext.Localization.GetText( "NO_FORUM_ACCESS" ) );
			}

			return strReturn;
		}

		/// <summary>
		/// Creates the text for the "Last Post" information on a forum listing.
		/// Detects user permissions and disables links if they have none.
		/// </summary>
		/// <param name="row">Current data row</param>
		/// <returns>Formatted "Last Post" text</returns>
		protected string FormatLastPost( System.Data.DataRow row )
		{
			if ( row ["RemoteURL"] != DBNull.Value )
				return "-";

			int ForumID = Convert.ToInt32( row ["ForumID"] );
			// defaults to "no posts" text
			string strTemp = PageContext.Localization.GetText( "NO_POSTS" );

			if ( !row.IsNull( "LastPosted" ) )
			{
				// Ederon : 7/14/2007
				strTemp = PageContext.Theme.GetItem("ICONS", (DateTime.Parse(Convert.ToString(row["LastPosted"])) > Mession.GetTopicRead((int)row["LastTopicID"])) ? "ICON_NEWEST" : "ICON_LATEST");
				//strTemp = PageContext.Theme.GetItem("ICONS", (DateTime.Parse(Convert.ToString(row["LastPosted"])) > Mession.LastVisit) ? "ICON_NEWEST" : "ICON_LATEST");

				if ( int.Parse( row ["ReadAccess"].ToString() ) > 0 )
				{
					strTemp = String.Format( "{0}<br/>{1}<br/>{2}&nbsp;<a title=\"{4}\" href=\"{5}\"><img src=\"{3}\"></a>",
							yaf_DateTime.FormatDateTimeTopic( ( DateTime ) row ["LastPosted"] ),
							String.Format( PageContext.Localization.GetText( "in" ), String.Format( "<a href=\"{0}\">{1}</a>", YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.posts, "t={0}", row ["LastTopicID"] ), Truncate( General.BadWordReplace( row ["LastTopicName"].ToString() ), 50 ) ) ),
							String.Format( PageContext.Localization.GetText( "by" ), String.Format( "<a href=\"{0}\">{1}</a>", YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.profile, "u={0}", row ["LastUserID"] ), BBCode.EncodeHTML( row ["LastUser"].ToString() ) ) ),
							strTemp,
							PageContext.Localization.GetText( "GO_LAST_POST" ),
							YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.posts, "m={0}#{0}", row ["LastMessageID"] )
						);
				}
				else
				{
					// no access to this forum... disable links and hide topic
					strTemp = String.Format( "{0}<br/>{1}",
							yaf_DateTime.FormatDateTimeTopic( ( DateTime ) row ["LastPosted"] ),
							// Removed by Mek(16 December 2006) to stop non access people viewing the topic title
                            // String.Format( PageContext.Localization.GetText( "in" ), String.Format( "{0}", Truncate( row ["LastTopicName"].ToString(), 50 ) ) ),
							String.Format( PageContext.Localization.GetText( "by" ), String.Format( "<a href=\"{0}\">{1}</a>", YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.profile, "u={0}", row ["LastUserID"] ), row ["LastUser"] ) )
						);
				}
			}

			return strTemp;
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

		private string Truncate( string input, int limit ) // Only use it here? (Jwendl)
		{
			string output = input;

			// Check if the string is longer than the allowed amount
			// otherwise do nothing
			if ( output.Length > limit && limit > 0 )
			{

				// cut the string down to the maximum number of characters
				output = output.Substring( 0, limit );

				// Check if the space right after the truncate point 
				// was a space. if not, we are in the middle of a word and 
				// need to cut out the rest of it
				if ( input.Substring( output.Length, 1 ) != " " )
				{
					int LastSpace = output.LastIndexOf( " " );

					// if we found a space then, cut back to that space
					if ( LastSpace != -1 )
					{
						output = output.Substring( 0, LastSpace );
					}
				}
				// Finally, add the "..."
				output += "...";
			}
			return output;
		}

		protected bool GetModerated( object _o )
		{
			DataRow row = ( DataRow ) _o;
			return ( ( int ) row ["Flags"] & ( int ) YAF.Classes.Data.ForumFlags.Moderated ) == ( int ) YAF.Classes.Data.ForumFlags.Moderated;
		}
	}
}
