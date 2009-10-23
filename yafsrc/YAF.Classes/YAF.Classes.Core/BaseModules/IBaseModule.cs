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
using System.Collections.Generic;
using System.Reflection;
using System.Web.Compilation;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Data;

namespace YAF.Modules
{
	[AttributeUsage( AttributeTargets.Class )]
	public class YafModule : System.Attribute
	{
		private string _moduleName;
		public string ModuleName
		{
			get
			{
				return _moduleName;
			}
			set
			{
				_moduleName = value;
			}
		}

		private string _moduleAuthor;
		public string ModuleAuthor
		{
			get
			{
				return _moduleAuthor;
			}
			set
			{
				_moduleAuthor = value;
			}
		}

		private int _moduleVersion;
		public int ModuleVersion
		{
			get
			{
				return _moduleVersion;
			}
			set
			{
				_moduleVersion = value;
			}
		}

		public YafModule( string moduleName, string moduleAuthor, int moduleVersion )
		{
			_moduleName = moduleName;
			_moduleAuthor = moduleAuthor;
			_moduleVersion = moduleVersion;
		}
	}

	public interface IBaseModule : IDisposable
	{
		object ForumControlObj
		{
			get;
			set;
		}

		void Init();
	}

	/// <summary>
	/// Handles IBaseModule types.
	/// </summary>
	public class YafBaseModuleManager : YafModuleManager<IBaseModule>
	{
		protected bool _initCalled = false;

		YafBaseModuleManager()
			: base( "YAF.Modules", "YAF.Modules.IBaseModule" )
		{
			if ( ModuleClassTypes == null )
			{
				// get the .Core module to add...
				base.AddModules( new List<Assembly>() {Assembly.GetExecutingAssembly()} );
				// re-add these modules...
				base.AddModules( BuildManager.CodeAssemblies );
			}
		}

		public void CallInitModules( object forumControl )
		{
			if ( !_initCalled )
			{
				foreach ( IBaseModule currentModule in Modules )
				{
					currentModule.ForumControlObj = forumControl;
					currentModule.Init();
				}

				_initCalled = true;
			}
		}
	}
}
