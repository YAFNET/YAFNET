using System;

namespace yaf.controls
{
	/// <summary>
	/// Summary description for AdminMenu.
	/// </summary>
	public class HelpMenu : BaseControl
	{
		public HelpMenu()
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
				/*
				writer.WriteLine("<tr><td nowrap class=\"header2\"><b>Host Admin</b></td></tr>");
				writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Host Settings</a></td></tr>",Forum.GetLink(Pages.admin_hostsettings));
				writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Boards</a></td></tr>",Forum.GetLink(Pages.admin_boards));
				*/
			}
			if(ForumPage.IsAdmin) 
			{
				/*
				writer.WriteLine("<tr><td nowrap class=\"header2\"><b>Admin</b></td></tr>");
				writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Admin Index</a></td></tr>",Forum.GetLink(Pages.admin_admin));
				writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Board Settings</a></td></tr>",Forum.GetLink(Pages.admin_boardsettings));
				writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Forums</a></td></tr>",Forum.GetLink(Pages.admin_forums));
				writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Banned IP</a></td></tr>",Forum.GetLink(Pages.admin_bannedip));
				writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Smilies</a></td></tr>",Forum.GetLink(Pages.admin_smilies));
				writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Replace Words</a></td></tr>",Forum.GetLink(Pages.admin_replacewords));
				*/
			}

			writer.WriteLine("<tr><td nowrap class=\"header2\"><b>Topics</b></td></tr>");
			writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Recover lost passwords</a></td></tr>",Forum.GetLink(Pages.help_recover));

			writer.WriteLine("</table>");
			writer.WriteLine("</td></tr></table>");

			writer.WriteLine("</td><td valign='top'>&nbsp;&nbsp;");
			writer.WriteLine("</td><td width='90%' valign='top'>");
			
			base.RenderChildren(writer);

			writer.WriteLine("</td></tr></table>");
		}
	}
}
