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
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace YAF.Providers.Utils
{
	public static class Transform
	{
		public static DateTime ToDateTime( object obj )
		{
			if ( ( obj != DBNull.Value ) && ( obj != null ) )
				return Convert.ToDateTime( obj.ToString() );
			else
				return DateTime.Now; // Yeah I admit it, what the hell should this return?
		}

		public static string ToString( object obj )
		{
			if ( ( obj != DBNull.Value ) && ( obj != null ) )
				return obj.ToString();
			else
				return String.Empty;
		}

		public static string ToString( object obj, string defValue )
		{
			if ( ( obj != DBNull.Value ) && ( obj != null ) )
				return obj.ToString();
			else
				return defValue;
		}

		public static string [] ToStringArray( StringCollection coll )
		{
			String [] strReturn = new String [coll.Count];
			coll.CopyTo( strReturn, 0 );
			return strReturn;
		}

		public static bool ToBool( object obj )
		{
			if ( ( obj != DBNull.Value ) && ( obj != null ) )
				return Convert.ToBoolean( obj );
			else
				return false;
		}

		public static int ToInt( object obj )
		{
			if ( ( obj != DBNull.Value ) && ( obj != null ) )
				return Convert.ToInt32( obj );
			else
				return 0;
		}
	}
}
