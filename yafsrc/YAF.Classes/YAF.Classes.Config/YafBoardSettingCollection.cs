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
using System.Linq;
using YAF.Classes.Pattern;

namespace YAF.Classes
{
	/// <summary>
	/// Gets the Board Settings as dictionary item for easy iteration.
	/// </summary>
	public class YafBoardSettingCollection
	{
		protected List<PropertyInfo> _settings = new List<PropertyInfo>();

		public Dictionary<string, PropertyInfo> SettingsString
		{
			get
			{
				return _settings.Where( x => x.PropertyType == typeof ( string ) ).ToDictionary( x => x.Name, x => x );
			}
		}
		public Dictionary<string, PropertyInfo> SettingsBool
		{
			get
			{
				return _settings.Where( x => x.PropertyType == typeof( bool ) ).ToDictionary( x => x.Name, x => x );
			}
		}
		public Dictionary<string, PropertyInfo> SettingsInt
		{
			get
			{
				return _settings.Where( x => x.PropertyType == typeof( int ) ).ToDictionary( x => x.Name, x => x );
			}
		}
		public Dictionary<string, PropertyInfo> SettingsOther
		{
			get
			{
				return
					_settings.Where(
						x =>
						x.PropertyType != typeof ( string ) && x.PropertyType != typeof ( int ) &&
						x.PropertyType != typeof( bool ) ).ToDictionary( x => x.Name, x => x );
			}
		}

		public YafBoardSettingCollection( YafBoardSettings boardSettings )
		{
			// load up the settings...
			Type boardSettingsType = boardSettings.GetType();
			_settings = boardSettingsType.GetProperties().ToList();
		}
	}
}
