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
using System;
using System.Collections.Generic;

namespace YAF.Classes.Pattern
{
  /// <summary>
  /// Allows basic type conversion of Dictionary objects.
  /// </summary>
  public class TypeDictionary : Dictionary<string, object>
  {
    /// <summary>
    /// The as type.
    /// </summary>
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
    public T AsType<T>(string key, T defaultValue)
    {
      if (!ContainsKey(key))
      {
        return defaultValue;
      }

      return (T) Convert.ChangeType(this[key], typeof (T));
    }

    /// <summary>
    /// The as type.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public T AsType<T>(string key)
    {
      return (T) Convert.ChangeType(this[key], typeof (T));
    }

    /// <summary>
    /// The as boolean.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <returns>
    /// </returns>
    public bool? AsBoolean(string key)
    {
      if (!ContainsKey(key))
      {
        return null;
      }

      return AsType<bool>(key);
    }

    /// <summary>
    /// The as int.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <returns>
    /// </returns>
    public int? AsInt(string key)
    {
      if (!ContainsKey(key))
      {
        return null;
      }

      return AsType<int>(key);
    }

    /// <summary>
    /// The as string.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <returns>
    /// The as string.
    /// </returns>
    public string AsString(string key)
    {
      if (!ContainsKey(key))
      {
        return null;
      }

      return AsType<string>(key);
    }
  }
}