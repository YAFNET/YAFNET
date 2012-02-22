/* Yet Another Forum.NET
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
	#region Using

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net.Mail;
	using System.Threading;

	using YAF.Classes.Data;
	using YAF.Types;
	using YAF.Types.Interfaces;
	using YAF.Types.Objects;
	using YAF.Utils;

	#endregion

	/// <summary>
	/// Separate class since SendThreaded isn't needed functionality for any instance except the <see cref="HttpModule"/> instance.
	/// </summary>
	public class YafSendMailThreaded : ISendMailThreaded
	{
		#region Constants and Fields

		/// <summary>
		///   The _unique id.
		/// </summary>
		protected int _uniqueId;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="YafSendMailThreaded"/> class.
		/// </summary>
		/// <param name="sendMail">
		/// The send mail. 
		/// </param>
		/// <param name="logger">
		/// The logger.
		/// </param>
		public YafSendMailThreaded([NotNull] ISendMail sendMail, ILogger logger)
		{
			CodeContracts.ArgumentNotNull(sendMail, "sendMail");

			this.SendMail = sendMail;
			this.Logger = logger;
			var rand = new Random();
			this._uniqueId = rand.Next();
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets Logger.
		/// </summary>
		public ILogger Logger { get; set; }

		/// <summary>
		///   Gets or sets SendMail.
		/// </summary>
		public ISendMail SendMail { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// The send threaded.
		/// </summary>
		public void SendThreaded()
		{
			var mailMessages = new Dictionary<MailMessage, TypedMailList>();

			try
			{
				IList<TypedMailList> mailList = this.GetMailListSafe();

				this.ConstructMessageList(mailMessages, mailList);

				this.SendMail.SendAllIsolated(
					mailMessages.Select(x => x.Key), 
					(message, ex) =>
						{
							if (ex is FormatException)
							{
								// email address is no good -- delete this email...
								this.Logger.Debug("Invalid Email Address: {0}".FormatWith(ex.ToString()));
#if (DEBUG)
								this.Logger.Warn("Invalid Email Address: {0}".FormatWith(ex.ToString()), ex.ToString());
#endif
							}
							else if (ex is SmtpException)
							{
#if (DEBUG)
								this.Logger.Warn("SendMailThread SmtpException", ex.ToString());
#endif
								this.Logger.Debug("Send Exception: {0}".FormatWith(ex.ToString()));

								if (mailMessages.ContainsKey(message) && mailMessages[message].SendTries < 2)
								{
									// remove from the collection so it doesn't get deleted...
									mailMessages.Remove(message);
								}
								else
								{
									this.Logger.Warn("SendMailThread Failed for the 2nd time:", ex.ToString());
								}
							}
							else
							{
								// general exception...
								this.Logger.Debug("Exception Thrown in SendMail Thread: " + ex.ToString());
#if (DEBUG)
								this.Logger.Warn("SendMailThread General Exception", ex.ToString());
#endif
							}
						});

				foreach (var message in mailMessages.Values)
				{
					// all is well, delete this message...
					this.Logger.Debug("Deleting email to {0} (ID: {1})".FormatWith(message.ToUser, message.MailID));
					LegacyDb.mail_delete(message.MailID);
				}
			}
			catch (Exception ex)
			{
				// general exception...
				this.Logger.Debug("Exception Thrown in SendMail Thread: " + ex);
#if (DEBUG)
				this.Logger.Warn("SendMailThread General Exception", ex.ToString());
#endif
			}
			finally
			{
				if (mailMessages.Any())
				{
					// dispose of all mail messages
					mailMessages.Where(x => x.Key != null).Select(m => m.Key).ToList().ForEach(m => m.Dispose());
				}
			}

			this.Logger.Debug("SendMailThread exiting");
		}

		#endregion

		#region Methods

		/// <summary>
		/// The construct message list.
		/// </summary>
		/// <param name="mailMessages">
		/// The mail messages.
		/// </param>
		/// <param name="mailList">
		/// The mail list.
		/// </param>
		private void ConstructMessageList(IDictionary<MailMessage, TypedMailList> mailMessages, IEnumerable<TypedMailList> mailList)
		{
			// construct mail message list...
			foreach (var mail in mailList.Where(x => x.MailID.HasValue))
			{
				// Build a MailMessage
				if (mail.FromUser.IsNotSet() || mail.ToUser.IsNotSet())
				{
					LegacyDb.mail_delete(mail.MailID.Value);
					continue;
				}

				try
				{
					MailAddress toEmailAddress = mail.ToUserName.IsSet()
					                             	? new MailAddress(mail.ToUser, mail.ToUserName)
					                             	: new MailAddress(mail.ToUser);
					MailAddress fromEmailAddress = mail.FromUserName.IsSet()
					                               	? new MailAddress(mail.FromUser, mail.FromUserName)
					                               	: new MailAddress(mail.FromUser);

					var newMessage = new MailMessage();
					mailMessages.Add(newMessage, mail);
					newMessage.Populate(fromEmailAddress, toEmailAddress, mail.Subject, mail.Body, mail.BodyHtml);
				}
				catch (FormatException ex)
				{
					// incorrect email format -- delete this message immediately
					LegacyDb.mail_delete(mail.MailID);

					this.Logger.Debug("Invalid Email Address: {0}".FormatWith(ex.ToString()));
#if (DEBUG)
					this.Logger.Warn("Invalid Email Address: {0}".FormatWith(ex.ToString()), ex.ToString());
#endif
				}
			}
		}

		/// <summary>
		/// The get mail list safe.
		/// </summary>
		/// <returns>
		/// </returns>
		private IList<TypedMailList> GetMailListSafe()
		{
			List<TypedMailList> mailList;

			try
			{
				this.Logger.Debug("Retrieving queued mail...");
				Thread.BeginCriticalRegion();

				mailList = LegacyDb.MailList(this._uniqueId).ToList();

				this.Logger.Debug("Got {0} Messages...".FormatWith(mailList.Count()));
			}
			finally
			{
				Thread.EndCriticalRegion();
			}

			return mailList;
		}

		#endregion
	}
}