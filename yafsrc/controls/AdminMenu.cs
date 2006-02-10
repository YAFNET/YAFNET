using System;

namespace yaf.controls
{
	/// <summary>
	/// Summary description for AdminMenu.
	/// </summary>
	public class AdminMenu : BaseControl
	{
		public AdminMenu()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	
		protected override void Render(System.Web.UI.HtmlTextWriter writer) 
		{	
			writer.WriteLine("<table width='100%' cellspacing='0' cellpadding='0'><tr>");
			writer.WriteLine("<td width='10%' valign='top'>");

			writer.WriteLine("<table class=\"content\" width=\"100%\" cellspacing=0 cellpadding=0><tr><td class=\"post\" valign='top'>");
			writer.WriteLine("<table width=\"100%\" cellspacing=0 cellpadding=0>");
			
			if(ForumPage.IsHostAdmin) 
			{
				writer.WriteLine("<tr><td nowrap class=\"header2\"><b>Host Admin</b></td></tr>");
				writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Host Settings</a></td></tr>",Forum.GetLink(Pages.admin_hostsettings));
				writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Boards</a></td></tr>",Forum.GetLink(Pages.admin_boards));
			}
			writer.WriteLine("<tr><td nowrap class=\"header2\"><b>Admin</b></td></tr>");
			writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Admin Index</a></td></tr>",Forum.GetLink(Pages.admin_admin));
			writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Board Settings</a></td></tr>",Forum.GetLink(Pages.admin_boardsettings));
			writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Forums</a></td></tr>",Forum.GetLink(Pages.admin_forums));
			writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Banned IP</a></td></tr>",Forum.GetLink(Pages.admin_bannedip));
			writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Smilies</a></td></tr>",Forum.GetLink(Pages.admin_smilies));
			writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Replace Words</a></td></tr>",Forum.GetLink(Pages.admin_replacewords));

			writer.WriteLine("<tr><td nowrap class=\"header2\"><b>Groups and Users</b></td></tr>");
			writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Access Masks</a></td></tr>",Forum.GetLink(Pages.admin_accessmasks));
			writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Groups (Roles)</a></td></tr>",Forum.GetLink(Pages.admin_groups));
			writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Users</a></td></tr>",Forum.GetLink(Pages.admin_users));
			writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Ranks</a></td></tr>",Forum.GetLink(Pages.admin_ranks));
			writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Mail</a></td></tr>",Forum.GetLink(Pages.admin_mail));

			writer.WriteLine("<tr><td nowrap class=\"header2\"><b>Maintenance</b></td></tr>");
			writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Prune Topics</a></td></tr>",Forum.GetLink(Pages.admin_prune));
			writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Private Messages</a></td></tr>",Forum.GetLink(Pages.admin_pm));
			writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Attachments</a></td></tr>",Forum.GetLink(Pages.admin_attachments));
			writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Event Log</a></td></tr>",Forum.GetLink(Pages.admin_eventlog));

			writer.WriteLine("<tr><td nowrap class=\"header2\"><b>NNTP</b></td></tr>");
			writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">NNTP Servers</a></td></tr>",Forum.GetLink(Pages.admin_nntpservers));
			writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">NNTP Forums</a></td></tr>",Forum.GetLink(Pages.admin_nntpforums));
			writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Retrieve Articles</a></td></tr>",Forum.GetLink(Pages.admin_nntpretrieve));

			writer.WriteLine("<tr><td nowrap class=\"header2\"><b>Upgrade</b></td></tr>");
			writer.WriteLine("<tr><td nowrap class=\"post\"><a href=\"{0}\">Version Check</a></td></tr>",Forum.GetLink(Pages.admin_version));
			writer.WriteLine("<tr><td nowrap class=post><a target=\"_top\" href=\"{0}install/\">Install</a></td></tr>",Data.ForumRoot);
			
			writer.WriteLine("</table>");
			writer.WriteLine("</td></tr></table>");

			writer.WriteLine("</td><td valign='top'>&nbsp;&nbsp;");
			writer.WriteLine("</td><td width='90%' valign='top'>");
			
			base.RenderChildren(writer);

			writer.WriteLine("</td></tr></table>");
		}
	}
}
