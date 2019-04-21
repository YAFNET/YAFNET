/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
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
namespace YAF.Controls
{
    /// <summary>
  /// The localization support interface
  /// </summary>
  public interface ILocalizationSupport
  {
    #region Properties

    /// <summary>
    /// Gets or sets a value indicating whether EnableBBCode.
    /// </summary>
    bool EnableBBCode { get; set; }

    /// <summary>
    /// Gets or sets LocalizedPage.
    /// </summary>
    string LocalizedPage { get; set; }

    /// <summary>
    /// Gets or sets LocalizedTag.
    /// </summary>
    string LocalizedTag { get; set; }

    /// <summary>
    /// Gets or sets Parameter 0.
    /// </summary>
    string Param0 { get; set; }

    /// <summary>
    /// Gets or sets Parameter 1.
    /// </summary>
    string Param1 { get; set; }

    /// <summary>
    /// Gets or sets Parameter 2.
    /// </summary>
    string Param2 { get; set; }

    #endregion
  }
}