/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
 * https://www.yetanotherforum.net/
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

namespace YAF.Tests.Utils;

using System;

using Microsoft.Web.Administration;

/// <summary>
/// IISManager Class to modify IIS Apps
/// </summary>
public static class IISManager
{
    /// <summary>
    /// Creates the IIS application.
    /// </summary>
    /// <param name="applicationName">Name of the application.</param>
    /// <param name="physicalPath">The physical path.</param>
    public static void CreateIISApplication(string applicationName, string physicalPath)
    {
        using (var serverManager = new ServerManager($@"\\{Environment.MachineName}\c$\windows\system32\inetsrv\config\applicationHost.config"))
        {
            var defaultSite = serverManager.Sites[TestConfig.DefaultWebsiteName];
            var newApplication = defaultSite.Applications[$"/{applicationName}"];

            // Remove if exists?!
            if (newApplication != null)
            {
                defaultSite.Applications.Remove(newApplication);
                serverManager.CommitChanges();
            }

            defaultSite = serverManager.Sites[TestConfig.DefaultWebsiteName];
            newApplication = defaultSite.Applications.Add($"/{applicationName}", physicalPath);

            newApplication.ApplicationPoolName = TestConfig.TestApplicationPool;

            serverManager.CommitChanges();
        }
    }

    /// <summary>
    /// Recycles the application pool.
    /// </summary>
    /// <param name="applicationName">Name of the application.</param>
    public static void RecycleApplicationPool(string applicationName)
    {
        var iisManager = new ServerManager();
        var defaultSite = iisManager.Sites["Default Web Site"];
        var application = defaultSite.Applications[$"/{applicationName}"];

        if (application == null)
        {
            return;
        }

        var appPool = application.ApplicationPoolName;
        var pool = iisManager.ApplicationPools[appPool];
        pool.Recycle();
    }

    /// <summary>
    /// Deletes the IIS application.
    /// </summary>
    /// <param name="applicationName">Name of the application.</param>
    public static void DeleteIISApplication(string applicationName)
    {
        using (var serverManager = new ServerManager())
        {
            var defaultSite = serverManager.Sites[TestConfig.DefaultWebsiteName];
            var newApplication = defaultSite.Applications[$"/{applicationName}"];

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