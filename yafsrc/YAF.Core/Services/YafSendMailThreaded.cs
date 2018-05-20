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

namespace YAF.Core.Services
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;
    using System.Threading;

    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    #endregion

    /// <summary>
    ///     Separate class since SendThreaded isn't needed functionality for any instance except the <see cref="HttpModule" />
    ///     instance.
    /// </summary>
    public class YafSendMailThreaded : ISendMailThreaded, IHaveServiceLocator
    {
        #region Static Fields

        /// <summary>
        /// The _random
        /// </summary>
        private static readonly Random _Random = new Random();

        #endregion

        #region Fields

        /// <summary>
        /// The _unique identifier
        /// </summary>
        private readonly Lazy<int> _uniqueId = new Lazy<int>(() => _Random.Next());

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="YafSendMailThreaded" /> class.
        /// </summary>
        /// <param name="sendMail">The send mail.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="serviceLocator">The service locator.</param>
        public YafSendMailThreaded([NotNull] ISendMail sendMail, ILogger logger, IServiceLocator serviceLocator)
        {
            this.SendMail = sendMail;
            this.Logger = logger;
            this.ServiceLocator = serviceLocator;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets Logger.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Gets or sets the ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; set; }

        /// <summary>
        ///     Gets the mail repository.
        /// </summary>
        public IRepository<Mail> MailRepository
        {
            get
            {
                return this.GetRepository<Mail>();
            }
        }

        /// <summary>
        ///     Gets or sets SendMail.
        /// </summary>
        public ISendMail SendMail { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the unique process identifier.
        /// </summary>
        /// <value>
        /// The unique process identifier.
        /// </value>
        protected int UniqueProcessId
        {
            get
            {
                return this._uniqueId.Value ^ Thread.CurrentThread.ManagedThreadId;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The send threaded.
        /// </summary>
        public void SendThreaded()
        {
            var mailMessages = new Dictionary<MailMessage, Mail>();

            try
            {
                IEnumerable<Mail> mailList = this.GetMailListSafe().ToList();

                foreach (var n in mailList)
                {
                    this.Logger.Debug("Process Id {0} to User {1}", this.UniqueProcessId, n.ToUser);    
                }

                this.ConstructMessageList(mailMessages, mailList);

                this.SendMail.SendAll(
                    mailMessages.Select(x => x.Key),
                    (message, ex) =>
                    {
                        if (ex is FormatException)
                        {
#if (DEBUG) // email address is no good -- delete this email...
                            this.Logger.Debug(
                                "Invalid Email Address: {0}, Exception: {1}",
                                mailMessages[message].ToUser,
                                ex.ToString());
#else

                            // email address is no good -- delete this email...
                            this.Logger.Log(
                                null,
                                this,
                                "Invalid Email Address: {0}, Exception: {1}".FormatWith(
                                    mailMessages[message].ToUser,
                                    ex.ToString()),
                                EventLogTypes.Warning);
#endif
                        }
                        else if (ex is SmtpException)
                        {
#if (DEBUG)
                            this.Logger.Debug("SendMailThread SmtpException: {0}", ex.ToString());
#else
                                this.Logger.Log(
                                    null,
                                    this,
                                    "SendMailThread failed with an SmtpException (Email to: {1}, Subject: {2}): {0}"
                                        .FormatWith(
                                            ex.ToString(),
                                            mailMessages[message].ToUser,
                                            mailMessages[message].Subject),
                                    EventLogTypes.Warning);
#endif
                            if (mailMessages.ContainsKey(message) && mailMessages[message].SendTries < 2)
                            {
                                // update messsage so it will be deleted on the second run
                                this.MailRepository.Create(
                                    mailMessages[message].FromUser,
                                    mailMessages[message].FromUserName,
                                    mailMessages[message].ToUser,
                                    mailMessages[message].ToUserName,
                                    mailMessages[message].Subject,
                                    mailMessages[message].Body,
                                    mailMessages[message].BodyHtml,
                                    mailMessages[message].SendTries + 1,
                                    DateTime.UtcNow);

                                mailMessages.Remove(message);
                            }
                            else
                            {
                                this.Logger.Log(
                                    null,
                                    this,
                                    "SendMailThread Failed for the 2nd time (the email will now deleted) with an SmtpException (Email to: {1}, Subject: {2}): {0}"
                                        .FormatWith(
                                            ex.ToString(),
                                            mailMessages[message].ToUser,
                                            mailMessages[message].Subject),
                                    EventLogTypes.Warning);
                            }
                        }
                        else
                        {
#if (DEBUG) // general exception...
                            this.Logger.Debug("SendMailThread General Exception: {0}", ex.ToString());
#else

                            // general exception...
                            this.Logger.Log(
                                null,
                                this,
                                "Exception Thrown in SendMail Thread: {0}".FormatWith(ex.ToString()),
                                EventLogTypes.Warning);
#endif
                        }
                    });
            }
            catch (Exception ex)
            {
#if (DEBUG) // general exception...
                this.Logger.Debug("SendMailThread General Exception: {0}", ex.ToString());
#else

    // general exception...
                this.Logger.Log(
                    null,
                    this,
                    "Exception Thrown in SendMail Thread: {0}".FormatWith(ex.ToString()),
                    EventLogTypes.Warning);
#endif
            }

            this.Logger.Debug("SendMailThread exiting");
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The construct message list.
        /// </summary>
        /// <param name="mailMessages">
        ///     The mail messages.
        /// </param>
        /// <param name="mailList">
        ///     The mail list.
        /// </param>
        private void ConstructMessageList(IDictionary<MailMessage, Mail> mailMessages, IEnumerable<Mail> mailList)
        {
            // construct mail message list...
            foreach (var mail in mailList.Where(x => x.ID > 0))
            {
                // Build a MailMessage
                if (mail.FromUser.IsNotSet() || mail.ToUser.IsNotSet())
                {
                    this.MailRepository.Delete(mail);
                    continue;
                }

                try
                {
                    var toEmailAddress = mail.ToUserName.IsSet()
                                                     ? new MailAddress(mail.ToUser, mail.ToUserName)
                                                     : new MailAddress(mail.ToUser);
                    var fromEmailAddress = mail.FromUserName.IsSet()
                                                       ? new MailAddress(mail.FromUser, mail.FromUserName)
                                                       : new MailAddress(mail.FromUser);

                    var newMessage = new MailMessage();

                    mailMessages.Add(newMessage, mail);

                    newMessage.Populate(
                        fromEmailAddress,
                        toEmailAddress,
                        fromEmailAddress,
                        mail.Subject,
                        mail.Body,
                        mail.BodyHtml);
                }
                catch (FormatException ex)
                {
                    // incorrect email format -- delete this message immediately
                    this.MailRepository.Delete(mail);
#if (DEBUG)
                    this.Logger.Debug("Invalid Email Address: {0}".FormatWith(ex.ToString()), ex.ToString());
#else
                    this.Logger.Log(
                        null,
                        this,
                        "Invalid Email Address: {0}".FormatWith(ex.ToString()),
                        EventLogTypes.Warning);
#endif
                }
            }
        }

        /// <summary>
        ///     Gets the mail list safe.
        /// </summary>
        /// <returns>
        ///     The get mail list safe.
        /// </returns>
        private IEnumerable<Mail> GetMailListSafe()
        {
            IList<Mail> mailList;

            Thread.BeginCriticalRegion();
            try
            {
                mailList = this.MailRepository.List(this.UniqueProcessId);
            }
            finally
            {
                Thread.EndCriticalRegion();
            }

            this.Logger.Debug("Retreived {0} Queued Messages Process Id ({1})...".FormatWith(mailList.Count, this.UniqueProcessId));

            return mailList;
        }

        #endregion
    }
}