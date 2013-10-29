/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
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

namespace YAF.Utils
{
    using System;
    using System.Web;
    using YAF.Classes;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;

    /// <summary>
    /// Class provides helper functions related to the forum path and urls as well as forum version information.
    /// </summary>
    public static class YafForumInfo
    {
        /// <summary>
        /// Gets the forum path (client-side).
        /// May not be the actual URL of the forum.
        /// </summary>
        public static string ForumClientFileRoot
        {
            get
            {
                return BaseUrlBuilder.ClientFileRoot;
            }
        }

        /// <summary>
        /// Gets the forum path (server-side).
        /// May not be the actual URL of the forum.
        /// </summary>
        public static string ForumServerFileRoot
        {
            get
            {
                return BaseUrlBuilder.ServerFileRoot;
            }
        }

        /// <summary>
        /// Gets complete application external (client-side) URL of the forum. (e.g. http://myforum.com/forum
        /// </summary>
        public static string ForumBaseUrl
        {
            get
            {
                return BaseUrlBuilder.BaseUrl + BaseUrlBuilder.AppPath;
            }
        }

        /// <summary>
        /// Gets full URL to the Root of the Forum
        /// </summary>
        public static string ForumURL
        {
            get
            {
                return YafBuildLink.GetLink(ForumPages.forum, true);
            }
        }

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
                string s = HttpContext.Current.Request.ServerVariables["SERVER_NAME"];
                return s != null && s.ToLower() == "localhost";
            }
        }

        /// <summary>
        /// Helper function that creates the the url of a resource.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>
        /// The get url to resource.
        /// </returns>
        public static string GetURLToResource([NotNull] string resourceName)
        {
            CodeContracts.VerifyNotNull(resourceName, "resourceName");

            return "{1}resources/{0}".FormatWith(resourceName, ForumClientFileRoot);
        }

        #region Version Information

        /// <summary>
        /// Gets the Current YAF Application Version string
        /// </summary>
        public static string AppVersionName
        {
            get
            {
                return AppVersionNameFromCode(AppVersionCode);
            }
        }

        /// <summary>
        /// Gets the Current YAF Database Version
        /// </summary>
        public static int AppVersion
        {
            get
            {
                return 52;
            }
        }

        private enum ReleaseType
        {
            Regular = 0,
            Alpha,
            BETA,
            RC,
        }

        /// <summary>
        /// Gets the Current YAF Application Version
        /// </summary>
        public static long AppVersionCode
        {
            get
            {
                int major = 2;
                byte minor = 0;
                byte build = 0;
                byte sub = 0;

                var releaseType = ReleaseType.Regular;
                byte releaseNumber = 0;
                
                long version = (long)major << 24;
                version |= (long)minor << 16;
                version |= (long)(build & 0x0F) << 12;

                if (sub > 0)
                {
                    version |= ((long)sub << 8);
                }

                if (releaseType != ReleaseType.Regular)
                {
                    version |= (long)releaseType << 4;
                    version |= (long)(releaseNumber & 0x0F) + 1;
                }

                return version;
            }
        }

        /// <summary>
        /// Gets the Current YAF Build Date
        /// </summary>
        public static DateTime AppVersionDate
        {
            get
            {
                return new DateTime(2013, 10, 28);
            }
        }

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
            string version = "{0}.{1}.{2}".FormatWith((code >> 24) & 0xFF, (code >> 16) & 0xFF, (code >> 12) & 0x0F);

            if (((code >> 8) & 0x0F) > 0)
            {
                version += ".{0}".FormatWith(((code >> 8) & 0x0F));
            }

            if (((code >> 4) & 0x0F) > 0)
            {
                var value = (code >> 4) & 0x0F;

                var number = String.Empty;

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
                    version += " {0} {1}".FormatWith(releaseType.ToString().ToUpper(), number);
                }
            }

            return version;
        }

        #endregion
    }
}