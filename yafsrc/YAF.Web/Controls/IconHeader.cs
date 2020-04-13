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

    using System.Web.UI;

    using ServiceStack;

    using YAF.Core.BaseControls;
    using YAF.Core.Context;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// Icon Header creates a header with Icon and localized header text
    /// </summary>
    public class IconHeader : BaseControl, ILocalizationSupport
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether enable bb code.
        /// </summary>
        public bool EnableBBCode { get; set; }

        /// <summary>
        /// Gets or sets the icon name.
        /// </summary>
        public string IconName { get; set; }

        /// <summary>
        /// Gets or sets the icon type.
        /// </summary>
        public string IconType { get; set; }

        /// <summary>
        /// Gets or sets the icon style.
        /// </summary>
        public string IconStyle { get; set; }

        /// <summary>
        /// Gets or sets the icon size.
        /// </summary>
        public string IconSize { get; set; }

        /// <summary>
        /// Gets or sets the override text.
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets LocalizedPage.
        /// </summary>
        public string LocalizedPage { get; set; } = BoardContext.Current.ForumPageType.ToString().ToUpper();

        /// <summary>
        /// Gets or sets LocalizedTag.
        /// </summary>
        public string LocalizedTag { get; set; } = "TITLE";

        /// <summary>
        /// Gets or sets Parameter 0.
        /// </summary>
        public string Param0 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets Parameter 1.
        /// </summary>
        public string Param1 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets Parameter 2.
        /// </summary>
        public string Param2 { get; set; } = string.Empty;

        #endregion

        #region Methods

        /// <summary>
        /// Shows the localized text string (if available)
        /// </summary>
        /// <param name="output">The output.</param>
        protected override void Render(HtmlTextWriter output)
        {
            output.BeginRender();

            if (string.IsNullOrEmpty(this.IconType))
            {
                this.IconType = "text-secondary";
            }

            var icon = new Icon { IconName = this.IconName, IconType = this.IconType, IconSize = this.IconSize };

            if (this.IconStyle.IsSet())
            {
                icon.IconStyle = this.IconStyle;
            }

            icon.RenderControl(output);

            if (this.Text.IsSet())
            {
                output.Write(this.Text);
            }
            else
            {
                var header = this.GetText(this.LocalizedPage, this.LocalizedTag).Fmt(this.Param0, this.Param1, this.Param2);

                output.Write(header);
            }

            output.EndRender();
        }

        #endregion
    }
}