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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.UI;
using YAF.Classes.Utils;
using YAF.Controls;

namespace YAF.Modules
{
	/// <summary>
	/// Summary description for PageTitleModule
	/// </summary>
	public class PageBBCodeRegistration : SimpleBaseModule
	{
		public PageBBCodeRegistration()
		{
		}

		override public void InitAfterPage()
		{
			switch ( PageContext.ForumPageType )
			{
				case ForumPages.cp_message:
				case ForumPages.search:
				case ForumPages.lastposts:
				case ForumPages.posts:
				case ForumPages.profile:
					YAF.Classes.UI.YafBBCode.RegisterCustomBBCodePageElements(PageContext.CurrentForumPage.Page, PageContext.CurrentForumPage.GetType() );
					break;
			}
		}
	}
}