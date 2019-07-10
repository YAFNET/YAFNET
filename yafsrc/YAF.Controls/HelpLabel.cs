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

    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using YAF.Core;
    using YAF.Core.BaseControls;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// Makes a very simple localized label
    /// </summary>
    public class HelpLabel : BaseControl, ILocalizationSupport
    {
        #region Constants and Fields

        /// <summary>
        /// The _localized tag.
        /// </summary>
        private string localizedHelpTag = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether EnableBBCode.
        /// </summary>
        public bool EnableBBCode { get; set; }

        /// <summary>
        /// Gets or sets Suffix. e.g: ":" or "?"
        /// </summary>
        public string Suffix { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets LocalizedPage.
        /// </summary>
        public string LocalizedPage { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets LocalizedTag.
        /// </summary>
        public string LocalizedHelpTag
        {
            get =>
                string.IsNullOrEmpty(this.localizedHelpTag)
                    ? $"{this.LocalizedTag}_HELP"
                    : this.localizedHelpTag;

            set => this.localizedHelpTag = value;
        }

        /// <summary>
        /// Gets or sets LocalizedTag.
        /// </summary>
        public string LocalizedTag { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets Param0.
        /// </summary>
        public string Param0 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets Param1.
        /// </summary>
        public string Param1 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets Param2.
        /// </summary>
        public string Param2 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets ParamHelp0.
        /// </summary>
        public string ParamHelp0 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets ParamHelp1.
        /// </summary>
        public string ParamHelp1 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets ParamHelp2.
        /// </summary>
        public string ParamHelp2 { get; set; } = string.Empty;

        #endregion

        #region Methods

        /// <summary>
        /// Shows the localized text string (if available)
        /// </summary>
        /// <param name="output">The output.</param>
        protected override void Render(HtmlTextWriter output)
        {
            output.BeginRender();

            output.Write("<h6>");

            var text = string
                    .Format(this.GetText(this.LocalizedPage, this.LocalizedTag), this.Param0, this.Param1, this.Param2);

            if (text.IsSet() && text.EndsWith(":"))
            {
                text = text.Remove(text.Length - 1, 1);
            }

            // Write Title
            output.Write(text);

            // Append Suffix
            if (this.Suffix.IsSet())
            {
                output.Write(this.Suffix);
            }

            var tooltip = string
                .Format(this.GetText(this.LocalizedPage, this.LocalizedHelpTag), this.ParamHelp0, this.ParamHelp1, this.ParamHelp2);

            tooltip = tooltip.IsSet() ? this.HtmlEncode(tooltip) : text;

            output.Write("&nbsp;");

            var button = new HtmlGenericControl("button");
            button.Attributes.Add(HtmlTextWriterAttribute.Type.ToString(), "button");
            button.Attributes.Add(HtmlTextWriterAttribute.Class.ToString(), "btn btn-info btn-circle btn-sm");
            button.Attributes.Add(HtmlTextWriterAttribute.Title.ToString(), tooltip);

            button.Attributes.Add("data-toggle", "tooltip");
            button.Attributes.Add("data-placement", "right");

            button.Controls.Add(new Literal { Text = "<i class=\"fa fa-question\"></i>" });

            button.RenderControl(output);

            output.Write("</h6>");

            output.EndRender();
        }

        #endregion
    }
}