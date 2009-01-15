/* Yet Another Forum.NET
 * Copyright (C) 2006-2008 Jaben Cargman
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

namespace YAF.Providers.Membership
{
	public static class CleanUtils
	{
		public static DateTime ToDate( object obj )
		{
			if ( obj != DBNull.Value )
				return Convert.ToDateTime( obj.ToString() );
			else
				return DateTime.Now;
		}

		public static string ToString( object obj )
		{
			if ( obj != DBNull.Value )
				return obj.ToString();
			else
				return String.Empty;
		}

		public static bool ToBool( object obj )
		{
			if ( obj != DBNull.Value )
				return Convert.ToBoolean( obj );
			else
				return false;
		}

		public static int ToInt( object obj )
		{
			if ( obj != DBNull.Value )
				return Convert.ToInt32( obj );
			else
				return 0;
		}

        public static string toHexString(byte[] hashedBytes)
        {
            StringBuilder hashedSB = new StringBuilder(hashedBytes.Length * 2 + 2);
            foreach (byte b in hashedBytes)
            {
                hashedSB.AppendFormat("{0:X2}", b);
            }
            return hashedSB.ToString();
        }
	}
}
