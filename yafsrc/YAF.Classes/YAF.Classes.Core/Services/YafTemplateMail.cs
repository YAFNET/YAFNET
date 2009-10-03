/* Yet Another Forum.net
 * Copyright (C) 2006-2009 Jaben Cargman
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
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Net.Mail;
using System.Web;
using System.Data;
using System.Web.Security;
using YAF.Classes.Data;
using YAF.Classes.Utils;

namespace YAF.Classes.Core
{
	public class YafTemplateEmail
	{
		#region Properties

		private string _templateName;

		public string TemplateName
		{
			get { return _templateName; }
			set { _templateName = value; }
		}

		private string _templateLanguageFile;
		public string TemplateLanguageFile
		{
			get
			{
				return _templateLanguageFile;
			}
			set
			{
				_templateLanguageFile = value;
			}
		}

		private bool _htmlEnabled;

		public bool HtmlEnabled
		{
			get { return _htmlEnabled; }
			set { _htmlEnabled = value; }
		}

		private StringDictionary _templateParams = new StringDictionary();
		public StringDictionary TemplateParams
		{
			get
			{
				return _templateParams;
			}
			set
			{
				_templateParams = value;
			}
		}

		#endregion

		public YafTemplateEmail()
			: this( null, true )
		{

		}
		public YafTemplateEmail( string templateName )
			: this( templateName, true )
		{

		}
		public YafTemplateEmail( string templateName, bool htmlEnabled )
		{
			_templateName = templateName;
			_htmlEnabled = htmlEnabled;
		}

		/// <summary>
		/// Reads a template from the language file
		/// </summary>
		/// <returns>The template</returns>
		private string ReadTemplate( string templateName, string templateLanguageFile )
		{
			if ( !String.IsNullOrEmpty( templateName ) )
			{
				if ( !String.IsNullOrEmpty( templateLanguageFile ) )
				{
					YafLocalization localization = new YafLocalization();
					localization.LoadTranslation( templateLanguageFile );
					return localization.GetText( "TEMPLATES", templateName );
				}

				return YafContext.Current.Localization.GetText( "TEMPLATES", templateName );
			}

			return null;
		}

		/// <summary>
		/// Creates an email from a template
		/// </summary>
		/// <returns></returns>
		public string ProcessTemplate( string templateName )
		{
			string email = ReadTemplate( templateName, TemplateLanguageFile );

			if ( !String.IsNullOrEmpty( email ) )
			{
				foreach ( string key in _templateParams.Keys )
				{
					email = email.Replace( key, _templateParams[key] );
				}
			}

			return email;
		}

		public void SendEmail( MailAddress toAddress, string subject, bool useSendThread )
		{
			this.SendEmail( new MailAddress( YafContext.Current.BoardSettings.ForumEmail, YafContext.Current.BoardSettings.Name ), toAddress, subject, useSendThread );
		}

		public void SendEmail( MailAddress fromAddress, MailAddress toAddress, string subject, bool useSendThread )
		{
			string textBody = null, htmlBody = null;

			textBody = ProcessTemplate( TemplateName + "_TEXT" ).Trim();
			htmlBody = ProcessTemplate( TemplateName + "_HTML" ).Trim();

			// null out html if it's not desired
			if ( !HtmlEnabled || String.IsNullOrEmpty( htmlBody ) ) htmlBody = null;

			if ( useSendThread )
			{
				// create this email in the send mail table...
				Data.DB.mail_create( fromAddress.Address, fromAddress.DisplayName, toAddress.Address, toAddress.DisplayName, subject, textBody, htmlBody );
			}
			else
			{
				// just send directly
				YafServices.SendMail.Send( fromAddress, toAddress, subject, textBody, htmlBody );
			}
		}

		public void CreateWatch( int topicID, int userId, MailAddress fromAddress, string subject )
		{
			string textBody = null, htmlBody = null;

			textBody = ProcessTemplate( TemplateName + "_TEXT" ).Trim();
			htmlBody = ProcessTemplate( TemplateName + "_HTML" ).Trim();

			// null out html if it's not desired
			if ( !HtmlEnabled || String.IsNullOrEmpty( htmlBody ) ) htmlBody = null;

			DB.mail_createwatch( topicID, fromAddress.Address, fromAddress.DisplayName, subject, textBody, htmlBody, userId );
		}
	}
}
