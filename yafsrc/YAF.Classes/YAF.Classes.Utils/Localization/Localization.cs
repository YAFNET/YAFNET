/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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
using System.Text;
using System.Web;
using System.Globalization;
using System.Xml;

namespace YAF.Classes.Utils
{
  public class YafLocalization
  {
	  private CultureInfo _culture = null;
    private Localizer _localizer = null;
    private Localizer _defaultLocale = null;
    private string _transPage = null;

  	public bool TranslationLoaded
  	{
  		get
  		{
  			return ( _localizer != null );
  		}
  	}

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
		  if (_localizer != null) return _localizer.LanguageCode;

        return LoadTranslation();
      }
    }

	  public CultureInfo Culture
	  {
		  get
		  {
			  if (_culture != null)
				  return _culture;
			  else if (_localizer == null)
			  {
				  LoadTranslation();
				  return _culture;
			  }

			  // fall back to current culture if there is some error
			  return CultureInfo.CurrentCulture;
		  }
	  }

    public string GetText( string text )
    {
      return GetText( TransPage, text );
    }

		public string LoadTranslation( string fileName )
		{
			if ( _localizer != null )
				return _localizer.LanguageCode;

#if !DEBUG
			if ( _localizer == null && HttpContext.Current.Cache ["Localizer." + fileName] != null )
				_localizer = ( Localizer ) HttpContext.Current.Cache ["Localizer." + fileName];
#endif
			if ( _localizer == null )
			{

				_localizer = new Localizer( HttpContext.Current.Server.MapPath( String.Format( "{0}languages/{1}", YafForumInfo.ForumFileRoot, fileName ) ) );
#if !DEBUG
				HttpContext.Current.Cache ["Localizer." + fileName] = _localizer;
#endif
			}
			// If not using default language load that too
			if ( fileName.ToLower() != "english.xml" )
			{
#if !DEBUG
        if ( _defaultLocale == null && HttpContext.Current.Cache ["DefaultLocale"] != null )
          _defaultLocale = ( Localizer ) HttpContext.Current.Cache ["DefaultLocale"];
#endif

				if ( _defaultLocale == null )
				{
					_defaultLocale = new Localizer( HttpContext.Current.Server.MapPath( String.Format( "{0}languages/english.xml", YafForumInfo.ForumFileRoot ) ) );
#if !DEBUG
          HttpContext.Current.Cache ["DefaultLocale"] = _defaultLocale;
#endif
				}
			}

			try
			{
				// try to load culture info defined in localization file
				_culture = new CultureInfo( _localizer.LanguageCode );
			}
			catch
			{
				// if it's wrong, fall back to current culture
				_culture = CultureInfo.CurrentCulture;
			}

			return _localizer.LanguageCode;			
		}

    public string LoadTranslation()
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

    	return LoadTranslation( filename );
    }

		protected string GetLocalizedTextInternal( string page, string tag )
		{
			string localizedText;

      LoadTranslation();     

      _localizer.SetPage( page );
      _localizer.GetText( tag, out localizedText );

      // If not default language, try to use that instead
      if ( localizedText == null && _defaultLocale != null )
      {
        _defaultLocale.SetPage( page );
        _defaultLocale.GetText( tag, out localizedText );
        if ( localizedText != null ) localizedText = '[' + localizedText + ']';
      }

			return localizedText;
		}

		public List<XmlNode> GetNodesUsingQuery( string page, string tagQuery )
		{
			LoadTranslation();

			_localizer.SetPage( page );
			return _localizer.GetNodesUsingQuery( tagQuery );
		}

		public bool GetTextExists( string page, string tag )
		{
			return !String.IsNullOrEmpty( GetLocalizedTextInternal( page, tag ) );
		}

    public string GetText( string page, string tag )
    {
			string localizedText = GetLocalizedTextInternal( page, tag );

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
        return String.Format( "[{1}.{0}]", tag.ToUpper(), page.ToUpper() );
      }

      localizedText = localizedText.Replace( "[b]", "<b>" );
      localizedText = localizedText.Replace( "[/b]", "</b>" );
      return localizedText;
    }

	  /// <summary>
	  /// Formats string using current culture.
	  /// </summary>
	  /// <param name="format">Format string.</param>
	  /// <param name="args">Parameters used in format string.</param>
	  /// <returns>Formatted string.</returns>
	  /// <remarks>If current localization culture is neutral, it's not used in formatting.</remarks>
	  public string FormatString(string format, params object[] args)
	  {
		  if (Culture.IsNeutralCulture)
			  return String.Format(format, args);
		  else
			  return String.Format(Culture, format, args);
	  }

	  /// <summary>
	  /// Formats date using given formatting string and current culture.
	  /// </summary>
	  /// <param name="format">Format string.</param>
	  /// <param name="date">Date to format.</param>
	  /// <returns>Formatted string.</returns>
	  /// <remarks>If current localization culture is neutral, it's not used in formatting.</remarks>
	  public string FormatDateTime(string format, DateTime date)
	  {
		  if (Culture.IsNeutralCulture)
			  return date.ToString(format);
		  else
			  return date.ToString(format, Culture);
	  }
  }
}
