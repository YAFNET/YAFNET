/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
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