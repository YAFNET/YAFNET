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
using System;
using System.Text;

namespace YAF.Providers.Membership
{
  /// <summary>
  /// The clean utils.
  /// </summary>
  public static class CleanUtils
  {
    /// <summary>
    /// The to date.
    /// </summary>
    /// <param name="obj">
    /// The obj.
    /// </param>
    /// <returns>
    /// </returns>
    public static DateTime ToDate(object obj)
    {
      if (obj != DBNull.Value)
      {
        return Convert.ToDateTime(obj.ToString());
      }
      else
      {
        return DateTime.Now;
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
      if (obj != DBNull.Value)
      {
        return obj.ToString();
      }
      else
      {
        return String.Empty;
      }
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
      if (obj != DBNull.Value)
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
      if (obj != DBNull.Value)
      {
        return Convert.ToInt32(obj);
      }
      else
      {
        return 0;
      }
    }

    /// <summary>
    /// The to hex string.
    /// </summary>
    /// <param name="hashedBytes">
    /// The hashed bytes.
    /// </param>
    /// <returns>
    /// The to hex string.
    /// </returns>
    public static string toHexString(byte[] hashedBytes)
    {
      var hashedSB = new StringBuilder(hashedBytes.Length*2 + 2);
      foreach (byte b in hashedBytes)
      {
        hashedSB.AppendFormat("{0:X2}", b);
      }

      return hashedSB.ToString();
    }
  }
}