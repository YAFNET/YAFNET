/* Yet Another Forum.net
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
namespace YAF.Utils
{
  #region Using

  using System;
  using System.Web.UI;

  #endregion

  /// <summary>
  /// The view state extensions.
  /// </summary>
  public static class ViewStateExtensions
  {
    #region Public Methods

    /// <summary>
    /// Returns the converted type (T) if ViewState[key] != <see langword="null"/> or
    /// <paramref name="defaultValue"/> if it's <see langword="null"/>.
    /// </summary>
    /// <param name="viewState">
    /// The view state.
    /// </param>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <param name="defaultValue">
    /// The default value.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public static T ToTypeOrDefault<T>(this StateBag viewState, string key, T defaultValue)
    {
      if (viewState[key] != null)
      {
        return viewState[key].ToType<T>();
      }

      return default(T);
    }

    /// <summary>
    /// Returns the converted type (T) if ViewState[key] != <see langword="null"/> or
    /// <paramref name="defaultFunc"/> if it's <see langword="null"/>.
    /// </summary>
    /// <param name="viewState">
    /// The view state.
    /// </param>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <param name="defaultFunc">
    /// The default func.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public static T ToTypeOrFunc<T>(this StateBag viewState, string key, Func<T> defaultFunc)
    {
      if (viewState[key] != null)
      {
        return viewState[key].ToType<T>();
      }

      return defaultFunc();
    }

    #endregion
  }
}