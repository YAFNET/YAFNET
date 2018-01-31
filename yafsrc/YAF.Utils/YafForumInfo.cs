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

namespace YAF.Utils
{
    using System;
    using System.Text;
    using System.Web;
    using YAF.Classes;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;

    /// <summary>
    /// Class provides helper functions related to the forum path and URLs as well as forum version information.
    /// </summary>
    public static class YafForumInfo
    {
        /// <summary>
        /// The YAF.NET Release Type
        /// </summary>
        private enum ReleaseType
        {
            /// <summary>
            /// regular release
            /// </summary>
            Regular = 0,

            /// <summary>
            /// alpha release
            /// </summary>
            Alpha,

            /// <summary>
            /// beta release
            /// </summary>
            BETA,

            /// <summary>
            /// release candidate release
            /// </summary>
            RC
        }

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
        public static string ForumBaseUrl => "{0}{1}".FormatWith(BaseUrlBuilder.BaseUrl, BaseUrlBuilder.AppPath);

        /// <summary>
        /// Gets full URL to the Root of the Forum
        /// </summary>
        public static string ForumURL => YafBuildLink.GetLink(ForumPages.forum, true);

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
        public static int AppVersion => 61;

        /// <summary>
        /// Gets the Current YAF Application Version
        /// </summary>
        public static long AppVersionCode
        {
            get
            {
                const int Major = 2;
                const byte Minor = 3;
                const byte Build = 0;
                const byte Sub = 0;

                const ReleaseType ReleaseType = ReleaseType.BETA;
                const byte ReleaseNumber = 0;

                var version = Major.ToType<long>() << 24;
                version |= Minor.ToType<long>() << 16;
                version |= (Build & 0x0F).ToType<long>() << 12;

                if (Sub > 0)
                {
                    version |= Sub.ToType<long>() << 8;
                }

                if (ReleaseType != ReleaseType.Regular)
                {
                    version |= ReleaseType.ToType<long>() << 4;
                    version |= (ReleaseNumber & 0x0F).ToType<long>() + 1;
                }

                return version;
            }
        }

        /// <summary>
        /// Gets the Current YAF Build Date
        /// </summary>
        public static DateTime AppVersionDate => new DateTime(2018, 01, 30);

        /// <summary>
        /// Creates a string that is the YAF Application Version from a long value
        /// </summary>
        /// <param name="code">
        /// Value of Current Version
        /// </param>
        /// <returns>
        /// Application Version String
        /// </returns>
        public static string AppVersionNameFromCode(long code)
        {
            var version = new StringBuilder();

            version.AppendFormat("{0}.{1}.{2}", (code >> 24) & 0xFF, (code >> 16) & 0xFF, (code >> 12) & 0x0F);

            if (((code >> 8) & 0x0F) > 0)
            {
                version.AppendFormat(".{0}", (code >> 8) & 0x0F);
            }

            if (((code >> 4) & 0x0F) <= 0)
            {
                return version.ToString();
            }

            var value = (code >> 4) & 0x0F;

            var number = string.Empty;

            if ((code & 0x0F) > 1)
            {
                number = ((code & 0x0F).ToType<int>() - 1).ToString();
            }
            else if ((code & 0x0F) == 1)
            {
                number = AppVersionDate.ToString("yyyyMMdd");
            }

            var releaseType = value.ToEnum<ReleaseType>();

            if (releaseType != ReleaseType.Regular)
            {
                version.AppendFormat(" {0} {1}", releaseType.ToString().ToUpper(), number);
            }

            return version.ToString();
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

            return "{1}Content/{0}".FormatWith(resourceName, ForumClientFileRoot);
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

            return "{1}Scripts/{0}".FormatWith(resourceName, ForumClientFileRoot);
        }
    }
}