/* Yet Another Forum.NET
 * Copyright (C) 2006-2007 Jaben Cargman
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
using YAF.Classes.Utils;

namespace YAF.Controls
{
	public partial class ForumWelcome : YAF.Classes.Base.BaseUserControl
	{
		public ForumWelcome()
		{
			this.PreRender += new EventHandler( ForumWelcome_PreRender );
		}

		void ForumWelcome_PreRender( object sender, EventArgs e )
		{
			TimeNow.Text = String.Format( PageContext.Localization.GetText( "Current_Time" ), YafDateTime.FormatTime( DateTime.Now ) );
			TimeLastVisit.Text = String.Format( PageContext.Localization.GetText( "last_visit" ), YafDateTime.FormatDateTime( Mession.LastVisit ) );

			if ( PageContext.UnreadPrivate > 0 )
			{
				UnreadMsgs.Visible = true;
				UnreadMsgs.NavigateUrl = YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.cp_pm );
				if ( PageContext.UnreadPrivate == 1 )
					UnreadMsgs.Text = String.Format( PageContext.Localization.GetText( "unread1" ), PageContext.UnreadPrivate );
				else
					UnreadMsgs.Text = String.Format( PageContext.Localization.GetText( "unread0" ), PageContext.UnreadPrivate );
			}
		}
	}
}