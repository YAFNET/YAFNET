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
		
		protected string GetSubForumIcon(object o)
		{
			DataRow row = (DataRow)o;
			DateTime lastRead = Mession.GetForumRead((int)row["ForumID"]);
			DateTime lastPosted = row["LastPosted"] != DBNull.Value ? (DateTime)row["LastPosted"] : lastRead;

			string img, imgTitle;

			try
			{
				if (lastPosted > lastRead)
				{
					img = PageContext.Theme.GetItem("ICONS", "SUBFORUM_NEW");
					imgTitle = PageContext.Localization.GetText("ICONLEGEND", "New_Posts");
				}
				else
				{
					img = PageContext.Theme.GetItem("ICONS", "SUBFORUM");
					imgTitle = PageContext.Localization.GetText("ICONLEGEND", "No_New_Posts");
				}
			}
			catch (Exception)
			{
				img = PageContext.Theme.GetItem("ICONS", "SUBFORUM");
				imgTitle = PageContext.Localization.GetText("ICONLEGEND", "No_New_Posts");
			}

			return String.Format("<img src=\"{0}\" title=\"{1}\"/>", img, imgTitle);
		}

		protected string GetForumIcon( object o )
		{
			DataRow row = ( DataRow ) o;
			bool locked = General.BinaryAnd(row["Flags"], ForumFlags.Locked);
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
		protected string GetModeratorsFooter(Repeater sender)
		{
			if (sender.DataSource != null && sender.DataSource is DataRow[] && ((DataRow[])sender.DataSource).Length < 1)
			{
				return "-";
			}
			else return "";
		}

		protected string GetModeratorLink(System.Data.DataRow row)
		{
            string output;

            if ((int)row["IsGroup"] == 0)
            {
                output = String.Format(
                    "<a href=\"{0}\">{1}</a>",
                    YafBuildLink.GetLink(ForumPages.profile, "u={0}", row["ModeratorID"]),
                    row["ModeratorName"]
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
                    row["ModeratorName"]
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
							YafDateTime.FormatDateTimeTopic( ( DateTime ) row ["LastPosted"] ),
							String.Format( PageContext.Localization.GetText( "in" ), String.Format( "<a href=\"{0}\">{1}</a>", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.posts, "t={0}", row ["LastTopicID"] ), Truncate( General.BadWordReplace( row ["LastTopicName"].ToString() ), 50 ) ) ),
							String.Format( PageContext.Localization.GetText( "by" ), String.Format( "<a href=\"{0}\">{1}</a>", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.profile, "u={0}", row ["LastUserID"] ), BBCode.EncodeHTML( row ["LastUser"].ToString() ) ) ),
							strTemp,
							PageContext.Localization.GetText( "GO_LAST_POST" ),
							YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.posts, "m={0}#{0}", row ["LastMessageID"] )
						);
				}
				else
				{
					// no access to this forum... disable links and hide topic
					strTemp = String.Format( "{0}<br/>{1}",
							YafDateTime.FormatDateTimeTopic( ( DateTime ) row ["LastPosted"] ),
							// Removed by Mek(16 December 2006) to stop non access people viewing the topic title
                            // String.Format( PageContext.Localization.GetText( "in" ), String.Format( "{0}", Truncate( row ["LastTopicName"].ToString(), 50 ) ) ),
							String.Format( PageContext.Localization.GetText( "by" ), String.Format( "<a href=\"{0}\">{1}</a>", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.profile, "u={0}", row ["LastUserID"] ), row ["LastUser"] ) )
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

		protected bool GetModerated( object o )
		{
			return General.BinaryAnd(((DataRow)o)["Flags"], ForumFlags.Moderated);
		}

		// Ederon : 08/27/2007
		protected bool HasSubforums(System.Data.DataRow row)
		{
			return ((int)row["Subforums"] > 0);
		}

		// Ederon : 08/27/2007
		protected System.Collections.IEnumerable GetSubforums(System.Data.DataRow row)
		{
			return YAF.Classes.Data.DB.forum_listread(
				PageContext.PageBoardID, PageContext.PageUserID, row["CategoryID"], row["ForumID"]).Rows;
		}
	}
}
