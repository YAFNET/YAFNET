/* YetAnotherForum.NET
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
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;
using YAF.Controls;

namespace YAF.Modules
{
	/// <summary>
	/// Summary description for PageTitleModule
	/// </summary>
	[YafModule( "Page Logo Handler Module", "Tiny Gecko", 1 )]
	public class PageLogoHandlerModule : SimpleBaseModule
	{
		public PageLogoHandlerModule()
		{
		}

		override public void InitAfterPage()
		{
			CurrentForumPage.PreRender += new EventHandler( ForumPage_PreRender );
		}

		override public void InitBeforePage()
		{

		}

		void ForumPage_PreRender( object sender, EventArgs e )
		{
			HtmlImage htmlImgBanner = ControlHelper.FindControlRecursiveBothAs<HtmlImage>( CurrentForumPage, "imgBanner" );
			Image imgBanner = ControlHelper.FindControlRecursiveBothAs<Image>( CurrentForumPage, "imgBanner" );

			if ( !CurrentForumPage.ShowToolBar )
			{
				if ( htmlImgBanner != null ) htmlImgBanner.Visible = false;
				else if ( imgBanner != null ) imgBanner.Visible = false;
			}

			if ( PageContext.BoardSettings.AllowThemedLogo && !Config.IsAnyPortal )
			{
				string graphicSrc = PageContext.Theme.GetItem( "FORUM", "BANNER", null );

				if ( !String.IsNullOrEmpty( graphicSrc ) )
				{
					if ( htmlImgBanner != null ) htmlImgBanner.Src = graphicSrc;
					else if ( imgBanner != null ) imgBanner.ImageUrl = graphicSrc;
				}
			}
		}
	}
}