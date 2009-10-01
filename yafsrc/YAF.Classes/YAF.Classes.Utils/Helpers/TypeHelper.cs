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
	public static class TypeHelper
	{
		static public bool DbStringIsNullOrEmpty(object dbString)
		{
			if (dbString == DBNull.Value) return true;
			if (String.IsNullOrEmpty(dbString.ToString().Trim())) return true;
			return false;
		}

		/// <summary>
		/// Converts an object to a type.
		/// </summary>
		/// <param name="value">Object to convert</param>
		/// <param name="type">Type to convert to e.g. System.Guid</param>
		/// <returns></returns>
		static public object ConvertObjectToType(object value, string type)
		{
			Type convertType;

			try
			{
				convertType = Type.GetType(type, true, true);
			}
			catch
			{
				convertType = Type.GetType("System.Guid", false);
			}

			if (value.GetType().ToString() == "System.String")
			{
				switch (convertType.ToString())
				{
					case "System.Guid":
						// do a "manual conversion" from string to Guid
						return new System.Guid(Convert.ToString(value));
					case "System.Int32":
						return Convert.ToInt32(value);
					case "System.Int64":
						return Convert.ToInt64(value);
				}
			}

			return Convert.ChangeType(value, convertType);
		}

		/// <summary>
		/// Gets an Int from an Object value
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		static public int ValidInt(object expression)
		{
			int value = 0;

			if (expression != null)
			{
				try
				{
					int.TryParse(expression.ToString(), out value);
				}
				catch
				{

				}
			}

			return value;
		}

		static public int VerifyInt32(object o)
		{
			return Convert.ToInt32(o);
		}

		static public bool VerifyBool(object o)
		{
			return Convert.ToBoolean(o);
		}

		static public T ConvertToClass<T>(object instance) where T : class
		{
			if ( instance is T )
			{
				return instance as T;
			}

			return null;
		}

		static public List<T> ConvertDataTableFirstColumnToList<T>( DataTable dataTable )
		{
			List<T> list = new List<T>();

			foreach ( DataRow item in dataTable.Rows )
			{
				list.Add( (T)Convert.ChangeType( item[0], typeof( T ) ) );
			}

			return list;
		}

		static public List<T> ConvertDataTableColumnToList<T>( string columnName, DataTable dataTable )
		{
			List<T> list = new List<T>();

			foreach( DataRow item in dataTable.Rows)
			{
				list.Add( (T)Convert.ChangeType( item[columnName], typeof( T ) ) );
			}

			return list;
		}

		static public List<T> ConvertToGenericList<T>( IList listObjects )
		{
			List<T> convertedList = new List<T>( listObjects.Count );

			foreach ( object listObject in listObjects )
			{
				convertedList.Add( (T)listObject );
			}

			return convertedList;
		}

		static public object[] GetCustomAttributes( Type objectType, Type attributeType )
		{
			object[] myAttrOnType = objectType.GetCustomAttributes( attributeType, false );
			if ( myAttrOnType.Length > 0 )
			{
				return myAttrOnType;
			}

			return null;
		}
	}
}
