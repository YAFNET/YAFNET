/* Yet Another Forum.net
 * Copyright (C) 2006-2010 Jaben Cargman
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
using System.Reflection;

namespace YAF.Classes.Utils
{
  /// <summary>
  /// The enum extensions.
  /// </summary>
  public static class EnumExtensions
  {
    /// <summary>
    /// Will get the string value for a given enums value, this will
    /// only work if you assign the StringValue attribute to
    /// the items in your enum.
    /// </summary>
    /// <param name="value">
    /// </param>
    /// <returns>
    /// The get string value.
    /// </returns>
    public static string GetStringValue(this Enum value)
    {
      // Get the type
      Type type = value.GetType();

      // Get fieldinfo for this type
      FieldInfo fieldInfo = type.GetField(value.ToString());

      if (fieldInfo != null)
      {
        // Get the stringvalue attributes
        var attribs = fieldInfo.GetCustomAttributes(typeof (StringValueAttribute), false) as StringValueAttribute[];

        // Return the first if there was a match.
        if (attribs != null)
        {
          return attribs.Length > 0 ? attribs[0].StringValue : Enum.GetName(type, value);
        }
      }

      return Enum.GetName(type, value);
    }

    /// <summary>
    /// Will get the string value for a given enums value, this will
    /// only work if you assign the StringValue attribute to
    /// the items in your enum.
    /// </summary>
    /// <param name="value">
    /// </param>
    /// <returns>
    /// The get alt string value.
    /// </returns>
    public static string GetAltStringValue(this Enum value)
    {
      string strValue = string.Empty;

      // Get the type
      Type type = value.GetType();

      // Get fieldinfo for this type
      FieldInfo fieldInfo = type.GetField(value.ToString());

      // Get the stringvalue attributes
      var altAttribs =
        fieldInfo.GetCustomAttributes(typeof(AltStringValueAttribute), false) as AltStringValueAttribute[];

      if (altAttribs != null && altAttribs.Length > 0)
      {
        strValue = altAttribs[0].AltStringValue;
      }
      else
      {
        // Get the stringvalue attributes
        var attribs = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];

        if (attribs != null && attribs.Length > 0)
        {
          strValue = attribs[0].StringValue;
        }
      }

      return String.IsNullOrEmpty(strValue) ? Enum.GetName(type, value) : strValue;
    }

    /// <summary>
    /// Checks if an enumerated type contains a value
    /// </summary>
    public static bool Has<T>(this Enum value, T check)
    {
      Type type = value.GetType();

      // determine the values
      object result = value;
      var parsed = new _Value(check, type);
      if (parsed.Signed is long)
      {
        return (Convert.ToInt64(value) & (long)parsed.Signed) == (long)parsed.Signed;
      }
      else if (parsed.Unsigned is ulong)
      {
        return (Convert.ToUInt64(value) & (ulong)parsed.Unsigned) == (ulong)parsed.Unsigned;
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Includes an enumerated type and returns the new value
    /// </summary>
    public static T Include<T>(this Enum value, T append)
    {
      Type type = value.GetType();

      // determine the values
      object result = value;
      var parsed = new _Value(append, type);
      if (parsed.Signed is long)
      {
        result = Convert.ToInt64(value) | (long)parsed.Signed;
      }
      else if (parsed.Unsigned is ulong)
      {
        result = Convert.ToUInt64(value) | (ulong)parsed.Unsigned;
      }

      // return the final value
      return (T)Enum.Parse(type, result.ToString());
    }

    /// <summary>
    /// Includes an enumerated type and returns the new value
    /// </summary>
    public static T Include<T>(this Enum value, ulong bitValues)
    {
      Type type = value.GetType();

      // determine the values
      object result = value;
      var parsed = new _Value(bitValues, type);
      if (parsed.Signed is long)
      {
        result = Convert.ToInt64(value) | (long)parsed.Signed;
      }
      else if (parsed.Unsigned is ulong)
      {
        result = Convert.ToUInt64(value) | (ulong)parsed.Unsigned;
      }

      // return the final value
      return (T)Enum.Parse(type, result.ToString());
    }

    /// <summary>
    /// Checks if an enumerated type is missing a value
    /// </summary>
    public static bool Missing<T>(this Enum obj, T value)
    {
      return !EnumExtensions.Has<T>(obj, value);
    }

    /// <summary>
    /// Removes an enumerated type and returns the new value
    /// </summary>
    public static T Remove<T>(this Enum value, T remove)
    {
      Type type = value.GetType();

      // determine the values
      object result = value;
      var parsed = new _Value(remove, type);
      if (parsed.Signed is long)
      {
        result = Convert.ToInt64(value) & (long)parsed.Signed;
      }
      else if (parsed.Unsigned is ulong)
      {
        result = Convert.ToUInt64(value) & (ulong)parsed.Unsigned;
      }

      // return the final value
      return (T)Enum.Parse(type, result.ToString());
    }

    /// <summary>
    /// Will get the enum value as an integer saving a cast (int).
    /// </summary>
    /// <param name="value">
    /// </param>
    /// <returns>
    /// The to int.
    /// </returns>
    public static int ToInt(this Enum value)
    {
      return Convert.ToInt32(value);
    }

    /// <summary>
    /// Will get the enum value as an Byte saving a cast (byte).
    /// </summary>
    /// <param name="value">
    /// </param>
    /// <returns>
    /// The to byte.
    /// </returns>
    public static byte ToByte(this Enum value)
    {
      return Convert.ToByte(value);
    }

    /// <summary>
    /// Will get the enum value as an char saving a cast (char).
    /// </summary>
    /// <param name="value">
    /// </param>
    /// <returns>
    /// The to char.
    /// </returns>
    public static char ToChar(this Enum value)
    {
      return Convert.ToChar(value);
    }

    private class _Value
    {
      // cached comparisons for tye to use
      #region Constants and Fields

      public long? Signed;

      public long? Int;

      public ulong? Unsigned;

      private static Type _UInt32 = typeof(long);

      private static Type _UInt64 = typeof(ulong);

      private static Type _Int32 = typeof(int);

      #endregion

      #region Constructors and Destructors

      public _Value(object value, Type type)
      {
        // make sure it is even an enum to work with
        if (!type.IsEnum)
        {
          throw new ArgumentException("Value provided is not an enumerated type!");
        }

        // then check for the enumerated value
        Type compare = Enum.GetUnderlyingType(type);

        // if this is an unsigned long then the only
        // value that can hold it would be a ulong
        if (compare.Equals(_UInt32) || compare.Equals(_UInt64))
        {
          this.Unsigned = Convert.ToUInt64(value);
        }
        else if (compare.Equals(_Int32))
        {
          this.Int = Convert.ToInt32(value);

        }
          // otherwise, a long should cover anything else
        else
        {
            this.Signed = Convert.ToInt64(value);
        }
      }

      #endregion
    }
  }

  /// <summary>
  /// The enum helper.
  /// </summary>
  public static class EnumHelper
  {
    /// <summary>
    /// Converts an Enum to a List
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public static List<T> EnumToList<T>()
    {
      Type enumType = typeof (T);

      // Can't use type constraints on value types, so have to do check like this
      if (enumType.BaseType != typeof (Enum))
      {
        throw new ArgumentException("EnumToList does not support non-enum types");
      }

      Array enumValArray = Enum.GetValues(enumType);

      var enumValList = new List<T>(enumValArray.Length);

      foreach (int val in enumValArray)
      {
        enumValList.Add((T) Enum.Parse(enumType, val.ToString()));
      }

      return enumValList;
    }

    /// <summary>
    /// The to enum.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    /// <exception cref="ApplicationException">
    /// </exception>
    public static T ToEnum<T>(this int value)
    {
      Type enumType = typeof(T);
      if (enumType.BaseType != typeof(Enum))
      {
        throw new ApplicationException("ToEnum does not support non-enum types");
      }

      return (T) Enum.Parse(enumType, value.ToString());
    }

    /// <summary>
    /// The to enum.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    /// <exception cref="ApplicationException">
    /// </exception>
    public static T ToEnum<T>(this ulong value)
    {
      Type enumType = typeof(T);
      if (enumType.BaseType != typeof(Enum))
      {
        throw new ApplicationException("ToEnum does not support non-enum types");
      }

      return (T)Enum.Parse(enumType, value.ToString());
    }

    /// <summary>
    /// The to enum.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    /// <exception cref="ApplicationException">
    /// </exception>
    public static T ToEnum<T>(this string value)
    {
      Type enumType = typeof(T);
      if (enumType.BaseType != typeof(Enum))
      {
        throw new ApplicationException("ToEnum does not support non-enum types");
      }

      return (T) Enum.Parse(enumType, value);
    }

    /// <summary>
    /// The to enum.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    /// <exception cref="ApplicationException">
    /// </exception>
    public static T ToEnum<T>(this string value, bool ignoreCase)
    {
      Type enumType = typeof(T);
      if (enumType.BaseType != typeof(Enum))
      {
        throw new ApplicationException("ToEnum does not support non-enum types");
      }

      return (T)Enum.Parse(enumType, value, ignoreCase);
    }

    /// <summary>
    /// The to enum.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    /// <exception cref="ApplicationException">
    /// </exception>
    public static T ToEnum<T>(this object value)
    {
      Type enumType = typeof (T);
      if (enumType.BaseType != typeof (Enum))
      {
        throw new ApplicationException("ObjToEnum does not support non-enum types");
      }

      return (T) Enum.Parse(enumType, value.ToString());
    }

    /// <summary>
    /// Converts an Enum to a Dictionary
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public static Dictionary<int, string> EnumToDictionary<T>()
    {
      Type enumType = typeof (T);

      if (enumType.BaseType != typeof (Enum))
      {
        throw new ApplicationException("EnumToDictionary does not support non-enum types");
      }

      var list = new Dictionary<int, string>();

      foreach (FieldInfo field in enumType.GetFields(BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public))
      {
        int value;
        string display;
        value = (int) field.GetValue(null);
        display = Enum.GetName(enumType, value);

        var attribs = field.GetCustomAttributes(typeof (StringValueAttribute), false) as StringValueAttribute[];

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
      Type enumType = typeof (T);

      if (enumType.BaseType != typeof (Enum))
      {
        throw new ApplicationException("EnumToDictionaryByte does not support non-enum types");
      }

      var list = new Dictionary<byte, string>();

      foreach (FieldInfo field in enumType.GetFields(BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public))
      {
        byte value;
        string display;
        value = (byte) field.GetValue(null);
        display = Enum.GetName(enumType, value);

        var attribs = field.GetCustomAttributes(typeof (StringValueAttribute), false) as StringValueAttribute[];

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
  }
}