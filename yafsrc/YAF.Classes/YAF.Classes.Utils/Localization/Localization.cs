/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2007 Jaben Cargman
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
using System.Text;
using System.Web;

namespace YAF.Classes.Utils
{
	public class yaf_Localization
	{
		private Localizer m_localizer = null;
		private Localizer m_defaultLocale = null;
		private string m_transPage = null;

		public yaf_Localization()
		{

		}

		public yaf_Localization( string transPage ) : this()
		{
			TransPage = transPage;
		}

		/// <summary>
		/// What section of the xml is used to translate this page
		/// </summary>
		public string TransPage
		{
			get
			{
				//if ( m_transPage != null )
				return m_transPage;

				//throw new ApplicationException( string.Format( "Missing TransPage property for {0}", GetType() ) );
			}
			set
			{
				m_transPage = value;
			}
		}

		public string LanguageCode
		{
			get
			{
				if ( m_localizer != null )
					return m_localizer.LanguageCode;

				return LoadTranslation();
			}
		}

		public string GetText( string text )
		{
			return GetText( TransPage, text );
		}

		private string LoadTranslation()
		{
			if ( m_localizer != null )
				return m_localizer.LanguageCode;

			string filename = null;

			if ( yaf_Context.Current.PageIsNull() || yaf_Context.Current.Page.IsLanguageFileNull() || !yaf_Context.Current.BoardSettings.AllowUserLanguage )
			{
				filename = yaf_Context.Current.BoardSettings.Language;
			}
			else
			{
				filename = yaf_Context.Current.Page.LanguageFile;
			}

			if ( filename == null ) filename = "english.xml";

#if !DEBUG
			if(m_localizer==null && HttpContext.Current.Cache["Localizer." + filename]!=null)
				m_localizer = (Localizer)HttpContext.Current.Cache["Localizer." + filename];
#endif
			if ( m_localizer == null )
			{

				m_localizer = new Localizer( HttpContext.Current.Server.MapPath( String.Format( "{0}languages/{1}", yaf_ForumInfo.ForumRoot, filename ) ) );
#if !DEBUG
				HttpContext.Current.Cache["Localizer." + filename] = m_localizer;
#endif
			}
			// If not using default language load that too
			if ( filename.ToLower() != "english.xml" )
			{
#if !DEBUG
				if(m_defaultLocale==null && HttpContext.Current.Cache["DefaultLocale"]!=null)
					m_defaultLocale = (Localizer)HttpContext.Current.Cache["DefaultLocale"];
#endif

				if ( m_defaultLocale == null )
				{
					m_defaultLocale = new Localizer( HttpContext.Current.Server.MapPath( String.Format( "{0}languages/english.xml", yaf_ForumInfo.ForumRoot ) ) );
#if !DEBUG
					HttpContext.Current.Cache["DefaultLocale"] = m_defaultLocale;
#endif
				}
			}

			return m_localizer.LanguageCode;
		}

		public string GetText( string page, string text )
		{
			LoadTranslation();

			string str = m_localizer.GetText( page, text );

			// If not default language, try to use that instead
			if ( str == null && m_defaultLocale != null )
			{
				str = m_defaultLocale.GetText( page, text );
				if ( str != null ) str = '[' + str + ']';
			}

			if ( str == null )
			{
#if !DEBUG
				string filename = null;

				if(m_pageinfo==null || m_pageinfo.IsNull("LanguageFile") || !BoardSettings.AllowUserLanguage)
					filename = BoardSettings.Language;
				else
					filename = (string)m_pageinfo["LanguageFile"];

				if(filename==null)
					filename = "english.xml";

				HttpContext.Current.Cache.Remove("Localizer." + filename);
#endif
				YAF.Classes.Data.DB.eventlog_create( yaf_Context.Current.Page.UserID, page.ToLower() + ".ascx", String.Format( "Missing Translation For {1}.{0}", text.ToUpper(), page.ToUpper() ), YAF.Classes.Data.EventLogTypes.Error );
				return String.Format( "[{1}.{0}]", text.ToUpper(), page.ToUpper() ); ;
			}

			str = str.Replace( "[b]", "<b>" );
			str = str.Replace( "[/b]", "</b>" );
			return str;
		}
	}
}
