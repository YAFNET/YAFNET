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
    #region Using

    using System;
    using System.Web.UI;

    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// The UserLabel
    /// </summary>
    public class UserLabel : BaseControl
    {
        #region Properties

        /// <summary>
        ///   Gets or sets CssClass.
        /// </summary>
        [NotNull]
        public string CssClass
        {
            get
            {
                return this.ViewState["CssClass"] != null ? this.ViewState["CssClass"].ToString() : string.Empty;
            }

            set
            {
                this.ViewState["CssClass"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets the onclick value for the profile link
        /// </summary>
        [NotNull]
        public string OnClick
        {
            get
            {
                return this.ViewState["OnClick"] != null ? this.ViewState["OnClick"].ToString() : string.Empty;
            }

            set
            {
                this.ViewState["OnClick"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets The onmouseover value for the profile link
        /// </summary>
        [NotNull]
        public string OnMouseOver
        {
            get
            {
                return this.ViewState["OnMouseOver"] != null ? this.ViewState["OnMouseOver"].ToString() : string.Empty;
            }

            set
            {
                this.ViewState["OnMouseOver"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets The name of the user for this profile link
        /// </summary>
        [NotNull]
        public string PostfixText
        {
            get
            {
                return this.ViewState["PostfixText"] != null ? this.ViewState["PostfixText"].ToString() : string.Empty;
            }

            set
            {
                this.ViewState["PostfixText"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets The replace Crawler name of this user for the link. Attention! Use it ONLY for crawlers. 
        /// </summary>
        [NotNull]
        public string CrawlerName
        {
            get
            {
                return this.ViewState["CrawlerName"] != null ? this.ViewState["CrawlerName"].ToString() : string.Empty;
            }

            set
            {
                this.ViewState["CrawlerName"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets Style.
        /// </summary>
        [NotNull]
        public string Style
        {
            get
            {
                return this.ViewState["Style"] != null ? this.ViewState["Style"].ToString() : string.Empty;
            }

            set
            {
                this.ViewState["Style"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets Style.
        /// </summary>
        [NotNull]
        public string ReplaceName
        {
            get
            {
                return this.ViewState["ReplaceName"] != null ? this.ViewState["ReplaceName"].ToString() : string.Empty;
            }

            set
            {
                this.ViewState["ReplaceName"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets The userid of this user for the link
        /// </summary>
        public int UserID
        {
            get
            {
                if (this.ViewState["UserID"] != null)
                {
                    return this.ViewState["UserID"].ToType<int>();
                }

                return -1;
            }

            set
            {
                this.ViewState["UserID"] = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="output">
        /// The output.
        /// </param>
        protected override void Render([NotNull] HtmlTextWriter output)
        {
            var displayName = this.ReplaceName.IsNotSet()
                              ? this.Get<IUserDisplayName>().GetName(this.UserID)
                              : this.ReplaceName;

            if (this.UserID == -1 || displayName.IsNotSet())
            {
                return;
            }

            output.BeginRender();

            output.WriteBeginTag("span");

            this.RenderMainTagAttributes(output);

            output.Write(HtmlTextWriter.TagRightChar);

            displayName = this.CrawlerName.IsNotSet() ? displayName : this.CrawlerName;

            output.WriteEncodedText(this.CrawlerName.IsNotSet() ? displayName : this.CrawlerName);

            output.WriteEndTag("span");

            if (this.PostfixText.IsSet())
            {
                output.Write(this.PostfixText);
            }

            output.EndRender();
        }

        /// <summary>
        /// Renders "id", "style", "onclick", "onmouseover" and "class"
        /// </summary>
        /// <param name="output">
        /// The output.
        /// </param>
        protected void RenderMainTagAttributes([NotNull] HtmlTextWriter output)
        {
            if (this.ClientID.IsSet())
            {
                output.WriteAttribute("id", this.ClientID);
            }

            if (this.Style.IsSet())
            {
                output.WriteAttribute("style", this.HtmlEncode(this.Style));
            }

            if (this.OnClick.IsSet())
            {
                output.WriteAttribute("onclick", this.OnClick);
            }

            if (this.OnMouseOver.IsSet())
            {
                output.WriteAttribute("onmouseover", this.OnMouseOver);
            }

            if (this.CssClass.IsSet())
            {
                output.WriteAttribute("class", this.CssClass);
            }
        }

        #endregion
    }
}