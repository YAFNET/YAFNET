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
using YAF.Classes.Utils;

namespace YAF.Controls
{
	/// <summary>
	/// Summary description for AdminMenu.
	/// </summary>
	public class AdminMenu : BaseControl
	{
		public AdminMenu()
		{

		}
	
		protected override void Render(System.Web.UI.HtmlTextWriter writer) 
		{	
			writer.WriteLine(@"<table width=""100%"" cellspacing=""0"" cellpadding=""0""><tr>");
			writer.WriteLine(@"<td width=""10%"" valign=""top"">");

			writer.WriteLine(@"<table class=""content"" width=""100%"" cellspacing=""0"" cellpadding=""0""><tr><td class=""post"" valign=""top"">");
			writer.WriteLine(@"<table width=""100%"" cellspacing=""0"" cellpadding=""0"">");
			
			if (PageContext.IsHostAdmin) 
			{
				// host admin -- create host admin menu...
				string [,] hostAdminLinks =
				{
					{"Host Settings",YafBuildLink.GetLink( ForumPages.admin_hostsettings)},
					{"Boards",YafBuildLink.GetLink( ForumPages.admin_boards)}
				};

				writer.Write( createMenu( "Host Admin", "header2", "post", ref hostAdminLinks ) );
			}

			// create the admin menu...
			string [,] adminLinks =
			{
				{"Admin Index",YafBuildLink.GetLink( ForumPages.admin_admin)},
				{"Board Settings",YafBuildLink.GetLink( ForumPages.admin_boardsettings)},
				{"Forums",YafBuildLink.GetLink( ForumPages.admin_forums)},
				{"Banned IP",YafBuildLink.GetLink( ForumPages.admin_bannedip)},
				{"Smilies",YafBuildLink.GetLink( ForumPages.admin_smilies)},
				{"Replace Words",YafBuildLink.GetLink( ForumPages.admin_replacewords)},
                {"File Extensions",YafBuildLink.GetLink( ForumPages.admin_extensions)}
			};

			writer.Write(createMenu( "Admin", "header2", "post", ref adminLinks ));

			// create the groups and users menu...
			string [,] groupsAndUsersLinks =
			{
				{"Access Masks",YafBuildLink.GetLink( ForumPages.admin_accessmasks)},
				{"Roles",YafBuildLink.GetLink( ForumPages.admin_groups)},
				{"Users",YafBuildLink.GetLink( ForumPages.admin_users)},
				{"Ranks",YafBuildLink.GetLink( ForumPages.admin_ranks)},
				{"Medals",YafBuildLink.GetLink( ForumPages.admin_medals)},
				{"Mail",YafBuildLink.GetLink( ForumPages.admin_mail)}
			};

			writer.Write(createMenu( "Users and Roles", "header2", "post", ref groupsAndUsersLinks ));

			// create maintenance menu...
			string [,] maintenanceLinks =
			{
				{"Prune Topics",YafBuildLink.GetLink( ForumPages.admin_prune)},
				{"Private Messages",YafBuildLink.GetLink( ForumPages.admin_pm)},
				{"Attachments",YafBuildLink.GetLink( ForumPages.admin_attachments)},
				{"Event Log",YafBuildLink.GetLink( ForumPages.admin_eventlog)}
			};

			writer.Write( createMenu( "Maintenance", "header2", "post", ref maintenanceLinks ) );

			// create NNTP menu...
			string [,] nntpMenu =
			{
				{"NNTP Servers",YafBuildLink.GetLink( ForumPages.admin_nntpservers)},
				{"NNTP Forums",YafBuildLink.GetLink( ForumPages.admin_nntpforums)},
				{"Retrieve Articles",YafBuildLink.GetLink( ForumPages.admin_nntpretrieve)}
			};

			writer.Write( createMenu( "NNTP", "header2", "post", ref nntpMenu ) );

			// create NNTP menu...
			string [,] upgradeMenu =
			{
				{"Version Check",YafBuildLink.GetLink( ForumPages.admin_version)},
				{"Install",YafForumInfo.ForumRoot + "install/"}
			};

			writer.Write( createMenu( "Upgrade", "header2", "post", ref upgradeMenu ) );
		
			writer.WriteLine(@"</table>");
			writer.WriteLine(@"</td></tr></table>");

			writer.WriteLine(@"</td><td valign=""top"">&nbsp;&nbsp;");
			writer.WriteLine(@"</td><td width=""90%"" valign=""top"">");
			
			base.RenderChildren(writer);

			writer.WriteLine(@"</td></tr></table>");
		}

		protected string createMenu( string menuTitle, string menuClass, string itemClass, ref string [,] menuItems )
		{
			string returnValue;

			returnValue = createHeader( menuClass, menuTitle ) + "\r\n";

			for ( int i = 0; i < menuItems.GetUpperBound(0)+1; i++ )
			{
				returnValue += createLink( itemClass, menuItems [i,0], menuItems [i,1] ) + "\r\n";	
			}

			return returnValue;
		}

		protected string createHeader( string className, string headerText )
		{
			return string.Format( @"<tr><td nowrap=""nowrap"" class=""{0}""><b>{1}</b></td></tr>", className, headerText );
		}

		protected string createLink( string className, string linkText, string linkURL )
		{
			return string.Format( @"<tr><td nowrap=""nowrap"" class=""{0}""><a href=""{1}"">{2}</a></td></tr>", className, linkURL, linkText );
		}
	}
}
