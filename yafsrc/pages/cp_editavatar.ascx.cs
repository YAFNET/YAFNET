/* Yet Another Forum.net
 * Copyright (C) 2006 Jaben Cargman
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

namespace yaf.pages
{
	public partial class cp_editavatar : ForumPage
	{
		public cp_editavatar()
			: base( "CP_EDITAVATAR" )
		{
		}

		protected void Page_Load( object sender, EventArgs e )
		{
			if ( User == null )
			{
				if ( CanLogin )
					Forum.Redirect( Pages.login, "ReturnUrl={0}", Utils.GetSafeRawUrl() );
				else
					Forum.Redirect( Pages.forum );
			}

			if ( !IsPostBack )
			{
				PageLinks.AddLink( BoardSettings.Name, Forum.GetLink( Pages.forum ) );
				PageLinks.AddLink( PageUserName, Forum.GetLink( Pages.cp_profile ) );
				PageLinks.AddLink( GetText( "TITLE" ), "" );
			}
		}
	}
}