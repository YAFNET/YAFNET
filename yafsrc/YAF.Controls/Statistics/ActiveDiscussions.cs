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
	[ToolboxData("<{0}:ActiveDiscussions runat=\"server\"></{0}:ActiveDiscussions>")]
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
		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			System.Text.StringBuilder html = new System.Text.StringBuilder(_displayNumber * 75 + 100);

			html.Append("<table width=\"90%\" class=\"content\" cellspacing=\"1\" border=\"0\" cellpadding=\"0\">");
			html.AppendFormat("<tr><td class=\"header1\">{0}</td></tr>", PageContext.Localization.GetText("LATEST_POSTS"));

			System.Data.DataTable dt = YAF.Classes.Data.DB.topic_latest(PageContext.PageBoardID, _displayNumber, PageContext.PageUserID);
			int i = 1;

			html.Append("<tr><td class=\"post\"><table cellspacing=\"0\" cellpadding=\"0\" align=\"center\">");

			foreach (System.Data.DataRow r in dt.Rows)
			{
				i++;
				html.Append("<tr class=\"post\">");

				html.Append("<td width=\"75%\">");

				//Output Topic Link
				html.AppendFormat("&nbsp;<a href=\"{1}\">{0}</a></td>",
					General.BadWordReplace(Convert.ToString(r["Topic"])),
					YafBuildLink.GetLink(ForumPages.posts, "m={0}#{0}", r["LastMessageID"])
					);
				//Output Message Icon
				html.AppendFormat("<img src=\"{0}\" border=\"0\" alt=\"\"></a>",
					PageContext.Theme.GetItem(
						"ICONS",
						(DateTime.Parse(Convert.ToString(r["LastPosted"])) > Mession.GetTopicRead((int)r["TopicID"])) ? "ICON_NEWEST" : "ICON_LATEST"
						)
					);

				html.Append("</td></tr>");
			}

			html.Append("</table></td></tr>");
			html.Append("</table>");

			writer.Write(html.ToString());
		}
	}
}
