/* Yet Another Forum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
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

  using System;
  using System.Collections.Generic;

  #endregion

  /// <summary>
  /// Defines a cache interface
  /// </summary>
  /// <typeparam name="T">
  /// </typeparam>
  public interface IDataCache<T> : IReadValue<T>, IWriteValue<T>, IRemoveValue
  {
    #region Indexers

    /// <summary>
    ///   The this.
    /// </summary>
    /// <param name = "key">
    ///   The key.
    /// </param>
    T this[[NotNull] string key] { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// The get all.
    /// </summary>
    /// <returns>
    /// </returns>
    IEnumerable<KeyValuePair<string, T>> GetAll();

    /// <summary>
    /// The get or set.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <param name="getValue">
    /// The get value.
    /// </param>
    /// <param name="timeout">
    /// The timeout.
    /// </param>
    /// <returns>
    /// </returns>
    T GetOrSet([NotNull] string key, [NotNull] Func<T> getValue, TimeSpan timeout);

    /// <summary>
    /// The get or set.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <param name="getValue">
    /// The get value.
    /// </param>
    /// <returns>
    /// </returns>
    T GetOrSet([NotNull] string key, [NotNull] Func<T> getValue);

    /// <summary>
    /// The remove.
    /// </summary>
    /// <param name="removeFunction">
    /// The remove function.
    /// </param>
    /// <returns>
    /// The remove.
    /// </returns>
    int Remove([NotNull] Func<string, bool> removeFunction);

    /// <summary>
    /// The set.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="timeout">
    /// The timeout.
    /// </param>
    void Set([NotNull] string key, T value, TimeSpan timeout);

    #endregion
  }
}