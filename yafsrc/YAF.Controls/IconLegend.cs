/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
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
namespace YAF.Controls
{
    #region Using

    using System;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using YAF.Core;
    using YAF.Types;

    #endregion

    /// <summary>
    /// Icon Legend Control to Render Topic Icons
    /// </summary>
    public class IconLegend : BaseControl
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "IconLegend" /> class.
        /// </summary>
        public IconLegend()
        {
            this.Load += this.IconLegendLoad;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void Render([NotNull] HtmlTextWriter writer)
        {
            base.Render(writer);
        }

        /// <summary>
        /// Icons the legend load.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void IconLegendLoad([NotNull] object sender, [NotNull] EventArgs e)
        {
            string[] themeImageTags =
                {
                    "TOPIC_NEW", "TOPIC", "TOPIC_HOT_NEW", "TOPIC_HOT", "TOPIC_NEW_LOCKED", "TOPIC_LOCKED",
                    "TOPIC_ANNOUNCEMENT_NEW", "TOPIC_ANNOUNCEMENT", "TOPIC_STICKY_NEW", "TOPIC_STICKY",
                    "TOPIC_POLL_NEW", "TOPIC_POLL", "TOPIC_MOVED"
                };

            string[] localizedTags =
                {
                    "NEW_POSTS", "NO_NEW_POSTS", "HOT_NEW_POSTS", "HOT_NO_NEW_POSTS", "NEW_POSTS_LOCKED",
                    "NO_NEW_POSTS_LOCKED", "ANNOUNCEMENT_NEW", "ANNOUNCEMENT", "STICKY_NEW", "STICKY", "POLL_NEW",
                    "POLL", "MOVED"
                };

            HtmlGenericControl tr = null;

            // add a table control
            var table = new HtmlGenericControl("div");
            table.Attributes.Add(HtmlTextWriterAttribute.Class.ToString(), "container");
            this.Controls.Add(table);

            for (var i = 0; i < themeImageTags.Length; i++)
            {
                if (i % 2 == 0 || tr == null)
                {
                    // add <tr>
                    tr = new HtmlGenericControl("div");
                    tr.Attributes.Add(HtmlTextWriterAttribute.Class.ToString(), "row");
                    table.Controls.Add(tr);
                }

                // add this to the tr...
                var td = new HtmlGenericControl("div");
                td.Attributes.Add(HtmlTextWriterAttribute.Class.ToString(), "col");
                tr.Controls.Add(td);

                // add the themed icons
                var themeImage = new ThemeImage { ThemeTag = themeImageTags[i] };
                td.Controls.Add(themeImage);

                // space
                var space = new Literal { Text = " " };
                td.Controls.Add(space);

                // localized text describing the image
                var localLabel = new LocalizedLabel { LocalizedTag = localizedTags[i] };
                td.Controls.Add(localLabel);
            }
        }

        #endregion
    }
}