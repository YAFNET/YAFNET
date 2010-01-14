/* Yet Another Forum.net
 * Copyright (C) 2006-2010 Jaben Cargman
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
using System.Data;
using System.Linq;

namespace YAF.Classes.Utils
{
  /// <summary>
  /// The db helper.
  /// </summary>
  public static class DBHelper
  {
    /// <summary>
    /// Gets the specified column of the first row as the type specified. If not available returns default value.
    /// </summary>
    /// <typeparam name="T">
    /// Type if column to return
    /// </typeparam>
    /// <param name="dt">
    /// DataTable to pull from
    /// </param>
    /// <param name="columnName">
    /// Name of column to convert
    /// </param>
    /// <param name="defaultValue">
    /// value to return if something is not available
    /// </param>
    /// <returns>
    /// </returns>
    public static T GetFirstRowColumnAsValue<T>(this DataTable dt, string columnName, T defaultValue)
    {
      if (dt.Rows.Count > 0 && dt.Columns.Contains(columnName))
      {
        return dt.Rows[0][columnName].ToType<T>();
      }

      return defaultValue;
    }

    /// <summary>
    /// Converts a DataTable to a List of type T using the convert function.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    /// <param name="dt">
    /// </param>
    /// <param name="convertFunction">
    /// </param>
    /// <returns>
    /// </returns>
    public static List<T> ToListObject<T>(this DataTable dt, Func<DataRow, T> convertFunction)
    {
      return (from x in dt.AsEnumerable()
              select convertFunction(x)).ToList();
    }

    /// <summary>
    /// Converts the first column of a DataTable to a generic List of type T.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    /// <param name="dataTable">
    /// </param>
    /// <returns>
    /// </returns>
    public static List<T> GetFirstColumnAsList<T>(this DataTable dataTable)
    {
      return (from x in dataTable.AsEnumerable()
              select x.Field<T>(0)).ToList();
    }

    /// <summary>
    /// Converts columnName in a DataTable to a generic List of type T.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    /// <param name="dataTable">
    /// </param>
    /// <param name="columnName">
    /// </param>
    /// <returns>
    /// </returns>
    public static List<T> GetColumnAsList<T>(this DataTable dataTable, string columnName)
    {
      return (from x in dataTable.AsEnumerable()
              select x.Field<T>(columnName)).ToList();
    }

    /// <summary>
    /// Gets the first row (DataRow) of a DataTable.
    /// </summary>
    /// <param name="dt">
    /// </param>
    /// <returns>
    /// </returns>
    public static DataRow GetFirstRow(this DataTable dt)
    {
      if (dt.Rows.Count > 0)
      {
        return dt.Rows[0];
      }

      return null;
    }

    /// <summary>
    /// Gets the first row of the data table or redirects to invalid request
    /// </summary>
    /// <param name="dt">
    /// </param>
    /// <returns>
    /// </returns>
    public static DataRow GetFirstRowOrInvalid(DataTable dt)
    {
      DataRow row = dt.GetFirstRow();

      if (row != null)
      {
        return row;
      }

      // fail...
      YafBuildLink.RedirectInfoPage(InfoMessage.Invalid);

      return null;
    }

    /// <summary>
    /// Tests if an DB object (in DataRow) is DBNull.Value, null or empty.
    /// </summary>
    /// <param name="columnValue">
    /// </param>
    /// <returns>
    /// The is null or empty db field.
    /// </returns>
    public static bool IsNullOrEmptyDBField(this object columnValue)
    {
      if (columnValue == DBNull.Value)
      {
        return true;
      }
      else if (String.IsNullOrEmpty(columnValue.ToString().Trim()))
      {
        return true;
      }

      return false;
    }
  }
}