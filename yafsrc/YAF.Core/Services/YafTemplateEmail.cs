/* Yet Another Forum.net
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Core.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;

    using YAF.Classes;
    using YAF.Core.Model;
    using YAF.Core.Services.Localization;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;

    /// <summary>
    ///     The yaf template email.
    /// </summary>
    public class YafTemplateEmail : IHaveServiceLocator
    {
        #region Fields

        /// <summary>
        /// The _html enabled.
        /// </summary>
        private bool _htmlEnabled = true;

        /// <summary>
        ///     The _template params.
        /// </summary>
        private IDictionary<string, string> _templateParams = new Dictionary<string, string>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="YafTemplateEmail" /> class.
        /// </summary>
        public YafTemplateEmail()
            : this(null, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="YafTemplateEmail"/> class.
        /// </summary>
        /// <param name="templateName">
        /// The template name.
        /// </param>
        public YafTemplateEmail(string templateName)
            : this(templateName, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="YafTemplateEmail"/> class.
        /// </summary>
        /// <param name="templateName">
        /// The template name.
        /// </param>
        /// <param name="htmlEnabled">
        /// The html enabled.
        /// </param>
        public YafTemplateEmail(string templateName, bool htmlEnabled)
        {
            this.TemplateName = templateName;
            this.HtmlEnabled = htmlEnabled;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether HtmlEnabled.
        /// </summary>
        public bool HtmlEnabled
        {
            get
            {
                return this._htmlEnabled;
            }

            set
            {
                this._htmlEnabled = value;
            }
        }

        /// <summary>
        ///     Gets the service locator.
        /// </summary>
        public IServiceLocator ServiceLocator
        {
            get
            {
                return YafContext.Current.ServiceLocator;
            }
        }

        /// <summary>
        ///     Gets or sets TemplateLanguageFile.
        /// </summary>
        public string TemplateLanguageFile { get; set; }

        /// <summary>
        ///     Gets or sets TemplateName.
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        ///     Gets or sets TemplateParams.
        /// </summary>
        public IDictionary<string, string> TemplateParams
        {
            get
            {
                return this._templateParams;
            }

            set
            {
                this._templateParams = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The create watch.
        /// </summary>
        /// <param name="topicID">
        /// The topic id.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="fromAddress">
        /// The from address.
        /// </param>
        /// <param name="subject">
        /// The subject.
        /// </param>
        public void CreateWatch(int topicID, int userId, MailAddress fromAddress, string subject)
        {
            string textBody = null, htmlBody = null;

            textBody = this.ProcessTemplate(this.TemplateName + "_TEXT").Trim();
            htmlBody = this.ProcessTemplate(this.TemplateName + "_HTML").Trim();

            // null out html if it's not desired
            if (!this.HtmlEnabled || htmlBody.IsNotSet())
            {
                htmlBody = null;
            }

            this.GetRepository<Mail>().CreateWatch(topicID, fromAddress.Address, fromAddress.DisplayName, subject, textBody, htmlBody, userId);
        }

        /// <summary>
        /// Creates an email from a template
        /// </summary>
        /// <param name="templateName">
        /// The template Name.
        /// </param>
        /// <returns>
        /// The process template.
        /// </returns>
        public string ProcessTemplate(string templateName)
        {
            string email = this.ReadTemplate(templateName, this.TemplateLanguageFile);

            if (email.IsSet())
            {
                email = this._templateParams.Keys.Aggregate(email, (current, key) => current.Replace(key, this._templateParams[key]));
            }

            return email;
        }

        /// <summary>
        /// The send email.
        /// </summary>
        /// <param name="toAddress">
        /// The to address.
        /// </param>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="useSendThread">
        /// The use send thread.
        /// </param>
        public void SendEmail(MailAddress toAddress, string subject, bool useSendThread)
        {
            this.SendEmail(
                new MailAddress(this.Get<YafBoardSettings>().ForumEmail, this.Get<YafBoardSettings>().Name), toAddress, subject, useSendThread);
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
        /// <param name="useSendThread">
        /// The use send thread.
        /// </param>
        public void SendEmail(MailAddress fromAddress, MailAddress toAddress, string subject, bool useSendThread)
        {
            string textBody = null, htmlBody = null;

            textBody = this.ProcessTemplate(this.TemplateName + "_TEXT").Trim();
            htmlBody = this.ProcessTemplate(this.TemplateName + "_HTML").Trim();

            // null out html if it's not desired
            if (!this.HtmlEnabled || htmlBody.IsNotSet())
            {
                htmlBody = null;
            }

            if (useSendThread)
            {
                // create this email in the send mail table...
                this.GetRepository<Mail>()
                    .Create(fromAddress.Address, fromAddress.DisplayName, toAddress.Address, toAddress.DisplayName, subject, textBody, htmlBody);
            }
            else
            {
                // just send directly
                this.Get<ISendMail>().Send(fromAddress, toAddress, subject, textBody, htmlBody);
            }
        }

        #endregion

        #region Methods

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
            if (!templateName.IsSet())
            {
                return null;
            }

            if (templateLanguageFile.IsSet())
            {
                var localization = new YafLocalization();
                localization.LoadTranslation(templateLanguageFile);
                return localization.GetText("TEMPLATES", templateName);
            }

            return this.Get<ILocalization>().GetText("TEMPLATES", templateName);
        }

        #endregion
    }
}