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

namespace YAF.Core.Context
{
    using System;
    using System.Web;
    using System.Web.Http;
    using System.Web.UI;

    using YAF.Configuration;
    using YAF.Core.Context.Start;
    using YAF.Types.Extensions;
    using YAF.Utils;

    /// <summary>
    /// The YAF HttpApplication.
    /// </summary>
    public abstract class YafHttpApplication : HttpApplication
    {
        /// <summary>
        /// The application_ start.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected virtual void Application_Start(object sender, EventArgs e)
        {
            // Pass a delegate to the Configure method.
            GlobalConfiguration.Configure(WebApiConfig.Register);

            RegisterJQuery();
        }

        /// <summary>
        /// Registers the jQuery script library.
        /// </summary>
        private static void RegisterJQuery()
        {
            string jqueryUrl;

            // Check if override file is set ?
            if (Config.JQueryOverrideFile.IsSet())
            {
                jqueryUrl = !Config.JQueryOverrideFile.StartsWith("http") && !Config.JQueryOverrideFile.StartsWith("//")
                    ? BoardInfo.GetURLToScripts(Config.JQueryOverrideFile)
                    : Config.JQueryOverrideFile;
            }
            else
            {
                jqueryUrl = BoardInfo.GetURLToScripts($"jquery-{Config.JQueryVersion}.min.js");
            }

            // load jQuery
            ScriptManager.ScriptResourceMapping.AddDefinition(
                "jquery",
                new ScriptResourceDefinition
                {
                    Path = jqueryUrl,
                    DebugPath = BoardInfo.GetURLToScripts($"jquery-{Config.JQueryVersion}.js"),
                    CdnPath = $"//ajax.aspnetcdn.com/ajax/jQuery/jquery-{Config.JQueryVersion}.min.js",
                    CdnDebugPath = $"//ajax.aspnetcdn.com/ajax/jQuery/jquery-{Config.JQueryVersion}.js",
                    CdnSupportsSecureConnection = true /*,
                            LoadSuccessExpression = "window.jQuery"*/
                });
        }
    }
}