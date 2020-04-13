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
    using YAF.Types;
    using YAF.Types.Extensions;

    /// <summary>
    /// The icon.
    /// </summary>
    [ToolboxData("<{0}:Icon runat=server></{0}:Icon>")]
    public class Icon : BaseControl
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [Category("Appearance")]
        [NotNull]
        public string IconName { get; set; }

        /// <summary>
        /// Gets or sets the icon style.
        /// </summary>
        [DefaultValue("fas")]
        public string IconStyle { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        [CanBeNull]
        [DefaultValue("")]
        public string IconSize { get; set; }

        /// <summary>
        /// Gets or sets the icon type.
        /// </summary>
        public string IconType { get; set; }

        /// <summary>
        /// Gets or sets the icon name stack.
        /// </summary>
        [CanBeNull]
        public string IconStackName { get; set; }

        /// <summary>
        /// Gets or sets the icon stack type.
        /// </summary>
        [CanBeNull]
        public string IconStackType { get; set; }

        /// <summary>
        /// Gets or sets the icon stack size.
        /// </summary>
        [CanBeNull]
        public string IconStackSize { get; set; }

        /// <summary>
        /// Gets or sets the icon name badge.
        /// </summary>
        [CanBeNull]
        public string IconNameBadge { get; set; }

        /// <summary>
        /// Gets or sets the icon badge type.
        /// </summary>
        [CanBeNull]
        public string IconBadgeType { get; set; }

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

            if (this.IconNameBadge.IsSet())
            {
                this.RenderIconBadge(writer);
            }
            else if (this.IconStackName.IsSet())
            {
                this.RenderIconStack(writer);
            }
            else
            {
                this.RenderNormalIcon(writer);
            }
        }

        /// <summary>
        /// Render Normal Icon
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        private void RenderNormalIcon(HtmlTextWriter writer)
        {
            writer.WriteBeginTag(HtmlTextWriterTag.I.ToString());

            var className = this.IconType.IsSet() ? $"fa-fw {this.IconType} mr-1" : "fa-fw mr-1";

            if (this.IconSize.IsSet())
            {
                className += $" {this.IconSize}";
            }

            if (this.IconStyle.IsNotSet())
            {
                this.IconStyle = "fas";
            }

            writer.WriteAttribute(
                HtmlTextWriterAttribute.Class.ToString(),
                $"{this.IconStyle} fa-{this.IconName} {className}");

            writer.Write(HtmlTextWriter.TagRightChar);

            writer.WriteEndTag(HtmlTextWriterTag.I.ToString());
        }

        /// <summary>
        /// Render Badge Icon
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        private void RenderIconBadge(HtmlTextWriter writer)
        {
            // Render Stack Icons
            writer.WriteBeginTag(HtmlTextWriterTag.Span.ToString());

            writer.WriteAttribute(HtmlTextWriterAttribute.Class.ToString(), "fa-stack mr-1");

            writer.Write(HtmlTextWriter.TagRightChar);

            writer.WriteBeginTag(HtmlTextWriterTag.I.ToString());

            var className = this.IconType.IsSet() ? $"fa-stack-1x {this.IconType}" : "fa-stack-1x";

            if (this.IconStyle.IsNotSet())
            {
                this.IconStyle = "fas";
            }

            writer.WriteAttribute(
                HtmlTextWriterAttribute.Class.ToString(),
                $"{this.IconStyle} fa-{this.IconName} {className}");

            writer.Write(HtmlTextWriter.TagRightChar);

            writer.WriteEndTag(HtmlTextWriterTag.I.ToString());

            writer.Write(@"<i class=""fa fa-circle fa-badge-bg fa-inverse fa-outline-inverse""></i>");
            writer.WriteBeginTag(HtmlTextWriterTag.I.ToString());

            writer.WriteAttribute(
                HtmlTextWriterAttribute.Class.ToString(),
                $"fa fa-{this.IconNameBadge} fa-badge {this.IconBadgeType}");

            writer.Write(HtmlTextWriter.TagRightChar);
            writer.WriteEndTag(HtmlTextWriterTag.I.ToString());

            writer.WriteEndTag(HtmlTextWriterTag.Span.ToString());
        }

        /// <summary>
        /// Render Stack Icon
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        private void RenderIconStack(HtmlTextWriter writer)
        {
            // Render Stack Icons
            writer.WriteBeginTag(HtmlTextWriterTag.Span.ToString());

            if (this.IconStackSize.IsNotSet())
            {
                this.IconStackSize = "fa-2x";
            }

            writer.WriteAttribute(HtmlTextWriterAttribute.Class.ToString(), $"fa-stack {this.IconStackSize} mr-1");

            writer.Write(HtmlTextWriter.TagRightChar);

            writer.WriteBeginTag(HtmlTextWriterTag.I.ToString());

            var className = this.IconType.IsSet() ? $"fa-stack-2x {this.IconType}" : "fa-stack-2x";

            if (this.IconStyle.IsNotSet())
            {
                this.IconStyle = "fas";
            }

            writer.WriteAttribute(
                HtmlTextWriterAttribute.Class.ToString(),
                $"{this.IconStyle} fa-{this.IconName} {className}");

            writer.Write(HtmlTextWriter.TagRightChar);

            writer.WriteEndTag(HtmlTextWriterTag.I.ToString());

            writer.WriteBeginTag(HtmlTextWriterTag.I.ToString());

            writer.WriteAttribute(
                HtmlTextWriterAttribute.Class.ToString(),
                $"fa fa-{this.IconStackName} fa-stack-1x {this.IconStackType}");

            writer.Write(HtmlTextWriter.TagRightChar);
            writer.WriteEndTag(HtmlTextWriterTag.I.ToString());

            writer.WriteEndTag(HtmlTextWriterTag.Span.ToString());
        }
    }
}