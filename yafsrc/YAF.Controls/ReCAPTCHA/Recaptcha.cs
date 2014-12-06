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

using YAF.Utils.Helpers;

namespace YAF.Controls
{
  #region Using

  using System;
  using System.ComponentModel;
  using System.Text;
  using System.Web.UI;
  using System.Web.UI.WebControls;

  using YAF.Types;

  #endregion

  /// <summary>
  /// The recaptcha control.
  /// </summary>
  public class RecaptchaControl : WebControl, IValidator
  {
    #region Constants and Fields

    /// <summary>
    ///   The recaptch a_ challeng e_ field.
    /// </summary>
    private const string RECAPTCHA_CHALLENGE_FIELD = "recaptcha_challenge_field";

    /// <summary>
    ///   The recaptch a_ host.
    /// </summary>
    private const string RECAPTCHA_HOST = "http://api.recaptcha.net";

    /// <summary>
    ///   The recaptch a_ respons e_ field.
    /// </summary>
    private const string RECAPTCHA_RESPONSE_FIELD = "recaptcha_response_field";

    /// <summary>
    ///   The recaptch a_ secur e_ host.
    /// </summary>
    private const string RECAPTCHA_SECURE_HOST = "https://api-secure.recaptcha.net";

    /// <summary>
    ///   The allow multiple instances.
    /// </summary>
    private bool allowMultipleInstances;

    // = YafContext.Current.BoardSettings.RecaptureMultipleInstances;
    /// <summary>
    ///   The custom theme widget.
    /// </summary>
    private string customThemeWidget;

    /// <summary>
    ///   The error message.
    /// </summary>
    private string errorMessage;

    /// <summary>
    ///   The language.
    /// </summary>
    private string language;

    /// <summary>
    ///   The override secure mode.
    /// </summary>
    private bool overrideSecureMode;

    // = YafContext.Current.BoardSettings.RecaptchaPrivateKey; 
    // ConfigurationManager.AppSettings["RecaptchaPrivateKey"];

    // = YafContext.Current.BoardSettings.RecaptchaPublicKey;
    // ConfigurationManager.AppSettings["RecaptchaPublicKey"];

    /// <summary>
    ///   The recaptcha response.
    /// </summary>
    private RecaptchaResponse recaptchaResponse;

    /// <summary>
    ///   The skip recaptcha.
    /// </summary>
    private bool skipRecaptcha;

    /// <summary>
    ///   The theme.
    /// </summary>
    private string theme;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "RecaptchaControl" /> class.
    /// </summary>
    public RecaptchaControl()
    {
      /* if (!bool.TryParse(ConfigurationManager.AppSettings["RecaptchaSkipValidation"], out this.skipRecaptcha))
            {
                this.skipRecaptcha = false;
            }
            */
      this.skipRecaptcha = false;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets a value indicating whether AllowMultipleInstances.
    /// </summary>
    [Category("Settings")]
    [DefaultValue(false)]
    [Description(
      "Set this to true to enable multiple reCAPTCHA on a single page. There may be complication between controls when this is enabled."
      )]
    public bool AllowMultipleInstances
    {
      get
      {
        return this.allowMultipleInstances;
      }

      set
      {
        this.allowMultipleInstances = value;
      }
    }

    /// <summary>
    ///   Gets or sets CustomThemeWidget.
    /// </summary>
    [Category("Appearence")]
    [Description("When using custom theming, this is a div element which contains the widget. ")]
    [DefaultValue((string)null)]
    public string CustomThemeWidget
    {
      get
      {
        return this.customThemeWidget;
      }

      set
      {
        this.customThemeWidget = value;
      }
    }

    /// <summary>
    ///   Gets or sets ErrorMessage.
    /// </summary>
    [NotNull, DefaultValue("The verification words are incorrect.")]
    [Localizable(true)]
    public string ErrorMessage
    {
      get
      {
        if (this.errorMessage != null)
        {
          return this.errorMessage;
        }

        return "The verification words are incorrect.";
      }

      set
      {
        this.errorMessage = value;
      }
    }

    /// <summary>
    ///   Gets or sets a value indicating whether IsValid.
    /// </summary>
    /// <exception cref = "NotImplementedException">
    /// </exception>
    [Browsable(false)]
    public bool IsValid
    {
      get
      {
        if ((!this.Page.IsPostBack || !this.Visible) || (!this.Enabled || this.skipRecaptcha))
        {
          return true;
        }

        if (this.recaptchaResponse == null)
        {
          this.Validate();
        }

        return (this.recaptchaResponse != null) && this.recaptchaResponse.IsValid;
      }

      set
      {
        throw new NotImplementedException("This setter is not implemented.");
      }
    }

    /// <summary>
    ///   Gets or sets a value indicating whether OverrideSecureMode.
    /// </summary>
    [Category("Settings")]
    [Description("Set this to true to override reCAPTCHA usage of Secure API.")]
    [DefaultValue(false)]
    public bool OverrideSecureMode
    {
      get
      {
        return this.overrideSecureMode;
      }

      set
      {
        this.overrideSecureMode = value;
      }
    }

    /// <summary>
    ///   Gets or sets PrivateKey.
    /// </summary>
    [Description("The private key from admin.recaptcha.net. Can also be set using RecaptchaPrivateKey in AppSettings.")]
    [Category("Settings")]
    public string PrivateKey { get; set; }

    /// <summary>
    ///   Gets or sets PublicKey.
    /// </summary>
    [Category("Settings")]
    [Description("The public key from admin.recaptcha.net. Can also be set using RecaptchaPublicKey in AppSettings.")]
    public string PublicKey { get; set; }

    /// <summary>
    ///   Gets or sets a value indicating whether SkipRecaptcha.
    /// </summary>
    [Description(
      "Set this to true to stop reCAPTCHA validation. Useful for testing platform. Can also be set using RecaptchaSkipValidation in AppSettings."
      )]
    [DefaultValue(false)]
    [Category("Settings")]
    public bool SkipRecaptcha
    {
      get
      {
        return this.skipRecaptcha;
      }

      set
      {
        this.skipRecaptcha = value;
      }
    }

    /// <summary>
    ///   Gets or sets Theme.
    /// </summary>
    [DefaultValue("red")]
    [Description("The theme for the reCAPTCHA control. Currently supported values are red, blackglass, white, and clean"
      )]
    [Category("Appearence")]
    public string Theme
    {
      get
      {
        return this.theme;
      }

      set
      {
        this.theme = value;
      }
    }

    #endregion

    #region Implemented Interfaces

    #region IValidator

    /// <summary>
    /// The validate.
    /// </summary>
    public void Validate()
    {
      if (this.skipRecaptcha)
      {
        this.recaptchaResponse = RecaptchaResponse.Valid;
      }

      if (((this.recaptchaResponse == null) && this.Visible) && this.Enabled)
      {
        var validator = new RecaptchaValidator();
        validator.PrivateKey = this.PrivateKey;
        validator.RemoteIP = this.Page.Request.GetUserRealIPAddress();
        validator.Challenge = this.Context.Request.Form["recaptcha_challenge_field"];
        validator.Response = this.Context.Request.Form["recaptcha_response_field"];
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
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    /// <exception cref="ApplicationException">
    /// </exception>
    protected override void OnInit([NotNull] EventArgs e)
    {
      base.OnInit(e);

      if (string.IsNullOrEmpty(this.PublicKey) || string.IsNullOrEmpty(this.PrivateKey))
      {
        throw new ApplicationException("reCAPTCHA needs to be configured with a public & private key.");
      }

      if (this.allowMultipleInstances || !this.CheckIfRecaptchaExists())
      {
        this.Page.Validators.Add(this);
      }
    }

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render([NotNull] HtmlTextWriter writer)
    {
      if (this.skipRecaptcha)
      {
        writer.WriteLine("reCAPTCHA validation is skipped. Set SkipRecaptcha property to false to enable validation.");
      }
      else
      {
        this.RenderContents(writer);
      }
    }

    /// <summary>
    /// The render contents.
    /// </summary>
    /// <param name="output">
    /// The output.
    /// </param>
    protected override void RenderContents([NotNull] HtmlTextWriter output)
    {
      output.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
      output.RenderBeginTag(HtmlTextWriterTag.Script);
      output.Indent++;
      output.WriteLine("var RecaptchaOptions = {");
      output.Indent++;
      output.WriteLine("theme : '{0}',", this.theme ?? string.Empty);
      if (this.customThemeWidget != null)
      {
        output.WriteLine("custom_theme_widget : '{0}',", this.customThemeWidget);
      }

      output.WriteLine("tabindex : {0}", this.TabIndex);
      output.Indent--;
      output.WriteLine("};");
      output.Indent--;
      output.RenderEndTag();
      output.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
      output.AddAttribute(HtmlTextWriterAttribute.Src, this.GenerateChallengeUrl(false), false);
      output.RenderBeginTag(HtmlTextWriterTag.Script);
      output.RenderEndTag();
      output.RenderBeginTag(HtmlTextWriterTag.Noscript);
      output.Indent++;
      output.AddAttribute(HtmlTextWriterAttribute.Src, this.GenerateChallengeUrl(true), false);
      output.AddAttribute(HtmlTextWriterAttribute.Width, "500");
      output.AddAttribute(HtmlTextWriterAttribute.Height, "300");
      output.AddAttribute("frameborder", "0");
      output.RenderBeginTag(HtmlTextWriterTag.Iframe);
      output.RenderEndTag();
      output.WriteBreak();
      output.AddAttribute(HtmlTextWriterAttribute.Name, "recaptcha_challenge_field");
      output.AddAttribute(HtmlTextWriterAttribute.Rows, "3");
      output.AddAttribute(HtmlTextWriterAttribute.Cols, "40");
      output.RenderBeginTag(HtmlTextWriterTag.Textarea);
      output.RenderEndTag();
      output.AddAttribute(HtmlTextWriterAttribute.Name, "recaptcha_response_field");
      output.AddAttribute(HtmlTextWriterAttribute.Value, "manual_challenge");
      output.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
      output.RenderBeginTag(HtmlTextWriterTag.Input);
      output.RenderEndTag();
      output.Indent--;
      output.RenderEndTag();
    }

    /// <summary>
    /// The check if recaptcha exists.
    /// </summary>
    /// <returns>
    /// The check if recaptcha exists.
    /// </returns>
    private bool CheckIfRecaptchaExists()
    {
      foreach (object obj2 in this.Page.Validators)
      {
        if (obj2 is RecaptchaControl)
        {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// The generate challenge url.
    /// </summary>
    /// <param name="noScript">
    /// The no script.
    /// </param>
    /// <returns>
    /// The generate challenge url.
    /// </returns>
    [NotNull]
    private string GenerateChallengeUrl(bool noScript)
    {
      var builder = new StringBuilder();
      builder.Append(
        (this.Context.Request.IsSecureConnection || this.overrideSecureMode)
          ? "https://api-secure.recaptcha.net"
          : "http://api.recaptcha.net");
      builder.Append(noScript ? "/noscript?" : "/challenge?");
      builder.AppendFormat("k={0}", this.PublicKey);
      if ((this.recaptchaResponse != null) && (this.recaptchaResponse.ErrorCode != string.Empty))
      {
        builder.AppendFormat("&error={0}", this.recaptchaResponse.ErrorCode);
      }

      return builder.ToString();
    }

    #endregion
  }
}
