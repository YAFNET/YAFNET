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
			html.AppendFormat("<tr class='header2'><td>{0}</td></tr>",MyPage.GetText("MESSENGER"));
			html.AppendFormat("<tr><td>");
			html.AppendFormat("<li><a href='{0}'>{1}</a></li>",Forum.GetLink(Pages.cp_inbox),MyPage.GetText("INBOX"));
			html.AppendFormat("<li><a href='{0}'>{1}</a></li>",Forum.GetLink(Pages.cp_inbox,"sent=1"),MyPage.GetText("SENTITEMS"));
			html.AppendFormat("<li><a href='{0}'>{1}</a></li>",Forum.GetLink(Pages.pmessage),MyPage.GetText("NEW_MESSAGE"));
			html.AppendFormat("</td></tr>");
			html.AppendFormat("<tr><td>&nbsp;</td></tr>");
			html.AppendFormat("<tr class='header2'><td>{0}</td></tr>",MyPage.GetText("PERSONAL_PROFILE"));
			html.AppendFormat("<tr><td>");
			html.AppendFormat("<li><a href='{0}'>{1}</a></li>",Forum.GetLink(Pages.cp_editprofile),MyPage.GetText("EDIT_PROFILE"));
			html.AppendFormat("<li><a href='{0}'>{1}</a></li>",Forum.GetLink(Pages.cp_signature),MyPage.GetText("SIGNATURE"));
			html.AppendFormat("<li><a href='{0}'>{1}</a></li>",Forum.GetLink(Pages.cp_subscriptions),MyPage.GetText("SUBSCRIPTIONS"));
			html.AppendFormat("</td></tr>");
			html.Append("</table>");

			writer.Write(html.ToString());
		}
	}
}
