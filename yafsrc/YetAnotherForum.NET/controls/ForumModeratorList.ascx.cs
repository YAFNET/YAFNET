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

namespace YAF.Controls
{
	public partial class ForumModeratorList : YAF.Classes.Base.BaseUserControl
	{
		public ForumModeratorList()
		{
			this.PreRender += new EventHandler( ForumModeratorList_PreRender );
		}

		void ForumModeratorList_PreRender( object sender, EventArgs e )
		{	
			if ( ((DataRow[])ModeratorList.DataSource).Length > 0)
			{
				// no need for the "blank dash"...
				BlankDash.Visible = false;
			}
		}

		public System.Collections.IEnumerable DataSource
		{
			set
			{
				ModeratorList.DataSource = value;
			}
		}

	}
}
