/* Yet Another Forum.NET
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
namespace YAF.Types.Interfaces
{
  #region Using

    using System;

    using YAF.Types.Interfaces;

    #endregion

  /// <summary>
  /// The i read value extensions.
  /// </summary>
  public static class IReadValueExtensions
  {
    #region Public Methods

    /// <summary>
    /// Gets a value with a default value...
    /// </summary>
    /// <param name="readValue">
    /// </param>
    /// <param name="key">
    /// </param>
    /// <param name="getValue">
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public static T Get<T>([NotNull] this IReadValue<T> readValue, [NotNull] string key, [NotNull] Func<T> getValue)
    {
      CodeContracts.VerifyNotNull(readValue, "readValue");
      CodeContracts.VerifyNotNull(key, "key");
      CodeContracts.VerifyNotNull(getValue, "getValue");

      var value = readValue.Get(key);

      return Equals(value, default(T)) ? getValue() : value;
    }

    /// <summary>
    /// Gets a value with a default value...
    /// </summary>
    /// <param name="readValue">
    /// </param>
    /// <param name="key">
    /// </param>
    /// <param name="defaultValue"></param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public static T Get<T>([NotNull] this IReadValue<T> readValue, [NotNull] string key, [CanBeNull] T defaultValue)
    {
      CodeContracts.VerifyNotNull(readValue, "readValue");
      CodeContracts.VerifyNotNull(key, "key");

      var value = readValue.Get(key);

      return Equals(value, default(T)) ? defaultValue : value;
    }

    /// <summary>
    /// The get as bool.
    /// </summary>
    /// <param name="readValue">
    /// The read value.
    /// </param>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <param name="defaultValue">
    /// The default value.
    /// </param>
    /// <returns>
    /// The get as bool.
    /// </returns>
    public static bool GetAsBool([NotNull] this IReadValue<string> readValue, [NotNull] string key, bool defaultValue)
    {
      CodeContracts.VerifyNotNull(readValue, "readValue");
      CodeContracts.VerifyNotNull(key, "key");

      var value = readValue.Get(key);

      return Equals(value, null) ? defaultValue : Convert.ToBoolean(value.ToLower());
    }

    #endregion
  }
}