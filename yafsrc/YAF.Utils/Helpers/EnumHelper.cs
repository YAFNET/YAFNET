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
  using System.Collections.Generic;
  using System.Reflection;

  using YAF.Types.Attributes;

  #endregion

  /// <summary>
  /// The enum helper.
  /// </summary>
  public static class EnumHelper
  {
    #region Public Methods

    /// <summary>
    /// Converts an Enum to a Dictionary
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public static Dictionary<int, string> EnumToDictionary<T>()
    {
      Type enumType = typeof(T);

      if (enumType.BaseType != typeof(Enum))
      {
        throw new ApplicationException("EnumToDictionary does not support non-enum types");
      }

      var list = new Dictionary<int, string>();

      foreach (FieldInfo field in enumType.GetFields(BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public))
      {
        int value;
        string display;
        value = (int)field.GetValue(null);
        display = Enum.GetName(enumType, value);

        var attribs = field.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];

        // Return the first if there was a match.
        if (attribs != null && attribs.Length > 0)
        {
          display = attribs[0].StringValue;
        }

        // add the value...
        list.Add(value, display);
      }

      return list;
    }

    /// <summary>
    /// Converts an Enum to a Dictionary
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public static Dictionary<byte, string> EnumToDictionaryByte<T>()
    {
      Type enumType = typeof(T);

      if (enumType.BaseType != typeof(Enum))
      {
        throw new ApplicationException("EnumToDictionaryByte does not support non-enum types");
      }

      var list = new Dictionary<byte, string>();

      foreach (FieldInfo field in enumType.GetFields(BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public))
      {
        byte value;
        string display;
        value = (byte)field.GetValue(null);
        display = Enum.GetName(enumType, value);

        var attribs = field.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];

        // Return the first if there was a match.
        if (attribs != null && attribs.Length > 0)
        {
          display = attribs[0].StringValue;
        }

        // add the value...
        list.Add(value, display);
      }

      return list;
    }

    #endregion
  }
}