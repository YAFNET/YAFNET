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
    using System;
    using System.ComponentModel;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using YAF.Core;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;

    /// <summary>
    /// 
    /// </summary>
    [DefaultProperty("Message")]
    [ToolboxData("<{0}:Alert runat=server></{0}:Altert>")]
    public class Alert : BaseControl
    {
        public Alert()
        {

        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("enter value")]
        [Localizable(true)]
        public string Message { get; set; } = string.Empty;

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
            /*if (this.RenderWrapper)
            {*/ /*
                writer.WriteBeginTag(HtmlTextWriterTag.Div.ToString());
                writer.WriteAttribute(HtmlTextWriterAttribute.Class.ToString(), this.wrapperCssClass);
                writer.Write(HtmlTextWriter.TagRightChar);

                if (this.labelText.IsSet())
                {
                    this.RenderLabel(writer);
                }

                base.RenderControl(writer);*/

            writer.WriteEndTag(HtmlTextWriterTag.Div.ToString());
            /*}*/
            /*else
            {*/
            base.RenderControl(writer);
            //}
        }
        /*
        /// <summary>
        /// Adds HTML attributes and styles that need to be rendered to the specified <see cref="T:System.Web.UI.HtmlTextWriter" /> instance.
        /// </summary>
        /// <param name="writer">An <see cref="T:System.Web.UI.HtmlTextWriter" /> that represents the output stream to render HTML content on the client.</param>
        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            if (this.CssClass.IsNotSet())
            {
                this.CssClass = DEFAULTCSSCLASS;
            }

            writer.AddAttribute("placeholder", this.placeholder);

            var textMode = this.Type;

            switch (textMode)
            {
                default:
                    writer.AddAttribute(HtmlTextWriterAttribute.Type, GetTypeAttributeValue(textMode));
                    break;
            }

            base.AddAttributesToRender(writer);
        }

        /// <summary>
        /// Renders the label.
        /// </summary>
        /// <param name="writer">The writer.</param>
        private void RenderLabel(HtmlTextWriter writer)
        {
            writer.WriteBeginTag(HtmlTextWriterTag.Label.ToString());
            writer.WriteAttribute(HtmlTextWriterAttribute.For.ToString(), this.ClientID);
            writer.Write(HtmlTextWriter.TagRightChar);

            writer.Write(this.labelText);

            writer.WriteEndTag(HtmlTextWriterTag.Label.ToString());
        }
        */

    }
}