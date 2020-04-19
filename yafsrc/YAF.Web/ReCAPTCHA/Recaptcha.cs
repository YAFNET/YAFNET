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
    using System.ComponentModel;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils.Helpers;
    using YAF.Web.ReCAPTCHA;

    #endregion

    /// <summary>
    /// The reCAPTCHA control.
    /// </summary>
    public class RecaptchaControl : WebControl, IValidator
    {
        #region Constants and Fields

        /// <summary>
        ///   The error message.
        /// </summary>
        private string errorMessage;

        /// <summary>
        ///   The reCAPTCHA response.
        /// </summary>
        private RecaptchaResponse recaptchaResponse;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RecaptchaControl" /> class.
        /// </summary>
        public RecaptchaControl()
        {
            this.SkipRecaptcha = false;
            this.SecretKey = BoardContext.Current.Get<BoardSettings>().RecaptchaPrivateKey;
            this.SiteKey = BoardContext.Current.Get<BoardSettings>().RecaptchaPublicKey;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets ErrorMessage.
        /// </summary>
        [NotNull]
        [DefaultValue("The verification words are incorrect.")]
        [Localizable(true)]
        public string ErrorMessage
        {
            get => this.errorMessage ?? "The verification words are incorrect.";

            set => this.errorMessage = value;
        }

        /// <summary>
        ///   Gets or sets a value indicating whether IsValid.
        /// </summary>
        [Browsable(false)]
        public bool IsValid
        {
            get
            {
                if (this.SkipRecaptcha)
                {
                    return true;
                }

                if (this.recaptchaResponse == null)
                {
                    this.Validate();
                }

                return this.recaptchaResponse != null && this.recaptchaResponse.IsValid;
            }

            set => throw new NotImplementedException("This setter is not implemented.");
        }

        /// <summary>
        ///   Gets or sets SecretKey.
        /// </summary>
        [Description("The secret key.")]
        [Category("Settings")]
        public string SecretKey { get; set; }

        /// <summary>
        ///   Gets or sets Site Key.
        /// </summary>
        [Category("Settings")]
        [Description("The Site key.")]
        public string SiteKey { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether Skip reCAPTCHA.
        /// </summary>
        [Description("Set this to true to stop reCAPTCHA validation. Useful for testing platform.")]
        [DefaultValue(false)]
        [Category("Settings")]
        public bool SkipRecaptcha { get; set; }

        #endregion

        #region Implemented Interfaces

        #region IValidator

        /// <summary>
        /// The validate.
        /// </summary>
        public void Validate()
        {
            if (this.SkipRecaptcha)
            {
                this.recaptchaResponse = RecaptchaResponse.Valid;
            }

            if (this.recaptchaResponse != null)
            {
                return;
            }

            var validator = new RecaptchaValidator
                                {
                                    SecretKey = this.SecretKey,
                                    RemoteIP = this.Page.Request.GetUserRealIPAddress(),
                                    Response = this.Context.Request.Form["g-recaptcha-response"]
                                };
            try
            {
                this.recaptchaResponse = validator.Validate();
            }
            catch (ArgumentNullException exception)
            {
                this.recaptchaResponse = null;
                this.errorMessage = exception.Message;
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            base.OnInit(e);

            if (!this.Visible)
            {
                return;
            }

            if (this.SiteKey.IsNotSet() || this.SecretKey.IsNotSet())
            {
                throw new ApplicationException("reCAPTCHA needs to be configured with a site & secret key.");
            }
        }

        /// <summary>
        /// Renders the control to the specified HTML writer.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter" /> object that receives the control content.</param>
        protected override void Render([NotNull] HtmlTextWriter writer)
        {
            if (this.SkipRecaptcha)
            {
                writer.WriteLine(
                    "reCAPTCHA validation is skipped. Set SkipRecaptcha property to false to enable validation.");
            }
            else
            {
                if (this.Visible)
                {
                    this.RenderContents(writer);
                }
            }
        }

        /// <summary>
        /// Renders the contents.
        /// </summary>
        /// <param name="output">The output.</param>
        protected override void RenderContents([NotNull] HtmlTextWriter output)
        {
            output.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
            output.AddAttribute(HtmlTextWriterAttribute.Src, "//www.google.com/recaptcha/api.js", false);
            output.RenderBeginTag(HtmlTextWriterTag.Script);
            output.RenderEndTag();

            output.AddAttribute(HtmlTextWriterAttribute.Class, "g-recaptcha");
            output.AddAttribute("data-sitekey", this.SiteKey);
            output.RenderBeginTag(HtmlTextWriterTag.Div);
            output.RenderEndTag();
        }

        #endregion
    }
}