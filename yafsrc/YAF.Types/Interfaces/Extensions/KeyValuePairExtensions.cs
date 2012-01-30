/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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
namespace YAF.Types.Interfaces
{
  #region Using

  using System.Collections.Generic;

  #endregion

  /// <summary>
  /// The key value pair extensions.
  /// </summary>
  public static class KeyValuePairExtensions
  {
    #region Public Methods

    /// <summary>
    /// The is not null.
    /// </summary>
    /// <param name="keyValuePair">
    /// The key value pair.
    /// </param>
    /// <typeparam name="TKey">
    /// </typeparam>
    /// <typeparam name="TValue">
    /// </typeparam>
    /// <returns>
    /// The is not null.
    /// </returns>
    public static bool IsNotNull<TKey, TValue>(this KeyValuePair<TKey, TValue> keyValuePair)
    {
      return !Equals(keyValuePair, default(KeyValuePair<TKey, TValue>));
    }

    /// <summary>
    /// The is null.
    /// </summary>
    /// <param name="keyValuePair">
    /// The key value pair.
    /// </param>
    /// <typeparam name="TKey">
    /// </typeparam>
    /// <typeparam name="TValue">
    /// </typeparam>
    /// <returns>
    /// The is null.
    /// </returns>
    public static bool IsNull<TKey, TValue>(this KeyValuePair<TKey, TValue> keyValuePair)
    {
      return Equals(keyValuePair, default(KeyValuePair<TKey, TValue>));
    }

    #endregion
  }
}