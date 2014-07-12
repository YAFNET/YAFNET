/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014 Ingo Herbote
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
    using System.ComponentModel;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using YAF.Types.Extensions;

    /// <summary>
    /// ModernBTextBox Control html5 style
    /// </summary>
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:ModernTextBox runat=server></{0}:ModernTextBox>")]
    public class ModernTextBox : TextBox
    {
        /// <summary>
        /// The default CSS class
        /// </summary>
        private const string DEFAULTCSSCLASS = "standardTextInput";

        /// <summary>
        /// The label text
        /// </summary>
        private string labelText = string.Empty;

        /// <summary>
        /// The wrapper CSS class
        /// </summary>
        private string wrapperCssClass = "formElement";

        /// <summary>
        /// The placeholder text
        /// </summary>
        private string placeholder = "enter value";

        /// <summary>
        /// Gets or sets the placeholder text.
        /// </summary>
        /// <value>
        /// The placeholder text.
        /// </value>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("enter value")]
        [Localizable(true)]
        public string Placeholder
        {
            get
            {
                return this.placeholder;
            }

            set
            {
                this.placeholder = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [render wrapper].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render wrapper]; otherwise, <c>false</c>.
        /// </value>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool RenderWrapper { get; set; }

        /// <summary>
        /// Gets or sets the placeholder text.
        /// </summary>
        /// <value>
        /// The placeholder text.
        /// </value>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("formElement")]
        public string WrapperCssClass
        {
            get
            {
                return this.wrapperCssClass;
            }

            set
            {
                this.wrapperCssClass = value;
            }
        }

        /// <summary>
        /// Gets or sets the localized label page.
        /// </summary>
        /// <value>
        /// The localized label page.
        /// </value>
        [Bindable(true)]
        [Category("Appearance")]
        public string LabelText
        {
            get
            {
                return this.labelText;
            }

            set
            {
                this.labelText = value;
            }
        }

        /// <summary>
        /// Outputs server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter" /> object and stores tracing information about the control if tracing is enabled.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter" /> object that receives the control content.</param>
        public override void RenderControl(HtmlTextWriter writer)
        {
            if (this.RenderWrapper)
            {
                writer.WriteBeginTag(HtmlTextWriterTag.Div.ToString());
                writer.WriteAttribute(HtmlTextWriterAttribute.Class.ToString(), this.wrapperCssClass);
                writer.Write(HtmlTextWriter.TagRightChar);

                if (this.labelText.IsSet())
                {
                    this.RenderLabel(writer);
                }

                base.RenderControl(writer);

                writer.WriteEndTag(HtmlTextWriterTag.Div.ToString());
            }
            else
            {
                base.RenderControl(writer);
            }
        }

        /// <summary>
        /// Adds HTML attributes and styles that need to be rendered to the specified <see cref="T:System.Web.UI.HtmlTextWriter" /> instance.
        /// </summary>
        /// <param name="writer">An <see cref="T:System.Web.UI.HtmlTextWriter" /> that represents the output stream to render HTML content on the client.</param>
        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            if (CssClass.IsNotSet())
            {
                this.CssClass = DEFAULTCSSCLASS;
            }

            writer.AddAttribute("placeholder", this.placeholder);

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
    }
}