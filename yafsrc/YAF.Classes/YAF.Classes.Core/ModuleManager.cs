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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using YAF.Classes.Core;
using YAF.Classes.Utils;

namespace YAF.Modules
{
	public abstract class YafModuleManager<T>
	{
		protected string _cacheName = String.Empty;
		protected List<T> _modules = new List<T>();
		protected List<Type> _moduleClassTypes = null;
		protected bool _loaded = false;

		public event EventHandler<EventArgs> LoadModules;
		public event EventHandler<EventArgs> UnloadModules;

		private string _moduleNamespace;
		public String ModuleNamespace
		{
			get
			{
				return _moduleNamespace;
			}
			protected set
			{
				_moduleNamespace = value;
			}
		}

		private string _moduleBaseType;
		public String ModuleBaseType
		{
			get
			{
				return _moduleBaseType;
			}
			protected set
			{
				_moduleBaseType = value;
			}
		}

		protected String CacheName
		{
			get
			{
				return "YafModuleManager_" + ModuleBaseType;
			}
		}

		protected List<Type> ModuleClassTypes
		{
			get
			{
				if ( _moduleClassTypes == null && YafContext.Current.Cache[CacheName] != null )
				{
					_moduleClassTypes = YafContext.Current.Cache[CacheName] as List<Type>;
				}

				return _moduleClassTypes;
			}
			set
			{
				if ( value == null ) YafContext.Current.Cache.Remove( CacheName );
				else YafContext.Current.Cache[CacheName] = value;
				_moduleClassTypes = value;
			}
		}

		public List<T> Modules
		{
			get
			{
				return _modules;
			}
		}

		public bool Loaded
		{
			get
			{
				return _loaded;
			}
		}

		protected YafModuleManager( string moduleNamespace, string moduleBaseType )
		{
			ModuleNamespace = moduleNamespace;
			ModuleBaseType = moduleBaseType;
		}

		protected void InitModules( IList assemblies )
		{
			if ( ModuleClassTypes == null )
			{
				// load modules...
				ModuleClassTypes = FindModules( assemblies, ModuleNamespace, ModuleBaseType );
			}
		}

		protected void InitModules( List<Assembly> assemblies )
		{
			if ( ModuleClassTypes == null )
			{
				// load modules...
				ModuleClassTypes = FindModules( assemblies, ModuleNamespace, ModuleBaseType );
			}
		}

		protected static List<Type> FindModules(IList assemblies, string moduleNamespace, string moduleBaseInterface )
		{
			List<Type> moduleClassTypes = new List<Type>();

				// get classes...
			foreach (Assembly assembly in assemblies)
			{
				TypeFilter filter = new TypeFilter( BaseModuleFilter );

				foreach ( Module module in assembly.GetModules() )
				{
					foreach ( Type modClass in module.FindTypes( filter, moduleNamespace ) )
					{
						// don't add any abstract classes...
						if ( modClass.IsAbstract ) continue;

						// YAF.Modules namespace, verify it implements the interface...
						Type[] interfaces = modClass.GetInterfaces();

						foreach ( Type inter in interfaces )
						{
							if (inter.ToString() == moduleBaseInterface)
								moduleClassTypes.Add(modClass);
						}
					}
				}
			}

			return moduleClassTypes;
		}

		public void Load()
		{
			if (!_loaded)
			{
				foreach ( Type module in ModuleClassTypes )
				{
					_modules.Add( GetInstance( module ) );
				}

				_loaded = true;

				if ( LoadModules != null ) LoadModules( this, new EventArgs());
			}
		}

		public void Unload()
		{
			if ( _loaded )
			{
				_modules.Clear();
				_loaded = false;

				if (UnloadModules != null) UnloadModules(this, new EventArgs());
			}
		}

		protected T GetInstance(Type module)
		{
			return (T) Activator.CreateInstance( module );
		}

		public static bool BaseModuleFilter(Type typeObj, Object criteriaObj)
		{
			if ( typeObj.Namespace == criteriaObj.ToString())
				return true;
			else
				return false;
		}
	}
}
