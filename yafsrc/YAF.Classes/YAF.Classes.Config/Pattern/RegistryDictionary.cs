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
using System;
using System.Collections.Generic;

namespace YAF.Classes.Pattern
{
  /// <summary>
  /// Provides a method for automatic overriding of a base hash...
  /// </summary>
  public class RegistryDictionaryOverride : RegistryDictionary
  {
    /// <summary>
    /// The _default get override.
    /// </summary>
    private bool _defaultGetOverride = true;

    /// <summary>
    /// The _default set override.
    /// </summary>
    private bool _defaultSetOverride = false;

    /// <summary>
    /// Gets or sets a value indicating whether DefaultGetOverride.
    /// </summary>
    public bool DefaultGetOverride
    {
      get
      {
        return this._defaultGetOverride;
      }

      set
      {
        this._defaultGetOverride = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether DefaultSetOverride.
    /// </summary>
    public bool DefaultSetOverride
    {
      get
      {
        return this._defaultSetOverride;
      }

      set
      {
        this._defaultSetOverride = value;
      }
    }

    /// <summary>
    /// Gets or sets OverrideDictionary.
    /// </summary>
    public RegistryDictionary OverrideDictionary
    {
      get;
      set;
    }

    /// <summary>
    /// The get value.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="defaultValue">
    /// The default value.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public override T GetValue<T>(string name, T defaultValue)
    {
      return GetValue<T>(name, defaultValue, DefaultGetOverride);
    }

    /// <summary>
    /// The get value.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="defaultValue">
    /// The default value.
    /// </param>
    /// <param name="allowOverride">
    /// The allow override.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public virtual T GetValue<T>(string name, T defaultValue, bool allowOverride)
    {
      if (allowOverride && OverrideDictionary != null && OverrideDictionary.ContainsKey(name.ToLower()) && OverrideDictionary[name.ToLower()] != null)
      {
        return OverrideDictionary.GetValue<T>(name, defaultValue);
      }

      // just pull the value from this dictionary...
      return base.GetValue<T>(name, defaultValue);
    }

    /// <summary>
    /// The set value.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    public override void SetValue<T>(string name, T value)
    {
      SetValue<T>(name, value, DefaultSetOverride);
    }

    /// <summary>
    /// The set value.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="setOverrideOnly">
    /// The set override only.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    public virtual void SetValue<T>(string name, T value, bool setOverrideOnly)
    {
      if (OverrideDictionary != null)
      {
        if (setOverrideOnly)
        {
          // just set the override dictionary...
          OverrideDictionary.SetValue<T>(name, value);
          return;
        }
        else if (OverrideDictionary.ContainsKey(name.ToLower()) && OverrideDictionary[name.ToLower()] != null)
        {
          // set the overriden value to null/erase it...
          OverrideDictionary.SetValue<T>(name, (T) Convert.ChangeType(null, typeof (T)));
        }
      }

      // save new value in the base...
      base.SetValue<T>(name, value);
    }
  }

  /// <summary>
  /// The registry dictionary.
  /// </summary>
  public class RegistryDictionary : Dictionary<string, object>
  {
    /* Ederon : 6/16/2007 -- modified by Jaben 7/17/2009 */

    /// <summary>
    /// The get value.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="defaultValue">
    /// The default value.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public virtual T GetValue<T>(string name, T defaultValue)
    {
      if (!ContainsKey(name.ToLower()))
      {
        return defaultValue;
      }

      object value = this[name.ToLower()];

      if (value == null)
      {
        return defaultValue;
      }

      Type objectType = typeof(T);

      if (objectType.BaseType == typeof(Enum))
      {
        return (T)Enum.Parse(objectType, value.ToString());
      }

      // special handling for boolean...
      if (objectType == typeof(bool))
      {
        int i;
        return int.TryParse(value.ToString(), out i)
                 ? (T)Convert.ChangeType(Convert.ToBoolean(i), typeof(T))
                 : (T)Convert.ChangeType(Convert.ToBoolean(value), typeof(T));
      }

      // special handling for int values...
      if (objectType == typeof(int))
      {
        return (T)Convert.ChangeType(Convert.ToInt32(value), typeof(T));
      }

      return (T)Convert.ChangeType(this[name.ToLower()], typeof(T));
    }

    /// <summary>
    /// The set value.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    public virtual void SetValue<T>(string name, T value)
    {
      var objectType = typeof(T);
      string stringValue = Convert.ToString(value);

      if (objectType == typeof(bool) || objectType.BaseType == typeof(Enum))
      {
        stringValue = Convert.ToString(Convert.ToInt32(value));
      }

      this[name.ToLower()] = stringValue;
    }

    /* 6/16/2007 */
  }
}