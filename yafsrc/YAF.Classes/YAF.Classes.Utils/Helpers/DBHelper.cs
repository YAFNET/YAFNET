/* Yet Another Forum.net
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
using System.Data;
using System.Text;

namespace YAF.Classes.Utils
{
	public static class DBHelper
	{
		/// <summary>
		/// Gets the specified column of the first row as the type specified. If not available returns default value.
		/// </summary>
		/// <typeparam name="T">Type if column to return</typeparam>
		/// <param name="dt">DataTable to pull from</param>
		/// <param name="columnName">Name of column to convert</param>
		/// <param name="defaultValue">value to return if something is not available</param>
		/// <returns></returns>
		static public T GetFirstRowColumnAsValue<T>( DataTable dt, string columnName, T defaultValue )
		{
			if ( dt.Rows.Count > 0 && dt.Columns.Contains( columnName ) )
			{
				return (T) Convert.ChangeType( dt.Rows[0][columnName], typeof ( T ) );
			}

			return defaultValue;
		}	

		/// <summary>
		/// Gets the first row of the data table or redirects to invalid request
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		static public DataRow GetFirstRowOrInvalid( DataTable dt )
		{
			if ( dt.Rows.Count > 0 )
			{
				return dt.Rows[0];
			}

			// fail...
			YafBuildLink.RedirectInfoPage( InfoMessage.Invalid );

			return null;
		}		
	}
}
