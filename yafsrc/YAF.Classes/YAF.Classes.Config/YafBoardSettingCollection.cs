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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace YAF.Classes
{
  /// <summary>
  /// Gets the Board Settings as dictionary item for easy iteration.
  /// </summary>
  public class YafBoardSettingCollection
  {
    /// <summary>
    /// The _settings.
    /// </summary>
    protected List<PropertyInfo> _settings = new List<PropertyInfo>();

    /// <summary>
    /// Initializes a new instance of the <see cref="YafBoardSettingCollection"/> class.
    /// </summary>
    /// <param name="boardSettings">
    /// The board settings.
    /// </param>
    public YafBoardSettingCollection(YafBoardSettings boardSettings)
    {
      // load up the settings...
      Type boardSettingsType = boardSettings.GetType();
      this._settings = boardSettingsType.GetProperties().ToList();
    }

    /// <summary>
    /// Gets SettingsString.
    /// </summary>
    public Dictionary<string, PropertyInfo> SettingsString
    {
      get
      {
        return this._settings.Where(x => x.PropertyType == typeof (string)).ToDictionary(x => x.Name, x => x);
      }
    }

    /// <summary>
    /// Gets SettingsBool.
    /// </summary>
    public Dictionary<string, PropertyInfo> SettingsBool
    {
      get
      {
        return this._settings.Where(x => x.PropertyType == typeof (bool)).ToDictionary(x => x.Name, x => x);
      }
    }

    /// <summary>
    /// Gets SettingsInt.
    /// </summary>
    public Dictionary<string, PropertyInfo> SettingsInt
    {
      get
      {
        return this._settings.Where(x => x.PropertyType == typeof (int)).ToDictionary(x => x.Name, x => x);
      }
    }

    /// <summary>
    /// Gets SettingsDouble.
    /// </summary>
    public Dictionary<string, PropertyInfo> SettingsDouble
    {
      get
      {
        return this._settings.Where(x => x.PropertyType == typeof (double)).ToDictionary(x => x.Name, x => x);
      }
    }

    /// <summary>
    /// Gets SettingsOther.
    /// </summary>
    public Dictionary<string, PropertyInfo> SettingsOther
    {
      get
      {
        var excludeTypes = new List<Type>()
          {
            typeof (string), 
            typeof (bool), 
            typeof (int), 
            typeof (double)
          };

        return this._settings.Where(x => !excludeTypes.Contains(x.PropertyType)).ToDictionary(x => x.Name, x => x);
      }
    }
  }
}