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
            this.RegisterCssFiles();

            this.RegisterJQuery();
        }

        /// <summary>
        /// Handles the PreRender event of the CurrentForumPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void CurrentForumPagePreRender([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.PageContext.Vars.ContainsKey("yafForumExtensions"))
            {
                return;
            }

#if DEBUG
            YafContext.Current.PageElements.RegisterJsScriptsInclude(
                "yafForumExtensions",
                this.PageContext.CurrentForumPage.IsAdminPage
                    ? "jquery.ForumAdminExtensions.js"
                    : "jquery.ForumExtensions.js");
#else
            YafContext.Current.PageElements.RegisterJsScriptsInclude(
                "yafForumExtensions",
                this.PageContext.CurrentForumPage.IsAdminPage
                    ? "jquery.ForumAdminExtensions.min.js"
                    : "jquery.ForumExtensions.min.js");
#endif

            this.PageContext.Vars["yafForumExtensions"] = true;
        }

        /// <summary>
        /// Registers the jQuery script library.
        /// </summary>
        private void RegisterJQuery()
        {
            var element = YafContext.Current.CurrentForumPage.TopPageControl;

            if (YafContext.Current.PageElements.PageElementExists("jquery") || Config.DisableJQuery)
            {
                return;
            }

            var registerJQuery = true;

            const string Key = "JQuery-Javascripts";

            // check to see if DotNetAge is around and has registered jQuery for us...
            if (HttpContext.Current.Items[Key] != null)
            {
                var collection = HttpContext.Current.Items[Key] as StringCollection;

                if (collection != null && collection.Contains("jquery"))
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
                    jqueryUrl = YafContext.Current.Get<YafBoardSettings>().JqueryCDNHosted
                                    ? "//ajax.aspnetcdn.com/ajax/jQuery/jquery-3.4.0.min.js"
#if DEBUG
                                    : YafForumInfo.GetURLToScripts("jquery-3.4.0.js");
#else
                                    : YafForumInfo.GetURLToScripts("jquery-3.4.0.min.js");
#endif
                }

                // load jQuery
                element.Controls.Add(ControlHelper.MakeJsIncludeControl(jqueryUrl));
            }

            YafContext.Current.PageElements.AddPageElement("jquery");
        }

        /// <summary>
        /// Register the CSS Files in the header.
        /// </summary>
        private void RegisterCssFiles()
        {
            var element = YafContext.Current.CurrentForumPage.TopPageControl;

            element.Controls.Add(
                ControlHelper.MakeCssIncludeControl(this.Get<ITheme>().BuildThemePath("bootstrap-forum.min.css")));

            // make the style sheet link controls.
            element.Controls.Add(
                this.PageContext.CurrentForumPage.IsAdminPage
                    ? ControlHelper.MakeCssIncludeControl(YafForumInfo.GetURLToContent("forum-admin.min.css"))
                    : ControlHelper.MakeCssIncludeControl(YafForumInfo.GetURLToContent("forum.min.css")));
        }

        #endregion
    }
}