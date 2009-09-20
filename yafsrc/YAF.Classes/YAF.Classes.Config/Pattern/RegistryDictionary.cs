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
	/// Provides a method for automatic overriding of a base hash...
	/// </summary>
	public class RegistryDictionaryOverride : RegistryDictionary
	{
		private bool _defaultGetOverride = true;
		public bool DefaultGetOverride
		{
			get
			{
				return _defaultGetOverride;
			}
			set
			{
				_defaultGetOverride = value;
			}
		}

		private bool _defaultSetOverride = false;
		public bool DefaultSetOverride
		{
			get
			{
				return _defaultSetOverride;
			}
			set
			{
				_defaultSetOverride = value;
			}
		}

		public RegistryDictionary OverrideDictionary
		{
			get;
			set;
		}

		public override T GetValue<T>( string name, T defaultValue )
		{
			return this.GetValue<T>( name, defaultValue, DefaultGetOverride );
		}

		public virtual T GetValue<T>( string name, T defaultValue, bool allowOverride )
		{
			if ( allowOverride && OverrideDictionary != null && OverrideDictionary.ContainsKey( name.ToLower() ) &&
			     OverrideDictionary[name.ToLower()] != null )
			{
				return OverrideDictionary.GetValue<T>( name, defaultValue );
			}

			// just pull the value from this dictionary...
			return base.GetValue<T>( name, defaultValue );
		}

		public override void SetValue<T>( string name, T value )
		{
			this.SetValue<T>( name, value, DefaultSetOverride );
		}

		public virtual void SetValue<T>( string name, T value, bool setOverrideOnly )
		{
			if ( OverrideDictionary != null )
			{
				if ( setOverrideOnly )
				{
					// just set the override dictionary...
					OverrideDictionary.SetValue<T>( name, value );
					return;
				}
				else if ( OverrideDictionary.ContainsKey( name.ToLower() ) &&
			     OverrideDictionary[name.ToLower()] != null )
				{
					// set the overriden value to null/erase it...
					OverrideDictionary.SetValue<T>( name, (T)Convert.ChangeType( null, typeof( T ) ) );
				}
			}

			// save new value in the base...
			base.SetValue<T>( name, value );
		}
	}

	public class RegistryDictionary : Dictionary<string, object>
	{
		/* Ederon : 6/16/2007 -- modified by Jaben 7/17/2009 */
		public virtual T GetValue<T>( string name, T defaultValue )
		{
			if ( !ContainsKey( name.ToLower() ) ) return defaultValue;

			object value = this[name.ToLower()];

			if ( value == null ) return defaultValue;

			// special handling for boolean...
			if ( typeof( T ) == typeof( bool ) )
			{
				int i;
				return int.TryParse( value.ToString(), out i )
				       	? (T)Convert.ChangeType( Convert.ToBoolean( i ), typeof( T ) )
				       	: (T)Convert.ChangeType( Convert.ToBoolean( value ), typeof( T ) );
			}
			// special handling for int values...
			if ( typeof( T ) == typeof( int ) )
			{
				return (T)Convert.ChangeType( Convert.ToInt32( value ), typeof( T ) );
			}

			return (T)Convert.ChangeType( this[name.ToLower()], typeof( T ) );
		}

		public virtual void SetValue<T>( string name, T value )
		{
			this[name.ToLower()] = typeof( T ).BaseType == typeof( bool ) ? Convert.ToString( Convert.ToInt32( value ) ) : Convert.ToString( value );
		}
		/* 6/16/2007 */
	}
}
