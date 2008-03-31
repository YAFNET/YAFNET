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
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;
using YAF.Classes.Data;
using YAF.Classes.UI;

namespace YAF.Pages // YAF.Pages
{
	/// <summary>
	///		Summary description for LastPosts.
	/// </summary>
	public partial class lastposts : YAF.Classes.Base.ForumPage
	{
		public lastposts()
			: base( "POSTMESSAGE" )
		{
			ShowToolBar = false;
		}

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( !PageContext.ForumReadAccess )
				YafBuildLink.AccessDenied();

			if ( Request.QueryString ["t"] != null )
			{
				repLastPosts.DataSource = YAF.Classes.Data.DB.post_list_reverse10( Request.QueryString ["t"] );
				repLastPosts.DataBind();
			}

			// handle custom BBCode javascript or CSS...
			BBCode.RegisterCustomBBCodePageElements( Page, this.GetType() );
		}
	}
}
