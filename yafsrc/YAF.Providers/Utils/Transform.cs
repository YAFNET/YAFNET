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
namespace YAF.Providers.Utils
{
  using System;
  using System.Collections.Specialized;
  using System.Text;

  /// <summary>
  /// The transform.
  /// </summary>
  public static class Transform
  {
    /// <summary>
    /// Convert Object to <see cref="DateTime"/>.
    /// </summary>
    /// <param name="obj">
    /// The <see cref="DateTime"/> object.
    /// </param>
    /// <param name="defaultDateTime">Default <see cref="DateTime"/> returned if obj is <see langword="null"/>.</param>
    /// <returns>
    /// </returns>
    public static DateTime ToDateTime(this object obj, DateTime defaultDateTime)
    {
      if (obj != null && obj != DBNull.Value)
      {
        return Convert.ToDateTime(obj.ToString());
      }

      return defaultDateTime;
    }

    /// <summary>
    /// The to string.
    /// </summary>
    /// <param name="obj">
    /// The obj.
    /// </param>
    /// <returns>
    /// The string.
    /// </returns>
    public static string ToStringDBNull(this object obj)
    {
      return ToStringDBNull(obj, String.Empty);
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
    public static string ToStringDBNull(this object obj, string defValue)
    {
      return obj != DBNull.Value && obj != null ? obj.ToString() : defValue;
    }

    /// <summary>
    /// The to string array.
    /// </summary>
    /// <param name="coll">
    /// The coll.
    /// </param>
    /// <returns>
    /// </returns>
    public static string[] ToStringArray(this StringCollection coll)
    {
      var strReturn = new string[coll.Count];
      coll.CopyTo(strReturn, 0);
      return strReturn;
    }

    /// <summary>
    /// Convert object to a boolean.
    /// </summary>
    /// <param name="obj">
    /// The obj.
    /// </param>
    /// <returns>
    /// The boolean.
    /// </returns>
    public static bool ToBool(this object obj)
    {
      return ToBool(obj, false);
    }

      /// <summary>
      /// Convert object to a boolean.
      /// </summary>
      /// <param name="obj">
      /// The object that will be converted
      /// </param>
      /// <param name="defaultValue">The defaultValue</param>
      /// <returns>
      /// The boolean.
      /// </returns>
      public static bool ToBool(this object obj, bool defaultValue)
    {
      bool value;

      if (obj != DBNull.Value && obj != null && bool.TryParse(obj.ToString(), out value))
      {
        return value;  
      }

      return defaultValue;
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
    public static int ToInt(this object obj)
    {
      return obj != DBNull.Value && obj != null ? Convert.ToInt32(obj) : 0;
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
    public static string ToHexString(this byte[] hashedBytes)
    {
      if (hashedBytes == null || hashedBytes.Length == 0)
      {
        throw new ArgumentException("hashedBytes is null or empty.", "hashedBytes");
      }

      var hashedSB = new StringBuilder((hashedBytes.Length * 2) + 2);

      foreach (byte b in hashedBytes)
      {
        hashedSB.AppendFormat("{0:X2}", b);
      }

      return hashedSB.ToString();
    }
  }
}