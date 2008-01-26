/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2008 Jaben Cargman
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
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using YAF.Classes.Utils;
using System.Configuration.Provider;

namespace YAF.Providers.Utils
{
	public static class ExceptionReporter
	{
		/// <summary>
		/// Get Exception XML File Name from AppSettings
		/// </summary>
		private static string XMLFile()
		{
			string temp = ConfigurationManager.AppSettings ["providerExceptionXML"];
			if ( String.IsNullOrEmpty( temp ) )
				return "ProviderExceptions.xml";
			else
				return temp;
		}

		/// <summary>
		/// Return XMLDocument containing text for the Exceptions
		/// </summary>
		private static XmlDocument ExceptionXML()
		{
			string exceptionFile = XMLFile();
			if ( String.IsNullOrEmpty( exceptionFile ) )
				throw new ApplicationException( "Exceptionfile cannot be null or empty!" );
			XmlDocument exceptionXmlDoc = new XmlDocument();
			exceptionXmlDoc.Load( System.Web.HttpContext.Current.Server.MapPath( String.Format( "{0}resources/{1}", YafForumInfo.ForumRoot, exceptionFile ) ) );
			return exceptionXmlDoc;
		}

		/// <summary>
		/// Get Exception String
		/// </summary>
		public static string GetReport( string providerSection, string tag )
		{
			string select = string.Format( "//provider[@name='{0}']/Resource[@tag='{1}']", providerSection.ToUpper(), tag.ToUpper() );
			XmlNode node = ExceptionXML().SelectSingleNode( select );

			if ( node != null )
				return node.InnerText;
			else
				return String.Format( "Exception({1}:{0}) cannot be found in Exception file!", tag, providerSection );

		}

		/// <summary>
		/// Throw Exception
		/// </summary>
		public static string Throw( string providerSection, string tag )
		{
			throw new ApplicationException( GetReport( providerSection, tag ) );
		}

		/// <summary>
		/// Throw ArgumentException
		/// </summary>
		public static string ThrowArgument( string providerSection, string tag )
		{
			throw new ArgumentException( GetReport( providerSection, tag ) );
		}

		/// <summary>
		/// Throw ArgumentNullException
		/// </summary>
		public static string ThrowArgumentNull( string providerSection, string tag )
		{
			throw new ArgumentNullException( GetReport( providerSection, tag ) );
		}

		/// <summary>
		/// Throw NotSupportedException
		/// </summary>
		public static string ThrowNotSupported( string providerSection, string tag )
		{
			throw new NotSupportedException( GetReport( providerSection, tag ) );
		}

		/// <summary>
		/// Throw ProviderException
		/// </summary>
		public static string ThrowProvider( string providerSection, string tag )
		{
			throw new ProviderException( GetReport( providerSection, tag ) );
		}
	}
}
