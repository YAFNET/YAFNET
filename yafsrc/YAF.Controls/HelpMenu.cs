/* Yet Another Forum.NET
 * Copyright (C) 2006-2008 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
using System;

namespace YAF.Controls
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
			
			if(PageContext.IsHostAdmin) 
			{
				/*
				writer.WriteLine("<tr><td nowrap class=\"header2\"><b>Host Admin</b></td></tr>");
				writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Host Settings</a></td></tr>",YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.admin_hostsettings));
				writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Boards</a></td></tr>",YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.admin_boards));
				*/
			}
			if(PageContext.IsAdmin) 
			{
				/*
				writer.WriteLine("<tr><td nowrap class=\"header2\"><b>Admin</b></td></tr>");
				writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Admin Index</a></td></tr>",YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.admin_admin));
				writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Board Settings</a></td></tr>",YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.admin_PageContext.BoardSettings));
				writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Forums</a></td></tr>",YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.admin_forums));
				writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Banned IP</a></td></tr>",YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.admin_bannedip));
				writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Smilies</a></td></tr>",YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.admin_smilies));
				writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Replace Words</a></td></tr>",YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.admin_replacewords));
				*/
			}

			writer.WriteLine("<tr><td nowrap class=\"header2\"><b>Topics</b></td></tr>");
			writer.WriteLine("<tr><td nowrap class=post><a href=\"{0}\">Recover lost passwords</a></td></tr>",YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.help_recover));

			writer.WriteLine("</table>");
			writer.WriteLine("</td></tr></table>");

			writer.WriteLine("</td><td valign='top'>&nbsp;&nbsp;");
			writer.WriteLine("</td><td width='90%' valign='top'>");
			
			base.RenderChildren(writer);

			writer.WriteLine("</td></tr></table>");
		}
	}
}
