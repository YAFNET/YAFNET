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
namespace YAF.Types.Interfaces
{
  /// <summary>
  /// The theme interface
  /// </summary>
  public interface ITheme
  {
    #region Properties

    /// <summary>
    ///   Gets or sets a value indicating whether LogMissingThemeItem.
    /// </summary>
    bool LogMissingThemeItem { get; set; }

    /// <summary>
    ///   Gets ThemeDir.
    /// </summary>
    string ThemeDir { get; }

    /// <summary>
    ///   Gets or sets the current Theme File
    /// </summary>
    string ThemeFile { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets full path to the given theme file.
    /// </summary>
    /// <param name="filename">
    /// Short name of theme file.
    /// </param>
    /// <returns>
    /// The build theme path.
    /// </returns>
    string BuildThemePath([NotNull] string filename);

    /// <summary>
    /// The get item.
    /// </summary>
    /// <param name="page">
    /// The page.
    /// </param>
    /// <param name="tag">
    /// The tag.
    /// </param>
    /// <returns>
    /// The get item.
    /// </returns>
    string GetItem([NotNull] string page, [NotNull] string tag);

    /// <summary>
    /// The get item.
    /// </summary>
    /// <param name="page">
    /// The page.
    /// </param>
    /// <param name="tag">
    /// The tag.
    /// </param>
    /// <param name="defaultValue">
    /// The default value.
    /// </param>
    /// <returns>
    /// The get item.
    /// </returns>
    string GetItem([NotNull] string page, [NotNull] string tag, [NotNull] string defaultValue);

    #endregion
  }
}