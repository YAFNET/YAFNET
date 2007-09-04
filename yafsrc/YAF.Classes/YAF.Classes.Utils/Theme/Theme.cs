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
using System.Xml;

namespace YAF.Classes.Utils
{
	public class YafTheme
	{
		private string _themeFile = null;
		private XmlDocument _themeXmlDoc = null;
		private bool _logMissingThemeItem = false;

		public YafTheme()
		{

		}

		public YafTheme( string newThemeFile )
		{
			ThemeFile = newThemeFile;
		}

		/// <summary>
		/// Get or Set the current Theme File (could use validation)
		/// </summary>
		public string ThemeFile
		{
			get
			{
				return _themeFile;
			}
			set
			{
				if (_themeFile != value)
				{
					_themeFile = value;
					_themeXmlDoc = null;
				}
			}
		}

		public bool LogMissingThemeItem
		{
			get
			{
				return _logMissingThemeItem;
			}
			set
			{
				_logMissingThemeItem = value;
			}
		}

		private void LoadThemeFile()
		{
			if (ThemeFile != null)
			{
#if !DEBUG
				if (_themeXmlDoc == null)
				{
					_themeXmlDoc = (XmlDocument)System.Web.HttpContext.Current.Cache[ThemeFile];
				}
#endif
				if ( _themeXmlDoc == null )
				{
					_themeXmlDoc = new XmlDocument();
					_themeXmlDoc.Load( System.Web.HttpContext.Current.Server.MapPath( String.Format( "{0}themes/{1}", YafForumInfo.ForumRoot, ThemeFile ) ) );
#if !DEBUG
					System.Web.HttpContext.Current.Cache[ThemeFile] = _themeXmlDoc;
#endif
				}
			}
		}

		public string GetItem( string page, string tag)
		{
			return GetItem(page,tag,String.Format( "[{0}.{1}]", page.ToUpper(), tag.ToUpper() ));
		}

		public string GetItem( string page, string tag, string defaultValue)
		{
			string item = "";

			LoadThemeFile();

			if (_themeXmlDoc != null)
			{
				string themeDir = _themeXmlDoc.DocumentElement.Attributes ["dir"].Value;
				string langCode = YafContext.Current.Localization.LanguageCode.ToUpper();
				string select = string.Format( "//page[@name='{0}']/Resource[@tag='{1}' and @language='{2}']", page.ToUpper(), tag.ToUpper(), langCode );

				XmlNode node = _themeXmlDoc.SelectSingleNode( select );
				if ( node == null )
				{
					select = string.Format( "//page[@name='{0}']/Resource[@tag='{1}']", page.ToUpper(), tag.ToUpper() );
					node = _themeXmlDoc.SelectSingleNode( select );
				}

				if ( node == null )
				{
					if ( LogMissingThemeItem ) YAF.Classes.Data.DB.eventlog_create( YafContext.Current.PageUserID, page.ToLower() + ".ascx", String.Format( "Missing Theme Item: {0}.{1}", page.ToUpper(), tag.ToUpper() ), YAF.Classes.Data.EventLogTypes.Error );
					return defaultValue;
				}

				item = node.InnerText.Replace( "~", String.Format( "{0}themes/{1}", YafForumInfo.ForumRoot, themeDir ) );
			}

			return item;
		}

		public string ThemeDir
		{
			get
			{
				LoadThemeFile();
				return String.Format( "{0}themes/{1}/", YafForumInfo.ForumRoot, _themeXmlDoc.DocumentElement.Attributes ["dir"].Value );
			}
		}

		public string GetURLToResource( string resourceName )
		{
      return string.Format( "{1}resources/{0}", resourceName, YafForumInfo.ForumRoot );
			//return string.Format( "{1}resource.ashx?r={0}", resourceName, YafForumInfo.ForumRoot );
		}
	}
}
