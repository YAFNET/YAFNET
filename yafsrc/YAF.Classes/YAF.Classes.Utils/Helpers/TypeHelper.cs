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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace YAF.Classes.Utils
{
  /// <summary>
  /// The type helper.
  /// </summary>
  public static class TypeHelper
  {
    /// <summary>
    /// Converts an object to a type.
    /// </summary>
    /// <param name="value">
    /// Object to convert
    /// </param>
    /// <param name="type">
    /// Type to convert to e.g. System.Guid
    /// </param>
    /// <returns>
    /// The convert object to type.
    /// </returns>
    public static object ConvertObjectToType(object value, string type)
    {
      Type convertType;

      try
      {
        convertType = Type.GetType(type, true, true);
      }
      catch
      {
        convertType = Type.GetType("System.Guid", false);
      }

      if (value.GetType().ToString() == "System.String")
      {
        switch (convertType.ToString())
        {
          case "System.Guid":

            // do a "manual conversion" from string to Guid
            return new Guid(Convert.ToString(value));
          case "System.Int32":
            return Convert.ToInt32(value);
          case "System.Int64":
            return Convert.ToInt64(value);
        }
      }

      return Convert.ChangeType(value, convertType);
    }

    /// <summary>
    /// Gets an Int from an Object value
    /// </summary>
    /// <param name="expression">
    /// </param>
    /// <returns>
    /// The valid int.
    /// </returns>
    public static int ValidInt(object expression)
    {
      int value = 0;

      if (expression != null)
      {
        try
        {
          int.TryParse(expression.ToString(), out value);
        }
        catch
        {
        }
      }

      return value;
    }

    /// <summary>
    /// The verify int 32.
    /// </summary>
    /// <param name="o">
    /// The o.
    /// </param>
    /// <returns>
    /// The verify int 32.
    /// </returns>
    public static int VerifyInt32(object o)
    {
      return Convert.ToInt32(o);
    }

    /// <summary>
    /// The verify bool.
    /// </summary>
    /// <param name="o">
    /// The o.
    /// </param>
    /// <returns>
    /// The verify bool.
    /// </returns>
    public static bool VerifyBool(object o)
    {
      return Convert.ToBoolean(o);
    }

    /// <summary>
    /// Converts the object to the class (T) or returns null if it's not 
    /// an instance of that class or instance is null.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    /// <param name="instance">
    /// </param>
    /// <returns>
    /// </returns>
    public static T ToClass<T>(this object instance) where T : class
    {
      if (instance != null && instance is T)
      {
        return instance as T;
      }

      return null;
    }

    /// <summary>
    /// Converts an object to Type using the Convert.ChangeType() call.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    /// <param name="instance">
    /// </param>
    /// <returns>
    /// </returns>
    public static T ToType<T>(this object instance)
    {
      return (T) Convert.ChangeType(instance, typeof (T));
    }

    /// <summary>
    /// The to generic list.
    /// </summary>
    /// <param name="listObjects">
    /// The list objects.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public static List<T> ToGenericList<T>(this IList listObjects)
    {
      var convertedList = new List<T>(listObjects.Count);

      foreach (object listObject in listObjects)
      {
        convertedList.Add((T) listObject);
      }

      return convertedList;
    }

    /// <summary>
    /// Converts an object to a different object (class) by copying fields (if they exist).
    /// Used to convert annonomous objects to strongly typed objects.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    /// <param name="obj">
    /// </param>
    /// <returns>
    /// </returns>
    public static T ToDifferentClassType<T>(this object obj) where T : class
    {
      // create instance of T type object:
      var tmp = Activator.CreateInstance(typeof (T));

      // loop through the fields of the object you want to covert:       
      foreach (FieldInfo fi in obj.GetType().GetFields())
      {
        try
        {
          tmp.GetType().GetField(fi.Name).SetValue(tmp, fi.GetValue(obj));
        }
        catch
        {
        }
      }

      // return the T type object:         
      return (T) tmp;
    }

    /// <summary>
    /// The get custom attributes.
    /// </summary>
    /// <param name="objectType">
    /// The object type.
    /// </param>
    /// <param name="attributeType">
    /// The attribute type.
    /// </param>
    /// <returns>
    /// </returns>
    public static object[] GetCustomAttributes(Type objectType, Type attributeType)
    {
      object[] myAttrOnType = objectType.GetCustomAttributes(attributeType, false);
      if (myAttrOnType.Length > 0)
      {
        return myAttrOnType;
      }

      return null;
    }
  }
}