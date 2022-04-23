/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

namespace YAF.Modules
{
    using System.Web.UI.HtmlControls;
    using YAF.Types.Attributes;

    /// <summary>
    ///     Generates the Favicon code inside the head section
    /// </summary>
    [Module("Favicon Module", "Ingo Herbote", 1)]
    public class FaviconModule : SimpleBaseForumModule
    {
        /// <summary>
        /// The initialization after page.
        /// </summary>
        public override void InitAfterPage()
        {
            this.CurrentForumPage.PreRender += this.CurrentForumPage_PreRender;
        }

        /// <summary>
        ///     Handles the PreRender event of the ForumPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void CurrentForumPage_PreRender([NotNull] object sender, [NotNull] EventArgs e)
        {
            var head = this.ForumControl.Page.Header
                       ?? this.CurrentForumPage.FindControlRecursiveBothAs<HtmlHead>("YafHead");

            if (head == null || Config.IsAnyPortal)
            {
                return;
            }

            // Link tags
            var appleTouchIcon = new HtmlLink { Href = BoardInfo.GetURLToContent("favicons/apple-touch-icon.png") };
            appleTouchIcon.Attributes.Add("rel", "apple-touch-icon");
            appleTouchIcon.Attributes.Add("sizes", "180x180");
            head.Controls.Add(appleTouchIcon);

            var icon32 = new HtmlLink { Href = BoardInfo.GetURLToContent("favicons/favicon-32x32.png") };
            icon32.Attributes.Add("rel", "icon");
            icon32.Attributes.Add("sizes", "32x32");
            head.Controls.Add(icon32);

            var icon16 = new HtmlLink { Href = BoardInfo.GetURLToContent("favicons/favicon-16x16.png") };
            icon16.Attributes.Add("rel", "icon");
            icon16.Attributes.Add("sizes", "16x16");
            head.Controls.Add(icon16);

            var manifest = new HtmlLink { Href = BoardInfo.GetURLToContent("favicons/site.webmanifest") };
            manifest.Attributes.Add("rel", "manifest");
            head.Controls.Add(manifest);

            var maskIcon = new HtmlLink { Href = BoardInfo.GetURLToContent("favicons/safari-pinned-tab.svg") };
            maskIcon.Attributes.Add("rel", "mask-icon");
            maskIcon.Attributes.Add("color", "#5bbad5");
            head.Controls.Add(maskIcon);

            var shortcutIcon = new HtmlLink { Href = BoardInfo.GetURLToContent("favicons/favicon.ico") };
            shortcutIcon.Attributes.Add("rel", "shortcut icon");
            head.Controls.Add(shortcutIcon);

            // Meta Tags
            head.Controls.Add(new HtmlMeta { Name = "msapplication-TileColor", Content = "#da532c" });
            head.Controls.Add(
                new HtmlMeta
                {
                    Name = "msapplication-config",
                    Content = BoardInfo.GetURLToContent("favicons/browserconfig.xml")
                });
            head.Controls.Add(new HtmlMeta { Name = "theme-color", Content = "#ffffff" });
        }
    }
}