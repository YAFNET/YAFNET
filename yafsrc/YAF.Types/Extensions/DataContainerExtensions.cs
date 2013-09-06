/* Yet Another Forum.net
 * Copyright (C) 2006-2013 Jaben Cargman
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
namespace YAF.Types.Extensions
{
  #region Using

    using System.Data;
    using System.Web.UI;

    #endregion

  /// <summary>
  /// The data container extensions.
  /// </summary>
  public static class DataContainerExtensions
  {
    #region Public Methods

    /// <summary>
    /// The data item to field.
    /// </summary>
    /// <param name="container">
    /// The container.
    /// </param>
    /// <param name="fieldName">
    /// The field name.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public static T DataItemToField<T>(this IDataItemContainer container, string fieldName)
    {
      if (container == null || fieldName.IsNotSet())
      {
        return default(T);
      }

      if (container.DataItem is DataRow)
      {
        return container.DataItem.ToType<DataRow>().Field<T>(fieldName);
      }
      else if (container.DataItem is DataRowView)
      {
        return container.DataItem.ToType<DataRowView>()[fieldName].ToType<T>();
      }

      // not sure about this "fall-through"
        return container.DataItem.ToType<T>();
    }

    /// <summary>
    /// The to data item type.
    /// </summary>
    /// <param name="container">
    /// The container.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public static T ToDataItemType<T>(this IDataItemContainer container)
    {
      return Equals(container, default(T)) ? default(T) : container.DataItem.ToType<T>();
    }

    /// <summary>
    /// Converts DataItem to a class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="container"></param>
    /// <returns></returns>
    public static T ToDataItemClass<T>(this IDataItemContainer container) where T : class
    {
      return Equals(container, default(T)) ? default(T) : container.DataItem.ToClass<T>();
    }

    #endregion
  }
}