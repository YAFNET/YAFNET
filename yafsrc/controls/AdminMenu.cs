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
			writer.WriteLine(@"<table width=""100%"" cellspacing=""0"" cellpadding=""0""><tr>");
			writer.WriteLine(@"<td width=""10%"" valign=""top"">");

			writer.WriteLine(@"<table class=""content"" width=""100%"" cellspacing=""0"" cellpadding=""0""><tr><td class=""post"" valign=""top"">");
			writer.WriteLine(@"<table width=""100%"" cellspacing=""0"" cellpadding=""0"">");
			
			if (ForumPage.IsHostAdmin) 
			{
				// host admin -- create host admin menu...
				string [,] hostAdminLinks =
				{
					{"Host Settings",Forum.GetLink(Pages.admin_hostsettings)},
					{"Boards",Forum.GetLink(Pages.admin_boards)}
				};

				writer.Write( createMenu( "Host Admin", "header2", "post", ref hostAdminLinks ) );
			}

			// create the admin menu...
			string [,] adminLinks =
			{
				{"Admin Index",Forum.GetLink(Pages.admin_admin)},
				{"Board Settings",Forum.GetLink(Pages.admin_boardsettings)},
				{"Forums",Forum.GetLink(Pages.admin_forums)},
				{"Banned IP",Forum.GetLink(Pages.admin_bannedip)},
				{"Smilies",Forum.GetLink(Pages.admin_smilies)},
				{"Replace Words",Forum.GetLink(Pages.admin_replacewords)}
			};

			writer.Write(createMenu( "Admin", "header2", "post", ref adminLinks ));

			// create the groups and users menu...
			string [,] groupsAndUsersLinks =
			{
				{"Access Masks",Forum.GetLink(Pages.admin_accessmasks)},
				{"Groups (Roles)",Forum.GetLink(Pages.admin_groups)},
				{"Users",Forum.GetLink(Pages.admin_users)},
				{"Ranks",Forum.GetLink(Pages.admin_ranks)},
				{"Mail",Forum.GetLink(Pages.admin_mail)}
			};

			writer.Write(createMenu( "Groups and Users", "header2", "post", ref groupsAndUsersLinks ));

			// create maintenance menu...
			string [,] maintenanceLinks =
			{
				{"Prune Topics",Forum.GetLink(Pages.admin_prune)},
				{"Private Messages",Forum.GetLink(Pages.admin_pm)},
				{"Attachments",Forum.GetLink(Pages.admin_attachments)},
				{"Event Log",Forum.GetLink(Pages.admin_eventlog)}
			};

			writer.Write( createMenu( "Maintenance", "header2", "post", ref maintenanceLinks ) );

			// create NNTP menu...
			string [,] nntpMenu =
			{
				{"NNTP Servers",Forum.GetLink(Pages.admin_nntpservers)},
				{"NNTP Forums",Forum.GetLink(Pages.admin_nntpforums)},
				{"Retrieve Articles",Forum.GetLink(Pages.admin_nntpretrieve)}
			};

			writer.Write( createMenu( "NNTP", "header2", "post", ref nntpMenu ) );

			// create NNTP menu...
			string [,] upgradeMenu =
			{
				{"Version Check",Forum.GetLink(Pages.admin_version)},
				{"Install",Data.ForumRoot + "install/"}
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
