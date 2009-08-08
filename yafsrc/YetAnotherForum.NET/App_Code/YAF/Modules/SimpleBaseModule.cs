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
using System.Web;
using YAF.Classes.Core;
using YAF.Classes;
using YAF.Classes.Data;

namespace YAF.Modules
{
	/// <summary>
	/// Summary description for SimpleBaseModule
	/// </summary>
	public class SimpleBaseModule : IBaseModule
	{
		virtual public void InitAfterPage()
		{
			
		}

		virtual public void InitBeforePage()
		{

		}

		#region IBaseModule Basic Members

		protected YafContext _pageContext;
		public YafContext PageContext
		{
			get
			{
				return _pageContext;
			}
			set
			{
				_pageContext = value;
			}
		}

		protected ForumPage _currentForumPage;
		public ForumPage CurrentForumPage
		{
			get
			{
				return PageContext.CurrentForumPage;
			}
		}

		protected object _forumControlObj;
		public object ForumControlObj
		{
			get
			{
				return _forumControlObj;
			}
			set
			{
				_forumControlObj = value;
			}
		}

		public Forum ForumControl
		{
			get
			{
				return (Forum)ForumControlObj;
			}
		}

		protected ForumPages _forumPageType;
		public ForumPages ForumPageType
		{
			get
			{
				return PageContext.ForumPageType;
			}
		}

		#endregion

		#region IBaseModule Module Information

		virtual public string ModuleAuthor
		{
			get
			{
				return "";
			}
		}

		virtual public string ModuleName
		{
			get
			{
				return "";
			}
		}

		virtual public int ModuleVersion
		{
			get
			{
				return 0;
			}
		}

		#endregion

		#region IDisposable Members

		virtual public void Dispose()
		{

		}

		#endregion
	}
}