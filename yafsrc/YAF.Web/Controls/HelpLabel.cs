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

    using ServiceStack;

    using YAF.Core.BaseControls;
    using YAF.Core.Extensions;
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
        /// Gets or sets the associated control id.
        /// </summary>
        public virtual string AssociatedControlID { get; set; }

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
            get => this.localizedHelpTag.IsNotSet() ? $"{this.LocalizedTag}_HELP" : this.localizedHelpTag;

            set => this.localizedHelpTag = value;
        }

        /// <summary>
        /// Gets or sets LocalizedTag.
        /// </summary>
        public string LocalizedTag { get; set; } = string.Empty;

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

        /// <summary>
        /// Gets or sets Parameter Help 0.
        /// </summary>
        public string ParamHelp0 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets Parameter Help 1.
        /// </summary>
        public string ParamHelp1 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets Parameter Help 2.
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

            var text = this.GetText(this.LocalizedPage, this.LocalizedTag).Fmt(this.Param0, this.Param1, this.Param2);

            if (text.IsSet() && text.EndsWith(":"))
            {
                text = text.Remove(text.Length - 1, 1);
            }

            string associatedControlID;

            try
            {
                var control = this.FindControl(this.AssociatedControlID);
                associatedControlID = control.ClientID;
            }
            catch (Exception)
            {
                associatedControlID = string.Empty;
            }

            var label = new HtmlGenericControl("label");

            label.Attributes.Add(HtmlTextWriterAttribute.For.ToString(), associatedControlID);

            label.Controls.Add(new Literal { Text = $"{text}" });

            // Append Suffix
            if (this.Suffix.IsSet())
            {
                label.Controls.Add(new Literal { Text = this.Suffix });
            }

            var tooltip = this.GetText(this.LocalizedPage, this.LocalizedHelpTag).Fmt(
                this.ParamHelp0,
                this.ParamHelp1,
                this.ParamHelp2);

            tooltip = tooltip.IsSet() ? this.HtmlEncode(tooltip) : text;

            label.Controls.Add(new Literal { Text = "&nbsp;" });

            var button = new HtmlGenericControl("button");
            button.Attributes.Add(HtmlTextWriterAttribute.Type.ToString(), "button");
            button.Attributes.Add(HtmlTextWriterAttribute.Class.ToString(), "btn btn-sm");
            button.Attributes.Add(HtmlTextWriterAttribute.Title.ToString(), tooltip);

            button.Attributes.Add("data-toggle", "tooltip");
            button.Attributes.Add("data-placement", "right");

            button.Controls.Add(new Icon { IconName = "info-circle", IconSize = "fa-lg", IconType = "text-info" });

            label.Controls.Add(button);

            label.RenderControl(output);

            output.EndRender();
        }

        #endregion
    }
}