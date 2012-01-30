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
namespace YAF.Classes.Pattern
{
  #region Using

  using System;
  using System.Web;

  #endregion

  /// <summary>
  /// Singleton factory implementation
  /// </summary>
  /// <typeparam name="T">
  /// </typeparam>
  public static class PageSingleton<T>
    where T : class, new()
  {
    // static constructor, 
    // runtime ensures thread safety
    #region Constants and Fields

    /// <summary>
    ///   The _instance.
    /// </summary>
    private static T _instance;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets Instance.
    /// </summary>
    public static T Instance
    {
      get
      {
        return GetInstance();
      }

      private set
      {
        _instance = value;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The get instance.
    /// </summary>
    /// <returns>
    /// </returns>
    private static T GetInstance()
    {
      if (HttpContext.Current == null)
      {
        return _instance ?? (_instance = (T)Activator.CreateInstance(typeof(T)));
      }

      string typeStr = typeof(T).ToString();

      return
        (T)
        (HttpContext.Current.Items[typeStr] ??
         (HttpContext.Current.Items[typeStr] = Activator.CreateInstance(typeof(T))));
    }

    #endregion
  }
}