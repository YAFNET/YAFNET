﻿/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
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

namespace YAF.Modules
{
    #region Using

    using System;
    using System.Collections.Specialized;
    using System.Web;
    using System.Web.UI;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Attributes;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// Automatic JavaScript Loading Module
    /// </summary>
    [YafModule("JavaScript Loading Module", "Ingo Herbote", 1)]
    public class ScriptsLoaderModule : SimpleBaseForumModule
    {
        #region Public Methods

        /// <summary>
        /// The initializes after page.
        /// </summary>
        public override void InitAfterPage()
        {
            this.CurrentForumPage.PreRender += this.CurrentForumPagePreRender;
            this.CurrentForumPage.Load += this.CurrentForumPageLoad;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the Load event of the ForumPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void CurrentForumPageLoad([NotNull] object sender, [NotNull] EventArgs e)
        {
            // Load CSS First
            this.RegisterCssFiles(this.Get<YafBoardSettings>().CdvVersion);
        }

        /// <summary>
        /// Handles the PreRender event of the CurrentForumPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void CurrentForumPagePreRender([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.RegisterJQuery();

            if (this.PageContext.Vars.ContainsKey("yafForumExtensions"))
            {
                return;
            }

            var version = this.Get<YafBoardSettings>().CdvVersion;

            ScriptManager.ScriptResourceMapping.AddDefinition(
                "yafForumAdminExtensions",
                new ScriptResourceDefinition
                    {
                        Path = YafForumInfo.GetURLToScripts($"jquery.ForumAdminExtensions.min.js?v={version}"),
                        DebugPath = YafForumInfo.GetURLToScripts($"jquery.ForumAdminExtensions.js?v={version}")
                });

            ScriptManager.ScriptResourceMapping.AddDefinition(
                "yafForumExtensions",
                new ScriptResourceDefinition
                    {
                        Path = YafForumInfo.GetURLToScripts($"jquery.ForumExtensions.min.js?v={version}"),
                        DebugPath = YafForumInfo.GetURLToScripts($"jquery.ForumExtensions.js?v={version}")
                });

            ScriptManager.ScriptResourceMapping.AddDefinition(
                "FileUploadScript",
                new ScriptResourceDefinition
                    {
                        Path = YafForumInfo.GetURLToScripts("jquery.fileupload.comb.min.js"),
                        DebugPath = YafForumInfo.GetURLToScripts("jquery.fileupload.comb.js")
                    });

            YafContext.Current.PageElements.AddScriptReference(
                this.PageContext.CurrentForumPage.IsAdminPage ? "yafForumAdminExtensions" : "yafForumExtensions");

            this.PageContext.Vars["yafForumExtensions"] = true;
        }

        /// <summary>
        /// Registers the jQuery script library.
        /// </summary>
        private void RegisterJQuery()
        {
            if (YafContext.Current.PageElements.PageElementExists("jquery"))
            {
                return;
            }

            var registerJQuery = true;

            const string Key = "JQuery-Javascripts";

            // check to see if DotNetAge is around and has registered jQuery for us...
            if (HttpContext.Current.Items[Key] != null)
            {
                if (HttpContext.Current.Items[Key] is StringCollection collection && collection.Contains("jquery"))
                {
                    registerJQuery = false;
                }
            }
            else if (Config.IsDotNetNuke)
            {
                // latest version of DNN (v5) should register jQuery for us...
                registerJQuery = false;
            }

            if (registerJQuery)
            {
                string jqueryUrl;

                // Check if override file is set ?
                if (Config.JQueryOverrideFile.IsSet())
                {
                    jqueryUrl = !Config.JQueryOverrideFile.StartsWith("http")
                                && !Config.JQueryOverrideFile.StartsWith("//")
                                    ? YafForumInfo.GetURLToScripts(Config.JQueryOverrideFile)
                                    : Config.JQueryOverrideFile;
                }
                else
                {
                    jqueryUrl = $"~/forum/Scripts/jquery-{Config.JQueryVersion}.min.js";
                }

                // load jQuery
                // element.Controls.Add(ControlHelper.MakeJsIncludeControl(jqueryUrl));
                ScriptManager.ScriptResourceMapping.AddDefinition(
                    "jquery",
                    new ScriptResourceDefinition
                        {
                            Path = jqueryUrl,
                            DebugPath = YafForumInfo.GetURLToScripts($"jquery-{Config.JQueryVersion}.js"),
                            CdnPath = $"//ajax.aspnetcdn.com/ajax/jQuery/jquery-{Config.JQueryVersion}.min.js",
                            CdnDebugPath = $"//ajax.aspnetcdn.com/ajax/jQuery/jquery-{Config.JQueryVersion}.js",
                            CdnSupportsSecureConnection = true/*,
                            LoadSuccessExpression = "window.jQuery"*/
                        });

                YafContext.Current.PageElements.AddScriptReference("jquery");
            }

            YafContext.Current.PageElements.AddPageElement("jquery");
        }

        /// <summary>
        /// Register the CSS Files in the header.
        /// </summary>
        /// <param name="version">
        /// The version.
        /// </param>
        private void RegisterCssFiles(int version)
        {
            var element = YafContext.Current.CurrentForumPage.TopPageControl;

            element.Controls.Add(
                ControlHelper.MakeCssIncludeControl(
                    $"{this.Get<ITheme>().BuildThemePath("bootstrap-forum.min.css")}?v={version}"));

            // make the style sheet link controls.
            element.Controls.Add(
                ControlHelper.MakeCssIncludeControl(
                    YafForumInfo.GetURLToContent(
                        this.PageContext.CurrentForumPage.IsAdminPage
                            ? $"forum-admin.min.css?v={version}"
                            : $"forum.min.css?v={version}")));
        }

        #endregion
    }
}