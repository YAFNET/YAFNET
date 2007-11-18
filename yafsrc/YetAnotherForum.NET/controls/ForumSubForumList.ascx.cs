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

		protected string GetSubForumIcon( object o )
		{
			DataRow row = ( DataRow )o;
			DateTime lastRead = Mession.GetForumRead( ( int )row ["ForumID"] );
			DateTime lastPosted = row ["LastPosted"] != DBNull.Value ? ( DateTime )row ["LastPosted"] : lastRead;

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