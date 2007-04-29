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
	public class yaf_Theme
	{
		private string themeFile = null;
		private XmlDocument themeXMLDoc = null;
		private bool logMissingThemeItem = false;

		public yaf_Theme()
		{

		}

		public yaf_Theme( string newThemeFile )
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
				return themeFile;
			}
			set
			{
				if (themeFile != value)
				{
					themeFile = value;
					themeXMLDoc = null;
				}
			}
		}

		public bool LogMissingThemeItem
		{
			get
			{
				return logMissingThemeItem;
			}
			set
			{
				logMissingThemeItem = value;
			}
		}

		private void LoadThemeFile()
		{
			if (ThemeFile != null)
			{
#if !DEBUG
				if (themeXMLDoc == null)
				{
					themeXMLDoc = (XmlDocument)System.Web.HttpContext.Current.Cache[ThemeFile];
				}
#endif
				if ( themeXMLDoc == null )
				{
					themeXMLDoc = new XmlDocument();
					themeXMLDoc.Load( System.Web.HttpContext.Current.Server.MapPath( String.Format( "{0}themes/{1}", yaf_ForumInfo.ForumRoot, ThemeFile ) ) );
#if !DEBUG
					System.Web.HttpContext.Current.Cache[ThemeFile] = themeXMLDoc;
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

			if (themeXMLDoc != null)
			{
				string themeDir = themeXMLDoc.DocumentElement.Attributes ["dir"].Value;
				string langCode = yaf_Context.Current.Localization.LanguageCode.ToUpper();
				string select = string.Format( "//page[@name='{0}']/Resource[@tag='{1}' and @language='{2}']", page.ToUpper(), tag.ToUpper(), langCode );

				XmlNode node = themeXMLDoc.SelectSingleNode( select );
				if ( node == null )
				{
					select = string.Format( "//page[@name='{0}']/Resource[@tag='{1}']", page.ToUpper(), tag.ToUpper() );
					node = themeXMLDoc.SelectSingleNode( select );
				}

				if ( node == null )
				{
					if ( LogMissingThemeItem ) YAF.Classes.Data.DB.eventlog_create( yaf_Context.Current.PageUserID, page.ToLower() + ".ascx", String.Format( "Missing Theme Item: {0}.{1}", page.ToUpper(), tag.ToUpper() ), YAF.Classes.Data.EventLogTypes.Error );
					return defaultValue;
				}

				item = node.InnerText.Replace( "~", String.Format( "{0}themes/{1}", yaf_ForumInfo.ForumRoot, themeDir ) );
			}

			return item;
		}

		public string ThemeDir
		{
			get
			{
				LoadThemeFile();
				return String.Format( "{0}themes/{1}/", yaf_ForumInfo.ForumRoot, themeXMLDoc.DocumentElement.Attributes ["dir"].Value );
			}
		}

		public string GetURLToResource( string resourceName )
		{
			return string.Format( "{1}resource.ashx?r={0}", resourceName, yaf_ForumInfo.ForumRoot );
		}
	}
}
