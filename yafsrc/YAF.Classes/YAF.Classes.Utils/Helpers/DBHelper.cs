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
namespace YAF.Classes.Utils
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Linq;

  using YAF.Classes.Pattern;

  #endregion

  /// <summary>
  /// The db helper.
  /// </summary>
  public static class DBHelper
  {
    #region Public Methods

    /// <summary>
    /// Selects a typed list of rows using the <paramref name="createNew"/> delegate.
    /// </summary>
    /// <param name="dataTable">
    /// The data table.
    /// </param>
    /// <param name="createNew">
    /// The create new.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IEnumerable<T> SelectTypedList<T>(
      [NotNull] this DataTable dataTable, [NotNull] Func<DataRow, T> createNew)
    {
      CodeContracts.ArgumentNotNull(dataTable, "dataTable");
      CodeContracts.ArgumentNotNull(createNew, "createNew");

      return dataTable.AsEnumerable().Select(createNew);
    }

    /// <summary>
    /// Converts <paramref name="columnName"/> in a <see cref="DataTable"/> to a generic List of type T.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    /// <param name="dataTable">
    /// </param>
    /// <param name="columnName">
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static List<T> GetColumnAsList<T>([NotNull] this DataTable dataTable, [NotNull] string columnName)
    {
      return (from x in dataTable.AsEnumerable() select x.Field<T>(columnName)).ToList();
    }

    /// <summary>
    /// Converts the first column of a <see cref="DataTable"/> to a generic List of type T.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    /// <param name="dataTable">
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static List<T> GetFirstColumnAsList<T>([NotNull] this DataTable dataTable)
    {
      return (from x in dataTable.AsEnumerable() select x.Field<T>(0)).ToList();
    }

    /// <summary>
    /// Gets the first row (<see cref="DataRow"/>) of a <see cref="DataTable"/>.
    /// </summary>
    /// <param name="dt">
    /// </param>
    /// <returns>
    /// </returns>
    public static DataRow GetFirstRow([NotNull] this DataTable dt)
    {
      return dt.Rows.Count > 0 ? dt.Rows[0] : null;
    }

    /// <summary>
    /// Gets the specified column of the first row as the type specified. If not available returns default value.
    /// </summary>
    /// <typeparam name="T">
    /// Type if column to return
    /// </typeparam>
    /// <param name="dt">
    /// <see cref="DataTable"/> to pull from
    /// </param>
    /// <param name="columnName">
    /// Name of column to convert
    /// </param>
    /// <param name="defaultValue">
    /// value to return if something is not available
    /// </param>
    /// <returns>
    /// </returns>
    public static T GetFirstRowColumnAsValue<T>([NotNull] this DataTable dt, [NotNull] string columnName, T defaultValue)
    {
      if (dt.Rows.Count > 0 && dt.Columns.Contains(columnName))
      {
        if (dt.Rows[0][columnName] != DBNull.Value)
        {
          return dt.Rows[0][columnName].ToType<T>();
        }
      }

      return defaultValue;
    }

    /// <summary>
    /// Gets the first row of the data table or redirects to invalid request
    /// </summary>
    /// <param name="dt">
    /// </param>
    /// <returns>
    /// </returns>
    [CanBeNull]
    public static DataRow GetFirstRowOrInvalid([NotNull] this DataTable dt)
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
    /// Tests if an DB object (in <see cref="DataRow"/>) is <see cref="DBNull"/>.Value, <see langword="null"/> or empty.
    /// </summary>
    /// <param name="columnValue">
    /// </param>
    /// <returns>
    /// The is <see langword="null"/> or empty db field.
    /// </returns>
    public static bool IsNullOrEmptyDBField([NotNull] this object columnValue)
    {
      if (columnValue == DBNull.Value)
      {
        return true;
      }
      else if (columnValue.ToString().IsNotSet())
      {
        return true;
      }

      return false;
    }

    #endregion
  }
}