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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages
{
	/// <summary>
	/// Summary description for _default.
	/// </summary>
	public partial class forum : YAF.Classes.Core.ForumPage
	{
		public forum()
			: base( "DEFAULT" )
		{
		}

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( !IsPostBack )
			{
				ShoutBox1.Visible = PageContext.BoardSettings.ShowShoutbox;
				ActiveDiscussions.Visible = PageContext.BoardSettings.ShowActiveDiscussions;

				if ( PageContext.Settings.LockedForum == 0 )
				{
					PageLinks.AddLink( PageContext.BoardSettings.Name, YafBuildLink.GetLink( ForumPages.forum ) );
					if ( PageContext.PageCategoryID != 0 )
					{
						PageLinks.AddLink( PageContext.PageCategoryName, YafBuildLink.GetLink( ForumPages.forum, "c={0}", PageContext.PageCategoryID ) );
						Welcome.Visible = false;
					}
				}
			}
		}
	}
}
