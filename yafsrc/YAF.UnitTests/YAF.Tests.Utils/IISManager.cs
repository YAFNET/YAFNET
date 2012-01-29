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
    using Microsoft.Web.Administration;

    using YAF.Utils;

    /// <summary>
    /// IISManager Class to modify IIS Apps
    /// </summary>
    public class IISManager
    {
        /// <summary>
        /// Creates the IIS application.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="physicalPath">The physical path.</param>
        public static void CreateIISApplication(string applicationName, string physicalPath)
        {
            using (ServerManager serverManager = new ServerManager())
            {
                var defaultSite = serverManager.Sites[TestConfig.DefaultWebsiteName];
                Application newApplication = defaultSite.Applications["/{0}".FormatWith(applicationName)];

                // Remove if exitsts?!
                if (newApplication != null)
                {
                    defaultSite.Applications.Remove(newApplication);
                    serverManager.CommitChanges();
                }

                defaultSite = serverManager.Sites[TestConfig.DefaultWebsiteName];
                newApplication = defaultSite.Applications.Add("/{0}".FormatWith(applicationName), physicalPath);

                newApplication.ApplicationPoolName = TestConfig.TestApplicationPool;

                serverManager.CommitChanges();
                serverManager.Dispose();
            }
        }

        /// <summary>
        /// Recycles the application pool.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        public static void RecycleApplicationPool(string applicationName)
        {
            ServerManager iisManager = new ServerManager();
            Site defaultSite = iisManager.Sites["Default Web Site"];
            var application = defaultSite.Applications["/{0}".FormatWith(applicationName)];

            if (application == null)
            {
                return;
            }

            string appPool = application.ApplicationPoolName;
            ApplicationPool pool = iisManager.ApplicationPools[appPool];
            pool.Recycle();
        }

        /// <summary>
        /// Deletes the IIS application.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        public static void DeleteIISApplication(string applicationName)
        {
            using (ServerManager serverManager = new ServerManager())
            {
                var defaultSite = serverManager.Sites[TestConfig.DefaultWebsiteName];
                Application newApplication = defaultSite.Applications["/{0}".FormatWith(applicationName)];

                // Remove
                if (newApplication != null)
                {
                    defaultSite.Applications.Remove(newApplication);
                    serverManager.CommitChanges();
                }

                serverManager.Dispose();
            }
        }
    }
}