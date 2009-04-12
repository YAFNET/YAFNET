/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Classes.Base
{
	/// <summary>
	/// Summary description for AdminPage.
	/// </summary>
	public class AdminPage : ForumPage
	{
		/// <summary>
		/// Creates the Administration page.
		/// </summary>
		public AdminPage()
			: base( null )
		{
			this.Load += new EventHandler( AdminPage_Load );
		}

		public AdminPage( string transPage )
			: base( transPage )
		{
			this._isAdminPage = true;
			this.Load += new EventHandler( AdminPage_Load );
		}


		private void AdminPage_Load( object sender, EventArgs e )
		{
			if ( !PageContext.IsAdmin )
				YafBuildLink.AccessDenied();

#if false
			if(!IsPostBack)
			{
				controls.PageLinks ctl = new controls.PageLinks();
				ctl.AddLink(BoardSettings.Name,YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum));
				ctl.AddLink("Administration",YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.admin_admin));
				Controls.AddAt(0,ctl);
			}
#endif
		}
	}
}
