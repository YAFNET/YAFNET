using System;
using System.Data;

namespace yaf.controls
{
	/// <summary>
	/// Summary description for ForumUsers.
	/// </summary>
	public class ProfileMenu : BaseControl
	{
		protected override void Render(System.Web.UI.HtmlTextWriter writer) 
		{
			System.Text.StringBuilder html = new System.Text.StringBuilder(2000);

			html.Append("<table width='100%' cellspacing=0 cellpadding=0>");
			html.AppendFormat("<tr class='header2'><td>{0}</td></tr>",Page.GetText("MESSENGER"));
			html.AppendFormat("<tr><td>");
			html.AppendFormat("<li><a href='cp_inbox.aspx'>{0}</a></li>",Page.GetText("INBOX"));
			html.AppendFormat("<li><a href='cp_inbox.aspx?sent=1'>{0}</a></li>",Page.GetText("SENTITEMS"));
			html.AppendFormat("<li><a href='pmessage.aspx'>{0}</a></li>",Page.GetText("NEW_MESSAGE"));
			html.AppendFormat("</td></tr>");
			html.AppendFormat("<tr><td>&nbsp;</td></tr>");
			html.AppendFormat("<tr class='header2'><td>{0}</td></tr>",Page.GetText("PERSONAL_PROFILE"));
			html.AppendFormat("<tr><td>");
			html.AppendFormat("<li><a href='cp_editprofile.aspx'>{0}</a></li>",Page.GetText("EDIT_PROFILE"));
			html.AppendFormat("<li><a href='cp_signature.aspx'>{0}</a></li>",Page.GetText("SIGNATURE"));
			html.AppendFormat("<li><a href='cp_subscriptions.aspx'>{0}</a></li>",Page.GetText("SUBSCRIPTIONS"));
			html.AppendFormat("</td></tr>");
			html.Append("</table>");

			writer.Write(html.ToString());
		}
	}
}
