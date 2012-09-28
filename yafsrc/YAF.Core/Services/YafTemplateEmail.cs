/* Yet Another Forum.net
 * Copyright (C) 2006-2012 Jaben Cargman
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
    using System.Collections.Specialized;
    using System.Net.Mail;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    /// <summary>
  /// The yaf template email.
  /// </summary>
  public class YafTemplateEmail
  {
    #region Properties

    /// <summary>
    /// The _html enabled.
    /// </summary>
    private bool _htmlEnabled;

    /// <summary>
    /// The _template language file.
    /// </summary>
    private string _templateLanguageFile;

    /// <summary>
    /// The _template name.
    /// </summary>
    private string _templateName;

    /// <summary>
    /// The _template params.
    /// </summary>
    private StringDictionary _templateParams = new StringDictionary();

    /// <summary>
    /// Gets or sets TemplateName.
    /// </summary>
    public string TemplateName
    {
      get
      {
        return this._templateName;
      }

      set
      {
        this._templateName = value;
      }
    }

    /// <summary>
    /// Gets or sets TemplateLanguageFile.
    /// </summary>
    public string TemplateLanguageFile
    {
      get
      {
        return this._templateLanguageFile;
      }

      set
      {
        this._templateLanguageFile = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether HtmlEnabled.
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
    /// Gets or sets TemplateParams.
    /// </summary>
    public StringDictionary TemplateParams
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

    /// <summary>
    /// Initializes a new instance of the <see cref="YafTemplateEmail"/> class.
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
      this._templateName = templateName;
      this._htmlEnabled = htmlEnabled;
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
      if (templateName.IsSet())
      {
        if (templateLanguageFile.IsSet())
        {
          var localization = new YafLocalization();
          localization.LoadTranslation(templateLanguageFile);
          return localization.GetText("TEMPLATES", templateName);
        }

        return YafContext.Current.Get<ILocalization>().GetText("TEMPLATES", templateName);
      }

      return null;
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
      string email = ReadTemplate(templateName, TemplateLanguageFile);

      if (email.IsSet())
      {
        foreach (string key in this._templateParams.Keys)
        {
          email = email.Replace(key, this._templateParams[key]);
        }
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
      SendEmail(new MailAddress(YafContext.Current.BoardSettings.ForumEmail, YafContext.Current.BoardSettings.Name), toAddress, subject, useSendThread);
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

      textBody = ProcessTemplate(TemplateName + "_TEXT").Trim();
      htmlBody = ProcessTemplate(TemplateName + "_HTML").Trim();

      // null out html if it's not desired
      if (!HtmlEnabled || htmlBody.IsNotSet())
      {
        htmlBody = null;
      }

      if (useSendThread)
      {
        // create this email in the send mail table...
        LegacyDb.mail_create(fromAddress.Address, fromAddress.DisplayName, toAddress.Address, toAddress.DisplayName, subject, textBody, htmlBody);
      }
      else
      {
        // just send directly
        YafContext.Current.Get<YafSendMail>().Send(fromAddress, toAddress, subject, textBody, htmlBody);
      }
    }

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

      textBody = ProcessTemplate(TemplateName + "_TEXT").Trim();
      htmlBody = ProcessTemplate(TemplateName + "_HTML").Trim();

      // null out html if it's not desired
      if (!HtmlEnabled || htmlBody.IsNotSet())
      {
        htmlBody = null;
      }

      LegacyDb.mail_createwatch(topicID, fromAddress.Address, fromAddress.DisplayName, subject, textBody, htmlBody, userId);
    }
  }
}