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
    using System.ComponentModel;
    using System.Web.UI;
    
    using YAF.Core.BaseControls;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;

    /// <summary>
    /// Alert Message Control
    /// </summary>
    [DefaultProperty("Message")]
    [ToolboxData("<{0}:Alert runat=server></{0}:Alert>")]
    public class Alert : BaseControl
    {
        /// <summary>
        /// Gets or sets the CSS class.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue("")]
        public string CssClass { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether dismissing.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool Dismissing { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether dismissing.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool MobileOnly { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [Category("Appearance")]
        [DefaultValue(MessageTypes.info)]
        public MessageTypes Type { get; set; }

        /// <summary>
        /// Outputs server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter" /> object and stores tracing information about the control if tracing is enabled.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter" /> object that receives the control content.</param>
        public override void RenderControl(HtmlTextWriter writer)
        {
            if (!this.Visible)
            {
                return;
            }

            writer.WriteBeginTag(HtmlTextWriterTag.Div.ToString());

            string cssClass;

            if (this.CssClass.IsSet())
            {
                cssClass = this.MobileOnly ? $" d-sm-none {this.CssClass}" : $" {this.CssClass}";
            }
            else
            {
                cssClass = this.MobileOnly ? " d-sm-none" : string.Empty;
            }

            writer.WriteAttribute(
                HtmlTextWriterAttribute.Class.ToString(),
                this.Dismissing
                    ? $"text-break alert alert-{this.Type.ToString()} alert-dismissible fade show{cssClass}"
                    : $"text-break alert alert-{this.Type.ToString()}{cssClass}");

            writer.WriteAttribute("role", "alert");

            writer.Write(HtmlTextWriter.TagRightChar);

            base.RenderControl(writer);

            if (this.Dismissing)
            {
                writer.Write("<button type=\"button\" class=\"btn-close\" data-dismiss=\"alert\" aria-label=\"Close\">");
                writer.Write("</button>");
            }

            writer.WriteEndTag(HtmlTextWriterTag.Div.ToString());
        }
    }
}