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

namespace YAF.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;

    using YAF.Configuration;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Objects;

    /// <summary>
    /// Class provides helper functions related to the forum path and URLs as well as forum version information.
    /// </summary>
    public static class BoardInfo
    {
        /// <summary>
        /// Gets the forum path (client-side).
        /// May not be the actual URL of the forum.
        /// </summary>
        public static string ForumClientFileRoot => BaseUrlBuilder.ClientFileRoot;

        /// <summary>
        /// Gets the forum path (server-side).
        /// May not be the actual URL of the forum.
        /// </summary>
        public static string ForumServerFileRoot => BaseUrlBuilder.ServerFileRoot;

        /// <summary>
        /// Gets complete application external (client-side) URL of the forum. (e.g. http://domain.com/forum
        /// </summary>
        public static string ForumBaseUrl => $"{BaseUrlBuilder.BaseUrl}{BaseUrlBuilder.AppPath}";

        /// <summary>
        /// Gets full URL to the Root of the Forum
        /// </summary>
        public static string ForumURL => BuildLink.GetLink(ForumPages.forum, true);

        /// <summary>
        /// Gets a value indicating whether this instance is local.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is local; otherwise, <c>false</c>.
        /// </value>
        public static bool IsLocal
        {
            get
            {
                var serverName = HttpContext.Current.Request.ServerVariables["SERVER_NAME"];
                return serverName != null && serverName.ToLower() == "localhost";
            }
        }

        #region Version Information

        /// <summary>
        /// Gets the Current YAF Application Version string
        /// </summary>
        public static string AppVersionName => AppVersionNameFromCode(AppVersionCode);

        /// <summary>
        /// Gets the Current YAF Database Version
        /// </summary>
        public static int AppVersion => 75;

        /// <summary>
        /// Gets the Current YAF Application Version
        /// </summary>
        public static byte[] AppVersionCode
        {
            get
            {
                const int Major = 2;
                const byte Minor = 3;
                const byte Build = 2;
                const byte Sub = 0;

                const ReleaseType ReleaseType = ReleaseType.BETA;
                const byte ReleaseNumber = 0;

                var list = new List<int>
                               {
                                   Major,
                                   Minor,
                                   Build,
                                   Sub,
                                   ReleaseType.ToType<int>(),
                                   ReleaseNumber
                               };

                return list.SelectMany(BitConverter.GetBytes).ToArray();
            }
        }

        /// <summary>
        /// Gets the Current YAF Build Date
        /// </summary>
        public static DateTime AppVersionDate => new DateTime(2020, 04, 19, 03, 00, 00);

        /// <summary>
        /// Creates a string that is the YAF Application Version from a long value
        /// </summary>
        /// <param name="code">
        /// Value of Current Version
        /// </param>
        /// <returns>
        /// Application Version String
        /// </returns>
        public static string AppVersionNameFromCode(byte[] code)
        {
            var originalList = code.ToListOf(BitConverter.ToInt32);

            var version = new YafVersion
                              {
                                  Major = originalList[0],
                                  Minor = originalList[1],
                                  Build = originalList[2],
                                  Sub = originalList[3],
                                  ReleaseType = originalList[4].ToEnum<ReleaseType>(),
                                  ReleaseNumber = originalList[5]
                              };

            var versionString = new StringBuilder();

            versionString.AppendFormat("{0}.{1}{2}", version.Major, version.Minor, version.Build);

            if (version.Sub > 0)
            {
                versionString.AppendFormat(".{0}", version.Sub);
            }

            if (version.ReleaseType == ReleaseType.Regular)
            {
                return versionString.ToString();
            }

            var number = version.ReleaseNumber >= 1
                             ? version.ReleaseNumber.ToString()
                             : AppVersionDate.ToString("yyyyMMddHHmm");

            versionString.AppendFormat(" {0} {1}", version.ReleaseType.ToString().ToUpper(), number);

            return versionString.ToString();
        }

        #endregion

        /// <summary>
        /// Helper function that creates the URL to the Content folder.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>
        /// Returns the URL including the Content path
        /// </returns>
        public static string GetURLToContent([NotNull] string resourceName)
        {
            CodeContracts.VerifyNotNull(resourceName, "resourceName");

            return $"{ForumClientFileRoot}Content/{resourceName}";
        }

        /// <summary>
        /// Helper function that creates the URL to the Content  themes folder.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>
        /// Returns the URL including the Content Themes path
        /// </returns>
        public static string GetURLToContentThemes([NotNull] string resourceName)
        {
            CodeContracts.VerifyNotNull(resourceName, "resourceName");

            return $"{ForumClientFileRoot}Content/Themes/{resourceName}";
        }

        /// <summary>
        /// Helper function that creates the URL to the Scripts folder.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>
        /// Returns the URL including the Scripts path
        /// </returns>
        public static string GetURLToScripts([NotNull] string resourceName)
        {
            CodeContracts.VerifyNotNull(resourceName, "resourceName");

            return $"{ForumClientFileRoot}Scripts/{resourceName}";
        }
    }
}