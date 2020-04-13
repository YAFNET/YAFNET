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
namespace YAF.Web.Controls
{
    #region Using

    using System;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using YAF.Core.BaseControls;
    using YAF.Types;
    using YAF.Types.Interfaces;

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

        /// <summary>Gets the topic icon.</summary>
        /// <param name="themeImageTag">The theme image tag.</param>
        /// <returns>Returns the topic icon.</returns>
        private static Icon GetTopicIcon(string themeImageTag)
        {
            var iconNew = new Icon
                                 {
                                     IconName = "comment",
                                     IconStackName = "comment",
                                     IconStackType = "fa-inverse",
                                     IconStackSize = "fa-1x",
                                     IconType = "text-success"
                                 };

            var icon = new Icon
                              {
                                  IconName = "comment",
                                  IconStackName = "comment",
                                  IconStackType = "fa-inverse",
                                  IconStackSize = "fa-1x",
                                  IconType = "text-secondary"
                              };

            switch (themeImageTag)
            {
                case "NEW_POSTS_LOCKED":
                    iconNew.IconStackName = "lock";
                    return iconNew;
                case "HOT_NEW_POSTS":
                    iconNew.IconStackName = "fire";
                    return iconNew;
                case "NEW_POSTS":
                    iconNew.IconStackName = "comment";
                    return iconNew;
                case "NO_NEW_POSTS_LOCKED":
                    icon.IconStackName = "lock";
                    return icon;
                case "HOT_NO_NEW_POSTS":
                    icon.IconStackName = "fire";
                    return icon;
                case "NO_NEW_POSTS":
                    icon.IconStackName = "comment";
                    return icon;
                case "MOVED":
                    icon.IconStackName = "arrows-alt";
                    return icon;
                default:
                    icon.IconStackName = "comment";
                    return icon;
            }
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
                    "TOPIC_MOVED"
                };

            string[] localizedTags =
                {
                    "NEW_POSTS", "NO_NEW_POSTS", "HOT_NEW_POSTS", "HOT_NO_NEW_POSTS", "NEW_POSTS_LOCKED",
                    "NO_NEW_POSTS_LOCKED", "MOVED"
                };

            HtmlGenericControl row = null;

            // add a table control
            var table = new HtmlGenericControl("div");

            // table.Attributes.Add(HtmlTextWriterAttribute.Class.ToString(), "container");
            this.Controls.Add(table);

            for (var i = 0; i < themeImageTags.Length; i++)
            {
                if (i % 2 == 0 || row == null)
                {
                    // add row
                    row = new HtmlGenericControl("div");
                    row.Attributes.Add(HtmlTextWriterAttribute.Class.ToString(), "row");
                    table.Controls.Add(row);
                }

                // add column
                var col = new HtmlGenericControl("div");
                col.Attributes.Add(HtmlTextWriterAttribute.Class.ToString(), "col");
                row.Controls.Add(col);

                // add the themed icons
                col.Controls.Add(GetTopicIcon(localizedTags[i]));

                var descriptionText = new Label
                                          {
                                              Text = this.GetText(localizedTags[i]),
                                              CssClass = "small"
                                          };

                // localized text describing the image
                col.Controls.Add(descriptionText);
            }
        }

        #endregion
    }
}