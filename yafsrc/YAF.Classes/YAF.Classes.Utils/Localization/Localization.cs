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
  public class YafLocalization
  {
    private Localizer _localizer = null;
    private Localizer _defaultLocale = null;
    private string _transPage = null;

    public YafLocalization()
    {

    }

    public YafLocalization( string transPage )
      : this()
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
        return _transPage;

        //throw new ApplicationException( string.Format( "Missing TransPage property for {0}", GetType() ) );
      }
      set
      {
        _transPage = value;
      }
    }

    public string LanguageCode
    {
      get
      {
        if ( _localizer != null )
          return _localizer.LanguageCode;

        return LoadTranslation();
      }
    }

    public string GetText( string text )
    {
      return GetText( TransPage, text );
    }

    private string LoadTranslation()
    {
      if ( _localizer != null )
        return _localizer.LanguageCode;

      string filename = null;

      if ( YafContext.Current.PageIsNull() || YafContext.Current.Page ["LanguageFile"] == DBNull.Value || !YafContext.Current.BoardSettings.AllowUserLanguage )
      {
        filename = YafContext.Current.BoardSettings.Language;
      }
      else
      {
        filename = YafContext.Current.LanguageFile;
      }

      if ( filename == null ) filename = "english.xml";

#if !DEBUG
      if ( _localizer == null && HttpContext.Current.Cache ["Localizer." + filename] != null )
        _localizer = ( Localizer ) HttpContext.Current.Cache ["Localizer." + filename];
#endif
      if ( _localizer == null )
      {

        _localizer = new Localizer( HttpContext.Current.Server.MapPath( String.Format( "{0}languages/{1}", YafForumInfo.ForumRoot, filename ) ) );
#if !DEBUG
        HttpContext.Current.Cache ["Localizer." + filename] = _localizer;
#endif
      }
      // If not using default language load that too
      if ( filename.ToLower() != "english.xml" )
      {
#if !DEBUG
        if ( _defaultLocale == null && HttpContext.Current.Cache ["DefaultLocale"] != null )
          _defaultLocale = ( Localizer ) HttpContext.Current.Cache ["DefaultLocale"];
#endif

        if ( _defaultLocale == null )
        {
          _defaultLocale = new Localizer( HttpContext.Current.Server.MapPath( String.Format( "{0}languages/english.xml", YafForumInfo.ForumRoot ) ) );
#if !DEBUG
          HttpContext.Current.Cache ["DefaultLocale"] = _defaultLocale;
#endif
        }
      }

      return _localizer.LanguageCode;
    }

    public string GetText( string page, string tag )
    {
      LoadTranslation();
      string localizedText;

      _localizer.SetPage( page );
      _localizer.GetText( tag, out localizedText );

      // If not default language, try to use that instead
      if ( localizedText == null && _defaultLocale != null )
      {
        _defaultLocale.SetPage( page );
        _defaultLocale.GetText( tag, out localizedText );
        if ( localizedText != null ) localizedText = '[' + localizedText + ']';
      }

      if ( localizedText == null )
      {
#if !DEBUG
        string filename = string.Empty;

        if ( YafContext.Current.PageIsNull() ||
             YafContext.Current.LanguageFile == string.Empty ||
             !YafContext.Current.BoardSettings.AllowUserLanguage )
        {
          filename = YafContext.Current.BoardSettings.Language;
        }
        else
        {
          filename = YafContext.Current.LanguageFile;
        }

        if ( filename == string.Empty ) filename = "english.xml";

        HttpContext.Current.Cache.Remove( "Localizer." + filename );
#endif
        YAF.Classes.Data.DB.eventlog_create( YafContext.Current.PageUserID, page.ToLower() + ".ascx", String.Format( "Missing Translation For {1}.{0}", tag.ToUpper(), page.ToUpper() ), YAF.Classes.Data.EventLogTypes.Error );
        return String.Format( "[{1}.{0}]", tag.ToUpper(), page.ToUpper() ); ;
      }

      localizedText = localizedText.Replace( "[b]", "<b>" );
      localizedText = localizedText.Replace( "[/b]", "</b>" );
      return localizedText;
    }
  }
}
