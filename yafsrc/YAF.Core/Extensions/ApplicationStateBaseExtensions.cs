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
namespace YAF.Core
{
  #region Using

  using System;
  using System.Web;

  using YAF.Types;

  #endregion

  /// <summary>
  /// The http application state base extensions.
  /// </summary>
  public static class HttpApplicationStateBaseExtensions
  {
    #region Public Methods

    /// <summary>
    /// The get or set.
    /// </summary>
    /// <param name="httpApplicationState">
    /// The http application state.
    /// </param>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <param name="getValue">
    /// The get value.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public static T GetOrSet<T>(
      [NotNull] this HttpApplicationStateBase httpApplicationState, [NotNull] string key, [NotNull] Func<T> getValue)
    {
      CodeContracts.VerifyNotNull(httpApplicationState, "httpApplicationState");
      CodeContracts.VerifyNotNull(key, "key");
      CodeContracts.VerifyNotNull(getValue, "getValue");

      var item = httpApplicationState[key];

      if (Equals(item, default(T)))
      {
        try
        {
          httpApplicationState.Lock();

          item = httpApplicationState[key];

          if (Equals(item, default(T)))
          {
            item = getValue();
            httpApplicationState[key] = item;
          }
        }
        finally
        {
          httpApplicationState.UnLock();
        }
      }

      return (T)item;
    }

    /// <summary>
    /// The set.
    /// </summary>
    /// <param name="httpApplicationState">
    /// The http application state.
    /// </param>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    public static void Set<T>(
      [NotNull] this HttpApplicationStateBase httpApplicationState, [NotNull] string key, [NotNull] T value)
    {
      CodeContracts.VerifyNotNull(httpApplicationState, "httpApplicationState");
      CodeContracts.VerifyNotNull(key, "key");

      try
      {
        httpApplicationState.Lock();
        httpApplicationState[key] = value;
      }
      finally
      {
        httpApplicationState.UnLock();
      }
    }

    #endregion
  }
}