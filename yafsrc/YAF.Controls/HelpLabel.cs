/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2016 Ingo Herbote
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
        /// The _enable bb code.
        /// </summary>
        protected bool _enableBBCode;

        /// <summary>
        /// The _suffix.
        /// </summary>
        protected string _suffix = string.Empty;

        /// <summary>
        /// The _localized page.
        /// </summary>
        protected string _localizedPage = string.Empty;

        /// <summary>
        /// The _localized tag.
        /// </summary>
        protected string _localizedTag = string.Empty;

        /// <summary>
        /// The _localized tag.
        /// </summary>
        protected string _localizedHelpTag = string.Empty;

        /// <summary>
        /// The _param 0.
        /// </summary>
        protected string _param0 = string.Empty;

        /// <summary>
        /// The _param 1.
        /// </summary>
        protected string _param1 = string.Empty;

        /// <summary>
        /// The _param 2.
        /// </summary>
        protected string _param2 = string.Empty;

        /// <summary>
        /// The _param 0.
        /// </summary>
        protected string _paramHelp0 = string.Empty;

        /// <summary>
        /// The _param 1.
        /// </summary>
        protected string _paramHelp1 = string.Empty;

        /// <summary>
        /// The _param 2.
        /// </summary>
        protected string _paramHelp2 = string.Empty;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpLabel"/> class.
        /// </summary>
        public HelpLabel()
            : base()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether EnableBBCode.
        /// </summary>
        public bool EnableBBCode
        {
            get
            {
                return this._enableBBCode;
            }

            set
            {
                this._enableBBCode = value;
            }
        }

        /// <summary>
        /// Gets or sets Suffix. e.g: ":" or "?"
        /// </summary>
        public string Suffix
        {
            get
            {
                return this._suffix;
            }

            set
            {
                this._suffix = value;
            }
        }

        /// <summary>
        /// Gets or sets LocalizedPage.
        /// </summary>
        public string LocalizedPage
        {
            get
            {
                return this._localizedPage;
            }

            set
            {
                this._localizedPage = value;
            }
        }

        /// <summary>
        /// Gets or sets LocalizedTag.
        /// </summary>
        public string LocalizedHelpTag
        {
            get
            {
                return string.IsNullOrEmpty(this._localizedHelpTag)
                           ? "{0}_HELP".FormatWith(this._localizedTag)
                           : this._localizedHelpTag;
            }

            set
            {
                this._localizedHelpTag = value;
            }
        }

        /// <summary>
        /// Gets or sets LocalizedTag.
        /// </summary>
        public string LocalizedTag
        {
            get
            {
                return this._localizedTag;
            }

            set
            {
                this._localizedTag = value;
            }
        }

        /// <summary>
        /// Gets or sets Param0.
        /// </summary>
        public string Param0
        {
            get
            {
                return this._param0;
            }

            set
            {
                this._param0 = value;
            }
        }

        /// <summary>
        /// Gets or sets Param1.
        /// </summary>
        public string Param1
        {
            get
            {
                return this._param1;
            }

            set
            {
                this._param1 = value;
            }
        }

        /// <summary>
        /// Gets or sets Param2.
        /// </summary>
        public string Param2
        {
            get
            {
                return this._param2;
            }

            set
            {
                this._param2 = value;
            }
        }

        /// <summary>
        /// Gets or sets ParamHelp0.
        /// </summary>
        public string ParamHelp0
        {
            get
            {
                return this._paramHelp0;
            }

            set
            {
                this._paramHelp0 = value;
            }
        }

        /// <summary>
        /// Gets or sets ParamHelp1.
        /// </summary>
        public string ParamHelp1
        {
            get
            {
                return this._paramHelp1;
            }

            set
            {
                this._paramHelp1 = value;
            }
        }

        /// <summary>
        /// Gets or sets ParamHelp2.
        /// </summary>
        public string ParamHelp2
        {
            get
            {
                return this._paramHelp2;
            }

            set
            {
                this._paramHelp2 = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Shows the localized text string (if available)
        /// </summary>
        /// <param name="output">The output.</param>
        protected override void Render(HtmlTextWriter output)
        {
            output.BeginRender();

            var text = this.GetText(this.LocalizedPage, this.LocalizedTag)
                    .FormatWith(this.Param0, this.Param1, this.Param2);

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

            var tooltip = this.GetText(this.LocalizedPage, this.LocalizedHelpTag)
                .FormatWith(this.ParamHelp0, this.ParamHelp1, this.ParamHelp2);

            tooltip = tooltip.IsSet() ? this.HtmlEncode(tooltip) : text;

            output.Write(
                "&nbsp;<button type=\"button\" class=\"btn btn-primary btn-circle\" data-toggle=\"tooltip\" data-placement=\"right\" title=\"{0}\"><i class=\"fa fa-question\"></i></button>",
                tooltip);

            output.EndRender();
        }

        #endregion
    }
}