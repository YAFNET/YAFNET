using System;
using System.Data;

namespace yaf.controls
{
	/// <summary>
	/// Summary description for ForumUsers.
	/// </summary>
	public class IconLegend : BaseControl
	{
		protected override void Render(System.Web.UI.HtmlTextWriter writer) 
		{
			System.Text.StringBuilder html = new System.Text.StringBuilder(2000);
			html.Append("<table cellspacing=1 cellpadding=1><tr>");
			html.AppendFormat("<td><img align=absMiddle src='{0}'/> {1}</td>",Page.GetThemeContents("ICONS","TOPIC_NEW"),Page.GetText("NEW_POSTS"));
			html.AppendFormat("<td><img align=absMiddle src='{0}'/> {1}</td>",Page.GetThemeContents("ICONS","TOPIC"),Page.GetText("NO_NEW_POSTS"));
			html.Append("</tr><tr>");
			html.AppendFormat("<td><img align=absMiddle src='{0}'/> {1}</td>",Page.GetThemeContents("ICONS","TOPIC_NEW_LOCKED"),Page.GetText("NEW_POSTS_LOCKED"));
			html.AppendFormat("<td><img align=absMiddle src='{0}'/> {1}</td>",Page.GetThemeContents("ICONS","TOPIC_LOCKED"),Page.GetText("NO_NEW_POSTS_LOCKED"));
			html.Append("</tr><tr>");
			html.AppendFormat("<td><img align=absMiddle src='{0}'/> {1}</td>",Page.GetThemeContents("ICONS","TOPIC_ANNOUNCEMENT"),Page.GetText("ANNOUNCEMENT"));
			html.AppendFormat("<td><img align=absMiddle src='{0}'/> {1}</td>",Page.GetThemeContents("ICONS","TOPIC_STICKY"),Page.GetText("STICKY"));
			html.Append("</tr><tr>");
			html.AppendFormat("<td colspan=1><img align=absMiddle src='{0}'/> {1}</td>",Page.GetThemeContents("ICONS","TOPIC_MOVED"),Page.GetText("MOVED"));
			html.AppendFormat("<td colspan=1><img align=absMiddle src='{0}'/> {1}</td>",Page.GetThemeContents("ICONS","TOPIC_POLL"),Page.GetText("POLL"));
			html.Append("</tr></table>");

			writer.Write(html.ToString());
		}
	}
}
