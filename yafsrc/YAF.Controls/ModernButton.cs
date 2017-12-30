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
    using System.ComponentModel;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    /// <summary>
    /// Modern Button Control html5 style
    /// </summary>
    [ParseChildren(false)]
    [PersistChildren(true)]
    [ToolboxData("<{0}:ModernButton runat=server></{0}:ModernButton>")]
    public class ModernButton : Button
    {
        /// <summary>
        /// Gets or sets a value indicating whether [enable loading animation].
        /// </summary>
        /// <value>
        /// <c>true</c> if [enable loading animation]; otherwise, <c>false</c>.
        /// </value>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool EnableLoadingAnimation { get; set; }
        
        /// <summary>
        /// Gets or sets the text caption displayed in the <see cref="T:System.Web.UI.WebControls.Button" /> control.
        /// </summary>
        /// <returns>The text caption displayed in the <see cref="T:System.Web.UI.WebControls.Button" /> control. The default value is <see cref="F:System.String.Empty" />.</returns>
        public new string Text
        {
            get
            {
                return this.ViewState["NewText"] as string;
            }

            set
            {
                this.ViewState["NewText"] = HttpUtility.HtmlDecode(value);
            }
        }

        /// <summary>
        /// Gets the name of the control tag. This property is used primarily by control developers.
        /// </summary>
        /// <returns>The name of the control tag.</returns>
        protected override string TagName
        {
            get
            {
                return "button";
            }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.UI.HtmlTextWriterTag" /> value that corresponds to this Web server control. This property is used primarily by control developers.
        /// </summary>
        /// <returns>One of the <see cref="T:System.Web.UI.HtmlTextWriterTag" /> enumeration values.</returns>
        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Button;
            }
        }

        /// <summary>
        /// Determines whether the button has been clicked prior to rendering on the client.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnPreRender(System.EventArgs e)
        {
            if (this.EnableLoadingAnimation)
            {
                this.CssClass += " ladda-button";
            }

            base.OnPreRender(e);

            var literalControl = new LiteralControl(this.Text);

            this.Controls.Add(literalControl);

            base.Text = this.UniqueID;
        }

        /// <summary>
        /// Renders the contents of the control to the specified writer.
        /// </summary>
        /// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter" /> object that represents the output stream to render HTML content on the client.</param>
        protected override void RenderContents(HtmlTextWriter writer)
        {
            this.RenderChildren(writer);
        }
    }
}