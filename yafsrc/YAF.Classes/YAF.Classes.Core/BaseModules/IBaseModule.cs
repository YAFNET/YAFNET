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
using System.Web.Compilation;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Data;

namespace YAF.Modules
{
	public interface IBaseModule : IDisposable
	{
		YafContext PageContext
		{
			get;
			set;
		}
		object ForumControlObj
		{
			get;
			set;
		}

		string ModuleName
		{
			get;
		}

		string ModuleAuthor
		{
			get;
		}

		int ModuleVersion
		{
			get;
		}

		void InitBeforePage();
		void InitAfterPage();
	}

	/// <summary>
	/// Handles IBaseModule types.
	/// </summary>
	public class YafBaseModuleManager : YafModuleManager<IBaseModule>
	{
		YafBaseModuleManager()
			: base(BuildManager.CodeAssemblies, "YAF.Modules", "YAF.Modules.IBaseModule")
		{

		}

		public void InitModulesBeforeForumPage(YafContext currentContext, object forumControl)
		{
			foreach (IBaseModule currentModule in Modules)
			{
				currentModule.ForumControlObj = forumControl;
				currentModule.PageContext = currentContext;
				currentModule.InitBeforePage();
			}
		}

		public void InitModulesAfterForumPage()
		{
			foreach (IBaseModule currentModule in Modules)
			{
				currentModule.InitAfterPage();
			}
		}
	}
}
