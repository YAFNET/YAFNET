/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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
namespace YAF.Core.Services;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;

using YAF.Configuration;
using YAF.Core.Context;
using YAF.Core.Extensions;
using YAF.Types;
using YAF.Types.Extensions;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;

/// <summary>
///     The YAF template email.
/// </summary>
public class TemplateEmail : IHaveServiceLocator
{
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateEmail"/> class.
    /// </summary>
    /// <param name="templateName">
    /// The template name.
    /// </param>
    public TemplateEmail([CanBeNull]string templateName)
    {
        this.HtmlTemplateFileName = "EmailTemplate.html";

        this.TemplateName = templateName;

        var logoUrl =
            $"{BoardInfo.ForumClientFileRoot}{this.Get<BoardFolders>().Logos}/{this.Get<BoardSettings>().ForumLogo}";

        var inlineCss = File.ReadAllText(
            this.Get<HttpContextBase>().Server
                .MapPath(this.Get<ITheme>().BuildThemePath("bootstrap-forum.min.css")));

        this.TemplateParams["{forumname}"] = this.Get<BoardSettings>().Name;
        this.TemplateParams["{forumlink}"] = this.Get<LinkBuilder>().ForumUrl;
        this.TemplateParams["{css}"] = inlineCss;
        this.TemplateParams["{logo}"] = $"{this.Get<BoardSettings>().BaseUrlMask}{logoUrl}";
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///     Gets the service locator.
    /// </summary>
    public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

    /// <summary>
    ///     Gets or sets TemplateLanguageFile.
    /// </summary>
    public string TemplateLanguageFile { get; set; }

    /// <summary>
    ///     Gets or sets TemplateName.
    /// </summary>
    public string TemplateName { get; set; }

    /// <summary>
    /// Gets or sets the html template file name.
    /// </summary>
    public string HtmlTemplateFileName { get; set; }

    /// <summary>
    ///     Gets or sets Template Parameter
    /// </summary>
    public IDictionary<string, string> TemplateParams { get; set; } = new Dictionary<string, string>();

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// The send email.
    /// </summary>
    /// <param name="toAddress">
    /// The to address.
    /// </param>
    /// <param name="subject">
    /// The subject.
    /// </param>
    public void SendEmail(MailAddress toAddress, string subject)
    {
        this.SendEmail(
            new MailAddress(
                this.Get<BoardSettings>().ForumEmail,
                this.Get<BoardSettings>().Name),
            toAddress,
            subject);
    }

    /// <summary>
    /// The send email.
    /// </summary>
    /// <param name="fromAddress">
    /// The from address.
    /// </param>
    /// <param name="toAddress">
    /// The to address.
    /// </param>
    /// <param name="subject">
    /// The subject.
    /// </param>
    public void SendEmail(MailAddress fromAddress, MailAddress toAddress, string subject)
    {
        var textBody = this.ProcessTemplate($"{this.TemplateName}_TEXT").Trim();

        // just send directly
        this.Get<IMailService>().Send(
            fromAddress,
            toAddress,
            fromAddress,
            subject,
            textBody,
            this.ProcessHtml(textBody));
    }

    /// <summary>
    /// Create Email.
    /// </summary>
    /// <param name="fromAddress">
    /// The from address.
    /// </param>
    /// <param name="toAddress">
    /// The to address.
    /// </param>
    /// <param name="subject">
    /// The subject.
    /// </param>
    /// <returns>
    /// The <see cref="MailMessage"/>.
    /// </returns>
    public MailMessage CreateEmail(MailAddress fromAddress, MailAddress toAddress, string subject)
    {
        var textBody = this.ProcessTemplate($"{this.TemplateName}_TEXT").Trim();

        // Create Mail Message
        return this.Get<IMailService>().CreateMessage(
            fromAddress,
            toAddress,
            fromAddress,
            subject,
            textBody,
            this.ProcessHtml(textBody));
    }

    #endregion

    #region Methods

    /// <summary>
    /// Creates an email from a template
    /// </summary>
    /// <param name="templateName">
    /// The template Name.
    /// </param>
    /// <returns>
    /// The process template.
    /// </returns>
    public string ProcessTemplate([CanBeNull]string templateName)
    {
        var email = this.ReadTemplate(templateName, this.TemplateLanguageFile);

        if (email.IsSet())
        {
            email = this.TemplateParams.Keys.Aggregate(
                email,
                (current, key) => this.TemplateParams[key].IsSet()
                                      ? current.Replace(key, this.TemplateParams[key])
                                      : current.Replace(key, string.Empty));
        }

        return email;
    }

    /// <summary>
    /// Load Email Template and inject content
    /// </summary>
    /// <param name="textBody">
    /// The text body.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    private string ProcessHtml(string textBody)
    {
        var htmlTemplate = File.ReadAllText(this.Get<HttpContextBase>().Server.MapPath(
            $"{BoardInfo.ForumServerFileRoot}Resources/{this.HtmlTemplateFileName}"));

        var formattedBody = this.Get<IBBCode>().MakeHtml(textBody, true, true);

        var html = this.TemplateParams.Keys.Aggregate(
            htmlTemplate,
            (current, key) => current.Replace(key, this.TemplateParams[key]));

        return html.Replace("{CONTENT}", formattedBody);
    }

    /// <summary>
    /// Reads a template from the language file
    /// </summary>
    /// <param name="templateName">
    /// The template Name.
    /// </param>
    /// <param name="templateLanguageFile">
    /// The template Language File.
    /// </param>
    /// <returns>
    /// The template
    /// </returns>
    private string ReadTemplate(string templateName, string templateLanguageFile)
    {
        if (templateName.IsNotSet())
        {
            return null;
        }

        if (templateLanguageFile.IsNotSet())
        {
            return this.Get<ILocalization>().GetText("TEMPLATES", templateName);
        }

        var localization = new Localization.Localization();

        localization.LoadTranslation(templateLanguageFile);

        return localization.GetText("TEMPLATES", templateName, templateLanguageFile);
    }

    #endregion
}