/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 * https://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core.Services;

using System.IO;

/// <summary>
/// The YAF theme.
/// </summary>
public class Theme : ITheme
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Theme"/> class.
    /// </summary>
    /// <param name="themeFile">
    /// The theme.
    /// </param>
    public Theme(string themeFile)
    {
        this.ThemeFile = themeFile;
    }

    /// <summary>
    ///   Gets or sets the current Theme File
    /// </summary>
    public string ThemeFile { get; set; }

    /// <summary>
    /// Determines whether [is valid theme] [the specified theme].
    /// </summary>
    /// <param name="theme">The theme.</param>
    /// <returns>
    ///   <c>true</c> if [is valid theme] [the specified theme]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsValidTheme(string theme)
    {
        return theme.IsSet() && Directory.Exists(GetMappedThemeFile(theme));
    }

    /// <summary>
    /// Gets full path to the given theme file.
    /// </summary>
    /// <param name="filename">
    /// Short name of theme file.
    /// </param>
    /// <returns>
    /// The build theme path.
    /// </returns>
    public string BuildThemePath(string filename)
    {
        return BoardContext.Current.Get<BoardInfo>().GetUrlToContentThemes(this.ThemeFile.CombineWith(filename));
    }

    /// <summary>
    /// Gets mapped path to the given theme file.
    /// </summary>
    /// <param name="filename">Short name of theme file.</param>
    /// <returns>The build theme path.</returns>
    public string BuildMappedThemePath(string filename)
    {
        var webRootPath = BoardContext.Current.Get<BoardInfo>().WebRootPath;

        return Path.Combine(webRootPath, "css", "themes", this.ThemeFile, filename);
    }


    /// <summary>
    /// Gets the mapped theme file.
    /// </summary>
    /// <param name="theme">The theme.</param>
    /// <returns>
    /// The get mapped theme file.
    /// </returns>
    private static string GetMappedThemeFile(string theme)
    {
        var webRootPath = BoardContext.Current.Get<BoardInfo>().WebRootPath;

        return Path.Combine(webRootPath, "css","themes", theme.Trim());
    }
}