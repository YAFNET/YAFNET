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

namespace YAF.Modules
{
	public abstract class YafModuleManager<T>
	{
		protected string _cacheName = String.Empty;
		protected List<T> _modules = null;
		protected List<Type> _moduleClassTypes = null;
		protected bool _loaded = false;

		public event EventHandler<EventArgs> LoadModules;
		public event EventHandler<EventArgs> UnloadModules;

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

		protected YafModuleManager( IList assemblies, string moduleNamespace, string moduleBaseType )
		{
			_cacheName = "YafModuleManager_" + moduleBaseType;
			_modules = new List<T>();
			
			if (YafContext.Current.Cache[_cacheName] == null)
			{
				// load modules...
				YafContext.Current.Cache[_cacheName] = FindModules( assemblies, moduleNamespace, moduleBaseType );
			}

			_moduleClassTypes = YafContext.Current.Cache[_cacheName] as List<Type>;
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
				foreach ( Type module in _moduleClassTypes )
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
