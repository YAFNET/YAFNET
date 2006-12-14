using System;
using System.Data;

namespace YAF.Controls
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
			if (ForumPage.BoardSettings.AllowPrivateMessages)
			{
				html.AppendFormat("<tr class='header2'><td>{0}</td></tr>",ForumPage.GetText("MESSENGER"));
				html.AppendFormat("<tr><td>");
				html.AppendFormat("<li><a href='{0}'>{1}</a></li>",Forum.GetLink( ForumPages.cp_inbox),ForumPage.GetText("INBOX"));
				html.AppendFormat("<li><a href='{0}'>{1}</a></li>",Forum.GetLink( ForumPages.cp_inbox,"sent=1"),ForumPage.GetText("SENTITEMS"));
				html.AppendFormat("<li><a href='{0}'>{1}</a></li>",Forum.GetLink( ForumPages.pmessage),ForumPage.GetText("NEW_MESSAGE"));
				html.AppendFormat("</td></tr>");
				html.AppendFormat("<tr><td>&nbsp;</td></tr>");
			}
			html.AppendFormat("<tr class='header2'><td>{0}</td></tr>",ForumPage.GetText("PERSONAL_PROFILE"));
			html.AppendFormat("<tr><td>");
			html.AppendFormat("<li><a href='{0}'>{1}</a></li>",Forum.GetLink( ForumPages.cp_editprofile),ForumPage.GetText("EDIT_PROFILE"));
			html.AppendFormat( "<li><a href='{0}'>{1}</a></li>", Forum.GetLink( ForumPages.cp_editavatar ), ForumPage.GetText( "EDIT_AVATAR" ) );
			if (ForumPage.BoardSettings.AllowSignatures) html.AppendFormat("<li><a href='{0}'>{1}</a></li>",Forum.GetLink( ForumPages.cp_signature),ForumPage.GetText("SIGNATURE"));
			html.AppendFormat("<li><a href='{0}'>{1}</a></li>",Forum.GetLink( ForumPages.cp_subscriptions),ForumPage.GetText("SUBSCRIPTIONS"));
            html.AppendFormat("<li><a href='{0}'>{1}</a></li>", Forum.GetLink( ForumPages.cp_changepassword), "Change Password"/*ForumPage.GetText("CHANGE_PASSWORD")*/);
            html.AppendFormat("</td></tr>");
			html.Append("</table>");

			writer.Write(html.ToString());
		}
	}
}
