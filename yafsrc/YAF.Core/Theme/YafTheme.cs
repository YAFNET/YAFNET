/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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

namespace YAF.Core.Theme
{
    #region Using

    using System.IO;
    using System.Web;

    using ServiceStack;

    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The YAF theme.
    /// </summary>
    public class YafTheme : ITheme
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="YafTheme"/> class.
        /// </summary>
        /// <param name="theme">
        /// The theme.
        /// </param>
        public YafTheme(string theme)
        {
            this.Theme = theme;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the current Theme File
        /// </summary>
        public string Theme { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Determines whether [is valid theme] [the specified theme].
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <returns>
        ///   <c>true</c> if [is valid theme] [the specified theme]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidTheme([NotNull] string theme)
        {
            CodeContracts.VerifyNotNull(theme, "theme");

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
        public string BuildThemePath([NotNull] string filename)
        {
            CodeContracts.VerifyNotNull(filename, "filename");

            return YafForumInfo.GetURLToContentThemes(this.Theme.CombineWith(filename));
        }

        #endregion

        /// <summary>
        /// Gets the mapped theme file.
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <returns>
        /// The get mapped theme file.
        /// </returns>
        private static string GetMappedThemeFile([NotNull] string theme)
        {
            CodeContracts.VerifyNotNull(theme, "theme");

            return
                HttpContext.Current.Server.MapPath($"{YafForumInfo.ForumServerFileRoot}Content/Themes/{theme.Trim()}");
        }
    }
}