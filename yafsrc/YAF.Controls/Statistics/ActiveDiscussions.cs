using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;

namespace YAF.Controls.Statistics
{
	[ToolboxData( "<{0}:ActiveDiscussions runat=\"server\"></{0}:ActiveDiscussions>" )]
	public class ActiveDiscussions : BaseControl
	{
		private int _displayNumber = 10;

		/// <summary>
		/// The default constructor for ActiveDiscussions.
		/// </summary>
		public ActiveDiscussions()
		{

		}

		public int DisplayNumber
		{
			get
			{
				return _displayNumber;
			}
			set
			{
				_displayNumber = value;
			}
		}

		/// <summary>
		/// Renders the ActiveDiscussions class.
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render( System.Web.UI.HtmlTextWriter writer )
		{
			string htmlOutput = "";

			htmlOutput += "<table width=\"90%\" class=\"content\" cellspacing=\"1\" border=\"0\" cellpadding=\"0\">";
			htmlOutput += "<tr><td class=\"header1\">" + PageContext.Localization.GetText( "LATEST_POSTS" ) + "</td></tr>";
			System.Data.DataTable dt = YAF.Classes.Data.DB.topic_latest( PageContext.PageBoardID, 7, PageContext.PageUserID );
			int i = 1;

			htmlOutput += "<tr><td class=post><table cellspacing=0 cellpadding=0 align=center>";

			foreach ( System.Data.DataRow r in dt.Rows )
			{
				i++;
				htmlOutput += "<tr class=\"post\">";

				htmlOutput += "<td width=\"75%\">";
				//Output Topic Link
				htmlOutput += string.Format( "&nbsp;<a href=\"{1}\">{0}</a></td>", General.BadWordReplace( Convert.ToString( r ["Topic"] ) ), YafBuildLink.GetLink( ForumPages.posts, "m={0}#{0}", r ["LastMessageID"] ) );
				//Output Message Icon
				htmlOutput += "<img src=\"" + PageContext.Theme.GetItem( "ICONS", "ICON_LATEST" ) + "\" border=\"0\" alt=\"\"></a>";
				htmlOutput += "</td></tr>";
			}

			htmlOutput += "</table></td></tr>";

			htmlOutput += "</table>";
			writer.Write( htmlOutput );
		}
	}
}
