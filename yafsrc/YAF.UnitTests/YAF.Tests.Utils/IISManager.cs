/* Yet Another Forum.NET
 *
 * Copyright (C) Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
 * documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
 * to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions 
 * of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
 * CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
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

                // Remove if exists?!
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