/* Yet Another Forum.NET
 *
 * Copyright (C) Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 3
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

namespace YAF.Tests.Utils
{
    using System;
    using System.Configuration;

    using YAF.Utils;

    /// <summary>
    /// Configuration Settings
    /// </summary>
    public class TestConfig
    {
        /// <summary>
        /// Gets the test forum ID.
        /// </summary>
        public static int TestForumID
        {
            get
            {
                return ConfigurationManager.AppSettings["YAF.TestForumID"].ToType<int>();
            }
        }

        /// <summary>
        /// Gets the test topic ID.
        /// </summary>
        public static int TestTopicID
        {
            get
            {
                return ConfigurationManager.AppSettings["YAF.TestTopicID"].ToType<int>();
            }
        }

        /// <summary>
        /// Gets a value indicating whether [use existing installation].
        /// </summary>
        /// <value>
        ///   <c>true</c> if use existing installation; otherwise, <c>false</c>.
        /// </value>
        public static bool UseExistingInstallation
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["YAF.UseExistingInstallation"]);
            }
        }

        /// <summary>
        /// Gets the config password.
        /// </summary>
        public static string ConfigPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["YAF.ConfigPassword"];
            }
        }

        /// <summary>
        /// Gets the test database.
        /// </summary>
        public static string TestDatabase
        {
            get
            {
                return ConfigurationManager.AppSettings["YAF.TestDatabase"];
            }
        }

        /// <summary>
        /// Gets the database server.
        /// </summary>
        public static string DatabaseServer
        {
            get
            {
                return ConfigurationManager.AppSettings["YAF.DatabaseServer"];
            }
        }

        /// <summary>
        /// Gets the install test site URL.
        /// </summary>
        public static string InstallTestSiteURL
        {
            get
            {
                return "http://localhost/{0}/".FormatWith(TestApplicationName);
            }
        }

        /// <summary>
        /// Gets the test application pool.
        /// </summary>
        public static string TestApplicationPool
        {
            get
            {
                return ConfigurationManager.AppSettings["YAF.TestApplicationPool"];
            }
        }

        /// <summary>
        /// Gets the name of the test application.
        /// </summary>
        /// <value>
        /// The name of the test application.
        /// </value>
        public static string TestApplicationName
        {
            get
            {
                return ConfigurationManager.AppSettings["YAF.TestApplicationName"];
            }
        }

        /// <summary>
        /// Gets the package location.
        /// </summary>
        public static string PackageLocation
        {
            get
            {
                return ConfigurationManager.AppSettings["YAF.PackageLocation"];
            }
        }
        
        /// <summary>
        /// Gets the install physical path.
        /// </summary>
        public static string InstallPhysicalPath
        {
            get
            {
                return ConfigurationManager.AppSettings["YAF.InstallPhysicalPath"];
            }
        }

        /// <summary>
        /// Gets the Release download URL.
        /// </summary>
        public static string ReleaseDownloadUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["YAF.ReleaseDownloadUrl"];
            }
        }

        /// <summary>
        /// Gets the local release package file.
        /// </summary>
        public static string LocalReleasePackageFile
        {
            get
            {
                return ConfigurationManager.AppSettings["YAF.LocalReleasePackageFile"];
            }
        }

        /// <summary>
        /// Gets the name of the admin user.
        /// </summary>
        public static string AdminUserName
        {
            get
            {
                return ConfigurationManager.AppSettings["YAF.AdminUserName"];
            }
        }

        /// <summary>
        /// Gets the admin password.
        /// </summary>
        public static string AdminPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["YAF.AdminPassword"];
            }
        }

        /// <summary>
        /// Gets the name of the test user.
        /// </summary>
        /// <value>
        /// The name of the test user.
        /// </value>
        public static string TestUserName
        {
            get
            {
                return ConfigurationManager.AppSettings["YAF.TestUserName"];
            }
        }

        /// <summary>
        /// Gets the test user password.
        /// </summary>
        public static string TestUserPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["YAF.TestUserPassword"];
            }
        }

        /// <summary>
        /// Gets Default Website Name.
        /// </summary>
        public static string DefaultWebsiteName
        {
            get
            {
                return ConfigurationManager.AppSettings["YAF.DefaultWebsiteName"];
            }
        }

        /// <summary>
        /// Gets Test Forum Url.
        /// </summary>
        public static string TestForumUrl
        {
            get
            {
                return !UseExistingInstallation
                           ? InstallTestSiteURL
                           : ConfigurationManager.AppSettings["YAF.TestForumUrl"];
            }
        }

        /// <summary>
        /// Gets a value indicating whether [use test mail server].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use test mail server]; otherwise, <c>false</c>.
        /// </value>
        public static bool UseTestMailServer
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["YAF.UseTestMailServer"]);
            }
        }

        /// <summary>
        /// Gets the test mail host.
        /// </summary>
        public static string TestMailHost
        {
            get
            {
                return ConfigurationManager.AppSettings["YAF.TestMailHost"];
            }
        }

        /// <summary>
        /// Gets the test mail password.
        /// </summary>
        public static string TestMailPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["YAF.TestMailPassword"];
            }
        }

        /// <summary>
        /// Gets the name of the test mail user.
        /// </summary>
        /// <value>
        /// The name of the test mail user.
        /// </value>
        public static string TestMailUserName
        {
            get
            {
                return ConfigurationManager.AppSettings["YAF.TestMailUserName"];
            }
        }

        /// <summary>
        /// Gets the test mail port.
        /// </summary>
        public static string TestMailPort
        {
            get
            {
                return ConfigurationManager.AppSettings["YAF.TestMailPort"];
            }
        }

        /// <summary>
        /// Gets the test forum mail.
        /// </summary>
        public static string TestForumMail
        {
            get
            {
                return ConfigurationManager.AppSettings["YAF.TestForumMail"];
            }
        }
    }
}
