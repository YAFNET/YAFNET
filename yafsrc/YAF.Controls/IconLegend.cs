/* Yet Another Forum.NET
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
namespace YAF.Controls
{
    #region Using

    using System;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Extensions;

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
                var icon = new Literal { Text =
                                           $"<span class=\"fa-stack fa-1x\">{this.GetTopicIcon(localizedTags[i])}</span>"
                                       };
                td.Controls.Add(icon);

                // space
                var space = new Literal { Text = " " };
                td.Controls.Add(space);

                // localized text describing the image
                var localLabel = new LocalizedLabel { LocalizedTag = localizedTags[i] };
                td.Controls.Add(localLabel);
            }
        }

        /// <summary>Gets the topic icon.</summary>
        /// <param name="themeImageTag">The theme image tag.</param>
        /// <returns>Returns the topic icon.</returns>
        private string GetTopicIcon(string themeImageTag)
        {
            switch (themeImageTag)
            {
                case "POLL_NEW":
                    return
                        "<i class=\"fa fa-comment fa-stack-2x\" style=\"color:green\"></i><i class=\"fa fa-poll-h fa-stack-1x fa-inverse\"></i>";
                case "STICKY_NEW":
                    return
                        "<i class=\"fa fa-comment fa-stack-2x\" style=\"color:green\"></i><i class=\"fa fa-sticky-note fa-stack-1x fa-inverse\"></i>";
                case "ANNOUNCEMENT_NEW":
                    return
                        "<i class=\"fa fa-comment fa-stack-2x\" style=\"color:green\"></i><i class=\"fa fa-bullhorn fa-stack-1x fa-inverse\"></i>";
                case "NEW_POSTS_LOCKED":
                    return
                        "<i class=\"fa fa-comment fa-stack-2x\" style=\"color:green\"></i><i class=\"fa fa-lock fa-stack-1x fa-inverse\"></i>";
                case "HOT_NEW_POSTS":
                    return
                        "<i class=\"fa fa-comment fa-stack-2x\" style=\"color:green\"></i><i class=\"fa fa-fire fa-stack-1x fa-inverse\"></i>";
                case "NEW_POSTS":
                    return
                        "<i class=\"fa fa-comment fa-stack-2x\" style=\"color:green\"></i><i class=\"fa fa-comment fa-stack-1x fa-inverse\"></i>";
                case "POLL":
                    return "<i class=\"fa fa-comment fa-stack-2x\"></i><i class=\"fa fa-poll-h fa-stack-1x fa-inverse\"></i>";
                case "STICKY":
                    return
                        "<i class=\"fa fa-comment fa-stack-2x\"></i><i class=\"fa fa-sticky-note fa-stack-1x fa-inverse\"></i>";
                case "ANNOUNCEMENT":
                    return
                        "<i class=\"fa fa-comment fa-stack-2x\"></i><i class=\"fa fa-bullhorn fa-stack-1x fa-inverse\"></i>";
                case "NO_NEW_POSTS_LOCKED":
                    return
                        "<i class=\"fa fa-comment fa-stack-2x\"></i><i class=\"fa fa-lock fa-stack-1x fa-inverse\"></i>";
                case "HOT_NO_NEW_POSTS":
                    return
                        "<i class=\"fa fa-comment fa-stack-2x\"></i><i class=\"fa fa-fire fa-stack-1x fa-inverse\"></i>";
                case "NO_NEW_POSTS":
                    return
                        "<i class=\"fa fa-comment fa-stack-2x\"></i><i class=\"fa fa-comment fa-stack-1x fa-inverse\"></i>";
                case "MOVED":
                    return
                        "<i class=\"fa fa-comment fa-stack-2x\"></i><i class=\"fa fa-arrows-alt fa-stack-1x fa-inverse\"></i>";
                default:
                    return
                        "<i class=\"fa fa-comment fa-stack-2x\"></i><i class=\"fa fa-comment fa-stack-1x fa-inverse\"></i>";
            }
        }

        #endregion
    }
}