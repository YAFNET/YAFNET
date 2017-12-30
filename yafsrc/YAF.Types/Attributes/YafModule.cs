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

namespace YAF.Types.Attributes
{
  using System;

  /// <summary>
  /// The yaf module.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class)]
  public class YafModule : Attribute
  {
    /// <summary>
    /// The _module author.
    /// </summary>
    private string _moduleAuthor;

    /// <summary>
    /// The _module name.
    /// </summary>
    private string _moduleName;

    /// <summary>
    /// The _module version.
    /// </summary>
    private int _moduleVersion;

    /// <summary>
    /// Initializes a new instance of the <see cref="YafModule"/> class.
    /// </summary>
    /// <param name="moduleName">
    /// The module name.
    /// </param>
    /// <param name="moduleAuthor">
    /// The module author.
    /// </param>
    /// <param name="moduleVersion">
    /// The module version.
    /// </param>
    public YafModule(string moduleName, string moduleAuthor, int moduleVersion)
    {
      this._moduleName = moduleName;
      this._moduleAuthor = moduleAuthor;
      this._moduleVersion = moduleVersion;
    }

    /// <summary>
    /// Gets or sets ModuleName.
    /// </summary>
    public string ModuleName
    {
      get
      {
        return this._moduleName;
      }

      set
      {
        this._moduleName = value;
      }
    }

    /// <summary>
    /// Gets or sets ModuleAuthor.
    /// </summary>
    public string ModuleAuthor
    {
      get
      {
        return this._moduleAuthor;
      }

      set
      {
        this._moduleAuthor = value;
      }
    }

    /// <summary>
    /// Gets or sets ModuleVersion.
    /// </summary>
    public int ModuleVersion
    {
      get
      {
        return this._moduleVersion;
      }

      set
      {
        this._moduleVersion = value;
      }
    }
  }
}