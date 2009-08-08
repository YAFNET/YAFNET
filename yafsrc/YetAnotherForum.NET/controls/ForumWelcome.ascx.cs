/* Yet Another Forum.NET
 * Copyright (C) 2006-2009 Jaben Cargman
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
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;

namespace YAF.Controls
{
	public partial class ForumWelcome : YAF.Classes.Core.BaseUserControl
	{
		public ForumWelcome()
		{
			this.PreRender += new EventHandler( ForumWelcome_PreRender );
		}

		void ForumWelcome_PreRender( object sender, EventArgs e )
		{
			TimeNow.Text = PageContext.Localization.GetTextFormatted( "Current_Time", YafServices.DateTime.FormatTime( DateTime.Now ) );
			TimeLastVisit.Text = PageContext.Localization.GetTextFormatted( "last_visit", YafServices.DateTime.FormatDateTime( Mession.LastVisit ) );

			if ( PageContext.UnreadPrivate > 0 )
			{
				UnreadMsgs.Visible = true;
				UnreadMsgs.NavigateUrl = YafBuildLink.GetLink( ForumPages.cp_pm );
				if ( PageContext.UnreadPrivate == 1 )
				{
					UnreadMsgs.Text = PageContext.Localization.GetTextFormatted( "unread1", PageContext.UnreadPrivate );
				}
				else
				{
					UnreadMsgs.Text = PageContext.Localization.GetTextFormatted( "unread0", PageContext.UnreadPrivate );
				}
			}
		}
	}
}