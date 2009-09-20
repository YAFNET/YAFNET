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
using System.Linq;
using System.Text;

namespace YAF.Classes.Pattern
{
	/// <summary>
	/// Allows basic type conversion of Dictionary objects.
	/// </summary>
	public class TypeDictionary : Dictionary<string, object>
	{
		public T AsType<T>( string key )
		{
			return (T) Convert.ChangeType( this[key], typeof ( T ) );
		}

		public bool AsBoolean( string key )
		{
			return AsType<bool>( key );
		}

		public int AsInt( string key )
		{
			return AsType<int>( key );
		}

		public string AsString( string key )
		{
			return AsType<string>( key );
		}
	}
}
