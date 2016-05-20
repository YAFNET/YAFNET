/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2016 Ingo Herbote
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
            this.CurrentForumPage.PreRender += this.CurrentForumPage_PreRender;
            this.CurrentForumPage.Load += this.CurrentForumPage_Load;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the Load event of the ForumPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void CurrentForumPage_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // Load CSS First
            this.RegisterCSSFiles();

            this.RegisterJQuery();
        }

        /// <summary>
        /// Handles the PreRender event of the CurrentForumPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void CurrentForumPage_PreRender([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.RegisterJQueryUI();

            if (this.PageContext.Vars.ContainsKey("yafForumExtensions"))
            {
                return;
            }
#if DEBUG
            YafContext.Current.PageElements.RegisterJsScriptsInclude("yafForumExtensions", "jquery.ForumExtensions.js");
#else
                YafContext.Current.PageElements.RegisterJsScriptsInclude("yafForumExtensions", "jquery.ForumExtensions.min.js");
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
                                    ? "//ajax.aspnetcdn.com/ajax/jQuery/jquery-2.2.3.min.js"
#if DEBUG
                                    : YafForumInfo.GetURLToScripts("jquery-2.2.3.js");
#else
                                    : YafForumInfo.GetURLToScripts("jquery-2.2.3.min.js");
#endif
                }

                // load jQuery
                element.Controls.Add(ControlHelper.MakeJsIncludeControl(jqueryUrl));
            }

            YafContext.Current.PageElements.AddPageElement("jquery");
        }
        
        /// <summary>
        /// Register the jQuery UI script library in the header.
        /// </summary>
        private void RegisterJQueryUI()
        {
            var element = YafContext.Current.CurrentForumPage.TopPageControl;

            // If registered or told not to register, don't bother
            if (YafContext.Current.PageElements.PageElementExists("jqueryui"))
            {
                return;
            }

            string jqueryUIUrl;

            // Check if override file is set ?
            if (Config.JQueryUIOverrideFile.IsSet())
            {
                jqueryUIUrl = !Config.JQueryUIOverrideFile.StartsWith("http")
                              && !Config.JQueryUIOverrideFile.StartsWith("//")
                                  ? YafForumInfo.GetURLToScripts(Config.JQueryOverrideFile)
                                  : Config.JQueryUIOverrideFile;
            }
            else
            {
                jqueryUIUrl = YafContext.Current.Get<YafBoardSettings>().JqueryUICDNHosted
                                  ? "//ajax.aspnetcdn.com/ajax/jquery.ui/1.11.4/jquery-ui.min.js"
#if DEBUG
                                  : YafForumInfo.GetURLToScripts("jquery-ui-1.11.4.js");
#else
                                  : YafForumInfo.GetURLToScripts("jquery-ui-1.11.4.min.js");
#endif
            }

            // load jQuery UI from google...
            element.Controls.Add(ControlHelper.MakeJsIncludeControl(jqueryUIUrl));

            YafContext.Current.PageElements.AddPageElement("jqueryui");
        }

        /// <summary>
        /// Register the CSS Files in the header.
        /// </summary>
        private void RegisterCSSFiles()
        {
            var element = YafContext.Current.CurrentForumPage.TopPageControl;

            // make the style sheet link controls.
#if DEBUG
            element.Controls.Add(ControlHelper.MakeCssIncludeControl(YafForumInfo.GetURLToContent("forum.css")));
#else
            element.Controls.Add(ControlHelper.MakeCssIncludeControl(YafForumInfo.GetURLToContent("forum.min.css")));
#endif

            element.Controls.Add(ControlHelper.MakeCssIncludeControl(this.Get<ITheme>().BuildThemePath("theme.css")));

            // Register the jQueryUI Theme CSS
            if (YafContext.Current.Get<YafBoardSettings>().JqueryUIThemeCDNHosted)
            {
                YafContext.Current.PageElements.RegisterCssInclude(
                     "//code.jquery.com/ui/1.11.4/themes/{0}/jquery-ui.min.css".FormatWith(
                         YafContext.Current.Get<YafBoardSettings>().JqueryUITheme));
            }
            else
            {
                YafContext.Current.PageElements.RegisterCssIncludeContent(
                    "themes/{0}/jquery-ui.min.css".FormatWith(YafContext.Current.Get<YafBoardSettings>().JqueryUITheme));
            }
        }

        #endregion
    }
}