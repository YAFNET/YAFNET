/* Yet Another Forum.NET
 * Copyright (C) 2006-2009 Jaben Cargman
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
namespace YAF.Providers.Utils
{
  using System;
  using System.Collections.Specialized;

  /// <summary>
  /// The transform.
  /// </summary>
  public static class Transform
  {
    /// <summary>
    /// The to date time.
    /// </summary>
    /// <param name="obj">
    /// The obj.
    /// </param>
    /// <returns>
    /// </returns>
    public static DateTime ToDateTime(object obj)
    {
      if ((obj != DBNull.Value) && (obj != null))
      {
        return Convert.ToDateTime(obj.ToString());
      }
      else
      {
        return DateTime.Now; // Yeah I admit it, what the hell should this return?
      }
    }

    /// <summary>
    /// The to string.
    /// </summary>
    /// <param name="obj">
    /// The obj.
    /// </param>
    /// <returns>
    /// The to string.
    /// </returns>
    public static string ToString(object obj)
    {
      if ((obj != DBNull.Value) && (obj != null))
      {
        return obj.ToString();
      }
      else
      {
        return String.Empty;
      }
    }

    /// <summary>
    /// The to string.
    /// </summary>
    /// <param name="obj">
    /// The obj.
    /// </param>
    /// <param name="defValue">
    /// The def value.
    /// </param>
    /// <returns>
    /// The to string.
    /// </returns>
    public static string ToString(object obj, string defValue)
    {
      if ((obj != DBNull.Value) && (obj != null))
      {
        return obj.ToString();
      }
      else
      {
        return defValue;
      }
    }

    /// <summary>
    /// The to string array.
    /// </summary>
    /// <param name="coll">
    /// The coll.
    /// </param>
    /// <returns>
    /// </returns>
    public static string[] ToStringArray(StringCollection coll)
    {
      var strReturn = new string[coll.Count];
      coll.CopyTo(strReturn, 0);
      return strReturn;
    }

    /// <summary>
    /// The to bool.
    /// </summary>
    /// <param name="obj">
    /// The obj.
    /// </param>
    /// <returns>
    /// The to bool.
    /// </returns>
    public static bool ToBool(object obj)
    {
      if ((obj != DBNull.Value) && (obj != null))
      {
        return Convert.ToBoolean(obj);
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// The to int.
    /// </summary>
    /// <param name="obj">
    /// The obj.
    /// </param>
    /// <returns>
    /// The to int.
    /// </returns>
    public static int ToInt(object obj)
    {
      if ((obj != DBNull.Value) && (obj != null))
      {
        return Convert.ToInt32(obj);
      }
      else
      {
        return 0;
      }
    }
  }
}